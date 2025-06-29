using System.Data;

namespace API
{
    public class User_Profile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }
        public string LinkedIn { get; set; }
        public string GitHub { get; set; }
        public string Route { get; set; }
        public string ImageUrl { get; set; }

        public bool PublicPhone { get; set; } = true;
        public bool PublicEmail { get; set; } = true;

        public readonly int UserId;


        public User_Profile(int id, string name, string description, string email, string phone, string location,
            string linkedIn, string gitHub, int userId, bool publicPhone, bool publicEmail ,string? route, string imageURL = "")
        {
            Id = id;
            Name = name;
            Description = description;
            Email = email;
            Phone = phone;
            Location = location;
            LinkedIn = linkedIn;
            GitHub = gitHub;
            UserId = userId;
            Route = route;
            ImageUrl = imageURL;
            PublicPhone = publicPhone;
            PublicEmail = publicEmail;
        }

        public static User_Profile CreateFromDataRow(DataRow row)
        {
            return new User_Profile(
                Convert.ToInt32(row["id"]),
                row["Name"].ToString(),
                row["Description"].ToString(),
                row["Email"].ToString(),
                row["Phone"].ToString(),
                row["Location"].ToString(),
                row["LinkedIn"].ToString(),
                row["GitHub"].ToString(),
                Convert.ToInt32(row["UserId"]),
                row.IsNull("PublicPhone") || Convert.ToBoolean(row["PublicPhone"]),
                row.IsNull("PublicEmail") || Convert.ToBoolean(row["PublicEmail"]),
                row.IsNull("Route") ? "" : row["Route"].ToString(),
                row.IsNull("Image") ? "" : row["ImageUrl"].ToString()
            );
        }
    }
}