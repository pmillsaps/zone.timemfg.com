using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Install;
using Time.Install.Models;
using Time.Install.ViewModels;

namespace Time.Install.Business_Logic
{
    public class LoadOptionsForQuote
    {
        // This class takes a QuoteViewModel and loads the Time and VSW options.
        // The user can then start a new quote or update an existing one. 
        public static QuoteViewModel GetOptions(QuoteViewModel quoteVM, EpicorInstallEntities dbE, VSWQuotesEntities dbQ)
        {
            QuoteViewModel qVM = quoteVM;
            var aerialOp = dbE.QuoteDtls.FirstOrDefault(x => x.QuoteNum == qVM.QuoteNum && x.QuoteLine == 1);
            qVM.GroupAndOptions.OptionGroups = dbQ.OptionGroups.ToList();// Added this line to simplify the view
            //qVM.AerialOptions = aQuote(aerialOp.QuoteComment, qVM.LiftFamilyId);
            qVM.AerialOptions = ParsingQuoteComments.GetAerialOptions(aerialOp.QuoteComment, qVM.LiftFamilyId, dbQ);
            qVM.GroupAndOptions.Options = dbQ.VSWOptions.Where(x => x.LiftFamilyId == qVM.LiftFamilyId).ToList();// Added this line to simplify the view
            qVM.AddOptnMnlly = new List<AddVSWOptionManually>();

            if (qVM.EditQuote)
            {
                var installQuote = dbQ.InstallQuotes.SingleOrDefault(x => x.LiftQuoteNumber == qVM.QuoteNum);
                var installDetail = dbQ.InstallDetails.Include("VSWOption").Where(x => x.InstallQuoteId == installQuote.Id).ToList();
                // Loading the existing lines for the Install details
                foreach (var item in installDetail)
                {
                    foreach (var opt in qVM.GroupAndOptions.Options)// Added the GroupsAndOptions view model to simplify the view
                    {
                        if (item.VSWOption.OptionName == opt.OptionName)
                        {
                            opt.Quantity = item.Quantity;
                            opt.Price = item.Price;
                            opt.InstallHours = item.InstallHours;
                        }
                    }
                }
                // Loading the existing manually added lines for the Install details
                var installDetailsMnllyAdded = dbQ.InstallDetailsManuallyAddedOptions.Where(x => x.InstallQuoteId == installQuote.Id).ToList();
                foreach (var item in installDetailsMnllyAdded)
                {
                    AddVSWOptionManually addMnlly = new AddVSWOptionManually
                    {
                        AddOptionManually = item.OptionName,
                        AddQuantityManually = item.Quantity,
                        AddPriceManually = item.Price,
                        AddInstallHoursManually = item.InstallHours,
                        AddPaintFlagManually = item.PaintFlag
                    };
                    qVM.AddOptnMnlly.Add(addMnlly);
                }
            }
            return qVM;
        }
    }
}