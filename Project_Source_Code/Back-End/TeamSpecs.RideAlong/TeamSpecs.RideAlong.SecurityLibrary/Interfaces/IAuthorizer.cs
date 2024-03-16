namespace TeamSpecs.RideAlong.SecurityLibrary
{
    public interface IAuthorizer
    {
        AppPrincipal? Authenticate(AuthenticationRequest authRequest);
        bool IsAuthorize(AppPrincipal currentPrincipal, IDictionary<string, string> claims);
    }
}
