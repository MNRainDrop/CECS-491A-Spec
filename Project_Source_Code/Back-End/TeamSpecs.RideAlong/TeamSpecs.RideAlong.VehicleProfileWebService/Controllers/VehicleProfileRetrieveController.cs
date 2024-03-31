using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.VehicleProfile;

namespace TeamSpecs.RideAlong.VehicleProfileWebService.Controllers;

[ApiController]
[Route("[controller]")]
public class VehicleProfileRetrieveController : Controller
{
    private readonly IGenericDAO _DAO;
    private readonly IHashService _hashService;
    private readonly ILogTarget _logTaret;
    private readonly ILogService _logService;
    private readonly IRetrieveVehiclesTarget _retrieveVehiclesTarget;
    private readonly IRetrieveVehicleDetailsTarget _retrieveVehicleDetailsTarget;
    private readonly IVehicleProfileDetailsRetrievalService _vpdRetrievalService;
    private readonly IVehicleProfileRetrievalService _vpRetrievalService;
    private readonly IVehicleProfileRetrievalManager _retrievalManager;

    public VehicleProfileRetrieveController()
    {
        _hashService = new HashService();
        _DAO = new SqlServerDAO();
        _logTaret = new SqlDbLogTarget(_DAO);
        _logService = new LogService(_logTaret, _hashService);
        _retrieveVehicleDetailsTarget = new SqlDbVehicleTarget(_DAO);
        _retrieveVehiclesTarget = new SqlDbVehicleTarget(_DAO);
        _vpdRetrievalService = new VehicleProfileDetailsRetrievalService(_retrieveVehicleDetailsTarget, _logService);
        _vpRetrievalService = new VehicleProfileRetrievalService(_retrieveVehiclesTarget, _logService);
        _retrievalManager = new VehicleProfileRetrievalManager(_logService, _vpRetrievalService, _vpdRetrievalService);
    }

    [HttpPost]
    [Route("/MyVehicleProfiles")]
    public IActionResult Post([FromBody]UserPageModel requestData)
    {
        try
        {
            var result = _retrievalManager.GetVehicleProfiles(requestData.AccountUser, requestData.Page);
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
    public IActionResult Post([FromBody]VehicleAccountModel requestData)
    {
        try
        {
            var result = _retrievalManager.GetVehicleProfileDetails(requestData.VehicleProfile, requestData.AccountUser);
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
        public AccountUserModel AccountUser { get; set; }
        public int Page { get; set; }
    }

    public class VehicleAccountModel
    {
        public VehicleProfileModel VehicleProfile { get; set; }
        public AccountUserModel AccountUser { get; set; }
    }
}
