using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Admin.EntityModels.Versalift;
using Orchard.Localization;
using Orchard;
using Orchard.Themes;
using Time.Admin.Models;

namespace Time.Admin.Controllers
{
    [Authorize]
    [Themed]
    public class EquipmentController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private VersaliftEntities db = new VersaliftEntities();

        public EquipmentController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        // GET: Equipment
        public ActionResult Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.ViewEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            return View(db.VersaliftLiftModels.OrderBy(x => x.Model).ToList());
        }

        // GET: VersaliftLiftModels/Details/5
        public ActionResult Details(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ViewEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VersaliftLiftModel versaliftLiftModel = db.VersaliftLiftModels.Find(id);
            if (versaliftLiftModel == null)
            {
                return HttpNotFound();
            }

            ViewBag.ModelId = id;
            
                
                var groups = db.VersaliftLiftCategories.Where(x => x.isEnabled).Select(x => x.CategoryGroup).Distinct().ToList();

                ViewBag.CategoryGroups = groups;
            return View(versaliftLiftModel);
        }

        // GET: Equipment/Create
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            return View();
        }

        // POST: Equipment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ModelID,Model,Image,Description,Height,isVisible,Condor")] VersaliftLiftModel versaliftLiftModel)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            versaliftLiftModel.CreateDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.VersaliftLiftModels.Add(versaliftLiftModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(versaliftLiftModel);
        }

        // GET: Equipment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VersaliftLiftModel versaliftLiftModel = db.VersaliftLiftModels.Find(id);
            if (versaliftLiftModel == null)
            {
                return HttpNotFound();
            }
            return View(versaliftLiftModel);
        }

        // POST: Equipment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ModelID,BaseModelID,Model,Image,Description,Height,isVisible,CreateDate,Condor")] VersaliftLiftModel versaliftLiftModel)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                db.Entry(versaliftLiftModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(versaliftLiftModel);
        }

        // GET: Equipment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VersaliftLiftModel versaliftLiftModel = db.VersaliftLiftModels.Find(id);
            if (versaliftLiftModel == null)
            {
                return HttpNotFound();
            }
            return View(versaliftLiftModel);
        }

        // POST: Equipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            VersaliftLiftModel versaliftLiftModel = db.VersaliftLiftModels.Find(id);
            db.VersaliftLiftModels.Remove(versaliftLiftModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Equipment/Edit/5
        public ActionResult AttributeEdit(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VersaliftLiftModelAttribute versaliftLiftModelAttribute = db.VersaliftLiftModelAttributes.Find(id);
            if (versaliftLiftModelAttribute == null)
            {
                return HttpNotFound();
            }
            ViewBag.Type = new SelectList(db.VersaliftLiftModelAttributeTypes, "AttributeTypeID", "AttributeName", versaliftLiftModelAttribute.Type);
            ViewBag.ModelID = new SelectList(db.VersaliftLiftModels, "ModelID", "Model", versaliftLiftModelAttribute.ModelID);
            return View(versaliftLiftModelAttribute);
        }

        // POST: Equipment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AttributeEdit([Bind(Include = "AttributeID,ModelID,Type,Value,Rank,isVisible,CreatedDate")] VersaliftLiftModelAttribute versaliftLiftModelAttribute)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                db.Entry(versaliftLiftModelAttribute).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = versaliftLiftModelAttribute.ModelID});
            }
            ViewBag.Type = new SelectList(db.VersaliftLiftModelAttributeTypes, "AttributeTypeID", "AttributeName", versaliftLiftModelAttribute.Type);
            ViewBag.ModelID = new SelectList(db.VersaliftLiftModels, "ModelID", "Model", versaliftLiftModelAttribute.ModelID);
            return View(versaliftLiftModelAttribute);
        }

        // GET: VersaliftLiftModelAttributes/Create
        public ActionResult AttributeCreate(int ModelId)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            ViewBag.Type = new SelectList(db.VersaliftLiftModelAttributeTypes, "AttributeTypeID", "AttributeName");
            ViewBag.ModelID = ModelId;
            return View();
        }

        // POST: VersaliftLiftModelAttributes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AttributeCreate([Bind(Include = "AttributeID,ModelID,Type,Value,Rank,isVisible,CreatedDate")] VersaliftLiftModelAttribute versaliftLiftModelAttribute)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            versaliftLiftModelAttribute.CreatedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.VersaliftLiftModelAttributes.Add(versaliftLiftModelAttribute);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = versaliftLiftModelAttribute.ModelID });
            }

            ViewBag.Type = new SelectList(db.VersaliftLiftModelAttributeTypes, "AttributeTypeID", "AttributeName", versaliftLiftModelAttribute.Type);
            ViewBag.ModelID = new SelectList(db.VersaliftLiftModels, "ModelID", "Model", versaliftLiftModelAttribute.ModelID);
            return View(versaliftLiftModelAttribute);
        }

        // GET: VersaliftLiftModelAttributes/Delete/5
        public ActionResult AttributeDelete(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VersaliftLiftModelAttribute versaliftLiftModelAttribute = db.VersaliftLiftModelAttributes.Find(id);
            if (versaliftLiftModelAttribute == null)
            {
                return HttpNotFound();
            }
            return View(versaliftLiftModelAttribute);
        }

        // POST: VersaliftLiftModelAttributes/Delete/5
        [HttpPost, ActionName("AttributeDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult AttributeDeleteConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            VersaliftLiftModelAttribute versaliftLiftModelAttribute = db.VersaliftLiftModelAttributes.Find(id);
            var ModelId = versaliftLiftModelAttribute.ModelID;
            db.VersaliftLiftModelAttributes.Remove(versaliftLiftModelAttribute);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = ModelId });
        }

        // GET: VersaliftLiftModelAttributes/Delete/5
        public ActionResult CategoryLinkDelete(int? id, int? ModelId)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            if (id == null || ModelId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            VersaliftLiftModel unit = db.VersaliftLiftModels.Find(ModelId);
            VersaliftLiftCategory category = db.VersaliftLiftCategories.Find(id);

            if (category == null)
            {
                return HttpNotFound();
            }

            CategoryDeleteVM vm = new CategoryDeleteVM
            {
                CategoryId = id ?? 0,
                ModelId = ModelId ?? 0,
                Lift = unit,
                Category = category
            };

            return View(vm);
        }

        // POST: VersaliftLiftModelAttributes/Delete/5
        [HttpPost, ActionName("CategoryLinkDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryLinkDeleteConfirmed(CategoryDeleteVM vm)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            var unit = db.VersaliftLiftModels.Find(vm.ModelId);
            var category = unit.VersaliftLiftCategories.First(x => x.CategoryID == vm.CategoryId);
            unit.VersaliftLiftCategories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = vm.ModelId });
        }

        // GET: VersaliftLiftModelAttributes/Create
        public ActionResult CategoryLinkCreate(int id, string group = "")
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            VersaliftLiftModel lift = db.VersaliftLiftModels.Find(id);
            var filter = lift.VersaliftLiftCategories.Select(x => x.CategoryID).ToList();
            var categories = db.VersaliftLiftCategories.Where(x => !filter.Contains(x.CategoryID) && x.isEnabled);
            if (!String.IsNullOrEmpty(group))
                categories = categories.Where(x => x.CategoryGroup == group);

            //ViewBag.CategoryId = new SelectList(categories.ToList(), "CategoryID", "CategoryName");
            CategoryAddVM vm = new CategoryAddVM
            {
                Lift = lift,
                ModelId = id,
                Categories = new SelectList(categories.OrderBy(x => x.CategoryName).ToList(), "CategoryID", "CategoryName")
            };
            
            return View(vm);
        }

        // POST: VersaliftLiftModelAttributes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryLinkCreate(CategoryAddVM vm)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                var category = db.VersaliftLiftCategories.Find(vm.CategoryId);
                VersaliftLiftModel lift = db.VersaliftLiftModels.Find(vm.ModelId);
                lift.VersaliftLiftCategories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = vm.ModelId });
            }

            return View(vm);
        }

        #region VersaliftLiftCategory

        // GET: VersaliftLiftCategories
        public ActionResult VLC_Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.ViewEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            return View(db.VersaliftLiftCategories.OrderBy(x => x.Sequence).ToList());
        }

        // GET: VersaliftLiftCategories/Details/5
        public ActionResult VLC_Details(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ViewEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VersaliftLiftCategory versaliftLiftCategory = db.VersaliftLiftCategories.Find(id);
            if (versaliftLiftCategory == null)
            {
                return HttpNotFound();
            }
            return View(versaliftLiftCategory);
        }

        // GET: VersaliftLiftCategories/Create
        public ActionResult VLC_Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            return View();
        }

        // POST: VersaliftLiftCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VLC_Create([Bind(Include = "CategoryName,CategoryGroup,isEnabled,Sequence,OverrideURL,VideoURL")] VersaliftLiftCategory versaliftLiftCategory)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            versaliftLiftCategory.CategoryParentID = 0;
            if (ModelState.IsValid)
            {
                db.VersaliftLiftCategories.Add(versaliftLiftCategory);
                db.SaveChanges();
                return RedirectToAction("VLC_Index");
            }

            return View(versaliftLiftCategory);
        }

        // GET: VersaliftLiftCategories/Edit/5
        public ActionResult VLC_Edit(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VersaliftLiftCategory versaliftLiftCategory = db.VersaliftLiftCategories.Find(id);
            if (versaliftLiftCategory == null)
            {
                return HttpNotFound();
            }
            return View(versaliftLiftCategory);
        }

        // POST: VersaliftLiftCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VLC_Edit([Bind(Include = "CategoryID,CategoryName,CategoryGroup,CategoryParentID,isEnabled,Sequence,OverrideURL,VideoURL")] VersaliftLiftCategory versaliftLiftCategory)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                db.Entry(versaliftLiftCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("VLC_Index");
            }
            return View(versaliftLiftCategory);
        }

        // GET: VersaliftLiftCategories/Delete/5
        public ActionResult VLC_Delete(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VersaliftLiftCategory versaliftLiftCategory = db.VersaliftLiftCategories.Find(id);
            if (versaliftLiftCategory == null)
            {
                return HttpNotFound();
            }
            return View(versaliftLiftCategory);
        }

        // POST: VersaliftLiftCategories/Delete/5
        [HttpPost, ActionName("VLC_Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult VLC_DeleteConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageEquipment, T("You are not authorized to view this content")))
                return new HttpUnauthorizedResult();

            VersaliftLiftCategory versaliftLiftCategory = db.VersaliftLiftCategories.Find(id);
            db.VersaliftLiftCategories.Remove(versaliftLiftCategory);
            db.SaveChanges();
            return RedirectToAction("VLC_Index");
        }

        #endregion

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
