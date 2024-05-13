using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary.Model;

namespace TeamSpecs.RideAlong.SecurityEndPoint.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly ISecurityManager _securityManager;
        public AuthController(ISecurityManager securityManager)
        {
            _securityManager = securityManager;
        }
        [HttpPost]
        [Route("startLogin")]
        public IActionResult StartLogin([FromBody] string username)
        {
            IResponse startLoginResponse;
            using var source = new CancellationTokenSource();

            // Run asynchronous task for login
            Task<IResponse> startLoginTask = Task.Run(() => _securityManager.StartLogin(username), source.Token);

            // Stop the task if it takes to long
            source.CancelAfter(5000);
            try
            {
                // Successful Execution Stored into response
                startLoginTask.Wait();
                startLoginResponse = startLoginTask.Result;
            }
            catch (Exception ex)
            {
                // Handles failure if it takes too long
                return StatusCode(500, "Start Login took Longer than 5 Seconds");
            }

            // Validate Response
            if (startLoginResponse.HasError)
            {
                return BadRequest(startLoginResponse.ErrorMessage);
            }
            if (startLoginResponse.ReturnValue is not null)
            {
                return Ok("OTP Generated! " + (string)startLoginResponse.ReturnValue.First());
            }
            return Ok("OTP Generated!");
        }
        [HttpPost]
        [Route("tryAuthentication")]
        public IActionResult tryAuthentication([FromBody] AuthNRequest loginRequest)
        {
            IResponse tryAuthentication;

            #region Creates cancellable async task for `TryAuthenticating`
            using var source = new CancellationTokenSource();
            Task<IResponse> tryAuthTask = Task.Run(() => _securityManager.TryAuthenticating(loginRequest), source.Token);
            source.CancelAfter(5000);
            try
            {
                tryAuthTask.Wait();
                tryAuthentication = tryAuthTask.Result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Start Login took Longer than 5 Seconds");
            }
            #endregion

            if (tryAuthentication.HasError)
            {
                return BadRequest(tryAuthentication.ErrorMessage);
            }
            if (tryAuthentication.ReturnValue is not null)
            {
                List<object> tokenList = tryAuthentication.ReturnValue.ToList();
                var idToken = tokenList[0];
                var accessToken = tokenList[1];
                var refreshToken = tokenList[2];
                return Ok(new { IdToken = idToken, AccessToken = accessToken, RefreshToken = refreshToken });
            }
            return Ok("");
        }
        [HttpPost]
        [Route("refreshTokens")]
        public IActionResult PostRefreshTokens()
        {
            IResponse tokenRefreshResponse;

            #region Creates cancellable async task for `RefreshTokens()`
            using var source = new CancellationTokenSource();
            Task<IResponse> refreshTokensTask = Task.Run(() => _securityManager.RefreshTokens(), source.Token);
            source.CancelAfter(5000);
            try
            {
                refreshTokensTask.Wait();
                tokenRefreshResponse = refreshTokensTask.Result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Start Login took Longer than 5 Seconds");
            }
            #endregion

            if (tokenRefreshResponse.HasError)
            {
                return BadRequest("Error creating tokens: " + tokenRefreshResponse.ErrorMessage);
            }
            else
            {
                try
                {
                    if (tokenRefreshResponse.ReturnValue is not null && tokenRefreshResponse.ReturnValue.First() is not null && tokenRefreshResponse.ReturnValue.ElementAt(1) is not null)
                    {
                        List<object> tokenList = tokenRefreshResponse.ReturnValue.ToList();
                        return Ok(new { idToken = tokenList[0], accessToken = tokenList[1] });
                    }
                }
                catch
                {
                    return BadRequest("Tokens Failed To generate");
                }
                ;
            }
            return BadRequest("Token Refresh Failed!");
        }
    }
}
