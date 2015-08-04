using System.ComponentModel.DataAnnotations;

namespace Time.Data.EntityModels.TimeMFG
{
    [MetadataType(typeof(TaskMetadata))]
    public partial class TicketTask
    {
    }

    public class TaskMetadata
    {
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
    }
}