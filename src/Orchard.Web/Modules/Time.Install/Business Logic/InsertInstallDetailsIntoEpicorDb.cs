using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Time.Data.EntityModels.Install;

namespace Time.Install.Business_Logic
{
    public class InsertInstallDetailsIntoEpicorDb
    {
        // Inserting the Install details into the Epicor db
        public static void InsertData(int installQuoteId, VSWQuotesEntities dbQ, EpicorInstallEntities dbE)
        {
            var installQ = dbQ.InstallQuotes.SingleOrDefault(x => x.Id == installQuoteId);
            var installDetails = dbQ.InstallDetails.Include("VSWOption").Where(x => x.InstallQuoteId == installQuoteId).ToList();
            var installManuallyAddedOpt = dbQ.InstallDetailsManuallyAddedOptions.Where(x => x.InstallQuoteId == installQuoteId).ToList();
            string line = "";
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("INSTALL DETAILS:");
            sb.AppendLine();
            sb.AppendLine("Quoted by: " + installQ.InstallQuotedBy);
            sb.AppendLine("Quote Date: " + installQ.QuoteDate.ToShortDateString());
            sb.AppendLine("Total Price Labor: " + installQ.TotalPriceLabor.ToString("c"));
            sb.AppendLine("Total Price Materials: " + installQ.TotalPriceMaterial.ToString("c"));
            sb.AppendLine("Total Install Price: " + installQ.TotalInstallPrice.Value.ToString("c"));
            sb.AppendLine("Installation Hours: " + installQ.TotalInstallHours);
            sb.AppendLine("Paint Hours: " + installQ.TotalPaintHours);
            sb.AppendLine();
            sb.AppendLine("Qty".PadLeft(3) + "Price".PadLeft(17) + "Install Hours".PadLeft(19) + "Extended Price".PadLeft(18) + "    VSW Option".PadRight(64));
            sb.AppendLine("----------------------------------------------------------------------------------------------------------------------------");

            foreach (var item in installDetails)
            {
                line += item.Quantity.ToString().PadLeft(3);
                line += item.Price.ToString("c").PadLeft(17);
                line += item.InstallHours.ToString().PadLeft(19);
                line += item.ExtendedPrice.Value.ToString("c").PadLeft(18);
                line += ("    " + item.VSWOption.OptionName.PadRight(64));
                sb.AppendLine(line);
                line = "";
            }
            sb.AppendLine();
            sb.AppendLine("Manually Added Options");
            sb.AppendLine();
            foreach (var item in installManuallyAddedOpt)
            {
                line += item.Quantity.ToString().PadLeft(3);
                line += item.Price.ToString("c").PadLeft(17);
                line += item.InstallHours.ToString().PadLeft(19);
                line += item.ExtendedPrice.Value.ToString("c").PadLeft(18);
                line += ("    " + item.OptionName.PadRight(64));
                sb.AppendLine(line);
                line = "";
            }

            var installQouteEpicor = dbE.QuoteDtls.SingleOrDefault(x => x.QuoteNum == installQ.LiftQuoteNumber && x.QuoteLine == 2);
            var installPriceEpicor = dbE.QuoteQties.SingleOrDefault(x => x.QuoteNum == installQ.LiftQuoteNumber && x.QuoteLine == 2);
            // Inserting the quote details into QuoteDtl table
            installQouteEpicor.QuoteComment = sb.ToString();
            dbE.QuoteDtls.Attach(installQouteEpicor);
            var entry = dbE.Entry(installQouteEpicor);
            entry.Property(x => x.QuoteComment).IsModified = true;
            // Inserting the install cost into QuoteQty
            installPriceEpicor.UnitPrice = installQ.TotalInstallPrice.Value;
            installPriceEpicor.DocUnitPrice = installQ.TotalInstallPrice.Value;
            dbE.QuoteQties.Attach(installPriceEpicor);
            var entry2 = dbE.Entry(installPriceEpicor);
            entry2.Property(x => x.UnitPrice).IsModified = true;
            entry2.Property(x => x.DocUnitPrice).IsModified = true;
            dbE.SaveChanges(); // Saving the changes
        }
    }
}