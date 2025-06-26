namespace API
{
    public class Language
    {
        private readonly int _id;
        public string Name { get; set; }
        public string Proficiency { get; set; }

        public readonly int ProfileId;

        public int Id
        {
            get { return _id; }
        }

        public Language(int id, string name, string proficiency, int profileId)
        {
            _id = id;
            Name = name;
            Proficiency = proficiency;
            ProfileId = profileId;
        }
    }
}