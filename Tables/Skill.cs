namespace API.Tables
{
    public class Skill
    {
        private readonly int _id;
        public string Name { get; set; }

        public string Description { get; set; }

        public readonly int ProfileId;

        public readonly int ProgrammingLanguageId;

        public readonly int projectId;

        public Skill(int id, string name, string description, int programmingLanguageId, int profileId)
        {
            _id = id;
            Name = name;
            Description = description;
            ProgrammingLanguageId = programmingLanguageId;
            ProfileId = profileId;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public int Id
        {
            get { return _id; }
        }
    }
}