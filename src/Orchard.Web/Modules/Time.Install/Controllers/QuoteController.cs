using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ActionResult Index()
        {
            return View();
        }

        // Load the data for the Index table
        public ActionResult LoadQuotes()
        {
            List<LoadAerialQuotes> model = new List<LoadAerialQuotes>();
            // This line is for testing
            var quotes = dbE.QuoteDtls.Where(x => x.PartNum == "INSTALLS").ToList();

            // Uncomment this lines when going live
            //var quotes = dbE.QuoteDtls.Where(x => x.PartNum == "INSTALLS" && x.QuoteComment == "").ToList();
            // Retrieving the LiftFamilyId
            //var cfgName = dbE.PartRevs.FirstOrDefault(x => x.PartNum == quoteDtls.PartNum);
            //var liftFmly = dbQ.LiftFamilies.FirstOrDefault(x => x.FamilyName == cfgName.ConfigID);
            //quoteVM.LiftFamilyId = liftFmly.Id;

            foreach (var item in quotes)
            {
                LoadAerialQuotes qvm = new LoadAerialQuotes();
                qvm.QuoteNum = item.QuoteNum;
                qvm.QuoteLine = item.QuoteLine;
                qvm.PartNum = item.PartNum;
                qvm.LineDesc = item.LineDesc;
                qvm.LastUpdate = (item.LastUpdate != null) ? item.LastUpdate.Value.ToShortDateString() : "";
                qvm.LastDcdUserId = item.LastDcdUserID;
                qvm.OrderQty = item.OrderQty;
                qvm.ChangedBy = item.ChangedBy;
                qvm.ChangeDate = (item.ChangeDate != null) ? item.ChangeDate.Value.ToShortDateString() : "";
                var quoteDtls = dbE.QuoteDtls.FirstOrDefault(x => x.QuoteNum == item.QuoteNum && x.QuoteLine == 1);
                var cfgName = dbE.PartRevs.FirstOrDefault(x => x.PartNum == quoteDtls.PartNum);
                if (cfgName != null)
                {
                    var liftFmly = dbQ.LiftFamilies.FirstOrDefault(x => x.FamilyName == cfgName.ConfigID);
                    qvm.LiftFamilyId = (liftFmly != null) ? liftFmly.Id : 0;
                }
                else
                {
                    qvm.LiftFamilyId = 0;
                }
                model.Add(qvm);
            }
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        // List of VSW Install quotes
        public ActionResult ListOfInstallQuotes()
        {
            return View();
        }

        // Load the data for the Index table
        public ActionResult LoadInstallationQuotes()
        {
            List<LoadInstallQuotes> model = new List<LoadInstallQuotes>();
            var quotes = dbQ.InstallQuotes.ToList();
            foreach (var item in quotes)
            {
                LoadInstallQuotes qvm = new LoadInstallQuotes();
                qvm.Id = item.Id;
                qvm.LiftName = item.LiftName;
                qvm.LiftQuoteNumber = item.LiftQuoteNumber;
                qvm.LiftQuoteLine = item.LiftQuoteLine;
                qvm.LiftInstallLine = item.LiftInstallLine;
                qvm.InstallQuotedBy = item.InstallQuotedBy;
                qvm.QuoteDate = (item.QuoteDate != null) ? item.QuoteDate.ToShortDateString() : "";
                qvm.TotalPriceLabor = item.TotalPriceLabor;
                qvm.TotalPriceMaterial = item.TotalPriceMaterial;
                qvm.TotalInstallPrice = (item.TotalInstallPrice != null) ? item.TotalInstallPrice.Value : 0;
                qvm.TotalInstallHours = item.TotalInstallHours;
                qvm.TotalPaintHours = item.TotalPaintHours;
                qvm.LiftFamilyId = item.LiftFamilyId;
                model.Add(qvm);
            }
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        // Loading the options for the Install Quote
        [HttpGet]
        public ActionResult AddQuote(int quoteNum, string installDesc, int? alreadyExist, bool editQuote, int? liftFamilyId)
        {
            var quoteVM = new QuoteViewModel();
            quoteVM.QuoteNum = quoteNum;
            quoteVM.InstallDescr = installDesc;
            quoteVM.EditQuote = editQuote;
            if (liftFamilyId == null)// Retrieving the LiftFamilyId used to load its VSW options
            {
                var quoteDtls = dbE.QuoteDtls.FirstOrDefault(x => x.QuoteNum == quoteNum && x.QuoteLine == 1);
                var cfgName = dbE.PartRevs.FirstOrDefault(x => x.PartNum == quoteDtls.PartNum);
                var liftFmly = dbQ.LiftFamilies.FirstOrDefault(x => x.FamilyName == cfgName.ConfigID);
                quoteVM.LiftFamilyId = liftFmly.Id;
            }
            else
            {
                quoteVM.LiftFamilyId = liftFamilyId.Value;
            }
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
                if (!vm.EditQuote) InsertIntoInstallDetailsAndManuallyAddedOptions.InsertData(vm, dbQ);
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

        // Method to display the options to generate the Word Document
        public ActionResult CreateQuotePresentation(int? id, string fileName, string userId, string liftFamilyId, string listOfOptions)
        {
            var options = dbQ.OptionTitlesForWordDocs.Where(x => x.LiftFamilyId == id).ToList();
            ViewBag.Users = new SelectList(dbQ.QuoteDeptUsersForWordDocs, "Id", "Name");
            return View(options);
        }

        // Method to generate the Word Document
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateDocument(string fileName, int? userId, int liftFamilyId, string listOfOptions)
        {
            if (String.IsNullOrEmpty(fileName)) fileName = "QuotationFile";
            var stream = CreateQuotationPresentation.CreateDocument(userId, liftFamilyId, listOfOptions, dbQ);
            return File(stream.ToArray(), "application/octet-stream", fileName + ".docx");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbQ.Dispose();
                dbE.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}