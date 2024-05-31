using Microsoft.AspNetCore.Mvc;
using TeamSpecs.RideAlong.Archiving;
using TeamSpecs.RideAlong.Model;

namespace TargetArchivingEntryPoint.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArchivingController : Controller
    {
        private readonly ArchivingService _aService;
        public ArchivingController(ArchivingService aService)
        {
            _aService = aService;
        }

        [HttpPost]
        [Route("storeLogs")]
        public IActionResult Storelogs()
        {
            // Get the file from the request
            IResponse response;
            using (var memoryStream = new MemoryStream())
            {
                Request.Body.CopyTo(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();
                
                // Do the archiving
                response = _aService.Archive(fileBytes);
            }
            if (response.HasError)
            {
                return BadRequest();
            }
            else
            {
                return Ok();
            }
        }
    }
}
