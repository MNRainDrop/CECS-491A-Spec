using TeamSpecs.RideAlong.Model;


namespace TeamSpecs.RideAlong.UserAdministration.Interfaces;


public interface IPostAccountUpdateService
{
    IResponse UpdateUserAccount(IAccountUserModel userAccount, string address, string name, string phone, string accountType);
}
