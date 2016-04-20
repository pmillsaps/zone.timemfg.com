//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class LoadList
    {
        public LoadList()
        {
            this.LoadListComments = new HashSet<LoadListComment>();
            this.LoadListImages = new HashSet<LoadListImage>();
            this.LoadListJobs = new HashSet<LoadListJob>();
            this.LoadListDistributors = new HashSet<LoadListDistributor>();
        }
    
        public int Id { get; set; }
        public System.DateTime DateIssued { get; set; }
        public Nullable<System.DateTime> DateRevised { get; set; }
        public Nullable<System.DateTime> DateSchedShip { get; set; }
        public string Name { get; set; }
        public string TruckingCompany { get; set; }
        public string Comments { get; set; }
        public byte Complete { get; set; }
        public string Distributors { get; set; }
        public byte MakeReady { get; set; }
    
        public virtual ICollection<LoadListComment> LoadListComments { get; set; }
        public virtual ICollection<LoadListImage> LoadListImages { get; set; }
        public virtual ICollection<LoadListJob> LoadListJobs { get; set; }
        public virtual ICollection<LoadListDistributor> LoadListDistributors { get; set; }
    }
}
