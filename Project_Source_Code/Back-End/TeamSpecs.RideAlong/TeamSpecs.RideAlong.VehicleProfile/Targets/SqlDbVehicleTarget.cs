using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public class SqlDbVehicleTarget : IRetrieveVehiclesTarget, IRetrieveVehicleDetailsTarget, ICreateVehicleTarget, IModifyVehicleTarget, IDeleteVehicleTarget
{
    private readonly IGenericDAO _dao;

    public SqlDbVehicleTarget(IGenericDAO dao)
    {
        _dao = dao;
    }

    public IResponse readVehicleProfileSql(ICollection<object> searchParameters, int numOfResults, int page)
    {
        #region Default sql setup
        var commandSql = "SELECT * ";
        var fromSql = "FROM VehicleProfile ";
        var defaultWhereSql = "WHERE ";
        var whereSql = "";
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

            
            if (searchParameters is not null)
            {
                foreach (var item in searchParameters)
                {
                    KeyValuePair<string, long> searchItem;
                    if (item is KeyValuePair<string, long>)
                    {
                        searchItem = (KeyValuePair<string, long>) item;

                        whereSql += defaultWhereSql + searchItem.Key + " = @" + searchItem.Key + " ";
                        parameters.Add(new SqlParameter("@" + searchItem.Key, searchItem.Value));
                    }
                }
            }
            var sqlString = commandSql + fromSql+ whereSql + orderBySql + offsetSql + fetchSql;

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Vehicle Profile Retrieval Sql.";
            return response;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteReadOnly(sqlCommands);
            response.ReturnValue = new List<object>();

            foreach (var item in daoValue)
            {
                response.ReturnValue.Add(new VehicleProfileModel((string)item[0], (long)item[1], (string)item[2], item[3].ToString(), item[4].ToString(), (int)item[5]));
            }
            response.HasError = false;
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Vehicle Profile Retrieval execution failed.";
        }
        return response;
    }

    public IResponse readVehicleProfileDetailsSql(ICollection<object> searchParameters)
    {
        #region Default sql setup
        var commandSql = "Select * ";
        var fromSql = "From VehicleDetails ";
        var defaultWhereSql = "Where ";
        var whereSql = "";
        #endregion

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            // create new hash set of SqlParameters
            var parameters = new HashSet<SqlParameter>();


            if (searchParameters is not null)
            {
                foreach (KeyValuePair<string, string> item in searchParameters)
                {
                    whereSql += defaultWhereSql + item.Key + " = @" + item.Key + " ";
                    parameters.Add(new SqlParameter("@" + item.Key, item.Value));
                }
            }
            var sqlString = commandSql + fromSql + whereSql;

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Vehicle Profile Details Retrieval Sql.";
            return response;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteReadOnly(sqlCommands);
            response.ReturnValue = new List<object>();

            foreach (var item in daoValue)
            {
                response.ReturnValue.Add(new VehicleDetailsModel((string)item[0], (string)item[1], (string)item[2]));
            }
            response.HasError = false;
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Vehicle Profile Details Retrieval execution failed.";
        }
        return response;
    }

    public IResponse createVehicleProfileSql(IVehicleProfileModel vehicleProfile, IVehicleDetailsModel vehicleDetails)
    {
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();
        
        var vehicleSql = createVehicle(vehicleProfile);
        if (vehicleSql.HasError == false)
        {
            if (vehicleSql.ReturnValue is not null)
            {
                sqlCommands.Add((KeyValuePair<string, HashSet<SqlParameter>?>)vehicleSql.ReturnValue.First());
            }
        }
        else
        {
            return vehicleSql;
        }

        var vehicleDetailsSql = createVehicleDetails(vehicleDetails);
        if (vehicleDetailsSql.HasError == false)
        {
            if (vehicleDetailsSql.ReturnValue is not null)
            {
                sqlCommands.Add((KeyValuePair<string, HashSet<SqlParameter>?>)vehicleDetailsSql.ReturnValue.First());
            }
        }
        else
        {
            return vehicleDetailsSql;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
            response.ReturnValue = new List<object>()
            {
                daoValue
            };
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Database execution failed";
            return response;
        }

        response.HasError = false;
        return response;
    }

    private IResponse createVehicle(IVehicleProfileModel vehicleProfile)
    {
        #region Default sql setup
        var commandSql = "INSERT INTO ";
        var tableSql = "VehicleProfile ";
        var rowsSql = "(";
        var valuesSql = "VALUES (";
        #endregion

        var response = new Response()
        {
            ReturnValue = new List<object>()
        };

        try
        {
            // create new hash set of SqlParameters
            var parameters = new HashSet<SqlParameter>();

            // convert VehicleProfile model to sql statement
            var configType = typeof(IVehicleProfileModel);
            var properties = configType.GetProperties();

            foreach (var property in properties)
            {
                if (property.GetValue(vehicleProfile) != null)
                {
                    rowsSql += property.Name + ",";
                    valuesSql += "@" + property.Name + ",";

                    parameters.Add(new SqlParameter("@" + property.Name, property.GetValue(vehicleProfile)));
                }

            }
            rowsSql = rowsSql.Remove(rowsSql.Length - 1, 1);
            valuesSql = valuesSql.Remove(valuesSql.Length - 1, 1);
            rowsSql += ") ";
            valuesSql += ");";

            var sqlString = commandSql + tableSql + rowsSql + valuesSql;

            // Add string and hash set to list that the dao will execute
            response.ReturnValue.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Vehicle Profile Sql";
            return response;
        }

        response.HasError = false;
        return response;
    }

    private IResponse createVehicleDetails(IVehicleDetailsModel vehicleDetails)
    {
        #region Default sql setup
        var commandSql = "INSERT INTO ";
        var tableSql = "VehicleDetails ";
        var rowsSql = "(";
        var valuesSql = "VALUES (";
        #endregion

        var response = new Response()
        {
            ReturnValue = new List<object>()
        };

        try
        {
            // create new hash set of SqlParameters
            var parameters = new HashSet<SqlParameter>();

            // convert vehicle profile model to sql statement
            var configType = typeof(IVehicleDetailsModel);
            var properties = configType.GetProperties();

            foreach (var property in properties)
            {
                if (property.GetValue(vehicleDetails) != null)
                {
                    rowsSql += property.Name + ",";
                    valuesSql += "@" + property.Name + ",";

                    parameters.Add(new SqlParameter("@" + property.Name, property.GetValue(vehicleDetails)));
                }

            }
            rowsSql = rowsSql.Remove(rowsSql.Length - 1, 1);
            valuesSql = valuesSql.Remove(valuesSql.Length - 1, 1);
            rowsSql += ") ";
            valuesSql += ");";

            var sqlString = commandSql + tableSql + rowsSql + valuesSql;

            // Add string and hash set to list that the dao will execute
            response.ReturnValue.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Vehicle Profile Details Sql";
            return response;
        }

        response.HasError = false;
        return response;
    }

    public IResponse modifyVehicleProfileSql(IVehicleProfileModel vehicleProfile, IVehicleDetailsModel vehicleDetails)
    {
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        var vehicleSql = modifyVehicle(vehicleProfile);
        if (vehicleSql.HasError == false)
        {
            if (vehicleSql.ReturnValue is not null)
            {
                sqlCommands.Add((KeyValuePair<string, HashSet<SqlParameter>?>)vehicleSql.ReturnValue.First());
            }
        }
        else
        {
            return vehicleSql;
        }

        var vehicleDetailsSql = modifyVehicleDetails(vehicleDetails);
        if (vehicleDetailsSql.HasError == false)
        {
            if (vehicleDetailsSql.ReturnValue is not null)
            {
                sqlCommands.Add((KeyValuePair<string, HashSet<SqlParameter>?>)vehicleDetailsSql.ReturnValue.First());
            }
        }
        else
        {
            return vehicleDetailsSql;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
            response.ReturnValue = new List<object>()
            {
                daoValue
            };
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Database execution failed";
            return response;
        }

        response.HasError = false;
        return response;
    }

    private IResponse modifyVehicle(IVehicleProfileModel vehicleProfile)
    {
        #region Default sql setup
        var commandSql = "UPDATE ";
        var tableSql = "VehicleProfile ";
        var setSql = "SET ";
        var whereSql = "WHERE ";
        #endregion

        var response = new Response()
        {
            ReturnValue = new List<object>()
        };

        try
        {
            // create new hash set of SqlParameters
            var parameters = new HashSet<SqlParameter>();

            // convert VehicleProfile model to sql statement
            var configType = typeof(IVehicleProfileModel);
            var properties = configType.GetProperties();

            foreach (var property in properties)
            {
                if (property.GetValue(vehicleProfile) != null)
                {
                    if (property.Name == "VIN")
                    {
                        whereSql += property.Name + "=@" + property.Name;
                        parameters.Add(new SqlParameter("@" + property.Name, property.GetValue(vehicleProfile)));
                    }
                    else if (property.Name != "Owner_UID")
                    {
                        setSql += property.Name + "=@" + property.Name + ",";
                        parameters.Add(new SqlParameter("@" + property.Name, property.GetValue(vehicleProfile)));
                    }
                    
                }
            }
            setSql = setSql.Remove(setSql.Length - 1, 1);
            setSql += " ";

            var sqlString = commandSql + tableSql + setSql + whereSql;

            // Add string and hash set to list that the dao will execute
            response.ReturnValue.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Vehicle Profile Update Sql";
            return response;
        }

        response.HasError = false;
        return response;
    }

    private IResponse modifyVehicleDetails(IVehicleDetailsModel vehicleDetails)
    {
        #region Default sql setup
        var commandSql = "UPDATE ";
        var tableSql = "VehicleDetails ";
        var setSql = "SET ";
        var whereSql = "WHERE ";
        #endregion

        var response = new Response()
        {
            ReturnValue = new List<object>()
        };

        try
        {
            // create new hash set of SqlParameters
            var parameters = new HashSet<SqlParameter>();

            // convert VehicleProfile model to sql statement
            var configType = typeof(IVehicleDetailsModel);
            var properties = configType.GetProperties();

            foreach (var property in properties)
            {
                if (property.GetValue(vehicleDetails) != null)
                {
                    if (property.Name == "VIN")
                    {
                        whereSql += property.Name + "=@" + property.Name;
                        parameters.Add(new SqlParameter("@" + property.Name, property.GetValue(vehicleDetails)));
                    }
                    else
                    {
                        setSql += property.Name + "=@" + property.Name + ",";
                        parameters.Add(new SqlParameter("@" + property.Name, property.GetValue(vehicleDetails)));
                    }

                }
            }
            setSql = setSql.Remove(setSql.Length - 1, 1);
            setSql += " ";
            var sqlString = commandSql + tableSql + setSql + whereSql;

            // Add string and hash set to list that the dao will execute
            response.ReturnValue.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Vehicle Profile Update Sql";
            return response;
        }

        response.HasError = false;
        return response;
    }

    public IResponse deleteVehicleProfileSql(IVehicleProfileModel vehicleProfile, IAccountUserModel userAccount)
    {
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        // DELETE FROM VehicleProfile WHERE Vin = @vin and Owner_UID = @uid
        #region Default sql setup
        var commandSql = "DELETE ";
        var tableSql = "FROM VehicleProfile ";
        var defaultWhereSql = "WHERE ";
        var whereSql = "";
        #endregion

        try
        {
            // create new hash set of SqlParameters
            var parameters = new HashSet<SqlParameter>();

            if (vehicleProfile is not null && userAccount is not null)
            {
                whereSql += "VIN = @vin and Owner_UID = @uid";
                parameters.Add(new SqlParameter("@vin", vehicleProfile.VIN));
                parameters.Add(new SqlParameter("@uid", userAccount.UserId));
            }
            
            var sqlString = commandSql + tableSql + defaultWhereSql + whereSql;

            // Add string and hash set to list that the dao will execute
            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Vehicle Profile deletion Sql. ";
            return response;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
            response.ReturnValue = new List<object>()
            {
                daoValue
            };
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Database execution failed";
            return response;
        }

        response.HasError = false;
        return response;
    }
}
