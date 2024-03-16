using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

namespace TeamSpecs.RideAlong.SecurityLibrary;

public class AppPrincipal : IAppPrincipal
{

    /*public AppPrincipal()
    {
        //expirationDate = getdatetimenow + expiration time
    }*/

    public AppPrincipal(IAccountUserModel UserI, IDictionary<string, string> claims)

    {
        UserIdentity = UserI;
        Claims = claims;
    }


    public IAccountUserModel UserIdentity { get; set; }
    // claims based add claims to the collection
    // roles based add the one role to the collection

    //public ICollection<(string ClaimName, string ClaimValue)> Claims { get; set; }
    // becomes a dictionary
    //DateTimeOffset? expirationdate { get; }
    public IDictionary<string, string> Claims { get; set; }
}
