using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

namespace TeamSpecs.RideAlong.UserAdministration.Targets
{
    public class SqlDbUserDeletionTarget : ISqlDbUserDeletionTarget
    {
        private readonly ISqlServerDAO _dao;

        public SqlDbUserDeletionTarget(ISqlServerDAO dao)
        {
            _dao = dao;
        }

        public IResponse DeleteVehicleProfiles(string userName)
        {
            IResponse response = new Response();

            //implement logic here

            return response;
        }

        public IResponse DeleteUserAccount(string userName)
        {
            IResponse response = new Response();

            //implement logic here

            return response;
        }
    }
}
