using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamSpecs.RideAlong.RegistrationEntryPoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {

        private readonly ILogService _logService;
        private readonly IAccountCreationManager _accountCreationManager;
        private readonly ISecurityManager _securityManager;

        public RegistrationController(ILogService logService, ISecurityManager securityManager, IAccountCreationManager accountCreationManager)
        {
            _accountCreationManager = accountCreationManager;
            _logService = logService;
            _securityManager = securityManager;
        }

        [HttpPost]
        [Route("PostVerify")]
        public IActionResult PostVerifyUsername([FromBody]string email) 
        {
            IResponse response = new Response();

            response = _accountCreationManager.CallVerifyUser(email);

            // Database/ sql generation failed
            if(response.HasError && response.ErrorMessage is not null && response.ErrorMessage.Contains("Could not"))
            {
                return StatusCode(500);
            }

            // If returns bad --> user exists OR other issue
            if (response.HasError)
            {
                return BadRequest(response.ErrorMessage);
            }

            // If returns OK --> user updated in Db
            return Ok("User confirmation created successfully!");
        }

        [HttpPost]
        [Route("PostCreateUser")]
        public IActionResult PostAccountCreation(DateTime dateOfBirth, string altEmail, string email, string otp, string acccountType)
        {
            IResponse response = new Response();
            IProfileUserModel profile = new ProfileUserModel(dateOfBirth, altEmail);

            response = _accountCreationManager.RegisterUser(profile, email, otp, acccountType);

            // If DB or sql generation fails
            if (response.HasError && response.ErrorMessage is not null && response.ErrorMessage.Contains("Could not"))
            {
                return StatusCode(500);
            }

            // If returns bad --> user exists, business rule violation
            if (response.HasError)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok("Account created successfully.\nWelcome to RideAlong!"); 
        }

    }
}
