using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Configurator.Models;
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Controllers
{
    [Themed]
    [Authorize]
    public class ComplexStructuresController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ComplexStructuresController(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public ComplexStructuresController(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: ComplexStructures
        public ActionResult Index(string ConfigNames, string ConfigData)
        {
            ViewBag.ConfigNames = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            ViewBag.ConfigData = new SelectList(db.ComplexStructures.Select(x => x.ConfigData).Distinct());

            if (String.IsNullOrEmpty(ConfigNames) && String.IsNullOrEmpty(ConfigData))
            {
                return View();
            }
            else
            {
                var complexStructures = db.ComplexStructures.AsQueryable();
                if (!String.IsNullOrEmpty(ConfigNames)) complexStructures = complexStructures.Where(x => x.ConfigName == ConfigNames);
                if (!String.IsNullOrEmpty(ConfigData)) complexStructures = complexStructures.Where(x => x.ConfigData == ConfigData);

                return View(complexStructures.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
            }
        }

        // GET: ComplexStructures/Details/5
        public ActionResult Details(int? id, string Search)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplexStructure complexStructure = db.ComplexStructures.Find(id);
            if (complexStructure == null)
            {
                return HttpNotFound();
            }

            ViewBag.Notes = db.ComplexStructures.Where(x => x.ConfigName == complexStructure.ConfigName && x.ConfigData == complexStructure.ConfigData).ToList();
            ViewBag.Search = Search;
            return View(complexStructure);
        }

        //public ActionResult ComplexLinks(int? id)
        //{
        //    // id = ComplexStructure.Id
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    var complexLinks = db.ComplexLinks.Where(x => x.ComplexDataId == id);

        //    if (complexLinks == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return View();
        //}

        public ActionResult _LookupMatrix(int id)
        {
            //var lookups = new List<Lookup>();
            var complexStructure = db.ComplexStructures.Find(id);
            var lookups = db.Lookups.Where(x => x.ConfigName == complexStructure.ConfigName && x.ConfigData == complexStructure.ConfigData).OrderBy(x => x.Id);
            //foreach (var item in cxLookups)
            //{
            //    foreach (var link in item.ComplexLinks)
            //    {
            //        lookups.Add(link.Lookup);
            //    }
            //}
            return PartialView(lookups);
        }

        public ActionResult _ComplexLinkMatrix(int id, string Search)
        {
            // id = ComplexStructure.Id
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            var complexStructure = db.ComplexStructures.Find(id);

            var cbArray = BuildCheckBoxArray(complexStructure, Search);

            var lookups = db.Lookups.Where(x => x.ConfigName == complexStructure.ConfigName && x.ConfigData == complexStructure.ConfigData).OrderBy(x => x.Id);
            ComplexLinkMatrixViewModel vm = new ComplexLinkMatrixViewModel
            {
                ComplexLinks = cbArray,
                complexStructure = complexStructure,
                Lookups = lookups
            };

            return PartialView(vm);
        }

        private List<List<ComplexLink>> BuildCheckBoxArray(ComplexStructure complexStructure, string Search)
        {
            var list = new List<List<ComplexLink>>();
            var details = db.ComplexLookups
                .Include(x => x.ComplexLinks)
                .Where(x => x.ConfigName == complexStructure.ConfigName && x.ConfigData == complexStructure.ConfigData);
            if (!string.IsNullOrEmpty(Search))
            {
                foreach (var item in Search.Split(' '))
                {
                    details = details.Where(x => x.LookupData.Contains(item));
                }
            }

            foreach (var detail in details.OrderBy(x => x.Id))
            {
                var innerList = new List<ComplexLink>();
                foreach (var source in detail.ComplexLinks.OrderBy(x => x.ComplexDataId))
                {
                    innerList.Add(source);
                }
                list.Add(innerList);
            }

            return list;
        }

        [HttpGet]
        public ActionResult EditComplexLinkMatrix(int id, string Search)
        {
            var complexStructure = db.ComplexStructures.Find(id);

            var cbArray = BuildCheckBoxArray(complexStructure, Search);

            var lookups = db.Lookups.Where(x => x.ConfigName == complexStructure.ConfigName && x.ConfigData == complexStructure.ConfigData)
                .OrderBy(x => x.Id);
            ComplexLinkMatrixViewModel vm = new ComplexLinkMatrixViewModel
            {
                ComplexLinks = cbArray,
                complexStructure = complexStructure,
                Lookups = lookups
            };
            ViewBag.Search = Search;

            return View(vm);
        }

        [HttpPost]
        public ActionResult EditComplexLinkMatrix(List<string> Matrix, int id, string Search)
        {
            //if (Matrix != null)
            //{
            // removed null check, as I need to remove them all if I want to delete the linkages
            SaveComplexLinkMatrix(id, Matrix, Search);
            //}

            return RedirectToAction("Details", new { id = id, Search = Search });
        }

        private void SaveComplexLinkMatrix(int id, List<string> matrix, string Search)
        {
            var cxStructure = db.ComplexStructures.Find(id);
            var cxLookups = db.ComplexLookups.Where(x => x.ConfigName == cxStructure.ConfigName && x.ConfigData == cxStructure.ConfigData);

            if (!string.IsNullOrEmpty(Search))
            {
                foreach (var item in Search.Split(' '))
                {
                    cxLookups = cxLookups.Where(x => x.LookupData.Contains(item));
                }
            }

            foreach (var lookup in cxLookups.OrderBy(x => x.Id))
            {
                foreach (var source in lookup.ComplexLinks.OrderBy(x => x.LookupId))
                {
                    string exists = String.Format("{0}, {1}", source.ComplexDataId, source.LookupId);
                    if (matrix != null && matrix.Contains(exists))
                    {
                        source.Available = true;
                    }
                    else
                    {
                        source.Available = false;
                    }
                }
            }

            db.SaveChanges();
        }

        // GET: ComplexStructures/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ComplexStructures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] ComplexStructure complexStructure)
        {
            if (ModelState.IsValid)
            {
                db.ComplexStructures.Add(complexStructure);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(complexStructure);
        }

        // GET: ComplexStructures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplexStructure complexStructure = db.ComplexStructures.Find(id);
            if (complexStructure == null)
            {
                return HttpNotFound();
            }
            return View(complexStructure);
        }

        // POST: ComplexStructures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ComplexStructure complexStructure)
        {
            if (ModelState.IsValid)
            {
                db.Entry(complexStructure).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { ConfigNames = complexStructure.ConfigName, ConfigData = complexStructure.ConfigData});
            }
            return View(complexStructure);
        }

        // GET: ComplexStructures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplexStructure complexStructure = db.ComplexStructures.Find(id);
            if (complexStructure == null)
            {
                return HttpNotFound();
            }
            return View(complexStructure);
        }

        // POST: ComplexStructures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ComplexStructure complexStructure = db.ComplexStructures.Find(id);
            db.ComplexStructures.Remove(complexStructure);
            db.SaveChanges();
            return RedirectToAction("Index", new { ConfigNames = complexStructure.ConfigName, ConfigData = complexStructure.ConfigData });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}