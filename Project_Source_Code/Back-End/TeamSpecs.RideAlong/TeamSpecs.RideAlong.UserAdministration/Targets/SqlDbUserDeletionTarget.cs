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


/*
 * -- Delete records from BuyRequest where buyerUID matches the provided UID
DELETE FROM BuyRequest WHERE buyerUID = @UID;

-- Delete records from Parts where ownerUID matches the provided UID
DELETE FROM Parts WHERE ownerUID = @UID;

-- Delete records from Listings where partUID matches the provided UID
DELETE FROM Listings WHERE partUID = @UID;

-- Delete records from NotificationCenter where UID matches the provided UID
DELETE FROM NotificationCenter WHERE UID = @UID;

-- Delete records from UserDetails where UID matches the provided UID
DELETE FROM UserDetails WHERE UID = @UID;

-- Delete records from UserProfile where UID matches the provided UID
DELETE FROM UserProfile WHERE UID = @UID;

-- Delete records from OTP where UID matches the provided UID
DELETE FROM OTP WHERE UID = @UID;
*/