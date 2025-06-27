using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class PingController : Controller
    {
        public async Task<IActionResult> Get()
        {
            return Ok("Pong");
        }
    }
}
