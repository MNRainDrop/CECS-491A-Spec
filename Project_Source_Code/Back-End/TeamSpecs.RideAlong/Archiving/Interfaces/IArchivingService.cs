using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Archiving
{
    public interface IArchivingService
    {
        public IResponse GetArchive(DateTime before, DateTime? butNotBefore);
        public IResponse Archive(byte[] zipFileBytes);
    }
}
