using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

namespace TeamSpecs.RideAlong.SystemObservabilityEntryPoint.Controllers
{
    [Route("[controller]")]
    public class LogController : Controller
    {
        ISecurityManager _securityManager;
        ILogService _logger;
        public LogController(ISecurityManager securityManager, ILogService logger)
        {
            _securityManager = securityManager;
            _logger = logger;
        }
        public class LogInformation
        {
            public string level;
            public string category;
            public string message;
            public LogInformation(string level, string category, string message)
            {
                this.level = level;
                this.category = category;
                this.message = message;
            }
        }
        [HttpPut]
        [Route("Log")]
        public IActionResult PutLog([FromBody] LogInformation logInfo)
        {
            try
            {
                string? userHash = _securityManager.JwtToPrincipal().userIdentity.userHash;
                _logger.CreateLogAsync(logInfo.level, logInfo.category, logInfo.message, null);
                return NoContent();
            }
            catch (Exception ex)
            {
                string remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress!.ToString();
                _logger.CreateLogAsync("Warning", "", $"Unauthorized Logging attempt from IP: {remoteIpAddress}", null);
                return Unauthorized("Could Not Get token from User");
            }
        }
    }
}
