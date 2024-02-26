using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SecurityLibrary.Interfaces
{
    public interface ISessionManager
    {
        /* ----- Fields ----- */
        //User Model
        IAuthUserModel? user { get; set; }
        //User Principal
        IAppPrincipal? appPrincipal { get; set; }
        //Principal Creation Time
        DateTime? creationTime { get; }
        //Principal Expiration Time
        DateTime? expirationTime { get; }

        /* ----- Methods ----- */
        /// <summary>
        /// Log in attempt function on design document
        /// Collects a user name, resets OTP on website, retrieves user account model
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Response object with outcome details(error=true/false)</returns>
        public Response attemptLogin(string username);
        /// <summary>
        /// Authenticate User functionality on Design Document. 
        /// Validate proof of identity against hashedOTP in datastore.
        /// get hashed OTP from database, Hash proof, compare proofHash to otpHash
        /// </summary>
        /// <param name="proof"></param>
        /// <returns>Response Object with bool containing outcome</returns>
        public Response authenticateLogin(string proof);

        /// <summary>
        /// Refresh User Principal fucntionality on Design Document.
        /// Updates the user Principal to its most current state.
        /// </summary>
        /// <returns>Response Object with </returns>
        public Response refreshUserPrincipal();

        /// <summary>
        /// Logout functionality on design document
        /// Deletes the current user pricipal from the 
        /// </summary>
        /// <returns>Boolean value with yes or no success value</returns>
        public bool logOut();

        /// <summary>
        /// Authorize functionality on design document
        /// Validates required claim and scope are present in principal.
        /// Refreshes the principal if it is expired
        /// </summary>
        /// <param name="requiredClaim"></param>
        /// <param name="requiredScope"></param>
        /// <returns></returns>
        public bool Authorize(string requiredClaim, string requiredScope);

    }
}
