using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Targets
{
    public class SqlDbUserRecoveryTarget
    {
        private readonly ISqlServerDAO _dao;


        public SqlDbUserRecoveryTarget(ISqlServerDAO dao)
        {
            _dao = dao;
        }

        public IResponse retrieveAltEmail(string email)
        {

        }


    }
}
