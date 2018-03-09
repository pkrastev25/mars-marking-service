namespace mars_marking_svc.Models
{
    public class MetadataModel
    {
        public const string FinishedState = "FINISHED";

        public const string ToBeDeletedState = "TO_BE_DELETED";

        public string dataId { get; set; }

        public string state { get; set; }
    }
}