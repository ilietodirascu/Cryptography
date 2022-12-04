using CryptoLabs.Ciphers.Abstractions;
using CryptoLabs.Ciphers.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLabs.Ciphers.Classical
{
    public class AffineCipher : ISubstitution<KeyValuePair<int, int>>
    {
        public string Decrypt(string text, KeyValuePair<int, int> key)
        {
            return DoCryptoAction(text.ToLower().Split(" "), false, key);
        }

        public string Encrypt(string text, KeyValuePair<int, int> key)
        {
            return DoCryptoAction(text.ToLower().Split(" "), true, key);
        }
        private string DoCryptoAction(string[] words, bool encrypt, KeyValuePair<int, int> key)
        {
            var caesar = new CaesarCipher();
            //i know there is no point in using my substitution methods from caesar, but i started writing code before i knew how affine works.
            Func<string, int, string> cryptAction = encrypt ? caesar.Encrypt : caesar.Decrypt;
            if (key.Key < 1 || key.Key > Utility.Alphabet.Count - 1 || key.Value < 0 || key.Value > Utility.Alphabet.Count - 1
                || Utility.GCD(key.Key, key.Value) != 1)
            {
                return "Wrong key";
            }
            var encryptedWords = new List<string>();
            foreach (var word in words)
            {
                var encryptedWord = "";
                for (int i = 0; i < word.Length; i++)
                {
                    var step = 0;
                    if (encrypt)
                    {
                        step = step = key.Key * (Utility.Alphabet.FindIndex(x => x == word[i])) + key.Value;
                        encryptedWord += cryptAction(Utility.Alphabet[0] + "", step);
                    }
                    else
                    {
                        step = Utility.Mod(Utility.ModInverse(key.Key, Utility.Alphabet.Count) * (Utility.Alphabet.FindIndex(x => x == word[i]) - key.Value), Utility.Alphabet.Count);
                        encryptedWord += Utility.Alphabet[step];
                    }
                }
                encryptedWords.Add(encryptedWord);
            }
            return string.Join(" ", encryptedWords);
        }

    }
}
