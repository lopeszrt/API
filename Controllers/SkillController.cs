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
    public class SkillController : Controller, IController<Skill>
    {
        private readonly DatabaseCalls _db;
        private const string SkillTable = "Skill";

        public SkillController(DatabaseCalls db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Skill item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@UserProfileId", item.UserProfileId },
                { "@Description", item.Description }
            };

            var success = await _db.InsertAsync(SkillTable, data);
            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add skill." });
            }
            item.Id = Convert.ToInt32(success);
            return Ok(new { message = "Created Skill", data = item });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid ID." });
            }

            var success = await _db.DeleteAsync(SkillTable, id.ToString());
            if (!success)
            {
                return NotFound(new { error = $"Skill with ID {id} not found." });
            }
            return Ok(new { message = $"Skill with ID {id} was deleted." });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var table = await _db.GetFromTableAsync(SkillTable);
            return Ok(new { data = (from DataRow row in table.Rows select Skill.CreateFromDataRow(row)).ToList() });
        }

        public async Task<IActionResult> GetByForeignId(int foreignId)
        {
            var data = new Dictionary<string, object>
            {
                { "UserProfileId", foreignId }
            };
            var res = await _db.GetFromTableFilteredAsync(SkillTable, data);
            if (res.Rows.Count == 0)
            {
                return NotFound(new { error = $"No skills found for User Profile ID {foreignId}." });
            }
            return Ok(new { data = (from DataRow row in res.Rows select Skill.CreateFromDataRow(row)).ToList() });
        }

        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid ID." });
            }
            var table = await _db.GetFromTableAsync(SkillTable, id.ToString());
            if (table.Rows.Count == 0)
            {
                return NotFound(new { error = $"Skill with ID {id} not found." });
            }
            return Ok(new { data = Skill.CreateFromDataRow(table.Rows[0]) });
        }

        public async Task<IActionResult> Update([FromBody] Skill item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@Description", item.Description }
            };

            var success = await _db.UpdateAsync(SkillTable, item.Id.ToString(), data);
            if (!success)
            {
                return NotFound(new { error = $"Skill with ID {item.Id} not found." });
            }
            return Ok(new { message = "Updated Skill", data = item });
        }
    }
}