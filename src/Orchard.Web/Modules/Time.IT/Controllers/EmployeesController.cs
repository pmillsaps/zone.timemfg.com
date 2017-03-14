using Orchard.Themes;
using Orchard;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.TimeMFG;
using Time.Data.EntityModels.ITInventory;
using Time.Data.EntityModels.Production;
using Time.IT.Models;

namespace Time.IT.Controllers
{
    [Themed]
    [Authorize]
    public class EmployeesController : Controller
    {
        private TimeMFGEntities db = new TimeMFGEntities();
        private ITInventoryEntities iti = new ITInventoryEntities();
        private ProductionEntities prod = new ProductionEntities();

        public IOrchardServices Services { get; set; }

        public EmployeesController(IOrchardServices services)
        {
            Services = services;
            db = new TimeMFGEntities();
            //Setup();
        }

        public EmployeesController(IOrchardServices services, TimeMFGEntities _db)
        {
            Services = services;
            db = _db;
            //Setup();
        }

        // GET: Term_Employees
        public ActionResult Index(string search = "", string ddl = "")
        {
            if (search.Length == 0 && ddl == "All")
            {
                return View(db.Term_Employees.OrderBy(x => x.FName).ThenBy(x => x.LName).ToList());
            }
            else if (search.Length == 0 && ddl == "Terminated")
            {
                var term = db.Term_Employees.Where(x => x.Terminated == true).OrderBy(x => x.FName).ThenBy(x => x.LName).ToList();

                return View(term);
            }
            else if (search.Length == 0 && ddl == "Employed")
            {
                var term = db.Term_Employees.Where(x => x.Terminated == false).OrderBy(x => x.FName).ThenBy(x => x.LName).ToList();

                return View(term);
            }
            else if (search.Length >= 1 && ddl == "All")
            {
                var term = db.Term_Employees.Where(x => x.FName.Contains(search) || x.LName.Contains(search) || x.Email.Contains(search)).OrderBy(x => x.FName).ThenBy(x => x.LName).ToList();

                return View(term);
            }
            else if (search.Length >= 1 && ddl == "Terminated")
            {
                var term = db.Term_Employees.Where(x => x.Terminated == true && (x.FName.Contains(search) || x.LName.Contains(search) || x.Email.Contains(search))).OrderBy(x => x.FName).ThenBy(x => x.LName).ToList();

                return View(term);
            }
            else if (search.Length >= 1 && ddl == "Employed")
            {
                var term = db.Term_Employees.Where(x => x.Terminated == false && (x.FName.Contains(search) || x.LName.Contains(search) || x.Email.Contains(search))).OrderBy(x => x.FName).ThenBy(x => x.LName).ToList();

                return View(term);
            }
            else
            {
                return View(db.Term_Employees.OrderBy(x => x.FName).ThenBy(x => x.LName).ToList());
            }
        }

        // GET: Term_Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Term_Employees term_Employees = db.Term_Employees.Find(id);
            if (term_Employees == null)
            {
                return HttpNotFound();
            }

            if (Services.Authorizer.Authorize(Permissions.IT))
                ViewBag.ITUser = true;
            else
                ViewBag.ITUser = false;


            var email = db.Term_Employees.Where(x => x.Id == id).Select(x => x.Email).SingleOrDefault();
            ViewBag.Email = email;

            var property = db.Term_Property.Where(x => x.EmpID == id).ToList();
            ViewBag.Property = property;
            var cellphone = db.Term_Property.Where(x => x.EmpID == id).Select(x => x.CellPhone).Single();
            ViewBag.CellPhone = cellphone;
            var cables = db.Term_Property.Where(x => x.EmpID == id).Select(x => x.Cables).Single();
            ViewBag.Cables = cables;
            var officekey = db.Term_Property.Where(x => x.EmpID == id).Select(x => x.OfficeKey).Single();
            ViewBag.OfficeKey = officekey;
            var buildingkey = db.Term_Property.Where(x => x.EmpID == id).Select(x => x.BuildingKey).Single();
            ViewBag.BuildingKey = buildingkey;

