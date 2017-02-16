using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Install;
using Time.Install.ViewModels;

namespace Time.Install.Business_Logic
{
    public class InsertIntoInstallDetailsAndManuallyAddedOptions
    {
        // Saving the install quote details to the db
        public static void InsertData(QuoteViewModel vm, VSWQuotesEntities dbQ)
        {
            var installQid = dbQ.InstallQuotes.Where(x => x.LiftQuoteNumber == vm.QuoteNum).Select(x => new { installId = x.Id }).Single();
            foreach (var item in vm.GroupAndOptions.Options.Where(x => x.Quantity > 0))// VSW Options in the db
            {
                InstallDetail installD = new InstallDetail
                {
                    InstallQuoteId = installQid.installId,
                    GroupId = item.GroupId,
                    OptionId = item.Id,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    InstallHours = (item.InstallHours * item.Quantity)
                };
                dbQ.InstallDetails.Add(installD);
            }
            foreach (var item in vm.AddOptnMnlly.Where(x => x.AddQuantityManually > 0))// VSW Options added manually
            {
                InstallDetailsManuallyAddedOption installDM = new InstallDetailsManuallyAddedOption
                {
                    InstallQuoteId = installQid.installId,
                    OptionName = item.AddOptionManually,
                    Quantity = item.AddQuantityManually,
                    Price = item.AddPriceManually,
                    InstallHours = (item.AddInstallHoursManually * item.AddQuantityManually),
                    PaintFlag = item.AddPaintFlagManually
                };
                dbQ.InstallDetailsManuallyAddedOptions.Add(installDM);
            }
            dbQ.SaveChanges();
        }
    }
}