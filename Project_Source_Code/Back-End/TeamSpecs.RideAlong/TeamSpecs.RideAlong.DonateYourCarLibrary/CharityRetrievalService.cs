using TeamSpecs.RideAlong.DonateYourCarLibrary.Interfaces;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.DonateYourCarLibrary
{
    public class CharityRetrievalService : ICharityRetrievalService
    {
        private readonly ISqlDbCharityTarget _charityTarget;
        private readonly ILogService _logService;

        public CharityRetrievalService(ISqlDbCharityTarget sqlDbCharityTarget, ILogService logService)
        {
            _charityTarget = sqlDbCharityTarget;
            _logService = logService;
        }

        public IResponse RetrieveCharities(IAccountUserModel userAccount)
        {
            var response = _charityTarget.RetrieveCharitiesSql();

            if (response.HasError)
            {
                response.ErrorMessage = "Could not retrieve charities." + response.ErrorMessage;
            }
            else
            {
                response.ErrorMessage = "Successful retrieval of charities.";
            }
            _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, userAccount.UserHash);

            return response;
        }

    }
}
