namespace CryptoAPI.Users
{
    public class User
    {
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set;}
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; }
        public int Otp { get; set; }
        public User(string email)
        {
            Email = email;
        }
    }
}
