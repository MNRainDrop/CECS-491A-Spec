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
    public IActionResult Post([FromBody] AccountUserModel userAccount, int page)
    {
        var result = _retrievalManager.GetVehicleProfiles(userAccount, page);
        return Ok("Recieved Something");
    }
}
