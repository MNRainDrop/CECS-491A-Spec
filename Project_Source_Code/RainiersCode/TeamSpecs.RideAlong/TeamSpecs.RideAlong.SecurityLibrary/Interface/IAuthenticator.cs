namespace TeamSpecs.RideAlong.SecurityLibrary;

public class IAuthenticator
{
    AppPrincipal Authenticate(AuthenticationRequest authRequest);
}
