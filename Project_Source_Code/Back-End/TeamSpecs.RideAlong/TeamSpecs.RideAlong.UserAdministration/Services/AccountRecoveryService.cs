using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

namespace TeamSpecs.RideAlong.UserAdministration.Services
{
    public class AccountRecoveryService : IAccountRecoveryService
    {
        private readonly ISqlDbUserRecoveryTarget _userTarget;
        private readonly ILogService _logService;

        public AccountRecoveryService(ISqlDbUserRecoveryTarget userTarget, ILogService logService)
        {
            _userTarget = userTarget;
            _logService = logService;
        }

        public IResponse getUserRecoveryEmail(string email)
        {
            IResponse response = new Response();

            response = _userTarget.retrieveAltEmail(email);

            return response;
        }

        public IResponse setRecoveryOtp()
        {
            IResponse response = new Response();



            return response;
        }

        public IResponse RecoverUserAccount(string userName)
        {
            IResponse response = new Response();

            return response;
        }

    }
}
