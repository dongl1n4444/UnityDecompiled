namespace UnityEditor.iOS
{
    using System;

    internal class FileUpdaterUtils
    {
        public static string GetDeviceIdiomForJson(DeviceType deviceType)
        {
            if (deviceType != DeviceType.iPad)
            {
                if (deviceType == DeviceType.iPhone)
                {
                    return "iphone";
                }
                if (deviceType == DeviceType.AppleTV)
                {
                    return "tv";
                }
            }
            else
            {
                return "ipad";
            }
            return "unknown";
        }
    }
}

