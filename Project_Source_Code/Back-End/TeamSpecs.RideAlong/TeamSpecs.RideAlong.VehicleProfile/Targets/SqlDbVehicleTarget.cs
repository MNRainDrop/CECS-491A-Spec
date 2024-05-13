using MailKit.Search;
using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public class SqlDbVehicleTarget : ICRUDVehicleTarget, IGetVehicleCountTarget
{
    private readonly ISqlServerDAO _dao;

    public SqlDbVehicleTarget(ISqlServerDAO dao)
    {
        _dao = dao;
    }

    public IResponse ReadVehicleProfileSql(ICollection<object> searchParameters, int numOfResults, int page)
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
                    if (item is KeyValuePair<string, long>)
                    {
                        var searchItem = (KeyValuePair<string, long>) item;

                        whereSql += defaultWhereSql + searchItem.Key + " = @" + searchItem.Key + " ";
                        parameters.Add(new SqlParameter("@" + searchItem.Key, searchItem.Value));
                    }
                    else if (item is KeyValuePair<string, string>)
                    {
                        var searchItem = (KeyValuePair<string, string>) item;

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
            response.ErrorMessage = "Could not generate Vehicle Profile Retrieval Sql. ";
            return response;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteReadOnly(sqlCommands);
            response.ReturnValue = new List<object>();

            foreach (var item in daoValue)
            {
                response.ReturnValue.Add(new VehicleProfileModel(
                    vin: (string)item[0],
                    owner_UID:item[1] is System.DBNull ? null : (long)item[1],
                    licensePlate: (string)item[2],
                    make: (string)item[3],
                    model: (string)item[4],
                    year: (int)item[5])
                );
            }
            response.HasError = false;
        }
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = "Vehicle Profile Retrieval execution failed. " + ex;
        }
        return response;
    }

    public IResponse ReadVehicleProfileDetailsSql(ICollection<object> searchParameters)
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
            response.ErrorMessage = "Could not generate Vehicle Profile Details Retrieval Sql. ";
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
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = "Vehicle Profile Details Retrieval execution failed. " + ex;
        }
        return response;
    }

    public IResponse CreateVehicleProfileSql(IVehicleProfileModel vehicleProfile, IVehicleDetailsModel vehicleDetails)
    {
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();
        
        var vehicleSql = CreateVehicle(vehicleProfile);
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

        var vehicleDetailsSql = CreateVehicleDetails(vehicleDetails);
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
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = $"Vehicle Profile Creation Failed for {vehicleProfile.VIN}. " + ex;
            return response;
        }

        response.HasError = false;
        return response;
    }

    private IResponse CreateVehicle(IVehicleProfileModel vehicleProfile)
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
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = $"Could not generate Vehicle Profile Sql for {vehicleProfile.VIN}. " + ex;
            return response;
        }

        response.HasError = false;
        return response;
    }

    private IResponse CreateVehicleDetails(IVehicleDetailsModel vehicleDetails)
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
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = $"Could not generate Vehicle Profile Details Sql for {vehicleDetails.VIN} . " + ex;
            return response;
        }

        response.HasError = false;
        return response;
    }

    public IResponse ModifyVehicleProfileSql(IVehicleProfileModel vehicleProfile, IVehicleDetailsModel vehicleDetails)
    {
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        var vehicleSql = ModifyVehicle(vehicleProfile);
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

        var vehicleDetailsSql = ModifyVehicleDetails(vehicleDetails);
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
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = $"Database execution failed for {vehicleProfile.VIN}" + ex;
            return response;
        }

        response.HasError = false;
        return response;
    }

    private IResponse ModifyVehicle(IVehicleProfileModel vehicleProfile)
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
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = $"Could not generate Vehicle Profile Update Sql for {vehicleProfile.VIN}. " + ex;
            return response;
        }

        response.HasError = false;
        return response;
    }

    private IResponse ModifyVehicleDetails(IVehicleDetailsModel vehicleDetails)
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
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = $"Could not generate Vehicle Profile Details Update Sql for {vehicleDetails.VIN}" + ex;
            return response;
        }

        response.HasError = false;
        return response;
    }

    public IResponse UpdateVehicleOwnerSql(IVehicleProfileModel vehicleProfile, IVehicleDetailsModel vehicleDetails)
    {
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        var ownerSql = UpdateOwner(vehicleProfile);
        if (ownerSql.HasError == false)
        {
            if (ownerSql.ReturnValue is not null)
            {
                sqlCommands.Add((KeyValuePair<string, HashSet<SqlParameter>?>)ownerSql.ReturnValue.First());
            }
        }
        else
        {
            return ownerSql;
        }

        var vehicleDetailsSql = ModifyVehicleDetails(vehicleDetails);
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
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = $"Database execution failed for {vehicleProfile.VIN}" + ex;
            return response;
        }

        response.HasError = false;
        return response;
    }

    private IResponse UpdateOwner(IVehicleProfileModel vehicleProfile)
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
                    if (property.Name == "Owner_UID")
                    {
                        setSql += property.Name + "=@" + property.Name + ", ";
                        parameters.Add(new SqlParameter("@" + property.Name, property.GetValue(vehicleProfile)));
                    }
                    else if (property.Name == "VIN")
                    {
                        whereSql += property.Name + "=@" + property.Name + " ";
                        parameters.Add(new SqlParameter("@" + property.Name, property.GetValue(vehicleProfile)));
                    }

                }
            }
            setSql = setSql.Remove(setSql.Length - 2, 2);
            setSql += " ";

            var sqlString = commandSql + tableSql + setSql + whereSql;

            // Add string and hash set to list that the dao will execute
            response.ReturnValue.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = $"Could not generate Update Vehicle Owner Sql for {vehicleProfile.Owner_UID} and {vehicleProfile.VIN}. " + ex;
            return response;
        }

        response.HasError = false;
        return response;
    }

    public IResponse DeleteVehicleProfileSql(IVehicleProfileModel vehicleProfile, IAccountUserModel userAccount)
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

            whereSql += "VIN = @vin and Owner_UID = @uid";
            parameters.Add(new SqlParameter("@vin", vehicleProfile.VIN));
            parameters.Add(new SqlParameter("@uid", userAccount.UserId));
            
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
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = $"Could not delete vehicle from database for {vehicleProfile.VIN}. " + ex;
            return response;
        }

        response.HasError = false;
        return response;
    }

    public IResponse GetVehicleCount(IAccountUserModel userAccount)
    {
        #region Default sql setup
        var sqlString = "SELECT COUNT(*) FROM VehicleProfile WHERE Owner_UID = @UID";
        #endregion

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            // create new hash set of SqlParameters
            var parameters = new HashSet<SqlParameter>()
            {
                new SqlParameter("@UID", userAccount.UserId)
            };

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Vehicle count Sql. ";
            return response;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteReadOnly(sqlCommands);
            response.ReturnValue = new List<object>()
            {
                daoValue.First<object[]>()
            };

            response.HasError = false;
        }
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = "Vehicle Profile Retrieval execution failed. " + ex;
        }
        return response;
    }
}
