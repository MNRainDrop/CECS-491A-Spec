using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CELibrary.Interfaces
{
    public interface ISqlDbCETarget
    {
        public IResponse GetSeller(string vin);
    }
}
