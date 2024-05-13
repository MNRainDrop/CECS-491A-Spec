using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Interfaces
{
    public interface IGetAccountUpdateManager
    {
        IResponse GetAccountUpdate(IAccountUserModel useraccount);
    }
}
