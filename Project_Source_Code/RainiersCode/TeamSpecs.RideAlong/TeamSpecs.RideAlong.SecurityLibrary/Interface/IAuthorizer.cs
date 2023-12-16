namespace TeamSpecs.RideAlong.SecurityLibrary;

public class IAuthorizer
{
    bool IsAuthorize(AppPrincipal currentPrincipal, IDictionary<string, string> claims);
}
