using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLabs.Ciphers.Symmetric
{
    public static class RC4
    {
        public static void PermuteS(ref int[] s, ref int[] t)
        {
            int j = 0;
            for (int i = 0; i < 256; i++)
            {
                j = (j + s[i] + t[i]) % 256;
                Swap(ref s, i, j);
            }
        }
        public static byte[] EncDec(byte[] data, byte[] key)
        {

            int[] S = new int[256];
            int[] T = new int[256];
            for (int z = 0; z < 256; z++)
            {
                S[z] = z;
                T[z] = key[z % key.Length];
            }
            int i = 0;
            int j = 0;
            for (; i < 256; i++)
            {
                j = (j + S[i] + T[i]) % 256;
                Swap(ref S, i, j);
            }
            i = j = 0;
            byte[] result = new byte[data.Length];
            for (int iteration = 0; iteration < data.Length; iteration++)
            {
                i = (i + 1) % 256;
                j = (j + S[i]) % 256;
                Swap(ref S, i, j);
                int K = S[(S[i] + S[j]) % 256];
                result[iteration] = Convert.ToByte(data[iteration] ^ K);
            }

            return result;
        }
        public static void Swap(ref int[] s, int i, int j)
        {
            int temp = s[i];
            s[i] = s[j];
            s[j] = temp;
        }
    }
}
