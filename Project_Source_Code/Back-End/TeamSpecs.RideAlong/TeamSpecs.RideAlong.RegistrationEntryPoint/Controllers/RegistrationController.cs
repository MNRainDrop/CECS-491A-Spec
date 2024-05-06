﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamSpecs.RideAlong.RegistrationEntryPoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {

        private readonly ILogService _logService;
        private readonly IAccountCreationManager _accountCreationManager;
        private readonly ISecurityManager _securityManager;

        public RegistrationController(ILogService logService, ISecurityManager securityManager, IAccountCreationManager accountCreationManager)
        {
            _accountCreationManager = accountCreationManager;
            _logService = logService;
            _securityManager = securityManager;
        }


        // GET: api/<RegistrationController>
        [HttpGet]
        [Route("GetVerify")]
        public IActionResult GetVerifyUsername(string email) 
        {
            IResponse response = new Response();

            // Will not have security check

            response = _accountCreationManager.CallVerifyUser(email);

            // If returns bad --> user exists OR other issue
            if (response.HasError)
            {
                return BadRequest(response.ErrorMessage);
            }


            // If returns OK --> user updated in Db
            return Ok("User confirmation created successfully!");
        }

    }
}
