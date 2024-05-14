

using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CoEsLibrary.Interfaces
{
    public interface ICommEstaManager
    {
        IResponse GetSeller(IAccountUserModel user, string vin);
    }
}
