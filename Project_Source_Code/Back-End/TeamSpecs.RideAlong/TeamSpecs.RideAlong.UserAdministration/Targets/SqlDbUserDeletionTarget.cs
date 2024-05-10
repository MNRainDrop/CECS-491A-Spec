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

        public IResponse DeleteUserAccountSql(string userName)
        {
            /*
             * Revise SQL pullled from UserTarget --> needs to be reivsed
             */


            #region Validate arguments
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }
            #endregion

            #region Default sql setup
            var commandSql = "DELETE FROM ";
            var tableSql = "UserAccount ";
            var whereSql = "WHERE UserName = @UserName";
            #endregion


            var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
            var response = new Response();

            // 
            try
            {
                // create new hash set of SqlParameters
                var parameters = new HashSet<SqlParameter>()
            {
                new SqlParameter("@UserName", userName)
            };

                var sqlString = commandSql + tableSql + whereSql;

                sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Could not generate sql to delete user";
                return response;
            }

            // DAO Executes the command
            try
            {
                var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
                response.ReturnValue = new List<object>()
            {
                (object) daoValue
            };
                response.HasError = false;
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Account Deletion execution failed";
                return response;
            }

            return response;
        }
    }
}
