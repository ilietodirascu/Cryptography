# Lab 3 Report

### Course: Cryptography & Security

### Author: Todirascu Ilie, FAF-203

---

## Overview:

&ensp;&ensp;&ensp; Asymmetric Cryptography (a.k.a. Public-Key Cryptography)deals with the encryption of plain text when having 2 keys, one being public and the other one private. The keys form a pair and despite being different they are related.

&ensp;&ensp;&ensp; As the name implies, the public key is available to the public but the private one is available only to the authenticated recipients.

&ensp;&ensp;&ensp; A popular use case of the asymmetric encryption is in SSL/TLS certificates along side symmetric encryption mechanisms. It is necessary to use both types of encryption because asymmetric ciphers are computationally expensive, so these are usually used for the communication initiation and key exchange, or sometimes called handshake. The messages after that are encrypted with symmetric ciphers.

## Objectives:

1. Get familiar with the asymmetric cryptography mechanisms.

2. Implement an example of an asymmetric cipher.

3. As in the previous task, please use a client class or test classes to showcase the execution of your programs.

## Implementation Description:

### RSA

1. **Generating the keys:**

   - The `RSA` class takes as input 2 keys, `p` and `q`, which must be co-prime.
   - Calculate`n = p * q`.
   - Calculate `Fi(n) = (p−1) * (q−1)`
   - Select integer `e`, so it is co-orime to `Fi(pq)` and `1 < e < Fi(pq)`. ** The pair (n, e) = public key.**
   - Calculate integer `d`, `d = (k * fi(n) + 1) / e such that (k * fi(n) + 1) % e == 0` ** The pair (n, d) = private key.**

```C#
    public class RSA
    {

        private int _e;
        private int _d;
        public List<BigInteger> Encrypt(string plaintext,int p, int q)
        {
            if (Utility.GCD(p, q) != 1) throw new Exception("p and q must be coprime");
            var intSymbols = Encoding.UTF8.GetBytes(plaintext).ToList().Select(x => Convert.ToInt32(x)).ToList();
            int n = p * q;
            var fi = (p - 1) *  (q - 1);
            _e = ComputeE(-1, fi, n);
            _d = ComputeD(1, fi, _e);
            var encryptedIntSymbols = new List<BigInteger>();
            intSymbols.ForEach(x =>
            {
                var encrypted = BigInteger.Pow(x, _e) % n;
                encryptedIntSymbols.Add(encrypted);
            });
            Console.WriteLine(string.Join("-", encryptedIntSymbols.Select(x => x.ToString("X")).ToList()));
            return encryptedIntSymbols;
        }
        private int ComputeE(int e,int fi,int n)
        {
            if(Utility.GCD(e,n) != 1 || Utility.GCD(e,fi) != 1)
            {
                return ComputeE(Utility.Random.Next(2, fi),fi,n);
            }
            return e;
        }
        private int ComputeD(int k,int fi, int e)
        {
            if((k * fi + 1) % e == 0)
            {
                return (k * fi + 1) / e;
            }
            var result = ComputeD(++k, fi, e);
            return result != e ? result : ComputeD(++k, fi, e);
        }
        public string Decrypt(List<BigInteger> intSymbols,int p, int q)
        {
            int n = p * q;
            var fi = (p - 1) * (q - 1);
            var decryptedSymbols = new List<byte>();
            intSymbols.ForEach(x =>
            {
                var decrypted = BigInteger.Pow(x, _d) % n;
                decryptedSymbols.AddRange(decrypted.ToByteArray());
            });
            return Encoding.UTF8.GetString(decryptedSymbols.ToArray());
        }
    }
```

2. **Encryption/Decryption:**

The encryption scheme is done as such, where C is the ciphertext and P - the plaintext:

$$C = P^e mod n$$

Now the decryption:

$$P = C^d mod n$$

`Encrypt` I have studied several sources, and from what I could understand the encryption is done in one go, meaning that let's say a string is converted into an array of bytes, then those bytes to
numbers so that modular arithmetic can be applied, then those numbers are combined into one giant number that is encrypted with the above listed formulas, however this implies that p and q need to be
huge prime numbers such that their product is greater than that of our giant number, otherwise due to how modulo works, we will never be able to decrypt it. But it really wasn't practical to implement this
for the purposes of this lab,although that's how RSA is practically applied. In my implementation I transform my bytes into integers and apply the corresponding formulas to determine e and d and ideally I
would also return an array of bytes but due to a difficulty in converting integers to bytes in C#, I didn't want to waste too much time on this, therefore I just console log the bytes, so that it is possible to test that the encryption worked. I hope ComputeD and ComputeE are self explanatory. E needs to be coprime with pq(n) and fi(n). D I explained above.
`Decrypt` Applies the decrypt formula.

## Conclusions:

While working on this laboratory work, I got to understand how The RSA(Rivest–Shamir–Adleman) algorithm works as well as
rekindle my knowledge of prime and co-prime numbers.
