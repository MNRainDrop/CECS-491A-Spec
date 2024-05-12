using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Interfaces
{
    public interface IAccountRetrievalManager
    {
        IResponse RetrieveAccount(long uid);
    }
}