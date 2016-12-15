﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.Configurator
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ConfiguratorEntities : DbContext
    {
        public ConfiguratorEntities()
            : base("name=ConfiguratorEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ConfigPricing> ConfigPricings { get; set; }
        public virtual DbSet<ConfiguratorName> ConfiguratorNames { get; set; }
        public virtual DbSet<Structure> Structures { get; set; }
        public virtual DbSet<StructureSeq> StructureSeqs { get; set; }
        public virtual DbSet<Lookup> Lookups { get; set; }
        public virtual DbSet<ComplexStructure> ComplexStructures { get; set; }
        public virtual DbSet<ComplexLink> ComplexLinks { get; set; }
        public virtual DbSet<ComplexLookup> ComplexLookups { get; set; }
        public virtual DbSet<PartOverride> PartOverrides { get; set; }
        public virtual DbSet<ConfigOption> ConfigOptions { get; set; }
        public virtual DbSet<SpecialConfig> SpecialConfigs { get; set; }
        public virtual DbSet<SpecialCustomer> SpecialCustomers { get; set; }
        public virtual DbSet<SpecialData> SpecialDatas { get; set; }
        public virtual DbSet<SpecialDataType> SpecialDataTypes { get; set; }
        public virtual DbSet<SpecialRelatedOp> SpecialRelatedOps { get; set; }
    }
}
