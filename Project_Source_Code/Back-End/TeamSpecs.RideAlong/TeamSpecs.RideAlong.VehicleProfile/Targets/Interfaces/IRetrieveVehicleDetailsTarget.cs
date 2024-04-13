using TeamSpecs.RideAlong.Model;

public interface IRetrieveVehicleDetailsTarget
{
    IResponse readVehicleProfileDetailsSql(ICollection<object> searchParameters);
}
