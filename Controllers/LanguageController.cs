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
    public class LanguageController : Controller, IController<Language>
    {
        private readonly DatabaseCalls _db;
        private const string LanguageTable = "Language";

        public LanguageController(DatabaseCalls db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Language item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@UserProfileId", item.UserProfileId },
                { "@Proficiency", item.Proficiency},
            };

            var success = await _db.InsertAsync(LanguageTable, data);
            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add language." });
            }
            item.Id = Convert.ToInt32(success);
            return Ok(new { message = "Created Language", data = item });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _db.DeleteAsync(LanguageTable, id.ToString());
            if (!success)
            {
                return NotFound(new { error = $"Language with ID {id} not found." });
            }
            return Ok(new { message = $"Language with ID {id} was deleted." });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var table = await _db.GetFromTableAsync(LanguageTable);
            return Ok(new { data = (from DataRow row in table.Rows select Language.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("profile/{foreignId}")]
        public async Task<IActionResult> GetByForeignId(int foreignId)
        {
            var data = new Dictionary<string, object>
            {
                { "UserProfileId", foreignId }
            };

            var res = await _db.GetFromTableFilteredAsync(LanguageTable, data);

            if (res.Rows.Count == 0)
            {
                return NotFound(new { error = "No languages found for the specified profile." });
            }

            return Ok(new { data = (from DataRow row in res.Rows select Language.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var table = await _db.GetFromTableAsync(LanguageTable, id.ToString());
            if (table.Rows.Count == 0)
            {
                return NotFound(new { error = $"Language with ID {id} not found." });
            }
            return Ok(new { data = Language.CreateFromDataRow(table.Rows[0]) });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Language item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@Proficiency", item.Proficiency }
            };
            var success = await _db.UpdateAsync(LanguageTable, item.Id.ToString(), data);
            if (!success)
            {
                return NotFound(new { error = $"Language with ID {item.Id} not found." });
            }
            return Ok(new { message = "Language updated successfully.", data = item });
        }
    }
}