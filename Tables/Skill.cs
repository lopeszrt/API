using System.Data;

namespace API.Tables
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public readonly int UserProfileId;

        public List<ProgrammingLanguage> ProgrammingLanguages = [];

        public Skill(int id, string name, string description, int userProfileId)
        {
            Id = id;
            Name = name;
            Description = description;
            UserProfileId = userProfileId;
        }

        public void AddProgrammingLanguage(ProgrammingLanguage programmingLanguage)
        {
            ProgrammingLanguages ??= [];
            if (programmingLanguage.SkillId == Id)
            {
                ProgrammingLanguages.Add(programmingLanguage);
            }
        }

        public static Skill CreateFromDataRow(DataRow row)
        {
            return new Skill(
                Convert.ToInt32(row["id"]),
                row["Name"].ToString(),
                row["Description"].ToString(),
                Convert.ToInt32(row["UserProfileId"])
            );
        }
    }
}