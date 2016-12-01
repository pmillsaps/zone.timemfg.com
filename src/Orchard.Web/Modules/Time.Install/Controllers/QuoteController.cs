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

        // Adding the options to the Install Quote
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuote(QuoteViewModel quoteVM)
        {
            ViewBag.OptionGroups = dbQ.OptionGroups.ToList();
            var aerialOp = dbE.QuoteDtls.FirstOrDefault(x => x.QuoteNum == quoteVM.QuoteNum && x.QuoteLine == 1);
            quoteVM.AerialOptions = aQuote(aerialOp.QuoteComment, quoteVM.LiftFamilyId);
            quoteVM.Options = dbQ.VSWOptions.Where(x => x.LiftFamilyId == quoteVM.LiftFamilyId).ToList();
            quoteVM.AddOptnMnlly = new List<AddVSWOptionManually>();

            if (quoteVM.EditQuote)
            {
                var installQuote = dbQ.InstallQuotes.SingleOrDefault(x => x.LiftQuoteNumber == quoteVM.QuoteNum);
                var installDetail = dbQ.InstallDetails.Include("VSWOption").Where(x => x.InstallQuoteId == installQuote.Id).ToList();
                // Loading the existing lines for the Install details
                foreach (var item in installDetail)
                {
                    foreach (var opt in quoteVM.Options)
                    {
                        if (item.VSWOption.OptionName == opt.OptionName)
                        {
                            opt.Quantity = item.Quantity;
                            opt.Price = item.Price;
                            opt.InstallHours = item.InstallHours;
                        }
                    }
                }
                // Loading the existing lines for the Install details
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
                    quoteVM.AddOptnMnlly.Add(addMnlly);
                }
            }

            return View(quoteVM);
        }

        // Adding the options to the Install Quote
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
                if (vm.EditQuote)
                {
                    LogInstallQuotesChanges(vm);
                }
                // Inserting the data into InstallQuote 
                InsertNewDataIntoInstallQuoteTable(vm);
                // Inserting data into the InstallDetails tables
                InsertDataIntoInstallDetailsAndManuallyAddedOptions(vm);
                // Inserting the Install details into the Epicor db
                var installQid = dbQ.InstallQuotes.Where(x => x.LiftQuoteNumber == vm.QuoteNum).Select(x => new { installId = x.Id }).Single();
                InsertInstallDetailsInEpicor(installQid.installId);

                return RedirectToAction("QuoteSummary", new { installQuoteId = installQid.installId, quoteNum = vm.QuoteNum, liftFamilyId = vm.LiftFamilyId });
            }
        }

        [HttpGet]
        public ActionResult QuoteSummary(int installQuoteId, int quoteNum, int liftFamilyId)
        {
            // Displaying a summary of the install quote
            InstallQuoteSummary installQuoteSummary = new InstallQuoteSummary();
            installQuoteSummary.InstallQuotes = dbQ.InstallQuotes.SingleOrDefault(x => x.Id == installQuoteId);
            installQuoteSummary.InstallDetails = dbQ.InstallDetails.Where(x => x.InstallQuoteId == installQuoteId).ToList();
            installQuoteSummary.InstallDetailsMnllyAdded = dbQ.InstallDetailsManuallyAddedOptions.Where(x => x.InstallQuoteId == installQuoteId).ToList();
            var aerialOp = dbE.QuoteDtls.FirstOrDefault(x => x.QuoteNum == quoteNum && x.QuoteLine == 1);
            installQuoteSummary.AerialOptions = aQuote(aerialOp.QuoteComment, liftFamilyId);

            return View(installQuoteSummary);
        }

        // Logging the Install Quote changes when updated
        private void LogInstallQuotesChanges(QuoteViewModel vm)
        {
            string changes = "";
            bool optionUpdated = false;
            var installQuote = dbQ.InstallQuotes.SingleOrDefault(x => x.LiftQuoteNumber == vm.QuoteNum);
            var installDetail = dbQ.InstallDetails.Include("VSWOption").Where(x => x.InstallQuoteId == installQuote.Id).ToList();
            // Looping trough existing lines in Install details to log changes
            foreach (var item in installDetail)
            {
                foreach (var opt in vm.Options)
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
                                UpdatedBy = HttpContext.User.Identity.Name,
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
            foreach (var item in vm.Options.Where(x => x.Quantity > 0))
            {
                var vswOption = dbQ.VSWOptions.SingleOrDefault(x => x.OptionName == item.OptionName && x.GroupId == item.GroupId && x.LiftFamilyId == item.LiftFamilyId);
                var iDetail = dbQ.InstallDetails.SingleOrDefault(x => x.OptionId == vswOption.Id && x.InstallQuoteId == installQuote.Id);
                if (iDetail == null)
                {
                    QuoteChangesLog chLog = new QuoteChangesLog
                        {
                            InstallQuoteId = installQuote.Id,
                            UpdatedBy = HttpContext.User.Identity.Name,
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
                        UpdatedBy = HttpContext.User.Identity.Name,
                        UpdatedOn = DateTime.Now,
                        ValueChanged = item.AddOptionManually + " was added"
                    };
                    dbQ.QuoteChangesLogs.Add(chLog);
                }
            }
            dbQ.SaveChanges();
        }

        // Inserting the Installation details
        private void InsertDataIntoInstallDetailsAndManuallyAddedOptions(QuoteViewModel vm)
        {
            // Saving the install quote details to the db
            var installQid = dbQ.InstallQuotes.Where(x => x.LiftQuoteNumber == vm.QuoteNum).Select(x => new { installId = x.Id }).Single();
            foreach (var item in vm.Options.Where(x => x.Quantity > 0))// VSW Options in the db
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

        // Inserting new data into InstallQuote table
        private void InsertNewDataIntoInstallQuoteTable(QuoteViewModel vm)
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
            foreach (var item in vm.Options.Where(x => x.Quantity > 0))
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
                InstallQuotedBy = HttpContext.User.Identity.Name,
                QuoteDate = DateTime.Now,
                TotalPriceLabor = totalPriceLabor,
                TotalPriceMaterial = totalPriceMaterial,
                TotalInstallHours = totalHours,
                TotalPaintHours = totalPaintHours,
                LiftFamilyId = vm.LiftFamilyId
            };
            dbQ.InstallQuotes.Add(installQ);
            dbQ.SaveChanges();
        }
        // Inserting the Install details into the Epicor db
        private void InsertInstallDetailsInEpicor(int installQuoteId)
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
            // Inserting the quote details
            installQouteEpicor.QuoteComment = sb.ToString();
            dbE.QuoteDtls.Attach(installQouteEpicor);
            var entry = dbE.Entry(installQouteEpicor);
            entry.Property(x => x.QuoteComment).IsModified = true;
            // Inserting the install cost
            installPriceEpicor.UnitPrice = installQ.TotalInstallPrice.Value;
            installPriceEpicor.DocUnitPrice = installQ.TotalInstallPrice.Value;
            dbE.QuoteQties.Attach(installPriceEpicor);
            var entry2 = dbE.Entry(installPriceEpicor);
            entry2.Property(x => x.UnitPrice).IsModified = true;
            entry2.Property(x => x.DocUnitPrice).IsModified = true;
            dbE.SaveChanges(); // Saving the changes
        }

        // Parsing the Quote Comments that contain the Lift Options
        private List<AerialOption> aQuote(string comment, int liftFamilyId)
        {
            string[] quoteComment = comment.Split(new String[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var aerialOptions = new List<AerialOption>();
            string line = "";
            int counter = 0;
            bool endOfAssMatl = false;
            string partNum = "";

            for (int i = 0; i < quoteComment.Length; i++)
            {
                line = quoteComment[i];
                if (line.Contains("Assemblies:"))
                {
                    endOfAssMatl = true;
                    counter = 1;
                }
                if (line.Contains("PAGE 1 SELECTIONS:"))
                {
                    endOfAssMatl = false;
                    break;
                }
                if (endOfAssMatl && counter == 3)
                {
                    AerialOption aQ = new AerialOption();

                    aQ.QtyPartAndDescription = line.Trim();
                    aQ.Hours = 0;
                    if (!String.IsNullOrEmpty(aQ.QtyPartAndDescription))
                    {
                        if ((!aQ.QtyPartAndDescription.Contains("Materials:")))
                        {
                            if (aQ.QtyPartAndDescription.Contains("<"))
                            {
                                // Replacing the angular brackets in the "<Error> Part Not Found" description to prevent an exception in MVC
                                aQ.QtyPartAndDescription = aQ.QtyPartAndDescription.Replace("<", "");
                                aQ.QtyPartAndDescription = aQ.QtyPartAndDescription.Replace(">", "!");
                            }
                            // Extracting the part number from the string to assign the labor hours
                            //int index = aQ.QtyPartAndDescription.IndexOf('-') - 3;
                            int index = aQ.QtyPartAndDescription.IndexOf('.') + 8;
                            if (index > 0)
                            {
                                partNum = aQ.QtyPartAndDescription.Substring(index, 17);
                                //partNum = aQ.QtyPartAndDescription.Substring(index, 11);
                                partNum = RemoveWhitespace(partNum);
                                // Assigns the labor hours to the Time options
                                var timeHours = dbQ.TimeOptions.Where(x => x.LiftFamilyId == liftFamilyId);
                                foreach (var item in timeHours)
                                {
                                    if (partNum == item.Option)
                                    {
                                        aQ.Hours = item.InstallHours;
                                        break;
                                    }
                                }
                            }
                            aerialOptions.Add(aQ);
                        }
                    }
                    counter = 2;
                }
                counter++;
                line = "";
            }
            return aerialOptions;
        }

        // Removing white spaces on the Part Number while processing the Line Comments from Epicor
        public string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}