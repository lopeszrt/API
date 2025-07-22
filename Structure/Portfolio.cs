using API.Tables;

namespace API.Structure
{
    public class Portfolio
    {
        public Portfolio(UserProfile profile, List<Project> lstProjects)
        {
            Name = profile.Name;
            Email = profile.PublicEmail ? profile.Email : "";
            Phone = profile.PublicPhone ? profile.Phone : "";
            Location = profile.Location;
            LinkedIn = profile.LinkedIn;
            GitHub = profile.GitHub;
            Description = profile.Description;
            ImageUrl = profile.ImageUrl;
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
        public List<Project> Projects;
    }
}
