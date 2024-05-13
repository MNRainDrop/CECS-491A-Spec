using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

namespace TeamSpecs.RideAlong.UserAdministration.Services
{
    public class PostAccountUpdateService : IPostAccountUpdateService
    {
        private readonly IUserTarget _userTarget;
        private readonly ILogService _logService;

        public PostAccountUpdateService(IUserTarget userTarget, ILogService logService)
        {
            _userTarget = userTarget;
            _logService = logService;
        }

        public IResponse UpdateUserAccount(IAccountUserModel userAccount, string address, string name, string phone, string accountType)
        {
            var response = _userTarget.PostUpdatedUserSql(userAccount, address, name, phone, accountType);
            if (response.HasError)
            {
                response.ErrorMessage = "Could not update account details." + response.ErrorMessage;
            }
            else
            {
                //response.ErrorMessage = "Successfully apdated account details.";
            }
#pragma warning disable CS8604
            _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, userAccount.UserHash);
#pragma warning restore CS8604
            return response;
        }
    }
}