            var archive = db.Term_ITInfo.Where(x => x.EmpID == id).Select(x => x.ArchiveEmail).Single();
            ViewBag.Archive = archive;
            var fwemail = db.Term_ITInfo.Where(x => x.EmpID == id).Select(x => x.FowardEmails).Single();
            ViewBag.FWEmail = fwemail;
            var deleteudrive = db.Term_ITInfo.Where(x => x.EmpID == id).Select(x => x.DeleteUDrive).Single();
            ViewBag.DeleteUDrive = deleteudrive;
            var deskphone = db.Term_ITInfo.Where(x => x.EmpID == id).Select(x => x.DeskPhone).Single();
            ViewBag.DeskPhone = deskphone;
            var deskphonefw = db.Term_ITInfo.Where(x => x.EmpID == id).Select(x => x.DeskPhoneFW).Single();
            ViewBag.DeskPhoneFW = deskphonefw;
            var deskphonerename = db.Term_ITInfo.Where(x => x.EmpID == id).Select(x => x.DeskPhoneRename).Single();
            ViewBag.DeskPhoneRename = deskphonerename;

            var propid = db.Term_Property.Where(x => x.EmpID == term_Employees.Id).Select(x => x.Id).Single();
            var infoid = db.Term_ITInfo.Where(x => x.EmpID == term_Employees.Id).Select(x => x.Id).Single();
            ViewBag.PropID = propid;
            ViewBag.InfoID = infoid;

            var itinfo = db.Term_ITInfo.Where(x => x.EmpID == id).ToList();
            ViewBag.ITInfo = itinfo;
            var epicoracc = prod.UserFiles.Where(x => x.Name == term_Employees.FName + " " + term_Employees.LName).Select(x => x.DcdUserID).SingleOrDefault();
            ViewBag.EpicorAcc = epicoracc;

            var userid = iti.Users.Where(x => x.Name == term_Employees.FName + " " + term_Employees.LName).Select(x => x.Id).SingleOrDefault();
            ViewBag.UserID = userid;

            return View(term_Employees);
        }

        // GET: Term_Employees/Create
        public ActionResult Create(string ddl = "")
        {
            return View();
        }

