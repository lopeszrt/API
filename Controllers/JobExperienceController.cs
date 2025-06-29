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
    public class JobExperienceController : Controller, IController<JobExperience>
    {
        private readonly DatabaseCalls _db;
        private const string JobExperienceTable = "JobExperience";

        public JobExperienceController(DatabaseCalls db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] JobExperience item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Company", item.Company },
                { "@Title", item.Title },
                { "@Description", item.Description },
                { "@Recommendation", item.Recommendation},
                { "@StartDate", item.StartDate },
                { "@EndDate", item.EndDate},
                { "@UserProfileId", item.UserProfileId }
            };
            var success = await _db.InsertAsync(JobExperienceTable, data);
            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add job experience." });
            }
            item.Id = Convert.ToInt32(success);
            return Ok(new { message = "Created Job Experience", data = item });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _db.DeleteAsync(JobExperienceTable, id.ToString());
            if (!result)
            {
                return NotFound(new { error = $"Job Experience with ID {id} not found." });
            }
            return Ok(new { message = $"Job Experience with ID {id} was deleted." });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _db.GetFromTableAsync(JobExperienceTable);
            return Ok(new { data = (from DataRow row in result.Rows select JobExperience.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("profile/{profileId}")]
        public async Task<IActionResult> GetByForeignId(int profileId)
        {
            var data = new Dictionary<string, object>
            {
                { "UserProfileId", profileId }
            };
            var result = await _db.GetFromTableFilteredAsync(JobExperienceTable, data);
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
            var table = await _db.GetFromTableAsync(JobExperienceTable, id.ToString());
            if (table.Rows.Count == 0)
            {
                return NotFound(new { error = $"Job Experience with ID {id} not found." });
            }
            return Ok(new { data = JobExperience.CreateFromDataRow(table.Rows[0]) });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] JobExperience item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Company", item.Company },
                { "@Title", item.Title },
                { "@Description", item.Description },
                { "@Recommendation", item.Recommendation },
                { "@StartDate", item.StartDate },
                { "@EndDate", item.EndDate }
            };

            var success = await _db.UpdateAsync(JobExperienceTable, item.Id.ToString(), data);
            if (!success)
            {
                return NotFound(new { error = $"Job Experience with ID {item.Id} not found." });
            }

            return Ok(new { message = $"Job Experience with ID {item.Id} was updated.", data = item });
        }
    }
}