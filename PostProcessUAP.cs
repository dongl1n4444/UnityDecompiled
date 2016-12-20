using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.Modules;
using UnityEditor.Scripting.Compilers;
using UnityEngine;

internal abstract class PostProcessUAP : PostProcessWSA
{
    private static Dictionary<PlayerSettings.SplashScreen.UnityLogoStyle, string> _defaultSplashScreens;
    private static string _platformAssemblyPath;
    private static string[] _uwpReferences;
    [CompilerGenerated]
    private static Func<string, string> <>f__am$cache0;

    public PostProcessUAP(BuildPostProcessArgs args, [Optional, DefaultParameterValue(null)] string stagingArea) : base(args, WSASDK.UWP, stagingArea)
    {
    }

    public override void CheckSafeProjectOverwrite()
    {
        base.CheckSafeProjectOverwrite();
        string str = Utility.CombinePath(base.InstallPath, base.VisualStudioName);
        string str2 = !this.UseIL2CPP() ? "App.cs" : "App.cpp";
        if (EditorUserBuildSettings.wsaUWPBuildType == WSAUWPBuildType.XAML)
        {
            if (File.Exists(Path.Combine(str, str2)))
            {
                throw new UnityException("Build path contains D3D UAP project which is incompatible with XAML project.");
            }
        }
        else if (File.Exists(Path.Combine(str, "App.xaml")))
        {
            throw new UnityException("Build path contains XAML UAP project which is incompatible with D3D project.");
        }
    }

