using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Utils;
using UnityEditorInternal;

internal static class Utility
{
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map1;
    private static string frameworkPath;
    private const FileAttributes INVALID_FILE_ATTRIBUTES = -1;
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
        <CopyDirectoryContents>c__AnonStorey0 storey = new <CopyDirectoryContents>c__AnonStorey0 {
            recursive = recursive,
            sourceDirectory = sourceDirectory,
            destinationDirectory = destinationDirectory
        };
        storey.sourceDirectory = Path.GetFullPath(storey.sourceDirectory);
        storey.destinationDirectory = Path.GetFullPath(storey.destinationDirectory);
        using (new ProfilerBlock("Utility.CopyDirectoryContents"))
        {
            CreateDirectory(storey.destinationDirectory);
            EnumerateDirectoryRecursive(storey.sourceDirectory, new Func<string, FileAttributes, bool>(storey.<>m__0));
        }
    }

    public static void CopyFile(string sourceFile, string destinationFile)
    {
        FileAttributes fileAttributesW = GetFileAttributesW(destinationFile);
        if ((fileAttributesW != -1) && (((fileAttributesW & FileAttributes.ReadOnly) != 0) || ((fileAttributesW & FileAttributes.Hidden) != 0)))
        {
            SetFileAttributesW(destinationFile, (fileAttributesW & ~FileAttributes.ReadOnly) & ~FileAttributes.Hidden);
        }
        if (!CopyFileW(sourceFile, destinationFile, false))
        {
            ThrowWin32IOException(Marshal.GetLastWin32Error(), $"Failed to copy {sourceFile} to {destinationFile}");
        }
    }

    [DllImport("Kernel32.dll", SetLastError=true, ExactSpelling=true)]
    private static extern bool CopyFileW([MarshalAs(UnmanagedType.LPWStr)] string lpExistingFileName, [MarshalAs(UnmanagedType.LPWStr)] string lpNewFileName, bool bFailIfExists);
    public static void CreateDirectory(string path)
    {
        if (!DirectoryExists(path))
        {
            string directoryName = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directoryName))
            {
                CreateDirectory(directoryName);
            }
            if (!CreateDirectoryW(path, IntPtr.Zero))
            {
                ThrowWin32IOException(Marshal.GetLastWin32Error(), "Failed to create directory at " + path);
            }
        }
    }

    [DllImport("Kernel32.dll", SetLastError=true, ExactSpelling=true)]
    private static extern bool CreateDirectoryW([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, IntPtr lpSecurityAttributes);
    public static void DeleteDirectoryRecursive(string path)
    {
        <DeleteDirectoryRecursive>c__AnonStorey3 storey = new <DeleteDirectoryRecursive>c__AnonStorey3 {
            path = path
        };
        using (new ProfilerBlock("Utility.DeleteDirectoryRecursive"))
        {
            if (EnumerateDirectoryRecursive(storey.path, new Func<string, FileAttributes, bool>(storey.<>m__0)) && !RemoveDirectoryW(storey.path))
            {
                ThrowWin32IOException(Marshal.GetLastWin32Error(), "Failed to delete " + storey.path);
            }
        }
    }

    public static void DeleteFileAccountingForReadOnly(string path)
    {
        FileInfo info = new FileInfo(path);
        if (info.Exists)
        {
            info.Attributes &= ~FileAttributes.ReadOnly;
            info.Delete();
        }
    }

    [DllImport("Kernel32.dll", SetLastError=true, ExactSpelling=true)]
    private static extern bool DeleteFileW([MarshalAs(UnmanagedType.LPWStr)] string lpFileName);
    public static bool DirectoryExists(string path)
    {
        FileAttributes fileAttributesW = GetFileAttributesW(path);
        if (fileAttributesW == -1)
        {
            return false;
        }
        return ((fileAttributesW & FileAttributes.Directory) != 0);
    }

    private static unsafe bool EnumerateDirectoryRecursive(string path, Func<string, FileAttributes, bool> onFileEnumerated)
    {
        Win32FindData data;
        string lpFileName = (path[path.Length - 1] != '\\') ? (path + @"\*") : (path + '*');
        using (FindHandle handle = FindFirstFileExW(lpFileName, FindInfoLevel.Basic, (void*) &data, FindSearchOperation.NameMatch, IntPtr.Zero, FindAdditionalFlags.LargeFetch))
        {
            if (!handle.IsValid())
            {
                int errorCode = Marshal.GetLastWin32Error();
                switch (errorCode)
                {
                    case 2:
                    case 3:
                        return false;
                }
                ThrowWin32IOException(errorCode, "Failed to enumerate " + path);
            }
            while ((((data.cFileName.FixedElementField == '\0') || ((data.cFileName.FixedElementField == '.') && (&data.cFileName.FixedElementField[1] == '\0'))) || ((((data.cFileName.FixedElementField == '.') && (&data.cFileName.FixedElementField[1] == '.')) && (&data.cFileName.FixedElementField[2] == '\0')) || onFileEnumerated(new string(&data.cFileName.FixedElementField), data.dwFileAttributes))) && FindNextFileW(handle, (void*) &data))
            {
            }
        }
        return true;
    }

    [DllImport("Psapi.dll")]
    private static extern bool EnumProcesses([In, Out, MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.U4)] uint[] processIds, uint arraySizeBytes, [MarshalAs(UnmanagedType.U4)] out uint bytesCopied);
    [DllImport("Kernel32.dll", SetLastError=true, ExactSpelling=true)]
    private static extern bool FindClose(FindHandle hFindFile);
    [DllImport("Kernel32.dll", SetLastError=true, ExactSpelling=true)]
    private static extern unsafe FindHandle FindFirstFileExW([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, FindInfoLevel fInfoLevelId, void* lpFindFileData, FindSearchOperation fSearchOp, IntPtr lpSearchFilter, FindAdditionalFlags additionalFlags);
    [DllImport("Kernel32.dll", SetLastError=true, ExactSpelling=true)]
    private static extern unsafe bool FindNextFileW(FindHandle hFindFile, void* lpFindFileData);
    internal static Version GetDesiredUWPSDK()
    {
        Version[] source = UWPReferences.GetInstalledSDKVersions().ToArray<Version>();
        if (source.Length == 0)
        {
            return new Version(10, 0, 0x2800, 0);
        }
        Version version2 = source.Max<Version>();
        string wsaUWPSDK = EditorUserBuildSettings.wsaUWPSDK;
        if (!string.IsNullOrEmpty(wsaUWPSDK))
        {
            foreach (Version version3 in source)
            {
                if (version3.ToString() == wsaUWPSDK)
                {
                    return version3;
                }
            }
        }
        return version2;
    }

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

    [DllImport("Kernel32.dll", SetLastError=true, ExactSpelling=true)]
    private static extern FileAttributes GetFileAttributesW([MarshalAs(UnmanagedType.LPWStr)] string lpFileName);
    public static string GetFrameworkPath()
    {
        if (frameworkPath == null)
        {
            string[] strArray = new string[] { @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Microsoft\VisualStudio\SxS\VC7", @"HKEY_CURRENT_USER\SOFTWARE\Wow6432Node\Microsoft\Microsoft\VisualStudio\SxS\VC7", @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft\VisualStudio\SxS\VC7", @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Microsoft\VisualStudio\SxS\VC7" };
            foreach (string str in strArray)
            {
                string str2 = RegistryUtil.GetRegistryStringValue(str, "FrameworkDir32", null, RegistryView._32);
                if (!string.IsNullOrEmpty(str2))
                {
                    string str3 = RegistryUtil.GetRegistryStringValue(str, "FrameworkVer32", null, RegistryView._32);
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
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("VS120COMNTOOLS")))
        {
            throw new Exception("Failed to locate env variables VS140COMNTOOLS or VS120COMNTOOLS.");
        }
        return "12.0";
    }

    public static bool HasExtension(string path, string extension) => 
        string.Equals(Path.GetExtension(path), extension, StringComparison.InvariantCultureIgnoreCase);

    public static bool IsDirectoryEmpty(string path)
    {
        <IsDirectoryEmpty>c__AnonStorey1 storey = new <IsDirectoryEmpty>c__AnonStorey1 {
            hasAnyFiles = false
        };
        EnumerateDirectoryRecursive(path, new Func<string, FileAttributes, bool>(storey.<>m__0));
        return !storey.hasAnyFiles;
    }

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
        using (new ProfilerBlock("Utility.MoveDirectory"))
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("source cannot be null or empty", "source");
            }
            if (string.IsNullOrEmpty(destination))
            {
                throw new ArgumentException("destination cannot be null or empty", "destination");
            }
            string str = Path.GetPathRoot(source).ToLower();
            string str2 = Path.GetPathRoot(destination).ToLower();
            if (str == str2)
            {
                bool flag = DirectoryExists(destination);
                if (!flag || IsDirectoryEmpty(destination))
                {
                    if (flag)
                    {
                        RemoveDirectoryW(destination);
                    }
                    else
                    {
                        CreateDirectory(Path.GetDirectoryName(destination));
                    }
                    if (!MoveFileExW(source, destination, MoveFileExFlags.NONE))
                    {
                        ThrowWin32IOException(Marshal.GetLastWin32Error(), $"Failed to move {source} to {destination}.");
                    }
                    return;
                }
            }
            using (new ProfilerBlock("Utility.MoveDirectorySlowPath"))
            {
                MoveDirectorySlowPath(source, destination, shouldOverwriteFile);
            }
        }
    }

    private static void MoveDirectorySlowPath(string source, string destination, Func<string, bool> shouldOverwriteFile)
    {
        <MoveDirectorySlowPath>c__AnonStorey2 storey = new <MoveDirectorySlowPath>c__AnonStorey2 {
            source = source,
            destination = destination,
            shouldOverwriteFile = shouldOverwriteFile
        };
        CreateDirectory(storey.destination);
        EnumerateDirectoryRecursive(storey.source, new Func<string, FileAttributes, bool>(storey.<>m__0));
        RemoveDirectoryW(storey.source);
    }

    [DllImport("kernel32.dll", SetLastError=true)]
    private static extern bool MoveFileExW([MarshalAs(UnmanagedType.LPWStr)] string source, [MarshalAs(UnmanagedType.LPWStr)] string destination, MoveFileExFlags flags);
    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenProcess(int dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);
    [DllImport("kernel32.dll", SetLastError=true)]
    private static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] int dwFlags, [Out] StringBuilder lpExeName, ref int lpdwSize);
    [DllImport("Kernel32.dll", SetLastError=true, ExactSpelling=true)]
    private static extern bool RemoveDirectoryW([MarshalAs(UnmanagedType.LPWStr)] string lpFileName);
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

    [DllImport("Kernel32.dll", SetLastError=true, ExactSpelling=true)]
    private static extern bool SetFileAttributesW([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, FileAttributes dwFileAttributes);
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll", SetLastError=true)]
    private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);
    private static void ThrowWin32IOException(int errorCode, string message)
    {
        string str = new Win32Exception(errorCode).Message;
        throw new IOException($"{message}: {str}");
    }

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

    [CompilerGenerated]
    private sealed class <CopyDirectoryContents>c__AnonStorey0
    {
        internal string destinationDirectory;
        internal bool recursive;
        internal string sourceDirectory;

        internal bool <>m__0(string fileName, FileAttributes fileAttributes)
        {
            if ((fileAttributes & FileAttributes.Directory) != 0)
            {
                if (this.recursive)
                {
                    Utility.CopyDirectoryContents(Path.Combine(this.sourceDirectory, fileName), Path.Combine(this.destinationDirectory, fileName), this.recursive);
                }
            }
            else
            {
                Utility.CopyFile(Path.Combine(this.sourceDirectory, fileName), Path.Combine(this.destinationDirectory, fileName));
            }
            return true;
        }
    }

    [CompilerGenerated]
    private sealed class <DeleteDirectoryRecursive>c__AnonStorey3
    {
        internal string path;

        internal bool <>m__0(string fileName, FileAttributes fileAttributes)
        {
            string path = Path.Combine(this.path, fileName);
            if ((fileAttributes & FileAttributes.Directory) != 0)
            {
                Utility.DeleteDirectoryRecursive(path);
            }
            else
            {
                if ((fileAttributes & FileAttributes.ReadOnly) != 0)
                {
                    Utility.SetFileAttributesW(path, fileAttributes & ~FileAttributes.ReadOnly);
                }
                if (!Utility.DeleteFileW(path))
                {
                    Utility.ThrowWin32IOException(Marshal.GetLastWin32Error(), "Failed to delete " + path);
                }
            }
            return true;
        }
    }

    [CompilerGenerated]
    private sealed class <IsDirectoryEmpty>c__AnonStorey1
    {
        internal bool hasAnyFiles;

        internal bool <>m__0(string fileName, FileAttributes fileAttributes)
        {
            this.hasAnyFiles = true;
            return false;
        }
    }

    [CompilerGenerated]
    private sealed class <MoveDirectorySlowPath>c__AnonStorey2
    {
        internal string destination;
        internal Func<string, bool> shouldOverwriteFile;
        internal string source;

        internal bool <>m__0(string fileName, FileAttributes fileAttributes)
        {
            string source = Path.Combine(this.source, fileName);
            string destination = Path.Combine(this.destination, fileName);
            if ((fileAttributes & FileAttributes.Directory) != 0)
            {
                Utility.MoveDirectory(source, destination, this.shouldOverwriteFile);
                return true;
            }
            FileAttributes fileAttributesW = Utility.GetFileAttributesW(destination);
            if (fileAttributesW == -1)
            {
                if (!Utility.MoveFileExW(source, destination, Utility.MoveFileExFlags.COPY_ALLOWED))
                {
                    Utility.ThrowWin32IOException(Marshal.GetLastWin32Error(), $"Failed to move {source} to {destination}");
                }
            }
            else if ((this.shouldOverwriteFile == null) || this.shouldOverwriteFile(destination))
            {
                if ((fileAttributesW & FileAttributes.ReadOnly) != 0)
                {
                    Utility.SetFileAttributesW(destination, fileAttributesW & ~FileAttributes.ReadOnly);
                }
                if (!Utility.MoveFileExW(source, destination, Utility.MoveFileExFlags.COPY_ALLOWED | Utility.MoveFileExFlags.REPLACE_EXISTING))
                {
                    Utility.ThrowWin32IOException(Marshal.GetLastWin32Error(), $"Failed to move {source} to {destination}");
                }
            }
            return true;
        }
    }

    [Flags]
    private enum FindAdditionalFlags
    {
        None,
        CaseSensitive,
        LargeFetch
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct FindHandle : IDisposable
    {
        private IntPtr value;
        public bool IsValid() => 
            (this.value.ToPointer() != -1);

        public void Dispose()
        {
            if (this.IsValid())
            {
                Utility.FindClose(this);
            }
        }
    }

    private enum FindInfoLevel
    {
        Standard,
        Basic,
        MaxInfoLevel
    }

    private enum FindSearchOperation
    {
        NameMatch,
        LimitToDirectories,
        LimitToDevices
    }

    [Flags]
    private enum MoveFileExFlags
    {
        COPY_ALLOWED = 2,
        CREATE_HARDLINK = 0x10,
        DELAY_UNTIL_REBOOT = 4,
        FAIL_IF_NOT_TRACKABLE = 0x20,
        NONE = 0,
        REPLACE_EXISTING = 1,
        WRITE_THROUGH = 8
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Win32FindData
    {
        public FileAttributes dwFileAttributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
        public uint dwReserved0;
        public uint dwReserved1;
        [FixedBuffer(typeof(char), 260)]
        public <cFileName>__FixedBuffer0 cFileName;
        [FixedBuffer(typeof(char), 14)]
        public <cAlternateFileName>__FixedBuffer1 cAlternateFileName;
        [StructLayout(LayoutKind.Sequential, Size=0x1c), UnsafeValueType, CompilerGenerated]
        public struct <cAlternateFileName>__FixedBuffer1
        {
            public char FixedElementField;
        }

        [StructLayout(LayoutKind.Sequential, Size=520), UnsafeValueType, CompilerGenerated]
        public struct <cFileName>__FixedBuffer0
        {
            public char FixedElementField;
        }
    }
}

