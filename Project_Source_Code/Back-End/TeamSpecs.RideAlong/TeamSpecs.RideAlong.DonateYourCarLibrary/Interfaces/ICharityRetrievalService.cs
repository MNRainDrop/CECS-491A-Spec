using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.DonateYourCarLibrary.Interfaces
{
    public interface ICharityRetrievalService
    {
        public IResponse RetrieveCharities(IAccountUserModel userAccount);


    }
}
