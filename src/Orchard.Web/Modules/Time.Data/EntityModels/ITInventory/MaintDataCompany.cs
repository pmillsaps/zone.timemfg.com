//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.ITInventory
{
    using System;
    using System.Collections.Generic;
    
    public partial class MaintDataCompany
    {
        public MaintDataCompany()
        {
            this.MaintenanceDatas = new HashSet<MaintenanceData>();
        }
    
        public int Id { get; set; }
        public string CompanyName { get; set; }
    
        public virtual ICollection<MaintenanceData> MaintenanceDatas { get; set; }
    }
}
