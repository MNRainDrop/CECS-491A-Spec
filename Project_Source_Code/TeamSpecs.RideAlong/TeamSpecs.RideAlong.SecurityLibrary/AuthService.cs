namespace TeamSpecs.RideAlong.SecurityLibrary;

// Library built in
using System.Security.Principal;

public class AuthService : IAuthenticator, IAuthorizer
{

    /// <summary>
    /// For Authentication
    /// </summary>
    /// <param name="authRequest"></param>
    /// <returns>This will return an AppPrincipal </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public AppPrincipal? Authenticate(AuthenticationRequest authRequest)
    {
        // parameter is the name in the signature
        // argument is the actual value

        // Early Exit - uses less resource
        #region Validate Arguments
        if (authRequest is null)
        {
            // user nameof instead of just quotes because it will flag it when there's a rename
            throw new ArgumentNullException(nameof(authRequest));
        }

        if (String.IsNullOrWhiteSpace(authRequest.UserIdentity))
        {
            throw new ArgumentException($"{nameof(authRequest.UserIdentity)} must be valid");
        }

        if (String.IsNullOrWhiteSpace(authRequest.Proof))
        {
            throw new ArgumentException($"{nameof(authRequest.Proof)} must be valid");
        }
        #endregion

        AppPrincipal appPrincipal = null;

        try
        {

            // Step 1: Validate auth request 
            // go to db
            // check if it matches
            // return the roles/claims if it matches

            // Step 2: Populate AppPrincipal
            var claims = new Dictionary<string, string>();

            appPrincipal = new AppPrincipal()
            {
                UserIdentity = authRequest.UserIdentity,
                Claims = claims
            };

        }
        catch (Exception ex)
        {
            var errorMessage = ex.GetBaseException().Message;
            // log error message
            // Should be Asynchronous
        }

        return appPrincipal;
    }

    // depending on design goals, implementation drastically changes
    // why do we throw only in the beginning, but not at the end
    // if user error, they have to correct it. we just check if it is a possible valid one
    // it is user/caller's responsiblity


    // use bool because all you need to know is true and false
    // creating a resposne object would use more memory
    public bool IsAuthorize(AppPrincipal currentPrincipal, IDictionary<string, string> requiredClaims)
    {

        // Dictionary<string, string>() {new ("RoleName", "Admin")}
        // key - RoleName, value - Admin

        foreach (var claim in requiredClaims)
        {
            // all of the rquired claims must be within the claims of the current user
            if (!currentPrincipal.Claims.Contains(claim))
            {
                return false;
            }
        }

        return true;
    }
}
