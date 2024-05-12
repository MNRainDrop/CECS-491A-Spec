using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Interfaces
{
    public interface ISqlDbUserRecoveryTarget
    {
        IResponse retrieveAltEmail(string email);
    }
}