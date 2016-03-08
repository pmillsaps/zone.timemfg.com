//using CrystalDecisions.CrystalReports.Engine;
//using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

//using EpicWeb.Helpers;
//using EpicWeb.Logging;
//using EpicWeb.Models;
//using EpicWeb.ViewModels;
using MoreLinq;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Epicor;
using Time.Data.EntityModels.TimeMFG;
using Time.Epicor.ViewModels;
using Time.Support.Helpers;

//using VersaliftDataServices.EntityModels.Epicor;
//using VersaliftDataServices.EntityModels.TimeMfg;
//using VersaliftDataServices.Models;

namespace Time.Epicor.Controllers
{
    [Themed]
    [Authorize]
    public class LoadListController : Controller
    {
        private readonly TimeMFGEntities _db;
        private readonly EpicorEntities _epicor;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        //private readonly ILogger _logger;
        public ILogger Logger { get; set; }

        private DateTime DefaultDate = DateTime.Parse("1900,01,01");

        private const string DbLogon = "TimeMFGApp";
        private const string DbPassword = "Tm@Time$!";
        private const string DbServer = "Aruba-Sql";

        public LoadListController(IOrchardServices services)
        {
            _db = new TimeMFGEntities();
            _epicor = new EpicorEntities();
            Logger = NullLogger.Instance;
            _epicor.Database.CommandTimeout = 600;
            Services = services;
            T = NullLocalizer.Instance;
            GetPermissions();
        }

        public LoadListController(TimeMFGEntities db, EpicorEntities epicor, IOrchardServices services)
        {
            _db = db;
            _epicor = epicor;
            Services = services;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
            GetPermissions();
        }

        //
        // GET: /LoadList/
        public ActionResult Index(int id = 0)
        {
            var loadlists = _db.LoadLists.AsQueryable();
            loadlists = loadlists.Where(x => x.Complete == id);
            loadlists = loadlists.OrderBy(x => x.Name).ThenBy(x => x.MakeReady);
            ViewBag.Complete = id;
            return View(loadlists);
        }

        //public ActionResult IndexComplete()
        //{
        //    var loadlists = _db.LoadLists.Where(x => x.Complete == 1).OrderBy(x => x.Name);
        //    return View(loadlists);
        //}

        private void GetPermissions()
        {
            if (Services.Authorizer.Authorize(Permissions.LoadListEditor))
                ViewBag.LoadListEditor = true;
            else
                ViewBag.LoadListEditor = false;
        }

        public ActionResult AsmInspect(AsmInspectViewModel vm = null)
        {
            if (vm == null) vm = new AsmInspectViewModel();
            var jobs = _db.LoadListJobs
                .Include(x => x.LoadList)
                .Where(x => x.LoadList.MakeReady == 1 ||
                    x.LoadList.Complete != 1 &&
                    x.LoadList.DateSchedShip <= vm.EndDate).AsEnumerable();
            // jobs = jobs.Where(x => x.LShip != true);

            jobs = jobs.Where(x =>
                (!vm.Claimed && x.Claimed == false) ||
                (!vm.Tested && x.Tested == false) ||
                (!vm.Posted && x.Posted == false) ||
                (!vm.Green && x.Green == false));

            jobs = jobs.OrderBy(x => x.LoadList.MakeReady)
                .ThenBy(x => x.LoadList.DateSchedShip)
                .ThenBy(x => x.CustomerName);

            vm.JobList = jobs.ToList();

            return View(vm);
        }

        //
        // GET: /LoadList/Details/5
        public ActionResult Details(int id)
        {
            var load = _db.LoadLists.FirstOrDefault(x => x.Id == id);
            if (load == null) return RedirectToAction("Index");

            return View(load);
        }

