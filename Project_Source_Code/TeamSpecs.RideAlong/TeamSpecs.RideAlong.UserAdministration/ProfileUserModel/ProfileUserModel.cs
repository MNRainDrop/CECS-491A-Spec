using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

<<<<<<< Updated upstream
namespace TeamSpecs.RideAlong.UserAdministration
{
    public class ProfileUserModel : IProfileUserModel
    {
        public DateTime DateOfBirth { get; set; }
=======
namespace TeamSpecs.RideAlong.UserAdministration.ProfileUserModel;
>>>>>>> Stashed changes

public class UserProfileModel : IUserProfileModel
{
    public DateTime DateOfBirth { get; set; }

    public UserProfileModel(DateTime dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
    }
}
