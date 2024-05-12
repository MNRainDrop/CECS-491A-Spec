using TeamSpecs.RideAlong.Model;


namespace TeamSpecs.RideAlong.UserAdministration.Interfaces;


public interface IGetAccountUpdateService
{
    IResponse GetUpdateUserAccount(IAccountUserModel userAccount);
}
