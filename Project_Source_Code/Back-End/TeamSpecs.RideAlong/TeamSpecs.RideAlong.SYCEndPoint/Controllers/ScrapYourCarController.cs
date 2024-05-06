using Microsoft.AspNetCore.Mvc;
using ScrapYourCarLibrary.Interfaces;
using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

namespace TeamSpecs.RideAlong.SYCEndPoint.Controllers
{
    [Route("[controller]")]
    public class ScrapYourCarController : Controller
    {
        IScrapYourCarManager _scrapYourCarManager;
        ISecurityManager _securityManager;
        public ScrapYourCarController(IScrapYourCarManager scrapYourCarManager, ISecurityManager securityManager)
        {
            _scrapYourCarManager = scrapYourCarManager;
            _securityManager = securityManager;
        }
        //[HttpPost]
        //[Route("RouteName")]
        public IActionResult Function([FromBody] IBuyRequest Request)
        {
            // TODO: Implement Crud for Endpoints
            return Ok();
        }
    }
}