    public override void CopyImages()
    {
        string str = Utility.CombinePath(base.PlayerPackage, @"Images\UAP");
        string str2 = base.CopyImage(PlayerSettings.WSAImageType.SplashScreenImage, PlayerSettings.WSAImageScale._100, null, "SplashScreen.scale-100.png");
        string str3 = base.CopyImage(PlayerSettings.WSAImageType.SplashScreenImage, PlayerSettings.WSAImageScale._125, null, "SplashScreen.scale-125.png");
        string str4 = base.CopyImage(PlayerSettings.WSAImageType.SplashScreenImage, PlayerSettings.WSAImageScale._150, null, "SplashScreen.scale-150.png");
        string str5 = base.CopyImage(PlayerSettings.WSAImageType.SplashScreenImage, PlayerSettings.WSAImageScale._200, Utility.CombinePath(str, base.GetDefaultSplashScreenImage()), "SplashScreen.scale-200.png");
        string str6 = base.CopyImage(PlayerSettings.WSAImageType.SplashScreenImage, PlayerSettings.WSAImageScale._400, null, "SplashScreen.scale-400.png");
        string[] strings = new string[] { str2, str3, str4, str5, str6 };
        base._images.uwpSplashScreen = base.CheckImageConsistencyAndGetName(strings);
        string str7 = base.CopyImage(PlayerSettings.WSAImageType.PackageLogo, PlayerSettings.WSAImageScale._125, null, "StoreLogo.scale-125.png");
        string str8 = base.CopyImage(PlayerSettings.WSAImageType.PackageLogo, PlayerSettings.WSAImageScale._150, null, "StoreLogo.scale-150.png");
        string str9 = base.CopyImage(PlayerSettings.WSAImageType.PackageLogo, PlayerSettings.WSAImageScale._200, null, "StoreLogo.scale-200.png");
        string str10 = base.CopyImage(PlayerSettings.WSAImageType.PackageLogo, PlayerSettings.WSAImageScale._400, null, "StoreLogo.scale-400.png");
        string destinationFileName = (((str7 != null) || (str8 != null)) || ((str9 != null) || (str10 != null))) ? "StoreLogo.scale-100.png" : "StoreLogo.png";
        string str12 = base.CopyImage(PlayerSettings.WSAImageType.PackageLogo, PlayerSettings.WSAImageScale._100, Utility.CombinePath(str, "StoreLogo.scale-100.png"), destinationFileName);
        string[] textArray2 = new string[] { str12, str7, str8, str9, str10 };
        base._images.uwpStoreLogo = base.CheckImageConsistencyAndGetName(textArray2);
        string str13 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare44x44Logo, PlayerSettings.WSAImageScale._100, null, "Square44x44Logo.scale-100.png");
        string str14 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare44x44Logo, PlayerSettings.WSAImageScale._125, null, "Square44x44Logo.scale-125.png");
        string str15 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare44x44Logo, PlayerSettings.WSAImageScale._150, null, "Square44x44Logo.scale-150.png");
        string str16 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare44x44Logo, PlayerSettings.WSAImageScale._200, Utility.CombinePath(str, "Square44x44Logo.scale-200.png"), "Square44x44Logo.scale-200.png");
        string str17 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare44x44Logo, PlayerSettings.WSAImageScale._400, null, "Square44x44Logo.scale-400.png");
        string str18 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare44x44Logo, PlayerSettings.WSAImageScale.Target16, null, "Square44x44Logo.targetsize-16_altform-unplated.png");
        string str19 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare44x44Logo, PlayerSettings.WSAImageScale.Target24, Utility.CombinePath(str, "Square44x44Logo.targetsize-24_altform-unplated.png"), "Square44x44Logo.targetsize-24_altform-unplated.png");
        string str20 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare44x44Logo, PlayerSettings.WSAImageScale.Target48, null, "Square44x44Logo.targetsize-48_altform-unplated.png");
        string str21 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare44x44Logo, PlayerSettings.WSAImageScale.Target256, null, "Square44x44Logo.targetsize-256_altform-unplated.png");
        string[] textArray3 = new string[] { str13, str14, str15, str16, str17, str18, str19, str20, str21 };
        base._images.uwpSquare44x44Logo = base.CheckImageConsistencyAndGetName(textArray3);
        string str22 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare71x71Logo, PlayerSettings.WSAImageScale._100, null, "Square71x71Logo.scale-100.png");
        string str23 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare71x71Logo, PlayerSettings.WSAImageScale._125, null, "Square71x71Logo.scale-125.png");
        string str24 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare71x71Logo, PlayerSettings.WSAImageScale._150, null, "Square71x71Logo.scale-150.png");
        string str25 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare71x71Logo, PlayerSettings.WSAImageScale._200, null, "Square71x71Logo.scale-200.png");
        string str26 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare71x71Logo, PlayerSettings.WSAImageScale._400, null, "Square71x71Logo.scale-400.png");
        string[] textArray4 = new string[] { str22, str23, str24, str25, str26 };
        base._images.uwpSquare71x71Logo = base.CheckImageConsistencyAndGetName(textArray4);
        string str27 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare150x150Logo, PlayerSettings.WSAImageScale._100, null, "Square150x150Logo.scale-100.png");
        string str28 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare150x150Logo, PlayerSettings.WSAImageScale._125, null, "Square150x150Logo.scale-125.png");
        string str29 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare150x150Logo, PlayerSettings.WSAImageScale._150, null, "Square150x150Logo.scale-150.png");
        string str30 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare150x150Logo, PlayerSettings.WSAImageScale._200, Utility.CombinePath(str, "Square150x150Logo.scale-200.png"), "Square150x150Logo.scale-200.png");
        string str31 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare150x150Logo, PlayerSettings.WSAImageScale._400, null, "Square150x150Logo.scale-400.png");
        string[] textArray5 = new string[] { str27, str28, str29, str30, str31 };
        base._images.uwpSquare150x150Logo = base.CheckImageConsistencyAndGetName(textArray5);
        string str32 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare310x310Logo, PlayerSettings.WSAImageScale._100, null, "Square310x310Logo.scale-100.png");
        string str33 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare310x310Logo, PlayerSettings.WSAImageScale._125, null, "Square310x310Logo.scale-125.png");
        string str34 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare310x310Logo, PlayerSettings.WSAImageScale._150, null, "Square310x310Logo.scale-150.png");
        string str35 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare310x310Logo, PlayerSettings.WSAImageScale._200, null, "Square310x310Logo.scale-200.png");
        string str36 = base.CopyImage(PlayerSettings.WSAImageType.UWPSquare310x310Logo, PlayerSettings.WSAImageScale._400, null, "Square310x310Logo.scale-400.png");
        string[] textArray6 = new string[] { str32, str33, str34, str35, str36 };
        base._images.uwpSquare310x310Logo = base.CheckImageConsistencyAndGetName(textArray6);
        string str37 = base.CopyImage(PlayerSettings.WSAImageType.UWPWide310x150Logo, PlayerSettings.WSAImageScale._100, null, "Wide310x150Logo.scale-100.png");
        string str38 = base.CopyImage(PlayerSettings.WSAImageType.UWPWide310x150Logo, PlayerSettings.WSAImageScale._125, null, "Wide310x150Logo.scale-125.png");
        string str39 = base.CopyImage(PlayerSettings.WSAImageType.UWPWide310x150Logo, PlayerSettings.WSAImageScale._150, null, "Wide310x150Logo.scale-150.png");
        string str40 = base.CopyImage(PlayerSettings.WSAImageType.UWPWide310x150Logo, PlayerSettings.WSAImageScale._200, Utility.CombinePath(str, "Wide310x150Logo.scale-200.png"), "Wide310x150Logo.scale-200.png");
        string str41 = base.CopyImage(PlayerSettings.WSAImageType.UWPWide310x150Logo, PlayerSettings.WSAImageScale._400, null, "Wide310x150Logo.scale-400.png");
        string[] textArray7 = new string[] { str37, str38, str39, str40, str41 };
        base._images.uwpWide310x150Logo = base.CheckImageConsistencyAndGetName(textArray7);
    }

    protected override ManifestWSA CreateManifestBuilder()
    {
        return new ManifestUAP();
    }

    protected override IEnumerable<string> GetAdditionalReferenceAssembliesDirectories()
    {
        if (<>f__am$cache0 == null)
        {
            <>f__am$cache0 = new Func<string, string>(null, (IntPtr) <GetAdditionalReferenceAssembliesDirectories>m__0);
        }
        List<string> collection = new List<string>(Enumerable.Select<string, string>(UWPReferences, <>f__am$cache0));
        List<string> list2 = new List<string>();
        list2.AddRange(base.GetAdditionalReferenceAssembliesDirectories());
        list2.AddRange(collection);
        return list2;
    }

    protected override IDictionary<PlayerSettings.SplashScreen.UnityLogoStyle, string> GetDefaultSplashScreens()
    {
        if (_defaultSplashScreens == null)
        {
            _defaultSplashScreens = new Dictionary<PlayerSettings.SplashScreen.UnityLogoStyle, string>();
            _defaultSplashScreens[PlayerSettings.SplashScreen.UnityLogoStyle.DarkOnLight] = "SplashScreen.scale-200.png";
            _defaultSplashScreens[PlayerSettings.SplashScreen.UnityLogoStyle.LightOnDark] = "LightSplashScreen.scale-200.png";
        }
        return _defaultSplashScreens;
    }

    protected override string GetPlayerFilesTargetDirectory()
    {
        return base.GetPlayerFilesTargetDirectory(!Utility.UseIl2CppScriptingBackend() ? @"UAP\dotnet" : @"UAP\il2cpp");
    }

    protected override string GetResourceCompilerPath()
    {
        return Path.Combine(MicrosoftCSharpCompiler.GetWindowsKitDirectory(WSASDK.UWP), @"bin\x86\rc.exe");
    }

    protected override string GetSDKNotFoundErrorMessage()
    {
        return "Make sure Visual Studio 2015 is installed.";
    }

    protected override Version GetToolsVersion()
    {
        return new Version(14, 0);
    }

    protected static string[] UWPReferences
    {
        get
        {
            string[] strArray;
            if (_uwpReferences != null)
            {
                strArray = _uwpReferences;
            }
            else
            {
                strArray = _uwpReferences = UnityEditor.Scripting.Compilers.UWPReferences.GetReferences();
            }
            return strArray;
        }
    }
}

