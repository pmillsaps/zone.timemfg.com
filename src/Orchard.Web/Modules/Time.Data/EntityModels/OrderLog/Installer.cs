//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.OrderLog
{
    using System;
    using System.Collections.Generic;
    
    public partial class Installer
    {
        public Installer()
        {
            this.Orders = new HashSet<Order>();
        }
    
        public int InstallerId { get; set; }
        public string InstallerName { get; set; }
    
        public virtual ICollection<Order> Orders { get; set; }
    }
}
