﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.TimeMFG
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TimeMFGEntities : DbContext
    {
        public TimeMFGEntities()
            : base("name=TimeMFGEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<LoadListComment> LoadListComments { get; set; }
        public virtual DbSet<LoadListDistributor> LoadListDistributors { get; set; }
        public virtual DbSet<LoadListEmail> LoadListEmails { get; set; }
        public virtual DbSet<LoadListImage> LoadListImages { get; set; }
        public virtual DbSet<LoadListJobComment> LoadListJobComments { get; set; }
        public virtual DbSet<LoadListJob> LoadListJobs { get; set; }
        public virtual DbSet<TicketAttachment> TicketAttachments { get; set; }
        public virtual DbSet<TicketCategory> TicketCategories { get; set; }
        public virtual DbSet<TicketDepartment> TicketDepartments { get; set; }
        public virtual DbSet<TicketEmployee> TicketEmployees { get; set; }
        public virtual DbSet<TicketLink> TicketLinks { get; set; }
        public virtual DbSet<TicketNote> TicketNotes { get; set; }
        public virtual DbSet<TicketPriority> TicketPriorities { get; set; }
        public virtual DbSet<TicketProject> TicketProjects { get; set; }
        public virtual DbSet<TicketStatusHistory> TicketStatusHistories { get; set; }
        public virtual DbSet<TicketTask> TicketTasks { get; set; }
        public virtual DbSet<TicketRequestor> TicketRequestors { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<TicketVisibility> TicketVisibilities { get; set; }
        public virtual DbSet<WaterTest> WaterTests { get; set; }
        public virtual DbSet<SysTask> SysTasks { get; set; }
        public virtual DbSet<SysTaskSchedule> SysTaskSchedules { get; set; }
        public virtual DbSet<TicketStatus> TicketStatuses { get; set; }
        public virtual DbSet<Message_Type> Message_Type { get; set; }
        public virtual DbSet<MSMQ_Status> MSMQ_Status { get; set; }
        public virtual DbSet<NLog_Entries> NLog_Entries { get; set; }
        public virtual DbSet<ValuedInventory> ValuedInventories { get; set; }
        public virtual DbSet<V_ValuedInventoryByPeriod> V_ValuedInventoryByPeriod { get; set; }
        public virtual DbSet<V_ClassIdSummary> V_ClassIdSummary { get; set; }
        public virtual DbSet<LoadListJobStatu> LoadListJobStatus { get; set; }
        public virtual DbSet<LoadList> LoadLists { get; set; }
    }
}
