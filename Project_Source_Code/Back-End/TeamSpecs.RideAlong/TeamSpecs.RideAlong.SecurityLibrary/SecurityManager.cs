using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary.Model;

namespace TeamSpecs.RideAlong.SecurityLibrary
{
    public class SecurityManager : ISecurityManager
    {
        private IAuthService _authService;

        private ILogService _logger;
        /// <summary>
        /// Creates an error response and logs it, based on message passed in through exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private IResponse createErrorResponse(Exception ex, string userHash)
        {
            IResponse errorResponse = new Response();
            errorResponse.ErrorMessage = ex.Message;
            _logger.CreateLogAsync("Error", "Business", ex.Message, userHash);
            return errorResponse;
        }
        private object? unpackRespone(IResponse response)
        {
            return response.ReturnValue.First() ?? null;
        }
        public SecurityManager(IAuthService authService, ILogService logger)
        {
            _authService = authService;
            _logger = logger;
        }
        public bool Authorize()
        {
            throw new NotImplementedException();
        }

        public IResponse CreateAccessToken()
        {
            throw new NotImplementedException();
        }

        public IResponse CreateIdToken()
        {
            throw new NotImplementedException();
        }

        public string GetUsernameFromJwt(string idJwt)
        {
            throw new NotImplementedException();
        }

        public RideAlongPrincipal JwtToPrincipal(string idJwt, string accessJwt)
        {
            throw new NotImplementedException();
        }

        public IResponse Logout()
        {
            throw new NotImplementedException();
        }

        public IResponse RefreshTokens()
        {
            throw new NotImplementedException();
        }

        public IResponse StartLogin(string username)
        {
            IResponse response = _authService.GetUserModel(username);
            if (response.HasError)
            {
                IResponse errorResponse = new Response();
                errorResponse.ErrorMessage = "Error User not found: " + response.ErrorMessage;
                return errorResponse;
            }

            IAuthUserModel authUserModel;
            if (response.ReturnValue.First() is not null)
            {
                authUserModel = (IAuthUserModel)response.ReturnValue.First();
            }
            else
            {

                return
            }


            throw new NotImplementedException();
        }

        public IResponse TryAuthenticating(string username, string otp)
        {
            throw new NotImplementedException();
        }
    }
}
