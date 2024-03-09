using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Managers;
using TeamSpecs.RideAlong.Managers.Interfaces;
using System;

namespace TeamSpecs.RideAlong.EntryPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAdministrationController : ControllerBase
    {
        private readonly IUserAdministrationManager _userManager;

        public UserAdministrationController(IUserAdministrationManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public IActionResult CreateUser(UserCreationRequest request)
        {
            // Call UserManager to create user
            IResponse response = _userManager.RegisterUser(request.UserName, request.DateOfBirth, request.AccountType);

            // Check if user creation was successful
            if (response.HasError)
            {
                return BadRequest();
            }
            else
            {
                
                return Ok(new
                {
                    userName = request.UserName,
                    dateOfBirth = request.DateOfBirth,
                    accountType = request.AccountType
                });
            }
        }
    }

    public class UserCreationRequest
    {
        public string UserName { get; set; }
        public string AccountType { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}

