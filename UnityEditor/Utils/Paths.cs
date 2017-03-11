namespace UnityEditor.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;

    internal static class Paths
    {
        public static bool AreEqual(string pathA, string pathB, bool ignoreCase)
        {
            if ((pathA == "") && (pathB == ""))
            {
                return true;
            }
            if (string.IsNullOrEmpty(pathA) || string.IsNullOrEmpty(pathB))
            {
                return false;
            }
            return (string.Compare(Path.GetFullPath(pathA), Path.GetFullPath(pathB), !ignoreCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) == 0);
        }

        private static bool CheckIfAssetPathIsValid(string assetPath, string requiredExtensionWithDot, ref string errorMsg)
        {
            try
            {
                if (string.IsNullOrEmpty(assetPath))
                {
                    if (errorMsg != null)
                    {
                        SetFullErrorMessage("Asset path is empty", assetPath, ref errorMsg);
                    }
                    return false;
                }
                string fileName = Path.GetFileName(assetPath);
                if (fileName.StartsWith("."))
                {
                    if (errorMsg != null)
                    {
                        SetFullErrorMessage("Do not prefix asset name with '.'", assetPath, ref errorMsg);
                    }
                    return false;
                }
                if (fileName.StartsWith(" "))
                {
                    if (errorMsg != null)
                    {
                        SetFullErrorMessage("Do not prefix asset name with white space", assetPath, ref errorMsg);
                    }
                    return false;
                }
                if (!string.IsNullOrEmpty(requiredExtensionWithDot) && !string.Equals(Path.GetExtension(assetPath), requiredExtensionWithDot, StringComparison.OrdinalIgnoreCase))
                {
                    if (errorMsg != null)
                    {
                        SetFullErrorMessage($"Incorrect extension. Required extension is: '{requiredExtensionWithDot}'", assetPath, ref errorMsg);
                    }
                    return false;
                }
            }
            catch (Exception exception)
            {
                if (errorMsg != null)
                {
                    SetFullErrorMessage(exception.Message, assetPath, ref errorMsg);
                }
                return false;
            }
            return true;
        }

        public static string Combine(params string[] components)
        {
            if (components.Length < 1)
            {
                throw new ArgumentException("At least one component must be provided!");
            }
            string str = components[0];
            for (int i = 1; i < components.Length; i++)
            {
                str = Path.Combine(str, components[i]);
            }
            return str;
        }

        public static string CreateTempDirectory()
        {
            string tempFileName = Path.GetTempFileName();
            File.Delete(tempFileName);
            Directory.CreateDirectory(tempFileName);
            return tempFileName;
        }

        public static string GetFileOrFolderName(string path)
        {
            if (File.Exists(path))
            {
                return Path.GetFileName(path);
            }
            if (!Directory.Exists(path))
            {
                throw new ArgumentException("Target '" + path + "' does not exist.");
            }
            string[] strArray = Split(path);
            return strArray[strArray.Length - 1];
        }

        public static bool IsValidAssetPath(string assetPath) => 
            IsValidAssetPath(assetPath, null);

        public static bool IsValidAssetPath(string assetPath, string requiredExtensionWithDot)
        {
            string errorMsg = null;
            return CheckIfAssetPathIsValid(assetPath, requiredExtensionWithDot, ref errorMsg);
        }

        public static bool IsValidAssetPath(string assetPath, string requiredExtensionWithDot, out string errorMsg)
        {
            errorMsg = string.Empty;
            return CheckIfAssetPathIsValid(assetPath, requiredExtensionWithDot, ref errorMsg);
        }

        public static bool IsValidAssetPathWithErrorLogging(string assetPath, string requiredExtensionWithDot)
        {
            string str;
            if (!IsValidAssetPath(assetPath, requiredExtensionWithDot, out str))
            {
                Debug.LogError(str);
                return false;
            }
            return true;
        }

        public static string NormalizePath(this string path)
        {
            if (Path.DirectorySeparatorChar == '\\')
            {
                return path.Replace('/', Path.DirectorySeparatorChar);
            }
            return path.Replace('\\', Path.DirectorySeparatorChar);
        }

        private static void SetFullErrorMessage(string error, string assetPath, ref string errorMsg)
        {
            errorMsg = $"Asset path error: '{ToLiteral(assetPath)}' is not valid: {error}";
        }

        public static string[] Split(string path)
        {
            char[] separator = new char[] { Path.DirectorySeparatorChar };
            List<string> list = new List<string>(path.Split(separator));
            int index = 0;
            while (index < list.Count)
            {
                list[index] = list[index].Trim();
                if (list[index].Equals(""))
                {
                    list.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
            return list.ToArray();
        }

        private static string ToLiteral(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder(input.Length + 2);
            foreach (char ch in input)
            {
                switch (ch)
                {
                    case '\a':
                        builder.Append(@"\a");
                        break;

                    case '\b':
                        builder.Append(@"\b");
                        break;

                    case '\t':
                        builder.Append(@"\t");
                        break;

                    case '\n':
                        builder.Append(@"\n");
                        break;

                    case '\v':
                        builder.Append(@"\v");
                        break;

                    case '\f':
                        builder.Append(@"\f");
                        break;

                    case '\r':
                        builder.Append(@"\r");
                        break;

                    case '\0':
                        builder.Append(@"\0");
                        break;

                    case '"':
                        builder.Append("\\\"");
                        break;

                    case '\'':
                        builder.Append(@"\'");
                        break;

                    case '\\':
                        builder.Append(@"\\");
                        break;

                    default:
                        if ((ch >= ' ') && (ch <= '~'))
                        {
                            builder.Append(ch);
                        }
                        else
                        {
                            builder.Append(@"\u");
                            builder.Append(((int) ch).ToString("x4"));
                        }
                        break;
                }
            }
            return builder.ToString();
        }

        public static string UnifyDirectorySeparator(string path) => 
            path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
    }
}

