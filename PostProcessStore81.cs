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

internal class PostProcessStore81 : PostProcessWSA
{
    private static string _platformAssemblyPath;
    private static string _referenceAssembliesDirectory;
    [CompilerGenerated]
    private static Func<string, string> <>f__am$cache0;

    public PostProcessStore81(BuildPostProcessArgs args, [Optional, DefaultParameterValue(null)] string stagingArea) : base(args, WSASDK.SDK81, stagingArea)
    {
    }

    public override void CheckSafeProjectOverwrite()
    {
        base.CheckSafeProjectOverwrite();
        string[] paths = new string[] { base.InstallPath, base.VisualStudioName, "Package.appxmanifest" };
        string path = Utility.CombinePath(paths);
        if (File.Exists(path) && PostProcessPhone81.HasManifestPhoneIdentity(path))
        {
            throw new UnityException("Build path contains Windows Phone project which is incompatible with current one.");
        }
    }

    public override void CopyImages()
    {
        string str = base.CopyImage(PlayerSettings.WSAImageType.StoreSmallTile, PlayerSettings.WSAImageScale._80, null, "SmallTile.scale-80.png");
        string str2 = base.CopyImage(PlayerSettings.WSAImageType.StoreSmallTile, PlayerSettings.WSAImageScale._100, null, "SmallTile.scale-100.png");
        string str3 = base.CopyImage(PlayerSettings.WSAImageType.StoreSmallTile, PlayerSettings.WSAImageScale._140, null, "SmallTile.scale-140.png");
        string str4 = base.CopyImage(PlayerSettings.WSAImageType.StoreSmallTile, PlayerSettings.WSAImageScale._180, null, "SmallTile.scale-180.png");
        string[] strings = new string[] { str, str2, str3, str4 };
        base._images.storeSmallTile = base.CheckImageConsistencyAndGetName(strings);
        string str5 = base.CopyImage(PlayerSettings.WSAImageType.StoreLargeTile, PlayerSettings.WSAImageScale._80, null, "LargeTile.scale-80.png");
        string str6 = base.CopyImage(PlayerSettings.WSAImageType.StoreLargeTile, PlayerSettings.WSAImageScale._100, null, "LargeTile.scale-100.png");
        string str7 = base.CopyImage(PlayerSettings.WSAImageType.StoreLargeTile, PlayerSettings.WSAImageScale._140, null, "LargeTile.scale-140.png");
        string str8 = base.CopyImage(PlayerSettings.WSAImageType.StoreLargeTile, PlayerSettings.WSAImageScale._180, null, "LargeTile.scale-180.png");
        string[] textArray2 = new string[] { str5, str6, str7, str8 };
        base._images.storeLargeTile = base.CheckImageConsistencyAndGetName(textArray2);
        string str9 = Utility.CombinePath(base.PlayerPackage, @"Images\Store");
        string str10 = base.CopyImage(PlayerSettings.WSAImageType.PackageLogo, PlayerSettings.WSAImageScale._100, Utility.CombinePath(str9, "StoreLogo.png"), "StoreLogo.scale-100.png");
        string str11 = base.CopyImage(PlayerSettings.WSAImageType.PackageLogo, PlayerSettings.WSAImageScale._140, null, "StoreLogo.scale-140.png");
        string str12 = base.CopyImage(PlayerSettings.WSAImageType.PackageLogo, PlayerSettings.WSAImageScale._180, null, "StoreLogo.scale-180.png");
        string[] textArray3 = new string[] { str10, str11, str12 };
        base._images.storeStoreLogo = base.CheckImageConsistencyAndGetName(textArray3);
        string str13 = base.CopyImage(PlayerSettings.WSAImageType.StoreTileSmallLogo, PlayerSettings.WSAImageScale._100, Utility.CombinePath(str9, "SmallLogo.png"), "SmallLogo.scale-100.png");
        string str14 = base.CopyImage(PlayerSettings.WSAImageType.StoreTileSmallLogo, PlayerSettings.WSAImageScale._80, null, "SmallLogo.scale-80.png");
        string str15 = base.CopyImage(PlayerSettings.WSAImageType.StoreTileSmallLogo, PlayerSettings.WSAImageScale._140, null, "SmallLogo.scale-140.png");
        string str16 = base.CopyImage(PlayerSettings.WSAImageType.StoreTileSmallLogo, PlayerSettings.WSAImageScale._180, null, "SmallLogo.scale-180.png");
        string str17 = base.CopyImage(PlayerSettings.WSAImageType.StoreTileSmallLogo, PlayerSettings.WSAImageScale.Target16, null, "SmallLogo.target-16.png");
        string str18 = base.CopyImage(PlayerSettings.WSAImageType.StoreTileSmallLogo, PlayerSettings.WSAImageScale.Target32, null, "SmallLogo.target-32.png");
        string str19 = base.CopyImage(PlayerSettings.WSAImageType.StoreTileSmallLogo, PlayerSettings.WSAImageScale.Target48, null, "SmallLogo.target-48.png");
        string str20 = base.CopyImage(PlayerSettings.WSAImageType.StoreTileSmallLogo, PlayerSettings.WSAImageScale.Target256, null, "SmallLogo.target-256.png");
        string[] textArray4 = new string[] { str13, str14, str15, str16, str17, str18, str19, str20 };
        base._images.storeSmallLogo = base.CheckImageConsistencyAndGetName(textArray4);
        string str21 = base.CopyImage(PlayerSettings.WSAImageType.StoreTileLogo, PlayerSettings.WSAImageScale._100, Utility.CombinePath(str9, "MediumTile.png"), "MediumTile.scale-100.png");
        string str22 = base.CopyImage(PlayerSettings.WSAImageType.StoreTileLogo, PlayerSettings.WSAImageScale._80, null, "MediumTile.scale-80.png");
        string str23 = base.CopyImage(PlayerSettings.WSAImageType.StoreTileLogo, PlayerSettings.WSAImageScale._140, null, "MediumTile.scale-140.png");
        string str24 = base.CopyImage(PlayerSettings.WSAImageType.StoreTileLogo, PlayerSettings.WSAImageScale._180, null, "MediumTile.scale-180.png");
        string[] textArray5 = new string[] { str21, str22, str23, str24 };
        base._images.storeTileLogo = base.CheckImageConsistencyAndGetName(textArray5);
        string str25 = base.CopyImage(PlayerSettings.WSAImageType.StoreTileWideLogo, PlayerSettings.WSAImageScale._80, null, "WideTile.scale-80.png");
        string str26 = base.CopyImage(PlayerSettings.WSAImageType.StoreTileWideLogo, PlayerSettings.WSAImageScale._100, Utility.CombinePath(str9, "WideTile.png"), "WideTile.scale-100.png");
        string str27 = base.CopyImage(PlayerSettings.WSAImageType.StoreTileWideLogo, PlayerSettings.WSAImageScale._140, null, "WideTile.scale-140.png");
        string str28 = base.CopyImage(PlayerSettings.WSAImageType.StoreTileWideLogo, PlayerSettings.WSAImageScale._180, null, "WideTile.scale-180.png");
        string[] textArray6 = new string[] { str26, str25, str27, str28 };
        base._images.storeTileWideLogo = base.CheckImageConsistencyAndGetName(textArray6);
        string str29 = base.CopyImage(PlayerSettings.WSAImageType.SplashScreenImage, PlayerSettings.WSAImageScale._100, Utility.CombinePath(str9, base.GetDefaultSplashScreenImage()), "SplashScreen.scale-100.png");
        string str30 = base.CopyImage(PlayerSettings.WSAImageType.SplashScreenImage, PlayerSettings.WSAImageScale._140, null, "SplashScreen.scale-140.png");
        string str31 = base.CopyImage(PlayerSettings.WSAImageType.SplashScreenImage, PlayerSettings.WSAImageScale._180, null, "SplashScreen.scale-180.png");
        string[] textArray7 = new string[] { str29, str30, str31 };
        base._images.storeSplashScreenImage = base.CheckImageConsistencyAndGetName(textArray7);
    }

