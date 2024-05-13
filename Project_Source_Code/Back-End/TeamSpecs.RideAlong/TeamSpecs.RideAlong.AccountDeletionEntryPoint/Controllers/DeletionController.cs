using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.UserAdministration.Managers;
using TeamSpecs.RideAlong.Model;
using System.Collections;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamSpecs.RideAlong.AccountDeletionEntryPoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DeletionController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly IAccountDeletionManager _accountDeletionManager;
        private readonly ISecurityManager _securityManager;

        public DeletionController(ILogService logService, IAccountDeletionManager accountDeletionManager, ISecurityManager securityManager)
        {
            _logService = logService;
            _accountDeletionManager = accountDeletionManager;
            _securityManager = securityManager;
        }

        [HttpPost]
        [Route("DeleteMyAccount")]
        public IActionResult DeleteUsersAccount() // change so user is not needed
        {
            IResponse response = new Response();
            IAccountUserModel user = new AccountUserModel("temp");
            IAppPrincipal principal = _securityManager.JwtToPrincipal();

            if (principal.userIdentity.userName is not null)
            {
                user.UserName = principal.userIdentity.userName;
                user.UserId = principal.userIdentity.UID;
                user.Salt = BitConverter.ToUInt32(principal.userIdentity.salt); 
                user.UserHash = principal.userIdentity.userHash;
            }

            response = _accountDeletionManager.DeleteUser(user);

            if (response.ErrorMessage is not null)
            {
                if (response.HasError && response.ErrorMessage.Contains("Could not"))
                {
                    return StatusCode(500);
                }
                else
                {
                    return BadRequest(response.ErrorMessage);
                }
            }

            return Ok("User deleted successfully!");
        }
    }
}
