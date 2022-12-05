using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RSA = CryptoLabs.Ciphers.Asymmetric.RSA;

namespace CryptoLabs.UtilityClasses
{
    public class Database
    {
        public List<User> Users { get; set; } = new();
        public void AddUser(string email,string password)
        {
            if (Users.Any(x => x.Login.Equals(email))) return;
            Users.Add(new User(email, password));
        }
        public void Login(string login, string password)
        {
            if (!Users.Any(x => x.Login.Equals(login)))
            {
                Console.WriteLine("Wrong email"); 
                return;
            }
            using SHA256 mySHA256 = SHA256.Create();
            var passwordHash = mySHA256.ComputeHash(Encoding.UTF32.GetBytes(password));
            var user = Users.FirstOrDefault(u => u.Login.Equals(login));
            if (StructuralComparisons.StructuralEqualityComparer.Equals(passwordHash, user.Password))
            {
                Console.WriteLine("Successful login");
                return;
            }
            Console.WriteLine("Wrong password");
        }
        public void AddMessage(User user,string text)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                RSA rsa = new();
                var initialMsgHash = mySHA256.ComputeHash(Encoding.UTF32.GetBytes(text));
                var encryptedHash = rsa.Encrypt(initialMsgHash, user.PublicKey.Key, user.PublicKey.Value);
                user.Messages.Add(encryptedHash);
            }
        }
        public void VerifySignature(User user,string text)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                RSA rsa = new();
                var initialMsgHash = mySHA256.ComputeHash(Encoding.UTF32.GetBytes(text));
                var encryptedHash = rsa.Encrypt(initialMsgHash, user.PublicKey.Key, user.PublicKey.Value);
                var decryptedHash = rsa.Decrypt(encryptedHash, user.PublicKey.Key, user.PublicKey.Value);
                var userMsg = user.Messages.FirstOrDefault(x =>
                BigInteger.Compare(new BigInteger(rsa.Decrypt(x,user.PublicKey.Key,user.PublicKey.Value)), new BigInteger(decryptedHash)) == 0);
                if (userMsg is null)
                {
                    Console.WriteLine("no such message");
                    return;
                }
                Console.WriteLine("Signature passed");
            }
        }
    }
}
