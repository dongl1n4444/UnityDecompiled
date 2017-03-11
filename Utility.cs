using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Utils;
using UnityEditorInternal;

internal static class Utility
{
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map1;
    private static string frameworkPath;
    private static int kProcessTerminateFlag = 1;
    private static int kQueryInformationLimitedFlag = 0x1000;
    private static readonly Regex packageNameRegex = new Regex(@"^[A-Za-z0-9\.\-]{2,49}[A-Za-z0-9\-]$", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr objectHandle);
    public static string CombinePath(params string[] paths) => 
        Paths.Combine(paths).ConvertToWindowsPath();

    public static string CombinePath(string path1, string path2) => 
        Path.Combine(path1, path2).ConvertToWindowsPath();

    public static string ConvertToWindowsPath(this string path) => 
        path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

    public static void CopyDirectoryContents(string sourceDirectory, string destinationDirectory, bool recursive = false)
    {
        if (!Directory.Exists(destinationDirectory))
        {
            Directory.CreateDirectory(destinationDirectory);
        }
        foreach (string str in Directory.GetFiles(sourceDirectory))
        {
            File.Copy(str, Path.Combine(destinationDirectory, Path.GetFileName(str)), true);
        }
        if (recursive)
        {
            foreach (string str2 in Directory.GetDirectories(sourceDirectory))
            {
                CopyDirectoryContents(str2, Path.Combine(destinationDirectory, Path.GetFileName(str2)), true);
            }
        }
    }

