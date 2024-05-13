using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamSpecs.RideAlong.AccountDeletionEntryPoint.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class RequestUserDataController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly IAccountRetrievalManager _manager;
        private readonly ISecurityManager _securityManager;
        public RequestUserDataController(ILogService logService, IAccountRetrievalManager accountRetrievalManager, ISecurityManager securityManager)
        {
            _logService = logService;
            _manager = accountRetrievalManager;
            _securityManager = securityManager;
        }


        [HttpPost]
        [Route("UserDataRequest")]
        public IActionResult UserDataRequest()
        {
            IAppPrincipal principal = _securityManager.JwtToPrincipal();
            var UID = principal.userIdentity.UID;
            IResponse response;
            try
            {
                response = _manager.RetrieveAccount(UID);
                if (response is not null)
                {
                    if (response.HasError)
                    {
                        return BadRequest(response.ErrorMessage);
                    }
                    else
                    {
                        return Ok(response.ReturnValue);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPost]
        [Route("RetrieveAllAccount")]
        public IActionResult RetrieveAllAccount()
        {

            IResponse response;
            try
            {
                response = _manager.RetrieveAllAccount();
                if (response is not null)
                {
                    if (response.HasError)
                    {
                        return BadRequest(response.ErrorMessage);
                    }
                    else
                    {
                        return Ok(response.ReturnValue);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}