        //
        // GET: /LoadList/MoveLoadListJob/5
        [HttpGet]
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult MoveLoadListJob(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            // Single version, callable from the Edit job Screen
            var loadListJob = _db.LoadListJobs.FirstOrDefault(x => x.Id == id);
            if (loadListJob == null) return RedirectToAction("EditJob", new { id = id });

            var loadLists = new SelectList(_db.LoadLists.Where(x => x.Complete != 1 && x.Id != loadListJob.LoadListId).OrderBy(x => x.Name),
                "id", "name");

            var vm = new MoveLoadListJobVM() { LoadListJobId = id, LoadLists = loadLists };

            return View(vm);
        }

        //
        // POST: /LoadList/MoveLoadListJob/5
        //[Authorize(Roles = "LoadListEditor")]
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult MoveLoadListJob(MoveLoadListJobVM vm)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            var llj = _db.LoadListJobs.FirstOrDefault(x => x.Id == vm.LoadListJobId);
            if (ModelState.IsValid)
            {
                if (vm.LoadList != 0)
                {
                    // This section will move the LoadListJob
                    if (llj != null)
                    {
                        var origLL = llj.LoadListId;
                        llj.LoadListId = vm.LoadList;
                        _db.SaveChanges();
                        // Log the Load List Change
                        Logger.Information("");
                        //Logger.Info("");
                        return RedirectToAction("Details", new { id = origLL });
                    }
                }
            }

            vm.LoadLists = new SelectList(_db.LoadLists.Where(x => x.Complete != 1 && x.Id != llj.LoadListId).OrderBy(x => x.Name),
                "id", "name");

            return View(vm);
        }

        //
        // GET: /LoadList/MoveLoadListJobs/5
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult MoveLoadListJobs(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            var load = _db.LoadLists.FirstOrDefault(x => x.Id == id);
            if (load == null) return RedirectToAction("Index");

            return View(load);
        }

        //
        // POST: /LoadList/MoveLoadListJobs/5
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult MoveLoadListJobs(LoadList load, List<int> selectedItems)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            return View(load);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult CommentWorkFlow(int page = 0)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            var load = _db.LoadLists.OrderByDescending(x => x.MakeReady).OrderBy(x => x.DateSchedShip).Skip(page);

            //if (page != 0)
            //{
            //    //load = (IOrderedQueryable<LoadList>)load.SkipUntil(x => x.Id == id);
            //    load = (IOrderedQueryable<LoadList>)load.Skip(page);
            //}

            var current = load.FirstOrDefault();

            if (current == null) return RedirectToAction("Index");

