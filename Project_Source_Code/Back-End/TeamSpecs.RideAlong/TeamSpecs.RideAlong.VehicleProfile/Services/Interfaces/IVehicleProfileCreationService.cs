namespace TeamSpecs.RideAlong.VehicleProfile;
using TeamSpecs.RideAlong.Model;

public interface IVehicleProfileCreationService
{
    IResponse CreateVehicleProfile(string vin, string licensePlate, string? make, string? model, int year, string? color, string? description, IAccountUserModel userAccount);
    IResponse CreateVehicleProfile(IVehicleProfileModel vehicle, IVehicleDetailsModel vehicleDetails, IAccountUserModel userAccount);
}
