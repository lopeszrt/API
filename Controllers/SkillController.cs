using API.Services;
using API.Structure;
using API.Tables;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SkillController : Controller, IController<SkillRequest>
    {
        private readonly DatabaseCalls _db;

        public SkillController(DatabaseCalls db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SkillRequest item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@UserProfileId", item.UserProfileId },
                { "@Description", item.Description ?? (object) DBNull.Value }
            };

            var success = await _db.InsertAsync(TableName.Skill, data);
            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add skill." });
            }
            int newId = Convert.ToInt32(success);
            var createdItem = await _db.GetFromTableAsync(TableName.Skill, newId.ToString());
            return CreatedAtAction(nameof(GetById),new {id=newId}, createdItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid ID." });
            }

            var success = await _db.DeleteAsync(TableName.Skill, id.ToString());
            if (!success)
            {
                return NotFound(new { error = $"Skill with ID {id} not found." });
            }
            return Ok(new { message = $"Skill with ID {id} was deleted." });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var table = await _db.GetFromTableAsync(TableName.Skill);
            return Ok(new { data = (from DataRow row in table.Rows select Skill.CreateFromDataRow(row)).ToList() });
        }
        [HttpGet("profile/{foreignId}")]
        public async Task<IActionResult> GetByForeignId(int foreignId)
        {
            var data = new Dictionary<string, object>
            {
                { "UserProfileId", foreignId }
            };
            var res = await _db.GetFromTableFilteredAsync(TableName.Skill, data);
            if (res.Rows.Count == 0)
            {
                return NotFound(new { error = $"No skills found for User Profile ID {foreignId}." });
            }
            return Ok(new { data = (from DataRow row in res.Rows select Skill.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid ID." });
            }
            var table = await _db.GetFromTableAsync(TableName.Skill, id.ToString());
            if (table.Rows.Count == 0)
            {
                return NotFound(new { error = $"Skill with ID {id} not found." });
            }
            return Ok(new { data = Skill.CreateFromDataRow(table.Rows[0]) });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,[FromBody] SkillRequest item)
        {
            if (id <= 0 || !ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid ID or model state.", ModelState });
            }
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@Description", item.Description ?? (object) DBNull.Value }
            };

            var success = await _db.UpdateAsync(TableName.Skill, id.ToString(), data);
            if (!success)
            {
                return NotFound(new { error = $"Skill with ID {id} not found." });
            }
            var updatedItem = await _db.GetFromTableAsync(TableName.Skill, id.ToString());
            return Ok(new { message = "Updated Skill", data = updatedItem });
        }
    }
}