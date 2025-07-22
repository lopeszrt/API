using System.Data;

namespace API.Tables
{
    public class Skill
    {
        public int Id;
        public string Name { get; set; }

        public readonly int UserProfileId;
        public int? ProjectId { get; set; }

        public Skill(int id, string name, int userProfileId, int? projectId = null)
        {
            Id = id;
            Name = name;
            UserProfileId = userProfileId;
            ProjectId = projectId;
        }

        public static Skill CreateFromDataRow(DataRow row)
        {
            return new Skill(
                Convert.ToInt32(row["id"]),
                row["Name"].ToString() ?? "",
                Convert.ToInt32(row["UserProfileId"]),
                row.IsNull("Project_Id") ? null : Convert.ToInt32(row["Project_Id"])
            );
        }
    }
}