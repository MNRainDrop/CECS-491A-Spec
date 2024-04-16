using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleMarketplace;

public interface IMarketplaceTarget
{
    public IResponse UploadVehicleToMarketplace(string VIN, int view, string Description, int Status);

    public IResponse DeleteVehicleFromMarketplace(string VIN);

    public IResponse ReadAllPublicVehicleProfileSql();

    public IResponse SearchMarketplaceVehicleProfile(ICollection<object> searchParameters);

    public IResponse VehicleMarketplaceSendRequestService(INotification buyRequest);

}