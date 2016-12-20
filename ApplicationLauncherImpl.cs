using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.Utils;

internal class ApplicationLauncherImpl
{
    private readonly string configuration;
    private readonly WSABuildAndRunDeployTarget deployTarget;
    private readonly WSASDK globalWSASDK;
    private readonly string installPath;
    private const uint kCppApp = 1;
    private const uint kCSharpApp = 2;
    private const uint kNormalApp = 4;
    private const uint kUniversalApp = 0x10;
    private const uint kWindowsPhoneApp = 0x20;
    private const uint kWindowsStoreApp = 8;
    private readonly string packageName;
    private readonly string platform;
    private readonly string playerPackage;
    private readonly ScriptingImplementation scriptingBackend;
    private readonly WSASDK wsaSDK;

    public ApplicationLauncherImpl(string playerPackage, string installPath, string packageName, string configuration, WSASDK wsaSDK, WSASDK globalWSASDK, WSABuildAndRunDeployTarget deployTarget)
    {
        this.playerPackage = playerPackage;
        this.installPath = installPath;
        this.packageName = packageName;
        this.configuration = configuration;
        this.wsaSDK = wsaSDK;
        this.globalWSASDK = globalWSASDK;
        this.deployTarget = deployTarget;
        DetermineBuildPlatform(wsaSDK, deployTarget, out this.platform);
        this.scriptingBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA);
    }

    public void Build()
    {
        switch (this.globalWSASDK)
        {
            case WSASDK.SDK80:
            case WSASDK.SDK81:
            case WSASDK.PhoneSDK81:
            case WSASDK.UniversalSDK81:
                Environment.SetEnvironmentVariable("VisualStudioVersion", Utility.GetVSVersion());
                break;

            case WSASDK.UWP:
                Environment.SetEnvironmentVariable("VisualStudioVersion", Utility.GetVSVersion());
                if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) == ScriptingImplementation.WinRTDotNET)
                {
                    EditorUtility.DisplayProgressBar("Deploying Player", "Running NuGet", 0.25f);
                    string str = string.Format("restore \"{0}/{1}/project.json\" -NonInteractive -Source https://api.nuget.org/v3/index.json", this.installPath, this.GetProjectName());
                    this.RunNuGet(str);
                }
                break;
        }
        string args = string.Format("\"{0}\\{1}.sln\" /nologo /maxcpucount /p:Configuration={2} /p:Platform={3} /p:SolutionDir=\"{4}\\\\\" /t:Build /clp:Verbosity=minimal", new object[] { this.installPath, this.packageName, this.configuration, this.platform, this.installPath.Replace(@"\", @"\\") });
        this.KillRunningAppInstances();
        EditorUtility.DisplayProgressBar("Deploying Player", string.Format("Building solution with Visual Studio version {0}, {1}|{2}", Utility.GetVSVersion(), this.configuration, this.platform), 0.5f);
        this.RunMSBuild(args);
    }

    private void BuildUsingMSBuild(bool registerAppxLayout)
    {
        string vSProjectPath = this.GetVSProjectPath();
        this.RunMSBuild(string.Format("\"{0}\" /nologo /maxcpucount /p:Configuration={1} /p:Platform={2} /p:SolutionDir=\"{3}\\\\\" /t:Publish{4} /clp:Verbosity=minimal", new object[] { vSProjectPath, this.configuration, this.platform, this.installPath.Replace(@"\", @"\\"), !registerAppxLayout ? "" : ";RegisterAppxLayout" }));
    }

    private static void DetermineBuildPlatform(WSASDK wsaSDK, WSABuildAndRunDeployTarget deployTarget, out string buildPlatform)
    {
        switch (wsaSDK)
        {
            case WSASDK.SDK80:
            case WSASDK.SDK81:
                buildPlatform = "x86";
                return;

            case WSASDK.PhoneSDK81:
                buildPlatform = "ARM";
                return;

            case WSASDK.UWP:
                if (deployTarget != WSABuildAndRunDeployTarget.LocalMachine)
                {
                    if (deployTarget != WSABuildAndRunDeployTarget.WindowsPhone)
                    {
                        throw new NotImplementedException("Specify correct platform for " + deployTarget.ToString());
                    }
                    buildPlatform = "ARM";
                    return;
                }
                buildPlatform = "x86";
                return;
        }
        throw new Exception(string.Format("Could not determine build platform for selected wsaSDK: {0}", wsaSDK));
    }

    private string GetArgumentForPlayerRunner()
    {
        string str;
        <GetArgumentForPlayerRunner>c__AnonStorey0 storey = new <GetArgumentForPlayerRunner>c__AnonStorey0 {
            $this = this
        };
        uint num = 0;
        if (this.scriptingBackend == ScriptingImplementation.IL2CPP)
        {
            num |= 1;
        }
        else
        {
            num |= 2;
        }
        if (this.globalWSASDK == WSASDK.UniversalSDK81)
        {
            num |= 0x10;
        }
        else
        {
            num |= 4;
        }
        switch (this.wsaSDK)
        {
            case WSASDK.SDK80:
            case WSASDK.SDK81:
                num |= 8;
                break;

            case WSASDK.PhoneSDK81:
                num |= 0x20;
                break;

            case WSASDK.UWP:
                switch (this.deployTarget)
                {
                    case WSABuildAndRunDeployTarget.LocalMachine:
                        num |= 8;
                        goto Label_00F1;

                    case WSABuildAndRunDeployTarget.WindowsPhone:
                        num |= 0x20;
                        goto Label_00F1;
                }
                throw new NotImplementedException("Specify correct configuration flags for " + this.deployTarget.ToString());

            default:
                throw new Exception(string.Format("Invalid Windows Store Apps SDK: {0}", this.wsaSDK));
        }
    Label_00F1:
        str = null;
        switch (num)
        {
            case 13:
            {
                string[] components = new string[] { this.installPath, "build", "bin", "Win32", this.configuration, "AppxManifest.xml" };
                return Paths.Combine(components);
            }
            case 14:
            {
                string[] textArray1 = new string[] { this.installPath, this.packageName, "bin", this.platform, this.configuration, "AppxManifest.xml" };
                return Paths.Combine(textArray1);
            }
            case 0x19:
            {
                string[] textArray4 = new string[] { this.installPath, this.configuration, this.packageName + ".Windows", "AppxManifest.xml" };
                return Paths.Combine(textArray4);
            }
            case 0x1a:
            {
                string[] textArray3 = new string[] { this.installPath, this.packageName, this.packageName + ".Windows", "bin", this.platform, this.configuration, "AppxManifest.xml" };
                return Paths.Combine(textArray3);
            }
            case 0x25:
            {
                string[] textArray6 = new string[] { this.installPath, "AppPackages" };
                str = Paths.Combine(textArray6);
                break;
            }
            case 0x26:
            {
                string[] textArray5 = new string[] { this.installPath, this.packageName, "AppPackages" };
                str = Paths.Combine(textArray5);
                break;
            }
            case 0x31:
            {
                string[] textArray8 = new string[] { this.installPath, "AppPackages", this.packageName + ".WindowsPhone" };
                str = Paths.Combine(textArray8);
                break;
            }
            case 50:
            {
                string[] textArray7 = new string[] { this.installPath, this.packageName, this.packageName + ".WindowsPhone", "AppPackages" };
                str = Paths.Combine(textArray7);
                break;
            }
            default:
                throw new Exception(string.Format("Invalid configuration flags: 0x{0:X}", num));
        }
        storey.configurationToLookFor = !this.configuration.Equals("Release", StringComparison.InvariantCultureIgnoreCase) ? ("_" + this.configuration) : "";
        string str3 = Enumerable.FirstOrDefault<string>(Directory.GetFiles(str, "*.appx", SearchOption.AllDirectories), new Func<string, bool>(storey, (IntPtr) this.<>m__0));
        if (string.IsNullOrEmpty(str3))
        {
            str3 = Enumerable.FirstOrDefault<string>(Directory.GetFiles(str, "*.appxbundle", SearchOption.AllDirectories), new Func<string, bool>(storey, (IntPtr) this.<>m__1));
        }
        return str3;
    }

    private string GetNuGetPath()
    {
        string path = Path.Combine(this.playerPackage, @"Tools\nuget.exe");
        if (!File.Exists(path))
        {
            throw new Exception("Failed to locate suitable " + path);
        }
        return path;
    }

    private string GetProjectExt()
    {
        return "csproj";
    }

    private string GetProjectName()
    {
        string packageName = this.packageName;
        if (this.globalWSASDK != WSASDK.UniversalSDK81)
        {
            return packageName;
        }
        WSASDK wsaSDK = this.wsaSDK;
        if (wsaSDK != WSASDK.SDK81)
        {
            if (wsaSDK != WSASDK.PhoneSDK81)
            {
                throw new Exception(string.Format("Invalid Windows Store Apps SDK for universal app: {0}", this.wsaSDK));
            }
        }
        else
        {
            return (packageName + ".Windows");
        }
        return (packageName + ".WindowsPhone");
    }

    private static string GetVSDevEnvPath(string vsEnvVariable)
    {
        string environmentVariable = Environment.GetEnvironmentVariable(vsEnvVariable);
        if (string.IsNullOrEmpty(environmentVariable))
        {
            throw new Exception(string.Format("Failed to find '{0}' env variable, ensure that it exists", vsEnvVariable));
        }
        string path = Path.Combine(environmentVariable, @"..\IDE\devenv.com");
        if (!File.Exists(path))
        {
            throw new Exception("Failed to locate " + path);
        }
        return path;
    }

    private string GetVSProjectPath()
    {
        if (this.globalWSASDK == WSASDK.UniversalSDK81)
        {
            string projectName = this.GetProjectName();
            string[] textArray1 = new string[] { this.installPath, this.packageName, projectName, projectName };
            return (Paths.Combine(textArray1) + "." + this.GetProjectExt());
        }
        string[] components = new string[] { this.installPath, this.packageName, this.packageName };
        return (Paths.Combine(components) + "." + this.GetProjectExt());
    }

    private void KillRunningAppInstances()
    {
        if (((this.platform == "x86") || (this.platform == "x64")) || (this.platform == "Win32"))
        {
            Utility.KillProcesses(new Func<string, bool>(this, (IntPtr) this.<KillRunningAppInstances>m__0));
        }
    }

    private void RegisterLayout()
    {
        if (Utility.GetVSVersion() == "14.0")
        {
            this.RegisterLayoutOnLocalMachineUsingVS2015();
        }
        else
        {
            this.BuildUsingMSBuild(true);
        }
    }

    private void RegisterLayoutOnLocalMachineUsingVS2015()
    {
        Dictionary<string, string> environmentVariables = new Dictionary<string, string> {
            { 
                "VsPromptUninstallNonVsPackage",
                "1"
            }
        };
        this.RunVS2015(string.Format("\"{0}\\{1}.sln\" /deploy \"{2}|{3}\"", new object[] { this.installPath.Replace(@"\", @"\\"), this.packageName, this.configuration, this.platform }), environmentVariables);
    }

    public void Run()
    {
        string commandLine = string.Format("\"{0}\"", this.GetArgumentForPlayerRunner());
        switch (this.wsaSDK)
        {
            case WSASDK.SDK80:
            case WSASDK.SDK81:
                this.RegisterLayout();
                this.RunOnLocalMachine(commandLine);
                return;

            case WSASDK.PhoneSDK81:
                this.RunOnPhone(commandLine);
                return;

            case WSASDK.UWP:
                switch (this.deployTarget)
                {
                    case WSABuildAndRunDeployTarget.LocalMachine:
                        this.RegisterLayout();
                        this.RunOnLocalMachine(commandLine);
                        return;

                    case WSABuildAndRunDeployTarget.WindowsPhone:
                        this.BuildUsingMSBuild(false);
                        commandLine = commandLine + " -noMDIL";
                        this.RunOnPhone(commandLine);
                        return;
                }
                throw new NotImplementedException("Run operation not implemented or " + this.deployTarget.ToString());
        }
        throw new Exception(string.Format("Unknown Windows Store Apps SDK for deployment: {0}", this.wsaSDK));
    }

    private void RunMSBuild(string args)
    {
        string str;
        string mSBuildExePath = Utility.GetMSBuildExePath(this.wsaSDK);
        if (Utility.RunAndWait(mSBuildExePath, args, out str, null) != 0)
        {
            if (str.Contains("used by another process"))
            {
                str = "Failed to build Visual Studio project, possibly because project is opened externally or application is already running.\nOutput:\n" + str;
            }
            else
            {
                str = string.Format("Failed to build Visual Studio project using arguments '{0} {1}'.\nOutput:{2}\n", mSBuildExePath, args, str);
            }
            throw new Exception(str);
        }
    }

    private void RunNuGet(string args)
    {
        string str;
        string nuGetPath = this.GetNuGetPath();
        if (Utility.RunAndWait(nuGetPath, args, out str, null) != 0)
        {
            throw new Exception(string.Format("Failed to run NuGet using arguments '{0} {1}'.\nOutput:{2}\n", nuGetPath, args, str));
        }
    }

    private void RunOnLocalMachine(string commandLine)
    {
        string str;
        if (Utility.RunAndWait(Path.Combine(this.playerPackage, @"Tools\MetroPlayerRunner.exe"), commandLine, out str, null) < 0)
        {
            throw new Exception("Deployment failed. Output:\n" + str);
        }
    }

    private void RunOnPhone(string commandLine)
    {
        string str;
        if (Utility.RunAndWait(Path.Combine(this.playerPackage, @"Tools\WindowsPhonePlayerRunner.exe"), commandLine, out str, null) != 0)
        {
            throw new Exception("Deployment failed. Output:\n" + str);
        }
    }

    private void RunVS2015(string args, IDictionary<string, string> environmentVariables)
    {
        string str;
        string vSDevEnvPath = GetVSDevEnvPath("VS140COMNTOOLS");
        if (Utility.RunAndWait(vSDevEnvPath, args, out str, environmentVariables) != 0)
        {
            if (str.Contains("used by another process"))
            {
                str = "Failed to build Visual Studio project, possibly because project is opened externally or application is already running.\nOutput:\n" + str;
            }
            else
            {
                str = string.Format("Failed to build Visual Studio project using arguments '{0} {1}'.\nOutput:{2}\n", vSDevEnvPath, args, str);
            }
            throw new Exception(str);
        }
    }

    [CompilerGenerated]
    private sealed class <GetArgumentForPlayerRunner>c__AnonStorey0
    {
        internal ApplicationLauncherImpl $this;
        internal string configurationToLookFor;

        internal bool <>m__0(string x)
        {
            return x.EndsWith(this.$this.platform + this.configurationToLookFor + ".appx", StringComparison.InvariantCultureIgnoreCase);
        }

        internal bool <>m__1(string x)
        {
            return x.EndsWith(this.$this.platform + this.configurationToLookFor + ".appxbundle", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

