using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class PingController : Controller
    {
        public IActionResult Get()
        {
            return Ok("Pong");
        }
    }
}