using System.Diagnostics.Eventing.Reader;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class EducationController : ControllerBase
    {
        [HttpGet("api/educations")]
        public List<Education> GetEducations()
        {
            return new List<Education>
            {
                new Education(1, "Bachelor of Science in Computer Science", "University of Example", new DateOnly(2015, 9, 1), new DateOnly(2019, 6, 30)),
                new Education(2, "Master of Science in Software Engineering", "Example University", new DateOnly(2020, 9, 1), null)
            };
        }

        [HttpGet("api/educations/{id}")]
        public ActionResult<Education> GetEducationById(int id)
        {
            var educations = GetEducations();
            var education = educations.FirstOrDefault(e => e.Id == id);
            if (education == null)
            {
                return NotFound($"Education with ID {id} not found.");
            }
            return Ok(education);
        }

        [HttpPut("api/educations/add")]
        public IActionResult AddEducation([FromBody] Education education)
        {
            if (education == null)
            {
                return BadRequest("Education cannot be null.");
            }
            // Here you would typically add the education to a database or a list.
            // For this example, we will just return the added education.
            return CreatedAtAction(nameof(GetEducationById), new { id = education.Id }, education);
        }

        [HttpPost("api/educations/update/{id}")]
        public IActionResult UpdateEducation(int id, [FromBody] Education education)
        {
            if (education == null || education.Id != id)
            {
                return BadRequest("Invalid education data.");
            }
            // Here you would typically update the education in a database or a list.
            // For this example, we will just return the updated education.
            return Ok(education);
        }

        [HttpDelete("api/educations/delete/{id}")]
        public IActionResult DeleteEducation(int id)
        {
            // Here you would typically delete the education from a database or a list.
            // For this example, we will just return a success message.
            return Ok($"Education with ID {id} has been deleted.");
        }
    }
}
