using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.VehicleProfile;

namespace TeamSpecs.RideAlong.VehicleProfileEndPoint.Controllers;

[ApiController]
[Route("[controller]")]
public class VehicleProfileCUDController : Controller
{
    private readonly IVehicleProfileCUDManager _vpCUD;
    private readonly ISecurityManager _securityManager;

    public VehicleProfileCUDController(IVehicleProfileCUDManager vehicleProfileCUDManager, ISecurityManager securityManager)
    {
        _vpCUD = vehicleProfileCUDManager;
        _securityManager = securityManager;
    }

    [HttpPost]
    [Route("CreateVehicleProfile")]
    public IActionResult PostCreateVehicleProfile([FromBody] VehicleProfileAndDetails profileAndDetailsModel)
    {
        #region Check for valid claims
        Dictionary<string, string> requiredClaims = new Dictionary<string, string>
        {
            { "canView", "vehicleProfile" },
            { "canCreateVehicle", "true"}
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
        #endregion

        #region Get user model
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
        #endregion

        try
        {
            var result = _vpCUD.CreateVehicleProfile(profileAndDetailsModel.vehicle, profileAndDetailsModel.vehicleDetails, user);
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
    [Route("ModifyVehicleProfile")]
    public IActionResult PostModifyVehicleProfile([FromBody] VehicleProfileAndDetails profileAndDetailsModel)
    {
        #region Check for valid claims
        Dictionary<string, string> requiredClaims = new Dictionary<string, string>
        {
            { "canView", "vehicleProfile" },
            { "canModifyVehicle", "true"}
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
        #endregion

        #region Get user model
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
        #endregion

        try
        {
            var result = _vpCUD.ModifyVehicleProfile(profileAndDetailsModel.vehicle, profileAndDetailsModel.vehicleDetails, user);
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

    public class VehicleProfileAndDetails
    {
        public VehicleProfileModel vehicle;
        public VehicleDetailsModel vehicleDetails;

        public VehicleProfileAndDetails(VehicleProfileModel vehicle, VehicleDetailsModel vehicleDetails)
        {
            this.vehicle = vehicle;
            this.vehicleDetails = vehicleDetails;
        }
    }
}
