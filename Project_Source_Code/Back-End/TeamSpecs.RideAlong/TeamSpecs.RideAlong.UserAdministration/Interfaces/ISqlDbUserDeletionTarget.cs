using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Interfaces
{
    public interface ISqlDbUserDeletionTarget
    {
        IResponse DeleteUserAccountSql(string userName);
    }
}