using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.DataAccess;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Data.SqlClient;

namespace TeamSpecs.RideAlong.CarNewsCenter
{
    public class SqlCarNewsCenterTarget : ICarNewsCenterTarget
    {
        private readonly IGenericDAO _dao;
        
        public SqlCarNewsCenterTarget(IGenericDAO dao)
        {
            _dao = dao; 
        }
        public IResponse GetsAllVehicles(ICollection<object> searchParameters)
        {
            #region Default sql setup
            var commandSql = "SELECT * ";
            var fromSql = "FROM VehicleProfile ";
            var defaultWhereSql = "WHERE ";
            var whereSql = "";
            var orderBySql = "ORDER BY DateCreated ";
            //var offsetSql = $"OFFSET {(page - 1) * numOfResults} ROWS ";
            //var fetchSql = $"FETCH NEXT {numOfResults} ROWS ONLY;";

            #endregion

            var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
            var response = new Response();

            try
            {
                // create new hash set of SqlParameters
                var parameters = new HashSet<SqlParameter>();


                if (searchParameters is not null)
                {
                    foreach (var item in searchParameters)
                    {
                        KeyValuePair<string, long> searchItem;
                        if (item is KeyValuePair<string, long>)
                        {
                            searchItem = (KeyValuePair<string, long>)item;

                            whereSql += defaultWhereSql + searchItem.Key + " = @" + searchItem.Key + " ";
                            parameters.Add(new SqlParameter("@" + searchItem.Key, searchItem.Value));
                        }
                    }
                }
                var sqlString = commandSql + fromSql + whereSql + orderBySql;

                sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Could not generate Vehicle Profile Retrieval Sql.";
                return response;
            }

            // DAO Executes the command
            try
            {
                var daoValue = _dao.ExecuteReadOnly(sqlCommands);
                response.ReturnValue = new List<object>();

                foreach (var item in daoValue)
                {
                    response.ReturnValue.Add(new VehicleProfileModel((string)item[0], (long)item[1], (string)item[2], (string)item[3], (string)item[4], (int)item[5]));
                }
                response.HasError = false;
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Vehicle Profile Retrieval execution failed.";
            }
            return response;
        }

        public IResponse UpdateNotification(INotification notification)
        {
            var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
            var response = new Response();

            #region Setting SQL Command
            var declareCommand = "DECLARE @UpdatedIDs TABLE (ID INT)";
            var commandSql = "UPDATE ";
            var tableSql = "VehicleProfile ";
            var setSql = "SET ";
            var whereSql = "WHERE ";

            //These 3 to insert into NotificationObject and Output the NotificationID 
            var insertSQL_notifObject = "INSERT INTO NotficationObject (Description,Type)";//MACTH DB
            var outputSQL = "OUTPUT INSERTED.NotificationID INTO @UpdatedIDs(ID)";
            var insertSQL_notifObject2 = "VALUES (@Description,@Type)"; 

            //These to use that NotificationID to update the NotificationCenter table 
            var insertSQL_notifCenter = "INSERT INTO NotificationCenter(UID, VIN, NotificationID)";
            var selectSQL = "SELECT @UID,@VIN,U.ID";
            var fromSQL = "FROM @UpdatedIDs U ";
            #endregion

            #region Extracting value 
            try
            {
                var parameters = new HashSet<SqlParameter>
                {
                    new SqlParameter("@VIN", notification.VIN),
                    new SqlParameter("@UID", notification.UID),
                    new SqlParameter("@Description", notification.description),
                    new SqlParameter("@Type", notification.type)
                };

                //Combine into SQL command 
                var sqlString = insertSQL_notifObject + outputSQL + insertSQL_notifObject2 + insertSQL_notifCenter + selectSQL + fromSQL;

                // Add string and hash set to list that the dao will execute
                sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));

            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Could not generate Vehicle Profile Update Sql";
                return response;
            }
            #endregion

            response.HasError = false;
            return response;
        }
    }
}
