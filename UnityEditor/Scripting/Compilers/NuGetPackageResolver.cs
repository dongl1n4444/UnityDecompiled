namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEditor;

    internal sealed class NuGetPackageResolver
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <PackagesDirectory>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <ProjectLockFile>k__BackingField;
        [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        private string[] <ResolvedReferences>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <TargetMoniker>k__BackingField;

        public NuGetPackageResolver()
        {
            this.TargetMoniker = "UAP,Version=v10.0";
        }

        private string ConvertToWindowsPath(string path) => 
            path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

        private string GetPackagesPath()
        {
            string packagesDirectory = this.PackagesDirectory;
            if (!string.IsNullOrEmpty(packagesDirectory))
            {
                return packagesDirectory;
            }
            packagesDirectory = Environment.GetEnvironmentVariable("NUGET_PACKAGES");
            if (!string.IsNullOrEmpty(packagesDirectory))
            {
                return packagesDirectory;
            }
            return Path.Combine(Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), ".nuget"), "packages");
        }

        public string[] Resolve()
        {
            Dictionary<string, object> dictionary = (Dictionary<string, object>) Json.Deserialize(File.ReadAllText(this.ProjectLockFile));
            Dictionary<string, object> dictionary2 = (Dictionary<string, object>) dictionary["targets"];
            Dictionary<string, object> dictionary3 = (Dictionary<string, object>) dictionary2[this.TargetMoniker];
            List<string> list = new List<string>();
            string str2 = this.ConvertToWindowsPath(this.GetPackagesPath());
            foreach (KeyValuePair<string, object> pair in dictionary3)
            {
                object obj2;
                Dictionary<string, object> dictionary4 = (Dictionary<string, object>) pair.Value;
                if (dictionary4.TryGetValue("compile", out obj2))
                {
                    Dictionary<string, object> dictionary5 = (Dictionary<string, object>) obj2;
                    char[] separator = new char[] { '/' };
                    string[] strArray = pair.Key.Split(separator);
                    string str3 = strArray[0];
                    string str4 = strArray[1];
                    string path = Path.Combine(Path.Combine(str2, str3), str4);
                    if (!Directory.Exists(path))
                    {
                        throw new Exception($"Package directory not found: "{path}".");
                    }
                    foreach (string str6 in dictionary5.Keys)
                    {
                        if (!string.Equals(Path.GetFileName(str6), "_._", StringComparison.InvariantCultureIgnoreCase))
                        {
                            string str7 = Path.Combine(path, this.ConvertToWindowsPath(str6));
                            if (!File.Exists(str7))
                            {
                                throw new Exception($"Reference not found: "{str7}".");
                            }
                            list.Add(str7);
                        }
                    }
                    if (dictionary4.ContainsKey("frameworkAssemblies"))
                    {
                        throw new NotImplementedException("Support for \"frameworkAssemblies\" property has not been implemented yet.");
                    }
                }
            }
            this.ResolvedReferences = list.ToArray();
            return this.ResolvedReferences;
        }

        public string PackagesDirectory { get; set; }

        public string ProjectLockFile { get; set; }

        public string[] ResolvedReferences { get; private set; }

        public string TargetMoniker { get; set; }
    }
}

