namespace API
{
    public class JobExperience
    {
        private readonly int _id;
        public string Title { get; set; }
        public string Company { get; set; }
        public string Description { get; set; }
        public string Recomendation { get; set; }
        public string StartDate { get; set; }
        public string? EndDate { get; set; }

        public readonly int ProfileId;

        public JobExperience(int id, int profileId, string title, string company, string startDate, string description, string? endDate = "", string recomendation = "")
        {
            _id = id;
            Title = title;
            Company = company;
            StartDate = startDate;
            EndDate = endDate;
            Description = description;
            Recomendation = recomendation;
            ProfileId = profileId;
        }

        public int Id
        {
            get { return _id; }
        }
    }
}
