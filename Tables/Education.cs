using API.Structure;
using System.Data;

namespace API.Tables
{
    public class Education
    {
        public int Id { get; set; }
        public string Degree { get; set; }
        public string Institution { get; set; }
        public string Description { get; set; }

        public string? EndDate { get; set; }
        public string StartDate { get; set; }

        public readonly int UserProfileId;

        public Education(int id, int userProfileId, string degree, string description, string institution, string startDate, string? endDate = "")
        {
            Id = id;
            Degree = degree;
            Institution = institution;
            StartDate = startDate;
            EndDate = endDate;
            Description = description;
            UserProfileId = userProfileId;
        }

        public static Education CreateFromDataRow(DataRow row)
        {
            return new Education(
                Convert.ToInt32(row["id"]),
                Convert.ToInt32(row["UserProfileId"]),
                row["Degree"].ToString(),
                row["Description"].ToString(),
                row["Institution"].ToString(),
                row["StartDate"].ToString(),
                row.IsNull("EndDate") ? "" : row["EndDate"].ToString()
            );
        }
    }
}