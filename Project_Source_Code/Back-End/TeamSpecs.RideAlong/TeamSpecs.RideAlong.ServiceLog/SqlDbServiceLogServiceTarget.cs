using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Model.PaginationModel;
using TeamSpecs.RideAlong.Model.ServiceLogModel;
using TeamSpecs.RideAlong.ServiceLog.Interfaces;

namespace TeamSpecs.RideAlong.ServiceLog
{
    public class SqlDbServiceLogServiceTarget : ISqlDbServiceLogTarget
    {

        private readonly IGenericDAO _dao;

        SqlDbServiceLogServiceTarget(IGenericDAO dao)
        {
            _dao = dao;
        }

        #region Private Functions (Creating Sql Commands) 
        private IResponse CreateServiceLogSqlCommand(IServiceLogModel serviceLog)
        {
            #region Varaibles 
            var commandSql = "INSERT INTO ";
            var tableSql = "ServiceLog ";
            var rowsSql = "(";
            var valuesSql = "VALUES (";

            var response = new Response() { ReturnValue = new List<object>() };
            #endregion

            #region Building Sql Command based on ServiceLog Obj. 

            try
            {
                // create new hash set of SqlParameters
                var parameters = new HashSet<SqlParameter>();

                // convert VehicleProfile model to sql statement
                var configType = typeof(IServiceLogModel);
                var properties = configType.GetProperties();

                foreach (var property in properties)
                {
                    if (property.GetValue(serviceLog) != null)
                    {
                        rowsSql += property.Name + ",";
                        valuesSql += "@" + property.Name + ",";

                        parameters.Add(new SqlParameter("@" + property.Name, property.GetValue(serviceLog)));
                    }

                }
                rowsSql = rowsSql.Remove(rowsSql.Length - 1, 1);
                valuesSql = valuesSql.Remove(valuesSql.Length - 1, 1);
                rowsSql += ") ";
                valuesSql += ");";

                var sqlString = commandSql + tableSql + rowsSql + valuesSql;

                // Add string and hash set to list that the dao will execute
                response.ReturnValue.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Could not generate Service Log creation Sql";
                return response;
            }

            #endregion

            response.HasError = false;
            return response;
        }

        private IResponse CreateModifyServiceLogSqlCommand(IServiceLogService serviceLog)
        {
            #region Varaibles 
            var commandSql = "INSERT INTO ";
            var tableSql = "ServiceLog ";
            var rowsSql = "(";
            var valuesSql = "VALUES (";
            string query = "" +
                "UPDATE ServiceLog" +
                "SET Thing = thing";

            var response = new Response() { ReturnValue = new List<object>() };
            #endregion

            #region Building Sql Command based on ServiceLog Obj. 

            try
            {
                // create new hash set of SqlParameters
                var parameters = new HashSet<SqlParameter>();

                // convert VehicleProfile model to sql statement
                var configType = typeof(IServiceLogModel);
                var properties = configType.GetProperties();

                foreach (var property in properties)
                {
                    if (property.GetValue(serviceLog) != null)
                    {
                        rowsSql += property.Name + ",";
                        valuesSql += "@" + property.Name + ",";

                        parameters.Add(new SqlParameter("@" + property.Name, property.GetValue(serviceLog)));
                    }

                }
                rowsSql = rowsSql.Remove(rowsSql.Length - 1, 1);
                valuesSql = valuesSql.Remove(valuesSql.Length - 1, 1);
                rowsSql += ") ";
                valuesSql += ");";

                var sqlString = commandSql + tableSql + rowsSql + valuesSql;

                // Add string and hash set to list that the dao will execute
                response.ReturnValue.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Could not generate Service Log creation Sql";
                return response;
            }

            #endregion

            response.HasError = false;
            return response;
        }

        private IResponse CreateDeleteServiceLogSql(string vin, int serviceLogPosition)
        {
            #region Sql Command Default Build 
            string query = @"
            DELETE FROM ServiceLog
            WHERE ServiceLogID IN (
                SELECT ServiceLogID
                FROM (
                    SELECT ServiceLogID,
                           ROW_NUMBER() OVER (ORDER BY [Date] DESC) AS RowNumber
                    FROM ServiceLog
                    WHERE VIN = @Vin
                ) AS OrderedServiceLogs
                WHERE RowNumber = @Position
            )";
            var response = new Response() { ReturnValue = new List<object>() };
            #endregion

            try
            {
                var parameters = new HashSet<SqlParameter>();

                parameters.Add(new SqlParameter("@Vin", vin));
                parameters.Add(new SqlParameter("@Position", serviceLogPosition));

                response.ReturnValue.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(query, parameters));
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Service Log Deletion Sql generation failed";
                return response;
            
            }

            response.HasError = false;
            return response;
        }

