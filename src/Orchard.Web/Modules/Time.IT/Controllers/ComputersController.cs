using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.ITInventory;

namespace Time.IT.Controllers
{
    [Themed]
    public class ComputersController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Computers
        public ActionResult Index()
        {
            var computers = db.Computers.Include(c => c.Ref_BuildYear).Include(c => c.Ref_Memory).Include(c => c.Ref_Model).Include(c => c.Ref_Status).Include(c => c.Ref_VideoCard).Include(c => c.Ref_DeviceType).Include(c => c.Ref_Processor).Include(c => c.Ref_Sound).Include(c => c.Ref_OS);
            return View(computers.ToList());
        }

        // GET: Computers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Computer computer = db.Computers.Find(id);
            if (computer == null)
            {
                return HttpNotFound();
            }
            return View(computer);
        }

        // GET: Computers/Create
        public ActionResult Create()
        {
            ViewBag.BuildYearId = new SelectList(db.Ref_BuildYear, "Id", "Name");
            ViewBag.MemoryId = new SelectList(db.Ref_Memory, "Id", "Name");
            ViewBag.ModelId = new SelectList(db.Ref_Model, "ID", "Model");
            ViewBag.StatusId = new SelectList(db.Ref_Status, "Id", "Status");
            ViewBag.VideoCardId = new SelectList(db.Ref_VideoCard, "Id", "VideoCard");
            ViewBag.DeviceTypeId = new SelectList(db.Ref_DeviceType, "Id", "DeviceType");
            ViewBag.ProcessorId = new SelectList(db.Ref_Processor, "Id", "Processor");
            ViewBag.SoundId = new SelectList(db.Ref_Sound, "Id", "Sound");
            ViewBag.OSId = new SelectList(db.Ref_OS, "Id", "OS");
            return View();
        }

        // POST: Computers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,StatusId,ModelId,WindowsKey,MemoryId,ProcessorId,DeviceTypeId,OSId,VideoCardId,SoundId,BuildYearId,LastEditedBy,LastDateEdited,Note,PO,PurchaseDate")] Computer computer)
        {
            if (ModelState.IsValid)
            {
                db.Computers.Add(computer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BuildYearId = new SelectList(db.Ref_BuildYear, "Id", "Name", computer.BuildYearId);
            ViewBag.MemoryId = new SelectList(db.Ref_Memory, "Id", "Name", computer.MemoryId);
            ViewBag.ModelId = new SelectList(db.Ref_Model, "ID", "Model", computer.ModelId);
            ViewBag.StatusId = new SelectList(db.Ref_Status, "Id", "Status", computer.StatusId);
            ViewBag.VideoCardId = new SelectList(db.Ref_VideoCard, "Id", "VideoCard", computer.VideoCardId);
            ViewBag.DeviceTypeId = new SelectList(db.Ref_DeviceType, "Id", "DeviceType", computer.DeviceTypeId);
            ViewBag.ProcessorId = new SelectList(db.Ref_Processor, "Id", "Processor", computer.ProcessorId);
            ViewBag.SoundId = new SelectList(db.Ref_Sound, "Id", "Sound", computer.SoundId);
            ViewBag.OSId = new SelectList(db.Ref_OS, "Id", "OS", computer.OSId);
            return View(computer);
        }

        // GET: Computers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Computer computer = db.Computers.Find(id);
            if (computer == null)
            {
                return HttpNotFound();
            }
            ViewBag.BuildYearId = new SelectList(db.Ref_BuildYear, "Id", "Name", computer.BuildYearId);
            ViewBag.MemoryId = new SelectList(db.Ref_Memory, "Id", "Name", computer.MemoryId);
            ViewBag.ModelId = new SelectList(db.Ref_Model, "ID", "Model", computer.ModelId);
            ViewBag.StatusId = new SelectList(db.Ref_Status, "Id", "Status", computer.StatusId);
            ViewBag.VideoCardId = new SelectList(db.Ref_VideoCard, "Id", "VideoCard", computer.VideoCardId);
            ViewBag.DeviceTypeId = new SelectList(db.Ref_DeviceType, "Id", "DeviceType", computer.DeviceTypeId);
            ViewBag.ProcessorId = new SelectList(db.Ref_Processor, "Id", "Processor", computer.ProcessorId);
            ViewBag.SoundId = new SelectList(db.Ref_Sound, "Id", "Sound", computer.SoundId);
            ViewBag.OSId = new SelectList(db.Ref_OS, "Id", "OS", computer.OSId);
            return View(computer);
        }

        // POST: Computers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,StatusId,ModelId,WindowsKey,MemoryId,ProcessorId,DeviceTypeId,OSId,VideoCardId,SoundId,BuildYearId,LastEditedBy,LastDateEdited,Note,PO,PurchaseDate")] Computer computer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(computer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BuildYearId = new SelectList(db.Ref_BuildYear, "Id", "Name", computer.BuildYearId);
            ViewBag.MemoryId = new SelectList(db.Ref_Memory, "Id", "Name", computer.MemoryId);
            ViewBag.ModelId = new SelectList(db.Ref_Model, "ID", "Model", computer.ModelId);
            ViewBag.StatusId = new SelectList(db.Ref_Status, "Id", "Status", computer.StatusId);
            ViewBag.VideoCardId = new SelectList(db.Ref_VideoCard, "Id", "VideoCard", computer.VideoCardId);
            ViewBag.DeviceTypeId = new SelectList(db.Ref_DeviceType, "Id", "DeviceType", computer.DeviceTypeId);
            ViewBag.ProcessorId = new SelectList(db.Ref_Processor, "Id", "Processor", computer.ProcessorId);
            ViewBag.SoundId = new SelectList(db.Ref_Sound, "Id", "Sound", computer.SoundId);
            ViewBag.OSId = new SelectList(db.Ref_OS, "Id", "OS", computer.OSId);
            return View(computer);
        }

        // GET: Computers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Computer computer = db.Computers.Find(id);
            if (computer == null)
            {
                return HttpNotFound();
            }
            return View(computer);
        }

        // POST: Computers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Computer computer = db.Computers.Find(id);
            db.Computers.Remove(computer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult _NIC(int id)
        {
            var computer = db.Computers.FirstOrDefault(x => x.Id == id);
            IEnumerable<Ref_NIC> nics;
            if (computer != null)
            {
                nics = computer.Ref_NIC;
                return PartialView(nics);
            }
            return null;
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
