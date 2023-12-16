using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration;

public class AccountCreationService : IAccountCreationService
{
    public IResponse CreateValidUserAccount(string userName, string dateOfBirth)
    {
        IResponse response = new Response();
        var userAccount = new AccountUserModel(userName, DateTime.Parse(dateOfBirth));

        return response;
    }
}
