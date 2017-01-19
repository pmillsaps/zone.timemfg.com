using MoreLinq;
using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Time.Data.EntityModels.Production;
using Time.Epicor.Helpers;
using Time.Epicor.Models;
using Time.Epicor.ViewModels;

namespace Time.Epicor.Controllers
{
    [Authorize]
    [Themed]
    public class BOMCostingController : Controller
    {
        public IOrchardServices Services { get; set; }
        private ProductionEntities db;

        public BOMCostingController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new ProductionEntities();
        }

        public Localizer T { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(BOMSearchVM searchVM)
        {
            List<string> partList = new List<string>();
            var timer = new Stopwatch();
            timer.Start();

            // For Debugging only
            if (searchVM.SearchText == null) searchVM.SearchText = "OR-1400-2";
            searchVM.SearchText = searchVM.SearchText.Replace("\n", ",");
            searchVM.SearchText = searchVM.SearchText.Replace("\r", "");
            searchVM.SearchText = searchVM.SearchText.Replace(" ", "");

            if (searchVM.SearchText.Contains(","))
                partList.AddRange(searchVM.SearchText.Split(','));
            else
                partList.Add(searchVM.SearchText);

            if (searchVM.ExportToExcel)
            {
                searchVM.DrillIntoPurchaseItems = false;
                searchVM.FilterRawMaterial = false;
            }
            //IPartRepository sr = new SqlPartRepository();
            //if (searchVM.UseTestData == true) sr = new SqlTestPartRepository();
            List<BOMInfo> tmpBOM = new List<BOMInfo>();
            foreach (string option in partList)
            {
                var part = db.V_PartDetails.FirstOrDefault(x => x.PartNum == option);
                if (part != null)
                {
                    var bom = new List<BOMInfo>();
                    if (part.TypeCode != "P" || (part.TypeCode == "P" && searchVM.DrillIntoPurchaseItems))
                    {
                        bom = BuildBomInMemory(option, searchVM, factorQty: 1, level: 1);
                    }
                    var partCost = GetPartCost(part.PartNum).ToList();
                    BOMInfo first = new BOMInfo
                    {
                        Level = 0,
                        MtlPartNum = part.PartNum,
                        Description = part.PartDescription,
                        QtyPer = 1,
                        bomInfo = bom,
                        PartType = part.TypeCode,
                        LaborTime = partCost.Sum(x => x.EstProdHrs),
                        SetupTime = partCost.Sum(x => x.EstSetHrs),
                        AvgLaborCost = Math.Round(partCost.Sum(x => x.LaborCost), 5),
                        AvgBurdenCost = Math.Round(partCost.Sum(x => x.BurdenCost), 5),
                        AvgMaterialCost = 0,
                        AvgMtlBurCost = 0,
                        AvgSubCost = 0,
                    };

                    if (part.TypeCode == "P" && !searchVM.DrillIntoPurchaseItems)
                    {
                        first.AvgBurdenCost = part.AvgBurdenCost ?? 0;
                        first.AvgLaborCost = part.AvgLaborCost ?? 0;
                        first.AvgMaterialCost = part.AvgMaterialCost ?? 0;
                        first.AvgMtlBurCost = part.AvgMtlBurCost ?? 0;
                        first.AvgSubCost = part.AvgSubContCost ?? 0;

                        first.ExtBurCost = first.AvgBurdenCost;
                        first.ExtLaborCost = first.AvgLaborCost;
                        first.ExtMaterialCost = first.AvgMaterialCost;
                        first.ExtMtlBurCost = first.AvgMtlBurCost;
                        first.ExtSubCost = first.AvgSubCost;
                    }

                    tmpBOM.Add(first);
                }
            }
            searchVM.bomInfo = tmpBOM;

            // Super variable to hold all the results
            List<BOMInfo> allOptions = new List<BOMInfo>{new BOMInfo{
                    MtlPartNum = searchVM.SearchText,
                    Part_Indented = searchVM.SearchText,
                    ExtBurCost = 0,
                    ExtLaborCost = 0,
                    ExtMaterialCost = 0,
                    ExtMtlBurCost = 0,
                    ExtSubCost = 0,
                }};

            if (searchVM.bomInfo != null)
            {
                // First clear all material costs except P items
                searchVM.bomInfo = ClearCosts(searchVM.bomInfo);

                // Next, calculate all non-P items Material costs from the bottom up
                searchVM.bomInfo = CalculateCosts(searchVM.bomInfo, searchVM);

                // look at results to see if this even worked
                foreach (var item in searchVM.bomInfo)
                {
                    var flattenedBOM = flattenBOM(new List<BOMInfo> { item });
                    flattenedBOM.Select(c => { c.ULPart = item.MtlPartNum; return c; }).ToList(); // This wil put the part number within the flatened BOM under ULPart

                    // Total the Extended MaterialCost, as well as the lastLaborCost and LastBurdenCost into the first item
                    flattenedBOM[0].ExtBurCost = flattenedBOM.Sum(x => x.ExtBurCost) + flattenedBOM[0].AvgBurdenCost;
                    flattenedBOM[0].ExtLaborCost = flattenedBOM.Sum(x => x.ExtLaborCost) + flattenedBOM[0].AvgLaborCost;
                    flattenedBOM[0].ExtMaterialCost = flattenedBOM.Sum(x => x.ExtMaterialCost) + flattenedBOM[0].AvgMaterialCost;
                    flattenedBOM[0].ExtMtlBurCost = flattenedBOM.Sum(x => x.ExtMtlBurCost) + flattenedBOM[0].AvgMtlBurCost;
                    flattenedBOM[0].ExtSubCost = flattenedBOM.Sum(x => x.ExtSubCost) + flattenedBOM[0].AvgSubCost;
                    flattenedBOM[0].TotalLaborTime = flattenedBOM.Sum(x => x.LaborTime) + flattenedBOM[0].LaborTime;
                    flattenedBOM[0].TotalSetupTime = flattenedBOM.Sum(x => x.SetupTime) + flattenedBOM[0].SetupTime;

                    flattenedBOM[0].TotalExtendedCost = flattenedBOM[0].ExtBurCost +
                                                        flattenedBOM[0].ExtLaborCost +
                                                        flattenedBOM[0].ExtMaterialCost +
                                                        flattenedBOM[0].ExtMtlBurCost +
                                                        flattenedBOM[0].ExtSubCost;

                    allOptions[0].ExtBurCost = allOptions[0].ExtBurCost + flattenedBOM[0].ExtBurCost;
                    allOptions[0].ExtLaborCost = allOptions[0].ExtLaborCost + flattenedBOM[0].ExtLaborCost;
                    allOptions[0].ExtMaterialCost = allOptions[0].ExtMaterialCost + flattenedBOM[0].ExtMaterialCost;
                    allOptions[0].ExtMtlBurCost = allOptions[0].ExtMtlBurCost + flattenedBOM[0].ExtMtlBurCost;
                    allOptions[0].ExtSubCost = allOptions[0].ExtSubCost + flattenedBOM[0].ExtSubCost;
                    allOptions[0].TotalLaborTime = allOptions[0].TotalLaborTime + flattenedBOM[0].TotalLaborTime;
                    allOptions[0].TotalSetupTime = allOptions[0].TotalSetupTime + flattenedBOM[0].TotalSetupTime;

                    allOptions.AddRange(flattenedBOM);
                }
                allOptions[0].TotalExtendedCost = allOptions[0].ExtBurCost +
                                                    allOptions[0].ExtLaborCost +
                                                    allOptions[0].ExtMaterialCost +
                                                    allOptions[0].ExtMtlBurCost +
                                                    allOptions[0].ExtSubCost;
            }

            if (searchVM.PurchaseItemsOnly)
            {
                List<BOMInfo> PurchaseOptionParts = new List<BOMInfo>();

                foreach (var allOption in allOptions)
                {
                    if (allOption.PartType != "M")
                    {
                        PurchaseOptionParts.Add(allOption);
                    }
                }

                allOptions = PurchaseOptionParts;
            }

            searchVM.bomInfo = allOptions;

            timer.Stop();
            ViewBag._ElapsedTime = timer.ElapsedMilliseconds;

            if (searchVM.ExportToExcel)
            {
                string fileName = searchVM.SearchText;
                if (fileName.Length > 20) fileName = fileName.Substring(0, 16) + ",";
                return new ExporttoExcelResult("BOMCost_" + fileName, allOptions.Cast<object>().ToList());
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("BOMPartial", searchVM.bomInfo);
            }
            else
            {
                return View(searchVM);
            }
        }

        private IQueryable<PartInfo> GetParts()
        {
            return db.V_PartDetails.Select(x => new PartInfo
            {
                PartNumber = x.PartNum,
                Description = x.PartDescription,
                binqty = x.binqty,
                PartLocation = x.BinNum,
                Draw = x.DrawNum,
                Price = x.BasePrice,
                Cost = x.AvgMaterialCost,
                VendorName = x.Name,
                VendorID = x.VendorID,
                VendorPartNumber = x.VenPartNum,
                Type = x.TypeCode,
                InActive = x.InActive,
                isPhantom = x.PhantomBOM,
                Revision = x.RevisionNum,
                ClassID = x.ClassID,
                BuyerID = x.BuyerID,
                BuyerName = x.buyername,
                SubPart = x.SubPart,
                Eco = x.ECO
            });
        }

        private List<BOMInfo> flattenBOM(List<BOMInfo> list)
        {
            List<BOMInfo> bom = new List<BOMInfo>();
            foreach (var item in list)
            {
                List<BOMInfo> childBom = new List<BOMInfo>();
                if (item.bomInfo.Count() > 0)
                    childBom = flattenBOM(item.bomInfo);
                item.bomInfo.Clear();
                item.Part_Indented = "";
                for (int i = 0; i < item.Level; i++)
                {
                    item.Part_Indented += ". ";
                }
                item.Part_Indented += item.MtlPartNum;
                bom.Add(item);
                if (childBom.Count() > 0)
                    bom = bom.Concat(childBom).ToList();
            }

            return bom;
        }

        private List<BOMInfo> CalculateCosts(List<BOMInfo> list, BOMSearchVM searchVM)
        {
            //IPartRepository sr = new SqlPartRepository();
            //if (searchVM.UseTestData == true) sr = new SqlTestPartRepository();
            foreach (BOMInfo item in list)
            {
                if (item.bomInfo.Count() > 0)
                {
                    item.bomInfo = CalculateCosts(item.bomInfo, searchVM);
                    //item.LLMaterialCost = item.bomInfo.Sum(x => x.ExtMaterialCost) * item.Factor;
                    item.LLMaterialCost = item.bomInfo.Sum(x => x.ExtMaterialCost);
                }
                else
                    item.ExtMaterialCost = Math.Round(item.AvgMaterialCost * item.Factor, 5);
                var partCost = GetPartCost(item.MtlPartNum).ToList();
                if (item.PartType != "P")
                {
                    item.LaborTime = partCost.Sum(x => x.EstProdHrs) * item.Factor;
                    item.SetupTime = partCost.Sum(x => x.EstSetHrs) * item.Factor;
                    //item.AvgLaborCost = Math.Round(partCost.Sum(x => x.LaborCost) * item.Factor, 5);
                    //item.AvgBurdenCost = Math.Round(partCost.Sum(x => x.BurdenCost) * item.Factor, 5);
                    item.AvgLaborCost = Math.Round(partCost.Sum(x => x.LaborCost), 5);
                    item.AvgBurdenCost = Math.Round(partCost.Sum(x => x.BurdenCost), 5);

                    // Calculate Extended Costs
                    item.ExtBurCost = Math.Round(item.AvgBurdenCost * item.Factor, 5);
                    item.ExtLaborCost = Math.Round(item.AvgLaborCost * item.Factor, 5);
                }
                //item.AvgSubCost = Math.Round((partCost.Where(x => x.SubContract == true).Sum(x => x.EstUnitCost)) * item.Factor, 5);
                item.AvgSubCost = Math.Round((partCost.Where(x => x.SubContract == true).Sum(x => x.EstUnitCost)), 5);
                item.ExtSubCost = Math.Round(item.AvgSubCost * item.Factor, 5);

                item.TotalExtendedCost = item.ExtBurCost + item.ExtLaborCost + item.ExtMaterialCost + item.ExtMtlBurCost + item.ExtSubCost;
            }

            return list.ToList();
        }

        private List<BOMInfo> ClearCosts(List<BOMInfo> bomInfo)
        {
            foreach (BOMInfo item in bomInfo)
            {
                if (item.PartType != "P")
                {
                    if (item.bomInfo.Count() > 0)
                        item.bomInfo = ClearCosts(item.bomInfo);
                    item.LLMaterialCost = 0;
                    item.AvgMaterialCost = 0;
                    item.AvgLaborCost = 0;
                    item.AvgBurdenCost = 0;
                    item.AvgSubCost = 0;
                    item.ExtMtlBurCost = 0;
                    item.ExtSubCost = 0;
                }
            }

            return bomInfo.ToList();
        }

        private List<BOMInfo> BuildBomInMemory(string option, BOMSearchVM searchVM, decimal factorQty, int level)
        {
            //IPartRepository sr = new SqlPartRepository();
            //if (searchVM.UseTestData == true) sr = new SqlTestPartRepository();
            List<BOMInfo> source = new List<BOMInfo>();

            var qry = GetBom(option);
            if (searchVM.FilterRawMaterial)
                qry = qry.Where(x => x.ClassID != "STL");

            //if (!searchVM.ViewAltMethods)
            //    qry = qry.Where(x => x.AltMethod == "");
            var qry2 = qry.OrderBy(x => x.MtlSeq).ToList();

            foreach (var item in qry2)
            {
                if (item.PartType != "P" || (item.PartType == "P" && searchVM.DrillIntoPurchaseItems))
                    item.bomInfo = BuildBomInMemory(item.MtlPartNum, searchVM, factorQty: item.QtyPer * factorQty, level: level + 1);
                else
                    item.bomInfo = new List<BOMInfo>();
                item.Level = level;
                item.Factor = item.QtyPer * factorQty;
                source.Add(item);
            }

            return source;
        }

        private IQueryable<BOMInfo> GetBom(string option)
        {
            return GetBom().Where(x => x.Part == option);
        }

        public IQueryable<BOMInfo> GetBom()
        {
            return db.V_BillOfMaterials.Where(b => b.RevisionNum == b.Current_Rev).Select(x => new BOMInfo
            {
                Part = x.PartNum,
                MtlSeq = x.MtlSeq,
                MtlPartNum = x.MtlPartNum,
                RelatedOperation = x.RelatedOperation,
                OpCode = x.OpCode,
                QtyPer = x.QtyPer,
                Description = x.PartDescription,
                PartType = x.TypeCode,
                SupplierID = x.VendorID,
                VendorNum = x.VendorNum,
                VendorName = x.Name,
                BuyerId = x.BuyerID,
                ClassID = x.ClassID,
                AltMethod = x.AltMethod,
                LeadTime = x.LeadTime ?? 0,
                OnHandQty = x.onhandqty ?? 0,
                AllocQty = x.AllocQty ?? 0,
                UnfirmAllocQty = x.UnfirmAllocQty ?? 0,
                MfgLotSize = x.MfgLotSize ?? 0,
                AvgBurdenCost = x.AvgBurdenCost ?? 0,
                AvgLaborCost = x.AvgLaborCost ?? 0,
                AvgMaterialCost = x.AvgMaterialCost ?? 0,
                AvgMtlBurCost = x.AvgMtlBurCost ?? 0,
                AvgSubCost = x.AvgSubContCost ?? 0,
            });
        }

        public IQueryable<PartCost> GetPartCost()
        {
            return db.V_PartCostStd.Select(x => new PartCost
            {
                PartNum = x.PartNum,
                CostinglotSize = x.CostingLotSize,
                EstSetHrs = x.EstSetHours,
                ProdStandard = x.ProdStandard,
                EstProdHrs = x.EstProdHours,
                ProdBurRate = x.ProdBurRate,
                ProdLbrRate = x.ProdLabRate,
                SetupBurRate = x.SetupBurRate,
                SetupLbrRate = x.SetupLabRate,
                LaborCost = x.LaborCost ?? 0,
                BurdenCost = x.BurdenCost ?? 0,
                SubContract = x.SubContract,
                EstUnitCost = x.EstUnitCost,
            });
        }

        public IQueryable<PartCost> GetPartCost(string part)
        {
            return GetPartCost().Where(x => x.PartNum == part);
        }
    }
}