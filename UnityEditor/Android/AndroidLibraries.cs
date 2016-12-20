namespace UnityEditor.Android
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;

    internal class AndroidLibraries : HashSet<string>
    {
        private const string ASSETS_DIR = "assets";
        private const string COMPILED_JAR_FILES = "bin/*.jar";
        private const string DEFAULT_PROPERTIES = "default.properties";
        private const string LIBRARY_DIR = "libs";
        public static readonly string ProjectPropertiesFileName = "project.properties";
        private static readonly Regex PROP_ISLIBRARYPROJECT = new Regex(@"^android\.library.*=.*true", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private const RegexOptions REGEXOPT_MULTILINEIGNORECASE = (RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private const string RESOURCES_DIR = "res";
        private static readonly Encoding UTF8_ENCODING = new UTF8Encoding(false);

        public bool AddLibraryProject(string projectPropertiesPath)
        {
            if (!IsLibraryProject(projectPropertiesPath))
            {
                Debug.LogWarning(string.Format("Project '{0}' is not an android library.", Directory.GetParent(projectPropertiesPath)));
                return false;
            }
            DirectoryInfo parent = Directory.GetParent(projectPropertiesPath);
            if (!File.Exists(Path.Combine(parent.ToString(), "AndroidManifest.xml")))
            {
                Debug.LogError(string.Format("Project '{0}' is missing {1} file.", parent, "AndroidManifest.xml"));
                throw new UnityException("Adding Android library projects failed!");
            }
            base.Add(parent.FullName);
            return true;
        }

        private string[] Find(string searchPattern)
        {
            List<string> result = new List<string>();
            foreach (string str in this)
            {
                AndroidFileLocator.Find(Path.Combine(str, searchPattern), result);
            }
            return result.ToArray();
        }

        public int FindAndAddLibraryProjects(string searchPattern)
        {
            base.Clear();
            int num = 0;
            string[] strArray = new string[] { Path.Combine(searchPattern, ProjectPropertiesFileName), Path.Combine(searchPattern, "default.properties") };
            foreach (string str in strArray)
            {
                foreach (string str2 in AndroidFileLocator.Find(str))
                {
                    if (this.AddLibraryProject(str2))
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public string[] GetAssetsDirectories()
        {
            return this.Find("assets");
        }

        public string[] GetCompiledJarFiles()
        {
            return this.Find("bin/*.jar");
        }

        public string[] GetLibraryDirectories()
        {
            return this.Find("libs");
        }

        public string[] GetManifestFiles()
        {
            return this.Find("AndroidManifest.xml");
        }

        public string[] GetPackageNames()
        {
            List<string> list = new List<string>();
            foreach (string str in this.GetManifestFiles())
            {
                list.Add(new AndroidManifest(str).packageName);
            }
            return list.ToArray();
        }

        public string[] GetResourceDirectories()
        {
            return this.Find("res");
        }

        public static bool IsAndroidLibraryProject(string libraryPath)
        {
            return (IsLibraryProject(Path.Combine(libraryPath, ProjectPropertiesFileName)) || IsLibraryProject(Path.Combine(libraryPath, "default.properties")));
        }

        private static bool IsLibraryProject(string projectPropertiesPath)
        {
            if (!File.Exists(projectPropertiesPath))
            {
                return false;
            }
            string input = File.ReadAllText(projectPropertiesPath, UTF8_ENCODING);
            return PROP_ISLIBRARYPROJECT.IsMatch(input);
        }
    }
}

