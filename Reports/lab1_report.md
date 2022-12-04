### Course: Cryptography & Security

### Author: Ilie Todirascu, FAF-203
----
## Objectives:

* Get familiar with the basics of Cryptography and classical ciphers.

* Implement 4 types of classical cyphers:

  - Caesar cipher,
  - Caesar cipher with an alphabet permutation,
  - Vigenere cipher,
  - Affine cipher.
* Structure the project in methods/classes/packages as needed.
## Implementation Description

### Caesar Cipher

The `CaesarCipher` class implements the ISubstitution<T> interface, to allow encryption with different datatypes, since Caesar works with an int, Viginere with a string

and Affine requieres 2 values no less, that's why I deemed such an implementation appropriate. This interace profides the `Decrypt` and `Encrypt` functions

```C#

 public interface ISubstitution<T>

    {

        public string Encrypt(string word, T key);

        public string Decrypt(string word, T key);

    }

```

I know the version I commited had Key<T> instead of T, but since I started writing the report after almost a month, I too was confused as for why I added this.

I guess I was really owerworked by the PR labs I was doing at the time. While writing this report I refactored the code to work without it, not that it 

really changes the core of the logic it was just a meaningless addition.

When I just started working on the first laboratory work, I thought we had to write the encryption based on a random key and actually break the obtained cipher text

without knowing the key. That's why this particular algorithm has another pair of decrypt/encrypt that require just the plain text for encryption and cipher text

for decryption. I have used an API of English words, but it is rather strange as it doesn't really know about verbs from what I remember or rather their forms, 

it might

know about to be but not the form "is". And since I also wanted it to work with romanian as long as there is at least one english word. I implemented a brute force

solution that keeps track of how many words resulting after decoding with a certain key the API "knows". And a kvp with the longest decoded word and its key.

To measure the "success rate" of each key. If any key had a "success rate" equal to the number of words it returned the key right away.

If not it would check if the "success rate" of every key was the same then it would return the key that generated the longest word known to the API.

This solution is not really perfect, but there is fault in the API too.

```C#

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
```
It inherits from Caesar the regular encryption and decryption, the only difference being that it moves every letter of the alphabet 
to the right by n places where n is the value of the key.

### Viginere
Viginere implements the ISubstitution interface but the key is of type string this time.
The encryption and decryption is a lot more exciting too. There is still the pair of overloaded decrypt/encryp methods as in caesar because I wanted
to actually allow the variant with the auto generated key and API decryption.
The bulk of the code is extracted in another methods `DoCryptoAction`
```C#
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
```
That acceptes a boolean in order to determine which function is needed from the `CaesarCipher` class. I create a delegate since the signature of encrypt and
decrypt is the same and invoke the appropriate method from a `CaesarCipher` instance.
```C#
public string Encrypt(string text, string key)
        {
            return DoCryptoAction(text.Split(" "), true, key);
        }
```
```C#
public string Decrypt(string text, string key)
        {
            return DoCryptoAction(text.Split(" "), false, key);
        }
```
###
Affine cipher implements ISubstitution with a KeyValuePair<int,int>. I check if the those values are compatible with how the formula is supposed to work
use the same logic as for viginere for no reason and that's pretty much it.

```C#
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
```
## Conclusion
In this laboratory work I implemented 4 classical ciphers which gave me insight on basic cryptography and familiarized with the basic concepts that persist
in newer ciphers.
