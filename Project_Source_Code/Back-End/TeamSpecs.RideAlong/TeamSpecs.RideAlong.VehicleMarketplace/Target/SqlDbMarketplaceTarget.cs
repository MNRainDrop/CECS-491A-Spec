using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.VehicleMarketplace;

namespace TeamSpecs.RideAlong.Services
{
    //This method generate SQL to adjust MarketplaceStatus table to upload (VPM-1)
    public class SqlDbMarketplaceTarget: IMarketplaceTarget
    {
        private readonly IGenericDAO _dao;

        public SqlDbMarketplaceTarget(IGenericDAO dao)
        {
            _dao = dao;
        }

        public IResponse RetrieveDetailVehicleProfileSql(string VIN)
        {
            #region Sql setup
            var commandSql = "Select * ";
            var fromSql = "From VehicleProfile ";
            var joinSql = "INNER JOIN MarketplaceStatus ON VehicleProfile.VIN = MarketplaceStatus.VIN ";
            var whereSql = "WHERE MarketplaceStatus.ViewStatus = 1 AND VehicleProfile.VIN = ";
            #endregion

            whereSql += "@VIN;";

            // Convert parameters into sql
            var parameters = new HashSet<SqlParameter>
            {
                new SqlParameter("@VIN", VIN)
            };


            var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
            var response = new Response();

            try
            {
                // create new hash set of SqlParameters
                var sqlString = commandSql + fromSql + joinSql + whereSql;
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
                    response.ReturnValue.Add(new VehicleMarketplaceDetailModel((string)item[0], (long)item[1], (string)item[2], (string)item[3], (string)item[4], (int)item[5], (DateTime)item[6], (int)item[8], (string)item[9], (int)item[10]));
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

        public IResponse UploadVehicleToMarketplace(string VIN, int view, string Description, int Status)
        {

            var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
            var response = new Response();
            #region Validate Arguments
            if (VIN is null)
            {
                throw new ArgumentNullException(VIN);
            }
            #endregion

            //Generating SQL query
            #region Default sql setup
            var commandSql = "INSERT INTO ";
            var tableSql = "MarketplaceStatus";
            var rowsSql = "(";
            var valuesSql = "VALUES (";
            #endregion

            #region Generating SQL
            //Extracting para to form query 
            rowsSql += "VIN,ViewStatus,MarketplaceDescription,MarketplaceStatus)";//HAS TO MATCH DB
            valuesSql += "@VIN, @view, @Description, @Status);";

            // Convert parameters into sql
            var parameters = new HashSet<SqlParameter> 
            {
                new SqlParameter("@VIN", VIN),
                new SqlParameter("@view", view),
                new SqlParameter("@Description", Description),
                new SqlParameter("@Status", Status)
            };

            //Combine into SQL command 
            var sqlString = commandSql + tableSql + rowsSql + valuesSql;

            // Add string and hash set to list that the dao will execute
            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
            #endregion

            #region DAO execute 
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
                //response.ErrorMessage = "AccountCreation execution failed";
                response.ErrorMessage = "Marketplace SQL Target giving error VIN:" + VIN + " Description: " + Description + " sqlCommand: "+sqlString;
                return response;
            }

            response.HasError = false;
            return response;
            #endregion



        }

        //This method generate SQL to adjust MarketplaceStatus table to delete (VPM-2)
        public IResponse DeleteVehicleFromMarketplace(string VIN)
        {
            var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
            var response = new Response();
            #region Validate Arguments
            if (VIN is null)
            {
                throw new ArgumentNullException(VIN);
            }
            #endregion

            //Generating SQL query
            #region Default sql setup
            var commandSql = "UPDATE ";
            var tableSql = "MarketplaceStatus";
            var rowsSql = " SET ";
            var valuesSql = " WHERE VIN = ";
            #endregion

            #region Generating SQL
            //Extracting para to form query 
            rowsSql += "ViewStatus = @view";//HAS TO MATCH DB
            valuesSql += "@VIN;";

            // Convert parameters into sql
            var parameters = new HashSet<SqlParameter>
            {
                new SqlParameter("@VIN", VIN),
                new SqlParameter("@view", (object)0),
                new SqlParameter("@Description", "Taken down from Marketplace"),
                new SqlParameter("@Status", (object)0)
            };

            //Combine into SQL command 
            var sqlString = commandSql + tableSql + rowsSql + valuesSql;

            // Add string and hash set to list that the dao will execute
            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
            #endregion

            #region DAO execute 
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
                response.ErrorMessage = "AccountCreation execution failed";
                return response;
            }

            response.HasError = false;
            return response;
            #endregion

        }

        //This method retrieve all public Vehicles (VPM-3)
        public IResponse ReadAllPublicVehicleProfileSql(int numOfResults, int page)
        {
            #region Sql setup
            var commandSql = "Select * ";
            var fromSql = "From VehicleProfile ";
            var joinSql = "INNER JOIN MarketplaceStatus ON VehicleProfile.VIN = MarketplaceStatus.VIN ";
            var whereSql = "WHERE MarketplaceStatus.ViewStatus = 1";
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
                var sqlString = commandSql + fromSql + joinSql + whereSql + orderBySql + offsetSql + fetchSql;
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
                    response.ReturnValue.Add(new VehicleProfileModel((string)item[0], (long)item[1], (string)item[2], (string)item[3], (string)item[4], (int)item[5]));
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

        //This method retrieve profile according manufacturer or car model (VPM-5)
        public IResponse SearchMarketplaceVehicleProfile(ICollection<object> searchParameters)
        {
            #region Default sql setup
            var commandSql = "Select * ";
            var fromSql = "From VehicleProfile ";
            var joinSql = "INNER JOIN MarketplaceStatus ON VehicleProfile.VIN = MarketplaceStatus.VIN";
            var whereSql = "WHERE MarketplaceStatus.ViewStatus = 1 AND";
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
                            searchItem = (KeyValuePair<string, long>)item;

                            whereSql += searchItem.Key + " = @" + searchItem.Key + " ";
                            parameters.Add(new SqlParameter("@" + searchItem.Key, searchItem.Value));
                        }
                    }
                }
                var sqlString = commandSql + fromSql + joinSql + whereSql;

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
                    response.ReturnValue.Add(new VehicleProfileModel((string)item[0], (long)item[1], (string)item[2], (string)item[3], (string)item[4], (int)item[5]));
                    
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

        //This method is for sedning buy request VPM-6
        public IResponse VehicleMarketplaceSendRequestService(string VIN)
        {

            #region Validate Arguments
            if (VIN is null)
            {
                throw new ArgumentNullException(nameof(VIN));
            }
            #endregion

            //Generating SQL query
            #region Default sql setup
            var commandSql = "SELECT TOP (1) UserName ";
            var tableSql = "FROM UserAccount ";
            var rowsSql = " INNER JOIN VehicleProfile ON VehicleProfile.Owner_UID = UserAccount.UID ";
            var valuesSql = " WHERE VehicleProfile.VIN = @VIN";
            #endregion
            var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
            var response = new Response();


            #region Generating SQL
 

            // Convert parameters into sql
            var parameters = new HashSet<SqlParameter>
            {
                new SqlParameter("@VIN", VIN),
            };

            //Combine into SQL command 
            var sqlString = commandSql + tableSql + rowsSql + valuesSql;

            // Add string and hash set to list that the dao will execute
            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
            #endregion


            // DAO Executes the command
            try
            {
                var daoValue = _dao.ExecuteReadOnly(sqlCommands);
                response.ReturnValue = new List<object>();

                foreach (var item in daoValue)
                {
                    response.ReturnValue.Add(item);

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

    }
}
