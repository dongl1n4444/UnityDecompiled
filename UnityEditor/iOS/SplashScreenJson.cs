namespace UnityEditor.iOS
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct SplashScreenJson
    {
        public readonly string minOsVersion;
        public readonly string fileSuffix;
        public readonly string size;
        public readonly bool hasOrientation;
        public readonly bool isPortrait;
        public static readonly SplashScreenJson[] types;
        public SplashScreenJson(string minOsVersion, string fileSuffix, string size, bool hasOrientation, bool isPortrait)
        {
            this.minOsVersion = minOsVersion;
            this.fileSuffix = fileSuffix;
            this.size = size;
            this.hasOrientation = hasOrientation;
            this.isPortrait = isPortrait;
        }

        static SplashScreenJson()
        {
            types = new SplashScreenJson[] { new SplashScreenJson("7.0", "700", "{320, 480}", false, false), new SplashScreenJson("7.0", "700-568h", "{320, 568}", false, false), new SplashScreenJson("8.0", "800-667h", "{375, 667}", false, false), new SplashScreenJson("8.0", "800-Portrait-736h", "{414, 736}", true, true), new SplashScreenJson("8.0", "800-Landscape-736h", "{414, 736}", true, false), new SplashScreenJson("7.0", "700-Portrait", "{768, 1024}", true, true), new SplashScreenJson("7.0", "700-Landscape", "{768, 1024}", true, false) };
        }
    }
}

