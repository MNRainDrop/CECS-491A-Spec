namespace TeamSpecs.RideAlong.SecurityLibrary;

public class AppPrincipal
{
    public AppPrincipal()
    {
        //expirationDate = getdatetimenow + expiration time
    }

    public string UserIdentity { get; set; }
    // claims based add claims to the collection
    // roles based add the one role to the collection

    //public ICollection<(string ClaimName, string ClaimValue)> Claims { get; set; }
    // becomes a dictionary
    DateTime? expirationdate {get;}
    public IDictionary<string, string> Claims { get; set; }

}