    protected override ManifestWSA CreateManifestBuilder()
    {
        return new ManifestStore81();
    }

    protected override string GetAssemblyConverterPlatform()
    {
        return "wsa81";
    }

    protected override string GetPlayerFilesSourceDirectory()
    {
        return base.GetPlayerFilesSourceDirectory("Windows81");
    }

    protected override string GetPlayerFilesTargetDirectory()
    {
        return base.GetPlayerFilesTargetDirectory("Windows81");
    }

    protected override string GetReferenceAssembliesDirectory()
    {
        if (_referenceAssembliesDirectory == null)
        {
            _referenceAssembliesDirectory = PostProcessWSA.GetReferenceAssembliesDirectory(WSASDK.SDK81);
        }
        return _referenceAssembliesDirectory;
    }

    protected override string GetResourceCompilerPath()
    {
        return Path.Combine(MicrosoftCSharpCompiler.GetWindowsKitDirectory(WSASDK.SDK81), @"bin\x86\rc.exe");
    }

    protected override string GetSDKNotFoundErrorMessage()
    {
        return "Make sure Visual Studio 2013 is installed.";
    }

    protected override string GetTemplateDirectorySource()
    {
        return base.GetTemplateDirectorySource("Windows81");
    }

    protected override Version GetToolsVersion()
    {
        return new Version(12, 0);
    }

    protected override IEnumerable<string> GetUnityAssemblies()
    {
        return new string[] { @"Store81\UnityEngine.dll", "WinRTLegacy.dll" };
    }

    protected override IEnumerable<string> GetUnityPluginOverwrites()
    {
        if (<>f__am$cache0 == null)
        {
            <>f__am$cache0 = new Func<string, string>(null, (IntPtr) <GetUnityPluginOverwrites>m__0);
        }
        return Enumerable.Select<string, string>(base.GetUnityPluginOverwrites(), <>f__am$cache0);
    }
}

