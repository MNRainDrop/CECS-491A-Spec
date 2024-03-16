using Microsoft.Data.SqlClient;
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
                    if (response.ReturnValue is not null)
                    {
                        // Retrieve data from the response
                        userData = (List<object>)response.ReturnValue;
                    }
                    else
                    {
                        throw new Exception("Database Response has null return value");
                    }

                    // Check if any data was returned
                    if (userData is not null && userData.Count == 0)
                    {
                        // If no data found, return error response
                        throw new Exception("User was not Found");
                    }

                    // Assuming only one row is returned for the given username
                    if (userData is null) { throw new Exception("User Data returned is null"); }
                    object[] userRow = (object[])userData[0];

                    // Create a UserModel object to store the retrieved data
                    IAuthUserModel userModel = new AuthUserModel
                    {
                        UID = (int)userRow[0],
                        userName = (string)userRow[1],
                        salt = (byte[])userRow[2],
                        userHash = (string)userRow[3]
                    };

                    // Return UserModel object as part of the response
                    response.ReturnValue = new List<object>() { userModel };

                    // Indicate success
                    response.HasError = false;
                    return response;
                }
                catch (Exception ex)
                {
                    // If an exception occurs during data retrieval or processing

                    IResponse errorResponse = new Response();
                    errorResponse.HasError = true;
                    errorResponse.ErrorMessage = "Error retrieving user data: " + ex.Message;
                    _logger.CreateLogAsync("Error", "Data Store", errorResponse.ErrorMessage, null);
                    return response;
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
                    // Validate we have a return value
                    if (response.ReturnValue is not null)
                    {
                        rowsReturned = (List<object>)response.ReturnValue;
                    }
                    else
                    {
                        throw new Exception("No Rows Returned");
                    }

                    // Check if we got any rows
                    if (rowsReturned.Count == 0)
                    {
                        throw new Exception("Pass/User Not found");
                    }

                    // We assume one row, so we only fetch that one row
                    object[] row = (object[])rowsReturned[0];
                    //Extract our passHash from the row
                    string passHash = (string)row[0];

                    //Pack it all up and ship it
                    IResponse successResponse = new Response();
                    successResponse.HasError = false;
                    successResponse.ReturnValue = new List<object>() { passHash };
                    return successResponse;

                }
                catch (Exception ex)
                {
                    IResponse errorResponse = new Response();
                    errorResponse.HasError = true;
                    errorResponse.ErrorMessage = "Error retrieving user data: " + ex.Message;
                    _logger.CreateLogAsync("Error", "Data Store", errorResponse.ErrorMessage, null);
                    return response;
                }
            }
        }

        public IResponse fetchAttempts(long UID)
        {
            // Create SELECT SQL statement and parameters
            SqlCommand sql = new SqlCommand();
            sql.CommandText = "SELECT attempts FROM ";
            // Execute SQL statement
            // Validate SQL response
            // Return error response, if error present
            // Return respone object with attempts

            throw new NotImplementedException();
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

        public IResponse getClaims(long UID)
        {
            // Create SQL statement and parameters
            // Execute SQL statement
            // Validate SQL response
            // Return error response, if error present
            // Return respone object with claims otherwise

            //The following is being commented out because it is useful, but I cannot use it yet
            /*
            if (response.HasError == true)
            {
                IResponse userClaimsResponse = new Response();
                userClaimsResponse.HasError = true;
                userClaimsResponse.ErrorMessage = response.ErrorMessage;
                return userClaimsResponse;
            }
            else
            {

                IDictionary<string, string> Claims = new Dictionary<string, string>();
                //Loop through all values, get all use claims
                //Generate the response
                //Return the response

            }
            */

            // DELETE THIS WHEN SUCCESSFULLY IMPLEMENTED
            throw new NotImplementedException();
        }
    }
}
