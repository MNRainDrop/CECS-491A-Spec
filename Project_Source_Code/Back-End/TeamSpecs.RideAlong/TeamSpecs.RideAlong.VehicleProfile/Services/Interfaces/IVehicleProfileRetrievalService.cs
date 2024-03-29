namespace TeamSpecs.RideAlong.VehicleProfile;
using TeamSpecs.RideAlong.Model;

public interface IVehicleProfileRetrievalService
{
    IResponse retrieveVehicleProfilesForUser(IAccountUserModel userAccount);
    IResponse retrieveVehicleProfilesForMarketplace();
}
