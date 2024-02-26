using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

namespace TeamSpecs.RideAlong.SecurityLibrary
{
    public class SessionManager : ISessionManager
    {
        /* ----- Members ----- */
        //User Model
        public IAuthUserModel? user { get; set; }
        //User Principal
        public IAppPrincipal? appPrincipal { get; set; }
        //Principal Creation Time
        public DateTime? creationTime { get; }
        //Principal Expiration Time
        public DateTime? expirationTime { get; }


        /* ----- Methods ----- */
        public SessionManager()
        {
            //Nothing is needed to be set up yet
        }

        public Response attemptLogin(string username)
        {
            throw new NotImplementedException();
        }

        public Response authenticateLogin(string proof)
        {
            throw new NotImplementedException();
        }

        public bool Authorize(string requiredClaim, string requiredScope)
        {
            throw new NotImplementedException();
        }

        public bool logOut()
        {
            throw new NotImplementedException();
        }

        public Response refreshUserPrincipal()
        {
            throw new NotImplementedException();
        }
    }
}
