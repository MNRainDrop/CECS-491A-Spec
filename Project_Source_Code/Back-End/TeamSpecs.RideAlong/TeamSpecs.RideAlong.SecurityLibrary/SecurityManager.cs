using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary.Model;
using TeamSpecs.RideAlong.Services;



namespace TeamSpecs.RideAlong.SecurityLibrary
{
    public class SecurityManager : ISecurityManager
    {
        private IAuthService _authService;

        private ILogService _logger;
        private IHttpContextAccessor _httpContextAccessor;
        private string _rideAlongSecretKey = "This is Ridealong's super secret key for testing security";
        private string _rideAlongIssuer = "Ride Along by Team Specs";
        private string _accessTokenHeader = "X-Access-Token";
        private string _refreshTokenHeader = "X-Refresh-Token";
        private int __otpLength = 10;

        public SecurityManager(IAuthService authService, ILogService logger, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Creates an error response and logs it, based on message passed in through exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private object? unpackResponse(IResponse response)
        {
            if (response.ReturnValue is not null)
            {
                return response.ReturnValue.First();
            }
            else
            {
                throw new Exception("Null Value passed into unpacker");
            }
        }
        public IAppPrincipal JwtToPrincipal()
        {
            HttpContext context = _httpContextAccessor.HttpContext;
            string accessToken;
            var handler = new JwtSecurityTokenHandler();

            #region Check for null, then get token
            if (context.Request.Headers[_accessTokenHeader].IsNullOrEmpty()) { throw new Exception("No Token found"); }
            accessToken = context.Request.Headers[_accessTokenHeader]!;
            var token = handler.ReadJwtToken(accessToken);
            #endregion

            #region Validate Access Token
            if (accessToken.IsNullOrEmpty()) { throw new Exception("No Token Provided"); }

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_rideAlongSecretKey)),
                ValidateIssuer = true,
                ValidIssuer = _rideAlongIssuer,
                ValidateAudience = true,
                ValidAudience = _rideAlongIssuer
            };
            // Check Signature
            try { handler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken validatedToken); }
            catch (Exception ex) { throw new Exception("Invalid Token: " + ex.Message); }
            #endregion

            #region Get principal from token
            string jsonPrincipal;
            try { jsonPrincipal = token.Claims.First(c => c.Type == "scope").Value; }
            catch (Exception ex) { throw new Exception("Could not get principal: " + ex.Message); }
            using JsonDocument jsonDocument = JsonDocument.Parse(jsonPrincipal);

            AuthUserModel userIdentity;
            ICollection<KeyValuePair<string, string>> claims;
            if (jsonDocument.RootElement.TryGetProperty("userIdentity", out JsonElement uidElement) && uidElement.ValueKind == JsonValueKind.Object)
            {
                userIdentity = JsonSerializer.Deserialize<AuthUserModel>(uidElement.ToString())!;
            }
            else
            {
                throw new Exception("User Identity Not present in Principal");
            }
            if (jsonDocument.RootElement.TryGetProperty("claims", out JsonElement claimsElement) && claimsElement.ValueKind == JsonValueKind.Array)
            {
                claims = JsonSerializer.Deserialize<List<KeyValuePair<string, string>>>(claimsElement)!;
            }
            else
            {
                throw new Exception("User Claims not present in Principal");
            }

            RideAlongPrincipal principal = new RideAlongPrincipal()
            {
                userIdentity = userIdentity,
                claims = claims
            };
            #endregion

            return principal;
        }
        public bool isAuthorize(Dictionary<string, string> requiredClaims)
        {
            bool result = false;
            IAppPrincipal rideAlongPrincipal;

            try { rideAlongPrincipal = JwtToPrincipal(); }
            catch (Exception ex) { throw new Exception("Error Retrieving Principal: " + ex.Message); }

            result = _authService.Authorize(rideAlongPrincipal, requiredClaims);

            return result;
        }

        public IResponse CreateAccessToken(IAppPrincipal userPrincpal)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("sub", userPrincpal.userIdentity.userName ?? ""),
                new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("azp", userPrincpal.userIdentity.UID.ToString()),
                new Claim("scope", JsonSerializer.Serialize(userPrincpal)),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_rideAlongSecretKey)); // This should be replaced as soon as we have configuration working
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(claims: claims, signingCredentials: credentials, audience: _rideAlongIssuer, issuer: _rideAlongIssuer, expires: DateTime.UtcNow.AddMinutes(15));
            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            IResponse accessTokenResponse = new Response();
            accessTokenResponse.HasError = false;
            accessTokenResponse.ReturnValue = new List<object>() { jwt };
            return accessTokenResponse;
        }

        public IResponse CreateIdToken(IAppPrincipal userPrincpal, DateTimeOffset timeAuthorized)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("sub", userPrincpal.userIdentity.userName ?? ""),
                new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("auth_time", timeAuthorized.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_rideAlongSecretKey)); // This should be replaced as soon as we have configuration working
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(claims: claims, signingCredentials: credentials, issuer: _rideAlongIssuer, audience: userPrincpal.userIdentity.UID.ToString(), expires: DateTime.UtcNow.AddMinutes(15));
            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            IResponse idTokenResponse = new Response();
            idTokenResponse.HasError = false;
            idTokenResponse.ReturnValue = new List<object>
            {
                jwt
            };
            return idTokenResponse;
        }
        public IResponse CreateRefreshToken(IAppPrincipal userPrincpal)
        {
            // Same format as ID token, except we change the expiration date
            // The goal is to give a user a long session, but periodically refresh the other tokens
            // This does not get refreshed when calling the refresh tokens function, rather it is used to ensure we are allowed to refresh the tokens
            List<Claim> claims = new List<Claim>
            {
                new Claim("iss",_rideAlongIssuer),
                new Claim("sub", userPrincpal.userIdentity.userName ?? ""),
                new Claim("aud", userPrincpal.userIdentity.UID.ToString()),
                new Claim("exp", DateTimeOffset.UtcNow.AddMinutes(120).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_rideAlongSecretKey)); // This should be replaced as soon as we have configuration working
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(claims: claims, signingCredentials: credentials);
            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            IResponse accessTokenResponse = new Response();
            accessTokenResponse.HasError = false;
            accessTokenResponse.ReturnValue = new List<object>() { jwt };
            return accessTokenResponse;
        }


        // Returns a response object with 2 tokens [ID, Access]
        public IResponse RefreshTokens()
        {
            IResponse refreshResponse = new Response();
            HttpContext context = _httpContextAccessor.HttpContext;
            var refreshToken = context.Request.Headers[_refreshTokenHeader];

            if (refreshToken.IsNullOrEmpty())
            {
                refreshResponse.ErrorMessage = "No Refresh Token Found";
                return refreshResponse;
            }

            #region Check for null, then get token
            string accessToken;
            var tempHandler = new JwtSecurityTokenHandler();

            if (context.Request.Headers[_accessTokenHeader].IsNullOrEmpty()) { throw new Exception("No Token found"); }
            accessToken = context.Request.Headers[_accessTokenHeader]!;
            var token = tempHandler.ReadJwtToken(accessToken);
            #endregion

            #region Get principal from token
            string jsonPrincipal;
            try { jsonPrincipal = token.Claims.First(c => c.Type == "scope").Value; }
            catch (Exception ex) { throw new Exception("Could not get principal: " + ex.Message); }
            using JsonDocument jsonDocument = JsonDocument.Parse(jsonPrincipal);

            AuthUserModel userIdentity;
            ICollection<KeyValuePair<string, string>> claims;
            if (jsonDocument.RootElement.TryGetProperty("userIdentity", out JsonElement uidElement) && uidElement.ValueKind == JsonValueKind.Object)
            {
                userIdentity = JsonSerializer.Deserialize<AuthUserModel>(uidElement.ToString())!;
            }
            else
            {
                throw new Exception("User Identity Not present in Principal");
            }
            if (jsonDocument.RootElement.TryGetProperty("claims", out JsonElement claimsElement) && claimsElement.ValueKind == JsonValueKind.Array)
            {
                claims = JsonSerializer.Deserialize<List<KeyValuePair<string, string>>>(claimsElement)!;
            }
            else
            {
                throw new Exception("User Claims not present in Principal");
            }

            RideAlongPrincipal principal = new RideAlongPrincipal()
            {
                userIdentity = userIdentity,
                claims = claims
            };
            #endregion

            long aud = principal.userIdentity.UID!;

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_rideAlongSecretKey)),
                ValidateIssuer = true,
                ValidIssuer = _rideAlongIssuer,
                ValidateAudience = true,
                ValidAudience = aud.ToString(),
            };

            try
            {
                // Trying to validate the refresh token here
                SecurityToken validatedToken;
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(refreshToken, tokenValidationParameters, out validatedToken);
            }
            catch (Exception ex)
            {
                refreshResponse.ErrorMessage = ex.Message;
                return refreshResponse;
            }

            IAppPrincipal userPrincpal = (RideAlongPrincipal)_authService.GetUserPrincipal(principal.userIdentity).ReturnValue!.First()!;

            if (context.Request.Headers["Authorization"].First() is null)
            {
                throw new Exception("No ID Token Found");
            }
            string idToken = context.Request.Headers["Authorization"].First()!.Split(" ").Last();
            var handler2 = new JwtSecurityTokenHandler();
            token = handler2.ReadJwtToken(idToken);
            string auth_time = token.Claims.First(c => c.Type == "auth_time").Value;

            DateTimeOffset timeAuthorized = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToUInt32(auth_time));

            try
            {
                string newIdToken;
                string newAccessToken;
                IResponse newIdResponse = CreateIdToken(userPrincpal, timeAuthorized);
                if (newIdResponse.HasError is true) { throw new Exception(newIdResponse.ErrorMessage); }

                if (newIdResponse.ReturnValue is not null && newIdResponse.ReturnValue.First() is not null && newIdResponse.ReturnValue.First() is string)
                {
                    newIdToken = (string)newIdResponse.ReturnValue.First();
                }
                else { throw new Exception("Could Not refresh ID Token"); }

                IResponse newAccessResponse = CreateAccessToken(userPrincpal);
                if (newAccessResponse.HasError is true) { throw new Exception(newAccessResponse.ErrorMessage); }

                if (newAccessResponse.ReturnValue is not null && newAccessResponse.ReturnValue.First() is not null && newAccessResponse.ReturnValue.First() is string)
                {
                    newAccessToken = (string)newAccessResponse.ReturnValue.First();
                }
                else { throw new Exception("could not refresh Access Token"); }

                List<object> newTokens = [newIdToken, newAccessToken];
                refreshResponse.ReturnValue = newTokens;
                refreshResponse.HasError = false;
            }
            catch (Exception ex)
            {
                if (!ex.Message.IsNullOrEmpty()) { refreshResponse.ErrorMessage = ex.Message; }
                else
                {
                    refreshResponse.ErrorMessage = "Unknown Error happened while creating Tokens";
                }
                return refreshResponse;
            }

            return refreshResponse;
        }

        public IResponse StartLogin(string username)
        {
            _logger.CreateLogAsync("Info", "Business", $"Username {username} has attempted a login", null);

            IResponse LogInAttempt = new Response();

            // Get the user's model to confirm they exist
            IResponse tryGetUserModel = _authService.GetUserModel(username);
            IAuthUserModel model;
            if (!tryGetUserModel.HasError && tryGetUserModel.ReturnValue is not null && tryGetUserModel.ReturnValue.First() is not null)
            {
                model = (AuthUserModel)tryGetUserModel.ReturnValue.First();
            }
            else
            {
                LogInAttempt.ErrorMessage = "This user could not be found";
                if (tryGetUserModel.ErrorMessage.IsNullOrEmpty())
                {
                    LogInAttempt.ErrorMessage += $": {tryGetUserModel.ErrorMessage}";
                }
                return LogInAttempt;
            }

            // Get the user's Princpal to check their permissions
            IResponse tryGetUserPrincipal = _authService.GetUserPrincipal(model);
            IAppPrincipal userPrincipal;
            if (!tryGetUserPrincipal.HasError && tryGetUserPrincipal.ReturnValue is not null && tryGetUserPrincipal.ReturnValue.First() is not null)
            {
                userPrincipal = (RideAlongPrincipal)tryGetUserPrincipal.ReturnValue.First();
            }
            else
            {
                LogInAttempt.ErrorMessage = "The principal could not be retreived";
                if (tryGetUserPrincipal.ErrorMessage.IsNullOrEmpty())
                {
                    LogInAttempt.ErrorMessage += $": {tryGetUserPrincipal.ErrorMessage}";
                }
                return LogInAttempt;
            }

            // Check if user can login
            Dictionary<string, string> loginClaim = new Dictionary<string, string>();
            loginClaim.Add("canLogin", "true");
            if (!_authService.Authorize(userPrincipal, loginClaim))
            {
                LogInAttempt.ErrorMessage = "Login Not Permitted";
                return LogInAttempt;
            }


            // create OTP, hash it and pass it in
            IRandomService randomizer = new RandomService();
            IHashService hasher = new HashService();
            string otp = randomizer.GenerateRandomString(__otpLength);
            string otpHash = hasher.hashUser(otp, BitConverter.ToInt32(model.salt));

            IResponse attemptToStoreHash = _authService.UpdateOtp(model, otpHash);
            if (attemptToStoreHash.HasError is true)
            {
                LogInAttempt.ErrorMessage = "Could not store OTP";
                return LogInAttempt;
            }

            // For A Development Environment Only
            LogInAttempt.ReturnValue = new List<object>();
            LogInAttempt.ReturnValue.Add(otp);
            // End

            // If we got here, then all was completed successfully. Set error to false, return response
            LogInAttempt.HasError = false;
            return LogInAttempt;
        }

        //Returns a response object with 3 tokens [ID, Access, Refresh]
        public IResponse TryAuthenticating(AuthNRequest loginRequest)
        {
            string username = loginRequest.username;
            string otp = loginRequest.otp;

            IResponse LogInAttempt = new Response();
            // Get User model from username
            IResponse tryGetUserModel = _authService.GetUserModel(username);
            IAuthUserModel model;
            if (!tryGetUserModel.HasError && tryGetUserModel.ReturnValue is not null && tryGetUserModel.ReturnValue.First() is not null)
            {
                model = (AuthUserModel)tryGetUserModel.ReturnValue.First();
            }
            else
            {
                LogInAttempt.ErrorMessage = "The user could not be retreived";
                if (tryGetUserModel.ErrorMessage.IsNullOrEmpty())
                {
                    LogInAttempt.ErrorMessage += $": {tryGetUserModel.ErrorMessage}";
                }
                return LogInAttempt;
            }
            // Check amount of login attempts
            #region getting login attempt count
            IResponse tryGettingLoginAttempts = _authService.GetUserLogInAttempts(model);
            int attempts = 0;
            // If 0, then do nothing    
            if (!tryGettingLoginAttempts.HasError && tryGettingLoginAttempts.ReturnValue is not null && tryGettingLoginAttempts.ReturnValue.First() is not null)
            {
                attempts = (int)tryGettingLoginAttempts.ReturnValue.First();
            }
            else
            {
                LogInAttempt.ErrorMessage = "Login Attempts could not be retreived";
                if (tryGetUserModel.ErrorMessage.IsNullOrEmpty())
                {
                    LogInAttempt.ErrorMessage += $": {tryGetUserModel.ErrorMessage}";
                }
                return LogInAttempt;
            }
            #endregion


            #region Validating First failed login, and login attempts
            // Get Last login time
            IResponse tryGetFirstFailed = _authService.GetFirstFailedLogin(model);
            DateTime firstFailed;
            if (!tryGetFirstFailed.HasError && tryGetFirstFailed.ReturnValue is not null)
            {
                firstFailed = (DateTime)tryGetFirstFailed.ReturnValue.First();
            }
            else
            {
                LogInAttempt.ErrorMessage = "The First Failed Login value could not be retreived";
                if (tryGetUserModel.ErrorMessage.IsNullOrEmpty())
                {
                    LogInAttempt.ErrorMessage += $": {tryGetUserModel.ErrorMessage}";
                }
                return LogInAttempt;
            }

            // If 1/2 and first failed login is longer than 24 hours ago, reset tries
            if (attempts < 3 && (firstFailed.AddDays(1) < DateTime.UtcNow))
            {
                IResponse resetTries = _authService.updateLoginAttempt(model, 0);
            }
            // If 3 or more, then early exit
            else if (attempts >= 3)
            {
                LogInAttempt.ErrorMessage = "Too Many Tries, Please perform account recovery or contact an admin";
                return LogInAttempt;
            }
            #endregion

            // Get OTP hash
            IResponse tryGetOtpHash = _authService.GetOtpHash(model);
            string otpHash;
            if (tryGetOtpHash.ReturnValue is not null && tryGetOtpHash.ReturnValue.First() is not null)
            {
                otpHash = (string)tryGetOtpHash.ReturnValue.First();
            }
            else
            {
                LogInAttempt.ErrorMessage = "Could not get stored passcode";
                return LogInAttempt;
            }

            // Hash current OTP and check if it matches OTPHash
            IHashService hasher = new HashService();
            string hashedUserAttempt = hasher.hashUser(otp, BitConverter.ToInt32(model.salt));

            // If they do not match:
            if (otpHash != hashedUserAttempt)
            {
                // If First failed attempt is longer than 24 hours ago, then update it to now
                if (firstFailed.AddDays(1) < DateTime.UtcNow)
                {
                    IResponse ffResponse = _authService.SetFirstFailedLogin(model, DateTime.UtcNow);
                    if (ffResponse.HasError)
                    {
                        _logger.CreateLogAsync("Warning", "Business", "Could not reset First Failed Login attempt to right now", model.userHash);
                    }
                }
                // increase attempts by 1
                IResponse tryUpdateAttempts = _authService.updateLoginAttempt(model, attempts + 1);
                if (tryUpdateAttempts.HasError)
                {
                    _logger.CreateLogAsync("Warning", "Business", "Could not update attempts", model.userHash);
                }
                // Exit
                LogInAttempt.ErrorMessage = "Incorrect login or password, please try again";
                return LogInAttempt;
            }


            // If they do match
            // Set attempts to 0
            IResponse tryClearAttempts = _authService.updateLoginAttempt(model, 0);
            if (tryClearAttempts.HasError)
            {
                _logger.CreateLogAsync("Warning", "Business", "Could not reset Login attempts upon successful login", model.userHash);
            }

            // Get user Principal
            IResponse tryGetUserPrincipal = _authService.GetUserPrincipal(model);
            IAppPrincipal userPrincipal;
            if (!tryGetUserPrincipal.HasError && tryGetUserPrincipal.ReturnValue is not null && tryGetUserPrincipal.ReturnValue.First() is not null)
            {
                userPrincipal = (RideAlongPrincipal)tryGetUserPrincipal.ReturnValue.First();
            }
            else
            {
                LogInAttempt.ErrorMessage = "The principal could not be retreived";
                if (tryGetUserPrincipal.ErrorMessage.IsNullOrEmpty())
                {
                    LogInAttempt.ErrorMessage += $": {tryGetUserPrincipal.ErrorMessage}";
                }
                return LogInAttempt;
            }

            // create Tokens
            List<object> tokens = new List<object>();
            IResponse tryGetIDToken = CreateIdToken(userPrincipal, DateTime.UtcNow);
            if (tryGetIDToken.HasError)
            {
                LogInAttempt.ErrorMessage = "Could not make Id Token";
                return LogInAttempt;
            }

            IResponse tryGetAccessToken = CreateAccessToken(userPrincipal);
            if (tryGetAccessToken.HasError)
            {
                LogInAttempt.ErrorMessage = "Could not make Access Token";
                return LogInAttempt;
            }

            IResponse tryGetRefreshToken = CreateRefreshToken(userPrincipal);
            if (tryGetRefreshToken.HasError)
            {
                LogInAttempt.ErrorMessage = "Could not make Refresh Token";
                return LogInAttempt;
            }

            if (tryGetIDToken.ReturnValue is not null) { tokens.Add(tryGetIDToken.ReturnValue.First()); }
            if (tryGetAccessToken.ReturnValue is not null) { tokens.Add(tryGetAccessToken.ReturnValue.First()); }
            if (tryGetRefreshToken.ReturnValue is not null) { tokens.Add(tryGetRefreshToken.ReturnValue.First()); }


            // Return all true
            LogInAttempt.ReturnValue = tokens;
            LogInAttempt.HasError = false;
            return LogInAttempt;
        }
    }
}
