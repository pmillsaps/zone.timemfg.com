using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using Time.Data.EntityModels.Production;
using Time.Epicor.Models;

namespace Time.Epicor.ViewModels
{
    public class PartSearchVM
    {
        private ProductionEntities db;

        public enum PartStatus
        {
            All,
            Active,
            Inactive
        }

        public enum SearchType
        {
            PartNumber,
            Description,
            VendorNumber,
            VendorPartNumber
        }

        public int RowCount { get; set; }

        public string OrderBy { get; set; }

        public int PageNum { get; set; }

        public int PageSize { get; set; }

        public string Query { get; set; }

        public List<PartInfo> PartData { get; set; }

        private int _TotalPages;

        public SearchType Type { get; set; }

        public PartStatus Status { get; set; }

        [DisplayName("Service?")]
        public bool ShowService { get; set; }

        [DisplayName("Reedrill Parts?")]
        public bool ShowReedrill { get; set; }

        [DisplayName("Export Parts?")]
        public bool ShowExportVT { get; set; }

        [DisplayName("Prototypes?")]
        public bool ShowPrototypes { get; set; }

        [DisplayName("MRO?")]
        public bool ShowMRO { get; set; }

        [DisplayName("Special?")]
        public bool ShowSpecial { get; set; }

        public bool RestrictData { get; set; }

        [DisplayName("Export To Excel")]
        public bool ExportToExcel { get; set; }

        public int TotalPages
        {
            get { return _TotalPages; }
        }

        //public PartSearchVM()
        //{
        //    PartSearchVM("");
        //}
        public PartSearchVM(string p_search, int p_pageSize)
        {
            db = new ProductionEntities();
            PageNum = 0;
            PageSize = p_pageSize;
            Query = p_search;
            Type = SearchType.PartNumber;
            Status = PartStatus.All;
            ShowExportVT = false;
            ShowReedrill = false;
            ShowService = false;
            ShowPrototypes = false;
            ShowMRO = false;
            ShowSpecial = false;
        }

        public PartSearchVM(string p_search = "")
        {
            db = new ProductionEntities();

            PageNum = 0;
            PageSize = 0;
            _TotalPages = 1;
            Query = p_search;
            Type = SearchType.PartNumber;
            Status = PartStatus.All;
            ShowExportVT = false;
            ShowReedrill = false;
            ShowService = false;
            ShowPrototypes = false;
            ShowMRO = false;
            ShowSpecial = false;
        }

        public PartSearchVM()
        {
            db = new ProductionEntities();

            PageNum = 0;
            PageSize = 0;
            _TotalPages = 1;
            Query = "";
            Type = SearchType.PartNumber;
            Status = PartStatus.All;
            ShowExportVT = false;
            ShowReedrill = false;
            ShowService = false;
            ShowPrototypes = false;
            ShowMRO = false;
            ShowSpecial = false;
        }

        public void refreshData()
        {
            using (var db = new ProductionEntities())
            {
                db.Database.CommandTimeout = 1200;   // Added to get rid of timeOut errors PEM 2014-06-16

                //var repo = new SqlPartRepository(db);
                var model = GetParts();

                switch (Status)
                {
                    case PartStatus.Active:
                        model = model.Where(c => !c.InActive);
                        break;

                    case PartStatus.Inactive:
                        model = model.Where(c => c.InActive);
                        break;

                    default:
                        break;
                }

                switch (Type)
                {
                    case SearchType.Description:
                        model = model.Where(c => c.Description.Contains(Query));
                        break;

                    case SearchType.PartNumber:
                        model = model.Where(c => c.PartNumber.Contains(Query));
                        break;

                    case SearchType.VendorNumber:
                        //var vn = int.Parse(Query);
                        model = model.Where(c => c.VendorID == Query);
                        break;

                    case SearchType.VendorPartNumber:
                        //var vn = int.Parse(Query);
                        model = model.Where(c => c.VendorPartNumber.Contains(Query));
                        break;

                    default:
                        break;
                }

                if (!ShowExportVT) model = model.Where(c => !c.PartNumber.StartsWith("00-"));
                if (!ShowService) model = model.Where(c => !c.PartNumber.StartsWith("X")).Where(c => !c.PartNumber.StartsWith("Y"));
                if (!ShowReedrill) model = model.Where(c => !c.PartNumber.StartsWith("00"));
                if (!ShowPrototypes) model = model.Where(c => !c.ClassID.ToUpper().Equals("PROT"));
                if (!ShowMRO) model = model.Where(c => !c.ClassID.ToUpper().Equals("MRO"));
                if (!ShowSpecial) model = model.Where(c => !c.Eco.ToUpper().Equals("SPECIAL"));

                model = (string.IsNullOrEmpty(OrderBy)) ? model.OrderBy(o => o.PartNumber) : model.OrderBy(OrderBy);

                RowCount = model.Count();
                if (this.PageSize != 0)
                {
                    model = model.Skip(PageSize * (PageNum)).Take(PageSize);
                    _TotalPages = RowCount / PageSize;
                }

                PartData = model.ToList();
                if (RestrictData)
                {
                    foreach (var item in PartData)
                    {
                        item.Cost = null;
                        item.OnHand = null;
                        item.Price = null;
                    }
                }
            }
        }

        public IQueryable<PartInfo> GetParts()
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
    }
}