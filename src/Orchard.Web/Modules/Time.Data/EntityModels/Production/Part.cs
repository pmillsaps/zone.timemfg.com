//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.Production
{
    using System;
    using System.Collections.Generic;
    
    public partial class Part
    {
        public string Company { get; set; }
        public string PartNum { get; set; }
        public string SearchWord { get; set; }
        public string PartDescription { get; set; }
        public string ClassID { get; set; }
        public string IUM { get; set; }
        public string PUM { get; set; }
        public string TypeCode { get; set; }
        public bool NonStock { get; set; }
        public decimal PurchasingFactor { get; set; }
        public decimal UnitPrice { get; set; }
        public string PricePerCode { get; set; }
        public decimal InternalUnitPrice { get; set; }
        public string InternalPricePerCode { get; set; }
        public string ProdCode { get; set; }
        public string MfgComment { get; set; }
        public string PurComment { get; set; }
        public string CostMethod { get; set; }
        public string UserChar1 { get; set; }
        public string UserChar2 { get; set; }
        public string UserChar3 { get; set; }
        public string UserChar4 { get; set; }
        public Nullable<System.DateTime> UserDate1 { get; set; }
        public Nullable<System.DateTime> UserDate2 { get; set; }
        public Nullable<System.DateTime> UserDate3 { get; set; }
        public Nullable<System.DateTime> UserDate4 { get; set; }
        public decimal UserDecimal1 { get; set; }
        public decimal UserDecimal2 { get; set; }
        public decimal UserDecimal3 { get; set; }
        public decimal UserDecimal4 { get; set; }
        public int UserInteger1 { get; set; }
        public int UserInteger2 { get; set; }
        public string TaxCatID { get; set; }
        public bool InActive { get; set; }
        public int LowLevelCode { get; set; }
        public bool Method { get; set; }
        public bool TrackLots { get; set; }
        public bool TrackDimension { get; set; }
        public string DefaultDim { get; set; }
        public bool TrackSerialNum { get; set; }
        public string CommodityCode { get; set; }
        public string WarrantyCode { get; set; }
        public bool PhantomBOM { get; set; }
        public string SalesUM { get; set; }
        public decimal SellingFactor { get; set; }
        public decimal MtlBurRate { get; set; }
        public decimal NetWeight { get; set; }
        public bool UsePartRev { get; set; }
        public int PartsPerContainer { get; set; }
        public decimal PartLength { get; set; }
        public decimal PartWidth { get; set; }
        public decimal PartHeight { get; set; }
        public int LotShelfLife { get; set; }
        public bool WebPart { get; set; }
        public bool RunOut { get; set; }
        public string SubPart { get; set; }
        public decimal Diameter { get; set; }
        public decimal Gravity { get; set; }
        public bool OnHold { get; set; }
        public Nullable<System.DateTime> OnHoldDate { get; set; }
        public string OnHoldReasonCode { get; set; }
        public string AnalysisCode { get; set; }
        public bool GlobalPart { get; set; }
        public string MtlAnalysisCode { get; set; }
        public bool GlobalLock { get; set; }
        public decimal ISSuppUnitsFactor { get; set; }
        public string PDMObjID { get; set; }
        public string ImageFileName { get; set; }
        public string ISOrigCountry { get; set; }
        public string SNPrefix { get; set; }
        public string SNFormat { get; set; }
        public string SNBaseDataType { get; set; }
        public bool Constrained { get; set; }
        public string UPCCode1 { get; set; }
        public string UPCCode2 { get; set; }
        public string UPCCode3 { get; set; }
        public string EDICode { get; set; }
        public bool WebInStock { get; set; }
        public bool ConsolidatedPurchasing { get; set; }
        public string PurchasingFactorDirection { get; set; }
        public string SellingFactorDirection { get; set; }
        public bool RecDocReq { get; set; }
        public int MDPV { get; set; }
        public bool ShipDocReq { get; set; }
        public string ReturnableContainer { get; set; }
        public decimal NetVolume { get; set; }
        public bool QtyBearing { get; set; }
        public string NAFTAOrigCountry { get; set; }
        public string NAFTAProd { get; set; }
        public string NAFTAPref { get; set; }
        public string ExpLicType { get; set; }
        public string ExpLicNumber { get; set; }
        public string ECCNNumber { get; set; }
        public string AESExp { get; set; }
        public string HTS { get; set; }
        public bool UseHTSDesc { get; set; }
        public string SchedBcode { get; set; }
        public bool HazItem { get; set; }
        public string HazTechName { get; set; }
        public string HazClass { get; set; }
        public string HazSub { get; set; }
        public string HazGvrnmtID { get; set; }
        public string HazPackInstr { get; set; }
        public string RevChargeMethod { get; set; }
        public decimal RCUnderThreshold { get; set; }
        public decimal RCOverThreshold { get; set; }
        public string OwnershipStatus { get; set; }
        public string UOMClassID { get; set; }
        public string SNMask { get; set; }
        public string SNMaskExample { get; set; }
        public string SNMaskSuffix { get; set; }
        public string SNMaskPrefix { get; set; }
        public string SNLastUsedSeq { get; set; }
        public bool UseMaskSeq { get; set; }
        public string NetWeightUOM { get; set; }
        public string NetVolumeUOM { get; set; }
        public bool LotBatch { get; set; }
        public bool LotMfgBatch { get; set; }
        public bool LotMfgLot { get; set; }
        public bool LotHeat { get; set; }
        public bool LotFirmware { get; set; }
        public bool LotBeforeDt { get; set; }
        public bool LotMfgDt { get; set; }
        public bool LotCureDt { get; set; }
        public bool LotExpDt { get; set; }
        public string LotPrefix { get; set; }
        public bool LotUseGlobalSeq { get; set; }
        public string LotSeqID { get; set; }
        public int LotNxtNum { get; set; }
        public int LotDigits { get; set; }
        public bool LotLeadingZeros { get; set; }
        public string LotAppendDate { get; set; }
        public bool BuyToOrder { get; set; }
        public bool DropShip { get; set; }
        public bool IsConfigured { get; set; }
        public bool ExtConfig { get; set; }
        public string RefCategory { get; set; }
        public bool CSFCJ5 { get; set; }
        public bool CSFLMW { get; set; }
        public decimal GrossWeight { get; set; }
        public string GrossWeightUOM { get; set; }
        public string BasePartNum { get; set; }
        public string FSAssetClassCode { get; set; }
        public decimal FSSalesUnitPrice { get; set; }
        public string FSPricePerCode { get; set; }
        public bool RcvInspectionReq { get; set; }
        public string EstimateID { get; set; }
        public string EstimateOrPlan { get; set; }
        public bool DiffPrc2PrchUOM { get; set; }
        public bool DupOnJobCrt { get; set; }
        public decimal PricingFactor { get; set; }
        public string PricingUOM { get; set; }
        public bool MobilePart { get; set; }
        public byte[] SysRevID { get; set; }
        public System.Guid SysRowID { get; set; }
        public bool AGUseGoodMark { get; set; }
        public bool AGProductMark { get; set; }
        public Nullable<System.Guid> ForeignSysRowID { get; set; }
        public byte[] UD_SysRevID { get; set; }
        public string HoseBuild_c { get; set; }
        public string PartsPDF_c { get; set; }
        public string ISRegion { get; set; }
        public string INChapterID { get; set; }
        public string PESUNATType { get; set; }
        public string PESUNATUOM { get; set; }
        public bool DEIsServices { get; set; }
        public bool DEIsSecurityFinancialDerivative { get; set; }
        public string DEInternationalSecuritiesID { get; set; }
        public bool LinkToContract { get; set; }
        public bool DEIsInvestment { get; set; }
        public string DEPayStatCode { get; set; }
        public string DEDenomination { get; set; }
        public string PartLengthWidthHeightUM { get; set; }
        public string DiameterUM { get; set; }
        public decimal DiameterInside { get; set; }
        public decimal DiameterOutside { get; set; }
        public string ThicknessUM { get; set; }
        public decimal Thickness { get; set; }
        public decimal ThicknessMax { get; set; }
        public string Durometer { get; set; }
        public string Specification { get; set; }
        public string EngineeringAlert { get; set; }
        public string Condition { get; set; }
        public bool IsCompliant { get; set; }
        public bool IsRestricted { get; set; }
        public bool IsSafetyItem { get; set; }
        public string CommercialBrand { get; set; }
        public string CommercialSubBrand { get; set; }
        public string CommercialCategory { get; set; }
        public string CommercialSubCategory { get; set; }
        public string CommercialStyle { get; set; }
        public string CommercialSize1 { get; set; }
        public string CommercialSize2 { get; set; }
        public string CommercialColor { get; set; }
        public bool IsGiftCard { get; set; }
        public string PhotoFile { get; set; }
        public bool PartPhotoExists { get; set; }
        public string CommentText { get; set; }
        public bool PartSpecificPackingUOM { get; set; }
        public string ImageID { get; set; }
        public string CNSpecification { get; set; }
        public bool SyncToExternalCRM { get; set; }
        public string ExternalCRMPartID { get; set; }
        public Nullable<System.DateTime> ExternalCRMLastSync { get; set; }
        public bool ExternalCRMSyncRequired { get; set; }
    }
}
