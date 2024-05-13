using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

namespace TeamSpecs.RideAlong.UserAdministration.Services
{
    public class GetAccountUpdateService : IGetAccountUpdateService
    {
        private readonly IUserTarget _userTarget;
        private readonly ILogService _logService;

        public GetAccountUpdateService(IUserTarget userTarget, ILogService logService)
        {
            _userTarget = userTarget;
            _logService = logService;
        }

        public IResponse GetUpdateUserAccount(IAccountUserModel accountUserModel)
        {
            var response = _userTarget.GetUpdatedUserSql(accountUserModel);
            if (response.HasError)
            {
                response.ErrorMessage = "Could not retrieve updated account details." + response.ErrorMessage;
            }
            else
            {
                response.ErrorMessage = "Successful retrieval of apdated account details.";
            }
            _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, accountUserModel.UserHash);

            return response;
        }
    }
}
