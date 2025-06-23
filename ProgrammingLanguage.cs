namespace API
{
    public class ProgrammingLanguage(int id, string name, int proficiency)
    {
        private readonly int _id = id;
        public string Name { get; set; } = name;
        public int Proficiency { get; set; } = proficiency;
    }
}
