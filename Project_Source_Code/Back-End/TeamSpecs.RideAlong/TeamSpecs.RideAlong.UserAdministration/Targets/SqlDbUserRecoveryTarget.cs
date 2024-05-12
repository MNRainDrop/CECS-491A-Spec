using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

namespace TeamSpecs.RideAlong.UserAdministration.Targets
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
            #region Varaibles 
            IResponse response = new Response();
            SqlCommand cmd = new SqlCommand();
            string query = @"SELECT UP.altUserName
                FROM UserAccount UA
                JOIN UserProfile UP ON UA.UID = UP.UID
                WHERE UA.UserName = @UserName;";
            #endregion

            #region Generate retrieveAltEmail sql 
            try
            {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@UserName", email);
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "could not generate retrievingAltEmail Sql";
                return response;
            }
            #endregion

            #region Executing Readonly
            try
            {
                response = _dao.ExecuteReadOnly(cmd);
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Checking for altEmail in SQL DB execution failed";
                return response;
            }
            #endregion

            if(response.ReturnValue is not null)
            {
                // No userAccount found
                if (response.ReturnValue.Count() == 0)
                {
                    response.HasError = true;
                    response.ErrorMessage = "User does not exist";
                    return response;
                }
            }

            response.HasError = false;
            return response;
        }


    }
}
