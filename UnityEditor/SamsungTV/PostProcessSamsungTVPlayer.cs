namespace UnityEditor.SamsungTV
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Xml;
    using UnityEditor;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class PostProcessSamsungTVPlayer
    {
        private const int requiredUnityLauncherVersionMajor = 1;
        private const int requiredUnityLauncherVersionMinor = 5;

        private static void AddDirectoryToZip(string zipName, string file, string execDir, string[] progressMessages)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                ExecuteSystemProcess(Path.Combine(Path.Combine(EditorApplication.applicationContentsPath, "Tools"), "7z.exe"), "a -r -tzip -y -bd -x!" + zipName + " " + zipName + " " + file, execDir, progressMessages, 0f);
            }
            else
            {
                ExecuteSystemProcess("/usr/bin/zip", "-r " + zipName + " " + file + " -x " + zipName, execDir, progressMessages, 0f);
            }
        }

        private static bool AllFilesAreSigned(string srcDir)
        {
            bool flag = true;
            if (!Directory.Exists(srcDir))
            {
                UnityEngine.Debug.LogError("Directory does not exist!: " + srcDir);
                return false;
            }
            string[] files = Directory.GetFiles(srcDir);
            foreach (string str in files)
            {
                if ((!str.EndsWith(".sig") && !str.EndsWith(".signature")) && !File.Exists(str + ".sig"))
                {
                    UnityEngine.Debug.LogError("File: " + str + " has not been signed and will not work with the latest Unity Launcher");
                    flag = false;
                }
            }
            return flag;
        }

        private static string CallLauncher(string ip, string command, string file, string[] progress_strings, float progress_value)
        {
            <CallLauncher>c__AnonStorey0 storey = new <CallLauncher>c__AnonStorey0();
            bool flag = !string.IsNullOrEmpty(file);
            storey.retVal = "";
            storey.failure = false;
            storey.http = new AsyncHTTPClient(ip + ":8899/" + command, !flag ? "GET" : "POST");
            storey.http.doneCallback = new AsyncHTTPClient.DoneCallback(storey.<>m__0);
            storey.upload_progress = 0f;
            storey.upload_done = 0;
            storey.upload_total = 1;
            if (flag)
            {
                storey.http.statusCallback = new AsyncHTTPClient.StatusCallback(storey.<>m__1);
                storey.http.postData = file;
                storey.http.header["Content-Type"] = "multipart/form-data";
            }
            storey.http.Begin();
            do
            {
                if (EditorUtility.DisplayCancelableProgressBar(progress_strings[0], progress_strings[1] + ((!flag || (storey.upload_done == 0)) ? "" : $" [{((((float) storey.upload_done) / 1024f) / 1024f):F1} MB / {((((float) storey.upload_total) / 1024f) / 1024f):F1} MB] {storey.upload_progress:P0}"), !flag ? progress_value : storey.upload_progress))
                {
                    storey.http.Abort();
                    throw new Exception(progress_strings[0] + " aborted");
                }
                AsyncHTTPClient.CurlRequestCheck();
                if (storey.failure)
                {
                    throw new Exception(progress_strings[0] + ": " + storey.retVal);
                }
                Thread.Sleep(10);
            }
            while (!storey.http.IsDone());
            return storey.retVal;
        }

        private static void CopyNativePluginsToStaging(string destinationDir, string targetPlatform)
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }
            foreach (PluginImporter importer in PluginImporter.GetImporters(BuildTarget.SamsungTV))
            {
                if (importer.GetPlatformData(BuildTarget.SamsungTV, STVPlugin.modelTag).Equals(targetPlatform))
                {
                    string fileName = Path.GetFileName(importer.assetPath);
                    FileUtil.UnityFileCopy(importer.assetPath, Path.Combine(destinationDir, fileName));
                    string path = importer.assetPath + ".signature";
                    if (File.Exists(path))
                    {
                        FileUtil.UnityFileCopy(path, Path.Combine(destinationDir, Path.GetFileName(path)));
                    }
                }
            }
        }

        private static bool CreateManifest(string manifestPath, string targetPlatform)
        {
            if ((!IsValidEmail(PlayerSettings.SamsungTV.productAuthorEmail) || !IsValidLink(PlayerSettings.SamsungTV.productLink)) || !IsValidBundleVersion(PlayerSettings.bundleVersion))
            {
                return false;
            }
            TextWriter writer = new StreamWriter(manifestPath);
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            writer.WriteLine("<widget>");
            writer.WriteLine("\t<category>" + PlayerSettings.SamsungTV.productCategory + "</category>");
            writer.WriteLine("\t<apptype>14</apptype>");
            writer.WriteLine("\t<contents>libmain.so</contents>");
            writer.WriteLine("\t<ver>" + PlayerSettings.bundleVersion + "</ver>");
            writer.WriteLine("\t<widgetname>" + PlayerSettings.productName + "</widgetname>");
            writer.WriteLine("\t<description>" + PlayerSettings.SamsungTV.productDescription + "</description>");
            writer.WriteLine("\t<width>" + PlayerSettings.defaultScreenWidth + "</width>");
            writer.WriteLine("\t<height>" + PlayerSettings.defaultScreenHeight + "</height>");
            writer.WriteLine("\t<unityexport>" + targetPlatform + "</unityexport>");
            writer.WriteLine("\t<author>");
            writer.WriteLine("\t\t<name>" + PlayerSettings.SamsungTV.productAuthor + "</name>");
            writer.WriteLine("\t\t<email>" + PlayerSettings.SamsungTV.productAuthorEmail + "</email>");
            writer.WriteLine("\t\t<link>" + PlayerSettings.SamsungTV.productLink + "</link>");
            writer.WriteLine("\t\t<organization>" + PlayerSettings.companyName + "</organization>");
            writer.WriteLine("\t</author>");
            if ((targetPlatform == "STANDARD_15") || (targetPlatform == "STANDARD_16"))
            {
                writer.WriteLine("\t<multitasking>N</multitasking>");
                writer.WriteLine("\t<MLS2>Y</MLS2>");
            }
            else
            {
                writer.WriteLine("\t<multiapp>n</multiapp>");
                writer.WriteLine("\t<multiscreen>n</multiscreen>");
            }
            writer.WriteLine("</widget>");
            writer.Close();
            return true;
        }

        private static string ExecuteSystemProcess(string command, string args, string workingdir, string[] progress_strings, float progress_value)
        {
            ProcessStartInfo si = new ProcessStartInfo {
                FileName = command,
                Arguments = args,
                WorkingDirectory = workingdir,
                CreateNoWindow = true
            };
            Program program = new Program(si);
            program.Start();
            do
            {
                if (EditorUtility.DisplayCancelableProgressBar(progress_strings[0], progress_strings[1], progress_value))
                {
                    program.Dispose();
                    throw new Exception(progress_strings[0] + " aborted");
                }
            }
            while (!program.WaitForExit(100));
            string str = StringConcat(program.GetStandardOutput()) + StringConcat(program.GetErrorOutput());
            program.Dispose();
            return str;
        }

        private static Dictionary<string, string> GetTargetInfo(string ip)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string[] strArray = new string[] { "[Samsung TV] Retrieving Target System Info", "Retrieving target system information." };
            string str = CallLauncher(ip, "get_info", null, strArray, 0f);
            if (!string.IsNullOrEmpty(str))
            {
                string[] separator = new string[] { "\n" };
                string[] strArray2 = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str2 in strArray2)
                {
                    string[] textArray3 = new string[] { ": " };
                    string[] strArray4 = str2.Split(textArray3, StringSplitOptions.RemoveEmptyEntries);
                    if (strArray4.Length == 2)
                    {
                        dictionary.Add(strArray4[0].Trim(), strArray4[1].Trim());
                    }
                }
            }
            return dictionary;
        }

        private static bool IsValidBundleVersion(string strIn)
        {
            bool flag = false;
            if (!string.IsNullOrEmpty(strIn))
            {
                try
                {
                    flag = strIn.Equals(XmlConvert.VerifyNMTOKEN(strIn));
                }
                catch (Exception)
                {
                    throw new Exception("The format of 'Bundle Version' is not suitable. (Please check PlayerSettings > Other Settings > Bundle Version : " + strIn + ")");
                }
                return flag;
            }
            return true;
        }

        private static bool IsValidEmail(string strIn)
        {
            bool flag = false;
            if (!string.IsNullOrEmpty(strIn))
            {
                flag = Regex.IsMatch(strIn, "^(?(\")(\".+?\"@)|(((\\p{Nd}|\\p{L})((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=(\\p{Nd}|\\p{L}))@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(((\\p{Nd}|\\p{L})[-\\w]*(\\p{Nd}|\\p{L})\\.)+\\p{L}{2,6}))$");
                if (!flag)
                {
                    throw new Exception("The format of 'E-Mail' is not suitable. (Please check PlayerSettings > Publising Settings > E-Mail : " + strIn + ")");
                }
                return flag;
            }
            return true;
        }

        private static bool IsValidLink(string strIn)
        {
            bool flag = false;
            if (!string.IsNullOrEmpty(strIn))
            {
                flag = Uri.IsWellFormedUriString(strIn, UriKind.Absolute);
                if (!flag)
                {
                    throw new Exception("The format of 'Link' is not suitable. (Please check PlayerSettings > Publising Settings > Link : " + strIn + ")");
                }
                return flag;
            }
            return true;
        }

        internal static void PostProcess(BuildTarget target, string stagingAreaData, string stagingArea, string stagingAreaDataManaged, string playerPackage, string installPath, string companyName, string productName, BuildOptions options)
        {
            string str = FileUtil.UnityGetFileName(installPath);
            string path = stagingArea;
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(Path.Combine(stagingAreaData, "Managed/mono/2.0"));
            FileUtil.CopyFileOrDirectory(playerPackage + "/Data/Resources/unity default resources", stagingAreaData + "/Resources/unity default resources");
            if (Directory.Exists("Assets/StreamingAssets"))
            {
                FileUtil.CopyDirectoryRecursive("Assets/StreamingAssets", Path.Combine(stagingAreaData, "Raw"), true, true);
            }
            Directory.CreateDirectory(path + "/resources");
            FileUtil.MoveFileOrDirectory(stagingAreaData, Path.Combine(Path.Combine(path, "resources"), "Data"));
            string str3 = path + "/app_icon.png";
            if (!File.Exists(str3))
            {
                FileUtil.CopyFileOrDirectory(playerPackage + "/assets/icon.png", str3);
            }
            string[] paths = new string[] { path, "resources", "Data", "Managed" };
            string str4 = FileUtil.CombinePaths(paths);
            if (File.Exists(Path.Combine(str4, "UnityEngine.xml")))
            {
                File.Delete(Path.Combine(str4, "UnityEngine.xml"));
            }
            if (AllFilesAreSigned(str4))
            {
                string[] progressMessages = new string[] { "[Samsung TV] Packaging", "Creating Samsung TV application package..." };
                string zipName = "\"" + str + ".zip\"";
                AddDirectoryToZip(zipName, ".", stagingArea, progressMessages);
                Directory.CreateDirectory(installPath);
                char[] separator = new char[] { '/' };
                string[] strArray2 = PlayerSettings.SamsungTV.deviceAddress.Split(separator);
                Dictionary<string, string> targetInfo = null;
                string str6 = "";
                string str7 = "";
                string str8 = "";
                string str9 = "";
                string str10 = ((options & BuildOptions.Development) == BuildOptions.CompressTextures) ? "STVPlayer" : "STVDevelopmentPlayer";
                List<STVPlugin.TVTargets> list = new List<STVPlugin.TVTargets>((STVPlugin.TVTargets[]) Enum.GetValues(typeof(STVPlugin.TVTargets)));
                if ((options & BuildOptions.AutoRunPlayer) != BuildOptions.CompressTextures)
                {
                    list.Clear();
                    foreach (string str11 in strArray2)
                    {
                        GetTargetInfo(str11).TryGetValue("Target Device", out str6);
                        STVPlugin.TVTargets item = (STVPlugin.TVTargets) Enum.Parse(typeof(STVPlugin.TVTargets), str6);
                        if (!list.Contains(item))
                        {
                            list.Add(item);
                        }
                    }
                }
                foreach (STVPlugin.TVTargets targets2 in list)
                {
                    string str12 = targets2.ToString();
                    str8 = "game_" + str12 + ".so";
                    str9 = "libmain_" + str12 + ".so";
                    string[] textArray3 = new string[] { playerPackage, str10, str8 };
                    if (File.Exists(FileUtil.CombinePaths(textArray3)))
                    {
                        str7 = Path.Combine(stagingArea, str12);
                        FileUtil.DeleteFileOrDirectory(str7);
                        Directory.CreateDirectory(str7);
                        File.Copy(Path.Combine(stagingArea, str + ".zip"), Path.Combine(str7, str + ".zip"));
                        CreateManifest(Path.Combine(str7, "config.xml"), str12);
                        Directory.CreateDirectory(str7 + "/resources/Data");
                        CopyNativePluginsToStaging(str7, str12);
                        string[] textArray4 = new string[] { playerPackage, str10, str8 };
                        File.Copy(FileUtil.CombinePaths(textArray4), Path.Combine(str7, "game.so"));
                        string[] textArray5 = new string[] { playerPackage, str10, str8 + ".sig" };
                        File.Copy(FileUtil.CombinePaths(textArray5), Path.Combine(str7 + "/resources", "game.so.sig"));
                        string[] textArray6 = new string[] { playerPackage, str10, str9 };
                        File.Copy(FileUtil.CombinePaths(textArray6), Path.Combine(str7, "libmain.so"));
                        string[] textArray7 = new string[] { playerPackage, str10, str9 + ".sig" };
                        File.Copy(FileUtil.CombinePaths(textArray7), Path.Combine(str7 + "/resources", "libmain.so.sig"));
                        progressMessages[1] = "Creating " + str12 + " package";
                        AddDirectoryToZip(zipName, ".", str7, progressMessages);
                        string str13 = Path.Combine(installPath, str + "_" + str12 + ".zip");
                        FileUtil.DeleteFileOrDirectory(str13);
                        FileUtil.MoveFileIfExists(Path.Combine(str7, str + ".zip"), str13);
                        FileUtil.DeleteFileOrDirectory(str7);
                    }
                }
                string str14 = Uri.EscapeDataString(PlayerSettings.productName);
                if ((options & BuildOptions.AutoRunPlayer) != BuildOptions.CompressTextures)
                {
                    foreach (string str15 in strArray2)
                    {
                        targetInfo = GetTargetInfo(str15);
                        string str16 = "";
                        targetInfo.TryGetValue("Version", out str16);
                        targetInfo.TryGetValue("Target Device", out str6);
                        char[] chArray2 = new char[] { '.' };
                        string[] strArray5 = str16.Split(chArray2);
                        int result = 0;
                        int num4 = 0;
                        if (((strArray5.Length < 2) || !int.TryParse(strArray5[0], out result)) || ((!int.TryParse(strArray5[1], out num4) || (result < 1)) || ((result == 1) && (num4 < 5))))
                        {
                            UnityEngine.Debug.LogError(string.Concat(new object[] { "Old version of Unity Launcher detected, please upgrade Unity Launcher to at least version ", 1, ".", 5 }));
                            break;
                        }
                        string str17 = Path.Combine(installPath, str + "_" + str6 + ".zip");
                        if (File.Exists(str17))
                        {
                            string str18 = "false";
                            targetInfo.TryGetValue("Game Running", out str18);
                            if (str18 == "true")
                            {
                                if (EditorUtility.DisplayDialog("Already Running Game", "There is already a game running. Would you like to close it?", "Yes!", "Cancel"))
                                {
                                    progressMessages[0] = "[Samsung TV] Stopping Running Game";
                                    progressMessages[1] = "Stopping Running Game";
                                    CallLauncher(str15, "launch?game=", "", progressMessages, 1f);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            progressMessages[0] = "[Samsung TV] Uploading...";
                            progressMessages[1] = "Uploading to SamsungTV";
                            CallLauncher(str15, "upload_archive", str17, progressMessages, 0f);
                            progressMessages[0] = "[Samsung TV] Launching Game";
                            progressMessages[1] = "Launching Game";
                            CallLauncher(str15, "launch?game=" + str14, "", progressMessages, 1f);
                        }
                        else
                        {
                            UnityEngine.Debug.LogError("Unable to find zip to upload for platform target: " + str6 + " currently looking for file: " + str17);
                        }
                    }
                }
            }
        }

        private static string StringConcat(IEnumerable<string> strings)
        {
            StringBuilder builder = new StringBuilder("");
            foreach (string str in strings)
            {
                builder.Append(str + Environment.NewLine);
            }
            return builder.ToString();
        }

        [CompilerGenerated]
        private sealed class <CallLauncher>c__AnonStorey0
        {
            internal bool failure;
            internal AsyncHTTPClient http;
            internal string retVal;
            internal int upload_done;
            internal float upload_progress;
            internal int upload_total;

            internal void <>m__0(AsyncHTTPClient c)
            {
                if (!this.http.IsSuccess())
                {
                    this.failure = true;
                }
                this.retVal = this.http.text;
            }

            internal void <>m__1(AsyncHTTPClient.State status, int bytesDone, int bytesTotal)
            {
                this.upload_progress = (1f * bytesDone) / ((float) bytesTotal);
                this.upload_done = bytesDone;
                this.upload_total = bytesTotal;
            }
        }
    }
}

