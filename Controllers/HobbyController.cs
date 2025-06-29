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
    [Produces("application/json")]
    public class HobbyController : Controller, IController<Hobby>
    {
        private readonly DatabaseCalls _db;
        private const string HobbyTable = "Hobby";

        public HobbyController(DatabaseCalls db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Hobby item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@UserProfileId", item.UserProfileId }
            };
            var success = await _db.InsertAsync(HobbyTable, data);
            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add hobby." });
            }
            item.Id = Convert.ToInt32(success);
            return Ok(new { message = "Created Hobby", data = item });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _db.DeleteAsync(HobbyTable, id.ToString());
            if (!success)
            {
                return NotFound(new { error = $"Hobby with ID {id} not found." });
            }
            return Ok(new { message = $"Hobby with ID {id} was deleted." });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var res = await _db.GetFromTableAsync(HobbyTable);

            return Ok(new { data = (from DataRow row in res.Rows select Hobby.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid ID." });
            }
            var res = await _db.GetFromTableAsync(HobbyTable, id.ToString());
            if (res.Rows.Count == 0)
            {
                return NotFound(new { error = $"Hobby with ID {id} not found." });
            }
            return Ok(new { data = Hobby.CreateFromDataRow(res.Rows[0]) });
        }

        [HttpGet("profile/{foreignId}")]
        public async Task<IActionResult> GetByForeignId(int profileId)
        {
            if (profileId <= 0)
            {
                return BadRequest(new { error = "Invalid User_Profile ID." });
            }
            var data = new Dictionary<string, object>
            {
                { "UserProfileId", profileId }
            };
            var res = await _db.GetFromTableFilteredAsync(HobbyTable, data);
            if (res.Rows.Count == 0)
            {
                return NotFound(new { error = $"No hobbies found for User_Profile ID {profileId}." });
            }
            return Ok(new { data = (from DataRow row in res.Rows select Hobby.CreateFromDataRow(row)).ToList() });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Hobby item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
            };

            var success = await _db.UpdateAsync(HobbyTable, item.Id.ToString(), data);
            if (!success)
            {
                return NotFound(new { error = $"Hobby with ID {item.Id} not found." });
            }
            return Ok(new { message = $"Hobby with ID {item.Id} was updated", data = item });
        }
    }
}