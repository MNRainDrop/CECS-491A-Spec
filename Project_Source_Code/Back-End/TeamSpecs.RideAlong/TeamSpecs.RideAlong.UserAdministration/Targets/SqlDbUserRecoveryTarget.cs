using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.UserAdministration.Targets;

namespace TeamSpecs.RideAlong.UserAdministration.Interfaces
{
    public class SqlDbUserRecoveryTarget : ISqlDbUserRecoveryTarget
    {
        private readonly ISqlServerDAO _dao;


        public SqlDbUserRecoveryTarget(ISqlServerDAO dao)
        {
            _dao = dao;
        }

        public IResponse retrieveAltEmail(string email)
        {
            IResponse response = new Response();



            return response;
        }


    }
}