    public static void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
        while (!Directory.Exists(path))
        {
            Thread.Sleep(0);
        }
    }

    public static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            while (Directory.Exists(path))
            {
                Thread.Sleep(0);
            }
        }
    }

    [DllImport("Psapi.dll")]
    private static extern bool EnumProcesses([In, Out, MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.U4)] uint[] processIds, uint arraySizeBytes, [MarshalAs(UnmanagedType.U4)] out uint bytesCopied);
    internal static Version GetDesiredUWPSDK() => 
        UWPReferences.GetDesiredSDKVersion();

    public static string GetDesiredUWPSDKString()
    {
        Version desiredUWPSDK = GetDesiredUWPSDK();
        string str = desiredUWPSDK.ToString();
        if (desiredUWPSDK.Build == -1)
        {
            str = str + ".0";
        }
        if (desiredUWPSDK.Revision == -1)
        {
            str = str + ".0";
        }
        return str;
    }

    public static string GetFrameworkPath()
    {
        if (frameworkPath == null)
        {
            string[] strArray = new string[] { @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Microsoft\VisualStudio\SxS\VC7", @"HKEY_CURRENT_USER\SOFTWARE\Wow6432Node\Microsoft\Microsoft\VisualStudio\SxS\VC7", @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft\VisualStudio\SxS\VC7", @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Microsoft\VisualStudio\SxS\VC7" };
            foreach (string str in strArray)
            {
                string str2 = RegistryUtil.GetRegistryStringValue32(str, "FrameworkDir32", null);
                if (!string.IsNullOrEmpty(str2))
                {
                    string str3 = RegistryUtil.GetRegistryStringValue32(str, "FrameworkVer32", null);
                    if (!string.IsNullOrEmpty(str3))
                    {
                        frameworkPath = Path.Combine(str2, str3);
                        break;
                    }
                }
            }
            if (frameworkPath == null)
            {
                frameworkPath = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), @"Microsoft.NET\Framework\v4.0.30319");
            }
        }
        return frameworkPath;
    }

    public static string GetMSBuildExePath(WSASDK wsaSDK)
    {
        string str = null;
        switch (wsaSDK)
        {
            case WSASDK.SDK80:
                str = Path.Combine(GetMSBuildPath(wsaSDK), "MSBuild.exe");
                break;

            case WSASDK.SDK81:
            case WSASDK.PhoneSDK81:
            case WSASDK.UniversalSDK81:
            case WSASDK.UWP:
                str = Path.Combine(GetMSBuildPath(wsaSDK), @"Bin\MSBuild.exe");
                break;

            default:
                throw new Exception("Unsupported Windows Store Apps SDK: " + wsaSDK);
        }
        if (string.IsNullOrEmpty(str) || !File.Exists(str))
        {
            throw new Exception($"Failed to locate suitable MSBuild '{str}'");
        }
        return str;
    }

    public static string GetMSBuildPath(string version)
    {
        string environmentVariable = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
        if (string.IsNullOrEmpty(environmentVariable))
        {
            environmentVariable = Environment.GetEnvironmentVariable("ProgramFiles");
            if (string.IsNullOrEmpty(environmentVariable))
            {
                environmentVariable = @"C:\Program Files (x86)";
            }
        }
        return Path.Combine(environmentVariable, $"MSBuild\{version}");
    }

    public static string GetMSBuildPath(WSASDK wsaSDK)
    {
        switch (wsaSDK)
        {
            case WSASDK.SDK80:
                return GetFrameworkPath();

            case WSASDK.SDK81:
            case WSASDK.PhoneSDK81:
            case WSASDK.UniversalSDK81:
            case WSASDK.UWP:
                return GetMSBuildPath(GetVSVersion());
        }
        throw new Exception("Unsupported Windows Store Apps SDK: " + wsaSDK);
    }

    public static string GetPackageName(bool wsa)
    {
        string str;
        if (wsa)
        {
            str = TryValidatePackageName(PlayerSettings.WSA.packageName);
            if (str != null)
            {
                return str;
            }
        }
        str = TryValidatePackageName(PlayerSettings.productName);
        if (str != null)
        {
            return str;
        }
        return "DefaultPackageName";
    }

    public static string GetVsName()
    {
        char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
        string str = PlayerSettings.productName.Normalize(NormalizationForm.FormD);
        StringBuilder builder = new StringBuilder();
        foreach (char ch in str)
        {
            if (!invalidFileNameChars.Contains<char>(ch) && (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark))
            {
                builder.Append(ch);
            }
        }
        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string GetVsNamespace()
    {
        string str2;
        char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
        StringBuilder builder = new StringBuilder();
        string vsName = GetVsName();
        for (int i = 0; i < vsName.Length; i++)
        {
            char ch = vsName[i];
            if (invalidFileNameChars.Contains<char>(ch))
            {
                continue;
            }
            switch (ch)
            {
                case '\'':
                case '(':
                case ')':
                case '-':
                case ' ':
                    break;

                case '.':
                {
                    if (!UseIl2CppScriptingBackend() && ((i + 1) != vsName.Length))
                    {
                        goto Label_00A3;
                    }
                    builder.Append('_');
                    continue;
                }
                default:
                    goto Label_00D8;
            }
            builder.Append('_');
            continue;
        Label_00A3:
            if (char.IsNumber(vsName[i + 1]))
            {
                builder.Append("._");
            }
            else
            {
                builder.Append(".");
            }
            continue;
        Label_00D8:
            builder.Append(ch);
        }
        if ((builder.Length > 0) && char.IsNumber(builder[0]))
        {
            str2 = "_" + builder;
        }
        else
        {
            str2 = builder.ToString();
        }
        if ((str2.Length == 20) && UseIl2CppScriptingBackend())
        {
            str2 = str2 + "_";
        }
        return str2;
    }

    public static string GetVSVersion()
    {
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("VS140COMNTOOLS")))
        {
            return "14.0";
        }
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("VS120COMNTOOLS")))
        {
            return "12.0";
        }
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("VS110COMNTOOLS")))
        {
            throw new Exception("Failed to locate env variables VS140COMNTOOLS, VS120COMNTOOLS or VS110COMNTOOLS.");
        }
        return "11.0";
    }

    public static bool HasExtension(string path, string extension) => 
        string.Equals(Path.GetExtension(path), extension, StringComparison.InvariantCultureIgnoreCase);

    private static bool IsValidPackageName(string value)
    {
        if (!packageNameRegex.IsMatch(value))
        {
            return false;
        }
        string key = value.ToUpperInvariant();
        if (key != null)
        {
            int num;
            if (<>f__switch$map1 == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(0x16) {
                    { 
                        "CON",
                        0
                    },
                    { 
                        "PRN",
                        0
                    },
                    { 
                        "AUX",
                        0
                    },
                    { 
                        "NUL",
                        0
                    },
                    { 
                        "COM1",
                        0
                    },
                    { 
                        "COM2",
                        0
                    },
                    { 
                        "COM3",
                        0
                    },
                    { 
                        "COM4",
                        0
                    },
                    { 
                        "COM5",
                        0
                    },
                    { 
                        "COM6",
                        0
                    },
                    { 
                        "COM7",
                        0
                    },
                    { 
                        "COM8",
                        0
                    },
                    { 
                        "COM9",
                        0
                    },
                    { 
                        "LPT1",
                        0
                    },
                    { 
                        "LPT2",
                        0
                    },
                    { 
                        "LPT3",
                        0
                    },
                    { 
                        "LPT4",
                        0
                    },
                    { 
                        "LPT5",
                        0
                    },
                    { 
                        "LPT6",
                        0
                    },
                    { 
                        "LPT7",
                        0
                    },
                    { 
                        "LPT8",
                        0
                    },
                    { 
                        "LPT9",
                        0
                    }
                };
                <>f__switch$map1 = dictionary;
            }
            if (<>f__switch$map1.TryGetValue(key, out num) && (num == 0))
            {
                return false;
            }
        }
        return true;
    }

    public static void KillProcesses(Func<string, bool> shouldKill)
    {
        uint bytesCopied = 0x1000;
        uint[] processIds = new uint[bytesCopied];
        if (EnumProcesses(processIds, 4 * bytesCopied, out bytesCopied))
        {
            for (int i = 0; i < bytesCopied; i++)
            {
                IntPtr hProcess = OpenProcess(kQueryInformationLimitedFlag | kProcessTerminateFlag, false, (int) processIds[i]);
                if (hProcess != IntPtr.Zero)
                {
                    int capacity = 0x400;
                    StringBuilder lpExeName = new StringBuilder(capacity);
                    QueryFullProcessImageName(hProcess, 0, lpExeName, ref capacity);
                    if (shouldKill(lpExeName.ToString()))
                    {
                        TerminateProcess(hProcess, 0);
                    }
                    CloseHandle(hProcess);
                }
            }
        }
    }

    public static void MoveDirectory(string source, string destination, Func<string, bool> shouldOverwriteFile = null)
    {
        if (!Directory.Exists(destination))
        {
            Directory.CreateDirectory(destination);
        }
        foreach (string str in Directory.GetFiles(source))
        {
            string path = Path.Combine(destination, Path.GetFileName(str));
            if (File.Exists(path))
            {
                if ((shouldOverwriteFile != null) && !shouldOverwriteFile(path))
                {
                    continue;
                }
                File.Delete(path);
            }
            File.Move(str, path);
        }
        foreach (string str3 in Directory.GetDirectories(source))
        {
            MoveDirectory(str3, Path.Combine(destination, Path.GetFileName(str3)), shouldOverwriteFile);
        }
        Directory.Delete(source, true);
    }

    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenProcess(int dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);
    [DllImport("kernel32.dll", SetLastError=true)]
    private static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] int dwFlags, [Out] StringBuilder lpExeName, ref int lpdwSize);
    public static int RunAndWait(string fileName, string arguments, out string result, IDictionary<string, string> environmentVariables = null)
    {
        ProcessStartInfo si = new ProcessStartInfo(fileName, arguments) {
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            UseShellExecute = false
        };
        if (environmentVariables != null)
        {
            foreach (KeyValuePair<string, string> pair in environmentVariables)
            {
                si.EnvironmentVariables.Add(pair.Key, pair.Value);
            }
        }
        using (Program program = new Program(si))
        {
            program.Start();
            program.WaitForExit();
            result = program.GetStandardOutputAsString();
            return program.ExitCode;
        }
    }

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll", SetLastError=true)]
    private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);
    public static string TryValidatePackageName(string value)
    {
        if ((value == null) || (value.Length < 3))
        {
            return null;
        }
        StringBuilder builder = new StringBuilder(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (char.IsLetterOrDigit(c) || (c == '-'))
            {
                builder.Append(c);
            }
            else if ((c == '.') && (i != (value.Length - 1)))
            {
                builder.Append(c);
            }
        }
        value = builder.ToString();
        if (!IsValidPackageName(value))
        {
            return null;
        }
        return value;
    }

    public static bool UseIl2CppScriptingBackend() => 
        (PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) == ScriptingImplementation.IL2CPP);

    public static string AssemblyCSharpFirstPassName =>
        ("Assembly-CSharp-firstpass" + EditorSettings.Internal_UserGeneratedProjectSuffix);

    public static string AssemblyCSharpName =>
        ("Assembly-CSharp" + EditorSettings.Internal_UserGeneratedProjectSuffix);

    public static string AssemblyUnityScriptFirstPassName =>
        ("Assembly-UnityScript-firstpass" + EditorSettings.Internal_UserGeneratedProjectSuffix);

    public static string AssemblyUnityScriptName =>
        ("Assembly-UnityScript" + EditorSettings.Internal_UserGeneratedProjectSuffix);
}

