using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ProgrammingLanguageRequest
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, Range(1,10)]
        public string Proficiency { get; set; }

        [Required]
        public int UserProfileId { get; set; }

        public int? SkillId { get; set; }

        public int? ProjectId { get; set; }
    }
}
