using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public class VehicleProfileCUDManager : ICreateVehicleProfileManager, IDeleteVehicleProfileManager, IModifyVehicleProfileManager
{
    private readonly ILogService _logService;
    private readonly IVehicleProfileCreationService _vpCreate;
    private readonly IVehicleProfileModificationService _vpModify;
    private readonly IVehicleProfileDeletionService _vpDelete;

    public VehicleProfileCUDManager(ILogService logService, IVehicleProfileCreationService vpCreate, IVehicleProfileModificationService vpModify, IVehicleProfileDeletionService vpDelete)
    {
        _logService = logService;
        _vpCreate = vpCreate;
        _vpModify = vpModify;
        _vpDelete = vpDelete;
    }

    public IResponse createVehicleProfile()
    {
        throw new NotImplementedException();
    }

    public IResponse deleteVehicleProfile()
    {
        throw new NotImplementedException();
    }

    public IResponse modifyVehicleProfile()
    {
        throw new NotImplementedException();
    }
}
