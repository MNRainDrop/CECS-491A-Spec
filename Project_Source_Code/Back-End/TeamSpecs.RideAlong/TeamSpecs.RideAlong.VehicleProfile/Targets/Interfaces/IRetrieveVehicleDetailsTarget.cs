using TeamSpecs.RideAlong.Model;

public interface IRetrieveVehicleDetailsTarget
{
    IResponse ReadVehicleProfileDetailsSql(ICollection<object> searchParameters);
}
