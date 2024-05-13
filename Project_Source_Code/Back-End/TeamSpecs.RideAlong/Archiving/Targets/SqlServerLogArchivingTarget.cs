using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Archiving
{
    public class SqlServerLogArchivingTarget : IArchivingTarget
    {
        private readonly ILogTarget _logTarget;
        private readonly ISqlServerDAO _dao;

        #region Target Template Functions
        private static void MapValues(object obj, object[] values)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var properties = obj.GetType().GetProperties();

            for (int i = 0; i < Math.Min(properties.Length, values.Length); i++)
            {
                PropertyInfo property = properties[i];
                object value = values[i];

                // Check if property is writable and value is compatible
                if (property.CanWrite && (value == null || property.PropertyType.IsAssignableFrom(value.GetType())))
                {
                    property.SetValue(obj, value);
                }
                else
                {
                    throw new ArgumentException($"Value at index {i} is not assignable to property '{property.Name}'.");
                }
            }
        }
        private static HashSet<SqlParameter> CreateSqlParameters(object obj)
        {
            HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();

            if (obj == null)
                return parameters;

            Type objectType = obj.GetType();

            if (IsPrimitiveType(objectType))
            {
                // Create a single parameter for primitive types
                parameters.Add(CreateParameter($"@{nameof(obj)}", GetSqlType(objectType), obj));
            }
            else
            {
                var properties = objectType.GetProperties();

                foreach (var property in properties)
                {
                    try
                    {
                        string paramName = "@" + property.Name;

                        SqlDbType sqlType = GetSqlType(property.PropertyType);

                        SqlParameter parameter = new SqlParameter(paramName, sqlType);
                        if (property.GetValue(obj) is not null)
                        {
                            parameter.Value = property.GetValue(obj);
                        }
                        else
                        {
                            parameter.Value = DBNull.Value;
                        }
                        parameters.Add(parameter);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return parameters;
        }
        private static bool IsPrimitiveType(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type == typeof(DateTime);
        }
        private static SqlParameter CreateParameter(string paramName, SqlDbType sqlType, object? value)
        {
            SqlParameter parameter = new SqlParameter(paramName, sqlType);
            if (value is null)
            {
                value = DBNull.Value;
            }
            parameter.Value = value;
            return parameter;
        }
        private static SqlDbType GetSqlType(Type type)
        {
            if (type == typeof(string) || type == typeof(string))
            {
                return SqlDbType.VarChar;
            }
            else if (type == typeof(long) || type == typeof(long?))
            {
                return SqlDbType.BigInt;
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                return SqlDbType.Int;
            }
            else if (type == typeof(float) || type == typeof(float?))
            {
                return SqlDbType.Float;
            }
            else if (type == typeof(DateTime))
            {
                return SqlDbType.DateTime;
            }
            else
            {
                throw new ArgumentException("Unsupported data type for SQL parameter.");
            }
        }
        private static HashSet<SqlParameter> RemoveSqlParameter(HashSet<SqlParameter> parameters, string paramName)
        {
            var parameterToRemove = parameters.FirstOrDefault(p => p.ParameterName == paramName);
            if (parameterToRemove != null)
            {
                parameters.Remove(parameterToRemove);
            }
            return parameters;
        }
        private IResponse createErrorResponse(Exception ex)
        {
            IResponse errorResponse = new Response();
            errorResponse.HasError = true;
            errorResponse.ErrorMessage = ex.Message;
            return errorResponse;
        }
#endregion

        public SqlServerLogArchivingTarget(ILogTarget logTarget, ISqlServerDAO dao)
        {
            _logTarget = logTarget;
            _dao = dao;
        }
        public IResponse GetLogs(DateTime beforeDate, DateTime? butNotBefore)
        {
            // Create SQL command list
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

            // Create SQL command text
            string commandText = "SELECT LogTime, LogLevel, LogCategory, LogContext, LogHash, UserHash FROM LoggingTable WHERE LogTime < @beforeDate";

            // Create Parameters
            HashSet<SqlParameter> parameters = [CreateParameter("@beforeDate", SqlDbType.Date, beforeDate)];

            // Make Necessary Tweaks
            if (butNotBefore is not null)
            {
                commandText += " AND LogTime >= @butNotBefore;";
                parameters.Add(CreateParameter("@butNotBefore", SqlDbType.Date, butNotBefore));
            }
            else
            {
                commandText += ";";
            }

            // Create Key Value pairs with sql and parameters
            KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(commandText, parameters);
            sqlCommandList.Add(sqlStatement);

            try
            {
                // Attempt SQL Execution
                List<object[]> rowsReturned = _dao.ExecuteReadOnly(sqlCommandList);

                // Create return value object
                List<object> returnValue = new List<object>();

                // Loop through rows
                foreach (object[] row in rowsReturned)
                {
                    // Map values from row into return object
                    ILog log = new Log((DateTime)row[0], (string)row[1], (string)row[2], (string)row[3], (string)row[4], (string)row[5] ?? null);

                    // Add to return value object
                    returnValue.Add(log);
                }
                // Return success with return value object in place of null
                IResponse sucessResponse = new Response();
                sucessResponse.HasError = false;
                sucessResponse.ReturnValue = returnValue;
                return sucessResponse;
                
            }
            catch (Exception ex)
            {
                // Return Error if we cannot execute sql successfullly
                return createErrorResponse(ex);
            }
        }
        public IResponse SetLogs(List<ILog> logs)
        {
            var tasks = new List<Task>();
            try
            {
                foreach (var log in logs)
                {
                    tasks.Add(Task.Run(() => _logTarget.WriteLog(log)));
                }
                Task.WhenAll(tasks);
            }
            catch (Exception ex) {
                IResponse failResponse = new Response();
                failResponse.ErrorMessage = ex.Message;
                return failResponse;
            }
            IResponse response = new Response();
            response.HasError = false;
            return response;
        }
    }
}
