namespace API.Tables
{
    public class ProgrammingLanguage
    {
        private readonly int _id;
        public string Name { get; set; }
        public int Proficiency { get; set; }

        public readonly int ProfileId;

        public List<Skill> Skills { get; set; } = [];

        public int Id
        {
            get { return _id; }
        }

        public ProgrammingLanguage(int id, string name, int proficiency, int profileId)
        {
            _id = id;
            Name = name;
            Proficiency = proficiency;
            ProfileId = profileId;
        }

        public void AddSkill(Skill skill)
        {
            Skills ??= [];
            if (skill != null && skill.ProgrammingLanguageId == _id)
            {
                Skills.Add(skill);
            }
        }
    }
}