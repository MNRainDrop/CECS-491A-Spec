using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Services.HashService;

namespace TeamSpecs.RideAlong.UserAdministration
{
    public class AccountModificationService
    {
        private IUserTarget _userTarget;
        private ILogService _logService;
        private IHashService _hashService;
        private IPepperService _pepper;

        public AccountModificationService(IUserTarget userTarget)
        {
            _userTarget = userTarget;
            _logService = new LogService(new SqlDbLogTarget(new SqlServerDAO()));
            _hashService = new HashService();
            _pepper = new PepperService(new FilePepperTarget(new JsonFileDAO()));

        }
        public IResponse ModifyUserProfile(string userName,DateTime dateOfBirth, string secondaryEmail) 
        {
            IResponse response = new Response();
             var userHash = _hashService.hashUser(userName, _pepper.RetrievePepper("Account Creation"));

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

            var profile = new ProfileUserModel(dateOfBirth);

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
