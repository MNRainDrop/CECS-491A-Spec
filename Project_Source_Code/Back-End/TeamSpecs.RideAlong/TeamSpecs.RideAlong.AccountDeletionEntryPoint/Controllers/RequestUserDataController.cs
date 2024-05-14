using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamSpecs.RideAlong.AccountDeletionEntryPoint.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class RequestUserDataController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly IAccountRetrievalManager _manager;
        private readonly IAccountUpdateManager _updateManager;
        private readonly ISecurityManager _securityManager;
        private readonly IAccountClaimManager _accountClaimManager;
        public RequestUserDataController(ILogService logService, IAccountRetrievalManager accountRetrievalManager, ISecurityManager securityManager, IAccountUpdateManager updateManager, IAccountClaimManager accountClaimManager)
        {
            _logService = logService;
            _manager = accountRetrievalManager;
            _securityManager = securityManager;
            _updateManager = updateManager;
            _accountClaimManager = accountClaimManager;
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

        [HttpPost]
        [Route("DisableUserAccount")]
        public IActionResult DisableUserAccount()
        {
            IAppPrincipal principal = _securityManager.JwtToPrincipal();
            var UID = principal.userIdentity.UID;
            var name = principal.userIdentity.userName;
            var salt = BitConverter.ToUInt32(principal.userIdentity.salt);
            IResponse response;
            if (name is not null)
            {
                IAccountUserModel userAccount = new AccountUserModel(name)
                {
                    UserId = UID,
                    Salt = salt,
                    UserHash = principal.userIdentity.userHash
                };
                try
                {
                    response = _accountClaimManager.DisableUser(userAccount);
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
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("EnableUserListTupplePassedIn")]
        public IActionResult EnableUserListTupplePassedIn([FromBody]string Claim, string Scope)
        {
            IAppPrincipal principal = _securityManager.JwtToPrincipal();
            var UID = principal.userIdentity.UID;
            var name = principal.userIdentity.userName;
            var salt = BitConverter.ToUInt32(principal.userIdentity.salt);
            IList<Tuple<string, string>> tupleList = new List<Tuple<string, string>>();
            tupleList.Add(Tuple.Create(Claim, Scope));

            IResponse response;
            if (name is not null)
            {
                IAccountUserModel userAccount = new AccountUserModel(name)
                {
                    UserId = UID,
                    Salt = salt,
                    UserHash = principal.userIdentity.userHash
                };
                try
                {
                    response = _accountClaimManager.CreateUserClaim(userAccount,tupleList);
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
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("EnableUserCollectionKPPassedIn")]
        public IActionResult EnableUserCollectionKPPassedIn([FromBody] string Claim, string Scope)
        {
            IAppPrincipal principal = _securityManager.JwtToPrincipal();
            var UID = principal.userIdentity.UID;
            var name = principal.userIdentity.userName;
            var salt = BitConverter.ToUInt32(principal.userIdentity.salt);
            ICollection<KeyValuePair<string, string>> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add(new KeyValuePair<string, string>(Claim, Scope));

            IResponse response;
            if (name is not null)
            {
                IAccountUserModel userAccount = new AccountUserModel(name)
                {
                    UserId = UID,
                    Salt = salt,
                    UserHash = principal.userIdentity.userHash
                };
                try
                {
                    response = _accountClaimManager.CreateUserClaim(userAccount, keyValuePairs);
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
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        [Route("UpdateUserAccount")]
        public IActionResult UpdateUserAccount([FromBody] UserUpdateRequest request)
        {
            IAppPrincipal principal = _securityManager.JwtToPrincipal();
            var UID = long.Parse(request.uid);
            var name = principal.userIdentity.userName;
            var salt = BitConverter.ToUInt32(principal.userIdentity.salt);
            IResponse response;
            if (request is not null)
            {
                IAccountUserModel userAccount = new AccountUserModel(request.userName)
                {
                    UserId = long.Parse(request.uid),
                    Salt = 0,
                    UserHash = request.hash
                };
                try
                {
                    response = _updateManager.PostAccountUpdate(userAccount, request.address, request.name, request.phone, request.accountType);
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
            else
            {
                return NotFound();
            }
        }

        public class UserUpdateRequest
        {
            public required string userName { get; set; }
            public required string uid { get; set; }
            public required uint salt { get; set; }
            public required string hash { get; set; }    
            public required string address { get; set; }
            public required string name { get; set; }
            public required string phone { get; set; }
            public required string accountType { get; set; }
        }

    }
}
