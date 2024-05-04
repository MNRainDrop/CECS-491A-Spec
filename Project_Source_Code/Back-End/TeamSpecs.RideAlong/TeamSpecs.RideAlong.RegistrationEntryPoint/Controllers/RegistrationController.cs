using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamSpecs.RideAlong.RegistrationEntryPoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {

        private readonly ILogService _logService;
        //private readonly ICarHealthRatingManager _manager;   put my manager here
        private readonly ISecurityManager _securityManager;

        public RegistrationController(ILogService logService, ISecurityManager securityManager)
        {
            _logService = logService;
            //_manager = carHealthManager;
            _securityManager = securityManager;
        }


        // GET: api/<RegistrationController>
        [HttpGet]
        [Route("GetVerify")]
        public IActionResult GetVerifyUsername(string email) 
        {
            // Will not have security check

            // call manager verify function

            // If returns OK --> user updated in Db

            // If returns bad --> user exists OR other issue

            return Ok();
        }

    }
}
