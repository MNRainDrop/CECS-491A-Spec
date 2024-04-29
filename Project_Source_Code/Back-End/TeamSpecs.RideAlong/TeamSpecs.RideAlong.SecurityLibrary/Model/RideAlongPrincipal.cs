using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

namespace TeamSpecs.RideAlong.SecurityLibrary.Model
{
    public class RideAlongPrincipal : IAppPrincipal
    {
        public IAuthUserModel userIdentity { get; set; }
        public ICollection<KeyValuePair<string, string>> claims { get; set; }
        public RideAlongPrincipal(IAuthUserModel pUserIdentity, ICollection<KeyValuePair<string, string>> pUserClaims)
        {
            userIdentity = pUserIdentity;
            claims = pUserClaims;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public RideAlongPrincipal() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
