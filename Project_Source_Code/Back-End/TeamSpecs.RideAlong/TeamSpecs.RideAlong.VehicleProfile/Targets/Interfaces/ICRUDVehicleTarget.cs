using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public interface ICRUDVehicleTarget : IRetrieveVehiclesTarget, IRetrieveVehicleDetailsTarget, ICreateVehicleTarget, IModifyVehicleTarget, IDeleteVehicleTarget
{
}
