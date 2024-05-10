using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.RentalFleetLibrary.Interfaces;
using TeamSpecs.RideAlong.RentalFleetLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using MailKit.Search;

namespace TeamSpecs.RideAlong.RentalFleetLibrary
{
    public class SqlServerRentalFleetTarget : IRentalFleetTarget
    {
        ISqlServerDAO _dao;
        ILogService _logger;
        public SqlServerRentalFleetTarget(ISqlServerDAO dao, ILogService logger)
        {
            _dao = dao;
            _logger = logger;
        }
        /// <summary>
        /// Utility function used to avoid repeated code
        /// Simply takes in an exception as a parameter, 
        /// then generates both a log and a tailored response object to the error recorded in the exeption
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>Response object, with error message based on exception</returns>
        private IResponse createErrorResponse(Exception ex)
        {
            IResponse errorResponse = new Response();
            errorResponse.HasError = true;
            errorResponse.ErrorMessage = "Error retrieving user data: " + ex.Message;
            _logger.CreateLogAsync("Error", "Data Store", errorResponse.ErrorMessage, null);
            return errorResponse;
        }
        /// <summary>
        /// Utility function used to avoid repeated code
        /// Simply takes in an object as a parameter
        /// then generates a response to encapsulate it
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Error free Response Object with whatever you put in</returns>
        private IResponse createSuccessResponse(object? item)
        {
            // Pack it up and ship it
            IResponse successResponse = new Response();
            successResponse.HasError = false;
            if (item is not null)
            {
                successResponse.ReturnValue = new List<object>() { item };
            }
            return successResponse;
        }
        public IResponse fetchFleetModels(long uid)
        {
            IResponse fleetResponse = new Response();

            SqlCommand sql = new SqlCommand();
            sql.CommandText = "SELECT vp.VIN, vp.Make, vp.Model, vp.Year, vp.DateCreated, vd.Color, fm.Status, fm.StatusInfo, fm.expectedReturnDate " +
                "FROM VehicleProfile as vp " +
                "RIGHT JOIN FleetManagement as fm on vp.VIN = fm.VIN " +
                "RIGHT JOIN VehicleDetails as vd on vd.VIN = vp.VIN " +
                "WHERE vp.Owner_UID = @uid";
            sql.Parameters.Add(new SqlParameter("@uid", uid));
            IResponse response = _dao.ExecuteReadOnly(sql);
            if (response.HasError == true)
            {
                return response;
            }
            else
            {
                try
                {
                    List<object> fleetData;
                    #region Input validation and assignment
                    if (response.ReturnValue is not null)
                    {
                        // Retrieve data from the response
                        fleetData = (List<object>)response.ReturnValue;
                    }
                    else { throw new Exception("Database Response has null return value"); }

                    // Check if any data was returned
                    if (fleetData is not null && fleetData.Count == 0) { throw new Exception("User was not Found"); }

                    if (fleetData is null) { throw new Exception("User Data returned is null"); }
                    #endregion
                    List<object> listOfModels = new List<object>();
                    foreach (object[] vehicle in fleetData)
                    {
                        FleetFullModel tempModel = new FleetFullModel((string)vehicle[0], (string)vehicle[1], (string)vehicle[2], (int)vehicle[3], (DateTime)vehicle[4], (string)vehicle[5], (int)vehicle[6], (string)vehicle[7], (DateTime)vehicle[8]);
                        listOfModels.Add(tempModel);
                    }

                    IResponse outResonse = new Response();
                    outResonse.HasError = false;
                    outResonse.ReturnValue = listOfModels;
                    return outResonse;
                }
                catch (Exception ex)
                {
                    // If an exception occurs during data retrieval or processing

                    return createErrorResponse(ex);
                }
            }

        }

        public IResponse saveRentalFleetStatus(List<FleetInfoModel> fleetInfoModels)
        {
            //Add sql statement to a collection
            List<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommandList = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
            foreach (FleetInfoModel model in fleetInfoModels)
            {
                // Create SQL statment
                string commandText = "MERGE INTO FleetManagement AS Target " +
                    "USING (VALUES ('@vin', @status, @statusInfo, @erDate)) " +
                    "AS Source (Vin, Status, StatusInfo, expectedReturnDate) " +
                    "ON Target.Vin = Source.Vin" +
                    "WHEN MATCHED THEN" +
                    "    UPDATE SET Status = Source.Status, StatusInfo = Source.StatusInfo, expectedReturnDate = Source.expectedReturnDate" +
                    "WHEN NOT MATCHED THEN" +
                    "    INSERT (Vin, Status, StatusInfo, expectedReturnDate)" +
                    "    VALUES (Source.Vin, Source.Status, Source.StatusInfo, Source.expectedReturnDate);";

                //Create Parameters
                HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();
                SqlParameter vinParam = new SqlParameter();
                vinParam.Value = model.vin;
                SqlParameter statusParam = new SqlParameter();
                statusParam.Value = model.status;
                SqlParameter sInfoParam = new SqlParameter();
                sInfoParam.Value = model.statusInfo;
                SqlParameter erDateParam = new SqlParameter();
                erDateParam.Value = model.expectedReturnDate;
                parameters.Add(vinParam);
                parameters.Add(statusParam);
                parameters.Add(sInfoParam);
                parameters.Add(erDateParam);

                //Create Key value pair with sql and parameters
                KeyValuePair<string, HashSet<SqlParameter>?> sqlStatement = new KeyValuePair<string, HashSet<SqlParameter>?>(commandText, parameters);

                sqlCommandList.Add(sqlStatement);
            }

            // Attempt SQL Execution
            int rowsAffected = _dao.ExecuteWriteOnly(sqlCommandList);

            // Validate SQL Response
            try
            {
                if (rowsAffected == 0) { throw new Exception("SQLDB error, no rows affected"); }
                return createSuccessResponse(null);
            }
            catch (Exception ex)
            {
                return createErrorResponse(ex);
            }

        }
    }
}
