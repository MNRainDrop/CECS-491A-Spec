using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.Intrinsics.X86;
using TeamSpecs.RideAlong.CarHealthRatingLibrary.Interfaces;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CarHealthRatingLibrary;

public class SqlDBCarHealthRatingTarget: ISqlDbCarHealthRatingTarget
{
    private readonly IGenericDAO _dao;
    public SqlDBCarHealthRatingTarget(IGenericDAO dao)
    {  
        _dao = dao; 
    }

    public IResponse ReadValidVehicleProfiles(long userID)
    {
        #region SQL Setup
        string sqlQuery = @"SELECT VIN, Make, Model, Year
                            FROM VehicleProfile
                            WHERE Owner_UID = @UserID 
                            ORDER BY DateCreated";
        #endregion

        var sqlCommand = new SqlCommand(sqlQuery);
        IResponse response = new Response();

        // Add parameter to Sql Command
        try 
        {
            sqlCommand.Parameters.AddWithValue("@UserID", userID);
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Vehicle Profile retrieval Sql.";
            return response;
        }

        // DAO executes command
        try 
        {
            response = _dao.ExecuteReadOnly(sqlCommand);

        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Unable to retrieve Vehicle Profiles";
            return response;
        }

        response.HasError = false;
        return response;
    }

    public IResponse ReadServiceLogs(string vin) 
    {
        IResponse response = new Response();

        #region SQL Setup
        // TOP 11 selected in case of potential 5, 4, 1 edge CASE 
        string sqlQuery = @"SELECT TOP 10 Part, Date, Mileage
                            FROM ServiceLog
                            WHERE Category = 'Maintenance' 
                            AND VIN = @vin
                            AND Mileage IS NOT NULL
                            ORDER BY Date DESC";

        #endregion

        var sqlCommand = new SqlCommand(sqlQuery);

        try
        {
            sqlCommand.Parameters.AddWithValue("@vin", vin);
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Service Log retrieval Sql.";
            return response;
        }

        // DAO executes command
        try
        {
            response = _dao.ExecuteReadOnly(sqlCommand);

        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Unable to retrieve Service Logs from Database";
            return response;
        }

        response.HasError = false;
        return response;

    }

}

