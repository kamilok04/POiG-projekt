using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;



namespace Projekt.Miscellaneous
{
    public interface IHashingHandler
    {
        public static byte[] GetHash(string inputString)
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(inputString));
        }

        public static string? GetHashString(string inputString)
        {
            StringBuilder sb = new();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static string GetRandomString(int count)
        {
            return RandomNumberGenerator.GetHexString(count);
        }

        
    }
}
