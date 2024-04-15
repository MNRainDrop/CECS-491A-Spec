using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.CarHealthRatingLibrary.Interfaces;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.Model;

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
        [Route("RetrieveProfiles")]
        public IActionResult GetVehicleProfiles(IAccountUserModel user)
        {
            IResponse response;

            /*
            #region Security Check
            Dictionary<string, string> requiredClaims = new Dictionary<string, string>();
            if(!_securityManager.isAuthorize(requiredClaims))
            {
                _logService.CreateLogAsync("Info", "Data", " |Insert User here| is unauthorized to access |Insert claim here|", USERHASH)

                return Unauthorized();
            }
            #endregion
            */

            response = _manager.RetrieveValidVehicleProfiles(user);

            if(response.HasError == true)
            {
                return BadRequest(response.ErrorMessage);
            }
            else if(response.HasError == false && response.ReturnValue.Count >= 1)
            {
                return Ok(response.ReturnValue);
            }
            else
            {
                return Ok("No Vehicle Profiles found.");
            }

        }

    }
}
