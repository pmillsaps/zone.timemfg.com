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
    
    public partial class Ref_Model
    {
        public Ref_Model()
        {
            this.Computers = new HashSet<Computer>();
            this.AttachmentForModels = new HashSet<AttachmentForModel>();
        }
    
        public int ID { get; set; }
        public string Model { get; set; }
        public Nullable<int> ManufacturerId { get; set; }
    
        public virtual ICollection<Computer> Computers { get; set; }
        public virtual Ref_Manufacturer Ref_Manufacturer { get; set; }
        public virtual ICollection<AttachmentForModel> AttachmentForModels { get; set; }
    }
}
