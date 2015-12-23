using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Epicor.Models
{
    public class BOMInfo
    {
        public string Part_Indented { get; set; }
        public string ULPart { get; set; }
        public string MtlPartNum { get; set; }
        public string Description { get; set; }
        public int? MtlSeq { get; set; }
        public string Rev { get; set; }
        public string PartType { get; set; }
        public int Level { get; set; }
        public int? Mtl { get; set; }
        public decimal QtyPer { get; set; }
        public decimal Factor { get; set; }
        public byte FixedQuantity { get; set; }
        public int? RelatedOperation { get; set; }
        public string OpCode { get; set; }
        public string SupplierID { get; set; }
        public int? VendorNum { get; set; }
        public string VendorName { get; set; }
        public string BuyerId { get; set; }
        public string Part_Line { get; set; }
        public string Part { get; set; }
        public ICollection<BOMInfo> bomInfo { get; set; }
        public string ClassID { get; set; }
        public string AltMethod { get; set; }
        public int LeadTime { get; set; }
        public decimal OnHandQty { get; set; }
        public decimal AllocQty { get; set; }
        public decimal UnfirmAllocQty { get; set; }
        public decimal MfgLotSize { get; set; }
        public decimal LaborTime { get; set; }
        public decimal SetupTime { get; set; }
        public decimal TotalLaborTime { get; set; }
        public decimal TotalSetupTime { get; set; }
        public decimal LastMaterialCost { get; set; }
        public decimal LastSubCost { get; set; }
        public decimal LastLaborCost { get; set; }
        public decimal LastBurdenCost { get; set; }
        public decimal LastMtlBurCost { get; set; }
        public decimal LLMaterialCost { get; set; }
        public decimal ExtMaterialCost { get; set; }
        public decimal ExtSubCost { get; set; }
        public decimal ExtLaborCost { get; set; }
        public decimal ExtBurCost { get; set; }
        public decimal ExtMtlBurCost { get; set; }
        public decimal TotalExtendedCost { get; set; }
    }
}