namespace UnityEditor.Tizen
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Networking;

    internal class PostProcessTizenPlayer
    {
        internal const string consoleMsg = " See the Console for details.";
        private static ProgressHelper progress = new ProgressHelper();

        private static void CreateManifest(string manifestPath, string companyName, string productName, string normalizedProductName, string id, AssemblyReferenceChecker checker)
        {
            string validVersionString = GetValidVersionString();
            TextWriter writer = new StreamWriter(manifestPath);
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?>");
            writer.WriteLine("<manifest xmlns=\"http://tizen.org/ns/packages\" api-version=\"" + TizenUtilities.StringFromMinOSVersion() + "\" package=\"" + id + "\" version=\"" + validVersionString + "\">");
            writer.WriteLine("    <author href=\"" + PlayerSettings.Tizen.productURL + "\">" + companyName + "</author>");
            writer.WriteLine("   <profile name=\"mobile\" />");
            writer.WriteLine("\t<description>" + SecurityElement.Escape(PlayerSettings.Tizen.productDescription) + "</description>");
            writer.WriteLine("\t<ui-application appid=\"" + id + "\" exec=\"" + normalizedProductName + "\" type=\"capp\" multiple=\"false\" taskmanage=\"true\" nodisplay=\"false\">");
            writer.WriteLine("\t\t<icon>app_icon.png</icon>");
            writer.WriteLine("\t\t<label>" + productName + "</label>");
            writer.WriteLine("\t</ui-application>");
            writer.WriteLine("\t<feature name=\"http://tizen.org/feature/screen.size.all\">true</feature>");
            writer.WriteLine("\t<privileges>");
            if (checker.HasReferenceToMethod("UnityEngine.Handheld::Vibrate") || checker.HasReferenceToMethod("UnityEngine.iPhoneUtils::Vibrate"))
            {
                writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/haptic</privilege>");
            }
            if (checker.HasReferenceToType("UnityEngine.WebCamTexture"))
            {
                writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/camera</privilege>");
            }
            if (checker.HasReferenceToMethod("UnityEngine.Application::OpenURL") || checker.HasReferenceToType("UnityEngine.Purchasing"))
            {
                writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/appmanager.launch</privilege>");
            }
            if (checker.HasReferenceToMethod("UnityEngine.Screen::set_sleepTimeout"))
            {
                writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/display</privilege>");
            }
            if (((checker.HasReferenceToType("UnityEngine.Networking") || checker.HasReferenceToType("System.Net.Sockets")) || (checker.HasReferenceToType("UnityEngine.Network") || checker.HasReferenceToType("UnityEngine.RPC"))) || (((checker.HasReferenceToType("UnityEngine.WWW") || checker.HasReferenceToType("UnityEngine.Video")) || (checker.HasReferenceToType(typeof(Ping).FullName) || checker.HasReferenceToType(typeof(UnityWebRequest).FullName))) || (EditorUserBuildSettings.allowDebugging || EditorUserBuildSettings.connectProfiler)))
            {
                writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/internet</privilege>");
            }
            if (checker.HasReferenceToType("UnityEngine.Video"))
            {
                writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/mediastorage</privilege>");
            }
            if (checker.HasReferenceToType("UnityEngine.Video"))
            {
                writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/externalstorage</privilege>");
            }
            if (TizenUtilities.ThisIsAUnityTestProject())
            {
                writer.WriteLine("\t\t<privilege>http://tizen.org/privilege/internet</privilege>");
            }
            TizenUtilities.WriteCapabilitiesToManifest(writer);
            writer.WriteLine("\t</privileges>");
            writer.WriteLine("</manifest>");
            writer.Close();
            string[] source = File.ReadAllLines(manifestPath);
            File.WriteAllLines(manifestPath, source.Distinct<string>().ToArray<string>());
        }

        private static void CreateProject(string stagingArea)
        {
            Directory.CreateDirectory(Path.Combine(stagingArea, "build"));
            string str = TizenUtilities.StringFromMinOSVersion();
            TextWriter writer = new StreamWriter(Path.Combine(stagingArea, ".cproject"));
            writer.WriteLine("<cproject>");
            writer.WriteLine("<configuration name=\"build\">");
            writer.WriteLine("<app>");
            writer.WriteLine("<option superClass=\"sbi.gnu.cpp.compiler.option\">");
            writer.WriteLine("<listOptionValue value=\"mobile-" + str + "-device.core_gcc46.armel.core.app\"/>");
            writer.WriteLine("</option>");
            writer.WriteLine("</app>");
            writer.WriteLine("</configuration>");
            writer.WriteLine("</cproject>");
            writer.Close();
            TextWriter writer2 = new StreamWriter(Path.Combine(stagingArea, ".project"));
            writer2.WriteLine("<projectDescription>");
            writer2.WriteLine("<name>StagingArea</name>");
            writer2.WriteLine("</projectDescription>");
            writer2.Close();
            TextWriter writer3 = new StreamWriter(Path.Combine(stagingArea, "project_def.prop"));
            writer3.WriteLine("APPNAME = $(appName)");
            writer3.WriteLine("type = app");
            writer3.WriteLine("profile = $(platform)");
            writer3.WriteLine("USER_SRCS = ");
            writer3.WriteLine("USER_DEFS = ");
            writer3.WriteLine("USER_INC_DIRS = ");
            writer3.WriteLine("USER_OBJS = ");
            writer3.WriteLine("USER_LIBS = ");
            writer3.WriteLine("USER_EDCS = ");
            writer3.Close();
            TextWriter writer4 = new StreamWriter(Path.Combine(stagingArea, Path.Combine("build", "build.info")));
            writer4.WriteLine("toolchain=mobile-" + str + "-device.core");
            writer4.WriteLine("architecture=arm");
            writer4.WriteLine("profile-version=" + str);
            writer4.WriteLine("type=mobile");
            writer4.Close();
        }

        private static string GetValidVersionString()
        {
            string bundleVersion = PlayerSettings.bundleVersion;
            if (bundleVersion.Length > 10)
            {
                TizenUtilities.ShowErrDlgAndThrow("Build failure!", "Version must be 10 characters or less (periods included).");
            }
            char[] separator = new char[] { '.' };
            string[] strArray = bundleVersion.Split(separator);
            while (strArray.Length <= 2)
            {
                bundleVersion = bundleVersion + ".0";
                char[] chArray2 = new char[] { '.' };
                strArray = bundleVersion.Split(chArray2);
            }
            if (strArray.Length > 3)
            {
                TizenUtilities.ShowErrDlgAndThrow("Build failure!", "Version must contain only 3 integer components separated by periods.");
            }
            char[] chArray3 = new char[] { '.' };
            strArray = bundleVersion.Split(chArray3);
            if ((!IsNumber(strArray[0]) || (Convert.ToInt16(strArray[0]) < 0)) || (Convert.ToInt16(strArray[0]) > 0xff))
            {
                TizenUtilities.ShowErrDlgAndThrow("Build failure!", "First version component must be an integer greater than zero and less than 256.");
            }
            if ((!IsNumber(strArray[1]) || (Convert.ToInt16(strArray[1]) < 0)) || (Convert.ToInt16(strArray[1]) > 0xff))
            {
                TizenUtilities.ShowErrDlgAndThrow("Build failure!", "Second version component must be an integer greater than zero and less than 256.");
            }
            if ((!IsNumber(strArray[2]) || (Convert.ToInt16(strArray[2]) < 0)) || (Convert.ToInt32(strArray[2]) > 0xffff))
            {
                TizenUtilities.ShowErrDlgAndThrow("Build failure!", "Third version component must be an integer greater than zero and less than 1296.");
            }
            return bundleVersion;
        }

        public static bool IsNumber(string lcWord)
        {
            int num;
            return int.TryParse(lcWord, out num);
        }

        private static bool IsValidBundleIdentifier(string domainish, string extraValidChars)
        {
            if (string.IsNullOrEmpty(domainish))
            {
                return false;
            }
            if ((domainish == "com.Company.ProductName") || (domainish == "com.unity3d.player"))
            {
                return false;
            }
            string str = "a-zA-Z0-9" + extraValidChars;
            Regex regex = new Regex(string.Format("^([{0}]+[.])*[{0}]+[.][{0}]+$", str));
            return regex.IsMatch(domainish);
        }

        private static bool IsValidTizenBundleIdentifier(string bundleIdentifier)
        {
            string extraValidChars = "_";
            bool flag = false;
            char[] separator = new char[] { '.' };
            foreach (string str2 in bundleIdentifier.Split(separator))
            {
                if (((str2.Length < 1) || char.IsDigit(str2[0])) || (extraValidChars.IndexOf(str2[0]) != -1))
                {
                    flag = true;
                }
            }
            char[] chArray2 = new char[] { '.' };
            if (bundleIdentifier.Split(chArray2).Length < 3)
            {
                flag = true;
            }
            return (!flag && IsValidBundleIdentifier(bundleIdentifier, extraValidChars));
        }

        private static void PackageTargets(int target, string userInstallPath, string playerPackage, string stagingArea, bool developmentPlayer, string normalizedProductName)
        {
            string[] strArray = new string[0];
            if (target == 2)
            {
                strArray = new string[] { "Emulator", "Device" };
            }
            else if (target == 1)
            {
                strArray = new string[] { "Emulator" };
            }
            else if (target == 0)
            {
                strArray = new string[] { "Device" };
            }
            string str = Path.Combine(stagingArea, "build");
            string str4 = Path.Combine(Path.Combine(Path.Combine(stagingArea, "shared"), "res"), "data");
            string str5 = !developmentPlayer ? "/TizenPlayer" : "/TizenDevelopmentPlayer";
            foreach (string str6 in strArray)
            {
                FileUtil.CopyFileIfExists(playerPackage + "/" + str6 + str5 + "/TizenPlayer", str + "/" + normalizedProductName, true);
                FileUtil.MoveFileIfExists(str4 + "/Managed/mscorlib.dll", str4 + "/Managed/mono/2.0/mscorlib.dll");
                progress.Step("Signing & Packaging", EditorGUIUtility.TextContent("Signing application with Tizen certificate \"" + PlayerSettings.Tizen.signingProfileName + "\"...").text);
                if (!TizenUtilities.CreateTpkPackage(stagingArea))
                {
                    TizenUtilities.ShowErrDlgAndThrow("Build Failure!", "Failed to sign and package the application. Check the editor log for more details.");
                }
                if (Path.GetExtension(userInstallPath) == "")
                {
                    userInstallPath = userInstallPath + ".tpk";
                }
                string fullPath = Path.GetFullPath(stagingArea + "/build/" + PlayerSettings.bundleIdentifier + "-" + GetValidVersionString() + "-arm.tpk");
                string str8 = FileUtil.UnityGetFileName(userInstallPath);
                string str9 = "";
                if (target == 2)
                {
                    str9 = Path.Combine(FileUtil.UnityGetDirectoryName(userInstallPath), str6);
                }
                else
                {
                    str9 = FileUtil.UnityGetDirectoryName(userInstallPath);
                }
                Console.WriteLine("Deleting " + Path.Combine(str9, str8));
                FileUtil.DeleteFileOrDirectory(Path.Combine(str9, str8));
                Directory.CreateDirectory(str9);
                FileUtil.MoveFileOrDirectory(fullPath, Path.Combine(str9, str8));
                Console.WriteLine("Moving " + fullPath + " to " + Path.Combine(str9, str8));
            }
        }

        internal static void PostProcess(BuildTarget target, string stagingAreaDataUpperCase, string stagingArea, string stagingAreaDataManaged, string playerPackage, string installPath, string companyName, string productName, BuildOptions options)
        {
            bool flag = (options & BuildOptions.AutoRunPlayer) != BuildOptions.CompressTextures;
            bool developmentPlayer = (options & BuildOptions.Development) != BuildOptions.CompressTextures;
            float numSteps = !flag ? ((float) 2) : ((float) 5);
            progress.Reset(numSteps);
            if (PlayerSettings.Tizen.deploymentTargetType == 2)
            {
                flag = false;
            }
            Regex regex = new Regex("[^a-zA-Z_-]");
            string normalizedProductName = regex.Replace(productName, "").ToLower();
            string str2 = Path.Combine(playerPackage, "assets");
            if (!IsValidTizenBundleIdentifier(PlayerSettings.bundleIdentifier))
            {
                string message = "Please set the Bundle Identifier in the Player Settings.";
                message = (message + " The value must follow the convention 'com.YourCompanyName.YourProductName'") + " and can contain alphanumeric characters and underscore." + "\nEach segment must not start with a numeric character or underscore.";
                Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
                TizenUtilities.ShowErrDlgAndThrow("Bundle Identifier has not been set up correctly", message);
            }
            if (TizenUtilities.ThisIsAUnityTestProject())
            {
                PlayerSettings.Tizen.deploymentTargetType = 0;
            }
            if (PlayerSettings.Tizen.deploymentTargetType < 0)
            {
                string str4 = "Please select a Tizen deployment target.";
                str4 = str4 + " Press the Discover button in the Publishing section of Player Settings." + " Then select a type of target to get a list of available targets to select.";
                Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
                TizenUtilities.ShowErrDlgAndThrow("Tizen Deployment Target has not been selected.", str4);
            }
            TizenUtilities.PrepareToolPaths();
            TizenUtilities.ValidateSigningProfile(stagingArea);
            if (flag)
            {
                TizenUtilities.AssertAnyDeviceReady(new Command.WaitingForProcessToExit(new Progress("Tizen Device Detection", "Detecting attached devices", progress.Get()).Show));
            }
            Directory.CreateDirectory(Path.Combine(stagingArea, "lib"));
            Directory.CreateDirectory(Path.Combine(stagingArea, "res"));
            Directory.CreateDirectory(Path.Combine(stagingArea, "setting"));
            string path = Path.Combine(stagingArea, "shared");
            Directory.CreateDirectory(path);
            string str9 = Path.Combine(path, "res");
            Directory.CreateDirectory(str9);
            string destDirName = Path.Combine(str9, "data");
            string dst = Path.Combine(str9, "app_icon.png");
            FileUtil.MoveFileIfExists(Path.Combine(stagingArea, "app_icon.png"), dst);
            if (!PlayerSettings.SplashScreen.show)
            {
                string str12 = Path.Combine(str9, "app_splash.png");
                FileUtil.MoveFileIfExists(Path.Combine(stagingArea, "app_splash.png"), str12);
                if (!File.Exists(str12))
                {
                    FileUtil.CopyFileOrDirectory(playerPackage + "/assets/splash.png", str12);
                }
            }
            Directory.CreateDirectory(Path.Combine(path, "trusted"));
            Directory.Move(stagingAreaDataUpperCase, destDirName + "2");
            Directory.Move(destDirName + "2", destDirName);
            if (!File.Exists(dst))
            {
                FileUtil.CopyFileOrDirectory(str2 + "/icon.png", dst);
            }
            Directory.CreateDirectory(Path.Combine(destDirName, "Managed/mono/2.0"));
            FileUtil.CopyFileOrDirectory(playerPackage + "/Data/Resources/unity default resources", destDirName + "/unity default resources");
            Directory.CreateDirectory(Path.Combine(stagingArea, "data"));
            string str15 = Path.Combine(stagingArea, "lib");
            Directory.CreateDirectory(str15);
            foreach (PluginImporter importer in PluginImporter.GetImporters(target))
            {
                string fileName = Path.GetFileName(importer.assetPath);
                FileUtil.UnityFileCopy(importer.assetPath, Path.Combine(str15, fileName));
            }
            if (Directory.Exists("Assets/StreamingAssets"))
            {
                FileUtil.CopyDirectoryRecursive("Assets/StreamingAssets", Path.Combine(destDirName, "Raw"), true, true);
            }
            AssemblyReferenceChecker checker = new AssemblyReferenceChecker();
            bool collectMethods = true;
            bool ignoreSystemDlls = true;
            string[] components = new string[] { destDirName, "Managed" };
            string str17 = Paths.Combine(components);
            checker.CollectReferences(str17, collectMethods, 0f, ignoreSystemDlls);
            string bundleIdentifier = PlayerSettings.bundleIdentifier;
            CreateManifest(Path.Combine(stagingArea, "tizen-manifest.xml"), companyName, productName, normalizedProductName, bundleIdentifier, checker);
            CreateProject(stagingArea);
            PackageTargets(PlayerSettings.Tizen.deploymentTargetType, installPath, playerPackage, stagingArea, developmentPlayer, normalizedProductName);
            if (flag)
            {
                progress.Step("Installing", EditorGUIUtility.TextContent("Installing application on " + TizenUtilities.SelectedDeploymentTarget() + "...").text);
                if (TizenUtilities.InstallTpkPackage(installPath))
                {
                    if (developmentPlayer)
                    {
                        progress.Step("Port Forwarding", EditorGUIUtility.TextContent("Setting up profiler tunnel...").text);
                        TizenUtilities.ForwardPort(ProfilerDriver.directConnectionPort, "55000");
                    }
                    progress.Step("Launching", EditorGUIUtility.TextContent("Launching application on " + TizenUtilities.SelectedDeploymentTarget() + "...").text);
                    TizenUtilities.LaunchTpkPackage(bundleIdentifier, stagingArea);
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

        private static bool ValidateCommandResult(string output) => 
            ((!output.Contains("error: device not found") && !output.Contains("error: device offline")) && !output.Contains("failed"));

        private class Progress
        {
            private string m_Message;
            private string m_Title;
            private float m_Value;

            public Progress(string title, string message, float val)
            {
                this.m_Title = title;
                this.m_Message = message;
                this.m_Value = val;
            }

            public void Show(Program program)
            {
                if (EditorUtility.DisplayCancelableProgressBar(this.m_Title, this.m_Message, this.m_Value))
                {
                    throw new ProcessAbortedException(this.m_Title);
                }
            }
        }

        internal class ProgressHelper
        {
            internal float currentBuildStep = 0f;
            internal float numBuildSteps = 0f;

            public float Advance() => 
                (++this.currentBuildStep / this.numBuildSteps);

            public float Get() => 
                (this.currentBuildStep / this.numBuildSteps);

            public float LastValue() => 
                ((this.currentBuildStep - 1f) / this.numBuildSteps);

            public void Reset(float numSteps)
            {
                this.currentBuildStep = 0f;
                this.numBuildSteps = numSteps;
            }

            public void Show(string title, string message)
            {
                if (EditorUtility.DisplayCancelableProgressBar(title, message, this.Get()))
                {
                    throw new ProcessAbortedException(title);
                }
            }

            public void Step(string title, string message)
            {
                this.Advance();
                this.Show(title, message);
            }
        }
    }
}

