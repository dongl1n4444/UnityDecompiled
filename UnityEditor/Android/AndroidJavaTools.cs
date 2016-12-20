namespace UnityEditor.Android
{
    using Microsoft.Win32;
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AndroidJavaTools
    {
        public const int DEFAULT_JVM_MEMORY = 0x800;
        public const int MIN_JVM_MEMORY = 0x40;
        private static string s_JDKLocation;

        internal static string BrowseForJDK(string jdkPath)
        {
            if (string.IsNullOrEmpty(jdkPath))
            {
                jdkPath = GuessJDKLocation();
            }
            if (!InternalEditorUtility.inBatchMode)
            {
                bool flag = false;
                string title = "Select Java Development Kit (JDK) folder";
                do
                {
                    jdkPath = EditorUtility.OpenFolderPanel(title, jdkPath, "");
                    if (jdkPath.Length == 0)
                    {
                        return "";
                    }
                    if (!IsValidJDKHome(jdkPath))
                    {
                        title = "Invalid JDK home selected";
                        string message = "The path you specified does not look like a valid JDK installation.\n";
                        message = (message + "Android development requires at least JDK 7 (1.7), having JRE only is not enough. " + "Please make sure you are selecting a suitable JDK home directory, ") + "or download and install the latest JDK: " + "\nhttp://www.oracle.com/technetwork/java/javase/downloads/index.html";
                        EditorUtility.DisplayDialog(title, message, "OK");
                    }
                }
                while (!flag);
            }
            return jdkPath;
        }

        public static void DumpDiagnostics()
        {
            Console.WriteLine("AndroidJavaTools:");
            Console.WriteLine("\tjava     : {0}", javaPath);
            Console.WriteLine("");
        }

        public static string Exe(string command)
        {
            return (command = command + ((Application.platform != RuntimePlatform.WindowsEditor) ? "" : ".exe"));
        }

        private static string GuessJDKLocation()
        {
            string environmentVariable = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (IsValidJDKHome(environmentVariable))
            {
                return environmentVariable;
            }
            string str3 = Environment.GetEnvironmentVariable("ProgramFiles");
            string str4 = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            string[] strArray = new string[] { "/System/Library/Frameworks/JavaVM.framework/Versions/CurrentJDK/Home/", "/System/Library/Frameworks/JavaVM.framework/Versions/Current/Home/", "/Library/Java/JavaVirtualMachines/*/Contents/Home/", "/System/Library/Java/JavaVirtualMachines/*/Contents/Home/", "/System/Library/Frameworks/JavaVM.framework/Versions/*/Home/", str3 + @"\Java*\jdk*\", str4 + @"\Java*\jdk*\", "/usr/lib/jvm/default-java", "/etc/alternatives/java_sdk", "/usr/lib/jvm/java", "/usr/lib/jvm/*/", "/opt/java*" };
            foreach (string str5 in strArray)
            {
                foreach (string str6 in AndroidFileLocator.Find(str5))
                {
                    if (IsValidJDKHome(str6))
                    {
                        return str6;
                    }
                }
            }
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                string[] strArray4 = new string[] { @"HKEY_LOCAL_MACHINE\SOFTWARE\JavaSoft\Java Development Kit\1.7", @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\JavaSoft\Java Development Kit\1.7", @"HKEY_LOCAL_MACHINE\SOFTWARE\JavaSoft\Java Development Kit\1.8", @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\JavaSoft\Java Development Kit\1.8" };
                foreach (string str7 in strArray4)
                {
                    environmentVariable = Registry.GetValue(str7, "JavaHome", "").ToString();
                    if (IsValidJDKHome(environmentVariable))
                    {
                        return environmentVariable;
                    }
                }
            }
            return "";
        }

        internal static bool IsValidJDKHome(string javaHome)
        {
            if (string.IsNullOrEmpty(javaHome))
            {
                return false;
            }
            string str = Path.Combine(javaHome, "bin");
            string path = Path.Combine(str, Exe("java"));
            string str3 = Path.Combine(str, Exe("javac"));
            return ((File.Exists(path) && VerifyJava(path)) && File.Exists(str3));
        }

        private static string LocateJDKHome()
        {
            if (!string.IsNullOrEmpty(s_JDKLocation))
            {
                return s_JDKLocation;
            }
            s_JDKLocation = EditorPrefs.GetString("JdkPath");
            if (IsValidJDKHome(s_JDKLocation))
            {
                return s_JDKLocation;
            }
            if (string.IsNullOrEmpty(s_JDKLocation))
            {
                s_JDKLocation = GuessJDKLocation();
                if (IsValidJDKHome(s_JDKLocation))
                {
                    EditorPrefs.SetString("JdkPath", s_JDKLocation);
                    return s_JDKLocation;
                }
            }
            s_JDKLocation = BrowseForJDK("");
            if (IsValidJDKHome(s_JDKLocation))
            {
                EditorPrefs.SetString("JdkPath", s_JDKLocation);
                return s_JDKLocation;
            }
            s_JDKLocation = "";
            EditorPrefs.SetString("JdkPath", s_JDKLocation);
            string title = "Unable to find suitable JDK installation.";
            string message = "Please make sure you have a suitable JDK installation.";
            message = (message + " Android development requires at least JDK 7 (1.7), having JRE only is not enough. ") + " The latest JDK can be obtained from the Oracle website " + "\nhttp://www.oracle.com/technetwork/java/javase/downloads/index.html";
            EditorUtility.DisplayDialog(title, message, "OK");
            throw new UnityException(title + " " + message);
        }

        private static bool VerifyJava(string javaExe)
        {
            try
            {
                string[] sdkToolCommand = new string[] { "javaversion" };
                string str = AndroidSDKTools.RunCommandSafe(javaExe, null, sdkToolCommand, 0x800, null, null, "Incompatible java version '" + javaExe + "'");
                char[] separator = new char[] { '.' };
                string[] strArray = str.Split(separator);
                if ((((strArray.Length <= 1) || (Convert.ToUInt32(strArray[0]) != 1)) || (Convert.ToUInt32(strArray[1]) < 7)) && ((strArray.Length <= 0) || (Convert.ToUInt32(strArray[0]) < 7)))
                {
                    throw new Exception(string.Format("Incompatible java version: {0} ('{1}')", str, javaExe));
                }
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(string.Format("Failed java version detection for '{0}'", javaExe));
                Console.WriteLine(exception);
            }
            return false;
        }

        public static string jarPath
        {
            get
            {
                string[] components = new string[] { LocateJDKHome(), "bin", Exe("jar") };
                return Paths.Combine(components);
            }
        }

        public static string javacPath
        {
            get
            {
                string[] components = new string[] { LocateJDKHome(), "bin", Exe("javac") };
                return Paths.Combine(components);
            }
        }

        public static string javaPath
        {
            get
            {
                string[] components = new string[] { LocateJDKHome(), "bin", Exe("java") };
                return Paths.Combine(components);
            }
        }

        public static string JDKDownloadUrl
        {
            get
            {
                return "http://www.oracle.com/technetwork/java/javase/downloads/index.html";
            }
        }
    }
}

