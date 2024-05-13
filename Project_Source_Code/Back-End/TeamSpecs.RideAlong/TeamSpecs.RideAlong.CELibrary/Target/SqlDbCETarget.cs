using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.CELibrary.Interfaces;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CELibrary.Target
{
    public class SqlDbCETarget : ISqlDbCETarget
    {
        private readonly SqlServerDAO _dao;

        public SqlDbCETarget(SqlServerDAO dao)
        {
            _dao = dao;
        }

        public IResponse GetSeller(string vin)
        {
            var commandSql = "UA.UserName ";
            var fromSql = "From VendingStatus VS ";
            var joinSql = "JOIN VehicleProfile VP ON VS.VIN = VP.VIN ";
            var join2Sql = "JOIN UserAccount UA ON VP.Owner_UID = UA.UID ";
            var whereSql = "WHERE VS.VIN = ";

            whereSql += "@VIN;";

            var parameters = new HashSet<SqlParameter>
            {
                new SqlParameter("@VIN", vin)
            };

            var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
            var response = new Response();

            try
            {
                // create new hash set of SqlParameters
                var sqlString = commandSql + fromSql + joinSql + join2Sql + whereSql;
                sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Could not generate Get Seller Sql.";
                return response;
            }

            // DAO Executes the command
            try
            {
                var daoValue = _dao.ExecuteReadOnly(sqlCommands);
                response.ReturnValue = new List<object>()
                {
                    daoValue.First<object[]>()
                };
                response.HasError = false;
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Seller username extraction execution failed.";
            }
            return response;
        }

    }
}
