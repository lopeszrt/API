namespace API.Tables
{
    public class ProgrammingLanguage
    {
        private readonly int _id;
        public string Name { get; set; }
        public int Proficiency { get; set; }
        public readonly int ProfileId;
        public int? SkillId { get; set; }
        public int? ProjectId { get; set; }

        public int Id
        {
            get { return _id; }
        }

        public ProgrammingLanguage(int id, string name, int proficiency, int profileId, int? skillId= null, int? projectId = null)
        {
            _id = id;
            Name = name;
            Proficiency = proficiency;
            ProfileId = profileId;
            SkillId = skillId;
            ProjectId = projectId;
        }
    }
}