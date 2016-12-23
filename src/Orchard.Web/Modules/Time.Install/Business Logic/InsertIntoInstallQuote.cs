using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Install;
using Time.Install.ViewModels;

namespace Time.Install.Business_Logic
{
    public class InsertIntoInstallQuote
    {
        public static void InsertData(QuoteViewModel vm, VSWQuotesEntities dbQ, EpicorInstallEntities dbE)
        {
            // Getting the hourly rate for the labor and paint
            var rate = dbQ.InstallHourlyRates;
            decimal hourRate = 0;
            decimal paintRate = 0;
            foreach (var item in rate)
            {
                if (item.RateType == "Hour Rate") hourRate = item.Rate;
                else paintRate = item.Rate;
            }
            // Getting the labor hours for the lift
            var laborHoursLift = dbQ.LiftFamilies.SingleOrDefault(x => x.Id == vm.LiftFamilyId);
            // Calculating the totals for Labor and Materials
            decimal totalPriceMaterial = 0;
            decimal totalHours = laborHoursLift.InstallHours; // totalHours is initialized with the labor hours of the lift
            decimal totalPriceLabor = 0;
            decimal totalPaintHours = 0;
            // Adding the Time options labor hours
            foreach (var item in vm.AerialOptions.Where(x => x.Hours > 0))
            {
                totalHours += item.Hours;
            }
            totalPriceLabor = (totalHours * hourRate); // Adding the cost of the lift hours(lift and aerial options) to the labor cost
            // Processing VSW options
            foreach (var item in vm.GroupAndOptions.Options.Where(x => x.Quantity > 0))
            {
                //totalPriceLabor += ((item.LaborHours * item.Quantity) * hourRate);
                totalPriceMaterial += (item.Price * item.Quantity);
                if (item.OptionName.Contains("PAINT"))
                {
                    totalPriceLabor += ((item.InstallHours * item.Quantity) * paintRate);
                    totalPaintHours += (item.InstallHours * item.Quantity);
                }
                else
                {
                    totalPriceLabor += ((item.InstallHours * item.Quantity) * hourRate);
                    totalHours += (item.InstallHours * item.Quantity);
                }
            }
            // Processing VSW manually added options
            foreach (var item in vm.AddOptnMnlly.Where(x => x.AddQuantityManually > 0))
            {

                totalPriceMaterial += (item.AddPriceManually * item.AddQuantityManually);
                if (item.AddPaintFlagManually == true)
                {
                    totalPriceLabor += ((item.AddInstallHoursManually * item.AddQuantityManually) * paintRate);
                    totalPaintHours += (item.AddInstallHoursManually * item.AddQuantityManually);
                }
                else
                {
                    totalPriceLabor += ((item.AddInstallHoursManually * item.AddQuantityManually) * hourRate);
                    totalHours += (item.AddInstallHoursManually * item.AddQuantityManually);
                }
            }

            // Saving the install quote to the db
            var liftName = dbE.QuoteDtls.SingleOrDefault(x => x.QuoteNum == vm.QuoteNum && x.QuoteLine == 1);
            InstallQuote installQ = new InstallQuote
            {
                LiftName = liftName.PartNum,
                LiftQuoteNumber = vm.QuoteNum,
                LiftQuoteLine = liftName.QuoteLine,
                LiftInstallLine = 2,
                InstallQuotedBy = HttpContext.Current.User.Identity.Name,
                QuoteDate = DateTime.Now,
                TotalPriceLabor = totalPriceLabor,
                TotalPriceMaterial = totalPriceMaterial,
                TotalInstallHours = totalHours,
                TotalPaintHours = totalPaintHours,
                LiftFamilyId = vm.LiftFamilyId
            };

            if(vm.EditQuote)
            {
                // Variables needed to perform the update
                var instQt = dbQ.InstallQuotes.FirstOrDefault(x => x.LiftQuoteNumber == vm.QuoteNum);
                dbQ.InstallQuotes.Attach(instQt);
                var entry = dbQ.Entry(instQt);
                // Updating the fields
                instQt.InstallQuotedBy = HttpContext.Current.User.Identity.Name;
                instQt.QuoteDate = DateTime.Now;
                instQt.TotalPriceLabor = totalPriceLabor;
                instQt.TotalPriceMaterial = totalPriceMaterial;
                instQt.TotalInstallHours = totalHours;
                instQt.TotalPaintHours = totalPaintHours;
                entry.Property(e => e.InstallQuotedBy).IsModified = true;
                entry.Property(e => e.QuoteDate).IsModified = true;
                entry.Property(e => e.TotalPriceLabor).IsModified = true;
                entry.Property(e => e.TotalPriceMaterial).IsModified = true;
                entry.Property(e => e.TotalInstallHours).IsModified = true;
                entry.Property(e => e.TotalPaintHours).IsModified = true;
            }
            else
            {
                // Adding a new Install quote
                dbQ.InstallQuotes.Add(installQ);
            }
            dbQ.SaveChanges();
        }
    }
}