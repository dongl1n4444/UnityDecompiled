namespace UnityEditor.PlaymodeTestsRunner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal static class EditorReferencesUtil
    {
        private static bool ADependsOnB(UnityEngine.Object obj, UnityEngine.Object selectedObj)
        {
            if (selectedObj != null)
            {
                if (selectedObj == obj)
                {
                    return false;
                }
                UnityEngine.Object[] roots = new UnityEngine.Object[] { obj };
                UnityEngine.Object[] objArray = EditorUtility.CollectDependencies(roots);
                if (objArray.Length < 2)
                {
                    return false;
                }
                foreach (UnityEngine.Object obj2 in objArray)
                {
                    if (obj2 == selectedObj)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static string CleanPathSeparators(string s) => 
            s.Replace(@"\", "/");

        private static List<FileInfo> DirSearch(DirectoryInfo d, string searchFor)
        {
            List<FileInfo> list = d.GetFiles(searchFor).ToList<FileInfo>();
            DirectoryInfo[] directories = d.GetDirectories();
            foreach (DirectoryInfo info in directories)
            {
                list.AddRange(DirSearch(info, searchFor));
            }
            return list;
        }

        public static List<UnityEngine.Object> FindScenesWhichContainAsset(string file)
        {
            <FindScenesWhichContainAsset>c__AnonStorey0 storey = new <FindScenesWhichContainAsset>c__AnonStorey0();
            string assetPathFromFileNameAndExtension = GetAssetPathFromFileNameAndExtension(file);
            storey.cur = AssetDatabase.LoadAssetAtPath(assetPathFromFileNameAndExtension, typeof(UnityEngine.Object));
            return Enumerable.Where<UnityEngine.Object>(AllScenes, new Func<UnityEngine.Object, bool>(storey.<>m__0)).ToList<UnityEngine.Object>();
        }

        private static string GetAssetPathFromFileNameAndExtension(string assetName)
        {
            string[] strArray = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(assetName));
            string str = null;
            foreach (string str2 in strArray)
            {
                string path = AssetDatabase.GUIDToAssetPath(str2);
                if (Path.GetFileName(path) == Path.GetFileName(assetName))
                {
                    str = path;
                }
            }
            return str;
        }

        private static string GetRelativeAssetPathFromFullPath(string fullPath)
        {
            fullPath = CleanPathSeparators(fullPath);
            if (fullPath.Contains(Application.dataPath))
            {
                return fullPath.Replace(Application.dataPath, "Assets");
            }
            Debug.LogWarning("Path does not point to a location within Assets: " + fullPath);
            return null;
        }

        private static List<UnityEngine.Object> AllScenes
        {
            get
            {
                List<FileInfo> list = DirSearch(new DirectoryInfo(Application.dataPath), "*.unity");
                List<UnityEngine.Object> list2 = new List<UnityEngine.Object>();
                foreach (FileInfo info in list)
                {
                    if (!info.Name.StartsWith("."))
                    {
                        list2.Add(AssetDatabase.LoadMainAssetAtPath(GetRelativeAssetPathFromFullPath(info.FullName)));
                    }
                }
                return list2;
            }
        }

        [CompilerGenerated]
        private sealed class <FindScenesWhichContainAsset>c__AnonStorey0
        {
            internal UnityEngine.Object cur;

            internal bool <>m__0(UnityEngine.Object a) => 
                EditorReferencesUtil.ADependsOnB(a, this.cur);
        }
    }
}

