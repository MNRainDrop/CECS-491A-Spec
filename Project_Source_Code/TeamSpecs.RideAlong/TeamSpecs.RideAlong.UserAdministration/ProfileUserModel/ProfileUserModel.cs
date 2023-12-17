using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSpecs.RideAlong.UserAdministration
{
    public class ProfileUserModel : IProfileUserModel
    {
        public DateTime DateOfBirth { get; set; }

        public ProfileUserModel(DateTime dateOfBirth)
        {
            DateOfBirth = dateOfBirth;
        }
    }
}
