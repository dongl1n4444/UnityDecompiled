using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditor;
using UnityEditor.Modules;

internal class ApplicationLauncher
{
    public static void BuildAndRun(BuildLaunchPlayerArgs args)
    {
        if (args.target != BuildTarget.WSAPlayer)
        {
            throw new ArgumentException("Invalid build target.", "target");
        }
        if (args.installPath.StartsWith(@"\\") || args.installPath.StartsWith("//"))
        {
            throw new ArgumentException("Can not Build & Run to network share!");
        }
        WSASDK wsaSDK = EditorUserBuildSettings.wsaSDK;
        if (wsaSDK == WSASDK.UniversalSDK81)
        {
            switch (EditorUserBuildSettings.wsaBuildAndRunDeployTarget)
            {
                case WSABuildAndRunDeployTarget.LocalMachine:
                    wsaSDK = WSASDK.SDK81;
                    break;

                case WSABuildAndRunDeployTarget.WindowsPhone:
                    wsaSDK = WSASDK.PhoneSDK81;
                    break;
            }
        }
        if (((wsaSDK == WSASDK.UniversalSDK81) || (wsaSDK == WSASDK.UWP)) && (EditorUserBuildSettings.wsaBuildAndRunDeployTarget == WSABuildAndRunDeployTarget.LocalMachineAndWindowsPhone))
        {
            BuildAndRunOnBoth(args, wsaSDK);
        }
        else
        {
            BuildAndRunOnSingle(args, wsaSDK, EditorUserBuildSettings.wsaBuildAndRunDeployTarget);
        }
    }

    private static void BuildAndRunOnBoth(BuildLaunchPlayerArgs args, WSASDK wsaSDK)
    {
        bool flag;
        <BuildAndRunOnBoth>c__AnonStorey0 storey = new <BuildAndRunOnBoth>c__AnonStorey0 {
            storeException = null,
            phoneException = null
        };
        if (wsaSDK != WSASDK.UniversalSDK81)
        {
            if (wsaSDK != WSASDK.UWP)
            {
                throw new Exception("Unexpected WSASDK: " + wsaSDK.ToString());
            }
        }
        else
        {
            storey.storeLauncher = CreateLauncherInstance(args, WSASDK.SDK81, WSABuildAndRunDeployTarget.LocalMachine);
            storey.phoneLauncher = CreateLauncherInstance(args, WSASDK.PhoneSDK81, WSABuildAndRunDeployTarget.WindowsPhone);
            flag = true;
            goto Label_008B;
        }
        storey.storeLauncher = CreateLauncherInstance(args, WSASDK.UWP, WSABuildAndRunDeployTarget.LocalMachine);
        storey.phoneLauncher = CreateLauncherInstance(args, WSASDK.UWP, WSABuildAndRunDeployTarget.WindowsPhone);
        flag = false;
    Label_008B:
        storey.storeLauncher.Build();
        storey.phoneLauncher.Build();
        if (flag)
        {
            EditorUtility.DisplayProgressBar("Deploying Player", "Running on local machine and windows phone", 0.75f);
            Thread thread = new Thread(new ThreadStart(storey.<>m__0));
            Thread thread2 = new Thread(new ThreadStart(storey.<>m__1));
            thread.Start();
            thread2.Start();
            thread.Join();
            thread2.Join();
        }
        else
        {
            EditorUtility.DisplayProgressBar("Deploying Player", "Running on local machine", 0.75f);
            try
            {
                storey.storeLauncher.Run();
            }
            catch (Exception exception)
            {
                storey.storeException = exception;
            }
            EditorUtility.DisplayProgressBar("Deploying Player", "Running on windows phone", 0.75f);
            try
            {
                storey.phoneLauncher.Run();
            }
            catch (Exception exception2)
            {
                storey.phoneException = exception2;
            }
        }
        if ((storey.storeException != null) || (storey.phoneException != null))
        {
            string str = "";
            throw new Exception(str + ((storey.storeException == null) ? "" : storey.storeException.Message) + ((storey.phoneException == null) ? "" : storey.phoneException.Message));
        }
    }

    private static void BuildAndRunOnSingle(BuildLaunchPlayerArgs args, WSASDK wsaSDK, WSABuildAndRunDeployTarget deployTarget)
    {
        ApplicationLauncherImpl impl = CreateLauncherInstance(args, wsaSDK, deployTarget);
        impl.Build();
        EditorUtility.DisplayProgressBar("Deploying Player", "Running", 0.75f);
        impl.Run();
    }

    private static ApplicationLauncherImpl CreateLauncherInstance(BuildLaunchPlayerArgs args, WSASDK wsaSDK, WSABuildAndRunDeployTarget deployTarget)
    {
        string packageName = Utility.GetPackageName(true);
        return new ApplicationLauncherImpl(args.playerPackage, Path.GetFullPath(FileUtil.NiceWinPath(args.installPath)), Utility.GetVsName(), GetConfiguration(), wsaSDK, EditorUserBuildSettings.wsaSDK, deployTarget);
    }

    private static string GetConfiguration() => 
        (!EditorUserBuildSettings.development ? "Master" : "Release");

    [CompilerGenerated]
    private sealed class <BuildAndRunOnBoth>c__AnonStorey0
    {
        internal Exception phoneException;
        internal ApplicationLauncherImpl phoneLauncher;
        internal Exception storeException;
        internal ApplicationLauncherImpl storeLauncher;

        internal void <>m__0()
        {
            try
            {
                this.storeLauncher.Run();
            }
            catch (Exception exception)
            {
                this.storeException = exception;
            }
        }

        internal void <>m__1()
        {
            try
            {
                this.phoneLauncher.Run();
            }
            catch (Exception exception)
            {
                this.phoneException = exception;
            }
        }
    }
}

