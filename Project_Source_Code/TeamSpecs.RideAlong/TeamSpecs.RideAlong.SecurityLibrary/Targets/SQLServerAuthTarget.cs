using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.DataAccess;
using Microsoft.IdentityModel.Tokens;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary.Model;

namespace TeamSpecs.RideAlong.SecurityLibrary.Targets
{
    
    public class SQLServerAuthTarget : IAuthTarget
    {
        IGenericDAO _dao;
        public SQLServerAuthTarget(IGenericDAO dao)
        {
            _dao = dao;
        }
        public IResponse fetchPass(long UID)
        {
            SqlCommand sql = new SqlCommand();
            sql.CommandText = "SELECT PassHash FROM OTP WHERE UID = @UID";
            sql.Parameters.Add(new SqlParameter("@UID", SqlDbType.BigInt) { Value = UID });
            IResponse response = _dao.ExecuteReadOnly(sql);
            if (response.HasError == false)
            {
                string passHash = (string)response.ReturnValue.First();
                Response userHashResponse = new Response();
                userHashResponse.HasError = false;
                if (!passHash.IsNullOrEmpty())
                {
                    if (userHashResponse.ReturnValue == null){ userHashResponse.ReturnValue = new List<object>(); }
                    userHashResponse.ReturnValue.Add(passHash);
                    return userHashResponse;
                }
                else
                {
                    userHashResponse.ErrorMessage = "Passhash Returned null";
                    return userHashResponse;
                }
            }
            else if (response.HasError == true)
            {
                IResponse userHashResponse = new Response();
                userHashResponse.ErrorMessage = response.ErrorMessage;
                return userHashResponse;
            }
            return new Response();
        }

        public IResponse fetchClaims(long UID)
        {
            SqlCommand sql = new SqlCommand();
            //Generate SQL statement
            sql.CommandText = "SELECT * FROM UserClaim WHERE UID = @UID";
            //populate sql statement
            sql.Parameters.Add(new SqlParameter("@UID", SqlDbType.BigInt) { Value = UID });
            //Execute
            IResponse response = _dao.ExecuteReadOnly(sql);
            //Retrieve values
            if(response.HasError == true)
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

            throw new NotImplementedException();
        }

        public IResponse storeHashedPass(long UID, string passHash)
        {
            //create Sql statement
            SqlCommand sql = new SqlCommand();
            sql.CommandText = "UPDATE OTP SET PassHash = @passHash WHERE UID = @uid";
            //Populate sql statement with new pass
            sql.Parameters.Add(new SqlParameter("@passHash", SqlDbType.VarChar) { Value = passHash});
            sql.Parameters.Add(new SqlParameter("@UID", SqlDbType.BigInt) { Value = UID });
            //Execute SQL Query 
            //validate response, check for successful upload
            //Return success/failure
            throw new NotImplementedException();
        }
        public IResponse fetchAuthAccountModel(string username)
        {
            //Create model and response that will be returned
            IAuthUserModel userModel = new AuthUserModel();
            IResponse finalResponse = new Response();

            //Get sql statement to retrieve claims
            SqlCommand sql = new SqlCommand();
            sql.CommandText = "SELECT a.UID, a.UserName, a.Salt, a.UserHash FROM UserAccount AS a WHERE a.UserName = @username";
            sql.Parameters.Add(new SqlParameter("@username", SqlDbType.VarChar) { Value = username });
            
            //Execute the sql command
            IResponse daoResponse = _dao.ExecuteReadOnly(sql);
            
            //Check for dao errors
            if(daoResponse.HasError == true) 
            { 
                finalResponse.ErrorMessage = daoResponse.ErrorMessage; 
                return finalResponse;
            }
            else
            {
                //retrieve data from response
                //validate data is not null
                //package data into AuthAccountModel
                //package model into Response
                //return response
            }

            throw new NotImplementedException();
        }
    }
}

//fetchAuthAccountModelSQLStatement
/*
SELECT a.UID, a.UserName, a.Salt, a.UserHash
FROM UserAccount AS a
WHERE a.UserName == @username
 */

//storeHashedPass
/*
UPDATE OTP
SET PassHash = @passHash
WHERE UID = @uid
 */