            ViewBag.CurrentPage = page + 1;
            return View(current);
        }

        //
        // POST: /LoadList/CommentWorkFlow
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult CommentWorkFlow(LoadList load, List<int> selectedItems)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            return View(load);
        }

        //
        // GET: /LoadList/Create
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            LoadList load = new LoadList() { DateIssued = DateTime.Now };
            var llvm = new LoadListView() { LoadList = load };

            return View(llvm);
        }

        //
        // POST: /LoadList/Create
        [HttpPost]
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult Create(LoadListView load)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            load.LoadList.Complete = Convert.ToByte(load.Complete);
            load.LoadList.MakeReady = Convert.ToByte(load.MakeReady);

            var exists = _db.LoadLists.FirstOrDefault(x => x.Name == load.LoadList.Name);
            if (exists != null)
                ModelState.AddModelError("LoadList.Name", "Duplicate Load List Name...Please Correct");

            if (ModelState.IsValid)
            {
                var newLoad = _db.LoadLists.Add(load.LoadList);
                _db.SaveChanges();

                if (!String.IsNullOrEmpty(load.LoadList.Comments)) SaveLoadListComment(load.LoadList);

                return RedirectToAction("JobSearch", new { id = newLoad.Id });
            }

            return View(load);
        }

        private void SaveLoadListComment(LoadList loadList)
        {
            var lastComment = "";
            var lastEntry = _db.LoadListComments.OrderByDescending(x => x.Date).FirstOrDefault(x => x.LoadListId == loadList.Id);
            if (lastEntry != null) lastComment = lastEntry.Comment;

            if (lastComment != loadList.Comments)
            {
                var llc = new LoadListComment
                {
                    LoadListId = loadList.Id,
                    Date = DateTime.Now,
                    Comment = loadList.Comments,
                    EnteredBy = HttpContext.User.Identity.Name,
                };

                _db.LoadListComments.Add(llc);
                _db.SaveChanges();
            }
        }

        private void SaveLoadListJobComment(LoadListJob job)
        {
            var lastComment = "";
            var lastEntry = _db.LoadListJobComments.OrderByDescending(x => x.Date).FirstOrDefault(x => x.LoadListJobId == job.Id);
            if (lastEntry != null) lastComment = lastEntry.Comment;

            if (lastComment != job.Comments)
            {
                var llc = new LoadListJobComment
                {
                    LoadListJobId = job.Id,
                    Date = DateTime.Now,
                    Comment = job.Comments,
                    EnteredBy = HttpContext.User.Identity.Name,
                };

                _db.LoadListJobComments.Add(llc);
                _db.SaveChanges();
            }
        }

        //
        // GET: /LoadList/Edit/5

        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult Edit(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            var load = _db.LoadLists.FirstOrDefault(x => x.Id == id);
            if (load == null) return HttpNotFound();
            var llvm = new LoadListView() { LoadList = load, Complete = load.Complete == 1, MakeReady = load.MakeReady == 1 };

            return View(llvm);
        }

        //
        // POST: /LoadList/Edit/5
        [HttpPost]
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult Edit(LoadListView vm)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            var loadList = vm.LoadList;

            var exists = _db.LoadLists.FirstOrDefault(x => x.Name == vm.LoadList.Name && x.Id != vm.LoadList.Id);
            if (exists != null)
                ModelState.AddModelError("LoadList.Name", "Duplicate Load List Name...Please Correct");

            if (ModelState.IsValid)
            {
                _db.LoadLists.Attach(loadList);
                _db.Entry(loadList).State = EntityState.Modified;
                //_db.ObjectStateManager.ChangeObjectState(loadList, EntityState.Modified);
                loadList.DateRevised = DateTime.Now;
                loadList.Complete = Convert.ToByte(vm.Complete);
                loadList.MakeReady = Convert.ToByte(vm.MakeReady);
                _db.SaveChanges();

                SaveLoadListComment(loadList);

                return RedirectToAction("Index");
            }

            return View(vm);
        }

        [HttpGet]
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult Delete(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            var load = _db.LoadLists.FirstOrDefault(x => x.Id == id);
            if (load == null) return HttpNotFound();

            return View(load);
        }

        //[Authorize(Roles = "LoadListEditor")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            var load = _db.LoadLists.Single(x => x.Id == id);
            load.LoadListDistributors.Clear();

            var loadListJobs = _db.LoadListJobs.Where(x => x.LoadListId == id);
            _db.LoadListJobs.RemoveRange(loadListJobs);

            _db.LoadLists.Remove(load);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult CompleteLoadList(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            var load = _db.LoadLists.FirstOrDefault(x => x.Id == id);
            if (load == null) return HttpNotFound();

            return View(load);
        }

        //[Authorize(Roles = "LoadListEditor")]
        [HttpPost, ActionName("CompleteLoadList")]
        public ActionResult CompleteLoadListConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            var load = _db.LoadLists.Single(x => x.Id == id);

            load.Complete = 1;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult _ListLifts(int id = 0)
        {
            IList<LoadListJob> jobs = new List<LoadListJob>();
            if (id != 0)
            {
                jobs = _db.LoadListJobs.Where(x => x.LoadListId == id)
                    .OrderBy(x => x.DateATS)
                    .ThenBy(x => x.JobNumber)
                    .Include(x => x.LoadListJobStatus).ToList();
            }

            return PartialView(jobs);
        }

        public ActionResult _ListJobComments(int id = 0)
        {
            IList<LoadListJobComment> comments = new List<LoadListJobComment>();
            if (id != 0)
            {
                comments = _db.LoadListJobComments.Where(x => x.LoadListJobId == id).ToList();
            }

            return PartialView(comments);
        }

        public ActionResult _ListLoadComments(int id = 0)
        {
            IList<LoadListComment> comments = new List<LoadListComment>();
            if (id != 0)
            {
                comments = _db.LoadListComments.Where(x => x.LoadListId == id).ToList();
            }

            return PartialView(comments);
        }

        [HttpGet]
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult JobSearch(int id = 0)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            if (id == 0) return HttpNotFound();
            var vm = new AddLiftViewModel { LoadListId = id };

            return View(vm);
        }

        [HttpPost]
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult JobSearch(AddLiftViewModel vm)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            return View(vm);
        }

        public ActionResult _EmailList()
        {
            return PartialView(_db.LoadListEmails);
        }

        public ActionResult _JobList(AddLiftViewModel vm)
        {
            var jobsEntered = _db.LoadListJobs.Select(x => x.JobNumber).ToList();
            var jobQuery = _epicor.v_JobInformation.Where(x => !jobsEntered.Contains(x.jobnum) && !String.IsNullOrEmpty(x.serialnumber)).AsQueryable();
            if (!String.IsNullOrEmpty(vm.Customer))
            {
                var custnum = int.Parse(vm.Customer);
                jobQuery = jobQuery.Where(x => x.custnum == custnum && x.jobclosed != 1);
            }

            if (!String.IsNullOrEmpty(vm.Search)) jobQuery = jobQuery.Where(x => x.jobnum.Contains(vm.Search));

            List<v_JobInformation> jobs = new List<v_JobInformation>();
            if (!String.IsNullOrEmpty(vm.Customer) || !String.IsNullOrEmpty(vm.Search))
            {
                jobs = jobQuery.OrderBy(x => x.reqduedate).ToList();
            }

            return PartialView(jobs);
        }

        [OutputCache(Duration = 10)]
        public ActionResult _CustomerList(string customer)
        {
            var cust = new SelectList(_epicor.v_CustomersWithOpenOrderPartLines.Where(x => x.openorder == 1 && x.voidorder != 1 && x.prodcode.Contains("lift")).DistinctBy(x => x.custnum)
                .OrderBy(x => x.name),
                "custnum", "name", customer);

            ViewBag.Customers = cust;

            return PartialView();
        }

        [HttpPost]
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult AddLift(int id, List<string> selectedLines)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            foreach (var job in selectedLines)
            {
                AddLiftToLoadList(id, job);
            }

            return RedirectToAction("Details", new { id = id });
        }

        private void AddLiftToLoadList(int id, string job)
        {
            //var jobProd = _epicor.v_JobProd.FirstOrDefault(x => x.jobnum == job);
            //var salesOrder = _epicor..FirstOrDefault(x => x.)
            var jobinfo = _epicor.v_JobInformation.FirstOrDefault(x => x.jobnum == job);

            var loadListJob = new LoadListJob
            {
                LoadListId = id,
                JobNumber = job.ToUpper(),
                LiftModel = jobinfo.partnum,
                SerialNo = jobinfo.serialnumber,
                DateATS = jobinfo.reqduedate ?? DefaultDate,
                Destination = String.Format("{0} | {1} | {2}", jobinfo.city, jobinfo.state, jobinfo.country),
                CustomerName = jobinfo.CustName,
                CustomerId = jobinfo.custnum ?? 0,
                Comments = "",
                DistributorPO = jobinfo.ponum,
                ShipTo = String.Format(@"{0}/{1}", jobinfo.custid, jobinfo.shiptonum),
                SalesOrder = String.Format("{0}-{1}-{2}", jobinfo.ordernum, jobinfo.orderline, jobinfo.orderrelnum),
            };

            var loadlist = _db.LoadLists.First(x => x.Id == id);
            var distributor = _db.LoadListDistributors.FirstOrDefault(x => x.CustomerId == jobinfo.custnum);
            if (distributor == null && jobinfo.custnum != null) AddNewDistributor(jobinfo);

            distributor = _db.LoadListDistributors.First(x => x.CustomerId == jobinfo.custnum);

            if (!loadlist.LoadListDistributors.Contains(distributor))
            {
                loadlist.LoadListDistributors.Add(distributor);
                _db.SaveChanges();
            }

            try
            {
                _db.LoadListJobs.Add(loadListJob);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            AddNewJobOperations(loadListJob.JobNumber);
        }

        private void AddNewDistributor(v_JobInformation jobinfo)
        {
            _db.LoadListDistributors.Add(new LoadListDistributor
            {
                CustomerId = jobinfo.custnum ?? 0,
                Name = jobinfo.CustName
            });
            _db.SaveChanges();
        }

        //
        // GET: /LoadList/EditJob/5
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult EditJob(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            var job = _db.LoadListJobs.Include(x => x.LoadListJobStatus).SingleOrDefault(x => x.Id == id);
            if (job == null) return HttpNotFound();
            //var llvm = new LoadListView() { LoadList = job, Complete = job.Complete == 1 };

            return View(job);
        }

        //
        // POST: /LoadList/EditJob/5
        [HttpPost]
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult EditJob(LoadListJob job)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            if (ModelState.IsValid)
            {
                _db.LoadListJobs.Attach(job);
                _db.Entry(job).State = EntityState.Modified;
                //_db.ObjectStateManager.ChangeObjectState(loadList, EntityState.Modified);
                //job.DateRevised = DateTime.Now;
                //job.Complete = Convert.ToByte(job.Complete);
                _db.SaveChanges();

                SaveLoadListJobComment(job);

                return RedirectToAction("Details", new { id = job.LoadListId });
            }

            return View(job);
        }

        [HttpGet]
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult DeleteJob(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            var job = _db.LoadListJobs.FirstOrDefault(x => x.Id == id);
            if (job == null) return HttpNotFound();

            return View(job);
        }

        //[Authorize(Roles = "LoadListEditor")]
        [HttpPost, ActionName("DeleteJob")]
        public ActionResult DeleteJobConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            var job = _db.LoadListJobs.Single(x => x.Id == id);
            var loadlist = _db.LoadLists.Single(x => x.Id == job.LoadListId);
            // Added null reference check to filter out bad customers PEM 20140630
            if (loadlist.LoadListJobs.Where(x => x.CustomerId == job.CustomerId).Count() == 1
                && loadlist.LoadListDistributors.FirstOrDefault(x => x.CustomerId == job.CustomerId) != null)
                loadlist.LoadListDistributors
                    .Remove(loadlist.LoadListDistributors
                    .First(x => x.CustomerId == job.CustomerId));

            _db.LoadListJobs.Remove(job);
            _db.SaveChanges();
            return RedirectToAction("Details", new { id = job.LoadListId });
        }

        public ActionResult JobDetails(int id)
        {
            var job = _db.LoadListJobs.Include(x => x.LoadListJobStatus).SingleOrDefault(x => x.Id == id);
            if (job == null) return HttpNotFound();
            //var llvm = new LoadListView() { LoadList = job, Complete = job.Complete == 1 };

            return View(job);
        }

        [HttpGet]
        public ActionResult PrintLoadLists()
        {
            var loadlists = _db.LoadLists.Where(x => x.Complete != 1).OrderBy(x => x.Name);

            ViewBag.View = "Open";
            return View(loadlists);
        }

        [HttpGet]
        public ActionResult PrintLoadListsCompleted()
        {
            var loadlists = _db.LoadLists.Where(x => x.Complete == 1).OrderBy(x => x.Name);
            ViewBag.View = "Complete";
            return View("PrintLoadLists", loadlists);
        }

        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult EmailIndex()
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            var emails = _db.LoadListEmails;
            return View(emails);
        }

        // GET: /LoadListEmail/Create
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult EmailCreate()
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            return View();
        }

        // POST: /LoadListEmail/Create
        // To protect from overposting attacks, please enable the specific properties you want to
        // bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult EmailCreate([Bind(Include = "Name,Email")] LoadListEmail loadlistemail)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            if (_db.LoadListEmails.FirstOrDefault(x => x.Email == loadlistemail.Email) != null)
                ModelState.AddModelError("Email", "Duplicate emails are not allowed.");
            if (_db.LoadListEmails.FirstOrDefault(x => x.Name == loadlistemail.Name) != null)
                ModelState.AddModelError("Name", "Duplicate names are not allowed.");

            if (ModelState.IsValid)
            {
                _db.LoadListEmails.Add(loadlistemail);
                _db.SaveChanges();
                return RedirectToAction("EmailIndex");
            }

            return View(loadlistemail);
        }

        // GET: /LoadListEmail/Edit/5
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult EmailEdit(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoadListEmail loadlistemail = _db.LoadListEmails.Find(id);
            if (loadlistemail == null)
            {
                return HttpNotFound();
            }
            return View(loadlistemail);
        }

        // POST: /LoadListEmail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to
        // bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult EmailEdit([Bind(Include = "Id,Name,Email")] LoadListEmail loadlistemail)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            if (_db.LoadListEmails.FirstOrDefault(x => x.Email == loadlistemail.Email && x.Id != loadlistemail.Id) != null)
                ModelState.AddModelError("Email", "Duplicate emails are not allowed.");
            if (_db.LoadListEmails.FirstOrDefault(x => x.Name == loadlistemail.Name && x.Id != loadlistemail.Id) != null)
                ModelState.AddModelError("Name", "Duplicate names are not allowed.");

            if (ModelState.IsValid)
            {
                _db.Entry(loadlistemail).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("EmailIndex");
            }
            return View(loadlistemail);
        }

        // GET: /LoadListEmail/Delete/5
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult EmailDelete(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoadListEmail loadlistemail = _db.LoadListEmails.Find(id);
            if (loadlistemail == null)
            {
                return HttpNotFound();
            }
            return View(loadlistemail);
        }

        // POST: /LoadListEmail/Delete/5
        [HttpPost, ActionName("EmailDelete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "LoadListEditor")]
        public ActionResult EmailDeleteConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            LoadListEmail loadlistemail = _db.LoadListEmails.Find(id);
            _db.LoadListEmails.Remove(loadlistemail);
            _db.SaveChanges();
            return RedirectToAction("EmailIndex");
        }

        [HttpGet]
        public ActionResult EmailLoadList(int id)
        {
            var loadlist = _db.LoadLists.Single(x => x.Id == id);
            EmailLoadListVM vm = new EmailLoadListVM() { LoadListId = id, LoadList = loadlist };

            return View(vm);
        }

        [HttpPost]
        public ActionResult EmailLoadList(EmailLoadListVM vm)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            if (vm.selectedLines == null)
            {
                ModelState.AddModelError("", "You must select at least one email address...");
                var loadlist = _db.LoadLists.Single(x => x.Id == vm.LoadListId);
                return View(loadlist);
            }

            // Now generate the PDF then Email it to the selected email addresses The Crystal
            // Reports version
            string fileName = String.Format("LoadList_{0}.pdf", DateTime.Now.ToShortDateString().Replace(@"/", "").Replace(@"\", ""));
            fileName.Replace(@"/", "").Replace(@"\", "");
            var OutputDirectory = ConfigurationManager.AppSettings["LoadListEmailDirectory"];
            var emailPath = Server.MapPath(OutputDirectory);
            if (!Directory.Exists(emailPath)) Directory.CreateDirectory(emailPath);
            CleanOutOldAttachments(emailPath);
            var outputFile = Path.Combine(emailPath, fileName);
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Reports/LoadListExternal.rpt");
            rptH.Load();
            rptH.SetDatabaseLogon(DbLogon, DbPassword);
            rptH.SetParameterValue("SelectedItems", vm.LoadListId.ToString());

            rptH.ExportToDisk(ExportFormatType.PortableDocFormat, outputFile);
            if (!String.IsNullOrEmpty(vm.Comments)) vm.Comments = vm.Comments.Replace(Environment.NewLine, "<br />");

            string body = String.Format("Attached File: Load List {0}<br /><br />{1}", vm.LoadList.Name, vm.Comments);

            EmailNotifier.FromAddress = ConfigurationManager.AppSettings["LoadListFromAddress"];
            EmailNotifier.FromName = ConfigurationManager.AppSettings["LoadListFromName"];
            EmailNotifier.SendMail("Load List Attached", body, vm.selectedLines, true, true, outputFile);

            //Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            //return File(stream, "application/pdf", fileName);

            return RedirectToAction("Details", new { id = vm.LoadListId });
        }

        private void CleanOutOldAttachments(string emailPath)
        {
            DirectoryInfo di = new DirectoryInfo(emailPath);
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.CreationTime <= DateTime.Now.AddDays(-1)) fi.Delete();
            }
        }

        [HttpGet]
        public ActionResult EmailLoadLists()
        {
            var loadlists = _db.LoadLists.Where(x => x.Complete != 1).OrderBy(x => x.Name).ThenBy(x => x.MakeReady);

            ViewBag.View = "Open";
            return View(loadlists);
        }

        [HttpPost]
        public ActionResult PrintLoadLists(List<int> selectedLines, string command, string EmailAddress)
        {
            if (selectedLines != null)
            {
                switch (command)
                {
                    case "Print Load List(s)...":
                        return PrintedLoadLists(selectedLines);
                    //break;
                    case "Print Load List(s) With Statuses...":
                        return PrintedLoadListsStatus(selectedLines);
                    //break;
                    case "Email Selected Load Lists...":
                        if (!String.IsNullOrEmpty(EmailAddress))
                            return EmailLoadLists(selectedLines, EmailAddress);
                        ModelState.AddModelError("", "You must fill in the email on this page to email the loadlists");
                        break;
                }
            }
            else
                ModelState.AddModelError("", "You must select at least one LoadList to continue...");

            var loadlists = _db.LoadLists.Where(x => x.Complete != 1).OrderBy(x => x.DateSchedShip);

            ViewBag.View = "Open";
            return View(loadlists);
        }

        private ActionResult PrintedLoadLists(List<int> selectedLines)
        {
            // The Crystal Reports version
            string fileName = String.Format("LoadLists_{0}.pdf", DateTime.Now.ToShortDateString().Replace(@"/", "").Replace(@"\", ""));
            fileName.Replace(@"/", "").Replace(@"\", "");
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Reports/LoadList.rpt");
            rptH.Load();
            rptH.SetDatabaseLogon(DbLogon, DbPassword);

            rptH.SetParameterValue("SelectedItems", string.Join(",", selectedLines));

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf", fileName);
        }

        private ActionResult PrintedLoadListsStatus(List<int> selectedLines)
        {
            // The Crystal Reports version
            string fileName = String.Format("LoadListsStatus_{0}.pdf", DateTime.Now.ToShortDateString().Replace(@"/", "").Replace(@"\", ""));
            fileName.Replace(@"/", "").Replace(@"\", "");
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Reports/LoadListStatus.rpt");
            rptH.Load();
            rptH.SetDatabaseLogon(DbLogon, DbPassword);

            rptH.SetParameterValue("SelectedItems", string.Join(",", selectedLines));

            Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf", fileName);
        }

        private ActionResult EmailLoadLists(List<int> selectedLines, string EmailAddress)
        {
            return View("EmailLoadLists");
        }

        private void AddNewJobOperations(string jobNumber)
        {
            var newLift = _db.LoadListJobs.Single(x => x.JobNumber == jobNumber);

            var jobOpers = _epicor.jobopers.Where(x => x.jobnum == newLift.JobNumber && x.assemblyseq == 1 && x.opcode != "593100" &&
                !(x.opcode.ToUpper() == "LSHIP" && x.assemblyseq == 0));

            foreach (var oper in jobOpers.OrderBy(x => x.assemblyseq).ThenBy(x => x.oprseq))
            {
                var existing = _db.LoadListJobStatus.SingleOrDefault(x => x.JobNumber == oper.jobnum && x.OpCode == oper.opcode);
                if (existing == null)
                {
                    var lls = new LoadListJobStatu
                    {
                        AssemblySeq = oper.assemblyseq ?? 0,
                        JobNumber = newLift.JobNumber,
                        LoadListJobId = newLift.Id,
                        OpCode = oper.opcode,
                        OpComplete = oper.opcomplete ?? 0,
                        OprSeq = oper.oprseq ?? 0
                    };
                    _db.LoadListJobStatus.Add(lls);
                    _db.SaveChanges();
                }
                else
                {
                    if (existing.AssemblySeq == 0 && oper.assemblyseq == 1)
                    {
                        existing.AssemblySeq = 1;
                        existing.OprSeq = oper.oprseq ?? 0;
                        if (oper.opcomplete == 1) existing.OpComplete = 1;
                        _db.SaveChanges();
                    }
                }
            }
        }

        //[Authorize(Roles = "LoadListEditor")]
        [HttpGet]
        public ActionResult UploadPictures(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            var load = _db.LoadLists.Single(x => x.Id == id);
            ViewBag.Id = id;
            ViewBag.Name = load.Name;

            return View();
        }

        //[Authorize(Roles = "LoadListEditor")]
        [HttpPost]
        public ActionResult UploadPictures(HttpPostedFileBase file, int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            //var result = "";
            if (file != null)
            {
                var imgFileName = Path.GetFileName(file.FileName);
                var count = 1;
                var img = _db.LoadListImages.SingleOrDefault(x => x.LoadListId == id && x.FileName == imgFileName);
                while (img != null)
                {
                    imgFileName = String.Format("{0}_{1:00}{2}",
                        Path.GetFileNameWithoutExtension(file.FileName),
                        count,
                        Path.GetExtension(file.FileName));

                    img = _db.LoadListImages.SingleOrDefault(x => x.FileName == imgFileName);
                }

                try
                {
                    var f = new LoadListImage();
                    f.LoadListId = id;
                    f.FileName = imgFileName;
                    f.UploadedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    f.UploadedDate = DateTime.Now;
                    f.FilePath = String.Format(@"\Content\LoadListImages\{0}\", id);
                    try
                    {
                        _db.LoadListImages.Add(f);
                        _db.SaveChanges();
                        file.StoreLoadListFile(id);
                        ViewBag.Notice = "File was uploaded successfully!";
                    }
                    catch (Exception err)
                    {
                        ViewBag.Error = "Could not save file, Please try again later";
                        ErrorTools.SendEmail(Request.Url, err, User.Identity.Name);
                        return Json(new { error = ViewBag.Error }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    ErrorTools.SendEmail(Request.Url, err, User.Identity.Name);
                }
            }

            var load = _db.LoadLists.Single(x => x.Id == id);

            ViewBag.Id = id;
            ViewBag.Name = load.Name;
            return View();
        }

        [ChildActionOnly]
        public ActionResult _DisplayThumbNails(int id)
        {
            var images = _db.LoadListImages.Where(x => x.LoadListId == id);
            ViewBag.Id = id;
            return PartialView(images);
        }

        public ActionResult ThumbImage(int id, int width, int height)
        {
            var img = _db.LoadListImages.SingleOrDefault(x => x.Id == id);
            if (img != null)
            {
                Image i = null;
                try
                {
                    i = Image.FromFile(Server.MapPath(Path.Combine(img.FilePath, img.FileName)));
                    return new ImageResult(i, width, height);
                }
                catch (Exception)
                {
                    i = new Bitmap(1, 1);
                    return new ImageResult(i, 1, 1);
                }
                finally
                {
                    if (i != null) i.Dispose();
                }
            }

            return null;
        }

        [HttpGet]
        public ActionResult SearchLoadLists()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchLoadLists(string Search)
        {
            if (!String.IsNullOrEmpty(Search))
                return View(_db.LoadListJobs.Where(x => x.JobNumber.Contains(Search)));

            return View();
        }
    }
}