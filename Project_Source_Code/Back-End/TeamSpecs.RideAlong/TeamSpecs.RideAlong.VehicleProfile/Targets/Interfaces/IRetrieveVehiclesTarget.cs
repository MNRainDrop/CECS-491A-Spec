namespace TeamSpecs.RideAlong.VehicleProfile;
using TeamSpecs.RideAlong.Model;

public interface IRetrieveVehiclesTarget
{
    IResponse ReadVehicleProfileSql(ICollection<object> searchParameters);
}
