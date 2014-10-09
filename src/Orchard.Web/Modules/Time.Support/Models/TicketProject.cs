using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Time.Support.EntityModels.TimeMfg
{
    [MetadataType(typeof(ProjectMetadata))]
    public partial class TicketProject
    {
    }

    public class ProjectMetadata
    {
        [DisplayName("Subject")]
        [Required]
        public string Title { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [DisplayName("Category")]
        public int CategoryID { get; set; }
        [DisplayName("Department")]
        public int? DepartmentID { get; set; }
        [DisplayName("Priority")]
        public int PriorityID { get; set; }
    }
}