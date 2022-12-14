namespace CryptoAPI.Users
{
    public class UserDto
    {
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Otp { get; set; }
    }
}
