using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Formats.Asn1;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortofolioController : Controller
    {
        private readonly DatabaseCalls _db;


        [AllowAnonymous]
        [HttpGet("route")]
        public async Task<IActionResult> GetProfileId(string route)
        {
            if (string.IsNullOrEmpty(route))
            {
                return BadRequest(new { error = "Route cannot be null or empty." });
            }
            var data = new Dictionary<string, object>
            {
                { "Route", route }
            };
            var res = await _db.GetFromTableFilteredAsync("User_Profile", data);
            if (res == null || res.Rows.Count == 0)
            {
                return NotFound(new { error = "Profile not found." });
            }

            var profileId = Convert.ToInt64(res.Rows[0]["Id"]);

            if (profileId <= 0)
            {
                return NotFound(new { error = "Profile not found." });
            }
            return Ok(new { profileId });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GetPortofolio([FromBody] string profileId)
        {
            if (string.IsNullOrEmpty(profileId))
                return BadRequest(new { error = "Profile ID cannot be null or empty." });

            var data = new Dictionary<string, object> { { "User_Profile_Id", profileId } };

            var res = new
            {
                profile = Auxiliar.FirstRowToDictionary(await _db.GetFromTableAsync("User_Profile", profileId)),
                educations = Auxiliar.DataTableToList(await _db.GetFromTableFilteredAsync("Education", data)),
                hobbies = Auxiliar.DataTableToList(await _db.GetFromTableFilteredAsync("Hobby", data)),
                skills = Auxiliar.DataTableToList(await _db.GetFromTableFilteredAsync("Skill", data)),
                jobExperience = Auxiliar.DataTableToList(await _db.GetFromTableFilteredAsync("JobExperience", data)),
                languages = Auxiliar.DataTableToList(await _db.GetFromTableFilteredAsync("Language", data)),
                projects = Auxiliar.DataTableToList(await _db.GetFromTableFilteredAsync("Project", data)),
                programmingLanguages = Auxiliar.DataTableToList(await _db.GetFromTableFilteredAsync("ProgrammingLanguage", data))
            };

            return Ok(new { data = res });
        }


        public PortofolioController(DatabaseCalls db)
        {
            _db = db;
        }

    }
}
