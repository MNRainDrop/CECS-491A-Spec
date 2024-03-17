using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Managers.Interfaces
{
    public interface IUserAdministrationManager
    {
        IResponse RegisterUser(string username, DateTime dateOfBirth, string accountType);
    }
}
