using AutoMapper;
using MoreLinq;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Time.Data.EntityModels.TimeMFG;
using Time.Data.Models.MessageQueue;
using Time.Epicor.Helpers;
using Time.Epicor.Models;
using Time.Epicor.ViewModels;

namespace Time.Epicor.Controllers
{
    [Authorize]
    [Themed]
    public class ValuedInventoryController : Controller
    {
        private TimeMFGEntities db;
        private IMapper mapper;

        public ValuedInventoryController()
        {
            db = new TimeMFGEntities();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ValuedInventory, ValuedInventoryExt>();
            });

            mapper = config.CreateMapper();
        }

        // GET: ValuedInventory
        public ActionResult Index()
        {
            var dates = new SelectList(db.ValuedInventories.OrderBy(x => x.ComparisonDate).DistinctBy(x => x.ComparisonDate), "ComparisonDate", "ComparisonDate");
            ValuedInventoryVM vm = new ValuedInventoryVM
            {
                ComparisonDates = dates
            };
            if (!String.IsNullOrEmpty((string)TempData["ErrorMessage"])) ViewBag.ErrorMessage = TempData["ErrorMessage"];
            if (!String.IsNullOrEmpty((string)TempData["Notice"])) ViewBag.Notice = TempData["Notice"];

            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(ValuedInventoryVM vm)
        {
            string[] PartClasses = { "BINR", "CPRO", "BODY", "INST", "JIT", "KANB", "LIFT", "PART", "PROT", "STL", "TRKS", "VPBR", "VPRO" };
            var dates = new SelectList(db.ValuedInventories.OrderBy(x => x.ComparisonDate).DistinctBy(x => x.ComparisonDate), "ComparisonDate", "ComparisonDate", vm.ComparisonDate);
            vm.ComparisonDates = dates;

            if (vm.ComparisonDate != null)
            {
                string filename = String.Format("ValuedInventory_{0}", vm.ComparisonDate.ToString("yyyyMMdd"));
                var data = db.ValuedInventories.Where(x => x.ComparisonDate == vm.ComparisonDate).OrderBy(x => x.PartNum).ToArray();
                List<ValuedInventoryExt> _data = mapper.Map<ValuedInventory[], List<ValuedInventoryExt>>(data);
                var data2 = db.V_ValuedInventoryByPeriod.Where(x => x.ComparisonDate == vm.ComparisonDate).OrderBy(x => x.PeriodYear).ThenBy(x => x.ClassId).ToList();
                var data3 = db.V_ClassIdSummary.Where(x => x.ComparisonDate == vm.ComparisonDate).OrderBy(x => x.ClassId).ToList();

                return new ValuedInventoryExcelResult(filename, _data, data2, data3);
            }

            //vm.ValuedInventoryItems = db.ValuedInventories.Where(x => x.ComparisonDate == vm.ComparisonDate).OrderBy(x => x.PartNum).ToList();

            return View(vm);
        }

        [HttpPost]
        public ActionResult GenerateData(string ComparisonDate)
        {
            try
            {
                var date = DateTime.Parse(ComparisonDate);
                var comparisonDate = DateTime.Now;
                if ((comparisonDate - date).TotalDays < 365 && date < comparisonDate)
                {
                    var command = new ValuedInventoryMessage
                    {
                        InventoryDate = date
                    };
                    var success = MSMQ.SendQueueMessage(command, MessageType.BuildValuedInventory.Value);
                    if (success) TempData["Notice"] = String.Format("Please check the Message Queue to see when the data for {0} will be available", date);
                }
                else TempData["ErrorMessage"] = "Date can only be a year back";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                //throw;
            }

            return RedirectToAction("Index");
        }
    }
}