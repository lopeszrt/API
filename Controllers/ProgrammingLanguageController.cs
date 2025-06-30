using API.Models;
using API.Services;
using API.Structure;
using API.Tables;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProgrammingLanguageController : Controller, IController<ProgrammingLanguageRequest>
    {
        private readonly DatabaseCalls _db;
        private const string ProgrammingLanguageTable = "ProgrammingLanguage";

        public ProgrammingLanguageController(DatabaseCalls db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProgrammingLanguageRequest item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@UserProfileId", item.UserProfileId },
                { "@Proficiency", item.Proficiency },
                { "@Project_Id", item.ProjectId ?? (object) DBNull.Value},
                { "@Skill_Id", item.SkillId ?? (object) DBNull.Value }
            };

            var success = await _db.InsertAsync("ProgrammingLanguage", data);

            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add programming language." });
            }
            int newId = Convert.ToInt32(success);
            var createdItem = await _db.GetFromTableAsync(ProgrammingLanguageTable, newId.ToString());
            return CreatedAtAction(nameof(GetById), new {id = newId}, createdItem);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,[FromBody] ProgrammingLanguageRequest item)
        {
            if (id <= 0 || !ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid ID or model state." , ModelState});
            }
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@Proficiency", item.Proficiency },
                { "@Project_Id", item.ProjectId ??(object) DBNull.Value },
                { "@Skill_Id", item.SkillId ??(object) DBNull.Value }
            };

            var success = await _db.UpdateAsync(ProgrammingLanguageTable, id.ToString(), data);
            if (!success)
            {
                return NotFound(new { error = $"Programming Language with ID {id} not found." });
            }
            var updatedItem = await _db.GetFromTableAsync(ProgrammingLanguageTable, id.ToString());

            return Ok(new { message = $"Programming Language with ID {id} was updated", data = updatedItem });
        }
    }
}