        // POST: Term_Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Term_Employees term_Employees, string ddl)
        {
            if (ddl == "Terminated")
            {
                term_Employees.Terminated = true;
            }
            else
            {
                term_Employees.Terminated = false;
            }

            var user = iti.Users.Where(x => x.Name == term_Employees.FName + " " + term_Employees.LName).Select(x => x.Id).SingleOrDefault();

            var cellid = iti.Ref_DeviceType.Where(x => x.DeviceType == "Cell Phone").Select(x => x.Id).Single();
            var cell = iti.Computers.Where(x => x.UserId == user && x.DeviceTypeId == cellid).Select(x => x.Name).SingleOrDefault();
            var celltf = false;
            if (!String.IsNullOrEmpty(cell))
                celltf = true;
            else
                celltf = false;

            var deskphonetf = false;
            var timephoneid = iti.Ref_DeviceType.Where(x => x.DeviceType == "Shoretel-TIME").Select(x => x.Id).Single();
            var timephone = iti.Computers.Where(x => x.UserId == user && x.DeviceTypeId == timephoneid).Select(x => x.Name).SingleOrDefault();
            var vswphoneid = iti.Ref_DeviceType.Where(x => x.DeviceType == "Shoretel-VSW").Select(x => x.Id).Single();
            var vswphone = iti.Computers.Where(x => x.UserId == user && x.DeviceTypeId == vswphoneid).Select(x => x.Name).SingleOrDefault();
            if (!String.IsNullOrEmpty(timephone) || !String.IsNullOrEmpty(vswphone))
                deskphonetf = true;
            else
                deskphonetf = false;

            var epicoracc = prod.UserFiles.Where(x => x.Name == term_Employees.FName + " " + term_Employees.LName).Select(x => x.DcdUserID).SingleOrDefault();
            ViewBag.EpicorAcc = epicoracc;

            if (ModelState.IsValid)
            {
                db.Term_Employees.Add(term_Employees);
                db.Term_Property.Add(new Term_Property
                {
                    EmpID = term_Employees.Id,
                    CellPhone = celltf,
                    FMPOff = false,
                    CellReceived = false,
                    Cables = false,
                    CablesReceived = false,
                    OfficeKey = false,
                    OKeyReceived = false,
                    BuildingKey = false,
                    BKeyReceived = false
                });
                if ((!String.IsNullOrEmpty(epicoracc)) && (String.IsNullOrEmpty(term_Employees.Email))) //have epicor but no email
                {
                    db.Term_ITInfo.Add(new Term_ITInfo { EmpID = term_Employees.Id, WindowsAccAccess = "No Windows Account", ArchiveEmail = false, EmailAtW = "N/A", FowardEmails = false, FowardAtW = "N/A", DeleteUDrive = false, UDriveAtW = "N/A", DeskPhone = false, DeskPhoneFW = false, DeskPhoneFWtW = "N/A", DeskPhoneRename = false, PhoneRenametW = "N/A", EpicorAcc = true, EpicorUserID = epicoracc });
                }
                else if ((String.IsNullOrEmpty(epicoracc)) && (String.IsNullOrEmpty(term_Employees.Email))) //have no epicor and no email
                {
                    db.Term_ITInfo.Add(new Term_ITInfo { EmpID = term_Employees.Id, WindowsAccAccess = "No Windows Account", ArchiveEmail = false, EmailAtW = "N/A", FowardEmails = false, FowardAtW = "N/A", DeleteUDrive = false, UDriveAtW = "N/A", DeskPhone = false, DeskPhoneFW = false, DeskPhoneFWtW = "N/A", DeskPhoneRename = false, PhoneRenametW = "N/A", EpicorAcc = false, EpicorUserID = "N/A" });
                }
                else if ((!String.IsNullOrEmpty(epicoracc)) && (!String.IsNullOrEmpty(term_Employees.Email))) //have epicor and email
                {
                    db.Term_ITInfo.Add(new Term_ITInfo { EmpID = term_Employees.Id, WindowsAccAccess = "Active Windows Account", ArchiveEmail = false, FowardEmails = false, DeleteUDrive = false, DeskPhone = deskphonetf, DeskPhoneFW = false, DeskPhoneRename = false, EpicorAcc = true, EpicorUserID = epicoracc });
                }
                else //have no epicor and have email
                {
                    db.Term_ITInfo.Add(new Term_ITInfo { EmpID = term_Employees.Id, WindowsAccAccess = "Active Windows Account", ArchiveEmail = false, FowardEmails = false, DeleteUDrive = false, DeskPhone = deskphonetf, DeskPhoneFW = false, DeskPhoneRename = false, EpicorAcc = false, EpicorUserID = "N/A" });
                }
                db.SaveChanges();
                return RedirectToAction("AfterCreate", new { id = db.Term_Property.Where(x => x.EmpID == term_Employees.Id).Select(x => x.EmpID).Single() });
            }

            return View(term_Employees);
        }

