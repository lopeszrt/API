using API.Services;
using API.Structure;
using API.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EducationController : Controller, IController<Education>
    {
        private readonly DatabaseCalls _db;
        private const string EducationTable = "Education";

        public EducationController(DatabaseCalls db)
        {
            _db = db;
        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            var table = await _db.GetFromTableAsync(EducationTable);
            return Ok(new { data = (from DataRow row in table.Rows select Education.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid ID." });
            }
            var table = await _db.GetFromTableAsync(EducationTable, id.ToString());
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

            var table = await _db.GetFromTableFilteredAsync(EducationTable, data);
            if (table.Rows.Count == 0)
            {
                return NotFound(new { error = $"No educations found for profile ID {profileId}." });
            }

            return Ok(new { data = (from DataRow row in table.Rows select Education.CreateFromDataRow(row)).ToList() });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] Education item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Degree", item.Degree },
                { "@Institution", item.Institution },
                { "@Description", item.Description },
                { "@StartDate", item.StartDate },
                { "@EndDate", item.EndDate ?? (object)DBNull.Value}
            };

            var success = await _db.UpdateAsync(EducationTable, item.Id.ToString(), data);

            if (!success)
            {
                return NotFound(new { error = $"Education with ID {item.Id} not found." });
            }

            return Ok(new { message = $"Education with {item.Id} was updated", data = item });
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Education item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Degree", item.Degree },
                { "@Institution", item.Institution },
                { "@Description", item.Description },
                { "@UserProfileId", item.User_Profile_Id },
                { "@StartDate", item.StartDate },
                { "@EndDate", item.EndDate ?? "" }
            };

            var success = await _db.InsertAsync(EducationTable, data);
            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add education" });
            }
            item.Id = Convert.ToInt32(success);
            return Ok(new { message = "Created Education", data = item });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _db.DeleteAsync(EducationTable, id.ToString());
            if (!success)
            {
                return NotFound(new { error = $"Education with ID {id} not found." });
            }
            return Ok(new { message = $"Education with ID {id} has been deleted." });
        }
    }
}