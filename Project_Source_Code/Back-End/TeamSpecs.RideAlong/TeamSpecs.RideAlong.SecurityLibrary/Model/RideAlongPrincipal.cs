using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

namespace TeamSpecs.RideAlong.SecurityLibrary.Model
{
    public class RideAlongPrincipal : IAppPrincipal
    {
        public IAuthUserModel userIdentity { get; set; }
        public IDictionary<string, string> claims { get; set; }
        public RideAlongPrincipal(IAuthUserModel pUserIdentity, IDictionary<string, string> pUserClaims)
        {
            userIdentity = pUserIdentity;
            claims = pUserClaims;
        }
    }
}
