using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.UserAdministration.ProfileUserModel;

namespace TeamSpecs.RideAlong.UserAdministration
{
    public class AccountModificationService
    {
        public IResponse ModifyUser(DateTime dateOfBirth) 
        {
            var profile = new UserProfileModel(dateOfBirth);
            IResponse response = new Response();

            // Check correct data type

            // Create UserProfileSQL (profile: IUserProfileModel): IResponse
            // Assuming response = to this statement

            // If error is found 


            return response;
        }
    }
}
