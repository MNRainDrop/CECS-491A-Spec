using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.UserAdministration.Managers;
using TeamSpecs.RideAlong.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamSpecs.RideAlong.AccountDeletionEntryPoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DeletionController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly AccountDeletionManager _accountDeletionManager;
        private readonly ISecurityManager _securityManager;

        public DeletionController(ILogService logService, AccountDeletionManager accountDeletionManager, ISecurityManager securityManager)
        {
            _logService = logService;
            _accountDeletionManager = accountDeletionManager;
            _securityManager = securityManager;
        }

        [HttpDelete]
        [Route("DeleteMyAccount")]
        public IActionResult DeleteUsersAccount() 
        {
            IResponse response = new Response();
            IAccountUserModel user;
            IAppPrincipal principal = _securityManager.JwtToPrincipal();

            if (principal.userIdentity.userName is not null)
            {
                user = new AccountUserModel(principal.userIdentity.userName);
                user.UserId = principal.userIdentity.UID;
                user.UserHash = principal.userIdentity.userHash;
            }

            //_accountDeletionManager

            return Ok("User deleted successfully!");
        }
    }
}
