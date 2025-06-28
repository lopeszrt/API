using API.Services;
using API.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EducationController : Controller
    {
        private readonly DatabaseCalls _db;
        private const string EducationTable = "Education";

        public EducationController(DatabaseCalls db)
        {
            _db = db;
        }

        [AllowAnonymous]
        [HttpGet()]
        public async Task<IActionResult> GetEducations()
        {
            Console.WriteLine("Fetching all educations from the database...");
            var table = await _db.GetFromTableAsync(EducationTable);

            var list = new List<Education>();
            foreach (DataRow row in table.Rows)
            {
                list.Add(new Education(
                    Convert.ToInt32(row["Id"]),
                    Convert.ToInt32(row["User_Profile_Id"]),
                    row["Degree"].ToString(),
                    row["Description"].ToString(),
                    row["Institution"].ToString(),
                    row["StartDate"].ToString(),
                    row.IsNull("EndDate") ? "" : row["EndDate"].ToString()
                ));
            }

            return Ok(new { data = list});
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEducationById(int id)
        {
            var table = await _db.GetFromTableAsync(EducationTable, id.ToString());
            if (table.Rows.Count == 0)
            {
                return NotFound(new { error = $"Education with ID {id} not found." });
            }
            var row = table.Rows[0];
            var education = new Education(
                Convert.ToInt32(row["Id"]),
                Convert.ToInt32(row["User_Profile_Id"]),
                row["Degree"].ToString(),
                row["Description"].ToString(),
                row["Institution"].ToString(),
                row["StartDate"].ToString(),
                row.IsNull("EndDate") ? "" : row["EndDate"].ToString()
            );
            return Ok(new { data = education });
        }

        [AllowAnonymous]
        [HttpGet("profile/{profileId}")]
        public async Task<IActionResult> GetEducationsFromProfileId(int profileId)
        {
            if (profileId <= 0)
            {
                return BadRequest(new { error = "Profile ID must be a positive integer." });
            }

            var data = new Dictionary<string, object>
            {
                { "User_Profile_Id", profileId }
            };

            var table = await _db.GetFromTableFilteredAsync(EducationTable, data);
            if (table.Rows.Count == 0)
            {
                return NotFound(new { error = $"No educations found for profile ID {profileId}." });
            }

            var list = new List<Education>();
            foreach (DataRow row in table.Rows)
            {
                list.Add(new Education(
                    Convert.ToInt32(row["Id"]),
                    Convert.ToInt32(row["User_Profile_Id"]),
                    row["Degree"].ToString(),
                    row["Description"].ToString(),
                    row["Institution"].ToString(),
                    row["StartDate"].ToString(),
                    row.IsNull("EndDate") ? "" : row["EndDate"].ToString()
                ));
            }

            return Ok(new { data = list });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEducation(int id, [FromBody] Education education)
        {
            if (education == null || education.Id != id)
            {
                return BadRequest(new { error = "Education cannot be null and ID must match." });
            }

            var data = new Dictionary<string, object>
            {
                { "@Id", education.Id },
                { "@Degree", education.Degree },
                { "@Institution", education.Institution },
                { "@Description", education.Description },
                { "@StartDate", education.StartDate },
                { "@EndDate", education.EndDate ?? (object)DBNull.Value}
            };

            var success = await _db.UpdateAsync(EducationTable, id.ToString(), data);

            if (!success)
            {
                return NotFound(new { error = $"Education with ID {id} not found." });
            }

            return Ok(new { message = $"Education with {id} was updated", data = education });
        }

        [HttpPost()]
        public async Task<IActionResult> AddEducationAsync([FromBody] Education education)
        {
            if (education == null)
            {
                return BadRequest(new { error = "Education cannot be null."});
            }

            var data = new Dictionary<string, object>
            {
                { "@Degree", education.Degree },
                { "@Institution", education.Institution },
                { "@Description", education.Description },
                { "@User_Profile_Id", education.ProfileId },
                { "@StartDate", education.StartDate },
                { "@EndDate", education.EndDate ?? "" }
            };

            var success = await _db.InsertAsync(EducationTable, data);
            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add education" });
            }
            return Ok(new { message = "Created Education" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEducation(int id)
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