        // GET: Term_Employees/AfterCreate/5
        public ActionResult AfterCreate(int id, string ddl = "")
        {
            var prop = db.Term_Property.Where(x => x.EmpID == id).Single();
            var info = db.Term_ITInfo.Where(x => x.EmpID == id).Single();

            var afterCreate = new AfterCreateViewModel
            {
                EmpID = id,
                PropID = prop.Id,
                CellPhone = prop.CellPhone,
                FindMyPhoneOff = prop.FMPOff,
                CellReceived = prop.CellReceived,
                Cables = prop.Cables,
                CablesReceived = prop.CablesReceived,
                OfficeKey = prop.OfficeKey,
                OKeyReceived = prop.OKeyReceived,
                BuildingKey = prop.BuildingKey,
                BKeyReceived = prop.BKeyReceived,
                ITID = info.Id,
                WindowsAccAccess = info.WindowsAccAccess,
                ArchiveEmail = info.ArchiveEmail,
                EmailAtW = info.EmailAtW,
                FowardEmails = info.FowardEmails,
                ForwardAtW = info.FowardAtW,
                DeleteUDrive = info.DeleteUDrive,
                UDriveAtW = info.UDriveAtW,
                DeskPhone = info.DeskPhone,
                DeskPhoneFW = info.DeskPhoneFW,
                DeskPhoneFWtW = info.DeskPhoneFWtW,
                DeskPhoneRename = info.DeskPhoneRename,
                PhoneRenametW = info.PhoneRenametW,
                EpicorAcc = info.EpicorAcc,
                EpicorUserID = info.EpicorUserID
            };

            var windows = db.Term_ITInfo.Where(x => x.EmpID == id).Select(x => x.WindowsAccAccess).Single();
            ViewBag.Windows = windows;

            return View(afterCreate);
        }

        // POST: Term_Employees/AfterCreate/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AfterCreate([Bind(Include = "")] AfterCreateViewModel afterCreate, Term_Property term_property, Term_ITInfo term_itinfo, string ddl)
        {
            term_property.Id = db.Term_Property.Where(x => x.EmpID == term_property.EmpID).Select(x => x.Id).Single();
            term_itinfo.Id = db.Term_ITInfo.Where(x => x.EmpID == term_itinfo.EmpID).Select(x => x.Id).Single();

            if (ddl == "None") term_itinfo.WindowsAccAccess = "No Windows Account";
            else if (ddl == "Active") term_itinfo.WindowsAccAccess = "Active Windows Account";
            else term_itinfo.WindowsAccAccess = "Deactivate Windows Account";

            if (term_property.CellPhone == false)
            {
                term_property.FMPOff = false;
                term_property.CellReceived = false;
            }
            if (term_property.Cables == false) term_property.CablesReceived = false;
            if (term_property.OfficeKey == false) term_property.OKeyReceived = false;
            if (term_property.BuildingKey == false) term_property.BKeyReceived = false;

            if (term_itinfo.ArchiveEmail == false) term_itinfo.EmailAtW = "N/A";
            if (term_itinfo.FowardEmails == false) term_itinfo.FowardAtW = "N/A";
            if (term_itinfo.DeleteUDrive == false) term_itinfo.UDriveAtW = "N/A";
            if (term_itinfo.DeskPhone == false)
            {
                term_itinfo.DeskPhoneFW = false;
                term_itinfo.DeskPhoneFWtW = "N/A";
                term_itinfo.DeskPhoneRename = false;
                term_itinfo.PhoneRenametW = "N/A";
            }
            if (term_itinfo.DeskPhoneFW == false) term_itinfo.DeskPhoneFWtW = "N/A";
            if (term_itinfo.DeskPhoneRename == false) term_itinfo.PhoneRenametW = "N/A";

            if (ModelState.IsValid)
            {
                db.Entry(term_property).State = EntityState.Modified;
                db.SaveChanges();
                db.Entry(term_itinfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Term_Employees/Edit/5
        public ActionResult Edit(int? id, string ddl = "")
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Term_Employees term_Employees = db.Term_Employees.Find(id);
            if (term_Employees == null)
            {
                return HttpNotFound();
            }

            return View(term_Employees);
        }

        // POST: Term_Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FName,LName,Email,Terminated,TerminatedDate")] Term_Employees term_Employees, string ddl)
        {
            if (ddl == "Terminated")
            {
                term_Employees.Terminated = true;
            }
            else
            {
                term_Employees.Terminated = false;
            }

            if (ModelState.IsValid)
            {
                db.Entry(term_Employees).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(term_Employees);
        }

        // GET: Term_Employees/EditProperty/5
        public ActionResult EditProperty(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Term_Property term_property = db.Term_Property.Find(id);
            if (term_property == null)
            {
                return HttpNotFound();
            }
            return View(term_property);
        }

        // POST: Term_Employees/EditProperty/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProperty([Bind(Include = "Id,EmpId,CellPhone,FMPOff,CellReceived,Cables,CablesReceived,OfficeKey,OKeyReceived,BuildingKey,BKeyReceived")] Term_Property term_property)
        {
            if (term_property.CellPhone == false)
            {
                term_property.FMPOff = false;
                term_property.CellReceived = false;
            }
            if (term_property.Cables == false) term_property.CablesReceived = false;
            if (term_property.OfficeKey == false) term_property.OKeyReceived = false;
            if (term_property.BuildingKey == false) term_property.BKeyReceived = false;

            if (ModelState.IsValid)
            {
                db.Entry(term_property).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = db.Term_Employees.Where(x => x.Id == term_property.EmpID).Select(x => x.Id).Single() });
            }
            return View(term_property);
        }

        // GET: Term_Employees/EditITInfo/5
        public ActionResult EditITInfo(int? id, string ddl = "")
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Term_ITInfo term_itinfo = db.Term_ITInfo.Find(id);
            if (term_itinfo == null)
            {
                return HttpNotFound();
            }

            var windows = db.Term_ITInfo.Where(x => x.Id == id).Select(x => x.WindowsAccAccess).Single();
            ViewBag.Windows = windows;

            return View(term_itinfo);
        }

