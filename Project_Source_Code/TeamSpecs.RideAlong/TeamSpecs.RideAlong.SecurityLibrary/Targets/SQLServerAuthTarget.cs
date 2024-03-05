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

        public IResponse getClaims(long UID)
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

        public IResponse saveHashedPass(long UID, string passHash)
        {
            throw new NotImplementedException();
        }
    }
}
