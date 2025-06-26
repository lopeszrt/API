using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EducationController : Controller
    {
        private readonly Database _db;

        public EducationController(Database db)
        {
            _db = db;
        }

        [AllowAnonymous]
        [HttpGet()]
        public async Task<ActionResult<List<Education>>> GetEducations()
        {
            var table = await _db.ExecuteQueryAsync("SELECT * FROM Education", new());

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

            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Education>> GetEducationById(int id)
        {
            var parameters = new Dictionary<string, object> { { "@Id", id } };
            var table = await _db.ExecuteQueryAsync("SELECT * FROM Education WHERE Id = @Id", parameters);
            if (table.Rows.Count == 0)
            {
                return NotFound($"Education with ID {id} not found.");
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
            return Ok(education);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEducation(int id, [FromBody] Education education)
        {
            if (education == null || education.Id != id)
            {
                return BadRequest("Education cannot be null and ID must match.");
            }

            var query = @"
                UPDATE Education
                SET
                    Degree = @Degree,
                    Institution = @Institution,
                    Description = @Description,
                    StartDate = @StartDate,
                    EndDate = @EndDate
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", education.Id },
                { "@Degree", education.Degree },
                { "@Institution", education.Institution },
                { "@Description", education.Description },
                { "@StartDate", education.StartDate },
                { "@EndDate", education.EndDate ?? (object)DBNull.Value }
            };

            var success = await _db.ExecuteNonQueryAsync(query, parameters);

            if (!success)
            {
                return NotFound($"Education with ID {id} not found.");
            }

            return Ok(education);
        }

        [HttpPost()]
        public async Task<IActionResult> AddEducationAsync([FromBody] Education education)
        {
            if (education == null)
            {
                return BadRequest("Education cannot be null.");
            }
            var query = @"
                INSERT INTO Education (Degree, Institution, Description,StartDate, EndDate, User_Profile_Id)
                VALUES (@Degree, @Institution, @Description,@StartDate, @EndDate, @ProfileId);
                ";
            var parameters = new Dictionary<string, object>
            {
                { "@Degree", education.Degree },
                { "@Institution", education.Institution },
                { "@Description", education.Description },
                { "@ProfileId", education.ProfileId },
                { "@StartDate", education.StartDate },
                { "@EndDate", education.EndDate ?? "" }
            };

            var success = await _db.ExecuteNonQueryAsync(query, parameters);
            if (!success)
            {
                return BadRequest("Failed to add education.");
            }
            return Ok("Created Education");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEducation(int id)
        {
            var parameters = new Dictionary<string, object> { { "@Id", id } };
            var query = "DELETE FROM Education WHERE Id = @Id";
            var success = await _db.ExecuteNonQueryAsync(query, parameters);
            if (!success)
            {
                return NotFound($"Education with ID {id} not found.");
            }
            return Ok($"Education with ID {id} has been deleted.");
        }
    }
}