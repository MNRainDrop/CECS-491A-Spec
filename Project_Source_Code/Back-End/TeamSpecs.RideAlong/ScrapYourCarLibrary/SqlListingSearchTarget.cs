using Microsoft.Data.SqlClient;
using ScrapYourCarLibrary.Interfaces;
using ScrapYourCarLibrary.Models;
using System.Data;
using System.Reflection;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.ScrapYourCarLibrary.Models;

namespace TeamSpecs.RideAlong.ScrapYourCarLibrary
{
    public class SqlListingSearchTarget : IListingSearchTarget
    {
        private IGenericDAO _dao;
        private ILogService _logger;
        public SqlListingSearchTarget(IGenericDAO dao, ILogService logger)
        {
            _dao = dao;
            _logger = logger;
        }
        #region Utility Functions
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
        /// <summary>
        /// Utility function used to avoid repeated code
        /// Simply takes in an exception as a parameter, 
        /// then generates both a log and a tailored response object to the error recorded in the exeption
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>Response object, with error message based on exception</returns>
        private IResponse createErrorResponse(Exception ex)
        {
            IResponse errorResponse = new Response();
            errorResponse.HasError = true;
            errorResponse.ErrorMessage = "Error retrieving user data: " + ex.Message;
            _logger.CreateLogAsync("Error", "Data Store", errorResponse.ErrorMessage, null);
            return errorResponse;
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
                parameters.Add(CreateParameter("@value", GetSqlType(objectType), obj));
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
        /// <summary>
        /// Usage: parameters = RemoveSqlParameter(parameters, "@paramNameToRemove");
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        private static HashSet<SqlParameter> RemoveSqlParameter(HashSet<SqlParameter> parameters, string paramName)
        {
            var parameterToRemove = parameters.FirstOrDefault(p => p.ParameterName == paramName);
            if (parameterToRemove != null)
            {
                parameters.Remove(parameterToRemove);
            }
            return parameters;
        }
        #endregion

        private ISearchParameterValidator searchBuilder(ISearchParameters searchBy)
        {
            string whereSql = ""; int pageSize = 0; int page = 0;

            Type objectType = searchBy.GetType();
            var properties = objectType.GetProperties();

            List<string> rejectParams = new List<string>();

            foreach (var property in properties)
            {
                string propName = property.Name;
                var propValue = property.GetValue(searchBy);
                if (propName != "page")
                {
                    page = Convert.ToInt32(propValue);
                }
                else if (propName != "pageSize")
                {
                    pageSize = Convert.ToInt32(propValue);
                    page = page * pageSize;
                }
                else if (propValue is not null)
                {
                    if (whereSql == "")
                    {
                        whereSql += "WHERE ";
                    }
                    else
                    {
                        whereSql += "AND ";
                    }
                    whereSql += $"p.{propName} LIKE '%@{propName}%' ";
                }
                else
                {
                    rejectParams.Add("@" + propName);
                }
            }
            whereSql += $"ORDER BY l.price OFFSET @page ROWS FETCH NEXT @pageSize ROWS ONLY; ";
            SearchParameterValidator validator = new SearchParameterValidator(whereSql, page, pageSize, rejectParams);
            return validator;
        }

        public IResponse GetListingsBySearch(ISearchParameters searchBy)
        {
            // Create SQL command list
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

            // Create SQL command text
            string commandText = "SELECT p.partUID, p.ownerUID, p.partName, p.partNumber, p.make, p.model, p.year, p.associatedVin, l.price, l.description" +
                "FROM Parts p" +
                "INNER JOIN Listings l ON p.partUID = l.partUID ";

            // Validator
            ISearchParameterValidator validator = searchBuilder(searchBy);
            // The following line adds pagination and search parameters from the earlier function
            commandText += validator.whereSql;

            // Create Parameters (The function creates a parameter for all of them, so i have to remove the null params afterwards)
            HashSet<SqlParameter> parameters = CreateSqlParameters(searchBy);
            RemoveSqlParameter(parameters, "@page");
            RemoveSqlParameter(parameters, "@pageSize");
            foreach (string param in validator.rejectedParams)
                RemoveSqlParameter(parameters, param);

            // Re add page and page size with correct values
            parameters.Add(CreateParameter("@page", SqlDbType.Int, validator.startOnRow));
            parameters.Add(CreateParameter("@pageSize", SqlDbType.Int, validator.pageSize));

            // Create Key Value pairs with sql and parameters
            KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(commandText, parameters);
            sqlCommandList.Add(sqlStatement);

            try
            {
                // Attempt SQL Execution
                List<object[]> rowsReturned = _dao.ExecuteReadOnly(sqlCommandList);

                // Create return value object
                IResponse response = new Response();
                response.ReturnValue = new List<object>();
                response.HasError = false;

                // Loop through rows
                foreach (object[] row in rowsReturned)
                {
                    // Map values from row into return object
                    object[] partValues = new object[] { row[0], row[1], row[2], row[3], row[4], row[5], row[6], row[7] };
                    ICarPart part = new CarPart();
                    MapValues(part, partValues);

                    IListing listing = new Listing(part, (float)(double)row[8], (string)row[9]);
                    // Add to return value object
                    response.ReturnValue.Add(listing);
                }
                // Return success
                return response;
            }
            catch (Exception ex)
            {
                // Return Error if we cannot execute sql successfullly
                return createErrorResponse(ex);
            }
        }
    }
}
