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
    
    public partial class OptionTitlesForWordDoc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OptionTitlesForWordDoc()
        {
            this.OptionTitleDescpForWordDocs = new HashSet<OptionTitleDescpForWordDoc>();
        }
    
        public int Id { get; set; }
        public int LiftFamilyId { get; set; }
        public string OptionTitle { get; set; }
    
        public virtual LiftFamily LiftFamily { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OptionTitleDescpForWordDoc> OptionTitleDescpForWordDocs { get; set; }
    }
}