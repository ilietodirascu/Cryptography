using CryptoLabs.Ciphers.Abstractions;
using CryptoLabs.Ciphers.UtilityClasses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLabs.Ciphers.Classical
{
    public class CaesarCipher : ISubstitution<int>
    {
        public string Decrypt(string text)
        {
            var key = GetKey(text);
            text = text.ToLower();
            return new string(String.Join(" ", text.Split(" ").Select(x => new string(x.Select(y => Utility.Alphabet[Utility.Mod(Utility.Alphabet.FindIndex(c => c == y) - key, Utility.Alphabet.Count)]).ToArray())).ToArray()));
        }
        public string Encrypt(string text)
        {
            int key = GenerateKey();
            text = text.ToLower();
            return Encrypt(text, key);
        }

        public int GenerateKey()
        {
            return Utility.Random.Next(1, Utility.Alphabet.Count + 1);
        }

        public int GetKey(string text)
        {
            text = text.ToLower().Trim();
            var words = text.Split(" ");
            bool isNotEnglish;
            var keySuccessRate = new Dictionary<int, int>();
            var longestWordKVP = new KeyValuePair<int, string>(0, "");
            for (int i = 1; i <= Utility.Alphabet.Count; i++)
            {
                var test = words.Select(x => x = Decrypt(x, i)).ToArray();
                foreach (var word in test)
                {
                    var uri = new Uri(@$"https://api.dictionaryapi.dev/api/v2/entries/en/{word}");
                    var response = Utility.Client.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;
                    dynamic json = JsonConvert.DeserializeObject(response);
                    isNotEnglish = json.Type == null;
                    if (isNotEnglish) continue;
                    if (keySuccessRate.ContainsKey(i))
                    {
                        keySuccessRate[i]++;
                    }
                    else
                    {
                        longestWordKVP = longestWordKVP.Value.Length < word.Length ? new KeyValuePair<int, string>(i, word) : longestWordKVP;
                        keySuccessRate.Add(i, 1);
                    }
                    if (keySuccessRate.Values.Any(x => x == words.Length)) return i;
                }
            }
            return keySuccessRate.Values.All(x => x == keySuccessRate.Values.First()) ? longestWordKVP.Key : keySuccessRate.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }
        public virtual string Encrypt(string text, int key)
        {
            text = text.ToLower();
            return new string(String.Join(" ", text.Split(" ").Select(x => new string(x.Select(y => Utility.Alphabet[(Utility.Alphabet.FindIndex(c => c == y) + key) % Utility.Alphabet.Count]).ToArray())).ToArray()));
        }
        public virtual string Decrypt(string text, int key)
        {
            text = text.ToLower();
            return new string(String.Join(" ", text.Split(" ").Select(x => new string(x.Select(y => Utility.Alphabet[Utility.Mod(Utility.Alphabet.FindIndex(c => c == y) - key, Utility.Alphabet.Count)]).ToArray())).ToArray()));
        }
    }
}
