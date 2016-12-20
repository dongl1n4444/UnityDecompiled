namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    public class GenerateIconsWithMipLevels
    {
        [CompilerGenerated]
        private static Comparison<Texture2D> <>f__am$cache0;
        private static string k_IconMipIdentifier = "@";
        private static string k_IconSourceFolder = "Assets/MipLevels For Icons/";
        private static string k_IconTargetFolder = "Assets/Editor Default Resources/Icons/Processed";

        private static void Blit(Texture2D source, Texture2D dest, int mipLevel)
        {
            Color32[] colors = source.GetPixels32();
            for (int i = 0; i < colors.Length; i++)
            {
                Color32 color = colors[i];
                if (color.a >= 3)
                {
                    color.a = (byte) (color.a - 3);
                }
                colors[i] = color;
            }
            dest.SetPixels32(colors, mipLevel);
        }

        private static bool BlitMip(Texture2D iconWithMips, List<Texture2D> sortedTextures, int mipLevel)
        {
            if ((mipLevel < 0) || (mipLevel >= sortedTextures.Count))
            {
                Debug.LogError("Invalid mip level: " + mipLevel);
                return false;
            }
            Texture2D source = sortedTextures[mipLevel];
            if (source != null)
            {
                Blit(source, iconWithMips, mipLevel);
                return true;
            }
            Debug.LogError("No texture at mip level: " + mipLevel);
            return false;
        }

        private static Texture2D CreateIconWithMipLevels(InputData inputData, string baseName, List<string> assetPathsOfAllIcons, Dictionary<int, Texture2D> mipTextures)
        {
            <CreateIconWithMipLevels>c__AnonStorey0 storey = new <CreateIconWithMipLevels>c__AnonStorey0 {
                baseName = baseName,
                inputData = inputData
            };
            List<string> list = assetPathsOfAllIcons.FindAll(new Predicate<string>(storey.<>m__0));
            List<Texture2D> sortedTextures = new List<Texture2D>();
            foreach (string str in list)
            {
                int key = MipLevelForAssetPath(str, storey.inputData.mipIdentifier);
                Texture2D item = null;
                if ((mipTextures != null) && mipTextures.ContainsKey(key))
                {
                    item = mipTextures[key];
                }
                else
                {
                    item = GetTexture2D(str);
                }
                if (item != null)
                {
                    sortedTextures.Add(item);
                }
                else
                {
                    Debug.LogError("Mip not found " + str);
                }
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = delegate (Texture2D first, Texture2D second) {
                    if (first.width == second.width)
                    {
                        return 0;
                    }
                    if (first.width < second.width)
                    {
                        return 1;
                    }
                    return -1;
                };
            }
            sortedTextures.Sort(<>f__am$cache0);
            int num2 = 0x1869f;
            int width = 0;
            foreach (Texture2D textured2 in sortedTextures)
            {
                int num4 = textured2.width;
                if (num4 > width)
                {
                    width = num4;
                }
                if (num4 < num2)
                {
                    num2 = num4;
                }
            }
            if (width == 0)
            {
                return null;
            }
            Texture2D iconWithMips = new Texture2D(width, width, TextureFormat.RGBA32, true, true);
            if (BlitMip(iconWithMips, sortedTextures, 0))
            {
                iconWithMips.Apply(true);
            }
            else
            {
                return iconWithMips;
            }
            int num5 = width;
            for (int i = 1; i < iconWithMips.mipmapCount; i++)
            {
                num5 /= 2;
                if (num5 < num2)
                {
                    break;
                }
                BlitMip(iconWithMips, sortedTextures, i);
            }
            iconWithMips.Apply(false, true);
            return iconWithMips;
        }

        private static void DeleteFile(string file)
        {
            if (AssetDatabase.GetMainAssetInstanceID(file) != 0)
            {
                Debug.Log("Deleted unused file: " + file);
                AssetDatabase.DeleteAsset(file);
            }
        }

        private static void EnsureFolderIsCreated(string targetFolder)
        {
            if (AssetDatabase.GetMainAssetInstanceID(targetFolder) == 0)
            {
                Debug.Log("Created target folder " + targetFolder);
                AssetDatabase.CreateFolder(Path.GetDirectoryName(targetFolder), Path.GetFileName(targetFolder));
            }
        }

        private static void EnsureFolderIsCreatedRecursively(string targetFolder)
        {
            if (AssetDatabase.GetMainAssetInstanceID(targetFolder) == 0)
            {
                EnsureFolderIsCreatedRecursively(Path.GetDirectoryName(targetFolder));
                Debug.Log("Created target folder " + targetFolder);
                AssetDatabase.CreateFolder(Path.GetDirectoryName(targetFolder), Path.GetFileName(targetFolder));
            }
        }

        public static void GenerateAllIconsWithMipLevels()
        {
            InputData inputData = GetInputData();
            EnsureFolderIsCreated(inputData.targetFolder);
            float realtimeSinceStartup = Time.realtimeSinceStartup;
            GenerateIconsWithMips(inputData);
            Debug.Log(string.Format("Generated {0} icons with mip levels in {1} seconds", inputData.generatedFileNames.Count, Time.realtimeSinceStartup - realtimeSinceStartup));
            RemoveUnusedFiles(inputData.generatedFileNames);
            AssetDatabase.Refresh();
            InternalEditorUtility.RepaintAllViews();
        }

        private static bool GenerateIcon(InputData inputData, string baseName, List<string> assetPathsOfAllIcons, Dictionary<int, Texture2D> mipTextures, FileInfo sourceFileInfo)
        {
            string path = inputData.targetFolder + "/" + baseName + " Icon.asset";
            if ((sourceFileInfo != null) && File.Exists(path))
            {
                FileInfo info = new FileInfo(path);
                if (info.LastWriteTime > sourceFileInfo.LastWriteTime)
                {
                    return false;
                }
            }
            Debug.Log("Generating MIP levels for " + path);
            EnsureFolderIsCreatedRecursively(Path.GetDirectoryName(path));
            Texture2D asset = CreateIconWithMipLevels(inputData, baseName, assetPathsOfAllIcons, mipTextures);
            if (asset == null)
            {
                Debug.Log("CreateIconWithMipLevels failed");
                return false;
            }
            asset.name = baseName + " Icon.png";
            AssetDatabase.CreateAsset(asset, path);
            inputData.generatedFileNames.Add(path);
            return true;
        }

        private static void GenerateIconsWithMips(InputData inputData)
        {
            List<string> files = GetIconAssetPaths(inputData.sourceFolder, inputData.mipIdentifier, inputData.mipFileExtension);
            if (files.Count == 0)
            {
                Debug.LogWarning("No mip files found for generating icons! Searching in: " + inputData.sourceFolder + ", for files with extension: " + inputData.mipFileExtension);
            }
            string[] baseNames = GetBaseNames(inputData, files);
            foreach (string str in baseNames)
            {
                GenerateIcon(inputData, str, files, null, null);
            }
        }

        public static void GenerateIconWithMipLevels(string assetPath, Dictionary<int, Texture2D> mipTextures, FileInfo fileInfo)
        {
            if (VerifyIconPath(assetPath, true))
            {
                InputData inputData = GetInputData();
                float realtimeSinceStartup = Time.realtimeSinceStartup;
                string baseName = assetPath.Replace(inputData.sourceFolder, "");
                baseName = baseName.Substring(0, baseName.LastIndexOf(inputData.mipIdentifier));
                List<string> assetPathsOfAllIcons = GetIconAssetPaths(inputData.sourceFolder, inputData.mipIdentifier, inputData.mipFileExtension);
                EnsureFolderIsCreated(inputData.targetFolder);
                if (GenerateIcon(inputData, baseName, assetPathsOfAllIcons, mipTextures, fileInfo))
                {
                    Debug.Log(string.Format("Generated {0} icon with mip levels in {1} seconds", baseName, Time.realtimeSinceStartup - realtimeSinceStartup));
                }
                InternalEditorUtility.RepaintAllViews();
            }
        }

        public static void GenerateSelectedIconsWithMips()
        {
            if (Selection.activeInstanceID == 0)
            {
                Debug.Log("Ensure to select a mip texture..." + Selection.activeInstanceID);
            }
            else
            {
                InputData inputData = GetInputData();
                string assetPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
                if (VerifyIconPath(assetPath, true))
                {
                    float realtimeSinceStartup = Time.realtimeSinceStartup;
                    string baseName = assetPath.Replace(inputData.sourceFolder, "");
                    baseName = baseName.Substring(0, baseName.LastIndexOf(inputData.mipIdentifier));
                    List<string> assetPathsOfAllIcons = GetIconAssetPaths(inputData.sourceFolder, inputData.mipIdentifier, inputData.mipFileExtension);
                    EnsureFolderIsCreated(inputData.targetFolder);
                    GenerateIcon(inputData, baseName, assetPathsOfAllIcons, null, null);
                    Debug.Log(string.Format("Generated {0} icon with mip levels in {1} seconds", baseName, Time.realtimeSinceStartup - realtimeSinceStartup));
                    InternalEditorUtility.RepaintAllViews();
                }
            }
        }

        private static string[] GetBaseNames(InputData inputData, List<string> files)
        {
            string[] collection = new string[files.Count];
            int length = inputData.sourceFolder.Length;
            for (int i = 0; i < files.Count; i++)
            {
                collection[i] = files[i].Substring(length, files[i].IndexOf(inputData.mipIdentifier) - length);
            }
            HashSet<string> set = new HashSet<string>(collection);
            collection = new string[set.Count];
            set.CopyTo(collection);
            return collection;
        }

        private static List<string> GetIconAssetPaths(string folderPath, string mustHaveIdentifier, string extension)
        {
            <GetIconAssetPaths>c__AnonStorey1 storey = new <GetIconAssetPaths>c__AnonStorey1 {
                mustHaveIdentifier = mustHaveIdentifier
            };
            string uriString = Path.Combine(Directory.GetCurrentDirectory(), folderPath);
            Uri uri = new Uri(uriString);
            List<string> list = new List<string>(Directory.GetFiles(uriString, "*." + extension, SearchOption.AllDirectories));
            list.RemoveAll(new Predicate<string>(storey.<>m__0));
            for (int i = 0; i < list.Count; i++)
            {
                Uri uri2 = new Uri(list[i]);
                list[i] = folderPath + uri.MakeRelativeUri(uri2).ToString();
            }
            return list;
        }

        private static InputData GetInputData()
        {
            return new InputData { 
                sourceFolder = k_IconSourceFolder,
                targetFolder = k_IconTargetFolder,
                mipIdentifier = k_IconMipIdentifier,
                mipFileExtension = "png"
            };
        }

        private static Texture2D GetTexture2D(string path)
        {
            return (AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D);
        }

        public static int MipLevelForAssetPath(string assetPath, string separator)
        {
            if (string.IsNullOrEmpty(assetPath) || string.IsNullOrEmpty(separator))
            {
                return -1;
            }
            int index = assetPath.IndexOf(separator);
            if (index == -1)
            {
                Debug.LogError("\"" + separator + "\" could not be found in asset path: " + assetPath);
                return -1;
            }
            int startIndex = index + separator.Length;
            int num4 = assetPath.IndexOf(".", startIndex);
            if (num4 == -1)
            {
                Debug.LogError("Could not find path extension in asset path: " + assetPath);
                return -1;
            }
            return int.Parse(assetPath.Substring(startIndex, num4 - startIndex));
        }

        private static void RemoveUnusedFiles(List<string> generatedFiles)
        {
            for (int i = 0; i < generatedFiles.Count; i++)
            {
                string file = generatedFiles[i].Replace("Icons/Processed", "Icons").Replace(".asset", ".png");
                DeleteFile(file);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                if (!fileNameWithoutExtension.StartsWith("d_"))
                {
                    DeleteFile(file.Replace(fileNameWithoutExtension, "d_" + fileNameWithoutExtension));
                }
            }
            AssetDatabase.Refresh();
        }

        public static bool VerifyIconPath(string assetPath, bool logError)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return false;
            }
            if (assetPath.IndexOf(k_IconSourceFolder) < 0)
            {
                if (logError)
                {
                    Debug.Log("Selection is not a valid mip texture, it should be located in: " + k_IconSourceFolder);
                }
                return false;
            }
            if (assetPath.IndexOf(k_IconMipIdentifier) < 0)
            {
                if (logError)
                {
                    Debug.Log("Selection does not have a valid mip identifier " + assetPath + "  " + k_IconMipIdentifier);
                }
                return false;
            }
            return true;
        }

        [CompilerGenerated]
        private sealed class <CreateIconWithMipLevels>c__AnonStorey0
        {
            internal string baseName;
            internal GenerateIconsWithMipLevels.InputData inputData;

            internal bool <>m__0(string o)
            {
                return (o.IndexOf('/' + this.baseName + this.inputData.mipIdentifier) >= 0);
            }
        }

        [CompilerGenerated]
        private sealed class <GetIconAssetPaths>c__AnonStorey1
        {
            internal string mustHaveIdentifier;

            internal bool <>m__0(string o)
            {
                return (o.IndexOf(this.mustHaveIdentifier) < 0);
            }
        }

        private class InputData
        {
            public List<string> generatedFileNames = new List<string>();
            public string mipFileExtension;
            public string mipIdentifier;
            public string sourceFolder;
            public string targetFolder;

            public string GetMipFileName(string baseName, int mipResolution)
            {
                object[] objArray1 = new object[] { this.sourceFolder, baseName, this.mipIdentifier, mipResolution, ".", this.mipFileExtension };
                return string.Concat(objArray1);
            }
        }
    }
}

