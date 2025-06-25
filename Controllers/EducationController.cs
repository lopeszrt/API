using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EducationController : ControllerBase
    {
        private readonly Database _db;

        public EducationController(Database db)
        {
            _db = db;
        }

        [HttpGet()]
        public async Task<ActionResult<List<Education>>> GetEducations()
        {
            var table = await _db.ExecuteQueryAsync("SELECT * FROM Education", new());

            var list = new List<Education>();
            foreach (DataRow row in table.Rows)
            {
                list.Add(new Education(
                    Convert.ToInt32(row["Id"]),
                    row["Degree"].ToString(),
                    row["Institution"].ToString(),
                    DateOnly.FromDateTime(Convert.ToDateTime(row["StartDate"])),
                    row.IsNull("EndDate") ? null : DateOnly.FromDateTime(Convert.ToDateTime(row["EndDate"]))
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
                row["Degree"].ToString(),
                row["Institution"].ToString(),
                DateOnly.FromDateTime(Convert.ToDateTime(row["StartDate"])),
                row.IsNull("EndDate") ? null : DateOnly.FromDateTime(Convert.ToDateTime(row["EndDate"]))
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
                    StartDate = @StartDate, 
                    EndDate = @EndDate 
                WHERE Id = @Id";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", education.Id },
                { "@Degree", education.Degree },
                { "@Institution", education.Institution },
                { "@StartDate", education.StartDate },
                { "@EndDate", education.EndDate ?? (object)DBNull.Value }
            };

            var success = await _db.ExecuteNonQueryAsync(query, parameters);

            if(!success)
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
                INSERT INTO Education (Degree, Institution, Start_Date, End_Date) 
                VALUES (@Degree, @Institution, @StartDate, @EndDate);
                ";
            var parameters = new Dictionary<string, object>
            {
                { "@Degree", education.Degree },
                { "@Institution", education.Institution },
                { "@StartDate", education.StartDate },
                { "@EndDate", education.EndDate ?? (object)DBNull.Value }
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
