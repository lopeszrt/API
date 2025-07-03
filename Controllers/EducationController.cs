using API.Services;
using API.Structure;
using API.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using API.Filters;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [CheckUserIdFilter(TableName.Education)]
    public class EducationController : Controller, IController<EducationRequest>
    {
        private readonly DatabaseCalls _db;

        public EducationController(DatabaseCalls db)
        {
            _db = db;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var table = await _db.GetFromTableAsync(TableName.Education);
            return Ok(new { data = (from DataRow row in table.Rows select Education.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return NotFound(new { error = "Invalid ID." });
            }
            var table = await _db.GetFromTableAsync(TableName.Education, id.ToString());
            if (table.Rows.Count == 0)
            {
                return NotFound(new { error = $"Education with ID {id} not found." });
            }
            return Ok(new { data = Education.CreateFromDataRow(table.Rows[0]) });
        }

        [HttpGet("profile/{profileId}")]
        public async Task<IActionResult> GetByForeignId(int profileId)
        {
            if (profileId <= 0)
            {
                return BadRequest(new { error = "Profile ID must be a positive integer." });
            }

            var data = new Dictionary<string, object>
            {
                { "UserProfileId", profileId }
            };

            var table = await _db.GetFromTableFilteredAsync(TableName.Education, data);
            if (table.Rows.Count == 0)
            {
                return NotFound(new { error = $"No educations found for profile ID {profileId}." });
            }

            return Ok(new { data = (from DataRow row in table.Rows select Education.CreateFromDataRow(row)).ToList() });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EducationRequest item)
        {
            if (id <= 0 || !ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid ID or model state.", ModelState });
            }

            var data = new Dictionary<string, object>
            {
                { "@Degree", item.Degree },
                { "@Institution", item.Institution },
                { "@Description", item.Description },
                { "@StartDate", item.StartDate },
                { "@EndDate", item.EndDate ?? (object)DBNull.Value}
            };

            var success = await _db.UpdateAsync(TableName.Education, id.ToString(), data);

            if (!success)
            {
                return NotFound(new { error = $"Education with ID {id} not found." });
            }

            var updatedItem = await _db.GetFromTableAsync(TableName.Education, id.ToString());

            return Ok(new { message = $"Education with {id} was updated", data = updatedItem });
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] EducationRequest item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var data = new Dictionary<string, object>
            {
                { "@Degree", item.Degree },
                { "@Institution", item.Institution },
                { "@Description", item.Description },
                { "@UserProfileId", item.UserProfileId },
                { "@StartDate", item.StartDate },
                { "@EndDate", item.EndDate ?? "" }
            };

            var success = await _db.InsertAsync(TableName.Education, data);
            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add education" });
            }
            int newId = Convert.ToInt32(success);
            var createdItem = await _db.GetFromTableAsync(TableName.Education, newId.ToString());
            return CreatedAtAction(nameof(GetById), new {id = newId}, createdItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _db.DeleteAsync(TableName.Education, id.ToString());
            if (!success)
            {
                return NotFound(new { error = $"Education with ID {id} not found." });
            }
            return Ok(new { message = $"Education with ID {id} has been deleted." });
        }
    }
}