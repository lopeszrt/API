namespace API
{
    public class Education
    {
        private readonly int _id;
        public string Degree { get; set; }
        public string Institution { get; set; }
        public string Description { get; set; }

        public Nullable<DateOnly> EndDate { get; set; }
        public DateOnly StartDate { get; set; }

        public Education(int id, string degree, string institution, DateOnly startDate, DateOnly? endDate = null)
        {
            _id = id;
            Degree = degree;
            Institution = institution;
            StartDate = startDate;
            EndDate = endDate;
        }

        public int Id
        {
            get { return _id; }
        }
    }
}
