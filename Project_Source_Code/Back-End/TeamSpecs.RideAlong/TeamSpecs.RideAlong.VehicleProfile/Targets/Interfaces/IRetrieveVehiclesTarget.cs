namespace TeamSpecs.RideAlong.VehicleProfile;
using TeamSpecs.RideAlong.Model;

public interface IRetrieveVehiclesTarget
{
    IResponse readVehicleProfileSql(ICollection<object> searchParameters, int numOfResults, int page);
}
