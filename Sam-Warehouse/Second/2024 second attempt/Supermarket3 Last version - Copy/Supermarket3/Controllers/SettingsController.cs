using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Supermarket3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        [HttpPost("SetTheme")]
        public IActionResult SetTheme([FromBody] UserSettings settings)
        {
            HttpContext.Session.SetString("Theme", settings.Theme);
            return Ok();
        }

        public class UserSettings
        {
            public string Theme { get; set; } = "Primary";
        }
    }
}
