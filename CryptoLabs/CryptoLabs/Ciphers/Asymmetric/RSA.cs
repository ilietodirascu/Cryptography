using CryptoLabs.Ciphers.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLabs.Ciphers.Asymmetric
{
    public class RSA
    {
        private int _e;
        private int _d;
        public List<BigInteger> Encrypt(string plaintext, int p, int q)
        {
            if (Utility.GCD(p, q) != 1) throw new Exception("p and q must be coprime");
            var intSymbols = Encoding.UTF8.GetBytes(plaintext).ToList().Select(x => Convert.ToInt32(x)).ToList();
            int n = p * q;
            var fi = (p - 1) * (q - 1);
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
        private int ComputeE(int e, int fi, int n)
        {
            if (Utility.GCD(e, n) != 1 || Utility.GCD(e, fi) != 1)
            {
                return ComputeE(Utility.Random.Next(2, fi), fi, n);
            }
            return e;
        }
        private int ComputeD(int k, int fi, int e)
        {
            if ((k * fi + 1) % e == 0)
            {
                return (k * fi + 1) / e;
            }
            var result = ComputeD(++k, fi, e);
            return result != e ? result : ComputeD(++k, fi, e);
        }
        public string Decrypt(List<BigInteger> intSymbols, int p, int q)
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
}
