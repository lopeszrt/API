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
    public class UserController : Controller, IController<UserRequest>
    {
        private readonly DatabaseCalls _db;

        public UserController(DatabaseCalls db, JwtService jwtService)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] UserRequest item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var data = new Dictionary<string, object>
            {
                { "@Username", item.Username },
                { "@Password", LoginRequest.HashPassword(item.Password) }
            };

            var success = await _db.InsertAsync(TableName.User, data);
            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add user." });
            }
            int newId = Convert.ToInt32(success);
            var createdItem = await _db.GetFromTableAsync(TableName.User, newId.ToString());
            return CreatedAtAction(nameof(GetById), new {id = newId}, createdItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid user ID." });
            }
            var success = await _db.DeleteAsync(TableName.User, id.ToString());
            if (!success)
            {
                return NotFound(new { error = $"User with ID {id} not found." });
            }
            return Ok(new { message = $"User with ID {id} was deleted." });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var table = await _db.GetFromTableAsync(TableName.User);
            return Ok(new { data = (from DataRow row in table.Rows select Tables.User.CreateFromDataRow(row)).ToList() });
        }

        public Task<IActionResult> GetByForeignId(int foreignId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _db.GetFromTableAsync(TableName.User, id.ToString());
            if (res.Rows.Count == 0)
            {
                return NotFound(new { error = $"User with ID {id} not found." });
            }
            return Ok(new { data = Tables.User.CreateFromDataRow(res.Rows[0]) });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserRequest item)
        {
            if (id <= 0 || !ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid ID or model state.", ModelState });
            }
            var data = new Dictionary<string, object>
            {
                { "@Username", item.Username },
                { "@Password", LoginRequest.HashPassword(item.Password) }
            };
            var success = await _db.UpdateAsync(TableName.User, id.ToString(), data);
            if (!success)
            {
                return NotFound(new { error = $"User with ID {id} not found." });
            }
            var updatedItem = await _db.GetFromTableAsync(TableName.User, id.ToString());
            return Ok(new { message = "User updated successfully", data = updatedItem });
        }
    }
}