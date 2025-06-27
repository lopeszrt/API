namespace API.Tables
{
    public class Project
    {
        private readonly int _id;
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Link { get; set; }
        public readonly int ProfileId;
        public List<Skill> Skills = [];

        public Project(int id, string name, string description, string link, int profileId)
        {
            _id = id;
            Name = name;
            Description = description;
            Link = link;
            ProfileId = profileId;
        }
        public int Id
        {
            get { return _id; }
        }

        public void AddSkill(Skill skill)
        {
            Skills ??= [];
            if (skill != null && skill.projectId == _id)
            {
                Skills.Add(skill);
            }
        }
    }
}
