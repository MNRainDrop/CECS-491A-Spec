using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
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

        public IResponse DeleteVehicleProfiles(long uid)
        {
            IResponse response = new Response();
            var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
            string query = @"
            UPDATE VehicleProfile
            SET Owner_UID = NULL
            WHERE Owner_UID = @UID;";

            #region Sql Generation 
            try
            {
                // Parameters for the query
                var parameters = new HashSet<SqlParameter>
                {
                    new SqlParameter("@UID", uid)
                };

                // Add query and parameters to the list
                sqlCommands.Add(new KeyValuePair<string, HashSet<SqlParameter>?>(query, parameters));
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "could not generate setting VP Uid to null Sql";
                return response;
            }
            #endregion

            #region Execute Write 
            try
            {
                var daoValue = _dao.ExecuteWriteOnly(sqlCommands);

            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Could not execute setting VP uid to null Sql";
                return response;
            }
            #endregion

            response.HasError = false;
            return response;
        }

        public IResponse DeleteUserAccount(long uid)
        {
            IResponse response = new Response();
            var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
            string query = @"
                DELETE FROM UserAccount WHERE UID = @UID;
            ";

            #region Generate Deletion Sql
            try
            {
                var parameters = new HashSet<SqlParameter>
                {
                    new SqlParameter("@UID", uid)
                };

                sqlCommands.Add(new KeyValuePair<string, HashSet<SqlParameter>?>(query, parameters));
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Could not generate deleting UID Sql";
                return response;
            }
            #endregion

            #region Execute Write
            try
            {
                var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
                if (daoValue == 0)
                {
                    response.HasError = true;
                    response.ErrorMessage = "UID was not found";
                    return response;
                }
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "could not execute setting VP uid to null Sql";
                return response;
            }
            #endregion

            response.HasError = false;
            return response;
        }

        public IResponse CreateAccountDeletionRequest(string userHash)
        {
            IResponse response = new Response();
            var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
            string query = @"
                INSERT INTO AccountDeletionRequest (LogID, LogTime, UserHash)
                SELECT TOP 1 LogID, LogTime, UserHash
                FROM Log
                WHERE UserHash = @UserHash
                ORDER BY LogTime DESC;
            ";

            try
            {
                var parameters = new HashSet<SqlParameter>
                {
                    new SqlParameter("@UserHash", userHash)
                };

                sqlCommands.Add(new KeyValuePair<string, HashSet<SqlParameter>?>(query, parameters));
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Could not generate deleting CreateAccountDeletionRequest Sql";
                return response;
            }
            try
            {
                var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
                if (daoValue == 0)
                {
                    response.HasError = true;
                    response.ErrorMessage = "UserHash was not found";
                    return response;
                }
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "could not execute CreateAccountDeletionRequestTable  Sql";
                return response;
            }

            return response;
        }
    }
}