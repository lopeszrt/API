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
    public class ProjectController : Controller, IController<Project>
    {
        private readonly DatabaseCalls _db;
        private const string ProjectTable = "Project";

        public ProjectController(DatabaseCalls db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var table = await _db.GetFromTableAsync(ProjectTable);
            return Ok(new { data = (from DataRow row in table.Rows select Project.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid ID." });
            }
            var table = await _db.GetFromTableAsync(ProjectTable, id.ToString());
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
            var table = await _db.GetFromTableFilteredAsync(ProjectTable, data);
            if (table.Rows.Count == 0)
            {
                return NotFound(new { error = $"No projects found for User Profile ID {foreignId}." });
            }
            return Ok(new { data = (from DataRow row in table.Rows select Project.CreateFromDataRow(row)).ToList() });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Project item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@Description", item.Description },
                { "@ImageUrl", item.ImageUrl},
                { "@Link", item.Link}
            };

            var success = await _db.UpdateAsync(ProjectTable, item.Id.ToString(), data);
            if (!success)
            {
                return NotFound(new { error = $"Project with ID {item.Id} not found." });
            }
            return Ok(new { message = "Project updated successfully.", data = item });
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Project item)
        {
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@Description", item.Description },
                { "@UserProfileId", item.UserProfileId },
                { "@Link", item.Link }
            };
            var success = await _db.InsertAsync(ProjectTable, data);
            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add project." });
            }
            item.Id = Convert.ToInt32(success);
            return Ok(new { message = "Created Project", data = item });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _db.DeleteAsync(ProjectTable, id.ToString());
            if (!success)
            {
                return NotFound(new { error = $"Project with ID {id} not found." });
            }
            return Ok(new { message = $"Project with ID {id} was deleted." });
        }
    }
}