using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.Services.HashService;

namespace TeamSpecs.RideAlong.UserAdministration
{
    public class AccountRecoveryService
    {
        private IUserTarget _userTarget;
        private ILogService _logService;
        private IHashService _hashService;
        private IPepperService _pepper;
        public AccountRecoveryService(IUserTarget userTarget)
        {
            _userTarget = userTarget;
            _logService = new LogService(new SqlDbLogTarget(new SqlServerDAO()));
            _hashService = new HashService();
            _pepper = new PepperService(new FilePepperTarget(new JsonFileDAO()));
        }

        public IResponse EnableUserAccount(string userName)
        {
            IResponse response = new Response();
            // Use this when RetrievePepper works
            //var userHash = _hashService.hashUser(userName, (int)_pepper.RetrievePepper("Account Creation"));

            // Using this for now
            var userHash = _hashService.hashUser(userName, 0);

            #region Validiating Arguements
            if (string.IsNullOrWhiteSpace(userName))
            {
                _logService.CreateLogAsync("Error", "Data", $"{nameof(userName)} must be valid", userHash);
                throw new ArgumentException($"{nameof(userName)} must be valid");
            }
            #endregion

            response = _userTarget.EnableUserAccountSql(userName);

            if(response.HasError) 
            {
                _logService.CreateLogAsync("Error", "Data", response.ErrorMessage, userHash);
                response.ErrorMessage = "User Account was unable to be enabled";
            }
            _logService.CreateLogAsync("Info", "Server", "EnableUserAccount Successful", userHash);
            return response;
        }

        public IResponse DisableUserAccount(string userName)
        {
            IResponse response = new Response();
            // Use this when RetrievePepper works
            //var userHash = _hashService.hashUser(userName, (int)_pepper.RetrievePepper("Account Creation"));

            // Using this for now
            var userHash = _hashService.hashUser(userName, 0);

            #region Validiating Arguements
            if (String.IsNullOrWhiteSpace(userName))
            {
                _logService.CreateLogAsync("Error", "Data", $"{nameof(userName)} must be valid", userHash);
                throw new ArgumentException($"{nameof(userName)} must be valid");
            }
            #endregion

            response = _userTarget.DisableUserAccountSql(userName);

            if (response.HasError)
            {
                _logService.CreateLogAsync("Error", "Data", response.ErrorMessage, userHash);
                response.ErrorMessage = "User Account was unable to be disabled";
            }
            _logService.CreateLogAsync("Info", "Server", "DisableUserAccount Successful", userHash);
            return response;
        }

        public IResponse RecoverUserAccount(string userName)
        {
            IResponse response = new Response();
            // Use this when RetrievePepper works
            //var userHash = _hashService.hashUser(userName, (int)_pepper.RetrievePepper("Account Creation"));

            // Using this for now
            var userHash = _hashService.hashUser(userName, 0);

            #region Validiating Arguements
            if (string.IsNullOrWhiteSpace(userName))
            {
                _logService.CreateLogAsync("Error", "Data", $"{nameof(userName)} must be valid", userHash);
                throw new ArgumentException($"{nameof(userName)} must be valid");
            }
            #endregion

            response = _userTarget.RecoverUserAccountSql(userName);

            if (response.HasError)
            {
                _logService.CreateLogAsync("Error", "Data", response.ErrorMessage, userHash);
                response.ErrorMessage = "User secondary email was unable to be recovered";
            }
            _logService.CreateLogAsync("Info", "Server", "DisableUserAccount Successful", userHash);
            // Response will hold secondaryEmail string and be sent to Manager Layer to process OTP sending
            return response;
        }

    }
}
