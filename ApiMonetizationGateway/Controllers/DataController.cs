using Microsoft.AspNetCore.Mvc;

namespace ApiMonetizationGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetData()
        {
            return Ok(new { message = "Access Granted: This is the protected data." });
        }
    }
}