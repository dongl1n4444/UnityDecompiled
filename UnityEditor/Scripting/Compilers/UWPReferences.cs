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
    using UnityEditor;
    using UnityEditorInternal;

    internal static class UWPReferences
    {
        private static string CombinePaths(params string[] paths)
        {
            return FileUtil.CombinePaths(paths);
        }

        private static UWPExtension[] GetExtensions(string folder, string version)
        {
            List<UWPExtension> list = new List<UWPExtension>();
            string referencesFolder = Path.Combine(folder, "References");
            foreach (UWPExtensionSDK nsdk in GetExtensionSDKs(folder, version))
            {
                try
                {
                    UWPExtension item = new UWPExtension(nsdk.ManifestPath, referencesFolder);
                    list.Add(item);
                }
                catch
                {
                }
            }
            return list.ToArray();
        }

        public static IEnumerable<UWPExtensionSDK> GetExtensionSDKs()
        {
            string str;
            string str2;
            GetSDKFolderAndVersion(out str, out str2);
            return GetExtensionSDKs(str, str2);
        }

        private static IEnumerable<UWPExtensionSDK> GetExtensionSDKs(string sdkFolder, string sdkVersion)
        {
            List<UWPExtensionSDK> list = new List<UWPExtensionSDK>();
            string path = Path.Combine(sdkFolder, "Extension SDKs");
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

        private static string[] GetPlatform(string folder, string version)
        {
            string referencesFolder = Path.Combine(folder, "References");
            string[] paths = new string[] { folder, @"Platforms\UAP", version, "Platform.xml" };
            string uri = FileUtil.CombinePaths(paths);
            XElement element = XDocument.Load(uri).Element("ApplicationPlatform");
            if (element.Attribute("name").Value != "UAP")
            {
                throw new Exception(string.Format("Invalid platform manifest at \"{0}\".", uri));
            }
            XElement containedApiContractsElement = element.Element("ContainedApiContracts");
            return GetReferences(referencesFolder, containedApiContractsElement);
        }

        public static string[] GetReferences()
        {
            string str;
            string str2;
            GetSDKFolderAndVersion(out str, out str2);
            HashSet<string> source = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            string item = Path.Combine(str, @"UnionMetadata\Facade\Windows.winmd");
            source.Add(item);
            foreach (string str4 in GetPlatform(str, str2))
            {
                source.Add(str4);
            }
            foreach (UWPExtension extension in GetExtensions(str, str2))
            {
                foreach (string str5 in extension.References)
                {
                    source.Add(str5);
                }
            }
            return Enumerable.ToArray<string>(source);
        }

        private static string[] GetReferences(string referencesFolder, XElement containedApiContractsElement)
        {
            List<string> list = new List<string>();
            foreach (XElement element in containedApiContractsElement.Elements("ApiContract"))
            {
                string str = element.Attribute("name").Value;
                string str2 = element.Attribute("version").Value;
                string[] paths = new string[] { referencesFolder, str, str2, str + ".winmd" };
                string path = FileUtil.CombinePaths(paths);
                if (File.Exists(path))
                {
                    list.Add(path);
                }
            }
            return list.ToArray();
        }

        private static void GetSDKFolderAndVersion(out string sdkFolder, out string sdkVersion)
        {
            Version version;
            GetWindowsKit10(out sdkFolder, out version);
            sdkVersion = version.ToString();
            if (version.Minor == -1)
            {
                sdkVersion = sdkVersion + ".0";
            }
            if (version.Build == -1)
            {
                sdkVersion = sdkVersion + ".0";
            }
            if (version.Revision == -1)
            {
                sdkVersion = sdkVersion + ".0";
            }
        }

        private static void GetWindowsKit10(out string folder, out Version version)
        {
            string environmentVariable = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            folder = Path.Combine(environmentVariable, @"Windows Kits\10\");
            version = new Version(10, 0, 0x2800);
            try
            {
                folder = RegistryUtil.GetRegistryStringValue32(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v10.0", "InstallationFolder", folder);
                string str2 = RegistryUtil.GetRegistryStringValue32(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v10.0", "ProductVersion", version.ToString());
                version = new Version(str2);
            }
            catch
            {
            }
        }

        private sealed class UWPExtension
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string <Name>k__BackingField;
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private string[] <References>k__BackingField;

            public UWPExtension(string manifest, string referencesFolder)
            {
                XElement element = XDocument.Load(manifest).Element("FileList");
                if (element.Attribute("TargetPlatform").Value != "UAP")
                {
                    throw new Exception(string.Format("Invalid extension manifest at \"{0}\".", manifest));
                }
                this.Name = element.Attribute("DisplayName").Value;
                XElement containedApiContractsElement = element.Element("ContainedApiContracts");
                this.References = UWPReferences.GetReferences(referencesFolder, containedApiContractsElement);
            }

            public string Name { get; private set; }

            public string[] References { get; private set; }
        }
    }
}

