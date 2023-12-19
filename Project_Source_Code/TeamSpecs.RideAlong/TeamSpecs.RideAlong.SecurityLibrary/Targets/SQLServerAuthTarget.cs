using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.DataAccess;

namespace TeamSpecs.RideAlong.SecurityLibrary.Targets
{
    
    public class SQLServerAuthTarget : IAuthTarget
    {
        IGenericDAO _dao;
        SQLServerAuthTarget(IGenericDAO dao)
        {
            _dao = dao;
        }
        public IResponse fetchPass(long UID)
        {
            SqlCommand sql = new SqlCommand();
            sql.CommandText = "SELECT passHash FROM Pass WHERE Pass.UID = @UID";
            sql.Parameters.Add(new SqlParameter("@UID", SqlDbType.BigInt) { Value = UID });
            IResponse response = _dao.ExecuteReadOnly(sql);
            if (response.HasError == false) 
            {
                string passHash = (string)response.ReturnValue.First();
                Response userHashResponse = new Response();
                userHashResponse.HasError = false;
                userHashResponse.ReturnValue.Add(passHash);
                return userHashResponse;
            }
            return new Response();
        }

        public IResponse getClaims(long UID)
        {
            throw new NotImplementedException();
        }

        public IResponse saveHashedPass(long UID, string passHash)
        {
            throw new NotImplementedException();
        }
    }
}
