using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.CarHealthRatingLibrary.Interfaces;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CarHealthRatingLibrary
{
    public class CarHealthRatingManager: ICarHealthRatingManager
    {
        private readonly ICarHealthRatingService _carHealthService;
        private readonly ILogService _logService;
        public CarHealthRatingManager(ICarHealthRatingService carHealthService, ILogService logService) 
        { 
            _carHealthService = carHealthService;
            _logService = logService;
        }

        public IResponse RetrieveValidVehicleProfiles(IAccountUserModel user)
        {
            #region Validate Parameters
            if (user is null)
            {
                _logService.CreateLogAsync("Debug", "Data", "Null User Account Passed Into Vehicle Profile Retrieval Manager", null);
                throw new ArgumentNullException(nameof(user));
            }
            if (user.UserHash is null)
            {
                _logService.CreateLogAsync("Debug", "Data", "Null User Hash Passed Into Vehicle Profile Retrieval Manager", null);
                throw new ArgumentNullException(nameof(user.UserHash));
            }
            #endregion

            IResponse response = new Response();
            var timer = new Stopwatch();

            timer.Start();
            response = _carHealthService.ValidVehicleProfileRetrievalService(user);
            timer.Stop();

            
            #region Logging timer information
            if (timer.Elapsed.TotalSeconds > 3 && timer.Elapsed.TotalSeconds <= 10)
            {
                _logService.CreateLogAsync("Warning", "Server", response.ErrorMessage + "Retrieving Vehicle Profiles via Car Health Rating Service took longer than 3 seconds, but less than 10.", user.UserHash);
            }
            if (timer.Elapsed.TotalSeconds > 10)
            {
                _logService.CreateLogAsync("Error", "Server", response.ErrorMessage + "Server Timeout on Car Health Rating Vehicle Profile Retrieval Service.", user.UserHash);
            }
            #endregion

            #region Log the action to the database
            if (response.HasError)
            {
                response.ErrorMessage = "Could not retrieve vehicle profile details." + response.ErrorMessage;
            }
            else
            {
                response.ErrorMessage = "Successful retrieval of vehicle profile details.";
            }
            _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, user.UserHash);
            #endregion

            return response;
        }

        public IResponse CallCalculateCarHealthRating(IAccountUserModel user, string vin)
        {
            #region Validate Parameters
            if (user is null)
            {
                _logService.CreateLogAsync("Debug", "Data", "Null User Account Passed Into Vehicle Profile Retrieval Manager", null);
                throw new ArgumentNullException(nameof(user));
            }
            if (user.UserHash is null)
            {
                _logService.CreateLogAsync("Debug", "Data", "Null User Hash Passed Into Vehicle Profile Retrieval Manager", null);
                throw new ArgumentNullException(nameof(user.UserHash));
            }
            #endregion

            IResponse response = new Response();
            var timer = new Stopwatch();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            List<int> pointsEarned = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            string pointsEarnedString;
            int totalPoints = 0;

            timer.Start();
            response = _carHealthService.CalculateCarHealthRatingService(user, vin);
            timer.Stop();

            #region Logging timer information
            if (timer.Elapsed.TotalSeconds > 3 && timer.Elapsed.TotalSeconds <= 10)
            {
                _logService.CreateLogAsync("Warning", "Server", response.ErrorMessage + "Retrieving service log information via Car Health Rating Service took longer than 3 seconds, but less than 10.", user.UserHash);
            }
            if (timer.Elapsed.TotalSeconds > 10)
            {
                _logService.CreateLogAsync("Error", "Server", response.ErrorMessage + "Server Timeout on Car Health Rating serivce log retrieval.", user.UserHash);
            }
            #endregion

            if (response.HasError == true && (response.ErrorMessage == "User has no service logs on the vehicle " + vin || response.ErrorMessage == "User has less than 10 service logs on the vehicle " + vin))
            {
                _logService.CreateLogAsync("Info", "Business", "Not enough Maintenance History", user.UserHash);
                response.ErrorMessage = "Not enough Maintenance History";
            }
            else if (response.HasError == true) // Means that SQL generation/ DB  failed
            {
#pragma warning disable CS8604 // Possible null reference argument.
                _logService.CreateLogAsync("Error", "Server", response.ErrorMessage, user.UserHash);
#pragma warning restore CS8604 // Possible null reference argument.
            }
            else if( response.HasError == false)
            {
                // Find total points
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                foreach (var pointsEarnedObject in response.ReturnValue)
                {
                    if (pointsEarnedObject is List<int> pointsList)
                    {
                        pointsEarned = pointsList;
                    }
                    else if (pointsEarnedObject is int points)
                    {
                        // Extract totalPoints if the object is an integer
                        totalPoints = points;
                    }

                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

#pragma warning disable CS8604 // Possible null reference argument.
                pointsEarnedString = string.Join(", ", pointsEarned);
#pragma warning restore CS8604 // Possible null reference argument.

                _logService.CreateLogAsync("Info", "Business", "Score assigned operation successful" + user.UserHash + " " +  totalPoints, user.UserHash);
                _logService.CreateLogAsync("Info", "Business", "Total points: " + totalPoints + " .Points gain from each event: " + pointsEarnedString, user.UserHash);
            }

            return response;
        }
    }
}
