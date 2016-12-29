namespace UnityEditor.iOS
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.iOS.Xcode;

    internal class IconJsonUpdater
    {
        internal static string CreateJsonString(List<Icon> icons, bool prerendered)
        {
            JsonDocument document = new JsonDocument {
                indentString = "\t"
            };
            JsonElementArray array = document.root.CreateArray("images");
            foreach (Icon icon in icons)
            {
                JsonElementDict dict = array.AddDict();
                dict.SetString("size", $"{icon.width}x{icon.height}");
                dict.SetString("idiom", FileUpdaterUtils.GetDeviceIdiomForJson(icon.deviceType));
                dict.SetString("filename", icon.xcodefile);
                dict.SetString("scale", $"{icon.scale}x");
            }
            JsonElementDict dict2 = document.root.CreateDict("info");
            dict2.SetInteger("version", 1);
            dict2.SetString("author", "xcode");
            document.root.CreateDict("properties").SetBoolean("pre-rendered", prerendered);
            return document.WriteToString();
        }
    }
}

