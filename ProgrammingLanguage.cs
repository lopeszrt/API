namespace API
{
    public class ProgrammingLanguage
    {
        private readonly int _id;
        public string Name { get; set; }
        public int Proficiency { get; set; }

        public readonly int ProfileId;

        public List<Skill> skills { get; set; } = new List<Skill>();

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
            if (skill != null && skill.ProgrammingLanguageId == _id)
            {
                skills.Add(skill);
            }
        }
    }
}
