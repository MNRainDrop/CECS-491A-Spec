using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Archiving
{
    public class SqlServerLogArchivingTarget : IArchivingTarget
    {
        public IResponse GetLogs(DateTime beforeDate, DateTime? butNotBefore)
        {
            throw new NotImplementedException();
        }

        public IResponse SetLogs(IList<ILog> logs)
        {
            throw new NotImplementedException();
        }
    }
}
