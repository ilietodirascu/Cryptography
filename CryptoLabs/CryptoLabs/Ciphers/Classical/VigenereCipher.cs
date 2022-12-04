using CryptoLabs.Ciphers.Abstractions;
using CryptoLabs.Ciphers.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLabs.Ciphers.Classical
{
    public class VigenereCipher : ISubstitution<string>
    {
        public string Decrypt(string text, string key)
        {
            return DoCryptoAction(text.Split(" "), false, key);
        }
        private string DoCryptoAction(string[] words, bool encrypt, string key)
        {
            var caesar = new CaesarCipher();
            Func<string, int, string> cryptAction = encrypt ? caesar.Encrypt : caesar.Decrypt;
            var count = 1;
            var encryptedWords = new List<string>();
            var keyChar = 0;
            var encryptedWord = "";
            foreach (var word in words)
            {
                for (int i = 0; i < word.Length; i++)
                {
                    if (keyChar == key.Length)
                    {
                        keyChar = 0;
                    }
                    encryptedWord += cryptAction(word[i] + "", (Utility.Alphabet.FindIndex(x => x == key[keyChar]) + 1));
                    if (encryptedWord.Length == key.Length || count == words.Length && i == word.Length - 1)
                    {
                        encryptedWords.Add(encryptedWord);
                        encryptedWord = "";
                    }
                    keyChar++;
                }
                count++;
            }
            return string.Join(" ", encryptedWords);
        }
        public string Encrypt(string text, string key)
        {
            return DoCryptoAction(text.Split(" "), true, key);
        }
        public string Encrypt(string text)
        {
            var key = "";
            key = string.Join("", Enumerable.Repeat(0, GenerateKey(text)).Select(n => Utility.Alphabet[Utility.Random.Next(Utility.Alphabet.Count)]));
            return DoCryptoAction(text.Split(" "), true, key);
        }
        public int GenerateKey(string text)
        {
            text = text.Replace(" ", "");
            return Utility.Random.Next(text.Length / (text.Length / 2) + 1, text.Length - 1);
        }
    }
}
