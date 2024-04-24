﻿using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.VehicleMarketplace;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.Model;
using System.Text.Json;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

namespace TeamSpecs.RideAlong.VehiclesMarketplaceEntryPoint.Controllers
{
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
    }
}