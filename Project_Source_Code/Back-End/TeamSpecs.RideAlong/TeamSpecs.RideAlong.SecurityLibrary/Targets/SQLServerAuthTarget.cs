﻿using Microsoft.Data.SqlClient;
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
        /// Simply takes in a value as a parameter
        /// then generates a response to encapsulate it
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Error free Response Object with whatever you put in</returns>
        private IResponse createSuccessResponse(object item)
        {
            // Pack it up and ship it
            IResponse successResponse = new Response();
            successResponse.HasError = false;
            successResponse.ReturnValue = new List<object>() { item };
            return successResponse;
        }
        public IResponse fetchUserModel(string username)
        {
            // Create SQL statement and parameters
            SqlCommand sql = new SqlCommand();
            sql.CommandText = "SELECT UID, UserName, Salt, UserHash FROM UserAccount WHERE UserName = @username";
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
                        UID = (int)userRow[0],
                        userName = (string)userRow[1],
                        salt = (byte[])userRow[2],
                        userHash = (string)userRow[3]
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
        public IResponse savePass(long UID, string passHash)
        {
            // Create SQL statement and parameters
            SqlCommand sql = new SqlCommand();

            // Attempt SQL Execution

            // Validate SQL return statement
            // Return error outcome if error
            // Return good response variable if success

            // DELETE THIS WHEN SUCCESSFULLY IMPLEMENTED
            throw new NotImplementedException();
        }
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
            // Create SQL command and parameters
            // Execute SQL statement
            // Validate SQL Response
            // Return Response with success outcome if successful
            // Return Response Object with failure outcome if not successful

            // DELETE THIS WHEN SUCCESSFULLY IMPLEMENTED
            throw new NotImplementedException();
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
            sql.CommandText = "SELECT ClaimID, ClaimScope FROM UserClaim WHERE UID = @uid";
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

                    Dictionary<string, string> claims = new Dictionary<string, string>();

                    foreach (object[] row in rowsReturned)
                    {

                        string claimID = (string)row[0];
                        string claimScope = (string)row[1];
                        if (claimID is not null && claimScope is not null)
                        {
                            claims.Add(claimID, claimScope);
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
    }
}
