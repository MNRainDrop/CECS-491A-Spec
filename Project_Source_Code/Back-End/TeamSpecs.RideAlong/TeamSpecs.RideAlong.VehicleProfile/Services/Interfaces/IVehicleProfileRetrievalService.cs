namespace TeamSpecs.RideAlong.VehicleProfile;
using TeamSpecs.RideAlong.Model;

public interface IVehicleProfileRetrievalService
{
    IResponse retrieveVehicleProfilesForUser(IAccountUserModel userAccount, int numOfResults, int page);
}
