using System.Data;

namespace API.Tables
{
    public class Project
    {
        public int Id;
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Link { get; set; }
        public readonly int UserProfileId;
        public List<ProgrammingLanguage> ProgrammingLanguages = [];
        public string? ImageUrl { get; set; }

        public Project(int id, string name, string description, string? link, int userProfileId, string? imageUrl = "")
        {
            Id = id;
            Name = name;
            Description = description;
            Link = link;
            UserProfileId = userProfileId;
            ImageUrl = imageUrl;

        }

        public void AddProgrammingLanguage(ProgrammingLanguage programmingLanguage)
        {
            ProgrammingLanguages ??= [];
            if (programmingLanguage.ProjectId == Id)
            {
                ProgrammingLanguages.Add(programmingLanguage);
            }
        }

        public static Project CreateFromDataRow(DataRow row)
        {
            return new Project(
                Convert.ToInt32(row["id"]),
                row["Name"].ToString() ?? "",
                row["Description"].ToString() ?? "",
                row.IsNull("Link") ? "" : row["Link"].ToString() ?? "",
                Convert.ToInt32(row["UserProfileId"]),
                row.IsNull("ImageUrl") ? "" : row["ImageUrl"].ToString()
            );
        }
    }
}
