using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Install;
using Time.Install.ViewModels;

namespace Time.Install.Business_Logic
{
    public class LogQuoteChanges
    {
        // Logging the Install Quote changes when updated
        public static void LogInstallQuotesChanges(QuoteViewModel vm, VSWQuotesEntities dbQ)
        {
            string changes = "";
            bool optionUpdated = false;
            var installQuote = dbQ.InstallQuotes.SingleOrDefault(x => x.LiftQuoteNumber == vm.QuoteNum);
            var installDetail = dbQ.InstallDetails.Include("VSWOption").Where(x => x.InstallQuoteId == installQuote.Id).ToList();
            // Looping trough existing lines in Install details to log changes
            foreach (var item in installDetail)
            {
                foreach (var opt in vm.GroupAndOptions.Options)
                {
                    if (item.VSWOption.OptionName == opt.OptionName)
                    {
                        changes = opt.OptionName + " ";
                        if (opt.Quantity != item.Quantity)
                        {
                            changes += "-- Quantity went from " + item.Quantity.ToString() + " to " + opt.Quantity.ToString();
                            optionUpdated = true;
                        }
                        if (opt.Price != item.Price)
                        {
                            changes += " -- Price went from " + item.Price.ToString("c") + " to " + opt.Price.ToString("c");
                            optionUpdated = true;
                        }
                        if (opt.InstallHours != item.InstallHours)
                        {
                            changes += " -- InstallHours went from " + item.InstallHours.ToString() + " to " + opt.InstallHours.ToString();
                            optionUpdated = true;
                        }
                        if (optionUpdated)
                        {
                            QuoteChangesLog chLog = new QuoteChangesLog
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
            }
            // Checking for new option added to the Install quote
            foreach (var item in vm.GroupAndOptions.Options.Where(x => x.Quantity > 0))
            {
                var vswOption = dbQ.VSWOptions.SingleOrDefault(x => x.OptionName == item.OptionName && x.GroupId == item.GroupId && x.LiftFamilyId == item.LiftFamilyId);
                var iDetail = dbQ.InstallDetails.SingleOrDefault(x => x.OptionId == vswOption.Id && x.InstallQuoteId == installQuote.Id);
                if (iDetail == null)
                {
                    QuoteChangesLog chLog = new QuoteChangesLog
                    {
                        InstallQuoteId = installQuote.Id,
                        UpdatedBy = HttpContext.Current.User.Identity.Name,
                        UpdatedOn = DateTime.Now,
                        ValueChanged = item.OptionName + " was added"
                    };
                    dbQ.QuoteChangesLogs.Add(chLog);
                }
            }
            foreach (var item in vm.AddOptnMnlly.Where(x => x.AddQuantityManually > 0))
            {
                var iDetail = dbQ.InstallDetailsManuallyAddedOptions.SingleOrDefault(x => x.OptionName == item.AddOptionManually && x.InstallQuoteId == installQuote.Id);
                if (iDetail == null)
                {
                    QuoteChangesLog chLog = new QuoteChangesLog
                    {
                        InstallQuoteId = installQuote.Id,
                        UpdatedBy = HttpContext.Current.User.Identity.Name,
                        UpdatedOn = DateTime.Now,
                        ValueChanged = item.AddOptionManually + " was added"
                    };
                    dbQ.QuoteChangesLogs.Add(chLog);
                }
            }
            dbQ.SaveChanges();
        }
    }
}