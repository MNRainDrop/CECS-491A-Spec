using Microsoft.Data.SqlClient;
using ScrapYourCarLibrary.Interfaces;
using ScrapYourCarLibrary.Models;
using System.Data;
using System.Reflection;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace ScrapYourCarLibrary
{
    public class SqlPartsTarget : IPartsTarget
    {
        private IGenericDAO _dao;
        private ILogService _logger;
        public SqlPartsTarget(IGenericDAO dao, ILogService logger)
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
            errorResponse.ErrorMessage = ex.Message;
            _logger.CreateLogAsync("Error", "Data Store", errorResponse.ErrorMessage, null);
            return errorResponse;
        }
        /// <summary>
        /// Utility function used to avoid repeated code
        /// Simply takes in an object as a parameter
        /// then generates a response to encapsulate it
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Error free Response Object with whatever you put in</returns>
        private IResponse createSuccessResponse(object? item)
        {
            // Pack it up and ship it
            IResponse successResponse = new Response();
            successResponse.HasError = false;
            if (item is not null)
            {
                successResponse.ReturnValue = new List<object>() { item };
            }
            return successResponse;
        }
        #endregion

        #region Implementation
        public IResponse SetCarPart(ICarPart part)
        {
            // Create sql command list object
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

            // Create sql command text
            string commandText = "INSERT INTO Parts (ownerUID, partName, partNumber, make, model, year, associatedVin) " +
                "VALUES (@ownerUID, @partName, @partNumber, @make, @model, @year, @associatedVin)";

            // Create Parameters
            HashSet<SqlParameter> parameters = CreateSqlParameters(part);

            parameters = RemoveSqlParameter(parameters, "@partUID");

            //Create Key value pair with sql and parameters
            KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(commandText, parameters);
            sqlCommandList.Add(sqlStatement);
            try
            {
                // Attempt SQL Execution
                int rowsAffected = _dao.ExecuteWriteOnly(sqlCommandList);
                if (rowsAffected == 0) { throw new Exception("No rows affected"); }
                return createSuccessResponse(null);
            }
            catch (Exception ex)
            {
                return createErrorResponse(ex);
            }
        }

        public IResponse AmendListing(IListing updatingListing)
        {
            // Create SQL command list
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

            // Create SQL command text
            string commandText = "UPDATE Listings " +
                "SET price = @price, description = @description " +
                "WHERE partUID = @partUID;";

            // Create Parameters
            HashSet<SqlParameter> parameters = CreateSqlParameters(updatingListing);

            // Add Part UID Parameter
            var partUIDParam = new SqlParameter("@partUID", SqlDbType.BigInt);
            partUIDParam.Value = updatingListing.part.partUID;
            parameters.Add(partUIDParam);

            // Create Key Value pairs with sql and parameters
            KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(commandText, parameters);

            sqlCommandList.Add(sqlStatement);

            try
            {
                // Attempt SQL Execution
                int rowsAffected = _dao.ExecuteWriteOnly(sqlCommandList);
                if (rowsAffected == 0) { throw new Exception("No rows affected"); }
                return createSuccessResponse(null);
            }
            catch (Exception ex)
            {
                return createErrorResponse(ex);
            }
        }
        /// <summary>
        /// Used to get parts that match the parts passed in
        /// Primarily for internal use, to get partUID
        /// </summary>
        /// <param name="parts"></param>
        /// <returns> IResponse with list of matching parts </returns>
        public IResponse GetMatchingParts(List<ICarPart> parts)
        {
            // Create SQL command list
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

            foreach (var part in parts)
            {
                // Create SQL command text
                string commandText = "SELECT partUID, ownerUID, partName, partNumber, make, model, year, associatedVin " +
                    "FROM Parts " +
                    "WHERE partName = @partName AND ownerUID = @ownerUID;";

                // Create Parameters
                HashSet<SqlParameter> parameters = CreateSqlParameters(part);

                // Make Necessary Tweaks
                parameters = RemoveSqlParameter(parameters, "@partUID");

                // Create Key Value pairs with sql and parameters
                KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(commandText, parameters);
                sqlCommandList.Add(sqlStatement);
            }

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
                    ICarPart returnedPart = new CarPart();
                    MapValues(returnedPart, row);

                    // Add to return value object
                    returnValue.Add(returnedPart);
                }
                if (returnValue.Count == 0)
                {
                    throw new Exception("No Rows Returned");
                }
                // Return success with return value object in place of null
                return createSuccessResponse(returnValue);
            }
            catch (Exception ex)
            {
                // Return Error if we cannot execute sql successfullly
                return createErrorResponse(ex);
            }
        }

        public IResponse GetPartListing(ICarPart part)
        {
            // Create SQL command list
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

            // Create SQL command text
            string commandText = "SELECT price, description " +
                "FROM Listings " +
                "WHERE partUID = @partUID";

            // Create Parameters
            long? partUID;
            #region Validate existence of partUID
            try
            {
                if (part.partUID is not null)
                {
                    partUID = part.partUID;
                }
                else
                {
                    throw new Exception("partUID is null");
                }
            }
            catch (Exception ex)
            {
                return createErrorResponse(ex);
            }
            #endregion
            HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();
            parameters.Add(CreateParameter("@partUID", SqlDbType.BigInt, partUID));

            // Create Key Value pairs with sql and parameters
            KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(commandText, parameters);
            sqlCommandList.Add(sqlStatement);
            try
            {
                // Attempt SQL Execution
                List<object[]> rowsReturned = _dao.ExecuteReadOnly(sqlCommandList);

                // Create return value object
                IListing? returnedListing = null;

                // Loop through rows
                foreach (object[] row in rowsReturned)
                {
                    if (row[0] is not null && row[1] is not null)
                    {
                        returnedListing = new Listing(part, (float)(double)row[0], (string)row[1]);
                    }
                }
                // Return success with return value object in place of null
                if (returnedListing is null)
                {
                    throw new Exception("No Rows Returned");
                }
                return createSuccessResponse(returnedListing);
            }
            catch (Exception ex)
            {
                // Return Error if we cannot execute sql successfullly
                return createErrorResponse(ex);
            }
        }

        /// <summary>
        /// Used to get all the users car parts. Can be used in conjunction with GetPartListing to get all the user's listings
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>IResponse with list of Parts</returns>
        public IResponse GetUserParts(long uid)
        {
            // Create SQL command list
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

            // Create SQL command text
            string commandText = "SELECT partUID, ownerUID, partName, partNumber, make, model, year, associatedVin " +
                    "FROM Parts " +
                    "WHERE ownerUID = @uid;";

            // Create Parameters
            HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();
            parameters.Add(CreateParameter("@uid", SqlDbType.BigInt, uid));

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
                    ICarPart returnedPart = new CarPart();
                    MapValues(returnedPart, row);

                    // Add to return value object
                    returnValue.Add(returnedPart);
                }

                // Return success with return value object in place of null
                if (returnValue.Count == 0)
                {
                    throw new Exception("No Rows Returned");
                }
                return createSuccessResponse(returnValue);
            }
            catch (Exception ex)
            {
                // Return Error if we cannot execute sql successfullly
                return createErrorResponse(ex);
            }
        }

        /// <summary>
        /// Deletes the listing for the part that is passed in
        /// </summary>
        /// <param name="part"></param>
        /// <returns>IResponse with outcome</returns>
        public IResponse RemoveListing(ICarPart part)
        {
            // Create SQL command list
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

            // Create SQL command text
            string commandText = "DELETE FROM Listings " +
                "WHERE partUID = @partUID";

            // Create Parameters
            HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();
            parameters.Add(CreateParameter("@partUID", SqlDbType.BigInt, part.partUID!));

            // Create Key Value pairs with sql and parameters
            KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(commandText, parameters);
            sqlCommandList.Add(sqlStatement);

            try
            {
                // Attempt SQL Execution
                int rowsAffected = _dao.ExecuteWriteOnly(sqlCommandList);
                if (rowsAffected == 0) { throw new Exception("No rows affected"); }
                return createSuccessResponse(null);
            }
            catch (Exception ex)
            {
                return createErrorResponse(ex);
            }
        }

        public IResponse RemoveParts(ICarPart part)
        {
            // Create SQL command list
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

            // Create SQL command text
            string commandText = "DELETE FROM Parts " +
                "WHERE partUID = @partUID";

            // Create Parameters
            HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();
            parameters.Add(CreateParameter("@partUID", SqlDbType.BigInt, part.partUID!));

            // Create Key Value pairs with sql and parameters
            KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(commandText, parameters);
            sqlCommandList.Add(sqlStatement);

            try
            {
                // Attempt SQL Execution
                int rowsAffected = _dao.ExecuteWriteOnly(sqlCommandList);
                if (rowsAffected == 0) { throw new Exception("No rows affected"); }
                return createSuccessResponse(null);
            }
            catch (Exception ex)
            {
                return createErrorResponse(ex);
            }
        }

        public IResponse SetListing(IListing listing)
        {
            // Create SQL command list
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

            // Create SQL command text
            string commandText = "INSERT INTO Listings (partUID, price, description) " +
                "VALUES (@partUID, @price, @description)";

            // Create Parameters
            HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();
            parameters.Add(CreateParameter("@partUID", SqlDbType.BigInt, listing.part.partUID!));
            parameters.Add(CreateParameter("@price", SqlDbType.Float, listing.price));
            parameters.Add(CreateParameter("@description", SqlDbType.VarChar, listing.description));

            // Create Key Value pairs with sql and parameters
            KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(commandText, parameters);
            sqlCommandList.Add(sqlStatement);
            try
            {
                // Attempt SQL Execution
                int rowsAffected = _dao.ExecuteWriteOnly(sqlCommandList);
                if (rowsAffected == 0) { throw new Exception("SQLDB error, no rows affected"); }
                return createSuccessResponse(null);
            }
            catch (Exception ex)
            {
                return createErrorResponse(ex);
            }
        }
        #endregion
    }
}
