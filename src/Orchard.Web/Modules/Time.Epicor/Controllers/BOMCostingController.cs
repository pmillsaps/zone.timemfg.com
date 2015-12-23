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
    [Themed]
    public class BOMCostingController : Controller
    {
        public IOrchardServices Services { get; set; }
        private ProductionEntities db;

        public BOMCostingController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
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
                    var bom = BuildBomInMemory(option, searchVM, factorQty: 1, level: 1);
                    var partCost = GetPartCost(part.PartNum).ToList();
                    BOMInfo first = new BOMInfo
                    {
                        Level = 0,
                        MtlPartNum = part.PartNum,
                        Description = part.PartDescription,
                        QtyPer = 1,
                        bomInfo = bom,
                        LaborTime = partCost.Sum(x => x.EstProdHrs),
                        SetupTime = partCost.Sum(x => x.EstSetHrs),
                        LastLaborCost = Math.Round(partCost.Sum(x => x.LaborCost), 5),
                        LastBurdenCost = Math.Round(partCost.Sum(x => x.BurdenCost), 5),
                        LastMaterialCost = 0,
                        LastMtlBurCost = 0,
                        LastSubCost = 0,
                    };
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
                    flattenedBOM[0].ExtBurCost = flattenedBOM.Sum(x => x.ExtBurCost) + flattenedBOM[0].LastBurdenCost;
                    flattenedBOM[0].ExtLaborCost = flattenedBOM.Sum(x => x.ExtLaborCost) + flattenedBOM[0].LastLaborCost;
                    flattenedBOM[0].ExtMaterialCost = flattenedBOM.Sum(x => x.ExtMaterialCost) + flattenedBOM[0].LastMaterialCost;
                    flattenedBOM[0].ExtMtlBurCost = flattenedBOM.Sum(x => x.ExtMtlBurCost) + flattenedBOM[0].LastMtlBurCost;
                    flattenedBOM[0].ExtSubCost = flattenedBOM.Sum(x => x.ExtSubCost) + flattenedBOM[0].LastSubCost;
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
                return new ExporttoExcelResult("BOMCost_" + fileName, allOptions);
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

        private IQueryable<PartCost> GetPartCost(string partNumber)
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

        private IQueryable<PartInfo> GetParts()
        {
            return db.V_PartDetails.Select(x => new PartInfo
            {
                PartNumber = x.PartNum,
                Description = x.PartDescription,
                OnHand = x.binqtyonhand,
                PartLocation = x.BinNum,
                Draw = x.DrawNum,
                Price = x.BasePrice,
                Cost = x.LastMaterialCost,
                VendorName = x.Name,
                VendorID = x.VendorID,
                VendorPartNumber = x.VenPartNum,
                Type = x.TypeCode,
                Active = x.InActive,
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
            throw new NotImplementedException();
        }

        private List<BOMInfo> CalculateCosts(List<BOMInfo> bomInfo, BOMSearchVM searchVM)
        {
            throw new NotImplementedException();
        }

        private List<BOMInfo> ClearCosts(List<BOMInfo> bomInfo)
        {
            throw new NotImplementedException();
        }

        private List<BOMInfo> BuildBomInMemory(string option, BOMSearchVM searchVM, int factorQty, int level)
        {
            throw new NotImplementedException();
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
                LastBurdenCost = x.LastBurdenCost ?? 0,
                LastLaborCost = x.LastLaborCost ?? 0,
                LastMaterialCost = x.LastMaterialCost ?? 0,
                LastMtlBurCost = x.LastMtlBurCost ?? 0,
                LastSubCost = x.LastSubContCost ?? 0,
            });
        }
    }
}