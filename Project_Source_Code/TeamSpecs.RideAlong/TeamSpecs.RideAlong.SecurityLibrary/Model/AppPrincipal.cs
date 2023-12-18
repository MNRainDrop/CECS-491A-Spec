using TeamSpecs.RideAlong.SecurityLibrary.Model;


namespace TeamSpecs.RideAlong.SecurityLibrary
{
    public class AppPrincipal
    {

        /*public AppPrincipal()
        {
            //expirationDate = getdatetimenow + expiration time
        }*/

        public AppPrincipal(UserAccount UserI, IDictionary<string, string> claims)
        {
            UserIdentity = UserI;
            Claims = claims;
        }


        public UserAccount UserIdentity { get; set; }
        // claims based add claims to the collection
        // roles based add the one role to the collection

        //public ICollection<(string ClaimName, string ClaimValue)> Claims { get; set; }
        // becomes a dictionary
        //DateTimeOffset? expirationdate { get; }
        public IDictionary<string, string> Claims { get; set; }
    }


}
