using TeamSpecs.RideAlong.Model;
namespace TeamSpecs.RideAlong.UserAdministration;

public interface IAccountCreationService
{
    IResponse verifyUser(string email);

    IResponse verifyAltUser(string email);
}
