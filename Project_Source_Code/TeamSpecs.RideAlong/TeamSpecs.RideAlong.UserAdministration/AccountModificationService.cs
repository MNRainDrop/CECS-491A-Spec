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
        public IResponse ModifyUser(string userName,DateTime dateOfBirth) 
        {
            IResponse response = new Response();

            #region Validiating Input
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException($"{nameof(userName)} must be valid");
            }
            if (dateOfBirth.GetType() != typeof(DateTime))
            {
                throw new ArgumentException($"{nameof(dateOfBirth)} is not of type {nameof(DateTime)}");
            }
            #endregion

            var profile = new ProfileUserModel(dateOfBirth);
            var UserName = userName;
            // Create UserProfileSQL (profile: IUserProfileModel): IResponse
            response = _userTarget.ModifyUserProfileSql(userName, profile);

            // If error is found 

            return response;
        }
    }
}
