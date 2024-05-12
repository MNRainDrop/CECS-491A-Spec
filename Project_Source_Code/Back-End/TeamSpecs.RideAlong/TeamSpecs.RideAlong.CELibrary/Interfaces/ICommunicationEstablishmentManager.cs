using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CELibrary.Interfaces
{
    public interface ICommunicationEstablishmentManager
    {
        IResponse SendRequest(IAccountUserModel userAccount, bool pending_status, string vin);

        IResponse
    }
}
