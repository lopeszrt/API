using System.Data;

namespace API.Tables
{
    public class Hobby
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public readonly int UserProfileId;

        public Hobby(int id, string name, int userProfileId)
        {
            Id = id;
            Name = name;
            UserProfileId = userProfileId;
        }

        public static Hobby CreateFromDataRow(DataRow row)
        {
            return new Hobby(
                Convert.ToInt32(row["id"]),
                row["Name"].ToString(),
                Convert.ToInt32(row["UserProfileId"])
            );
        }
    }
}