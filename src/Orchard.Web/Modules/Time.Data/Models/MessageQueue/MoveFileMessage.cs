namespace Time.Data.Models.MessageQueue
{
    public class MoveFileMessage
    {
        public string SourceFile { get; set; }

        public string TargetFile { get; set; }

        public string TargetDirectory { get; set; }
    }
}