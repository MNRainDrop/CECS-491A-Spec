

using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CoEsLibrary.Interfaces
{
    public interface ISendEmailService
    {
        public IResponse SendEmail(IAccountUserModel accountUser, string vin);
    }
}
