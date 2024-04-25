using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

namespace TeamSpecs.RideAlong.SecurityLibrary;

public class AppPrincipal : IAppPrincipal
{

    /*public AppPrincipal()
    {
        //expirationDate = getdatetimenow + expiration time
    }*/

    public AppPrincipal(IAuthUserModel UserI, IDictionary<string, string> Claims)

    {
        userIdentity = UserI;
        claims = Claims;
    }


    public IAuthUserModel userIdentity { get; set; }
    // claims based add claims to the collection
    // roles based add the one role to the collection

    //public ICollection<(string ClaimName, string ClaimValue)> Claims { get; set; }
    // becomes a dictionary
    //DateTimeOffset? expirationdate { get; }
    public ICollection<KeyValuePair<string, string>> claims { get; set; }
}
