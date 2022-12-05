using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLabs.Ciphers.UtilityClasses
{
    public static class Utility
    {
        public static List<char> Alphabet { get; set; } = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        public static Random Random { get; set; } = new Random();
        public static HttpClient Client { get; set; } = new();
        public static List<int> PrimeNumbers = GetPrimesList();
        public static int GCD(int a, int b)
        {
            if (a == 0) return b;
            return GCD(b % a, a);
        }
        public static int ModInverse(int a, int m)
        {
            int m0 = m;
            int y = 0, x = 1;

            if (m == 1)
                return 0;

            while (a > 1)
            {
                int q = a / m;
                int t = m;
                m = a % m;
                a = t;
                t = y;
                y = x - q * y;
                x = t;
            }
            if (x < 0)
                x += m0;
            return x;
        }
        private static bool IsPrime(int n)
        {
            if (n == 1 || n == 0) return false;
            for (int i = 2; i * i <= n; i++)
            {
                if (n % i == 0) return false;
            }
            return true;
        }
        public static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
        private static List<int> GetPrimesList()
        {
            var num = 60;
            var listOfPrimes = new List<int>();
            while (listOfPrimes.Count < 100)
            {
                ++num;
                if (!IsPrime(num)) continue;
                listOfPrimes.Add(num);
            }
            return listOfPrimes;
        }
        public static KeyValuePair<int,int> GetCoPrimePair()
        {
            int a = PrimeNumbers[Random.Next(0, PrimeNumbers.Count)];
            int b = PrimeNumbers[Random.Next(0, PrimeNumbers.Count)];
            if (a != b) return new KeyValuePair<int, int>(a, b);
            return GetCoPrimePair();
        }
    }
}
