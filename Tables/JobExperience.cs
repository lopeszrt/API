using System.Data;

namespace API.Tables
{
    public class JobExperience
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Description { get; set; }
        public string Recommendation { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public readonly int UserProfileId;

        public JobExperience(int id, int userProfileId, string title, string company, string startDate, string description, string endDate = "", string recommendation = "")
        {
            Id = id;
            Title = title;
            Company = company;
            StartDate = startDate;
            EndDate = endDate;
            Description = description;
            Recommendation = recommendation;
            UserProfileId = userProfileId;
        }

        public static JobExperience CreateFromDataRow(DataRow row)
        {
            return new JobExperience(
                Convert.ToInt32(row["id"]),
                Convert.ToInt32(row["UserProfileId"]),
                row["Title"].ToString(),
                row["Company"].ToString(),
                row["StartDate"].ToString(),
                row["Description"].ToString(),
                row.IsNull("EndDate") ? "" : row["EndDate"].ToString(),
                row.IsNull("Recommendation") ? "" : row["Recommendation"].ToString()
            );
        }
    }
}