using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Install;
using Time.Install.Business_Logic;
using Time.Install.Models;
using Time.Install.ViewModels;

namespace Time.Install.Controllers
{
    [Themed]
    [Authorize]
    public class QuoteController : Controller
    {
        private EpicorInstallEntities dbE = new EpicorInstallEntities();
        private VSWQuotesEntities dbQ = new VSWQuotesEntities();

        private string ErrorMessage { get; set; }
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public QuoteController(IOrchardServices services)
        {
            Services = services;
        }

        public ActionResult Index(int? quote)
        {
            if (quote != null)
            {
                // This line is for testing 
                var quotes = dbE.QuoteDtls.Where(x => x.QuoteLine == 2 && x.QuoteNum == quote).OrderByDescending(x => x.QuoteNum).ToList();

                // Uncomment this line when going live
                //var quotes = dbE.QuoteDtls.Where(x => x.QuoteLine == 2 && x.QuoteNum == quote && x.QuoteComment == "").OrderByDescending(x => x.QuoteNum).ToList();
                return View(quotes);
            }
            else
            {
                // This line is for testing
                var quotes = dbE.QuoteDtls.Where(x => x.QuoteLine == 2).OrderByDescending(x => x.QuoteNum).ToList();

                // Uncomment this line when going live
                //var quotes = dbE.QuoteDtls.Where(x => x.QuoteLine == 2 && x.QuoteComment == "").OrderByDescending(x => x.QuoteNum).ToList();
                return View(quotes);
            }
        }

        // List of VSW Install quotes
        public ActionResult ListOfInstallQuotes()
        {
            return View(dbQ.InstallQuotes.ToList());
        }

        // Start the Install Quote
        [HttpGet]
        public ActionResult StartQuote(int quoteNum, string installDesc, int? alreadyExist, bool editQuote, int? liftFamilyId)
        {
            var vm = new QuoteViewModel();
            vm.QuoteNum = quoteNum;
            vm.InstallDescr = installDesc;
            vm.EditQuote = editQuote;
            ViewBag.AlreadyExist = alreadyExist;
            ViewBag.AerialOptions = dbE.QuoteDtls.FirstOrDefault(x => x.QuoteNum == quoteNum && x.QuoteComment != "");
            if (editQuote) ViewBag.LiftFamilyId = new SelectList(dbQ.LiftFamilies.OrderBy(x => x.FamilyName), "Id", "FamilyName", liftFamilyId);
            else ViewBag.LiftFamilyId = new SelectList(dbQ.LiftFamilies.OrderBy(x => x.FamilyName), "Id", "FamilyName");
            return View(vm);
        }

        // Loading the options for the Install Quote
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuote(QuoteViewModel quoteVM)
        {
            return View(LoadOptionsForQuote.GetOptions(quoteVM, dbE, dbQ));
        }

        // Saving the Install Quote to the db
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuoteConfirmed(QuoteViewModel vm)
        {
            // Checking for existing install quote
            var installQuoteExists = dbQ.InstallQuotes.FirstOrDefault(x => x.LiftQuoteNumber == vm.QuoteNum);
            if (installQuoteExists != null && vm.EditQuote == false)
            {
                return RedirectToAction("StartQuote", new { quoteNum = vm.QuoteNum, installDesc = vm.InstallDescr, alreadyExist = 1 });
            }
            else
            {
                // Logging the changes when updating the installation quote
                if (vm.EditQuote) LogQuoteChanges.LogInstallQuotesChanges(vm, dbQ);
                // Inserting the data into InstallQuote table
                InsertIntoInstallQuote.InsertData(vm, dbQ, dbE);
                // Inserting data into the InstallDetails tables
                if(!vm.EditQuote) InsertIntoInstallDetailsAndManuallyAddedOptions.InsertData(vm, dbQ );
                // Inserting the Install details into the Epicor db
                var installQid = dbQ.InstallQuotes.Where(x => x.LiftQuoteNumber == vm.QuoteNum).Select(x => new { installId = x.Id }).Single();
                InsertInstallDetailsIntoEpicorDb.InsertData(installQid.installId, dbQ, dbE);

                return RedirectToAction("QuoteSummary", new { installQuoteId = installQid.installId, quoteNum = vm.QuoteNum, liftFamilyId = vm.LiftFamilyId });
            }
        }

        // Displaying a summary of the install quote
        [HttpGet]
        public ActionResult QuoteSummary(int installQuoteId, int quoteNum, int liftFamilyId)
        {
            // Loading the install quote info
            InstallQuoteSummary installQuoteSummary = new InstallQuoteSummary();
            installQuoteSummary.InstallQuotes = dbQ.InstallQuotes.SingleOrDefault(x => x.Id == installQuoteId);
            installQuoteSummary.OptionGroups = dbQ.OptionGroups.ToList();
            installQuoteSummary.InstallDetails = dbQ.InstallDetails.Where(x => x.InstallQuoteId == installQuoteId).ToList();
            installQuoteSummary.InstallDetailsMnllyAdded = dbQ.InstallDetailsManuallyAddedOptions.Where(x => x.InstallQuoteId == installQuoteId).ToList();
            var aerialOp = dbE.QuoteDtls.FirstOrDefault(x => x.QuoteNum == quoteNum && x.QuoteLine == 1);
            installQuoteSummary.AerialOptions = ParsingQuoteComments.GetAerialOptions(aerialOp.QuoteComment, liftFamilyId, dbQ);

            return View(installQuoteSummary);
        }
    }
}