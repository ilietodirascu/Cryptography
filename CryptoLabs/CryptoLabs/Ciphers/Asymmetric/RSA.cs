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
            var intSymbols = Encoding.BigEndianUnicode.GetBytes(plaintext).ToList().Select(x => Convert.ToInt32(x)).ToList();
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
            return encryptedIntSymbols;
        }
        public byte[] Encrypt(byte[] bytes, int p, int q)
        {
            if (Utility.GCD(p, q) != 1) throw new Exception("p and q must be coprime");
            int n = p * q;
            var fi = (p - 1) * (q - 1);
            _e = ComputeE(-1, fi, n);
            _d = ComputeD(1, fi, _e);
            var encryptedIntSymbols = new List<int>();
            bytes.ToList().ForEach(x =>
            {
                var encrypted = (int)(BigInteger.Pow(x, _e) % n);
                encryptedIntSymbols.Add(encrypted);
            });
            var test = encryptedIntSymbols.SelectMany(x => BitConverter.GetBytes(x)).ToArray();
            //return BitConverter.IsLittleEndian ? test.Reverse().ToArray() : test;
            return test;
        }
        public byte[] Decrypt(byte[] bytes, int p, int q)
        {
            var intSymbols = new List<int>();
            for (int i = 0; i < bytes.Length; i+=4)
            {
                int sum = 0;
                byte[] fourBytes = new byte[4];
                Array.Copy(bytes, i, fourBytes, 0,4);
                sum = BitConverter.ToInt32(fourBytes, 0);
                intSymbols.Add(sum);
            }
            int n = p * q;
            var fi = (p - 1) * (q - 1);
            var decryptedSymbols = new List<byte>();
            intSymbols.ToList().ForEach(x =>
            {
                var decrypted = (int)(BigInteger.Pow(x, _d) % n);
                decryptedSymbols.AddRange(BitConverter.GetBytes(decrypted).Where(b => b != 0).ToList());
            });
            return decryptedSymbols.ToArray();
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
            return Encoding.BigEndianUnicode.GetString(decryptedSymbols.ToArray());
        }
    }
}
