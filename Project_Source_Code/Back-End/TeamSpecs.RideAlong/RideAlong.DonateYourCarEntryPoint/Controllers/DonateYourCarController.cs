using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TeamSpecs.RideAlong.DonateYourCarLibrary.Interfaces;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

namespace TeamSpecs.RideAlong.DonateYourCarEntryPoint.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DonateYourCarController : Controller
    {
        private readonly ILogService _logService;
        private readonly IDonateYourCarManager _donateYourCarManager;
        private readonly ISecurityManager _securityManager;

        public DonateYourCarController(ILogService logService, IDonateYourCarManager donateYourCarManager, ISecurityManager securityManager)
        {
            _logService = logService;
            _donateYourCarManager = donateYourCarManager;
            _securityManager = securityManager;
        }

        [HttpPost]
        [Route("GetAuthStatus")]
        public IActionResult GetAuthStatus()
        {
            Dictionary<string, string> requiredClaims = new Dictionary<string, string>();
            requiredClaims.Add("canView", "charity");
            bool hasPermission;
            try
            {
                hasPermission = _securityManager.isAuthorize(requiredClaims);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            if (!hasPermission)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            return NoContent();
        }

        [HttpGet]
        [Route("RetrieveCharities")]
        public IActionResult GetCharities()
        {
            IResponse response;
            IAppPrincipal principal = _securityManager.JwtToPrincipal();
#pragma warning disable CS8604
            IAccountUserModel user = new AccountUserModel(principal.userIdentity.userName);
#pragma warning restore CS8604
            user.UserId = principal.userIdentity.UID;
            user.UserHash = principal.userIdentity.userHash;

            response = _donateYourCarManager.retrieveCharities(user);

#pragma warning disable CS8602
            if (response.HasError == true)
            {
                return BadRequest();
            }
            else if (response.HasError == false && response.ReturnValue.Count == 4)
            {
                var JsonCharity = JsonSerializer.Serialize(response.ReturnValue);

                return Ok(JsonCharity);
            }
            else
            {
                return Ok("No charities found");
            }
#pragma warning restore CS8602
        }

    }
}
