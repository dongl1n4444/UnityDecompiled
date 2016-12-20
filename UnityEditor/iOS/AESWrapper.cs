namespace UnityEditor.iOS
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    internal class AESWrapper
    {
        public static string DecryptString(byte[] cypher, string secret, string salt)
        {
            string str;
            using (Aes aes = new AesManaged())
            {
                aes.Key = new Rfc2898DeriveBytes(secret, Encoding.UTF8.GetBytes(salt)).GetBytes(0x10);
                using (MemoryStream stream = new MemoryStream(cypher))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        aes.IV = reader.ReadBytes(reader.ReadInt32());
                        using (CryptoStream stream2 = new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (StreamReader reader2 = new StreamReader(stream2, Encoding.UTF8))
                            {
                                str = reader2.ReadToEnd();
                            }
                        }
                    }
                }
            }
            return str;
        }

        public static byte[] EncryptString(string text, string secret, string salt)
        {
            byte[] buffer;
            using (Aes aes = new AesManaged())
            {
                aes.Key = new Rfc2898DeriveBytes(secret, Encoding.UTF8.GetBytes(salt)).GetBytes(0x10);
                aes.GenerateIV();
                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(BitConverter.GetBytes(aes.IV.Length), 0, 4);
                    stream.Write(aes.IV, 0, aes.IV.Length);
                    using (CryptoStream stream2 = new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        stream2.Write(Encoding.UTF8.GetBytes(text), 0, text.Length);
                        stream2.FlushFinalBlock();
                        buffer = stream.ToArray();
                    }
                }
            }
            return buffer;
        }
    }
}

