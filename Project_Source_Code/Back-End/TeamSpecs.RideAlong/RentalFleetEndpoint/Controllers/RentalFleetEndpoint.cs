using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TeamSpecs.RideAlong.RentalFleetLibrary.Interfaces;
using TeamSpecs.RideAlong.Model;

using TeamSpecs.RideAlong.RentalFleetLibrary.Models;
using Newtonsoft.Json;

namespace TeamSpecs.RideAlong.RentalFleetEndpoint.Controllers
{
    [Route("rentals/[controller]")]
    public class RentalFleetEndpoint : Controller
    {

        IRentalFleetService _service;
        public RentalFleetEndpoint(IRentalFleetService service)
        {
            _service = service;
        }
        [HttpPost]
        [Route("GetFleet")]
        public IActionResult GetFleet([FromBody] long uid)
        {

            if (uid < 0)
            {
                return BadRequest("No uid Provided");
            }
            IResponse response = _service.GetFleetFullModel(uid);
            if (response.ReturnValue is not null)
            {
                List<FleetFullModel> vehicles = (List<FleetFullModel>)response.ReturnValue;
                var jsonVehicles = JsonConvert.SerializeObject(vehicles);
                return Ok(jsonVehicles);
            }            
            return Ok("Did Not get JSON Values");
        }
    }
}
