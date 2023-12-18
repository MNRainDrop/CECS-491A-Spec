using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration
{
    public class AccountRecoveryService
    {
        private IUserTarget _userTarget;

        public AccountRecoveryService(IUserTarget userTarget)
        {
            _userTarget = userTarget;
        }

        public IResponse EnableUserAccount(string userName)
        {
            IResponse response = new Response();

            #region Validiating Arguements
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException($"{nameof(userName)} must be valid");
            }
            #endregion

            response = _userTarget.EnableUserAccountSql(userName);

            if(response.HasError) 
            {
                response.ErrorMessage = "User Account was unable to be enabled";
            }

            return response;
        }

        public IResponse DisableUserAccount(string userName)
        {
            IResponse response = new Response();

            #region Validiating Arguements
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException($"{nameof(userName)} must be valid");
            }
            #endregion

            response = _userTarget.DisableUserAccountSql(userName);

            if (response.HasError)
            {
                response.ErrorMessage = "User Account was unable to be disabled";
            }

            return response;
        }

        public IResponse RecoverUserAccount(string userName)
        {
            IResponse response = new Response();

            #region Validiating Arguements
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException($"{nameof(userName)} must be valid");
            }
            #endregion

            response = _userTarget.RecoverUserAccountSql(userName);

            if (response.HasError)
            {
                response.ErrorMessage = "User Account was unable to be enabled";
            }

            // Response will hold secondaryEmail string and be sent to Manager Layer
            return response;
        }

    }
}
