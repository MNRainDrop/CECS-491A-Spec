using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SecurityLibrary.Targets
{

    public class SQLServerAuthTarget : IAuthTarget
    {
        IGenericDAO _dao;
        public SQLServerAuthTarget(IGenericDAO dao)
        {
            _dao = dao;
        }
        public IResponse fetchUserModel(string username)
        {
            // DELETE THIS WHEN SUCCESSFULLY IMPLEMENTED
            throw new NotImplementedException();
        }
        public IResponse savePass(long UID, string passHash)
        {
            // DefineSQL Command
            SqlCommand sql = new SqlCommand();

            // Create SQL insert/update statement
            // Create parameters for SQL statement
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
            // Execute SQL statement
            // Validate SQL response
            // Return error response, if error present
            // Return respone object with claims otherwise

            // DELETE THIS WHEN SUCCESSFULLY IMPLEMENTED
            throw new NotImplementedException();
        }

        public IResponse fetchAttempts(long UID)
        {
            // Create SELECT SQL statement and parameters
            // Execute SQL statement
            // Validate SQL response
            // Return error response, if error present
            // Return respone object with claims otherwise

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
