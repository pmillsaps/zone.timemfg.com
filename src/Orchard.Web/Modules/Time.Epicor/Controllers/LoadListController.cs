using CrystalDecisions.CrystalReports.Engine;
using MoreLinq;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Production;
using Time.Data.EntityModels.TimeMFG;
using Time.Epicor.ViewModels;
using Time.Support.Helpers;

namespace Time.Epicor.Controllers
{
    [Themed]
    [Authorize]
    public class LoadListController : Controller
    {
        private readonly TimeMFGEntities _db;
        private readonly ProductionEntities production;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        private DateTime DefaultDate = DateTime.Parse("1900,01,01");

        private const string DbLogon = "TimeMFGApp";
        private const string DbPassword = "Tm@Time$!";
        private const string DbServer = "Aruba-Sql";

        public LoadListController(IOrchardServices services)
        {
            _db = new TimeMFGEntities();
            production = new ProductionEntities();
            Logger = NullLogger.Instance;
            production.Database.CommandTimeout = 600;
            Services = services;
            T = NullLocalizer.Instance;
            GetPermissions();
        }

        public LoadListController(TimeMFGEntities db, ProductionEntities _production, IOrchardServices services)
        {
            _db = db;
            production = _production;
            Services = services;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
            GetPermissions();
        }

        //
        // GET: /LoadList/
        public ActionResult Index(int? LoadListYear, int? id = 0)
        {
            ViewBag.LoadListYear = new SelectList(_db.LoadLists.Select(x => new { x.DateIssued.Year }).Distinct().OrderByDescending(x => x.Year), "Year", "Year");
            var loadlists = _db.LoadLists.AsQueryable();

            if (id == 0)
            {
                loadlists = loadlists.Where(x => x.Complete == (id == 1));
                loadlists = loadlists.OrderBy(x => x.Name).ThenBy(x => x.MakeReady);
            }
            else
            {
                if (LoadListYear == null) loadlists = loadlists.Where(x => x.Complete == true && (x.DateSchedShip.Value.Year == DateTime.Now.Year || x.DateSchedShip.Value.Year == null)).OrderBy(x => x.Name);
                else loadlists = loadlists.Where(x => x.Complete == true && (x.DateSchedShip.Value.Year == LoadListYear || x.DateSchedShip.Value.Year == null)).OrderBy(x => x.Name);
            }

            ViewBag.Complete = id;
            return View(loadlists);
        }

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
                .Where(x => x.LoadList.MakeReady ||
                    x.LoadList.Complete != true &&
                    x.LoadList.DateSchedShip <= vm.EndDate).AsEnumerable();
            // jobs = jobs.Where(x => x.LShip != true);

            jobs = jobs.Where(x =>
                (!vm.Claimed && x.Claimed == false) ||
                (!vm.Tested && x.Tested == false) ||
                (!vm.Posted && x.Blue == false) ||
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

            var loadLists = new SelectList(_db.LoadLists.Where(x => x.Complete != true && x.Id != loadListJob.LoadListId).OrderBy(x => x.Name),
                "id", "name");

            var vm = new MoveLoadListJobVM() { LoadListJobId = id, LoadLists = loadLists, JobNumber = loadListJob.JobNumber, OriginalLLId = loadListJob.LoadListId };

            return View(vm);
        }

