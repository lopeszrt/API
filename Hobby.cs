namespace API
{
    public class Hobby(int id, string name)
    {
        private readonly int _id = id;
        public string Name { get; set; } = name;
    }
}
