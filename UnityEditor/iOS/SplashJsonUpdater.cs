namespace UnityEditor.iOS
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.iOS.Xcode;

    internal class SplashJsonUpdater
    {
        private static void AddSplashEntry(JsonElementArray entries, SplashScreen splash)
        {
            if (Utils.CompareVersionNumbers(splash.minOsVersion, "7.0") <= 0)
            {
                AddSplashEntryForOSVersion(entries.AddDict(), splash, null);
            }
            if (splash.minOsVersion != null)
            {
                AddSplashEntryForOSVersion(entries.AddDict(), splash, splash.minOsVersion);
            }
        }

        private static void AddSplashEntryForOSVersion(JsonElementDict entry, SplashScreen splash, string minOsVersion)
        {
            entry.SetString("orientation", !splash.isPortrait ? "landscape" : "portrait");
            entry.SetString("idiom", FileUpdaterUtils.GetDeviceIdiomForJson(splash.deviceType));
            entry.SetString("filename", splash.xcodeFile);
            if (minOsVersion != null)
            {
                entry.SetString("minimum-system-version", splash.minOsVersion);
            }
            if (splash.subtype != null)
            {
                entry.SetString("subtype", splash.subtype);
            }
            entry.SetString("extent", "full-screen");
            entry.SetString("scale", $"{splash.scale}x");
        }

        internal static string CreateJsonString(List<SplashScreen> splashes)
        {
            JsonDocument document = new JsonDocument {
                indentString = "\t"
            };
            JsonElementArray entries = document.root.CreateArray("images");
            foreach (SplashScreen screen in splashes)
            {
                AddSplashEntry(entries, screen);
            }
            JsonElementDict dict = document.root.CreateDict("info");
            dict.SetInteger("version", 1);
            dict.SetString("author", "xcode");
            return document.WriteToString();
        }
    }
}

