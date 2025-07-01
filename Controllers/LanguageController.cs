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
    public class LanguageController : Controller, IController<LanguageRequest>
    {
        private readonly DatabaseCalls _db;

        public LanguageController(DatabaseCalls db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] LanguageRequest item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@UserProfileId", item.UserProfileId },
                { "@Proficiency", item.Proficiency},
            };

            var success = await _db.InsertAsync(TableName.Language, data);
            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add language." });
            }
            int newId = Convert.ToInt32(success);
            var createdItem = await _db.GetFromTableAsync(TableName.Language, newId.ToString());
            return CreatedAtAction(nameof(GetById), new {id = newId}, createdItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _db.DeleteAsync(TableName.Language, id.ToString());
            if (!success)
            {
                return NotFound(new { error = $"Language with ID {id} not found." });
            }
            return Ok(new { message = $"Language with ID {id} was deleted." });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var table = await _db.GetFromTableAsync(TableName.Language);
            return Ok(new { data = (from DataRow row in table.Rows select Language.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("profile/{foreignId}")]
        public async Task<IActionResult> GetByForeignId(int foreignId)
        {
            var data = new Dictionary<string, object>
            {
                { "UserProfileId", foreignId }
            };

            var res = await _db.GetFromTableFilteredAsync(TableName.Language, data);

            if (res.Rows.Count == 0)
            {
                return NotFound(new { error = "No languages found for the specified profile." });
            }

            return Ok(new { data = (from DataRow row in res.Rows select Language.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var table = await _db.GetFromTableAsync(TableName.Language, id.ToString());
            if (table.Rows.Count == 0)
            {
                return NotFound(new { error = $"Language with ID {id} not found." });
            }
            return Ok(new { data = Language.CreateFromDataRow(table.Rows[0]) });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,[FromBody] LanguageRequest item)
        {
            if (!ModelState.IsValid || id <= 0)
            {
                return BadRequest(new { error = "Invalid ID or model state.", ModelState });
            }
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@Proficiency", item.Proficiency }
            };
            var success = await _db.UpdateAsync(TableName.Language, id.ToString(), data);
            if (!success)
            {
                return NotFound(new { error = $"Language with ID {id} not found." });
            }
            var updatedItem = await _db.GetFromTableAsync(TableName.Language, id.ToString());
            return Ok(new { message = "Language updated successfully.", data = updatedItem });
        }
    }
}