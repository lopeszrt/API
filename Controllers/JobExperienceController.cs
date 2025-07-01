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
    public class JobExperienceController : Controller, IController<JobExperienceRequest>
    {
        private readonly DatabaseCalls _db;

        public JobExperienceController(DatabaseCalls db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] JobExperienceRequest item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Company", item.Company },
                { "@Title", item.Title },
                { "@Description", item.Description },
                { "@StartDate", item.StartDate },
                { "@EndDate", item.EndDate ?? (object) DBNull.Value},
                { "@UserProfileId", item.UserProfileId }
            };
            var success = await _db.InsertAsync(TableName.JobExperience, data);
            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add job experience." });
            }
            int newId = Convert.ToInt32(success);
            var createdItem = await _db.GetFromTableAsync(TableName.JobExperience, newId.ToString());
            return CreatedAtAction(nameof(GetById), new {id = newId}, createdItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _db.DeleteAsync(TableName.JobExperience, id.ToString());
            if (!result)
            {
                return NotFound(new { error = $"Job Experience with ID {id} not found." });
            }
            return Ok(new { message = $"Job Experience with ID {id} was deleted." });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _db.GetFromTableAsync(TableName.JobExperience);
            return Ok(new { data = (from DataRow row in result.Rows select JobExperience.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("profile/{profileId}")]
        public async Task<IActionResult> GetByForeignId(int profileId)
        {
            var data = new Dictionary<string, object>
            {
                { "UserProfileId", profileId }
            };
            var result = await _db.GetFromTableFilteredAsync(TableName.JobExperience, data);
            if (result.Rows.Count == 0)
            {
                return NotFound(new { error = $"No job experiences found for profile ID {profileId}." });
            }
            return Ok(new { data = (from DataRow row in result.Rows select JobExperience.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid ID." });
            }
            var table = await _db.GetFromTableAsync(TableName.JobExperience, id.ToString());
            if (table.Rows.Count == 0)
            {
                return NotFound(new { error = $"Job Experience with ID {id} not found." });
            }
            return Ok(new { data = JobExperience.CreateFromDataRow(table.Rows[0]) });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,[FromBody] JobExperienceRequest item)
        {
            if ( id<= 0 || !ModelState.IsValid) 
            {
                return BadRequest(new { error = "Invalid ID or model state.", ModelState });
            }
            var data = new Dictionary<string, object>
            {
                { "@Company", item.Company },
                { "@Title", item.Title },
                { "@Description", item.Description },
                { "@StartDate", item.StartDate },
                { "@EndDate", item.EndDate ?? (object)DBNull.Value}
            };

            var success = await _db.UpdateAsync(TableName.JobExperience, id.ToString(), data);
            if (!success)
            {
                return NotFound(new { error = $"Job Experience with ID {id} not found." });
            }

            var updatedItem = await _db.GetFromTableAsync(TableName.JobExperience, id.ToString());

            return Ok(new { message = $"Job Experience with ID {id} was updated.", data = updatedItem });
        }
    }
}