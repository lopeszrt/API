namespace API
{
    public class Profile(int id, string name, string description, string email, string number, string location, string linkedIn, string github)
    {
        private readonly int _id = id;
        public string Name { get; set; } = name;
        public string Description { get; set; } = description;
        public string Email { get; set; } = email;
        public string Phone { get; set; } = number;
        public string Location { get; set; } = location;
        public string LinkedIn { get; set; } = linkedIn;
        public string GitHub { get; set; } = github;
    }
}
