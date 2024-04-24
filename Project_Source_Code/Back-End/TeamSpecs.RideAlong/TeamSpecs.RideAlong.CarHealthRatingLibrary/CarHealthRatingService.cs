using Org.BouncyCastle.Asn1.X509;
using System.ComponentModel.DataAnnotations;
using TeamSpecs.RideAlong.CarHealthRatingLibrary.Interfaces;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Model.ServiceLogModel;

namespace TeamSpecs.RideAlong.CarHealthRatingLibrary
{
    /// <summary>
    /// Constants chosen as average mileage between checks for each category:
    /// Fluid, Mechinally wearing parts, and Non mechinally wearing parts
    /// </summary>
    public static class Globals
    {
        public const Int32 FLUID_MANTAINENCE_AVERAGE_MILEAGE = 4000;
        public const Int32 MECHINALLY_WEARING_PARTS_AVERAGE_MILEAGE = 6500;
        public const Int32 NON_MECHANICAL_WEARING_PARTS_AVERAGE_MILEAGE = 7000;
    }

    public class CarHealthRatingService: ICarHealthRatingService
    {
        private readonly ISqlDbCarHealthRatingTarget _target;
        private readonly ILogService _logService;

        public CarHealthRatingService(ISqlDbCarHealthRatingTarget target, ILogService logService)
        {
            _target = target;
            _logService = logService;
        }

        public IResponse ValidVehicleProfileRetrievalService(IAccountUserModel userAccount)
        {
            IResponse response = new Response();

            #region Validate Parameters
            if (userAccount is null)
            {
                throw new ArgumentNullException(nameof(userAccount));
            }
            if (string.IsNullOrWhiteSpace(userAccount.UserHash))
            {
                throw new ArgumentNullException(nameof(userAccount.UserHash));
            }
            if(userAccount.UserId < 0) 
            {
                throw new ArgumentOutOfRangeException(nameof(userAccount.UserId));
            }
            #endregion

            // Calling target
            response = _target.ReadValidVehicleProfiles(userAccount.UserId);

            #region logging to DB
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (response.HasError == true)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                _logService.CreateLogAsync( "Info", "Server", response.ErrorMessage, userAccount.UserHash);
#pragma warning restore CS8604 // Possible null reference argument.
                return response;
            }
            else if(response.HasError == false && response.ReturnValue.Count == 0)
            {
                _logService.CreateLogAsync("Info", "Server", "Successful retrieval of no Vehicle Profiles", userAccount.UserHash);
                return response;
            }
            else
            {
                _logService.CreateLogAsync("Info", "Server", "Successful retrieval of vehicle profile(s).", userAccount.UserHash);
                return response;
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            #endregion
        }

