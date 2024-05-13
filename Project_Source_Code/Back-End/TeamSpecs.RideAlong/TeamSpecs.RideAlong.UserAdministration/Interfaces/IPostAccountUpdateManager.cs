using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Interfaces
{
    public interface IPostAccountUpdateManager
    {
        IResponse PostAccountUpdate(IAccountUserModel useraccount, string address, string name, string phone, string accountType);
    }
}
