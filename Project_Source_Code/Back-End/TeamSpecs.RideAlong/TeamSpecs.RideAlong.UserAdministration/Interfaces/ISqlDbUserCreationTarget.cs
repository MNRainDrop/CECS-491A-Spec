using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Interfaces
{
    public interface ISqlDbUserCreationTarget
    {
        public IResponse CheckDbForEmail(string email);

        public IResponse CreateUserConfirmation(IAccountUserModel userAccount, string otp);

        public IResponse UpdateUserConfirmation(IAccountUserModel userAccount, string otp);

        public IResponse CreateDefaultUser();

        public IResponse CreateVendorUser();

        public IResponse CreateFleetUser();

        public IResponse CreateAdminUser();
    }
}
