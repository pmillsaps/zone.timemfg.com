﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.DataPlates
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DataPlateEntities : DbContext
    {
        public DataPlateEntities()
            : base("name=DataPlateEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CA_Options> CA_Options { get; set; }
        public virtual DbSet<EP_SS_Options> EP_SS_Options { get; set; }
        public virtual DbSet<HR_Options> HR_Options { get; set; }
        public virtual DbSet<LB_Options> LB_Options { get; set; }
        public virtual DbSet<LiftData> LiftDatas { get; set; }
        public virtual DbSet<PS_Option> PS_Option { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<TemplateData> TemplateDatas { get; set; }
        public virtual DbSet<DielectricRating> DielectricRatings { get; set; }
    }
}
