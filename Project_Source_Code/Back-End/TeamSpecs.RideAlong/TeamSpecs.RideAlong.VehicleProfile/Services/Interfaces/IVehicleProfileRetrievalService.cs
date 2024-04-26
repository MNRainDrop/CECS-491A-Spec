namespace TeamSpecs.RideAlong.VehicleProfile;
using TeamSpecs.RideAlong.Model;

public interface IVehicleProfileRetrievalService
{
    IResponse RetrieveVehicleProfilesForUser(IAccountUserModel userAccount, int numOfResults, int page);
}
