using System.Data;

namespace API.Tables
{
    public class Language
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Proficiency { get; set; }

        public readonly int UserProfileId;


        public Language(int id, string name, string proficiency, int userProfileId)
        {
            Id = id;
            Name = name;
            Proficiency = proficiency;
            UserProfileId = userProfileId;
        }

        public static Language CreateFromDataRow(DataRow row)
        {
            return new Language(
                Convert.ToInt32(row["id"]),
                row["Name"].ToString() ?? "",
                row["Proficiency"].ToString() ?? "",
                Convert.ToInt32(row["UserProfileId"])
            );
        }
    }
}