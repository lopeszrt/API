namespace API
{
    public class Language(int id, string name, string proficiency)
    {
        private readonly int _id = id;
        public string Name { get; set; } = name;
        public string Proficiency { get; set; } = proficiency;
    }
}