        // POST: Term_Employees/EditITInfo/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditITInfo([Bind(Include = "Id,EmpID,WindowsAccAccess,ArchiveEmail,EmailAtW,FowardEmails,FowardAtW,DeleteUDrive,UDriveAtW,DeskPhone,DeskPhoneFW,DeskPhoneFWtW,DeskPhoneRename,PhoneRenametW,EpicorACC,EpicorUserID")] Term_ITInfo term_itinfo, string ddl)
        {
            if (ddl == "None") term_itinfo.WindowsAccAccess = "No Windows Account";
            else if (ddl == "Active") term_itinfo.WindowsAccAccess = "Active Windows Account";
            else term_itinfo.WindowsAccAccess = "Deactivate Windows Account";

            if (term_itinfo.ArchiveEmail == false) term_itinfo.EmailAtW = "N/A";
            if (term_itinfo.FowardEmails == false) term_itinfo.FowardAtW = "N/A";
            if (term_itinfo.DeleteUDrive == false) term_itinfo.UDriveAtW = "N/A";
            if (term_itinfo.DeskPhone == false)
            {
                term_itinfo.DeskPhoneFW = false;
                term_itinfo.DeskPhoneFWtW = "N/A";
                term_itinfo.DeskPhoneRename = false;
                term_itinfo.PhoneRenametW = "N/A";
            }
            if (term_itinfo.DeskPhoneFW == false) term_itinfo.DeskPhoneFWtW = "N/A";
            if (term_itinfo.DeskPhoneRename == false) term_itinfo.PhoneRenametW = "N/A";

            if (ModelState.IsValid)
            {
                db.Entry(term_itinfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = db.Term_Employees.Where(x => x.Id == term_itinfo.EmpID).Select(x => x.Id).Single() });
            }
            return View(term_itinfo);
        }

        // GET: Term_Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Term_Employees term_Employees = db.Term_Employees.Find(id);
            if (term_Employees == null)
            {
                return HttpNotFound();
            }
            return View(term_Employees);
        }

        // POST: Term_Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Term_Employees term_Employees = db.Term_Employees.Find(id);
            Term_Property term_Property = db.Term_Property.Where(x => x.EmpID == id).Single();
            Term_ITInfo term_ITInfo = db.Term_ITInfo.Where(x => x.EmpID == id).Single();

            db.Term_ITInfo.Remove(term_ITInfo);
            db.Term_Property.Remove(term_Property);
            db.Term_Employees.Remove(term_Employees);
            db.SaveChanges();
            return RedirectToAction("Index");
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
