using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLabs.Ciphers.UtilityClasses
{
    public static class BinaryConverter
    {
        public static string ToString(string data)
        {
            List<byte> byteList = new();

            for (int i = 0; i < data.Length; i += 8)
            {
                byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
            }
            return Encoding.ASCII.GetString(byteList.ToArray());
        }

        public static string StringToBinary(string data)
        {
            return string.Join("", data.ToCharArray().Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
        }
        public static string IntegerToBinary(int data)
        {
            return Convert.ToString(Convert.ToInt32(data + "", 10), 2).PadLeft(8, '0');
        }
    }
}
