using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

namespace TeamSpecs.RideAlong.SecurityLibrary
{
    public interface IAuthorizer
    {
        bool Authorize(IAppPrincipal principal, Dictionary<string, string> requiredClaims);
    }
}
