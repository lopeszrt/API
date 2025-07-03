using API.Services;
using API.Structure;
using API.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProfileController : Controller, IController<UserProfileRequest>
    {
        private readonly DatabaseCalls _db;

        public ProfileController(DatabaseCalls db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] UserProfileRequest item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@Description", item.Description },
                { "@Email", item.Email },
                { "@Phone", item.Phone },
                { "@Location", item.Location },
                { "@LinkedIn", item.LinkedIn },
                { "@GitHub", item.GitHub },
                { "@Route", item.Route },
                { "@ImageUrl", item.ImageUrl ?? (object) DBNull.Value },
                { "@PublicPhone", item.PublicPhone},
                { "@PublicEmail", item.PublicEmail},
                { "@UserId", item.UserId }
            };
            var success = await _db.InsertAsync(TableName.UserProfile, data);
            if (success == -1)
            {
                return BadRequest(new { error = "Failed to add profile." });
            }
            int newId = Convert.ToInt32(success);

            var createdItem = await _db.GetFromTableAsync(TableName.UserProfile, newId.ToString());
            return CreatedAtAction(nameof(GetById), new {id = newId}, createdItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _db.DeleteAsync(TableName.UserProfile, id.ToString());
            if (!success)
            {
                return NotFound(new { error = $"Profile with ID {id} not found." });
            }
            return Ok(new { message = $"Profile with ID {id} was deleted." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var table = await _db.GetFromTableAsync(TableName.UserProfile);
            return Ok(new { data = (from DataRow row in table.Rows select UserProfile.CreateFromDataRow(row)).ToList() });
        }

        [HttpGet("user/{foreignId}")]
        public async Task<IActionResult> GetByForeignId(int foreignId)
        {
            var data = new Dictionary<string, object>
            {
                { "UserId", foreignId }
            };
            var res = await _db.GetFromTableFilteredAsync(TableName.UserProfile, data);
            if (res.Rows.Count == 0)
            {
                return NotFound(new { error = $"Profile with ForeignId {foreignId} not found." });
            }
            return Ok(new { data = UserProfile.CreateFromDataRow(res.Rows[0]) });
        }

        [AllowAnonymous]
        [HttpGet("portfolio/{route}")]
        public async Task<IActionResult> GetPortfolio(string route)
        {
            var data = new Dictionary<string, object>
            {
                { "Route", route }
            };

            var res = await _db.GetFromTableFilteredAsync("User_Profile", data);
            if (res.Rows.Count == 0)
            {
                return NotFound(new { error = "User_Profile not found." });
            }

            var profileId = Convert.ToInt32(res.Rows[0]["Id"]);

            var fdata = new Dictionary<string, object> { { "UserProfileId", profileId } };

            var profileRes = await _db.GetFromTableAsync(TableName.UserProfile, profileId.ToString());
            var educationsRes = await _db.GetFromTableFilteredAsync(TableName.Education, fdata);
            var hobbiesRes = await _db.GetFromTableFilteredAsync(TableName.Hobby, fdata);
            var skillsRes = await _db.GetFromTableFilteredAsync(TableName.Skill, fdata);
            var jobExperienceRes = await _db.GetFromTableFilteredAsync(TableName.JobExperience, fdata);
            var languagesRes = await _db.GetFromTableFilteredAsync(TableName.Language, fdata);
            var projectsRes = await _db.GetFromTableFilteredAsync(TableName.Project, fdata);
            var programmingLanguagesRes = await _db.GetFromTableFilteredAsync(TableName.ProgrammingLanguage, fdata);

            var profile = UserProfile.CreateFromDataRow(profileRes.Rows[0]);
            var skills = new List<Skill>();
            var projects = new List<Project>();
            var hobbies = (from DataRow row in hobbiesRes.Rows select Hobby.CreateFromDataRow(row)).ToList();
            var educations = (from DataRow ed in educationsRes.Rows select Education.CreateFromDataRow(ed)).ToList();
            var programmingLanguages = (from DataRow pgl in programmingLanguagesRes.Rows select ProgrammingLanguage.CreateFromDataRow(pgl)).ToList();
            var jobExperiences = (from DataRow job in jobExperienceRes.Rows select JobExperience.CreateFromDataRow(job)).ToList();
            var languages = (from DataRow lang in languagesRes.Rows select Language.CreateFromDataRow(lang)).ToList();

            foreach (DataRow sk in skillsRes.Rows)
            {
                var skill = Skill.CreateFromDataRow(sk);
                foreach (var pgl in programmingLanguages)
                {
                    skill.AddProgrammingLanguage(pgl);
                }
                skills.Add(skill);
            }

            foreach (DataRow proj in projectsRes.Rows)
            {
                var project = Project.CreateFromDataRow(proj);
                foreach (var pgl in programmingLanguages)
                {
                    project.AddProgrammingLanguage(pgl);
                }
                projects.Add(project);
            }
            return Ok(new
            {
                data = new Portfolio(profile, educations, jobExperiences, hobbies, skills, languages, projects)
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _db.GetFromTableAsync(TableName.UserProfile, id.ToString());
            if (res.Rows.Count == 0)
            {
                return NotFound(new { error = $"User_Profile with ID {id} not found." });
            }

            return Ok(new { data = UserProfile.CreateFromDataRow(res.Rows[0]) });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,[FromBody] UserProfileRequest item)
        {
            if (id <= 0 || !ModelState.IsValid )
            {
                return BadRequest(new { error = "Invalid ID or model state.", ModelState });
            }
            var data = new Dictionary<string, object>
            {
                { "@Name", item.Name },
                { "@Description", item.Description },
                { "@Email", item.Email },
                { "@Phone", item.Phone },
                { "@Location", item.Location },
                { "@LinkedIn", item.LinkedIn },
                { "@GitHub", item.GitHub },
                { "@Route", item.Route },
                { "@ImageUrl", item.ImageUrl ?? (object) DBNull.Value },
                { "@PublicPhone", item.PublicPhone },
                { "@PublicEmail", item.PublicEmail }
            };

            var res = await _db.UpdateAsync(TableName.UserProfile, id.ToString(), data);
            if (!res)
            {
                return BadRequest(new { error = "Failed to update profile." });
            }
            var updatedItem = await _db.GetFromTableAsync(TableName.UserProfile, id.ToString());

            return Ok(new { message = $"User_Profile with ID {id} was updated", data = updatedItem });
        }
    }
}