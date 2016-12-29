namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Text;

    public static class EncodingPortable
    {
        public static Encoding GetDefaultPortable() => 
            Encoding.Default;
    }
}

