using CryptoLabs.Ciphers.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLabs.Ciphers.Classical
{
    public class CaesarPermutation : CaesarCipher
    {
        public List<char> PermutatedAlphabet { get; set; } = new();

        public override string Encrypt(string text, int key)
        {
            PermutatedAlphabet = Utility.Alphabet.Select((x, i) => Utility.Alphabet[(i + key) % Utility.Alphabet.Count]).ToList();
            text = text.ToLower();
            return new string(String.Join(" ", text.Split(" ").Select(x => new string(x.Select(y => PermutatedAlphabet[(Utility.Alphabet.FindIndex(c => c == y) + key) % PermutatedAlphabet.Count]).ToArray())).ToArray()));
        }
        public override string Decrypt(string text, int key)
        {
            text = text.ToLower();
            return new string(String.Join(" ", text.Split(" ").Select(x => new string(x.Select(y => PermutatedAlphabet[Utility.Mod(Utility.Alphabet.FindIndex(c => c == y) - key, PermutatedAlphabet.Count)]).ToArray())).ToArray()));
        }
    }
}
