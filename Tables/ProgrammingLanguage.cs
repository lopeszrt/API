using System.Data;

namespace API.Tables
{
    public class ProgrammingLanguage
    {
        public int Id;
        public string Name { get; set; }
        public int Proficiency { get; set; }

        public readonly int UserProfileId;
        public int? ProjectId { get; set; }

        public ProgrammingLanguage(int id, string name, int proficiency, int userProfileId, int? projectId = null)
        {
            Id = id;
            Name = name;
            Proficiency = proficiency;
            UserProfileId = userProfileId;
            ProjectId = projectId;
        }

        public static ProgrammingLanguage CreateFromDataRow(DataRow row)
        {
            return new ProgrammingLanguage(
                Convert.ToInt32(row["id"]),
                row["Name"].ToString() ?? "",
                Convert.ToInt32(row["Proficiency"]),
                Convert.ToInt32(row["UserProfileId"]),
                row.IsNull("Project_Id") ? null : Convert.ToInt32(row["Project_Id"])
            );
        }
    }
}