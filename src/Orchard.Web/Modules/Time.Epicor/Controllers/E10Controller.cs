using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Epicor;
using Time.Data.EntityModels.Production;
using Time.Data.Models;
using Time.Data.Models.MessageQueue;
using System.Text.RegularExpressions;
using Time.Epicor.ViewModels;
using Time.Epicor.Helpers;

namespace Time.Epicor.Controllers
{
    [Themed]
    [Authorize]
    public class E10Controller : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private ProductionEntities db;
        private EpicorEntities epicor;

        public E10Controller(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new ProductionEntities();
            epicor = new EpicorEntities();
            db.Database.CommandTimeout = 1200;
        }

        public E10Controller(IOrchardServices services, EpicorEntities _epicor, ProductionEntities _db)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = _db;
            epicor = _epicor;
            db.Database.CommandTimeout = 1200;
        }

        // GET: E10
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RequestPartTransfer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RequestPartTransfer(string PartNum)
        {
            string errorMessage = "";
            PartNum = PartNum.Trim();
            var oldPart = epicor.v_PartDetails.FirstOrDefault(x => x.partnum == PartNum);
            var newPart = db.Parts.FirstOrDefault(x => x.PartNum == PartNum);
            if (oldPart == null) errorMessage += "This part number does not exist in the old Epicor System....  Please Investigate";
            if (newPart != null) errorMessage += "This part number already exists in the new Epicor 10 System....  Please Investigate";
            if (errorMessage != "") ViewBag.Message = errorMessage;
            else
            {
                var email = GetSetting.String("TestNotifications");
                email += ",paulm@timemfg.com";
                var currentUser = System.Web.HttpContext.Current.User.Identity.Name;

                string subject = String.Format("Part Transfer Request For '{0}'", PartNum);
                string emailBody = String.Format("Transfer '{0}' from Vantage 8 to Epicor 10 - Checked by Zone Process<br />", PartNum);
                emailBody += String.Format("Transfer Requested by '{0}'<br />", currentUser);
                emailBody += String.Format("E10_DataTransformer.exe --spawn --prod -i \"{0}\"<br />", PartNum);

                var success = MSMQ.SendEmailMessage(subject, emailBody, email, true);
                ViewBag.Message = String.Format("Transfer request for '{0}' has been submitted...", PartNum);

                RequestTransferFile(PartNum);
            }
            return View();
        }

        private void RequestTransferFile(string partNum)
        {
            var command = "J:" + Environment.NewLine;
            command += @"CD \IncomingConversion\E10_DataTransformer" + Environment.NewLine;
            command += String.Format("E10_DataTransformer.exe --spawn --prod -i \"{0}\"", partNum) + Environment.NewLine;
            command += "(goto) 2>nul & DEL \"%~f0\"" + Environment.NewLine;

            var OutputDirectory = GetSetting.String("RequestTransferFile_MoveDirectory");
            var filePath = Server.MapPath(OutputDirectory);
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            var fileName = Path.Combine(filePath, partNum + ".bat");
            using (var sw = new StreamWriter(fileName))
            {
                sw.Write(command);
                sw.Close();

                var fi = new FileInfo(fileName);
                if (fi.Exists)
                {
                    string sourceFile = GetUNCPath(fi.FullName);
                    string targetDirectory = GetSetting.String("Belize_TransferFileDirectory");
                    var ret = MSMQ.SendMoveFileMessage(sourceFile, targetDirectory, fi.Name);
                }
            }
        }

        private string GetUNCPath(string fullFileName)
        {
            string returnName = "";
            string server = Environment.MachineName;
            string drive = fullFileName.Substring(0, 1);
            returnName = String.Format(@"\\{0}\{1}$", server, drive);
            returnName += fullFileName.Substring(2);

            return returnName;
        }

        public ActionResult IssueOperation_S1(string job)
        {
            if (!String.IsNullOrEmpty(job))
            {
                var jobInfo = db.V_JobInformation.FirstOrDefault(x => x.JobNum == job);
                if (jobInfo != null)
                {
                    if (jobInfo.JobComplete ?? false) ViewBag.ErrorMessage += String.Format("Job : {0} is already Complete..." + Environment.NewLine, job.ToUpper());
                    if (jobInfo.JobClosed ?? false) ViewBag.ErrorMessage += String.Format("Job : {0} is already Closed..." + Environment.NewLine, job.ToUpper());
                    if (String.IsNullOrEmpty(ViewBag.ErrorMessage))
                    {
                        //var opers = db.JobOpers.Where(x => x.JobNum == job).OrderBy(x => x.OpDesc).Select(x => x.OpDesc).Distinct().ToList();
                        //var viewmodel = new IssueOperationVM { JobNumber = job, Operations = opers };
                        return RedirectToAction("IssueOperation_S2", new { id = job });
                        //return View("IssueOperation_S2", vm);
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = String.Format("Job : {0} does not exist...", job.ToUpper());
                }
            }

            return View();
        }

        public ActionResult IssueOperation_S2(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var opers = db.JobOpers.Where(x => x.JobNum == id).OrderBy(x => x.OpDesc).Select(x => x.OpCode).Distinct().ToList();
                var viewmodel = new IssueOperationVM { JobNumber = id, Operations = opers.Select(x => new SelectListItem { Text = x }) };
                return View(viewmodel);
            }

            return RedirectToAction("IssueOperation_S1");
        }

        [HttpGet]
        public ActionResult IssueOperation(IssueOperationVM viewmodel)
        {
            return View(viewmodel);
        }

        [HttpPost, ActionName("IssueOperation")]
        public ActionResult IssueOperationConfirmed(IssueOperationVM viewmodel)
        {
            WriteIssueFile(viewmodel);
            return View();
        }

        private void WriteIssueFile(IssueOperationVM viewmodel)
        {
            //var command = "J:" + Environment.NewLine;
            //command += @"CD \IncomingConversion\E10_DataTransformer" + Environment.NewLine;
            //command += String.Format("E10_DataTransformer.exe --spawn --prod -i \"{0}\"", partNum) + Environment.NewLine;
            //command += "(goto) 2>nul & DEL \"%~f0\"" + Environment.NewLine;

            //var OutputDirectory = GetSetting.String("RequestTransferFile_MoveDirectory");
            //var filePath = Server.MapPath(OutputDirectory);
            //if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            //var fileName = Path.Combine(filePath, partNum + ".bat");
            //using (var sw = new StreamWriter(fileName))
            //{
            //    sw.Write(command);
            //    sw.Close();

            //    var fi = new FileInfo(fileName);
            //    if (fi.Exists)
            //    {
            //        string sourceFile = GetUNCPath(fi.FullName);
            //        string targetDirectory = GetSetting.String("Belize_TransferFileDirectory");
            //        var ret = MSMQ.SendMoveFileMessage(sourceFile, targetDirectory, fi.Name);
            //    }
            //}

            //throw new NotImplementedException();
        }

        public ActionResult ExportView(string viewName)
        {
            return View();
        }

        public ActionResult V_PartInfo(string SelectedParts)
        {
            if (!String.IsNullOrEmpty(SelectedParts))
            {
                SelectedParts = SelectedParts.Replace("\n", ",");
                SelectedParts = SelectedParts.Replace("\r", "");
                SelectedParts = SelectedParts.Replace(" ", "");

                List<string> partList = new List<string>();
                if (SelectedParts.Contains(","))
                    partList.AddRange(SelectedParts.Split(','));
                else
                    partList.Add(SelectedParts);

                var output = db.V_PartInfo.Where(x => partList.Contains(x.Part_PartNum)).ToList();

                string fileName = "PartInfo_By_Parts";
                return new ExporttoExcelResult(fileName, output.Cast<object>().ToList());
            }

            return View();
        }
    }
}