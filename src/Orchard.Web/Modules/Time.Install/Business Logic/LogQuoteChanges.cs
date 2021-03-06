﻿using System;
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
            List<int> ids = new List<int>();// To store the ids of the options that qty was set to zero

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
                            if(opt.Quantity > 0)// Update the row only if qty is 1 or more else delete it
                            {
                                instDtls.Quantity = opt.Quantity;
                                entry.Property(e => e.Quantity).IsModified = true;
                            }
                            else
                            {
                                ids.Add(item.Id);// Storing the Ids of the options that went from qty greater than 0 to qty of 0
                            }
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
            // Deleting the options with qty of zero form the InstallDetail table
            foreach (var item in ids)
            {
                var instDtls = dbQ.InstallDetails.FirstOrDefault(x => x.Id == item);
                dbQ.InstallDetails.Attach(instDtls);
                var entry = dbQ.Entry(instDtls);
                entry.State = EntityState.Deleted;
            }
            dbQ.SaveChanges();
            // Checking for new options added
            foreach (var item in vm.GroupAndOptions.Options.Where(x => x.Quantity > 0))
            {
                var vswOption = dbQ.VSWOptions.SingleOrDefault(x => x.OptionName == item.OptionName && x.GroupId == item.GroupId && x.LiftFamilyId == item.LiftFamilyId);
                var inDetail = dbQ.InstallDetails.SingleOrDefault(x => x.OptionId == vswOption.Id && x.InstallQuoteId == installQuote.Id);
                if (inDetail == null)
                {
                    QuoteChangesLog chLog = new QuoteChangesLog // Adding the new row to the log
                    {
                        InstallQuoteId = installQuote.Id,
                        UpdatedBy = HttpContext.Current.User.Identity.Name,
                        UpdatedOn = DateTime.Now,
                        ValueChanged = item.OptionName + " was added"
                    };
                    dbQ.QuoteChangesLogs.Add(chLog);

                    InstallDetail inDt = new InstallDetail // Adding the new row to the InstallDetail table
                    {
                        InstallQuoteId = installQuote.Id,
                        GroupId = item.GroupId,
                        OptionId = item.Id,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        InstallHours = item.InstallHours
                    };
                    dbQ.InstallDetails.Add(inDt);
                }
            }

            // Resetting the variables
            ids.Clear();
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
                            if(opt.AddQuantityManually > 0)// Update the row only if qty is 1 or more else delete it
                            {
                                instDtlsMnAd.Quantity = opt.AddQuantityManually;
                                entry.Property(e => e.Quantity).IsModified = true;
                            }
                            else
                            {
                                ids.Add(item.Id);// Storing the Ids of the options that went from qty greater than 0 to qty of 0
                            }
                        }
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
            // Deleting the options with qty of zero form the InstallDetailsManuallyAddedOption table
            foreach (var id in ids)
            {
                var instDtlsMnAd = dbQ.InstallDetailsManuallyAddedOptions.FirstOrDefault(x => x.Id == id);
                dbQ.InstallDetailsManuallyAddedOptions.Attach(instDtlsMnAd);
                var entry = dbQ.Entry(instDtlsMnAd);
                entry.State = EntityState.Deleted;
            }
            // Checking for new option added manually
            foreach (var item in vm.AddOptnMnlly.Where(x => x.AddQuantityManually > 0))
            {
                var inDetail = dbQ.InstallDetailsManuallyAddedOptions.SingleOrDefault(x => x.OptionName == item.AddOptionManually && x.InstallQuoteId == installQuote.Id);
                if (inDetail == null)
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
                    InstallDetailsManuallyAddedOption inDtMnAd = new InstallDetailsManuallyAddedOption
                    {
                        InstallQuoteId = installQuote.Id,
                        OptionName = item.AddOptionManually,
                        Quantity = item.AddQuantityManually,
                        Price = item.AddPriceManually,
                        InstallHours = item.AddInstallHoursManually,
                        PaintFlag = item.AddPaintFlagManually
                    };
                    dbQ.InstallDetailsManuallyAddedOptions.Add(inDtMnAd);
                }
            }
            dbQ.SaveChanges();
        }
    }
}