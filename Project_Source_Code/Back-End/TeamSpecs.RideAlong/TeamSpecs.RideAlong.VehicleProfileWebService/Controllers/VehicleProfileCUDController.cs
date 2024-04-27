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
    public IActionResult PostCreateVehicleProfile([FromBody] RequestData requestData)
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
                    Salt = BitConverter.ToUInt32(temp.salt, 0),
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

        var vehicle = new VehicleProfileModel(requestData.vehicleProfile.VIN, user.UserId, requestData.vehicleProfile.LicensePlate, requestData.vehicleProfile.Make, requestData.vehicleProfile.Model, requestData.vehicleProfile.Year);
        var details = new VehicleDetailsModel(requestData.vehicleDetails.VIN, requestData.vehicleDetails.Color, requestData.vehicleDetails.Description);

        try
        {
            var result = _vpCUD.CreateVehicleProfile(vehicle, details, user);
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
    public IActionResult PostModifyVehicleProfile([FromBody] RequestData requestData)
    {
        #region Check for valid claims
        Dictionary<string, string> requiredClaims = new Dictionary<string, string>
        {
            { "canView", "vehicleProfile" },
            { "canModifyVehicle", requestData.vehicleProfile.VIN.ToString() }
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
                    Salt = BitConverter.ToUInt32(temp.salt, 0),
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

        var vehicle = new VehicleProfileModel(requestData.vehicleProfile.VIN, user.UserId, requestData.vehicleProfile.LicensePlate, requestData.vehicleProfile.Make, requestData.vehicleProfile.Model, requestData.vehicleProfile.Year);
        var details = new VehicleDetailsModel(requestData.vehicleDetails.VIN, requestData.vehicleDetails.Color, requestData.vehicleDetails.Description);

        try
        {
            var result = _vpCUD.ModifyVehicleProfile(vehicle, details, user);
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
    [Route("DeleteVehicleProfile")]
    public IActionResult PostDeleteVehicleProfile([FromBody] RequestData requestData)
    {
        #region Check for valid claims
        Dictionary<string, string> requiredClaims = new Dictionary<string, string>
        {
            { "canView", "vehicleProfile" },
            { "canDeleteVehicle", requestData.vehicleProfile.VIN.ToString() }
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
                    Salt = BitConverter.ToUInt32(temp.salt, 0),
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

        var vehicle = new VehicleProfileModel(requestData.vehicleProfile.VIN, user.UserId, requestData.vehicleProfile.LicensePlate, requestData.vehicleProfile.Make, requestData.vehicleProfile.Model, requestData.vehicleProfile.Year);
        var details = new VehicleDetailsModel(requestData.vehicleDetails.VIN, requestData.vehicleDetails.Color, requestData.vehicleDetails.Description);

        try
        {
            var result = _vpCUD.DeleteVehicleProfile(vehicle, user);
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

    public class RequestData
    {
        public required JavaScriptVehicle vehicleProfile { get; set; }
        public required JavaScriptDetails vehicleDetails { get; set; }

    }
    public class JavaScriptVehicle
    {
        public required string VIN { get; set; }
        public required string LicensePlate { get; set; }
        public required string Make { get; set; }
        public required string Model { get; set; }
        public required int Year { get; set; }
    }

    public class JavaScriptDetails
    {
        public required string VIN { get; set; }
        public required string Color { get; set; }
        public required string Description { get; set; }
    }
}
