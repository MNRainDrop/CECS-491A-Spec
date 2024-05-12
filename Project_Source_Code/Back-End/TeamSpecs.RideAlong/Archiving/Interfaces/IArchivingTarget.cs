using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Archiving
{
    public interface IArchivingTarget
    {
        public IResponse GetLogs(DateTime beforeDate, DateTime? butNotBefore);
        public IResponse SetLogs(IList<ILog> logs);
    }
}
