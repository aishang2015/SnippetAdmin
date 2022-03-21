using Microsoft.AspNetCore.Mvc;

namespace SnippetAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class HealthController : ControllerBase
    {
        [HttpGet("apihealth")]
        public IActionResult GetApiHealth()
        {
            return Ok("ok");
        }
    }
}
