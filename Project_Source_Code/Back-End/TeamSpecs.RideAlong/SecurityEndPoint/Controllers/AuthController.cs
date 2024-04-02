using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary.Model;

namespace TeamSpecs.RideAlong.SecurityEndPoint.Controllers
{
    [Route( "[controller]")]
    public class AuthController: Controller
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
                return Ok(new { IdToken = idToken, AccessToken = accessToken, RefreshToken = refreshToken});
            }
            return Ok("");
        }
    }
}
