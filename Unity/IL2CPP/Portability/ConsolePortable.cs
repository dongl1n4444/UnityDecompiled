namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Text;

    public static class ConsolePortable
    {
        public static void SetInputEncodingPortable(Encoding encoding)
        {
            Console.InputEncoding = encoding;
        }

        public static void SetOutputEncodingPortable(Encoding encoding)
        {
            Console.OutputEncoding = encoding;
        }
    }
}

