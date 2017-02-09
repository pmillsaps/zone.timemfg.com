using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
    }

    public class UserMetadata
    {
        [Required]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Notes{ get; set; }
        [DisplayName("Building")]
        public Nullable<int> BuildingId { get; set; }
        [DisplayName("Location")]
        public Nullable<int> LocationId { get; set; }
        [DisplayName("Last Date Edited")]
        public Nullable<System.DateTime> LastDateEdited { get; set; }
        [DisplayName("Last Edited By")]
        public string LastEditedBy { get; set; }
        [DisplayName("Inactive")]
        public Nullable<bool> InActive { get; set; }
    }
}