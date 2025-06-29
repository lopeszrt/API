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
    public class UserController : Controller, IController<User>
    {
        private readonly DatabaseCalls _db;
        private const string UserTable = "User";

        public UserController(DatabaseCalls db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] User item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Username", item.Username },
                { "@Password", item.Password }
            };

            var success = await _db.InsertAsync(UserTable, data);
            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add user." });
            }
            item.Id = Convert.ToInt32(success);
            return Ok(new { message = "Created User", data = item });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid user ID." });
            }
            var success = await _db.DeleteAsync(UserTable, id.ToString());
            if (!success)
            {
                return NotFound(new { error = $"User with ID {id} not found." });
            }
            return Ok(new { message = $"User with ID {id} was deleted." });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var table = await _db.GetFromTableAsync(UserTable);
            return Ok(new { data = (from DataRow row in table.Rows select Tables.User.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("profile/{foreignId}")]
        public async Task<IActionResult> GetByForeignId(int foreignId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _db.GetFromTableAsync(UserTable, id.ToString());
            if (res.Rows.Count == 0)
            {
                return NotFound(new { error = $"User with ID {id} not found." });
            }
            return Ok(new { data = Tables.User.CreateFromDataRow(res.Rows[0]) });
        }

        public async Task<IActionResult> Update([FromBody] User item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Username", item.Username },
                { "@Password", item.Password }
            };
            var success = await _db.UpdateAsync(UserTable, item.Id.ToString(), data);
            if (!success)
            {
                return NotFound(new { error = $"User with ID {item.Id} not found." });
            }
            return Ok(new { message = "User updated successfully", data = item });
        }
    }
}