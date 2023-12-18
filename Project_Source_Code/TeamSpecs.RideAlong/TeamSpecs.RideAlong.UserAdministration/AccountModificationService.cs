using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.UserAdministration;

namespace TeamSpecs.RideAlong.UserAdministration
{
    public class AccountModificationService
    {
        private IUserTarget _userTarget;

        public AccountModificationService(IUserTarget userTarget)
        {
            _userTarget = userTarget;
        }
        public IResponse ModifyUserProfile(string userName,DateTime dateOfBirth) 
        {
            IResponse response = new Response();

            #region Validiating Arguements
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException($"{nameof(userName)} must be valid");
            }
            #endregion

            var profile = new ProfileUserModel(dateOfBirth);
            var UserName = userName;
            response = _userTarget.ModifyUserProfileSql(userName, profile);

            if(response.HasError)
            {
                response.ErrorMessage = "Could not modify the user profile";
            }
        
            return response;
        }
    }
}
