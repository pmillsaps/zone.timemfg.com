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
    
    public partial class Ref_VideoCard
    {
        public Ref_VideoCard()
        {
            this.Computers = new HashSet<Computer>();
        }
    
        public int Id { get; set; }
        public string VideoCard { get; set; }
        public Nullable<int> ManufacturerId { get; set; }
        public string VideoMemory { get; set; }
    
        public virtual ICollection<Computer> Computers { get; set; }
    }
}