        public IResponse CalculateCarHealthRatingService(IAccountUserModel userAccount, string vin)
        {
            #region Variable Declartion
            IResponse response = new Response();

            // Define lists containing the parts for each category
            List<string> fluidMaintenanceParts = new List<string>
                {
                    "Oil", "Coolant", "Brake Fluid", "Power Steering Fluid", "Transmission Fluid"
                };

            List<string> mechanicallyWearingParts = new List<string>
                {
                    "Brake Pads/Rotors", "Tires", "Windshield Wipers", "Belts/Chains", "Air Filter", "Spark Plugs", "PCV Valve"
                };

            List<string> nonMechanicalMaintenanceParts = new List<string>
                {
                    "Tire Pressure", "Tire Alignment", "Tire Rotations", "Battery Health", "Inspection of Struts/Shocks"
                };

            // Initialize three lists to store Service Logs for each category
            List<IServiceLogModel> fluidServiceLogs = new List<IServiceLogModel>();
            List<IServiceLogModel> mechanicallyWearingServiceLogs = new List<IServiceLogModel>();
            List<IServiceLogModel> nonMechanicalServiceLogs = new List<IServiceLogModel>();

            var totalPoints = 0;
            var iterator = 0;
            IServiceLogModel tempServiceLogModel = new ServiceLogModel();
            List<string> category = new List<string>();
            List<int> pointsEarned = new List<int>();
            #endregion

            response = _target.ReadServiceLogs(vin);

            if(response.HasError == true)
            {
                return response;
            }

            // Check for 10 objects 
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (response.ReturnValue.Count == 0)
            {
                _logService.CreateLogAsync("Info", "Server", "Successful retrieval of no Service Logs", userAccount.UserHash);
                response.HasError = true;
                response.ErrorMessage = "User has no service logs on the vehicle " + vin;
                response.ReturnValue = null;
                return response;
            }
            else if (response.ReturnValue.Count < 10)
            {
                _logService.CreateLogAsync("Info", "Server", "Successful retrieval of Service Logs", userAccount.UserHash);
                response.HasError = true;
                response.ErrorMessage = "User has less than 10 service logs on the vehicle " + vin;
                response.ReturnValue = null;
                return response;
            }
            else
            {
                // Iterate over each Service Log in the collection
                foreach (var serviceLogObject in response.ReturnValue)
                {

                    if (serviceLogObject is object[] array)
                    {
                        var currentPart = array[0].ToString();
#pragma warning disable CS8604 // Possible null reference argument.
                        var currentDate = DateTime.Parse(array[1].ToString());
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning disable CS8604 // Possible null reference argument.
                        var currentMileage = Int32.Parse(array[2].ToString());
#pragma warning restore CS8604 // Possible null reference argument.

#pragma warning disable CS8604 // Possible null reference argument.
                        IServiceLogModel currentServiceLog = new ServiceLogModel(currentPart, currentDate, currentMileage, vin);
#pragma warning restore CS8604 // Possible null reference argument.

                        // For edge case, if List/Category only has one item
                        if (iterator == 0)
                        {
                            tempServiceLogModel = currentServiceLog;
                        }

                        // Check if the Part property of the service log matches the desired value for each category
                        if (fluidMaintenanceParts.Contains(currentServiceLog.Part))
                        {
                            fluidServiceLogs.Add(currentServiceLog);
                        }
                        else if (mechanicallyWearingParts.Contains(currentServiceLog.Part))
                        {
                            mechanicallyWearingServiceLogs.Add(currentServiceLog);
                        }
                        else if (nonMechanicalMaintenanceParts.Contains(currentServiceLog.Part))
                        {
                            nonMechanicalServiceLogs.Add(currentServiceLog);
                        }

                        iterator++;
                    }
                }

                // Add most recent Service Log to give score based on their most recent mileage
                #region Checking for Lists with 1 Service Log
                if (fluidServiceLogs.Count == 1 )
                {
                    fluidServiceLogs.Add(tempServiceLogModel);
                }
                else if(mechanicallyWearingServiceLogs.Count == 1)
                {
                    mechanicallyWearingServiceLogs.Add(tempServiceLogModel);
                }
                else if(nonMechanicalServiceLogs.Count == 1)
                {
                    nonMechanicalServiceLogs.Add(tempServiceLogModel);
                }
                #endregion

                #region Calculating CHR Rank
                for (int i = 0; i < fluidServiceLogs.Count - 1; i++) 
                {
                    var totalMileage = fluidServiceLogs[i].Mileage - fluidServiceLogs[i + 1].Mileage;

                    totalPoints += 6;

                    category.Add("Fluid");
                    
                    if( totalMileage < (Globals.FLUID_MANTAINENCE_AVERAGE_MILEAGE - (int)(Globals.FLUID_MANTAINENCE_AVERAGE_MILEAGE * .10)))
                    {
                        // assign max points
                        pointsEarned.Add(6);
                    }
                    else if(totalMileage >= (Globals.FLUID_MANTAINENCE_AVERAGE_MILEAGE - (int)(Globals.FLUID_MANTAINENCE_AVERAGE_MILEAGE * .10)) 
                        && totalMileage <= (Globals.FLUID_MANTAINENCE_AVERAGE_MILEAGE + (int)(Globals.FLUID_MANTAINENCE_AVERAGE_MILEAGE * .10)))
                    {
                        // assign 5 points
                        pointsEarned.Add(5);
                    }
                    else if(totalMileage > (Globals.FLUID_MANTAINENCE_AVERAGE_MILEAGE + (int)(Globals.FLUID_MANTAINENCE_AVERAGE_MILEAGE * .50)))
                    {
                        pointsEarned.Add(1);
                    }
                    else
                    {
                        pointsEarned.Add(3);
                    }

                }

                for (int i = 0; i < mechanicallyWearingServiceLogs.Count - 1; i++)
                {
                    var totalMileage = mechanicallyWearingServiceLogs[i].Mileage - mechanicallyWearingServiceLogs[i + 1].Mileage;

                    totalPoints += 12;

                    category.Add("Mechanically Wearing Parts");

                    if (totalMileage < (Globals.MECHINALLY_WEARING_PARTS_AVERAGE_MILEAGE - (int)(Globals.MECHINALLY_WEARING_PARTS_AVERAGE_MILEAGE * .10)))
                    {
                        pointsEarned.Add(12);
                    }
                    else if (totalMileage >= (Globals.MECHINALLY_WEARING_PARTS_AVERAGE_MILEAGE - (int)(Globals.MECHINALLY_WEARING_PARTS_AVERAGE_MILEAGE * .10)) &&
                             totalMileage <= (Globals.MECHINALLY_WEARING_PARTS_AVERAGE_MILEAGE + (int)(Globals.MECHINALLY_WEARING_PARTS_AVERAGE_MILEAGE * .10)))
                    {
                        pointsEarned.Add(10);
                    }
                    else if (totalMileage > (Globals.MECHINALLY_WEARING_PARTS_AVERAGE_MILEAGE + (int)(Globals.MECHINALLY_WEARING_PARTS_AVERAGE_MILEAGE * .50)))
                    {
                        pointsEarned.Add(2);
                    }
                    else
                    {
                        pointsEarned.Add(6);
                    }
                }

                for (int i = 0; i < nonMechanicalServiceLogs.Count - 1; i++)
                {
                    var totalMileage = nonMechanicalServiceLogs[i].Mileage - nonMechanicalServiceLogs[i + 1].Mileage;

                    totalPoints += 6;

                    category.Add("Non-Mechanical Maintenance");

                    if (totalMileage < (Globals.NON_MECHANICAL_WEARING_PARTS_AVERAGE_MILEAGE - (int)(Globals.NON_MECHANICAL_WEARING_PARTS_AVERAGE_MILEAGE * .10)))
                    {
                        pointsEarned.Add(6);
                    }
                    else if (totalMileage >= (Globals.NON_MECHANICAL_WEARING_PARTS_AVERAGE_MILEAGE - (int)(Globals.NON_MECHANICAL_WEARING_PARTS_AVERAGE_MILEAGE * .10)) &&
                             totalMileage <= (Globals.NON_MECHANICAL_WEARING_PARTS_AVERAGE_MILEAGE + (int)(Globals.NON_MECHANICAL_WEARING_PARTS_AVERAGE_MILEAGE * .10)))
                    {
                        pointsEarned.Add(5);
                    }
                    else if (totalMileage > (Globals.NON_MECHANICAL_WEARING_PARTS_AVERAGE_MILEAGE + (int)(Globals.NON_MECHANICAL_WEARING_PARTS_AVERAGE_MILEAGE * .50)))
                    {
                        pointsEarned.Add(1);
                    }
                    else
                    {
                        pointsEarned.Add(3);
                    }
                }
                #endregion

                #region Returns
                response.ReturnValue = new List<object>();
                response.ReturnValue.Add(pointsEarned);
                response.ReturnValue.Add(category);
                response.ReturnValue.Add(totalPoints);
                return response;
                #endregion
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

       
    }
}
