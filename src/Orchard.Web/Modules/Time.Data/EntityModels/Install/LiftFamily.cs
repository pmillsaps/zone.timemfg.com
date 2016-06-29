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
    
    public partial class LiftFamily
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LiftFamily()
        {
            this.InstallQuotes = new HashSet<InstallQuote>();
            this.VSWOptions = new HashSet<VSWOption>();
            this.TimeOptions = new HashSet<TimeOption>();
        }
    
        public int Id { get; set; }
        public string FamilyName { get; set; }
        public decimal LaborHours { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InstallQuote> InstallQuotes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VSWOption> VSWOptions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TimeOption> TimeOptions { get; set; }
    }
}
