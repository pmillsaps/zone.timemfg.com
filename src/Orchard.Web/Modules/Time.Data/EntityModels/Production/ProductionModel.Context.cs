﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ProductionEntities : DbContext
    {
        public ProductionEntities()
            : base("name=ProductionEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Part> Parts { get; set; }
        public virtual DbSet<SysTask> SysTasks { get; set; }
        public virtual DbSet<SysAgentTask> SysAgentTasks { get; set; }
        public virtual DbSet<IMJobOper> IMJobOpers { get; set; }
        public virtual DbSet<IMPartBin> IMPartBins { get; set; }
        public virtual DbSet<SysAgentSched> SysAgentScheds { get; set; }
        public virtual DbSet<V_DistributorOrderList> V_DistributorOrderList { get; set; }
        public virtual DbSet<V_NowReportClaim> V_NowReportClaim { get; set; }
        public virtual DbSet<V_QuoteOrderInformation> V_QuoteOrderInformation { get; set; }
        public virtual DbSet<V_WhereUsed> V_WhereUsed { get; set; }
        public virtual DbSet<V_YardReport> V_YardReport { get; set; }
        public virtual DbSet<V_BillOfMaterials> V_BillOfMaterials { get; set; }
        public virtual DbSet<V_PartCostStd> V_PartCostStd { get; set; }
        public virtual DbSet<V_PartDetails> V_PartDetails { get; set; }
        public virtual DbSet<V_NowReport> V_NowReport { get; set; }
    }
}