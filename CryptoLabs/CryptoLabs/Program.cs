using CryptoLabs.Ciphers.Asymmetric;
using CryptoLabs.Ciphers.Classical;
using CryptoLabs.Ciphers.Symmetric;
using CryptoLabs.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RSA = CryptoLabs.Ciphers.Asymmetric.RSA;

namespace CryptoLabs
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=========================Classical===================================");
            var caesar = new CaesarCipher();
            var viginere = new VigenereCipher();
            var affine = new AffineCipher();
            var permCaesar = new CaesarPermutation();
            var word = caesar.Encrypt("hello", 24);
            Console.WriteLine(word);
            Console.WriteLine(caesar.Decrypt(word, 24));
            word = "machine";
            Console.WriteLine(permCaesar.Encrypt(word, 100));
            Console.WriteLine(permCaesar.Decrypt(word, 100));
            var encryptedViginere = viginere.Encrypt("I am a FAF student", "cafe");
            Console.WriteLine(encryptedViginere);
            Console.WriteLine(viginere.Decrypt(encryptedViginere, "cafe"));
            var encryptedAffine = affine.Encrypt("AFFINE CIPHER", new KeyValuePair<int, int>(17, 20));
            Console.WriteLine(encryptedAffine);
            Console.WriteLine(affine.Decrypt(encryptedAffine, new KeyValuePair<int, int>(17, 20)));
            Console.WriteLine("========================EndClassical==================================");
            Console.WriteLine("========================SymmetricCiphers==============================");
            Feistel _ = new("I am a faf student");
            string phrase = "One lab a day keeps restanta away";
            string key_phrase = "Viorel Bostan";
            byte[] data = Encoding.UTF8.GetBytes(phrase);
            byte[] key = Encoding.UTF8.GetBytes(key_phrase);
            byte[] encrypted_data = RC4.EncDec(data, key);
            byte[] decrypted_data = RC4.EncDec(encrypted_data, key);
            string decrypted_phrase = Encoding.UTF8.GetString(decrypted_data);
            Console.WriteLine("Phrase:\t\t\t{0}", phrase);
            Console.WriteLine("Phrase Bytes:\t\t{0}", BitConverter.ToString(data).Replace("-", ""));
            Console.WriteLine("Key Phrase:\t\t{0}", key_phrase);
            Console.WriteLine("Key Bytes:\t\t{0}", BitConverter.ToString(key).Replace("-", ""));
            Console.WriteLine("Encryption Result:\t{0}", BitConverter.ToString(encrypted_data).Replace("-", ""));
            Console.WriteLine("Decryption Result:\t{0}", BitConverter.ToString(decrypted_data).Replace("-", ""));
            Console.WriteLine("Decrypted Phrase:\t{0}", decrypted_phrase);
            Console.WriteLine("====================EndSymmetricCiphers================================");
            Console.WriteLine("====================AsymmetricCiphers==================================");
            var rsa = new RSA();
            var encrypted = rsa.Encrypt("Viorel Bostan", 59, 61);
            Console.WriteLine(rsa.Decrypt(encrypted, 59, 61));
            Console.WriteLine("====================EndAsymetricCiphers================================");
            using (SHA256 mySHA256 = SHA256.Create())
            {
                var initialMsgHash = mySHA256.ComputeHash(Encoding.UTF8.GetBytes("hello"));
                var encryptedHash = rsa.Encrypt(initialMsgHash, 59, 61);
                Console.WriteLine($"Encrypted Hash: {Convert.ToHexString(encryptedHash)}");
                Console.WriteLine($"InitialHash {Convert.ToHexString(initialMsgHash)}");
                var decryptedHash = rsa.Decrypt(encryptedHash, 59, 61);
                Console.WriteLine($"DecryptHash {Convert.ToHexString(decryptedHash)}");
            }
            var database = new Database();
            database.AddUser("ilietodirascu@gmail.com", "admin121");
            database.Login("ilietodirascu@gmail.com", "admin121");
            database.AddMessage(database.Users.First(), "hello");
            database.VerifySignature(database.Users.First(), "hello");
        }
    }
}
