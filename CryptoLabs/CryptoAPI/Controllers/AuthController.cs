using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CryptoAPI.Users;
using CryptoLabs.Ciphers.Classical;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;

namespace CryptoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static List<User> users = new();
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterDto request,bool isAdmin)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] salt);
            if (users.Any(u => u.Email == request.Email)) return Conflict();
            var user = new User(request.Email)
            {
                PasswordHash = passwordHash,
                PasswordSalt = salt,
                Role = isAdmin ? "Admin" : "User"
            };
            users.Add(user);
            return Ok(user);
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            if (users.Count < 1) return BadRequest("User Not found");
            if (!users.Any(u => u.Email.Equals(request.Email))) return BadRequest("User not found");
            var user = users.First(u => u.Email == request.Email);
            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password");
            }
            if (request.Otp != user.Otp) return BadRequest("Wrong otp");
            return Ok(CreateToken(user));
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        [HttpGet("otp")]
        public async Task<IActionResult> GetOtp(string email)
        {
            if (!users.Any(u => u.Email == email)) return BadRequest("User not found") ;
            var otp = new Random().Next(100000, 1000000);
            var dbUser = users.First(u => u.Email == email);
            using var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                "cryptocatlabs",
                "cryptocatdelivery@gmail.com"
            ));
            message.To.Add(new MailboxAddress(
                "AppUser",
                dbUser.Email
            ));
            message.Subject = "CryptoCatLabs - Verification Code";
            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Your verification code is: {otp}.",
                HtmlBody = $"Your verification code is: <b>{otp}</b>"
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.sendgrid.net", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(
                userName: "apikey", 
                password: _configuration.GetSection("Email:SENDGRID_API_KEY").Value 
            );

            await client.SendAsync(message);
            dbUser.Otp = otp;
            await client.DisconnectAsync(true);
            return Ok();
        }
    }
}
