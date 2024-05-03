using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.VehicleMarketplace;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.Model;
using System.Text.Json;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

namespace TeamSpecs.RideAlong.VehiclesMarketplaceEntryPoint.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleMarketplaceController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly IVehicleMarketplaceManager _manager;
        private readonly ISecurityManager _securityManager;


        public VehicleMarketplaceController(ILogService logService, IVehicleMarketplaceManager marketplaceManager, ISecurityManager securityManager)
        {
            _logService = logService;
            _manager = marketplaceManager;
            _securityManager = securityManager;
        }

        [HttpPost]
        [Route("GetAuthStatus")]
        public IActionResult GetAuthStatus()
        {
            Dictionary<string, string> requiredClaims = new Dictionary<string, string>();
            requiredClaims.Add("canView", "marketplace");

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

        


        [HttpGet]
        [Route("GetVehicleMarketplace")]
        public IActionResult GetAllVehiclesFromMarketplace()
        {
            IResponse response;
            /*IAppPrincipal principal = _securityManager.JwtToPrincipal();
#pragma warning disable CS8604 // Possible null reference argument.
            IAccountUserModel user = new AccountUserModel(principal.userIdentity.userName);
#pragma warning restore CS8604 // Possible null reference argument.
            user.UserId = principal.userIdentity.UID;
            user.UserHash = principal.userIdentity.userHash;*/

            response = _manager.RetrieveAllPublicPost();

            if (response.HasError == false)
            {
                //var jsonCarHealthRank = JsonSerializer.Serialize(response.ReturnValue);

                return Ok(response.ReturnValue);
            }
            else if (response.HasError == true && response.ErrorMessage == "Error")
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }

        }


        [HttpPost]
        [Route("VehicleMarketplacePostCreation")]
        public IActionResult VehicleMarketplacePostCreation([FromBody] RequestData requestData)
        {
            IResponse response;
            /*IAppPrincipal principal = _securityManager.JwtToPrincipal();
#pragma warning disable CS8604 // Possible null reference argument.
            IAccountUserModel user = new AccountUserModel(principal.userIdentity.userName);
#pragma warning restore CS8604 // Possible null reference argument.
            user.UserId = principal.userIdentity.UID;
            user.UserHash = principal.userIdentity.userHash;*/
            try
            {
                response = _manager.CreateVehicleProfilePost(requestData.vehicleProfile.VIN, 1, requestData.vehicleProfile.Make, 1);
                if (response is not null)
                {
                    if (response.HasError)
                    {
                        return BadRequest(response.ErrorMessage);
                    }
                    else
                    {
                        return Ok(response.ReturnValue);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }



        public class RequestData
        {
            public required JavaScriptVehicle vehicleProfile { get; set; }
            public required JavaScriptDetails vehicleDetails { get; set; }

        }
        public class JavaScriptVehicle
        {
            public required string VIN { get; set; }
            public required string LicensePlate { get; set; }
            public required string Make { get; set; }
            public required string Model { get; set; }
            public required int Year { get; set; }
        }

        public class JavaScriptDetails
        {
            public required string VIN { get; set; }
            public required string Color { get; set; }
            public required string Description { get; set; }
        }
    }
}
