using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TeamSpecs.RideAlong.RentalFleetLibrary.Interfaces;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.RentalFleetLibrary.Models;
using System.Text.Json;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

namespace TeamSpecs.RideAlong.RentalFleetEndpoint.Controllers
{
    [Route("[controller]")]
    public class RentalsController : Controller
    {

        IRentalFleetService _service;
        ISecurityManager _securityManager;
        public RentalsController(IRentalFleetService service, ISecurityManager securityManager)
        {
            _service = service;
            _securityManager = securityManager;
        }
        [HttpPost]
        [Route("GetFleet")]
        public IActionResult GetFleet()
        {
            IAppPrincipal principal = _securityManager.JwtToPrincipal();
            IResponse response = _service.GetFleetFullModel(principal.userIdentity.UID);
            if (response.ReturnValue is not null)
            {
                List<FleetFullModel> vehicles = (List<FleetFullModel>)response.ReturnValue;
                var jsonVehicles = JsonSerializer.Serialize(vehicles);
                return Ok(jsonVehicles);
            }            
            return Ok("No Json Values returned");
        }
        [HttpPost]
        [Route("GetAuthStatus")]
        public IActionResult GetAuthStatus()
        {
            Dictionary<string, string> requiredClaims = new Dictionary<string, string>();
            requiredClaims.Add("canView", "rental");
            bool hasPermission;
            try
            {
                hasPermission = _securityManager.isAuthorize(requiredClaims);
            } catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
            if (!hasPermission)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            return NoContent();
        }
    }
}
