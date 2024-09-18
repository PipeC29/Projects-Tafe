using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookStoreApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        [HttpPost("SetTheme")]
        public async Task<IActionResult> SetTheme([FromBody]ThemeSettings setting)
        {
            HttpContext.Session.SetString("Theme", setting.Theme);
            return Ok();
        }

        public class ThemeSettings
        {
            public string Theme { get; set; }
        }
    }
}
