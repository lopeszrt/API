using API.Services;
using API.Structure;
using API.Tables;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProgrammingLanguageController : Controller, IController<ProgrammingLanguage>
    {
        private readonly DatabaseCalls _db;
        private const string ProgrammingLanguageTable = "ProgrammingLanguage";

        public ProgrammingLanguageController(DatabaseCalls db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProgrammingLanguage item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@UserProfileId", item.UserProfileId },
                { "@Proficiency", item.Proficiency },
                { "@Project_Id", item.ProjectId },
                { "@Skill_Id", item.SkillId }
            };

            var success = await _db.InsertAsync("ProgrammingLanguage", data);

            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add programming language." });
            }
            item.Id = Convert.ToInt32(success);
            return Ok(new { message = "Created Programming Language", data = item });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _db.DeleteAsync(ProgrammingLanguageTable, id.ToString());
            if (!success)
            {
                return NotFound(new { error = $"Programming Language with ID {id} not found." });
            }
            return Ok(new { message = $"Programming Language with ID {id} was deleted." });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var table = await _db.GetFromTableAsync(ProgrammingLanguageTable);
            return Ok(new { data = (from DataRow row in table.Rows select ProgrammingLanguage.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("profile/{profileId}")]
        public async Task<IActionResult> GetByForeignId(int profileId)
        {
            var data = new Dictionary<string, object>
            {
                { "UserProfileId", profileId }
            };

            var table = await _db.GetFromTableFilteredAsync(ProgrammingLanguageTable, data);
            if (table.Rows.Count == 0)
            {
                return NotFound(new { error = $"No programming languages found for profile ID {profileId}." });
            }
            return Ok(new { data = (from DataRow row in table.Rows select ProgrammingLanguage.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _db.GetFromTableAsync(ProgrammingLanguageTable, id.ToString());
            if (result.Rows.Count == 0)
            {
                return NotFound(new { error = $"Programming Language with ID {id} not found." });
            }

            return Ok(new { data = ProgrammingLanguage.CreateFromDataRow(result.Rows[0]) });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProgrammingLanguage item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@Proficiency", item.Proficiency },
                { "@Project_Id", item.ProjectId },
                { "@Skill_Id", item.SkillId }
            };

            var success = await _db.UpdateAsync(ProgrammingLanguageTable, item.Id.ToString(), data);
            if (!success)
            {
                return NotFound(new { error = $"Programming Language with ID {item.Id} not found." });
            }
            return Ok(new { message = $"Programming Language with ID {item.Id} was updated", data = item });
        }
    }
}