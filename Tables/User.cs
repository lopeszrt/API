using System.Data;

namespace API.Tables
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public bool checkHashed(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, this.Password);
        }

        public static string hashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public LoginRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public static LoginRequest CreateFromDataRow(DataRow row)
        {
            return new LoginRequest(
                row["Username"].ToString(),
                row["Password"].ToString()
            );
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        User(int id, string username, string password)
        {
            Id = id;
            Username = username;
            Password = password;
        }

        public static User CreateFromDataRow(DataRow row)
        {
            return new User(
                Convert.ToInt32(row["id"]),
                row["Username"].ToString(),
                row["Password"].ToString()
            );
        }
    }
}