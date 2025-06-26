using BCrypt.Net;
using System.Security.Cryptography.Xml;

namespace API
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

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