        //
        // POST: /LoadList/MoveLoadListJob/5
        //[Authorize(Roles = "LoadListEditor")]
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
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
                        // These lines adds or removes the distributor in the Load List
                        CheckDistributorLoadList(llj.LoadListId);
                        CheckDistributorLoadList(vm.OriginalLLId);
                        return RedirectToAction("Details", new { id = origLL });
                    }
                }
            }

            vm.LoadLists = new SelectList(_db.LoadLists.Where(x => x.Complete != true && x.Id != llj.LoadListId).OrderBy(x => x.Name),
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
            load.LoadList.Complete = load.Complete;
            load.LoadList.MakeReady = load.MakeReady;

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
            var llvm = new LoadListView() { LoadList = load, Complete = load.Complete == true, MakeReady = load.MakeReady == true };

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
                loadList.Complete = vm.Complete;
                loadList.MakeReady = vm.MakeReady;
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

            load.Complete = true;
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
            var jobQuery = production.V_JobInformation.Where(x => !jobsEntered.Contains(x.JobNum) && !String.IsNullOrEmpty(x.SerialNumber)).AsQueryable();
            if (!String.IsNullOrEmpty(vm.Customer))
            {
                var custnum = int.Parse(vm.Customer);
                jobQuery = jobQuery.Where(x => x.CustNum == custnum && x.JobClosed != true);
            }

            if (!String.IsNullOrEmpty(vm.Search)) jobQuery = jobQuery.Where(x => x.JobNum.Contains(vm.Search));

            List<V_JobInformation> jobs = new List<V_JobInformation>();
            if (!String.IsNullOrEmpty(vm.Customer) || !String.IsNullOrEmpty(vm.Search))
            {
                jobs = jobQuery.OrderBy(x => x.ReqDueDate).ToList();
            }

            return PartialView(jobs);
        }

        [OutputCache(Duration = 10)]
        public ActionResult _CustomerList(string customer)
        {
            var cust = new SelectList(production.V_CustomersWithOpenOrderPartLines.Where(x => x.OpenOrder == true && x.VoidOrder != true && x.ProdCode.Contains("lift")).DistinctBy(x => x.CustNum)
                .OrderBy(x => x.Name),
                "custnum", "name", customer);

            ViewBag.Customers = cust;

            return PartialView();
        }

        [HttpPost]
        //[Authorize(Roles = "LoadListEditor")]
        [ValidateAntiForgeryToken]
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
            var jobinfo = production.V_JobInformation.FirstOrDefault(x => x.JobNum == job);

            var loadListJob = new LoadListJob
            {
                LoadListId = id,
                JobNumber = job.ToUpper(),
                LiftModel = jobinfo.PartNum,
                SerialNo = jobinfo.SerialNumber,
                DateATS = jobinfo.ReqDueDate ?? DefaultDate,
                Destination = String.Format("{0} | {1} | {2}", jobinfo.City, jobinfo.State, jobinfo.Country),
                CustomerName = jobinfo.CustName,
                CustomerId = jobinfo.CustNum ?? 0,
                Comments = "",
                DistributorPO = jobinfo.PONum,
                ShipTo = String.Format(@"{0}/{1}", jobinfo.CustID, jobinfo.ShipToNum),
                SalesOrder = String.Format("{0}-{1}-{2}", jobinfo.OrderNum, jobinfo.OrderLine, jobinfo.OrderRelNum),
            };

            //var loadlist = _db.LoadLists.First(x => x.Id == id);
            //var distributor = _db.LoadListDistributors.FirstOrDefault(x => x.CustomerId == jobinfo.custnum);
            //if (distributor == null && jobinfo.custnum != null) AddNewDistributor(jobinfo);

            //distributor = _db.LoadListDistributors.First(x => x.CustomerId == jobinfo.custnum);

            //if (!loadlist.LoadListDistributors.Contains(distributor))
            //{
            //    loadlist.LoadListDistributors.Add(distributor);
            //    _db.SaveChanges();
            //}

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
            // These line adds the distributor to the load list
            CheckDistributorLoadList(id);
        }

        // This method is called to add or remove the Distributor(s) to(from) the Load List
        private void CheckDistributorLoadList(int id)
        {
            var loadlist = _db.LoadLists.First(x => x.Id == id);
            var jobs = _db.LoadListJobs.Where(x => x.LoadListId == loadlist.Id).ToList();
            // Adding the distributor if doesn't exist
            foreach (var j in jobs)
            {
                var distributor = _db.LoadListDistributors.FirstOrDefault(x => x.CustomerId == j.CustomerId);
                var jobinfo = production.V_JobInformation.FirstOrDefault(x => x.JobNum == j.JobNumber);
                if (distributor == null && jobinfo.CustNum != null) AddNewDistributor(jobinfo.CustNum, jobinfo.CustName);
            }
            // Adding and removing distributors from the Load List distributors
            var distributors = _db.LoadListDistributors.ToList();
            foreach (var d in distributors)
            {
                if (jobs.Any(x => x.CustomerName == d.Name))
                {
                    loadlist.LoadListDistributors.Add(d);
                    _db.SaveChanges();
                }
                else
                {
                    loadlist.LoadListDistributors.Remove(d);
                    _db.SaveChanges();
                }
            }
        }

        private void AddNewDistributor(int? id, string Name)
        {
            _db.LoadListDistributors.Add(new LoadListDistributor
            {
                CustomerId = id ?? 0,
                Name = Name
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
            //var job = _db.LoadListJobs.FirstOrDefault(x => x.Id == id);
            //if (job == null) return HttpNotFound();

            //return View(job);
            var job = _db.LoadListJobs.FirstOrDefault(x => x.Id == id);
            if (job == null) return HttpNotFound();

            var closedJob = production.V_JobInformation_NotClosed.FirstOrDefault(x => x.JobNum == job.JobNumber);
            if (closedJob == null)
            {
                ViewBag.ID = job.LoadListId;
                return View("_DeleteJobNotAllowed");
            }
            else
            {
                return View(job);
            }
        }

        //[Authorize(Roles = "LoadListEditor")]
        [HttpPost, ActionName("DeleteJob")]
        public ActionResult DeleteJobConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
                return new HttpUnauthorizedResult();
            var job = _db.LoadListJobs.Single(x => x.Id == id);
            //var loadlist = _db.LoadLists.Single(x => x.Id == job.LoadListId);
            //// Added null reference check to filter out bad customers PEM 20140630
            //if (loadlist.LoadListJobs.Where(x => x.CustomerId == job.CustomerId).Count() == 1
            //    && loadlist.LoadListDistributors.FirstOrDefault(x => x.CustomerId == job.CustomerId) != null)
            //    loadlist.LoadListDistributors
            //        .Remove(loadlist.LoadListDistributors
            //        .First(x => x.CustomerId == job.CustomerId));

            _db.LoadListJobs.Remove(job);
            _db.SaveChanges();
            // Removing the distributor tide to this job from the Load List distributors
            CheckDistributorLoadList(job.LoadListId);
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
        public ActionResult PrintLoadLists(int? LoadListYear, int id = 0)
        {
            //var loadlists = _db.LoadLists.Where(x => x.Complete == id).OrderBy(x => x.Name);

            //ViewBag.View = "Open";
            //return View(loadlists);
            ViewBag.LoadListYear = new SelectList(_db.LoadLists.Select(x => new { x.DateIssued.Year }).Distinct().OrderByDescending(x => x.Year), "Year", "Year");
            var loadlists = _db.LoadLists.AsQueryable();

            if (id == 0)
            {
                loadlists = loadlists.Where(x => x.Complete == (id == 1));
                loadlists = loadlists.OrderBy(x => x.Name).ThenBy(x => x.MakeReady);
            }
            else
            {
                if (LoadListYear == null) loadlists = loadlists.Where(x => x.Complete == true && (x.DateSchedShip.Value.Year == DateTime.Now.Year || x.DateSchedShip.Value.Year == null)).OrderBy(x => x.Name);
                else loadlists = loadlists.Where(x => x.Complete == true && (x.DateSchedShip.Value.Year == LoadListYear || x.DateSchedShip.Value.Year == null)).OrderBy(x => x.Name);
            }

            ViewBag.Complete = id;
            return View(loadlists);
        }

        //[HttpGet]
        //public ActionResult PrintLoadListsCompleted()
        //{
        //    var loadlists = _db.LoadLists.Where(x => x.Complete == 1).OrderBy(x => x.Name);
        //    ViewBag.View = "Complete";
        //    return View("PrintLoadLists", loadlists);
        //}

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

        //[HttpGet]
        //public ActionResult EmailLoadList(int id)
        //{
        //    var loadlist = _db.LoadLists.Single(x => x.Id == id);
        //    EmailLoadListVM vm = new EmailLoadListVM() { LoadListId = id, LoadList = loadlist };

        //    return View(vm);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EmailLoadList(EmailLoadListVM vm)
        //{
        //    if (!Services.Authorizer.Authorize(Permissions.LoadListEditor, T("Not Authorized")))
        //        return new HttpUnauthorizedResult();
        //    if (vm.selectedLines == null)
        //    {
        //        ModelState.AddModelError("", "You must select at least one email address...");
        //        var loadlist = _db.LoadLists.Single(x => x.Id == vm.LoadListId);
        //        return View(loadlist);
        //    }

        //    // Now generate the PDF then Email it to the selected email addresses The Crystal
        //    // Reports version
        //    string fileName = String.Format("LoadList_{0}.pdf", DateTime.Now.ToShortDateString().Replace(@"/", "").Replace(@"\", ""));
        //    fileName.Replace(@"/", "").Replace(@"\", "");
        //    var OutputDirectory = ConfigurationManager.AppSettings["LoadListEmailDirectory"];
        //    var emailPath = Server.MapPath(OutputDirectory);
        //    if (!Directory.Exists(emailPath)) Directory.CreateDirectory(emailPath);
        //    CleanOutOldAttachments(emailPath);
        //    var outputFile = Path.Combine(emailPath, fileName);
        //    ReportClass rptH = new ReportClass();
        //    rptH.FileName = Server.MapPath("~/Modules/Time.Epicor/Content/Reports/LoadListExternal.rpt");
        //    rptH.Load();
        //    rptH.SetDatabaseLogon(DbLogon, DbPassword);
        //    rptH.SetParameterValue("SelectedItems", vm.LoadListId.ToString());

        //    rptH.ExportToDisk(ExportFormatType.PortableDocFormat, outputFile);
        //    if (!String.IsNullOrEmpty(vm.Comments)) vm.Comments = vm.Comments.Replace(Environment.NewLine, "<br />");

        //    string body = String.Format("Attached File: Load List {0}<br /><br />{1}", vm.LoadList.Name, vm.Comments);

        //    EmailNotifier.FromAddress = ConfigurationManager.AppSettings["LoadListFromAddress"];
        //    EmailNotifier.FromName = ConfigurationManager.AppSettings["LoadListFromName"];
        //    EmailNotifier.SendMail("Load List Attached", body, vm.selectedLines, true, true, outputFile);

        //    //Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
        //    //return File(stream, "application/pdf", fileName);

        //    return RedirectToAction("Details", new { id = vm.LoadListId });
        //}

        //private void CleanOutOldAttachments(string emailPath)
        //{
        //    DirectoryInfo di = new DirectoryInfo(emailPath);
        //    foreach (FileInfo fi in di.GetFiles())
        //    {
        //        if (fi.CreationTime <= DateTime.Now.AddDays(-1)) fi.Delete();
        //    }
        //}

        //[HttpGet]
        //public ActionResult EmailLoadLists()
        //{
        //    var loadlists = _db.LoadLists.Where(x => x.Complete != 1).OrderBy(x => x.Name).ThenBy(x => x.MakeReady);

        //    ViewBag.View = "Open";
        //    return View(loadlists);
        //}

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

            var loadlists = _db.LoadLists.Where(x => x.Complete != true).OrderBy(x => x.DateSchedShip);

            ViewBag.View = "Open";
            return View(loadlists);
        }

        private ActionResult PrintedLoadLists(List<int> selectedLines)
        {
            // The Crystal Reports version
            string fileName = String.Format("LoadLists_{0}.pdf", DateTime.Now.ToShortDateString().Replace(@"/", "").Replace(@"\", ""));
            fileName.Replace(@"/", "").Replace(@"\", "");
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Modules/Time.Epicor/Content/Reports/LoadList.rpt");
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
            rptH.FileName = Server.MapPath("~/Modules/Time.Epicor/Content/Reports/LoadListStatus.rpt");
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

            var jobOpers = production.JobOpers.Where(x => x.JobNum == newLift.JobNumber && x.AssemblySeq == 1 && x.OpCode != "593100" &&
                !(x.OpCode.ToUpper() == "LSHIP" && x.AssemblySeq == 0));

            foreach (var oper in jobOpers.OrderBy(x => x.AssemblySeq).ThenBy(x => x.OprSeq))
            {
                var existing = _db.LoadListJobStatus.SingleOrDefault(x => x.JobNumber == oper.JobNum && x.OpCode == oper.OpCode);
                if (existing == null)
                {
                    var lls = new LoadListJobStatu
                    {
                        AssemblySeq = oper.AssemblySeq,
                        JobNumber = newLift.JobNumber,
                        LoadListJobId = newLift.Id,
                        OpCode = oper.OpCode,
                        OpComplete = oper.OpComplete,
                        OprSeq = oper.OprSeq
                    };
                    _db.LoadListJobStatus.Add(lls);
                    _db.SaveChanges();
                }
                else
                {
                    if (existing.AssemblySeq == 0 && oper.AssemblySeq == 1)
                    {
                        existing.AssemblySeq = 1;
                        existing.OprSeq = oper.OprSeq;
                        if (oper.OpComplete == true) existing.OpComplete = true;
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
                return View(_db.LoadListJobs.Where(x => x.JobNumber.Contains(Search) || x.SerialNo.Contains(Search)).OrderBy(x => x.LoadList.Name));

            return View();
        }
    }
}