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
    
    public partial class Term_Property
    {
        public int Id { get; set; }
        public int EmpID { get; set; }
        public bool CellPhone { get; set; }
        public bool FMPOff { get; set; }
        public bool CellReceived { get; set; }
        public bool Cables { get; set; }
        public bool CablesReceived { get; set; }
        public bool OfficeKey { get; set; }
        public bool OKeyReceived { get; set; }
        public bool BuildingKey { get; set; }
        public bool BKeyReceived { get; set; }
    
        public virtual Term_Employees Term_Employees { get; set; }
    }
}