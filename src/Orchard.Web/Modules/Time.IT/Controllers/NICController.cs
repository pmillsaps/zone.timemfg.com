using Orchard.Themes;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Time.Data.EntityModels.ITInventory;

namespace Time.IT.Controllers
{
    [Themed]
    [Authorize]
    public class NICController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Ref_NIC
        public ActionResult Index(string search = "")
        {
            var ref_NIC = db.Ref_NIC.Include(r => r.Ref_CableNo).Include(r => r.Ref_NICSpeed).Include(r => r.Ref_SwitchPort).Include(x => x.Computer);
            if (!String.IsNullOrEmpty(search))
                ref_NIC = ref_NIC.Where(x => x.MAC.Contains(search) || x.IP.Contains(search)
                    || x.Type.Contains(search) || x.Ref_CableNo.Name.Contains(search) || x.Ref_SwitchPort.SwitchPort.Contains(search));

            ref_NIC = ref_NIC.OrderBy(x => x.IP).ThenBy(x => x.Computer.Name);

            return View(ref_NIC.ToList());
        }

        // GET: Ref_NIC/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_NIC ref_NIC = db.Ref_NIC.Find(id);
            if (ref_NIC == null)
            {
                return HttpNotFound();
            }
            return View(ref_NIC);
        }

        // GET: Ref_NIC/Create
        public ActionResult Create(int id)
        {
            ViewBag.Id = id;
            GenerateDropDowns(new Ref_NIC());
            // var computer = db.Computers.Single(x => x.Id == id);
            Ref_NIC nic = new Ref_NIC
            {
                ComputerId = id
            };

            return View(nic);
        }

        // POST: Ref_NIC/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Ref_NIC ref_NIC)
        {
            ValidateNIC(ref_NIC);
            if (ModelState.IsValid)
            {
                db.Ref_NIC.Add(ref_NIC);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            GenerateDropDowns(ref_NIC);
            return View(ref_NIC);
        }

        // GET: Ref_NIC/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_NIC ref_NIC = db.Ref_NIC.Find(id);
            if (ref_NIC == null)
            {
                return HttpNotFound();
            }
            GenerateDropDowns(ref_NIC);
            return View(ref_NIC);
        }

        private void GenerateDropDowns(Ref_NIC nic)
        {
            //var usedCables = db.Ref_NIC.Select(x => x.CableId);
            //var usedPorts = db.Ref_SwitchPort.Select(x => x.Id);
            var usedCables = db.Ref_NIC.Where(x => x.CableId != null && x.CableId != nic.CableId).Select(x => x.CableId).ToList();
            // if (usedCables.Contains(nic.CableId)) usedCables.Remove(nic.CableId);
            var usedPorts = db.Ref_NIC.Where(x => x.Ref_SwitchPort != null && x.SwitchPortId != nic.SwitchPortId).Select(x => x.SwitchPortId).ToList();
            //if (usedPorts.Contains(nic.SwitchPortId)) usedPorts.Remove(nic.SwitchPortId);
            ViewBag.CableId = new SelectList(db.Ref_CableNo.Where(x => !usedCables.Contains(x.Id)).OrderBy(x => x.Name), "Id", "Name", nic.CableId);
            ViewBag.SpeedId = new SelectList(db.Ref_NICSpeed.OrderBy(x => x.NIC_Speed), "Id", "NIC_Speed", nic.SpeedId);
            ViewBag.SwitchPortId = new SelectList(db.Ref_SwitchPort.Where(x => !usedPorts.Contains(x.Id)).OrderBy(x => x.SwitchPort), "Id", "SwitchPort", nic.SwitchPortId);
            ViewBag.ComputerId = new SelectList(db.Computers.OrderBy(x => x.Name), "Id", "Name", nic.ComputerId);
        }

        // POST: Ref_NIC/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ref_NIC ref_NIC)
        {
            ValidateNIC(ref_NIC);
            if (ModelState.IsValid)
            {
                var returnId = ref_NIC.ComputerId;
                db.Entry(ref_NIC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Computers", new { id = returnId });
            }
            GenerateDropDowns(ref_NIC);
            return View(ref_NIC);
        }

        // GET: Ref_NIC/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_NIC ref_NIC = db.Ref_NIC.Find(id);
            if (ref_NIC == null)
            {
                return HttpNotFound();
            }
            return View(ref_NIC);
        }

        // POST: Ref_NIC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_NIC ref_NIC = db.Ref_NIC.Find(id);
            var returnID = ref_NIC.ComputerId;
            db.Ref_NIC.Remove(ref_NIC);
            db.SaveChanges();
            return RedirectToAction("Details", "Computers", new { id = returnID });
        }

        private void ValidateNIC(Ref_NIC ref_NIC)
        {
            if (ref_NIC.MAC.Length != 12) ModelState.AddModelError("MAC", "MAC does not conform to the standard length");
            if (Regex.Match("^[a-fA-F0-9]{12}$", ref_NIC.MAC).Success) ModelState.AddModelError("MAC", "MAC does not pass the regex validation");
            if (db.Ref_NIC.Where(x => x.MAC == ref_NIC.MAC && x.Id != ref_NIC.Id).Count() > 0) ModelState.AddModelError("MAC", "MAC is a duplicate of an existing NIC entry");
            if (!String.IsNullOrEmpty(ref_NIC.IP) && ref_NIC.IP.ToUpper() != "DHCP")
            {
                if (!Regex.IsMatch(ref_NIC.IP, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$")) ModelState.AddModelError("IP", "IP address is not formatted correctly.");
                if (db.Ref_NIC.Where(x => x.IP == ref_NIC.IP && x.Id != ref_NIC.Id).Count() > 0) ModelState.AddModelError("IP", "IP address is a duplicate of an existing NIC entry");
                foreach (var item in ref_NIC.IP.Split('.'))
                {
                    int number;
                    if (Int32.TryParse(item, out number))
                    {
                        if (number < 1 || number > 255) ModelState.AddModelError("IP", "IP address segment is out of range");
                    }
                    else
                    {
                        ModelState.AddModelError("IP", "IP address segment is not a number");
                    }
                }
            }
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