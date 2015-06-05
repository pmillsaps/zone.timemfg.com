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
using Time.IT.ViewModel;

namespace Time.IT.Controllers
{
    [Themed]
    [Authorize]
    public class UsersController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Users
        public ActionResult Index(string search = "")
        {
            var users = db.Users.Include(u => u.Ref_Building).Include(u => u.Ref_Location).Include(u => u.Monitors).Include(u => u.Licenses);

            if (!String.IsNullOrEmpty(search))
                users = users.Where(x => x.Name.Contains(search) || x.Ref_Building.Name.Contains(search)
                    || x.Notes.Contains(search) || x.Ref_Location.Name.Contains(search));

            users = users.OrderBy(x => x.Name);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = id;
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            UserDropDowns(new User());
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] User user)
        {
            SetLastEdited(user);

            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            UserDropDowns(user);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            UserDropDowns(user);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            SetLastEdited(user);
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            UserDropDowns(user);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void UserDropDowns(User user)
        {
            ViewBag.BuildingId = new SelectList(db.Ref_Building.OrderBy(x => x.Name), "Id", "Name", user.BuildingId);
            ViewBag.LocationId = new SelectList(db.Ref_Location.OrderBy(x => x.Name), "Id", "Name", user.LocationId);
        }

        private static void SetLastEdited(User user)
        {
            user.LastDateEdited = DateTime.Now;
            user.LastEditedBy = System.Web.HttpContext.Current.User.Identity.Name;
        }

        public ActionResult AddMonitor(int id)
        {
            Monitor monitor = new Monitor { UserId = id };
            GetMonitorDropDowns(monitor);
            return View(monitor);
        }

        // POST: Monitors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMonitor(Monitor monitor)
        {
            if (ModelState.IsValid)
            {
                db.Monitors.Add(monitor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            GetMonitorDropDowns(monitor);
            return View(monitor);
        }

        private void GetMonitorDropDowns(Monitor monitor)
        {
            ViewBag.ManufacturerId = new SelectList(db.Ref_Manufacturer.OrderBy(x => x.Name), "Id", "Name", monitor.ManufacturerId);
            ViewBag.UserId = new SelectList(db.Users.OrderBy(x => x.Name), "Id", "Name", monitor.UserId);
            ViewBag.SizeId = new SelectList(db.Ref_MonitorSizes.OrderBy(x => x.Size), "Id", "Size", monitor.SizeId);
        }

        public ActionResult LinkMonitor(int id)
        {
            ViewBag.MonitorId = new SelectList(db.Monitors.Where(x => x.UserId == null).OrderBy(x => x.SerialNo), "Id", "AssetId");

            LinkMonitorViewModel vm = new LinkMonitorViewModel
            {
                UserId = id,
                //Monitors = new SelectList(db.Monitors.Where(x => x.UserId == null).OrderBy(x => x.SerialNo), "Id", "SerialNo")
            };
            return View(vm);
        }

        // POST: Monitors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkMonitor(LinkMonitorViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(vm.UserId);
                var monitor = db.Monitors.Find(vm.MonitorId);
                user.Monitors.Add(monitor);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = vm.UserId });
            }

            return View(vm);
        }

        public ActionResult UnLinkMonitor(int id)
        {
            var monitor = db.Monitors.Find(id);
            var user = db.Users.Find(monitor.UserId);
            user.Monitors.Remove(monitor);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = user.Id });
        }

        public ActionResult UnLinkComputer(int id)
        {
            var computer = db.Computers.Find(id);
            var user = db.Users.Find(computer.UserId);
            user.Computers.Remove(computer);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = user.Id });
        }

        public ActionResult LinkComputer(int id)
        {
            ViewBag.ComputerId = new SelectList(db.Computers.Where(x => x.UserId == null).OrderBy(x => x.Name), "Id", "Name");
            LinkComputerViewModel vm = new LinkComputerViewModel
            {
                UserId = id,
                //Monitors = new SelectList(db.Monitors.Where(x => x.UserId == null).OrderBy(x => x.SerialNo), "Id", "SerialNo")
            };
            return View(vm);
        }

        // POST: Monitors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkComputer(LinkComputerViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(vm.UserId);
                var computer = db.Computers.Find(vm.ComputerId);
                user.Computers.Add(computer);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = vm.UserId });
            }

            return View(vm);
        }

        public ActionResult LinkLicense(int id)
        {
            var usedLicenses = db.Users.Find(id).Licenses.Select(x => x.Id);

            ViewBag.LicenseId = new SelectList(db.Licenses.Where(x => !usedLicenses.Contains(x.Id) && x.QuantityAssigned < x.Quantity).OrderBy(x => x.Name).ThenBy(x => x.LicenseKey), "Id", "FullName");
            LinkLicenseViewModel vm = new LinkLicenseViewModel
            {
                UserId = id,
                //Monitors = new SelectList(db.Monitors.Where(x => x.UserId == null).OrderBy(x => x.SerialNo), "Id", "SerialNo")
            };
            return View(vm);
        }

        // POST: Monitors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLicense(LinkLicenseViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(vm.UserId);
                var license = db.Licenses.Find(vm.LicenseId);
                license.QuantityAssigned++;
                user.Licenses.Add(license);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = vm.UserId });
            }

            return View(vm);
        }

        public ActionResult UnLinkLicense(int id, int UserId)
        {
            var license = db.Licenses.Find(id);
            var user = db.Users.Find(UserId);
            user.Licenses.Remove(license);
            license.QuantityAssigned--;
            db.SaveChanges();
            return RedirectToAction("Details", new { id = user.Id });
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