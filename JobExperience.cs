namespace API
{
    public class JobExperience(int id, string title, string company, string description)
    {
        private readonly int _id = id;
        public string Title { get; set; } = title;
        public string Company { get; set; } = company;
        public string Description { get; set; } = description;
        public Nullable<int> recomendation { get; set; }
    }
}
