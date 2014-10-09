using System.ComponentModel.DataAnnotations;

namespace Time.Support.EntityModels.TimeMfg
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