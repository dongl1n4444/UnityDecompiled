namespace UnityEditor.iOS
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;

    [StructLayout(LayoutKind.Sequential)]
    internal struct SplashScreen
    {
        public readonly DeviceType deviceType;
        public readonly string subtype;
        public readonly string minOsVersion;
        public readonly int scale;
        public readonly int width;
        public readonly int height;
        public readonly bool isPortrait;
        public readonly bool matchOrientation;
        public readonly string serializationName;
        public readonly string nonLocalizedName;
        public readonly string xcodeFile;
        public static readonly UnityEditor.iOS.SplashScreen[] iOSTypes;
        public static readonly UnityEditor.iOS.SplashScreen[] tvOSTypes;
        public SplashScreen(DeviceType deviceType, string subtype, string minOsVersion, int scale, int width, int height, bool isPortrait, bool matchOrientation, string serializationName, string nonLocalizedName, string xcodeFile)
        {
            this.deviceType = deviceType;
            this.subtype = subtype;
            this.minOsVersion = minOsVersion;
            this.scale = scale;
            this.width = width;
            this.height = height;
            this.isPortrait = isPortrait;
            this.matchOrientation = matchOrientation;
            this.serializationName = serializationName;
            this.nonLocalizedName = nonLocalizedName;
            this.xcodeFile = xcodeFile;
        }

        public string defaultSource =>
            $"SplashScreen-{this.width}x{this.height}.png";
        public string localizedName =>
            LocalizationDatabase.GetLocalizedString(this.nonLocalizedName);
        public string localizedNameAndTooltip
        {
            get
            {
                if (this.xcodeFile == "Default.png")
                {
                    return this.localizedName;
                }
                string str2 = string.Format(LocalizationDatabase.GetLocalizedString("Optimal resolution - {0}x{1}"), this.width, this.height);
                return (this.localizedName + "|" + str2);
            }
        }
        static SplashScreen()
        {
            iOSTypes = new UnityEditor.iOS.SplashScreen[] { new UnityEditor.iOS.SplashScreen(DeviceType.iPhone, null, null, 1, 320, 480, true, true, "iPhoneSplashScreen", LocalizationDatabase.MarkForTranslation("Mobile Splash Screen*"), "Default.png"), new UnityEditor.iOS.SplashScreen(DeviceType.iPhone, null, "7.0", 2, 640, 960, true, true, "iPhoneHighResSplashScreen", LocalizationDatabase.MarkForTranslation("iPhone 3.5''/Retina"), "Default@2x.png"), new UnityEditor.iOS.SplashScreen(DeviceType.iPhone, "retina4", "7.0", 2, 640, 0x470, true, true, "iPhoneTallHighResSplashScreen", LocalizationDatabase.MarkForTranslation("iPhone 4''/Retina"), "Default-568h@2x.png"), new UnityEditor.iOS.SplashScreen(DeviceType.iPhone, "667h", "8.0", 2, 750, 0x536, true, true, "iPhone47inSplashScreen", LocalizationDatabase.MarkForTranslation("iPhone 4.7''/Retina"), "Default-667h@2x.png"), new UnityEditor.iOS.SplashScreen(DeviceType.iPhone, "736h", "8.0", 3, 0x4da, 0x8a0, true, false, "iPhone55inPortraitSplashScreen", LocalizationDatabase.MarkForTranslation("iPhone 5.5''/Retina"), "Default-Portrait@3x.png"), new UnityEditor.iOS.SplashScreen(DeviceType.iPhone, "736h", "8.0", 3, 0x8a0, 0x4da, false, false, "iPhone55inLandscapeSplashScreen", LocalizationDatabase.MarkForTranslation("iPhone 5.5'' Landscape/Retina"), "Default-Landscape@3x.png"), new UnityEditor.iOS.SplashScreen(DeviceType.iPad, null, "7.0", 1, 0x300, 0x400, true, false, "iPadPortraitSplashScreen", LocalizationDatabase.MarkForTranslation("iPad Portrait"), "Default-Portrait.png"), new UnityEditor.iOS.SplashScreen(DeviceType.iPad, null, "7.0", 1, 0x400, 0x300, false, false, "iPadLandscapeSplashScreen", LocalizationDatabase.MarkForTranslation("iPad Landscape"), "Default-Landscape.png"), new UnityEditor.iOS.SplashScreen(DeviceType.iPad, null, "7.0", 2, 0x600, 0x800, true, false, "iPadHighResPortraitSplashScreen", LocalizationDatabase.MarkForTranslation("iPad Portrait/Retina"), "Default-Portrait@2x.png"), new UnityEditor.iOS.SplashScreen(DeviceType.iPad, null, "7.0", 2, 0x800, 0x600, false, false, "iPadHighResLandscapeSplashScreen", LocalizationDatabase.MarkForTranslation("iPad Landscape/Retina"), "Default-Landscape@2x.png") };
            tvOSTypes = new UnityEditor.iOS.SplashScreen[] { new UnityEditor.iOS.SplashScreen(DeviceType.AppleTV, null, "9.0", 1, 0x780, 0x438, false, false, "appleTVSplashScreen", LocalizationDatabase.MarkForTranslation("AppleTV"), "Default-AppleTV.png") };
        }
    }
}

