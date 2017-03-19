namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Xml.Linq;
    using UnityEditor.Utils;
    using UnityEditorInternal;

    internal static class UWPReferences
    {
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache0;

        private static string CombinePaths(params string[] paths) => 
            Paths.Combine(paths);

        private static bool FindVersionInNode(XElement node, out Version version)
        {
            for (XAttribute attribute = node.FirstAttribute; attribute != null; attribute = attribute.NextAttribute)
            {
                if (string.Equals(attribute.Name.LocalName, "version", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        version = new Version(attribute.Value);
                        return true;
                    }
                    catch
                    {
                    }
                }
            }
            version = null;
            return false;
        }

        private static UWPExtension[] GetExtensions(string windowsKitsFolder, string version)
        {
            List<UWPExtension> list = new List<UWPExtension>();
            foreach (UWPExtensionSDK nsdk in GetExtensionSDKs(windowsKitsFolder, version))
            {
                try
                {
                    UWPExtension item = new UWPExtension(nsdk.ManifestPath, windowsKitsFolder, version);
                    list.Add(item);
                }
                catch
                {
                }
            }
            return list.ToArray();
        }

        public static IEnumerable<UWPExtensionSDK> GetExtensionSDKs(Version sdkVersion)
        {
            string str = GetWindowsKit10();
            if (string.IsNullOrEmpty(str))
            {
                return new UWPExtensionSDK[0];
            }
            return GetExtensionSDKs(str, SdkVersionToString(sdkVersion));
        }

        private static IEnumerable<UWPExtensionSDK> GetExtensionSDKs(string sdkFolder, string sdkVersion)
        {
            List<UWPExtensionSDK> list = new List<UWPExtensionSDK>();
            string path = Path.Combine(sdkFolder, "Extension SDKs");
            if (!Directory.Exists(path))
            {
                return new UWPExtensionSDK[0];
            }
            foreach (string str2 in Directory.GetDirectories(path))
            {
                string[] paths = new string[] { str2, sdkVersion, "SDKManifest.xml" };
                string str3 = CombinePaths(paths);
                string fileName = Path.GetFileName(str2);
                if (File.Exists(str3))
                {
                    list.Add(new UWPExtensionSDK(fileName, sdkVersion, str3));
                }
                else if (fileName == "XboxLive")
                {
                    string[] textArray2 = new string[] { str2, "1.0", "SDKManifest.xml" };
                    str3 = CombinePaths(textArray2);
                    if (File.Exists(str3))
                    {
                        list.Add(new UWPExtensionSDK(fileName, "1.0", str3));
                    }
                }
            }
            return list;
        }

        public static IEnumerable<Version> GetInstalledSDKVersions()
        {
            string str = GetWindowsKit10();
            if (string.IsNullOrEmpty(str))
            {
                return new Version[0];
            }
            string[] paths = new string[] { str, "Platforms", "UAP" };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = f => string.Equals("Platform.xml", Path.GetFileName(f), StringComparison.OrdinalIgnoreCase);
            }
            IEnumerable<string> enumerable2 = Enumerable.Where<string>(Directory.GetFiles(CombinePaths(paths), "*", SearchOption.AllDirectories), <>f__am$cache0);
            List<Version> list = new List<Version>();
            foreach (string str2 in enumerable2)
            {
                XDocument document;
                try
                {
                    document = XDocument.Load(str2);
                }
                catch
                {
                    continue;
                }
                foreach (XNode node in document.Nodes())
                {
                    Version version;
                    XElement element = node as XElement;
                    if ((element != null) && FindVersionInNode(element, out version))
                    {
                        list.Add(version);
                    }
                }
            }
            return list;
        }

        private static string[] GetPlatform(string folder, string version)
        {
            string[] paths = new string[] { folder, @"Platforms\UAP", version, "Platform.xml" };
            string uri = CombinePaths(paths);
            XElement element = XDocument.Load(uri).Element("ApplicationPlatform");
            if (element.Attribute("name").Value != "UAP")
            {
                throw new Exception($"Invalid platform manifest at "{uri}".");
            }
            XElement containedApiContractsElement = element.Element("ContainedApiContracts");
            return GetReferences(folder, version, containedApiContractsElement);
        }

        public static string[] GetReferences(Version sdkVersion)
        {
            string str = GetWindowsKit10();
            if (string.IsNullOrEmpty(str))
            {
                return new string[0];
            }
            string version = SdkVersionToString(sdkVersion);
            HashSet<string> source = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            string[] paths = new string[] { str, "UnionMetadata", version, "Facade", "Windows.winmd" };
            string path = CombinePaths(paths);
            if (!File.Exists(path))
            {
                string[] textArray2 = new string[] { str, "UnionMetadata", "Facade", "Windows.winmd" };
                path = CombinePaths(textArray2);
            }
            source.Add(path);
            foreach (string str4 in GetPlatform(str, version))
            {
                source.Add(str4);
            }
            foreach (UWPExtension extension in GetExtensions(str, version))
            {
                foreach (string str5 in extension.References)
                {
                    source.Add(str5);
                }
            }
            return source.ToArray<string>();
        }

        private static string[] GetReferences(string windowsKitsFolder, string sdkVersion, XElement containedApiContractsElement)
        {
            List<string> list = new List<string>();
            foreach (XElement element in containedApiContractsElement.Elements("ApiContract"))
            {
                string str = element.Attribute("name").Value;
                string str2 = element.Attribute("version").Value;
                string[] paths = new string[] { windowsKitsFolder, "References", sdkVersion, str, str2, str + ".winmd" };
                string path = CombinePaths(paths);
                if (!File.Exists(path))
                {
                    string[] textArray2 = new string[] { windowsKitsFolder, "References", str, str2, str + ".winmd" };
                    path = CombinePaths(textArray2);
                    if (!File.Exists(path))
                    {
                        continue;
                    }
                }
                list.Add(path);
            }
            return list.ToArray();
        }

        private static string GetWindowsKit10()
        {
            string defaultValue = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), @"Windows Kits\10\");
            try
            {
                defaultValue = RegistryUtil.GetRegistryStringValue(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v10.0", "InstallationFolder", defaultValue, RegistryView._32);
            }
            catch
            {
            }
            if (!Directory.Exists(defaultValue))
            {
                return string.Empty;
            }
            return defaultValue;
        }

        private static string SdkVersionToString(Version version)
        {
            string str = version.ToString();
            if (version.Minor == -1)
            {
                str = str + ".0";
            }
            if (version.Build == -1)
            {
                str = str + ".0";
            }
            if (version.Revision == -1)
            {
                str = str + ".0";
            }
            return str;
        }

        private sealed class UWPExtension
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string <Name>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string[] <References>k__BackingField;

            public UWPExtension(string manifest, string windowsKitsFolder, string sdkVersion)
            {
                XElement element = XDocument.Load(manifest).Element("FileList");
                if (element.Attribute("TargetPlatform").Value != "UAP")
                {
                    throw new Exception($"Invalid extension manifest at "{manifest}".");
                }
                this.Name = element.Attribute("DisplayName").Value;
                XElement containedApiContractsElement = element.Element("ContainedApiContracts");
                this.References = UWPReferences.GetReferences(windowsKitsFolder, sdkVersion, containedApiContractsElement);
            }

            public string Name { get; private set; }

            public string[] References { get; private set; }
        }
    }
}

