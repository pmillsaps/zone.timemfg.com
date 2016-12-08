using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Install;
using Time.Install.ViewModels;

namespace Time.Install.Business_Logic
{
    public class LogQuoteChanges
    {
        // Logging the changes of Install Quote when updated
        public static void LogInstallQuotesChanges(QuoteViewModel vm, VSWQuotesEntities dbQ)
        {
            string changes = "";
            bool optionUpdated = false;
            var installQuote = dbQ.InstallQuotes.SingleOrDefault(x => x.LiftQuoteNumber == vm.QuoteNum);
            var installDetail = dbQ.InstallDetails.Include("VSWOption").Where(x => x.InstallQuoteId == installQuote.Id).ToList();

            // Looping trough existing lines in InstallDetails to log changes
            foreach (var item in installDetail)
            {
                foreach (var opt in vm.GroupAndOptions.Options)
                {
                    if (item.VSWOption.OptionName == opt.OptionName)
                    {
                        // Variables needed to perform the update in the InstallDetail table
                        var instDtls = dbQ.InstallDetails.FirstOrDefault(x => x.Id == item.Id);
                        dbQ.InstallDetails.Attach(instDtls);
                        var entry = dbQ.Entry(instDtls);
                        changes = opt.OptionName + " "; // This is for the QuoteChangesLog table

                        if (opt.Quantity != item.Quantity)// If quantity changes
                        {
                            changes += "-- Quantity went from " + item.Quantity.ToString() + " to " + opt.Quantity.ToString();
                            optionUpdated = true;
                            instDtls.Quantity = opt.Quantity;
                            entry.Property(e => e.Quantity).IsModified = true;
                        }
                        if (opt.Price != item.Price && opt.Quantity > 0)// If price changes
                        {
                            changes += " -- Price went from " + item.Price.ToString("c") + " to " + opt.Price.ToString("c");
                            optionUpdated = true;
                            instDtls.Price = opt.Price;
                            entry.Property(e => e.Price).IsModified = true;
                        }
                        if (opt.InstallHours != item.InstallHours && opt.Quantity > 0)// If hours change
                        {
                            changes += " -- InstallHours went from " + item.InstallHours.ToString() + " to " + opt.InstallHours.ToString();
                            optionUpdated = true;
                            instDtls.InstallHours = opt.InstallHours;
                            entry.Property(e => e.InstallHours).IsModified = true;
                        }
                        if (optionUpdated)
                        {
                            QuoteChangesLog chLog = new QuoteChangesLog // Logging changes
                            {
                                InstallQuoteId = installQuote.Id,
                                UpdatedBy = HttpContext.Current.User.Identity.Name,// Added "Current" to use HttpContext in this static method.
                                UpdatedOn = DateTime.Now,
                                ValueChanged = changes
                            };
                            dbQ.QuoteChangesLogs.Add(chLog);
                        }
                    }
                    optionUpdated = false;
                }
                dbQ.SaveChanges();
            }
            // Checking for new options added
            foreach (var item in vm.GroupAndOptions.Options.Where(x => x.Quantity > 0))
            {
                var vswOption = dbQ.VSWOptions.SingleOrDefault(x => x.OptionName == item.OptionName && x.GroupId == item.GroupId && x.LiftFamilyId == item.LiftFamilyId);
                var iDetail = dbQ.InstallDetails.SingleOrDefault(x => x.OptionId == vswOption.Id && x.InstallQuoteId == installQuote.Id);
                if (iDetail == null)
                {
                    QuoteChangesLog chLog = new QuoteChangesLog // Adding the new row to the log
                    {
                        InstallQuoteId = installQuote.Id,
                        UpdatedBy = HttpContext.Current.User.Identity.Name,
                        UpdatedOn = DateTime.Now,
                        ValueChanged = item.OptionName + " was added"
                    };
                    dbQ.QuoteChangesLogs.Add(chLog);

                    InstallDetail iDt = new InstallDetail // Adding the new row to the InstallDetail table
                    {
                        InstallQuoteId = installQuote.Id,
                        GroupId = item.GroupId,
                        OptionId = item.Id,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        InstallHours = item.InstallHours
                    };
                    dbQ.InstallDetails.Add(iDt);
                }
            }

            // Resetting the variables
            changes = "";
            optionUpdated = false;
            var installDetailManuallyAdded = dbQ.InstallDetailsManuallyAddedOptions.Where(x => x.InstallQuoteId == installQuote.Id).ToList();
            // Looping trough existing lines in InstallDetailsManuallyAddedOption to log changes
            foreach (var item in installDetailManuallyAdded)
            {
                foreach (var opt in vm.AddOptnMnlly)
                {
                    if (item.OptionName == opt.AddOptionManually)
                    {
                        // Variables needed to perform the update in the InstallDetailsManuallyAddedOption table
                        var instDtlsMnAd = dbQ.InstallDetailsManuallyAddedOptions.FirstOrDefault(x => x.Id == item.Id);
                        dbQ.InstallDetailsManuallyAddedOptions.Attach(instDtlsMnAd);
                        var entry = dbQ.Entry(instDtlsMnAd);
                        changes = opt.AddOptionManually + " "; // This is for the QuoteChangesLog table

                        if (opt.AddQuantityManually != item.Quantity)// If quantity changes
                        {
                            changes += "-- Quantity went from " + item.Quantity.ToString() + " to " + opt.AddQuantityManually.ToString();
                            optionUpdated = true;
                            instDtlsMnAd.Quantity = opt.AddQuantityManually;
                            entry.Property(e => e.Quantity).IsModified = true;
                        }
                        //if (opt.AddOptionManually != item.OptionName && opt.AddQuantityManually > 0)// If name changes
                        //{
                        //    changes += " -- Option went from " + item.OptionName + " to " + opt.AddOptionManually;
                        //    optionUpdated = true;
                        //    instDtlsMnAd.OptionName = opt.AddOptionManually;
                        //    entry.Property(e => e.OptionName).IsModified = true;
                        //}
                        if (opt.AddPriceManually != item.Price && opt.AddQuantityManually > 0)// If price changes
                        {
                            changes += " -- Price went from " + item.Price.ToString("c") + " to " + opt.AddPriceManually.ToString("c");
                            optionUpdated = true;
                            instDtlsMnAd.Price = opt.AddPriceManually;
                            entry.Property(e => e.Price).IsModified = true;
                        }
                        if (opt.AddInstallHoursManually != item.InstallHours && opt.AddQuantityManually > 0)// If install hours change
                        {
                            changes += " -- InstallHours went from " + item.InstallHours.ToString() + " to " + opt.AddInstallHoursManually.ToString();
                            optionUpdated = true;
                            instDtlsMnAd.InstallHours = opt.AddInstallHoursManually;
                            entry.Property(e => e.InstallHours).IsModified = true;
                        }
                        if (optionUpdated)
                        {
                            QuoteChangesLog chLog = new QuoteChangesLog // Logging changes
                            {
                                InstallQuoteId = installQuote.Id,
                                UpdatedBy = HttpContext.Current.User.Identity.Name,// Added "Current" to use HttpContext in this static method.
                                UpdatedOn = DateTime.Now,
                                ValueChanged = changes
                            };
                            dbQ.QuoteChangesLogs.Add(chLog);
                        }
                        dbQ.SaveChanges();
                        break;
                        //optionUpdated = false;
                    }
                }
            }

            // Checking for new option added manually
            foreach (var item in vm.AddOptnMnlly.Where(x => x.AddQuantityManually > 0))
            {
                var iDetail = dbQ.InstallDetailsManuallyAddedOptions.SingleOrDefault(x => x.OptionName == item.AddOptionManually && x.InstallQuoteId == installQuote.Id);
                if (iDetail == null)
                {
                    QuoteChangesLog chLog = new QuoteChangesLog// Adding the new row to the log
                    {
                        InstallQuoteId = installQuote.Id,
                        UpdatedBy = HttpContext.Current.User.Identity.Name,
                        UpdatedOn = DateTime.Now,
                        ValueChanged = item.AddOptionManually + " was added"
                    };
                    dbQ.QuoteChangesLogs.Add(chLog);
                    // Adding the new row to the InstallDetailsManuallyAddedOption table
                    InstallDetailsManuallyAddedOption iDtMnAd = new InstallDetailsManuallyAddedOption
                    {
                        InstallQuoteId = installQuote.Id,
                        OptionName = item.AddOptionManually,
                        Quantity = item.AddQuantityManually,
                        Price = item.AddPriceManually,
                        InstallHours = item.AddInstallHoursManually,
                        PaintFlag = item.AddPaintFlagManually
                    };
                    dbQ.InstallDetailsManuallyAddedOptions.Add(iDtMnAd);
                }
            }
            dbQ.SaveChanges();
        }
    }
}