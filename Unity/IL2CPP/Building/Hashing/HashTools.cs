namespace Unity.IL2CPP.Building.Hashing
{
    using NiceIO;
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    internal static class HashTools
    {
        public static string HashOf(string toString)
        {
            using (MD5 md = MD5.Create())
            {
                return HashToString(md.ComputeHash(Encoding.UTF8.GetBytes(toString)));
            }
        }

        public static string HashOfFile(NPath path)
        {
            string str;
            using (MD5 md = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(path.ToString()))
                {
                    str = HashToString(md.ComputeHash(stream));
                }
            }
            return str;
        }

        private static string HashToString(byte[] computeHash) => 
            BitConverter.ToString(computeHash).Replace("-", "");
    }
}

