using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.InventoryManagement;

public class SqlDbVendorVehicleTarget : IRetrieveVendorVehicleTarget, IModifyVendorVehicleTarget
{
    private readonly ISqlServerDAO _dao;

    public SqlDbVendorVehicleTarget(ISqlServerDAO dao)
    {
        _dao = dao;
    }
    public IResponse readVendorVehicleProfilesSql(ICollection<object> searchParameters, int numOfResults, int page)
    {
        #region Default sql setup
        var commandSql = "SELECT vp.VIN, Owner_UID, LicensePlate, Make, Model, Year, Color, Status, PostingDate, Price, PriceDate, Inquiries ";
        var fromSql = "FROM VehicleProfile as vp ";
        var whereSql = "WHERE ";
        var joinSql = "LEFT JOIN VendingStatus as vs ON vs.VIN = vp.VIN LEFT JOIN VehicleDetails as vd ON vd.VIN = vp.VIN ";
        var orderBySql = "ORDER BY DateCreated ";
        var offsetSql = $"OFFSET {(page - 1) * numOfResults} ROWS ";
        var fetchSql = $"FETCH NEXT {numOfResults} ROWS ONLY;";

        #endregion

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            // create new hash set of SqlParameters
            var parameters = new HashSet<SqlParameter>();
            var sqlString = "";

            if (searchParameters is not null)
            {
                foreach (var item in searchParameters)
                {
                    if (item is KeyValuePair<string, long>)
                    {
                        var searchItem = (KeyValuePair<string, long>)item;

                        whereSql += searchItem.Key + " = @" + searchItem.Key + " AND ";
                        parameters.Add(new SqlParameter("@" + searchItem.Key, searchItem.Value));
                    }
                    else if (item is KeyValuePair<string, DateTime[]>)
                    {
                        var searchItem = (KeyValuePair<string, DateTime[]>)item;

                        whereSql += searchItem.Key + " BETWEEN @value1 AND @value2 AND ";
                        parameters.Add(new SqlParameter("@value1", searchItem.Value[0]));
                        parameters.Add(new SqlParameter("@value2", searchItem.Value[1]));
                    }
                    else if (item is KeyValuePair<string, string>)
                    {
                        var searchItem = (KeyValuePair<string, string>)item;

                        whereSql += searchItem.Key + " = @" + searchItem.Key + " AND ";
                        parameters.Add(new SqlParameter("@" + searchItem.Key, searchItem.Value));
                    }
                }
                whereSql = whereSql.Remove(whereSql.Length - 4);
                sqlString = commandSql + fromSql + joinSql + whereSql + orderBySql + offsetSql + fetchSql;
            }
            else
            {
                sqlString = commandSql + fromSql + joinSql + orderBySql + offsetSql + fetchSql;
            }
            

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Vendor Vehicle Profile Retrieval Sql.";
            return response;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteReadOnly(sqlCommands);
            response.ReturnValue = new List<object>();

            foreach (var item in daoValue)
            {
                response.ReturnValue.Add(new VendorVehicleModel(vin: (string)item[0], owner_UID: (long)item[1], licensePlate: (string)item[2], make: (string)item[3], model: (string)item[4], year: (int)item[5], status: (string)item[7], postingDate: (DateTime)item[8], price: (decimal)item[9], priceDate: (DateTime)item[10], inquiries: (int)item[11], color: (string)item[6]));
            }
            response.HasError = false;
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Vendor Vehicle Profile Retrieval execution failed.";
        }
        return response;
    }

    public IResponse modifyVendorVehicleProfileSql(IVendorVehicleModel vehicle, IAccountUserModel userModel)
    {
        throw new NotImplementedException();
    }
}
