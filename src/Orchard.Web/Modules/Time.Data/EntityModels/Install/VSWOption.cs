//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.Install
{
    using System;
    using System.Collections.Generic;
    
    public partial class VSWOption
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VSWOption()
        {
            this.InstallDetails = new HashSet<InstallDetail>();
        }
    
        public int Id { get; set; }
        public int LiftFamilyId { get; set; }
        public int GroupId { get; set; }
        public string OptionName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal LaborHours { get; set; }
        public bool PaintFlag { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InstallDetail> InstallDetails { get; set; }
        public virtual LiftFamily LiftFamily { get; set; }
        public virtual OptionGroup OptionGroup { get; set; }
    }
}
