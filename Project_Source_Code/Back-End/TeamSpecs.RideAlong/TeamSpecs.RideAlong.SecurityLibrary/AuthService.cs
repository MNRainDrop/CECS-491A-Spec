using Microsoft.IdentityModel.Tokens;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Targets;

namespace TeamSpecs.RideAlong.SecurityLibrary;

public class AuthService : IAuthService
{
    IAuthTarget _authTarget;
    ILogService _logService;
    public AuthService(IAuthTarget authTarget, ILogService logService)
    {
        _authTarget = authTarget;
        _logService = logService;
    }

    public bool Authenticate(AuthNRequest loginAttempt, string otpHash)
    {
        return loginAttempt.otp == otpHash;
    }

    public bool Authorize(IAppPrincipal principal, Dictionary<string, string> requiredClaims)
    {
        bool hasMissingPermission = false;

        foreach (var claim in requiredClaims)
        {
            if (!principal.claims.Contains(claim))
            {
                hasMissingPermission = true;
                break;
            }
        }
        return !hasMissingPermission;
    }

    public IResponse GetOtpHash(IAuthUserModel model)
    {
        long uid = model.UID;
        IResponse response = _authTarget.fetchPass(uid);
        if (response.HasError)
        {
            if (response.ErrorMessage is null)
            {
                response.ErrorMessage = "Unknown Layer occurred at target layer or below";
            }
            _logService.CreateLogAsync("Error", "Data", response.ErrorMessage, model.userHash);
        }
        return response;
    }

    public IResponse GetUserLogInAttempts(IAuthUserModel model)
    {
        long uid = model.UID;
        IResponse response = _authTarget.fetchAttempts(uid);
        if (response.HasError)
        {
            if (response.ErrorMessage is null)
            {
                response.ErrorMessage = "Unknown Error occurred at target layer or below";
            }
            _logService.CreateLogAsync("Error", "Data", response.ErrorMessage, model.userHash);
        }
        return response;
    }

    public IResponse GetUserModel(string username)
    {
        IResponse response = _authTarget.fetchUserModel(username);
        if (response.HasError)
        {
            if (response.ErrorMessage is null)
            {
                response.ErrorMessage = $"Unknown error occurred at target layer or below for user {username}";
            }
            else
            {
                response.ErrorMessage += $"for user {username}";
            }
            _logService.CreateLogAsync("Error", "data", response.ErrorMessage, null);
        }
        return response;
    }

    public IResponse GetUserPrincipal(IAuthUserModel model)
    {
        // Get response with claims from ds
        IResponse userClaimsResponse = _authTarget.getClaims(model.UID);
        // Check if there are errors present
        if (userClaimsResponse.HasError)
        {
            IResponse errorResponse = new Response();
            if (userClaimsResponse.ErrorMessage is null)
            {
                errorResponse.ErrorMessage = "Unknown Error occurred at target or below";
            }
            else
            {
                errorResponse.ErrorMessage = userClaimsResponse.ErrorMessage;
            }
            _logService.CreateLogAsync("Error", "Server", errorResponse.ErrorMessage, model.userHash);
            return errorResponse;
        }
        // Check for presense of returnValue list
        if (userClaimsResponse.ReturnValue is null)
        {
            IResponse errorResponse = new Response();
            errorResponse.ErrorMessage = "Successful Response from Target, but no return value present";
            _logService.CreateLogAsync("Error", "Server", errorResponse.ErrorMessage, model.userHash); //Log error
            return errorResponse;
        }
        var value = userClaimsResponse.ReturnValue.First();
        // Check for null response
        if (value is null)
        {
            IResponse errorResponse = new Response();
            errorResponse.ErrorMessage = "DB returned a null value";
            return errorResponse;
        }
        if (value is List<KeyValuePair<string, string>>)
        {
            // Combine user model and claims to create a principal
            List<KeyValuePair<string, string>> claims = (List<KeyValuePair<string, string>>)value;
            IAppPrincipal principal = new RideAlongPrincipal(model, claims);
            IResponse successResponse = new Response();
            successResponse.ReturnValue = new List<object> { principal };
            successResponse.HasError = false;
            return successResponse;
        }
        else
        {
            IResponse errorResponse = new Response();
            errorResponse.ErrorMessage = "An unknown error has occcurred";
            _logService.CreateLogAsync("Error", "Server", errorResponse.ErrorMessage, model.userHash);
            return errorResponse;
        }
    }

    public IResponse UpdateOtp(IAuthUserModel model, string opt)
    {
        IResponse response = _authTarget.savePass(model.UID, opt);
        if (response.HasError)
        {
            if (response.ErrorMessage.IsNullOrEmpty() || response.ErrorMessage is null)
            {
                response.ErrorMessage = "Unknown Layer occurred at target layer or below";
            }
            _logService.CreateLogAsync("Error", "Service", response.ErrorMessage, model.userHash);
        }
        return response;
    }

    public IResponse updateLoginAttempt(IAuthUserModel model, int attempts)
    {
        IResponse response = _authTarget.updateAttempts(model.UID, attempts);
        if (response.HasError)
        {
            if (response.ErrorMessage.IsNullOrEmpty() || response.ErrorMessage is null)
            {
                response.ErrorMessage = "Unknown error happened at target layer or below";
            }
            _logService.CreateLogAsync("Error", "Service", response.ErrorMessage, model.userHash);
        }
        return response;
    }
    public IResponse GetFirstFailedLogin(IAuthUserModel model)
    {
        IResponse response = _authTarget.fetchFirstFailedLogin(model.UID);
        if (response.HasError)
        {
            if (response.ErrorMessage.IsNullOrEmpty() || response.ErrorMessage is null)
            {
                response.ErrorMessage = "Unknown error happened at target layer or below";
            }
            _logService.CreateLogAsync("Error", "Service", response.ErrorMessage, model.userHash);
        }
        return response;
    }
    public IResponse SetFirstFailedLogin(IAuthUserModel model, DateTime datetime)
    {
        IResponse response = _authTarget.setFirstFailedLogin(model.UID, datetime);
        if (response.HasError)
        {
            if (response.ErrorMessage.IsNullOrEmpty() || response.ErrorMessage is null)
            {
                response.ErrorMessage = "Unknown error happened at target layer or below";
            }
            _logService.CreateLogAsync("Error", "Service", response.ErrorMessage, model.userHash);
        }
        return response;
    }
}
