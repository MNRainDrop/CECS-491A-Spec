using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleMarketplace;

public interface IMarketplaceTarget
{
    IResponse UploadVehicleToMarketplace(string VIN, int view, string Description, int Status);

    IResponse DeleteVehicleFromMarketplace(string VIN);

    IResponse ReadAllPublicVehicleProfileSql(int numOfResults, int page);

    IResponse RetrieveDetailVehicleProfileSql(string VIN);

    IResponse SearchMarketplaceVehicleProfile(ICollection<object> searchParameters);

    IResponse VehicleMarketplaceSendRequestService(string VIN);

}