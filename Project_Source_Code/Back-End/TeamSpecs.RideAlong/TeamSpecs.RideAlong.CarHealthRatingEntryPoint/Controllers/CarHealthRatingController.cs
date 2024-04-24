using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.CarHealthRatingLibrary.Interfaces;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.Model;
using System.Text.Json;

// To note https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types?view=aspnetcore-8.0

namespace TeamSpecs.RideAlong.CarHealthRatingEntryPoint.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarHealthRatingController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly ICarHealthRatingManager _manager;
        private readonly ISecurityManager _securityManager;

        public CarHealthRatingController(ILogService logService, ICarHealthRatingManager carHealthManager, ISecurityManager securityManager)
        {
            _logService = logService;
            _manager = carHealthManager;
            _securityManager = securityManager;
        }

        [HttpPost]
        [Route("GetAuthStatus")]
        public IActionResult GetAuthStatus()
        {
            Dictionary<string, string> requiredClaims = new Dictionary<string, string>();
            requiredClaims.Add("ownsVehicle", "true");
            requiredClaims.Add("canRequestCarHealthRanking", "true");

            bool hasPermission;

            #region Security Check
            try
            {
                hasPermission = _securityManager.isAuthorize(requiredClaims);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            if (hasPermission == false)
            {

                return StatusCode(StatusCodes.Status403Forbidden, "Insufficient Permissions");
            }
            return NoContent();
            #endregion

        }

        [HttpPost]
        [Route("RetrieveProfiles")]
        public IActionResult GetVehicleProfiles()
        {
            IResponse response;
            IAppPrincipal principal = _securityManager.JwtToPrincipal();
#pragma warning disable CS8604 // Possible null reference argument.
            IAccountUserModel user = new AccountUserModel(principal.userIdentity.userName);
#pragma warning restore CS8604 // Possible null reference argument.
            user.UserId = principal.userIdentity.UID;
            user.UserHash = principal.userIdentity.userHash;


            response = _manager.RetrieveValidVehicleProfiles(user);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (response.HasError == true)
            {
                return BadRequest(response.ErrorMessage);
            }
            else if (response.HasError == false && response.ReturnValue.Count >= 1)
            {
                var jsonVehicles = JsonSerializer.Serialize(response.ReturnValue);

                return Ok(jsonVehicles);
            }
            else
            {
                return Ok("No Vehicle Profiles found.");
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        }

        [HttpPost]
        [Route("GetRanking")]
        public IActionResult GetCalculateCarHealthRating([FromBody] string vin) 
        { 
            IResponse response;
            IAppPrincipal principal = _securityManager.JwtToPrincipal();
#pragma warning disable CS8604 // Possible null reference argument.
            IAccountUserModel user = new AccountUserModel(principal.userIdentity.userName);
#pragma warning restore CS8604 // Possible null reference argument.
            user.UserId = principal.userIdentity.UID;
            user.UserHash = principal.userIdentity.userHash;

            response = _manager.CallCalculateCarHealthRating(user, vin);

            if(response.HasError == false)
            {
                //var jsonCarHealthRank = JsonSerializer.Serialize(response.ReturnValue);

                return Ok(response.ReturnValue);
            }
            else if(response.HasError == true && response.ErrorMessage == "Not enough Maintenance History")
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }

        }
        
    }
}