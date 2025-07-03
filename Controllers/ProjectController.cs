using API.Models;
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
    public class ProjectController : Controller, IController<ProjectRequest>
    {
        private readonly DatabaseCalls _db;

        public ProjectController(DatabaseCalls db)
        {
            _db = db;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var table = await _db.GetFromTableAsync(TableName.Project);
            return Ok(new { data = (from DataRow row in table.Rows select Project.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid ID." });
            }
            var table = await _db.GetFromTableAsync(TableName.Project, id.ToString());
            if (table.Rows.Count == 0)
            {
                return NotFound(new { error = $"Project with ID {id} not found." });
            }
            return Ok(new { data = Project.CreateFromDataRow(table.Rows[0]) });
        }

        [HttpGet("profile/{foreignId}")]
        public async Task<IActionResult> GetByForeignId(int foreignId)
        {
            if (foreignId <= 0)
            {
                return BadRequest(new { error = "Invalid foreign ID." });
            }
            var data = new Dictionary<string, object>
            {
                { "UserProfileId", foreignId }
            };
            var table = await _db.GetFromTableFilteredAsync(TableName.Project, data);
            if (table.Rows.Count == 0)
            {
                return NotFound(new { error = $"No projects found for User Profile ID {foreignId}." });
            }
            return Ok(new { data = (from DataRow row in table.Rows select Project.CreateFromDataRow(row)).ToList() });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,[FromBody] ProjectRequest item)
        {
            if (id <= 0 || !ModelState.IsValid )
            {
                return BadRequest(new { error = "Invalid ID or model state.", ModelState });
            }
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@Description", item.Description },
                { "@ImageUrl", item.ImageUrl ?? (object) DBNull.Value},
                { "@Link", item.Link ?? (object) DBNull.Value}
            };

            var success = await _db.UpdateAsync(TableName.Project, id.ToString(), data);
            if (!success)
            {
                return NotFound(new { error = $"Project with ID {id} not found." });
            }
            var updatedItem = await _db.GetFromTableAsync(TableName.Project, id.ToString());
            return Ok(new { message = "Project updated successfully.", data = updatedItem });
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProjectRequest item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@Description", item.Description },
                { "@UserProfileId", item.UserProfileId },
                { "@Link", item.Link ??(object) DBNull.Value }
            };
            var success = await _db.InsertAsync(TableName.Project, data);
            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add project." });
            }
            int newId = Convert.ToInt32(success);
            var createdItem = await _db.GetFromTableAsync(TableName.Project, newId.ToString());
            return CreatedAtAction(nameof(GetById), new {id = newId}, createdItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _db.DeleteAsync(TableName.Project, id.ToString());
            if (!success)
            {
                return NotFound(new { error = $"Project with ID {id} not found." });
            }
            return Ok(new { message = $"Project with ID {id} was deleted." });
        }
    }
}