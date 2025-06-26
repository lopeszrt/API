namespace API
{
    public class Education
    {
        private readonly int _id;
        public string Degree { get; set; }
        public string Institution { get; set; }
        public string Description { get; set; }

        public string? EndDate { get; set; }
        public string StartDate { get; set; }

        public readonly int ProfileId;

        public Education(int id, int profileId, string degree, string description, string institution, string startDate, string? endDate = "")
        {
            _id = id;
            Degree = degree;
            Institution = institution;
            StartDate = startDate;
            EndDate = endDate;
            Description = description;
            ProfileId = profileId;
        }

        public int Id
        {
            get { return _id; }
        }
    }
}