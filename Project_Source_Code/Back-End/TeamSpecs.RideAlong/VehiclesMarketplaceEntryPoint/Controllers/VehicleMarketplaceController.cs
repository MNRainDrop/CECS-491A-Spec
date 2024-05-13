using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.CoEsLibrary.Interfaces;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.VehicleMarketplace;

namespace TeamSpecs.RideAlong.VehiclesMarketplaceEntryPoint.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleMarketplaceController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly IVehicleMarketplaceManager _manager;
        private readonly ISecurityManager _securityManager;
        private readonly ICommEstaManager _commEstaManager;


        public VehicleMarketplaceController(ILogService logService, IVehicleMarketplaceManager marketplaceManager, ISecurityManager securityManager, ICommEstaManager commEstaManager)
        {
            _logService = logService;
            _manager = marketplaceManager;
            _securityManager = securityManager;
            _commEstaManager = commEstaManager;
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




        [HttpPost]
        [Route("GetVehicleMarketplace")]
        public IActionResult GetAllVehiclesFromMarketplace([FromBody] int page)
        {
            IResponse response;
            /*IAppPrincipal principal = _securityManager.JwtToPrincipal();
#pragma warning disable CS8604 // Possible null reference argument.
            IAccountUserModel user = new AccountUserModel(principal.userIdentity.userName);
#pragma warning restore CS8604 // Possible null reference argument.
            user.UserId = principal.userIdentity.UID;
            user.UserHash = principal.userIdentity.userHash;*/

            response = _manager.RetrieveAllPublicPost(page);

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
        [Route("VehicleMarketplacePostDetailRetrieval")]
        public IActionResult VehicleMarketplacePostDetailRetrieval([FromBody] string VIN)

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
                response = _manager.RetrieveDetailVehicleProfile(VIN);
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

        [HttpPost]
        [Route("SendBuyRequest")]
        public IActionResult SendBuyRequest([FromBody] BuyRequest request1)

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
                response = _manager.SendBuyRequest(request1.UID, request1.VIN, request1.price);
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


        [HttpPost]
        [Route("VehicleMarketplacePostCreation")]
        public IActionResult VehicleMarketplacePostCreation([FromBody] RequestDataMarket request1)

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
                response = _manager.CreateVehicleProfilePost(request1.vehicleProfile.VIN, request1.status.View, request1.status.Description, request1.status.MarketplaceStatus);
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


        [HttpPost]
        [Route("VehicleMarketplacePostDeletion")]
        public IActionResult VehicleMarketplacePostDeletion([FromBody] RequestDataMarket request1)

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
                response = _manager.DeletePostFromMarketplace(request1.vehicleProfile.VIN);
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

        [HttpPost]
        [Route("CommunicationEstablishment")]
        public IActionResult GetSeller(string vin)
        {
            IResponse response;
            IAppPrincipal principal = _securityManager.JwtToPrincipal();
#pragma warning disable CS8604 // Possible null reference argument.
            IAccountUserModel user = new AccountUserModel(principal.userIdentity.userName);
#pragma warning restore CS8604 // Possible null reference argument.
            user.UserId = principal.userIdentity.UID;
            user.UserHash = principal.userIdentity.userHash;
            try
            {
                response = _commEstaManager.GetSeller(user, vin);
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

        public class BuyRequest
        {
            public required long UID { get; set; }

            public required string VIN { get; set; }

            public required int price { get; set; }

        }


        public class RequestDataMarket
        {
            public required JavaScriptVehicle vehicleProfile { get; set; }
            public required JavaScriptDetails vehicleDetails { get; set; }
            public required JavaScriptMarketplaceStatus status { get; set; }


        }
        public class JavaScriptVehicle
        {
            public required string VIN { get; set; }
            public required string LicensePlate { get; set; }
            public required string Make { get; set; }
            public required string Model { get; set; }
            public required int Year { get; set; }
        }

        public class JavaScriptMarketplaceStatus
        {
            public required int View { get; set; }

            public required int MarketplaceStatus { get; set; }

            public required string Description { get; set; }

        }
        public class JavaScriptDetails
        {
            public required string VIN { get; set; }
            public required string Color { get; set; }
            public required string Description { get; set; }
        }
    }
}
