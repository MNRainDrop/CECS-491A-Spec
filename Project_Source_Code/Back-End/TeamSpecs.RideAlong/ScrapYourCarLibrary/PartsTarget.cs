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
    public class PartsTarget : IPartsTarget
    {    
        private IGenericDAO _dao;
        private ILogService _logger;
        public PartsTarget(IGenericDAO dao, ILogService logger)
        {
            _dao = dao;
            _logger = logger;
        }

        private static SqlDbType GetSqlType(Type type)
        {
            if (type == typeof(string))
            {
                return SqlDbType.VarChar;
            }
            else if (type == typeof(long))
            {
                return SqlDbType.BigInt;
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
        private static HashSet<SqlParameter> CreateSqlParameters(object obj)
        {
            HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();

            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                string paramName = "@" + property.Name;
                SqlDbType sqlType = GetSqlType(property.PropertyType);

                SqlParameter parameter = new SqlParameter(paramName, sqlType);
                parameter.Value = property.GetValue(obj);

                parameters.Add(parameter);
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
            errorResponse.ErrorMessage = "Error retrieving parts data: " + ex.Message;
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

        private static List<string> GetMemberVariableNames(object obj)
        {
            List<string> memberVariableNames = new List<string>();

            Type type = obj.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (FieldInfo field in fields)
            {
                memberVariableNames.Add(field.Name);
            }

            return memberVariableNames;
        }
        private HashSet<SqlParameter> createParameters(List<string> preParams)
        {
            
        }

        public IResponse SetCarPart(ICarPart part)
        {
            // Create sql command list object
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

            // Create sql command text
            string commandText = "INSERT INTO CarParts (ownerUID, partName, partNumber, make, model, year, associatedVin)" +
                "VALUES (@uid, @name, @number, @make, @model, @year, @associatedVin)";

            // Create Parameters
            HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();

            var ownerUIDParam = new SqlParameter();
            ownerUIDParam.Value = part.ownerUID;

            parameters.Add(ownerUIDParam);

            var partNameParam = new SqlParameter();
            partNameParam.Value = part.partName;
            parameters.Add(partNameParam);

            var partNumberParam = new SqlParameter();
            partNumberParam.Value = part.partNumber;
            parameters.Add(partNumberParam);

            var makeParam = new SqlParameter();
            makeParam.Value = part.make;
            parameters.Add(makeParam);

            var modelParam = new SqlParameter();
            modelParam.Value = part.model;
            parameters.Add(modelParam);

            var yearParam = new SqlParameter();
            yearParam.Value = part.year;
            parameters.Add(yearParam);

            var associatedVinParam = new SqlParameter();
            associatedVinParam.Value = part.associatedVin;
            parameters.Add(associatedVinParam);

            //Create Key value pair with sql and parameters
            KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(commandText, parameters);

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

        public IResponse AmendListing(IListing updatingListing)
        {
            // Create SQL command list
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

            // Create SQL command text
            string commandText = "";

            // Create Parameters
            HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();

            var Param = new SqlParameter();
            Param.Value = updatingListing.part.partUID;
            parameters.Add(Param);
            
            // Create Key Value pairs with sql and parameters
            KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(commandText, parameters);

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

        public IResponse GetMatchingParts(List<ICarPart> part)
        {
            throw new NotImplementedException();
        }

        public IResponse GetPartListing(ICarPart part)
        {
            throw new NotImplementedException();
        }

        public IResponse GetUserListings(long uid)
        {
            throw new NotImplementedException();
        }

        public IResponse GetUserParts(long uid)
        {
            throw new NotImplementedException();
        }

        public IResponse RemoveListing(IListing listing)
        {
            // Create SQL command list
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

            // Create SQL command text
            string commandText = "";

            // Create Parameters
            HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();

            var Param = new SqlParameter();
            Param.Value = listing.part.partUID;
            parameters.Add(Param);

            // Create Key Value pairs with sql and parameters
            KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(commandText, parameters);

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

        public IResponse RemoveParts(ICarPart part)
        {
            // Create SQL command list
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

            // Create SQL command text
            string commandText = "";

            // Create Parameters
            HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();

            var Param = new SqlParameter();
            Param.Value = part.partUID;
            parameters.Add(Param);

            // Create Key Value pairs with sql and parameters
            KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(commandText, parameters);

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

        public IResponse SetListing(IListing listing)
        {
            // Create SQL command list
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

            // Create SQL command text
            string commandText = "";

            // Create Parameters
            HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();

            var Param = new SqlParameter();
            Param.Value = listing.part.partUID;
            parameters.Add(Param);

            // Create Key Value pairs with sql and parameters
            KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(commandText, parameters);

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
    }
}
