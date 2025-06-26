namespace API
{
    public class Profile
    {
        private readonly int _id;
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }
        public string LinkedIn { get; set; }
        public string GitHub { get; set; }
        public readonly int UserId;

        public int Id
        {
            get { return _id; }
        }

        public Profile(int id, string name, string description, string email, string phone, string location, string linkedIn, string gitHub, int userId)
        {
            _id = id;
            Name = name;
            Description = description;
            Email = email;
            Phone = phone;
            Location = location;
            LinkedIn = linkedIn;
            GitHub = gitHub;
            UserId = userId;
        }
    }
}