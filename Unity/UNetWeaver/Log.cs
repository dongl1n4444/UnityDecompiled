namespace Unity.UNetWeaver
{
    using System;

    public static class Log
    {
        public static Action<string> ErrorMethod;
        public static Action<string> WarningMethod;

        public static void Error(string msg)
        {
            ErrorMethod("UNetWeaver error: " + msg);
        }

        public static void Warning(string msg)
        {
            WarningMethod("UNetWeaver warning: " + msg);
        }
    }
}

