using System.Diagnostics;
using TeamSpecs.RideAlong.DonateYourCarLibrary.Interfaces;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.DonateYourCarLibrary
{
    public class DonateYourCarManager : IDonateYourCarManager
    {
        private readonly ICharityRetrievalService _charityRetrievalService;
        private readonly ILogService _logService;

        public DonateYourCarManager(ICharityRetrievalService charityRetrievalService, ILogService logService)
        {
            _charityRetrievalService = charityRetrievalService;
            _logService = logService;
        }

        public IResponse retrieveCharities(IAccountUserModel user)
        {
            IResponse response = new Response();
            var timer = new Stopwatch();

            timer.Start();
            response = _charityRetrievalService.RetrieveCharities(user);
            timer.Stop();


            if (response.HasError == true) // Means that SQL generation/ DB  failed
            {
#pragma warning disable CS8604 // Possible null reference argument.
                _logService.CreateLogAsync("Error", "Server", response.ErrorMessage, user.UserHash);
#pragma warning restore CS8604 // Possible null reference argument.
            }
            else if (response.HasError == false)
            {
                _logService.CreateLogAsync("Info", "Business", "Successful retrieval of charities", user.UserHash);
            }
            return response;
        }

    }


}
