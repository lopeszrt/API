using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HobbyController : Controller
    {
        private readonly Database _db;

        public HobbyController(Database db)
        {
            _db = db;
        }

        [AllowAnonymous]
        [HttpGet()]
        public async Task<ActionResult<List<Hobby>>> GetHobbies()
        {
            var table = await _db.ExecuteQueryAsync("SELECT * FROM Hobby", new());

            var list = new List<Hobby>();
            foreach (DataRow row in table.Rows)
            {
                list.Add(new Hobby(
                    Convert.ToInt32(row["Id"]),
                    row["Name"].ToString(),
                    Convert.ToInt32(row["User_Profile_Id"])
                ));
            }

            return Ok(list);
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