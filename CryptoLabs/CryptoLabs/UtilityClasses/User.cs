using CryptoLabs.Ciphers.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RSA = CryptoLabs.Ciphers.Asymmetric.RSA;


namespace CryptoLabs.UtilityClasses
{
    public class User
    {
        public string Login { get; set; }
        public byte[] Password { get; set; }
        public List<byte[]> Messages { get; set; } = new();
        public KeyValuePair<int, int> PublicKey;

        public User(string login, string password)
        {
            Login = login;
            using SHA256 mySHA256 = SHA256.Create();
            var passwordHash = mySHA256.ComputeHash(Encoding.UTF32.GetBytes(password));
            PublicKey = Utility.GetCoPrimePair();
            Password = passwordHash;
        }
    }
}
