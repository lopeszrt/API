using API.Services;
using API.Structure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class GeneralController : Controller
    {
        private readonly Database _db;
        private readonly DatabaseCalls _dbCalls;
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Pong");
        }

        [HttpGet("health")]
        public async Task<IActionResult> HealthCheck()
        {
            if (!await _db.TestConnectionAsync())
            {
                return StatusCode(503, new { status = "Unhealthy", message = "Database connection failed." });
            }
            return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var res = new
            {
                users = Auxiliar.DataTableToList(await _dbCalls.GetFromTableAsync(TableName.User)),
                userProfiles = Auxiliar.DataTableToList(await _dbCalls.GetFromTableAsync(TableName.UserProfile)),
                skills = Auxiliar.DataTableToList(await _dbCalls.GetFromTableAsync(TableName.Skill)),
                hobbies = Auxiliar.DataTableToList(await _dbCalls.GetFromTableAsync(TableName.Hobby)),
                educations = Auxiliar.DataTableToList(await _dbCalls.GetFromTableAsync(TableName.Education)),
                jobExperiences = Auxiliar.DataTableToList(await _dbCalls.GetFromTableAsync(TableName.JobExperience)),
                languages = Auxiliar.DataTableToList(await _dbCalls.GetFromTableAsync(TableName.Language)),
                projects = Auxiliar.DataTableToList(await _dbCalls.GetFromTableAsync(TableName.Project)),
                programmingLanguages = Auxiliar.DataTableToList(await _dbCalls.GetFromTableAsync(TableName.ProgrammingLanguage))
            };
            return Ok(new
            {
                data = res
            });
        }

        GeneralController(Database db, JwtService jwt, DatabaseCalls dbCalls)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db), "Database service is required.");
            _dbCalls = dbCalls ?? throw new ArgumentNullException(nameof(dbCalls), "Database calls service is required.");
        }
    }
}