        private SqlCommand CreateRetrieveServiceLogSqlCommand(IPaginationModel page, string vin)
        {
            #region Variables
            SqlCommand command = new SqlCommand();

            string query = $@"
            SELECT *,
                RANK() OVER (ORDER BY [Date]) AS RowRank
            FROM ServiceLog
            WHERE VIN = @Vin";

            List<string> whereConditions = new List<string>();

            string orderByClause = "";
            #endregion

            #region Reading filters 
            if (page.filter != null && page.filter.Count() > 0)
            {
                foreach (string condition in page.filter)
                {
                    if (condition.StartsWith("Where:"))
                    {
                        string whereCondition = condition.Substring("Where:".Length).Trim();
                        whereConditions.Add(whereCondition);
                    }
                    else if (condition.StartsWith("OrderBy:"))
                    {
                        orderByClause = condition.Substring("OrderBy:".Length).Trim();
                    }
                }

                // Construct the WHERE clause from filter conditions
                string whereClause = string.Join(" AND ", whereConditions);

                // Append the WHERE clause to the query
                if (!string.IsNullOrEmpty(whereClause))
                {
                    query += " " + whereClause;
                }

                // Append the ORDER BY clause to the query
                if (!string.IsNullOrEmpty(orderByClause))
                {
                    query += " ORDER BY " + orderByClause;
                }
                else
                {
                    // Default ORDER BY clause if not provided
                    query += " ORDER BY [Date] ASC";
                }
            }
            #endregion

            #region Adding parameters 
            query += @"
            OFFSET (@PageNumber - 1) * @PageSize ROWS
            FETCH NEXT @PageSize ROWS ONLY";

            command.CommandText = query;
            command.Parameters.AddWithValue("@Vin", vin);
            command.Parameters.AddWithValue("@PageNumber", page.pageNumber);
            command.Parameters.AddWithValue("@PageSize", page.pageSize);
            #endregion

            return command;
        }

        private SqlCommand CreateMantainenceReminderSql()
        {
            // subject to change
            throw new NotImplementedException();
        }

        #endregion

        public IResponse GenerateCreateServiceLogSql (IServiceLogModel serviceLog)
        {
            #region Variables
            IResponse response = new Response();
            var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
            #endregion

            #region Building Sql Command
            // Pass reference of servicelog to sql build function
            var serviceLogSql = CreateServiceLogSqlCommand(serviceLog);

            if (serviceLogSql.HasError == false)
            {
                if (serviceLogSql.ReturnValue is not null)
                {
                    sqlCommands.Add((KeyValuePair<string, HashSet<SqlParameter>?>)serviceLogSql.ReturnValue.First());
                }
            }
            else
            {
                return serviceLogSql;
            }

            #endregion

            #region Executing Write to DB
            // DAO Executes the command
            try
            {
                var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
                response.ReturnValue = new List<object>() { daoValue };
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Database execution failed";
                return response;
            }

            #endregion

            response.HasError = false;
            return response;
        }

        public IResponse GenerateModifyServiceLogSql(IServiceLogModel serviceLog)
        {
            throw new NotImplementedException();
        }

        public IResponse GenerateDeleteServiceLogSql(string vin, int serviceLogPostion)
        {
            #region Variables
            IResponse response = new Response();
            var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
            #endregion

            #region Building Sql Command
            // Pass reference of servicelog to sql build function
            var serviceLogSql = CreateDeleteServiceLogSql(vin, serviceLogPostion);

            if (serviceLogSql.HasError == false)
            {
                if (serviceLogSql.ReturnValue is not null)
                {
                    sqlCommands.Add((KeyValuePair<string, HashSet<SqlParameter>?>)serviceLogSql.ReturnValue.First());
                }
            }
            else
            {
                return serviceLogSql;
            }

            #endregion

            #region Executing Write to DB
            // DAO Executes the command
            try
            {
                var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
                response.ReturnValue = new List<object>() { daoValue };
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Database execution failed";
                return response;
            }

            #endregion

            response.HasError = false;
            return response;
        }

        public IResponse GenerateRetrieveServiceLogSql(IPaginationModel page, string vin)
        {
            #region Variables
            IResponse response = new Response();
            SqlCommand command = new SqlCommand();
            #endregion

            #region SqlCommand Creation 
            try
            {
                command = CreateRetrieveServiceLogSqlCommand(page, vin);
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Could not generate Service Log retrieval Sql.";
                return response;
            }

            #endregion

            #region DAO execution 
            try
            {
                response = _dao.ExecuteReadOnly(command);
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "Unable to retrieve Service Logs";
                return response;
            }
            #endregion

            response.HasError = false;
            return response;
        }

        public IResponse GenerateCreateMantainenceReminderSql()
        {
            #region Variables
            IResponse response = new Response();
            #endregion

            throw new NotImplementedException();
        }
    }
}
