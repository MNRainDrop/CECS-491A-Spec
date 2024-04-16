using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.VehicleProfile;

namespace TeamSpecs.RideAlong.VehicleProfileWebService.Controllers;

[ApiController]
[Route("[controller]")]
public class VehicleProfileRetrieveController : Controller
{
    private readonly ILogService _logService;
    private readonly IVehicleProfileRetrievalManager _retrievalManager;
    private readonly ISecurityManager _securityManager;

    public VehicleProfileRetrieveController(ILogService logService, IVehicleProfileRetrievalManager retrievalManager, ISecurityManager securityManager)
    {
        _logService = logService;
        _retrievalManager = retrievalManager;
        _securityManager = securityManager;
    }

    [HttpPost]
    [Route("/MyVehicleProfiles")]
    public IActionResult Post([FromBody]int page)
    {
        IAccountUserModel user;
        try
        {
            var temp = _securityManager.JwtToPrincipal().userIdentity;
            if (!string.IsNullOrWhiteSpace(temp.userName))
            {
                user = new AccountUserModel(temp.userName)
                {
                    Salt = BitConverter.ToInt64(temp.salt, 0),
                    UserHash = temp.userHash,
                    UserId = temp.UID
                };
            }
            else
            {
                return BadRequest();
            }

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        try
        {
            var result = _retrievalManager.GetVehicleProfiles(user, page);
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
    [Route("/MyVehicleProfileDetails")]
    public IActionResult Post([FromBody]IVehicleProfileModel vehicle)
    {
        IAccountUserModel user;
        try
        {
            var temp = _securityManager.JwtToPrincipal().userIdentity;
            if (!string.IsNullOrWhiteSpace(temp.userName))
            {
                user = new AccountUserModel(temp.userName)
                {
                    Salt = BitConverter.ToInt64(temp.salt, 0),
                    UserHash = temp.userHash,
                    UserId = temp.UID
                };
            }
            else
            {
                return BadRequest();
            }

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        try
        {
            var result = _retrievalManager.GetVehicleProfileDetails(vehicle, user);
            if (result is not null)
            {
                if (result.HasError)
                {
                    return BadRequest(result.ErrorMessage);
                }
                else
                {
                    if (result.ReturnValue is null || result.ReturnValue.Count == 0)
                    {
                        // Could not retrieve values for vehicle details
                        return StatusCode(202);
                    }
                    // Could find vehicle details
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
            // Input values are wrong
            return BadRequest(ex.Message);
        }
    }

    public class UserPageModel
    {
        public AccountUserModel? AccountUser { get; set; }
        public int Page { get; set; }
    }
}
