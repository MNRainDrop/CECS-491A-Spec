using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SecurityLibrary.Targets
{
    public interface IAuthTarget
    {
        /// <summary>
        /// Looks up the specified target and retrieves the 
        /// user's information in the form of an authUserModel in a response
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Response with AuthUserModel or success outcome</returns>
        IResponse fetchUserModel(string username);
        /// <summary>
        /// stores a otp or password to a datastore
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="passHash"></param>
        /// <returns>Response object with success or failure outcome</returns>
        IResponse savePass(long UID, string pass);
        /// <summary>
        /// gets the list of user claims in the form of a Dictionary<string, string>
        /// </summary>
        /// <param name="UID"></param>
        /// <returns>Response Object with claims or success outcome</returns>
        IResponse getClaims(long UID);
        /// <summary>
        /// fetches the users pass from the datastore
        /// </summary>
        /// <param name="UID"></param>
        /// <returns>Response Object with user pass or Success outcome</returns>
        IResponse fetchPass(long UID);
        /// <summary>
        /// fetches the user's current number of login attempts, as recorded by the datestore
        /// </summary>
        /// <param name="UID"></param>
        /// <returns>Response with int attempts or Success outcome</returns>
        IResponse fetchAttempts(long UID);
        /// <summary>
        /// Sets the number of login attempts to the value provided in parameter
        /// This value is not defined here for flexibility of use
        /// The value may change for testing, security, or business requriements, so it is not hardcoded here
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="attempts"></param>
        /// <returns>Response Object with success outcome</returns>
        IResponse updateAttempts(long UID, int attempts);
        /// <summary>
        /// Sets the FirstFailedLoginAttempt to the dateTime passed in as a parameter
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="datetime"></param>
        /// <returns>Response Object with success outcome</returns>
        IResponse setFirstFailedLogin(long uid, DateTime datetime);
        /// <summary>
        /// Gets the time of the first failed login<br>
        /// This function should be called alongside a check for how many login attempts have been done<br>
        /// If login attempts are higher than 3, and less than 24 hours have passed since this time, we fail authN request
        /// </summary>
        /// <returns>Response object with the datetime of firstFailedLogin</returns>
        IResponse fetchFirstFailedLogin(long uid);
    }
}
