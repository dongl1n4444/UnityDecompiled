namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Security.Cryptography;

    public static class CryptographyPortability
    {
        public static SHA1 CreateSHA1() => 
            new SHA1Managed();
    }
}

