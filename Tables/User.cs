namespace API.Tables
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public bool checkHashed(string password)
        {
            return BCrypt.Net.BCrypt.Verify(Password,password);
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
    }
}