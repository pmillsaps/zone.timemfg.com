using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Drawings.EntityModels;
using Time.Drawings.ViewModels;

namespace Time.Drawings.Controllers
{
    [Themed]
    public class HomeController : Controller
    {
        private readonly DrawingsEntities _db;

        public IOrchardServices Services { get; set; }

        public Localizer T { get; set; }

        public HomeController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            _db = new DrawingsEntities();
        }

        //public HomeController()
        //{
        //    _db = new DrawingsEntities();
        //}

        //public HomeController(DrawingsEntities db)
        //{
        //    _db = db;
        //}

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(SearchDrawingsViewModel vm)
        {
            if (vm.Search != null)
            {
                if (vm.Search.Length > 2) //If Search string has more than two digits
                {
                    vm.DrawingsList = _db.Drawings_PDF.Where(x => x.Drawing.StartsWith(vm.Search)).ToList();
                    if (vm.DrawingsList.Count() < 1)
                    {
                        //STRIP DASHES AND TRAILING DIGITS, AND DROP REMAINING DIGITS UNTIL COUNT > 0
                        int a = (vm.Search.IndexOf('-')); //Locates position of dash (zero-based)
                        if (a > 0) { vm.Search = vm.Search.Substring(0, a); } //Length of string preceding dash
                        vm.DrawingsList = _db.Drawings_PDF.Where(x => x.Drawing.StartsWith(vm.Search)).ToList();
                        a = vm.Search.Length;
                        while (vm.DrawingsList.Count() < 1)
                        {
                            a = a - 1;
                            vm.Search = vm.Search.Substring(0, a);
                            vm.DrawingsList = _db.Drawings_PDF.Where(x => x.Drawing.StartsWith(vm.Search)).ToList();
                        }
                        ViewBag.WhereDash = vm.Search.IndexOf('-');
                        ViewBag.SearchNote = "Searching on '" + vm.Search + "'.";
                        ViewBag.Count = vm.DrawingsList.Count() + " Record(s) Found.";
                        if (vm.DrawingsList.Count() > 0) { return View(vm); }
                    }
                    ViewBag.SearchNote = "Searching on '" + vm.Search + "'.";
                    ViewBag.Count = vm.DrawingsList.Count() + " Record(s) Found.";
                    //if (vm.DrawingsList.Count() == 1) return RedirectToAction("DownloadDrawing", new { id = vm.DrawingsList.First().Id });
                    return View(vm);
                }
                if (vm.Search.Length < 3) //If Search string has one or two digits
                {
                    ViewBag.Count = "Enter more digits and try again.";
                    return View();
                }
            }
            ViewBag.Count = "Doh!"; //If Search string is null
            return View();
        }

        public ActionResult DownloadDrawing(int id)
        {
            var doc = _db.Drawings_PDF.Where(x => x.Id == id).FirstOrDefault();
            string dir = doc.Directory;
            //string path = ConfigurationManager.AppSettings["PartsPDFDirectory"];
            string path = Properties.Settings.Default.PartsPDFDirectory;
            if (doc != null)
            {
                string docPath = Path.Combine(path, dir);
                docPath = Path.Combine(docPath, doc.FileName);
                string gpath = Server.MapPath(docPath);

                if (System.IO.File.Exists(gpath))
                {
                    var fileExtension = Path.GetExtension(gpath);
                    string fileType = String.Empty;
                    switch (fileExtension)
                    {
                        case ".pdf":
                            fileType = "application/octet-stream";
                            break;

                        default:
                            fileType = "application/octet-stream";
                            break;
                    }
                    return File(gpath, fileType, doc.FileName);
                }
            }

            return View("Index");
        }

        //[ChildActionOnly]
        public FileResult AutoDownloadDrawing(int id)
        {
            var doc = _db.Drawings_PDF.Where(x => x.Id == id).FirstOrDefault();
            string dir = doc.Directory;
            string path = ConfigurationManager.AppSettings["PartsPDFDirectory"];
            if (doc != null)
            {
                string docPath = Path.Combine(path, dir);
                docPath = Path.Combine(docPath, doc.FileName);
                string gpath = Server.MapPath(docPath);

                if (System.IO.File.Exists(gpath))
                {
                    var fileExtension = Path.GetExtension(gpath);
                    string fileType = String.Empty;
                    switch (fileExtension)
                    {
                        case ".pdf":
                            fileType = "application/octet-stream";
                            break;

                        default:
                            fileType = "application/octet-stream";
                            break;
                    }
                    return File(gpath, fileType, doc.FileName);
                }
            }
            return null;
        }
    }
}