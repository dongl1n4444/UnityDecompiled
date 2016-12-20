namespace UnityEditor.RestService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditorInternal;
    using UnityEngine;

    internal class ProjectStateRestHandler : Handler
    {
        [CompilerGenerated]
        private static Func<MonoIsland, Island> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Island, IEnumerable<string>> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<Island, JSONValue> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<JSONValue, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache4;

        private static string[] FindEmptyDirectories(string path, string[] files)
        {
            <FindEmptyDirectories>c__AnonStorey2 storey = new <FindEmptyDirectories>c__AnonStorey2 {
                files = files
            };
            return Enumerable.ToArray<string>(Enumerable.Where<string>(FindPotentialEmptyDirectories(path), new Func<string, bool>(storey, (IntPtr) this.<>m__0)));
        }

        private static IEnumerable<string> FindPotentialEmptyDirectories(string path)
        {
            List<string> result = new List<string>();
            FindPotentialEmptyDirectories(path, result);
            return result;
        }

        private static void FindPotentialEmptyDirectories(string path, ICollection<string> result)
        {
            string[] directories = Directory.GetDirectories(path);
            if (directories.Length == 0)
            {
                result.Add(path.Replace('\\', '/'));
            }
            else
            {
                foreach (string str in directories)
                {
                    FindPotentialEmptyDirectories(str, result);
                }
            }
        }

        private static IEnumerable<string> GetAllSupportedFiles()
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = new Func<string, bool>(null, (IntPtr) <GetAllSupportedFiles>m__4);
            }
            return Enumerable.Where<string>(AssetDatabase.GetAllAssetPaths(), <>f__am$cache4);
        }

        protected override JSONValue HandleGet(Request request, JSONValue payload)
        {
            AssetDatabase.Refresh();
            return JsonForProject();
        }

        private static bool IsSupportedExtension(string extension)
        {
            <IsSupportedExtension>c__AnonStorey1 storey = new <IsSupportedExtension>c__AnonStorey1 {
                extension = extension
            };
            if (storey.extension.StartsWith("."))
            {
                storey.extension = storey.extension.Substring(1);
            }
            return Enumerable.Any<string>(Enumerable.Concat<string>(EditorSettings.projectGenerationBuiltinExtensions, EditorSettings.projectGenerationUserExtensions), new Func<string, bool>(storey, (IntPtr) this.<>m__0));
        }

        private static JSONValue JsonForIsland(Island island)
        {
            if ((island.Name == "UnityEngine") || (island.Name == "UnityEditor"))
            {
                return 0;
            }
            JSONValue value3 = new JSONValue();
            value3["name"] = island.Name;
            value3["language"] = !island.Name.Contains("Boo") ? (!island.Name.Contains("UnityScript") ? "C#" : "UnityScript") : "Boo";
            value3["files"] = Handler.ToJSON(island.MonoIsland._files);
            value3["defines"] = Handler.ToJSON(island.MonoIsland._defines);
            value3["references"] = Handler.ToJSON(island.MonoIsland._references);
            value3["basedirectory"] = ProjectPath;
            return value3;
        }

        private static JSONValue JsonForProject()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<MonoIsland, Island>(null, (IntPtr) <JsonForProject>m__0);
            }
            List<Island> list = Enumerable.ToList<Island>(Enumerable.Select<MonoIsland, Island>(InternalEditorUtility.GetMonoIslands(), <>f__am$cache0));
            foreach (Island island in list)
            {
                List<string> list2 = new List<string>();
                List<string> list3 = new List<string>();
                foreach (string str in island.References)
                {
                    <JsonForProject>c__AnonStorey0 storey = new <JsonForProject>c__AnonStorey0 {
                        refName = Path.GetFileNameWithoutExtension(str)
                    };
                    if (str.StartsWith("Library/") && Enumerable.Any<Island>(list, new Func<Island, bool>(storey, (IntPtr) this.<>m__0)))
                    {
                        list2.Add(storey.refName);
                        list3.Add(str);
                    }
                    if ((str.EndsWith("/UnityEditor.dll") || str.EndsWith("/UnityEngine.dll")) || (str.EndsWith(@"\UnityEditor.dll") || str.EndsWith(@"\UnityEngine.dll")))
                    {
                        list3.Add(str);
                    }
                }
                island.References.Add(InternalEditorUtility.GetEditorAssemblyPath());
                island.References.Add(InternalEditorUtility.GetEngineAssemblyPath());
                foreach (string str2 in list2)
                {
                    island.References.Add(str2);
                }
                foreach (string str3 in list3)
                {
                    island.References.Remove(str3);
                }
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<Island, IEnumerable<string>>(null, (IntPtr) <JsonForProject>m__1);
            }
            string[] files = Enumerable.ToArray<string>(Enumerable.Distinct<string>(Enumerable.Concat<string>(Enumerable.SelectMany<Island, string>(list, <>f__am$cache1), GetAllSupportedFiles())));
            string[] strings = RelativeToProjectPath(FindEmptyDirectories(AssetsPath, files));
            JSONValue value2 = new JSONValue();
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<Island, JSONValue>(null, (IntPtr) <JsonForProject>m__2);
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<JSONValue, bool>(null, (IntPtr) <JsonForProject>m__3);
            }
            value2["islands"] = new JSONValue(Enumerable.ToList<JSONValue>(Enumerable.Where<JSONValue>(Enumerable.Select<Island, JSONValue>(list, <>f__am$cache2), <>f__am$cache3)));
            value2["basedirectory"] = ProjectPath;
            JSONValue value3 = new JSONValue();
            value3["files"] = Handler.ToJSON(files);
            value3["emptydirectories"] = Handler.ToJSON(strings);
            value2["assetdatabase"] = value3;
            return value2;
        }

        internal static void Register()
        {
            Router.RegisterHandler("/unity/projectstate", new ProjectStateRestHandler());
        }

        private static string[] RelativeToProjectPath(string[] paths)
        {
            <RelativeToProjectPath>c__AnonStorey4 storey = new <RelativeToProjectPath>c__AnonStorey4 {
                projectPath = !ProjectPath.EndsWith("/") ? (ProjectPath + "/") : ProjectPath
            };
            return Enumerable.ToArray<string>(Enumerable.Select<string, string>(paths, new Func<string, string>(storey, (IntPtr) this.<>m__0)));
        }

        private static string AssetsPath
        {
            get
            {
                return (ProjectPath + "/Assets");
            }
        }

        private static string ProjectPath
        {
            get
            {
                return Path.GetDirectoryName(Application.dataPath);
            }
        }

        [CompilerGenerated]
        private sealed class <FindEmptyDirectories>c__AnonStorey2
        {
            internal string[] files;

            internal bool <>m__0(string d)
            {
                <FindEmptyDirectories>c__AnonStorey3 storey = new <FindEmptyDirectories>c__AnonStorey3 {
                    <>f__ref$2 = this,
                    d = d
                };
                return !Enumerable.Any<string>(this.files, new Func<string, bool>(storey, (IntPtr) this.<>m__0));
            }

            private sealed class <FindEmptyDirectories>c__AnonStorey3
            {
                internal ProjectStateRestHandler.<FindEmptyDirectories>c__AnonStorey2 <>f__ref$2;
                internal string d;

                internal bool <>m__0(string f)
                {
                    return f.StartsWith(this.d);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <IsSupportedExtension>c__AnonStorey1
        {
            internal string extension;

            internal bool <>m__0(string s)
            {
                return string.Equals(s, this.extension, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        [CompilerGenerated]
        private sealed class <JsonForProject>c__AnonStorey0
        {
            internal string refName;

            internal bool <>m__0(ProjectStateRestHandler.Island i)
            {
                return (i.Name == this.refName);
            }
        }

        [CompilerGenerated]
        private sealed class <RelativeToProjectPath>c__AnonStorey4
        {
            internal string projectPath;

            internal string <>m__0(string d)
            {
                return (!d.StartsWith(this.projectPath) ? d : d.Substring(this.projectPath.Length));
            }
        }

        public class Island
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private UnityEditor.Scripting.MonoIsland <MonoIsland>k__BackingField;
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private string <Name>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private List<string> <References>k__BackingField;

            public UnityEditor.Scripting.MonoIsland MonoIsland { get; set; }

            public string Name { get; set; }

            public List<string> References { get; set; }
        }
    }
}

