using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration
{
    public interface IAccountModificationService
    {
        IResponse ModifyUserProfie(string userName, DateTime dateOfBirth);
    }
}
