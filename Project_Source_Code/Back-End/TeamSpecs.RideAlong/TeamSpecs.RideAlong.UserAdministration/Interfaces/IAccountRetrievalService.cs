using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration
{
    public interface IAccountRetrievalService
    {
        IResponse RetrieveAccount(long uid);

        IResponse RetrieveAllAccount();

    }
}