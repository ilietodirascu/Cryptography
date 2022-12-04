using CryptoLabs.Ciphers.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLabs.Ciphers.Symmetric
{
    public class Feistel
    {
        public string Text { get; set; }
        public string L1 { get; set; }
        public string R1 { get; set; }
        public Feistel(string text)
        {
            Console.WriteLine($"Plaintext:{text}");
            Text = BinaryConverter.StringToBinary(text);
            L1 = new string(Text.Take(Text.Length / 2).ToArray());
            R1 = new string(Text.Skip(Text.Length / 2).ToArray());
            Console.WriteLine("Encryption: " + Encrypt("ilie", "todirascu"));
            Console.WriteLine("Decryption: " + Decrypt("ilie", "todirascu"));
        }
        public string Encrypt(string key1, string key2)
        {
            key1 = BinaryConverter.StringToBinary(key1);
            key2 = BinaryConverter.StringToBinary(key2);
            key1 = key1.Length < Text.Length / 2 ? key1 + new string('0', Text.Length / 2 - key1.Length) : new(key1.Take(Text.Length / 2).ToArray());
            key2 = key2.Length < Text.Length / 2 ? key2 + new string('0', Text.Length / 2 - key2.Length) : new(key2.Take(Text.Length / 2).ToArray());
            var l1 = XOR(L1, XOR(R1, key1));
            var r1 = XOR(R1, XOR(l1, key2));
            L1 = r1;
            R1 = l1;
            return l1 + r1;
        }
        public string Decrypt(string key1, string key2)
        {
            Encrypt(key2, key1);
            return BinaryConverter.ToString(L1 + R1);
        }
        public static string XOR(string a, string b)
        {
            string result = "";
            for (int i = 0; i < a.Length; i++)
            {
                result += a[i] ^ b[i];
            }
            return result;
        }
    }
}
