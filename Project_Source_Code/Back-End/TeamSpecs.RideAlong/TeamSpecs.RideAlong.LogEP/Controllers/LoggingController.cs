using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

namespace TeamSpecs.RideAlong.LogEP.Controllers
{
    [Route("[controller]")]
    public class LoggingController : Controller
    {
        ISecurityManager _securityManager;
        ILogService _logger;
        public LoggingController(ISecurityManager securityManager, ILogService logger)
        {
            _securityManager = securityManager;
            _logger = logger;
        }

        [HttpPut]
        [Route("PutNewLog")]
        public IActionResult PutLog()
        {

            return NoContent();
        }
    }
}
