using API.Tables;

namespace API.Structure
{
    public class Portfolio
    {
        public Portfolio(User_Profile profile, List<Education> lstEducations, List<JobExperience> lstJobExperiences, List<Hobby> lstHobbies, List<Skill> lstSkills, List<Language> lstLanguages, List<Project> lstProjects)
        {
            Name = profile.Name;
            Email = profile.PublicEmail ? profile.Email : "";
            Phone = profile.PublicPhone ? profile.Phone : "";
            Location = profile.Location;
            LinkedIn = profile.LinkedIn;
            GitHub = profile.GitHub;
            Description = profile.Description;
            ImageUrl = profile.ImageUrl;
            Educations = lstEducations ?? [];
            JobExperiences = lstJobExperiences ?? [];
            Hobbies = lstHobbies ?? [];
            Skills = lstSkills ?? [];
            Languages = lstLanguages ?? [];
            Projects = lstProjects ?? [];

        }

        public string Name;
        public string? Email;
        public string? Phone;
        public string Location;
        public string LinkedIn;
        public string GitHub;
        public string Description;
        public string ImageUrl;
        public List<Education> Educations;
        public List<JobExperience> JobExperiences;
        public List<Hobby> Hobbies;
        public List<Skill> Skills;
        public List<Language> Languages;
        public List<Project> Projects;
    }
}
