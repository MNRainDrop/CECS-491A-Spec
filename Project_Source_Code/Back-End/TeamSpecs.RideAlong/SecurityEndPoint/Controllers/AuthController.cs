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
            var startLoginResponse = _securityManager.StartLogin(username);
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
            var tryAuthentication = _securityManager.TryAuthenticating(loginRequest);
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
            IResponse tokenRefreshResponse = _securityManager.RefreshTokens();
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
