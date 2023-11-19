namespace TeamSpecs.RideAlong.LoggingLibrary
{
    public class Log : ILog
    {
        public Log(int? logID, DateTime logTime, string logLevel, string logCategory, string logContext, string? createdBy = null)
        {
            this.logID = logID;
            this.logTime = logTime;
            this.logLevel = logLevel;
            this.logCategory = logCategory;
            this.logContext = logContext;
            this.createdBy = createdBy;
        }
        public int? logID { get; set; }
        public DateTime logTime { get; set; }
        public string logLevel { get; set; }
        public string logCategory { get; set; }
        public string logContext { get; set; }
        public string? createdBy { get; set; }
    }
}