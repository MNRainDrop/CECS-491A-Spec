
using Microsoft.Data.SqlClient;
using System.Data;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary.Model;

namespace TeamSpecs.RideAlong.SecurityLibrary.Targets
{

    public class SQLServerAuthTarget : IAuthTarget
    {
        IGenericDAO _dao;
        ILogService _logger;
        public SQLServerAuthTarget(IGenericDAO dao, ILogService logger)
        {
            _dao = dao;
            _logger = logger;

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
        /// <summary>
        /// Gets the rows from the "UserAccount" table in the datastore, and combines them into a AuthUserModel object
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public IResponse fetchUserModel(string username)
        {
            // Create SQL statement and parameters
            SqlCommand sql = new SqlCommand();
            sql.CommandText = "SELECT UID, Salt, UserHash FROM UserAccount WHERE UserName = @username";
            sql.Parameters.AddWithValue("@username", username);
            // Attempt SQL Execution
            IResponse response = _dao.ExecuteReadOnly(sql);
            // Validate SQL return statement
            if (response.HasError == true)
            {
                return response;
            }
            else
            {
                try
                {
                    List<object> userData;
                    #region Input validation and assignment
                    if (response.ReturnValue is not null)
                    {
                        // Retrieve data from the response
                        userData = (List<object>)response.ReturnValue;
                    }
                    else { throw new Exception("Database Response has null return value"); }

                    // Check if any data was returned
                    if (userData is not null && userData.Count == 0) { throw new Exception("User was not Found"); }

                    if (userData is null) { throw new Exception("User Data returned is null"); }
                    #endregion

                    // Assuming only one row is returned for the given username
                    object[] userRow = (object[])userData[0];

                    // Create a UserModel object to store the retrieved data
                    IAuthUserModel userModel = new AuthUserModel
                    {
                        UID = (long)userRow[0],
                        userName = username,
                        salt = BitConverter.GetBytes((int)userRow[1]),
                        userHash = (string)userRow[2]
                    };

                    return createSuccessResponse(userModel);
                }
                catch (Exception ex)
                {
                    // If an exception occurs during data retrieval or processing

                    return createErrorResponse(ex);
                }
            }

        }
        /// <summary>
        /// Stores the given pass to the Datastore row with the matching UID
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="passHash"></param>
        /// <returns>Response with rows affected</returns>
        public IResponse savePass(long UID, string passHash)
        {
            // Create SQL statement
            //The following SQL command tries to update a row for the user's otp. If the row doesnt exist, then it creates it
            string CommandText =
                "BEGIN TRANSACTION " +
                "UPDATE OTP SET PassHash = @passHash WHERE UID = @uid;" +
                "IF @@ROWCOUNT = 0" +
                "BEGIN INSERT INTO OTP (UID, PassHash, attempts, firstFailedLogin) VALUES (@uid, @passHash, 0, @firstFailedLogin); END " +
                "COMMIT TRANSACTION;";

            //Create parameters
            HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();
            SqlParameter uidParam = new SqlParameter("@uid", SqlDbType.BigInt);
            uidParam.Value = UID;
            SqlParameter passHashParam = new SqlParameter("@passHash", SqlDbType.VarChar);
            passHashParam.Value = passHash;
            SqlParameter failedLoginParam = new SqlParameter("@firstFailedLogin", SqlDbType.DateTime);
            failedLoginParam.Value = DateTime.UtcNow;
            parameters.Add(uidParam);
            parameters.Add(passHashParam);
            parameters.Add(failedLoginParam);

            //Create Key Value pair with sql string and parameters
            KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(CommandText, parameters);

            //Add sql statement to a collection
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
            sqlCommandList.Add(sqlStatement);

            // Attempt SQL Execution
            int rowsAffected = _dao.ExecuteWriteOnly(sqlCommandList);

            // Validate SQL return statement
            try
            {
                if (rowsAffected == 0) { throw new Exception("SQLDB error, no rows affected"); }
                return createSuccessResponse(null);
            }
            catch (Exception ex)
            {
                return createErrorResponse(ex);
            }
        }
        /// <summary>
        /// Retrieves the passcode from the Datastore
        /// </summary>
        /// <param name="UID"></param>
        /// <returns>Response Object with passcode</returns>
        public IResponse fetchPass(long UID)
        {
            // Create SQL statement and parameters
            SqlCommand sql = new SqlCommand();
            sql.CommandText = "SELECT PassHash FROM OTP WHERE UID = @uid";
            sql.Parameters.AddWithValue("@uid", UID);
            // Execute SQL statement
            IResponse response = _dao.ExecuteReadOnly(sql);
            // Validate SQL response
            if (response.HasError)
            {
                return response;
            }
            else
            {
                try
                {
                    List<object> rowsReturned;

                    #region validate assign validate
                    // Validate we have a return value
                    if (response.ReturnValue is not null) { rowsReturned = (List<object>)response.ReturnValue; }
                    else { throw new Exception("No Return value from DB"); }

                    // Check if we got any rows
                    if (rowsReturned.Count == 0) { throw new Exception("No Rows Returned"); }

                    #endregion

                    // We assume one row, so we only fetch that one row
                    object[] row = (object[])rowsReturned[0];
                    //Extract our passHash from the row
                    string passHash = (string)row[0];


                    return createSuccessResponse(passHash);
                }
                catch (Exception ex)
                {
                    return createErrorResponse(ex);
                }
            }
        }
        /// <summary>
        /// Fetches integer with login attempts from the Databbase
        /// </summary>
        /// <param name="UID"></param>
        /// <returns>Response Object with attempts as integer</returns>
        public IResponse fetchAttempts(long UID)
        {
            // Create SELECT SQL statement and parameters
            SqlCommand sql = new SqlCommand();
            sql.CommandText = "SELECT attempts FROM OTP WHERE UID = @uid";
            sql.Parameters.AddWithValue("@uid", UID);
            // Execute SQL statement
            IResponse response = _dao.ExecuteReadOnly(sql);
            // Validate SQL response
            if (response.HasError)
            {
                return response;
            }
            else
            {
                try
                {
                    List<object> rowsReturned;

                    #region Validate Assign Validate
                    if (response.ReturnValue is not null) { rowsReturned = (List<object>)response.ReturnValue; }
                    else { throw new Exception("No Return Value from DB"); }

                    if (rowsReturned.Count == 0) { throw new Exception("No Rows Returned"); }
                    #endregion

                    // We should only be getting one row
                    object[] row = (object[])rowsReturned[0];
                    // Extract attempts value from row
                    int attempts = (int)row[0];

                    return createSuccessResponse(attempts);
                }
                catch (Exception ex)
                {
                    return createErrorResponse(ex);
                }
            }

        }
        public IResponse updateAttempts(long UID, int attempts)
        {
            // Create SQL statment
            string commandText = "UPDATE OTP SET attempts = @attempts WHERE UID = @uid;";

            //Create Parameters
            HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();
            SqlParameter uidParam = new SqlParameter("@uid", SqlDbType.BigInt);
            uidParam.Value = UID;
            SqlParameter attemptsParam = new SqlParameter("@attempts", SqlDbType.Int);
            attemptsParam.Value = attempts;
            parameters.Add(uidParam);
            parameters.Add(attemptsParam);

            //Create Key value pair with sql and parameters
            KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(commandText, parameters);

            //Add sql statement to a collection
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
            sqlCommandList.Add(sqlStatement);

            // Attempt SQL Execution
            int rowsAffected = _dao.ExecuteWriteOnly(sqlCommandList);

            // Validate SQL Response
            try
            {
                if (rowsAffected == 0) { throw new Exception("SQLDB error, no rows affected"); }
                return createSuccessResponse(null);
            }
            catch (Exception ex)
            {
                return createErrorResponse(ex);
            }
        }

        /// <summary>
        /// Will return a Respone object with a dictionary full of claims.<br>
        /// NOTE: if a claim is formatted incorrectly(one or both values null), it will be skipped and a log will be made
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        public IResponse getClaims(long UID)
        {
            // Create SQL statement and parameters
            SqlCommand sql = new SqlCommand();
            sql.CommandText = "SELECT c.Claim, uc.ClaimScope FROM UserClaim uc INNER JOIN claim c on uc.ClaimId = c.ClaimId WHERE UID = @uid";
            sql.Parameters.AddWithValue("@uid", UID);
            // Execute SQL statement
            IResponse response = _dao.ExecuteReadOnly(sql);
            // Validate SQL response
            if (response.HasError)
            {
                return response;
            }
            else
            {
                try
                {
                    List<object> rowsReturned;

                    #region validate assign validate
                    if (response.ReturnValue is not null) { rowsReturned = (List<object>)response.ReturnValue; }
                    else { throw new Exception("NO Return Value from DB"); }

                    if (rowsReturned.Count == 0) { throw new Exception("No Rows Returned"); }

                    #endregion

                    ICollection<KeyValuePair<string, string>> claims = new List<KeyValuePair<string, string>>();

                    foreach (object[] row in rowsReturned)
                    {

                        string claimID = (string)row[0];
                        string claimScope = (string)row[1];
                        if (claimID is not null && claimScope is not null)
                        {
                            claims.Add(new KeyValuePair<string, string>(claimID, claimScope));
                        }
                        else
                        {
                            _logger.CreateLogAsync("Warning", "Data Store", $"Claim ID:{claimID}/Scope:{claimScope} for user {UID} is broken", null);
                            continue;
                        }
                    }
                    return createSuccessResponse(claims);
                }
                catch (Exception ex)
                {
                    return createErrorResponse(ex);
                }
            }
        }
        public IResponse setFirstFailedLogin(long uid, DateTime datetime)
        {
            // Make sql command

            string sql = "UPDATE OTP SET firstFailedLogin = @ffLogin WHERE UID = @uid";

            // Set up parameters
            HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();
            SqlParameter uidParam = new SqlParameter("@uid", SqlDbType.BigInt);
            uidParam.Value = uid;
            SqlParameter ffLogin = new SqlParameter("@ffLogin", SqlDbType.DateTime);
            ffLogin.Value = datetime;
            parameters.Add(uidParam);
            parameters.Add(ffLogin);


            // Create Key value pair
            KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(sql, parameters);

            // Place in list
            List< KeyValuePair<string, HashSet<SqlParameter>?> > sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { sqlStatement };

            //Execute
            int rowsAffected = _dao.ExecuteWriteOnly(sqlCommandList);

            //Validate
            try
            {
                if (rowsAffected == 0) { throw new Exception("SQLDB error, no rows affected"); }
                return createSuccessResponse(null);
            }
            catch (Exception ex)
            {
                return createErrorResponse(ex);
            }

        }
        public IResponse fetchFirstFailedLogin(long uid)
        {
            //Create SQL command and parameters
            SqlCommand sql = new SqlCommand();
            sql.CommandText = "SELECT firstFailedLogin FROM OTP WHERE UID = @uid";
            sql.Parameters.AddWithValue("@uid", uid);

            IResponse response = _dao.ExecuteReadOnly(sql);

            if (response.HasError)
            {
                return response;
            }
            else
            {
                try
                {
                    List<object> rowsReturned;

                    #region validate assign validate
                    if (response.ReturnValue is not null) { rowsReturned = (List<object>)response.ReturnValue; }
                    else { throw new Exception("NO Return Value from DB"); }

                    if (rowsReturned.Count == 0) { throw new Exception("No Rows Returned"); }
                    #endregion

                    //We are assuming there is only one row returned
                    object[] row = (object[])rowsReturned[0];

                    //We extract value from row
                    DateTime dateTimeofFirstFailLogin = (DateTime)row[0];

                    // Return the date time in a response object
                    return createSuccessResponse(dateTimeofFirstFailLogin);

                }
                catch (Exception ex)
                {
                    return createErrorResponse(ex);
                }
            }
            throw new NotImplementedException();
        }
    }
}
