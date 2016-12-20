namespace UnityEditor.iOS
{
    using System;
    using System.Runtime.InteropServices;

    public static class TrampolineProExtras
    {
        private const string nativeDll = "__Internal";

        [DllImport("__Internal")]
        private static extern void NativeFree(IntPtr ptr);
        public static string Salt()
        {
            IntPtr ptr = TrampolineProExtrasSalt();
            string str = Marshal.PtrToStringAnsi(ptr);
            NativeFree(ptr);
            return str;
        }

        public static string Secret()
        {
            IntPtr ptr = TrampolineProExtrasSecret();
            string str = Marshal.PtrToStringAnsi(ptr);
            NativeFree(ptr);
            return str;
        }

        [DllImport("__Internal")]
        private static extern IntPtr TrampolineProExtrasSalt();
        [DllImport("__Internal")]
        private static extern IntPtr TrampolineProExtrasSecret();
    }
}

