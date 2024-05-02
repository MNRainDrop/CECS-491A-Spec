using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.DonateYourCarLibrary.Interfaces
{
    public interface IDonateYourCarManager
    {
        public IResponse retrieveCharities(IAccountUserModel userAccount);

    }
}
