namespace API
{
    public class Skill
    {
        private readonly int _id;
        public string Name { get; set; }

        public string Description { get; set; }
        public Skill(int id, string name, string description)
        {
            _id = id;
            Name = name;
            Description = description;
        }

        public int GetId()
        {
            return _id;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }
    }
}
