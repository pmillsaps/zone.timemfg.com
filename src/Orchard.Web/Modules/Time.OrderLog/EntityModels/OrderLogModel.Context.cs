﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.OrderLog.EntityModels
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class OrderLogEntities : DbContext
    {
        public OrderLogEntities()
            : base("name=OrderLogEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Dealer> Dealers { get; set; }
        public virtual DbSet<Install> Installs { get; set; }
        public virtual DbSet<LiftModel> LiftModels { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderLine> OrderLines { get; set; }
        public virtual DbSet<OrderLineUnit> OrderLineUnits { get; set; }
        public virtual DbSet<Territory> Territories { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
    }
}
