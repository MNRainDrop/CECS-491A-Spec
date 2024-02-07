using TeamSpecs.RideAlong.Model

namespace TeamSpecs.RideAlong.SecurityLibrary
{
    public interface IAuthenticator
    {
        // Returns a tuple
        // ValueTuple when specifies identifier
        // (string name, string roles)

        // better to create an object of AuthRequest rather than just putting in values 
        // instead of a tuple, just have a new response object
        AppPrincipal? Authenticate(AuthenticationRequest authRequest);

        public Response getUserAccountModel(string username);
        public Response getPrincipal(long UID);

        public Response getPass(long UID);
    }
}
