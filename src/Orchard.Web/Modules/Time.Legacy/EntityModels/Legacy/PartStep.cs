//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Legacy.EntityModels.Legacy
{
    using System;
    using System.Collections.Generic;
    
    public partial class PartStep
    {
        public PartStep()
        {
            this.PartStepDetails = new HashSet<PartStepDetail>();
        }
    
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public string Cost { get; set; }
        public string Lab { get; set; }
    
        public virtual ICollection<PartStepDetail> PartStepDetails { get; set; }
    }
}
