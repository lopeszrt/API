namespace API.Tables
{
    public class Skill
    {
        private readonly int _id;
        public string Name { get; set; }
        public string Description { get; set; }

        public readonly int ProfileId;

        public readonly int? ProjectId;

        public List<ProgrammingLanguage> ProgrammingLanguages = [];

        public Skill(int id, string name, string description, int profileId, int? projectId = null)
        {
            _id = id;
            Name = name;
            Description = description;
            ProfileId = profileId;
            ProjectId = projectId;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public void AddProgrammingLanguage(ProgrammingLanguage programmingLanguage)
        {
            ProgrammingLanguages ??= [];
            if (programmingLanguage != null && programmingLanguage.SkillId == _id)
            {
                ProgrammingLanguages.Add(programmingLanguage);
            }
        }

        public int Id
        {
            get { return _id; }
        }
    }
}