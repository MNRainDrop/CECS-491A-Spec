using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CoEsLibrary.Interfaces
{
    public interface ISqlDbCETarget
    {
        IResponse GetSellerSql(string vin);
    }
}
