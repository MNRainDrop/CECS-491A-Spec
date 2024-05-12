using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;
using TeamSpecs.RideAlong.UserAdministration.Targets;

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

        public IResponse RecoverUserAccount(string userName)
        {
            IResponse response = new Response();

            return response;
        }

    }
}
