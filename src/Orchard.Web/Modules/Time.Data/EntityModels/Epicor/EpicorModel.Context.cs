﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.Epicor
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EpicorEntities : DbContext
    {
        public EpicorEntities()
            : base("name=EpicorEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<imjoboper> imjobopers { get; set; }
        public virtual DbSet<impartbin> impartbins { get; set; }
        public virtual DbSet<joboper> jobopers { get; set; }
        public virtual DbSet<v_CustomersWithOpenOrderPartLines> v_CustomersWithOpenOrderPartLines { get; set; }
        public virtual DbSet<v_JobInformation> v_JobInformation { get; set; }
        public virtual DbSet<systask> systasks { get; set; }
        public virtual DbSet<systaskparam> systaskparams { get; set; }
        public virtual DbSet<sysagentsched> sysagentscheds { get; set; }
        public virtual DbSet<sysagenttask> sysagenttasks { get; set; }
        public virtual DbSet<C_TMC_Status> C_TMC_Status { get; set; }
        public virtual DbSet<v_PartDetails> v_PartDetails { get; set; }
    }
}
