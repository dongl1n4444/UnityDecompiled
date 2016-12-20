namespace Unity.PackageManager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.DataContract;

    [Extension]
    public static class PackageInfoExtensions
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<string, PackageFileData>, bool> <>f__am$cache0;

        [Extension]
        public static PackageInfo Refresh(PackageInfo info)
        {
            if (info != null)
            {
                RefreshReleaseNotes(info);
            }
            return info;
        }

        private static void RefreshReleaseNotes(PackageInfo package)
        {
            if (string.IsNullOrEmpty(package.releaseNotes))
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<KeyValuePair<string, PackageFileData>, bool>(null, (IntPtr) <RefreshReleaseNotes>m__0);
                }
                KeyValuePair<string, PackageFileData> pair = Enumerable.FirstOrDefault<KeyValuePair<string, PackageFileData>>(package.files, <>f__am$cache0);
                if (pair.Key == null)
                {
                    package.releaseNotes = "None";
                }
                else
                {
                    string basePath;
                    if (package.basePath != null)
                    {
                        basePath = package.basePath;
                    }
                    else
                    {
                        basePath = Settings.installLocation;
                    }
                    basePath = Path.Combine(basePath, pair.Key);
                    if (File.Exists(basePath))
                    {
                        try
                        {
                            package.releaseNotes = File.ReadAllText(basePath);
                        }
                        catch (Exception)
                        {
                            package.releaseNotes = "Refreshing...";
                        }
                        if (string.IsNullOrEmpty(package.releaseNotes))
                        {
                            package.releaseNotes = "None";
                        }
                    }
                }
            }
        }
    }
}

