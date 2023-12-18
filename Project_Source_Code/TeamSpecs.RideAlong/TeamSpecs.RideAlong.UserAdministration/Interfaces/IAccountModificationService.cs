using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration
{
    public interface IAccountModificationService
    {
        IResponse ModifyUserProfie(string userName, DateTime dateOfBirth);
    }
}
