using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SystemObservability;

namespace SystemObservabilityEntryPoint.Controllers;

[ApiController]
[Route("[controller]")]
public class SystemObservabilityController : ControllerBase
{
    private readonly ISystemObservabilityManager _systemObservabilityManager;
    private readonly ISecurityManager _securityManager;

    public SystemObservabilityController(ISystemObservabilityManager systemObservability, ISecurityManager security)
    {
        _systemObservabilityManager = systemObservability;
        _securityManager = security;
    }

    [HttpPost]
    [Route("PostAuthStatus")]
    public IActionResult PostAuthStatus()
    {
        Dictionary<string, string> requiredClaims = new Dictionary<string, string>
        {
            { "canView", "usageDashboard" }
        };
        bool hasPermission;
        try
        {
            hasPermission = _securityManager.isAuthorize(requiredClaims);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
        if (!hasPermission)
        {
            return StatusCode(StatusCodes.Status403Forbidden, "Insufficient Permissions");
        }
        return NoContent();
    }

    [HttpPost]
    [Route("PostGetLogs")]
    public IActionResult PostGetLogs([FromBody] int dateRange)
    {
        try
        {
            var result = _systemObservabilityManager.GetAllLogs(dateRange);
            if (result is not null)
            {
                if (result.HasError)
                {
                    return BadRequest(result.ErrorMessage);
                }
                else
                {
                    return Ok(result.ReturnValue);
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
    [Route("PostGetKPIs")]
    public IActionResult PostGetKPIs([FromBody] int dateRange)
    {
        try
        {
            var result = _systemObservabilityManager.GetALlKPIs(dateRange);
            if (result is not null)
            {
                if (result.HasError)
                {
                    return BadRequest(result.ErrorMessage);
                }
                else
                {
                    return Ok(result.ReturnValue);
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

}
