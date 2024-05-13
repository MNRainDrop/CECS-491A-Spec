using Microsoft.Data.SqlClient;
using System.Data;
using TeamSpecs.RideAlong.CoEsLibrary.Interfaces;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CELibrary.Target
{
    public class SqlDbCETarget : ISqlDbCETarget
    {
        private readonly ISqlServerDAO _dao;

        public SqlDbCETarget(ISqlServerDAO dao)
        {
            _dao = dao;
        }

        public IResponse GetSellerSql(string vin)
        {

            var sqlCommandString = "";
            var response = new Response();
            sqlCommandString = @"
            SELECT UA.UserName 
            From VendingStatus VS 
            JOIN VehicleProfile VP ON VS.VIN = VP.VIN 
            JOIN UserAccount UA ON VP.Owner_UID = UA.UID 
            WHERE VS.VIN = {vin} ";

            var sqlCommand = new SqlCommand
            {
                CommandText = sqlCommandString,
                CommandType = CommandType.Text
            };

            try
            {

                var daoValue = _dao.ExecuteReadOnly(sqlCommand);
                response.ReturnValue = new List<object>();

                foreach (var item in daoValue)
                {
                    response.ReturnValue.Add(new EmailModel((string)item[0]));
                };
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = "GetSellerSql execute failed";
                return response;
            }
            response.HasError = false;
            return response;
        }

    }
}
