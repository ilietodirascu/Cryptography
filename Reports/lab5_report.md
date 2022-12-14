# Lab 5 Report

# Topic: Web Authentication & Authorisation.

### Course: Cryptography & Security

### Author: Ilie Todirascu

---

## Overview

&ensp;&ensp;&ensp; Authentication & authorization are 2 of the main security goals of IT systems and should not be used interchangibly. Simply put, during authentication the system verifies the identity of a user or service, and during authorization the system checks the access rights, optionally based on a given user role.

&ensp;&ensp;&ensp; There are multiple types of authentication based on the implementation mechanism or the data provided by the user. Some usual ones would be the following:

- Based on credentials (Username/Password);
- Multi-Factor Authentication (2FA, MFA);
- Based on digital certificates;
- Based on biometrics;
- Based on tokens.

&ensp;&ensp;&ensp; Regarding authorization, the most popular mechanisms are the following:

- Role Based Access Control (RBAC): Base on the role of a user;
- Attribute Based Access Control (ABAC): Based on a characteristic/attribute of a user.

## Objectives:

1. Take what you have at the moment from previous laboratory works and put it in a web service / serveral web services.
2. Your services should have implemented basic authentication and MFA (the authentication factors of your choice).
3. Your web app needs to simulate user authorization and the way you authorise user is also a choice that needs to be done by you.
4. As services that your application could provide, you could use the classical ciphers. Basically the user would like to get access and use the classical ciphers, but they need to authenticate and be authorized.

## Implementation:

### User

The user will be able to authenticate and use the services provided by the web application. The user will be able to use the classical ciphers, but only if they are authenticated and authorized.

```C#
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
```

### Web Service

The web api is implemented using _.net_ framework.
users is our in memory db (a list)
For user registration we have the endpoint `api/auth/register`. First we create the password hash, then we can choose the role. If it already exists the app will return an error.
I noticed while writing the report that it would make more sense to have the check first and hashing later.

```C#
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
```

When logging in, we get the user form the in-memory database, then we compare the password hash with the one from the database. If the password is correct, we generate a JWT token and return it.
That handles the authentication part. The token then must be copied and pasted inside a special container with a special schema. I used a special library for this. Only after that
the endpoints will be usable. If we dont paste the token, the endpoints will return 401, if the token doesn't contain the right claim it will return 403.

```C#
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization Header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
}
```

```C#
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
```

### JWT Token

The JWT token is generated using the code below. The token is signed using the HMAC algorithm and the secret key is stored in appsettings.

```c#
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
```

### OTP

For two factor authentication is used one time passwords (OTP) that are sent to the user email address. I created a sendgrid account and configured it to work with
Mailkit - a dotnet library for handling mail.

```C#
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
```

### Authorization

I use the authorize attribute to specify which endpoints require which role
Admin has access to all, User only to endpoints with just [Authorize]

```c#
[HttpGet("decryptCaesar")]
        [Authorize(Roles = "Admin")]
        public string DecryptCaesar(string cipherText, int key)
        {
            return _caesarCipher.Decrypt(cipherText, key);
        }
```

## Conclusion

In this laboratory work I implemented a REST API for encrypting and decrypting messages using classic ciphers. The API is secured by using JWT tokens and two factor authentication using OTP
send via email.
The emails are sent via sendgrid.
