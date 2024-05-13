using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Interfaces
{
    public interface IAccountRetrievalService
    {
        IResponse RetrieveAccount(long uid);
    }
}