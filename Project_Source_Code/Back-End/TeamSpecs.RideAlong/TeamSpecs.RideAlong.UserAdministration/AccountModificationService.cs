using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.LoggingLibrary;

namespace TeamSpecs.RideAlong.UserAdministration
{
    public class AccountModificationService
    {
        private IUserTarget _userTarget;
        private ILogService _logService;
        private IHashService _hashService;
        private IPepperService _pepper;

        public AccountModificationService(IUserTarget userTarget, ILogService logService, IHashService hashService, IPepperService pepperService)
        {
            _userTarget = userTarget;
            _logService = logService;
            _hashService = hashService;
            _pepper = pepperService;

        }
        public IResponse ModifyUserProfile(string userName,DateTime dateOfBirth, string secondaryEmail) 
        {
            IResponse response = new Response();
            var userHash = _hashService.hashUser(userName, (int) _pepper.RetrievePepper("Account Creation"));

            #region Validiating Arguements
            if (String.IsNullOrWhiteSpace(userName))
            {
                _logService.CreateLogAsync("Error", "Data", $"{nameof(userName)} must be valid", userHash);
                throw new ArgumentException($"{nameof(userName)} must be valid");
            }
            if (String.IsNullOrWhiteSpace(secondaryEmail))
            {
                _logService.CreateLogAsync("Error", "Data", $"{nameof(secondaryEmail)} must be valid", userHash);
                throw new ArgumentException($"{nameof(secondaryEmail)} must be valid");
            }
            #endregion

            var profile = new ProfileUserModel(dateOfBirth, secondaryEmail);

            response = _userTarget.ModifyUserProfileSql(userName, profile);

            if(response.HasError)
            {
                _logService.CreateLogAsync("Error", "Data", response.ErrorMessage, userHash);
                response.ErrorMessage = "Could not modify the user profile";
            }

            _logService.CreateLogAsync("Info", "Server", "UserProfile successfully modified", userHash);
            return response;
        }
    }
}
