namespace TeamSpecs.RideAlong.LoggingLibrary
{
    public interface ILog
    {
        public int? logID { get; set; }
        public DateTime logTime { get; set; }
        public string logLevel { get; set; }
        public string logCategory { get; set; }
        public string logContext { get; set; }
        public string? createdBy { get; set; }
    }
}
