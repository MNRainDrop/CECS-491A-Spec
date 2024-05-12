using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Targets
{
    public interface ISqlDbUserRecoveryTarget
    {
        IResponse retrieveAltEmail(string email);
    }
}