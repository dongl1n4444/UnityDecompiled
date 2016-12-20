using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Modules;
using UnityEditor.Scripting.Compilers;
using UnityEngine;

internal class PostProcessPhone81 : PostProcessWSA
{
    private static string _platformAssemblyPath;
    private static string _referenceAssembliesDirectory;
    [CompilerGenerated]
    private static Func<string, string> <>f__am$cache0;

    public PostProcessPhone81(BuildPostProcessArgs args, [Optional, DefaultParameterValue(null)] string stagingArea) : base(args, WSASDK.PhoneSDK81, stagingArea)
    {
    }

    public override void CheckSafeProjectOverwrite()
    {
        base.CheckSafeProjectOverwrite();
        string[] paths = new string[] { base.InstallPath, base.VisualStudioName, "Package.appxmanifest" };
        string path = Utility.CombinePath(paths);
        if (File.Exists(path) && !HasManifestPhoneIdentity(path))
        {
            throw new UnityException("Build path contains Windows Store project which is incompatible with current one.");
        }
    }

    public override void CopyImages()
    {
        string str = Utility.CombinePath(base.PlayerPackage, @"Images\Phone");
        string str2 = base.CopyImage(PlayerSettings.WSAImageType.PhoneAppIcon, PlayerSettings.WSAImageScale._100, null, "AppIcon.scale-100.png");
        string str3 = base.CopyImage(PlayerSettings.WSAImageType.PhoneAppIcon, PlayerSettings.WSAImageScale._140, null, "AppIcon.scale-140.png");
        string str4 = base.CopyImage(PlayerSettings.WSAImageType.PhoneAppIcon, PlayerSettings.WSAImageScale._240, Utility.CombinePath(str, "AppIcon.png"), "AppIcon.scale-240.png");
        string[] strings = new string[] { str2, str3, str4 };
        base._images.phoneAppIcon = base.CheckImageConsistencyAndGetName(strings);
        string str5 = base.CopyImage(PlayerSettings.WSAImageType.PhoneSmallTile, PlayerSettings.WSAImageScale._100, null, "SmallTile.scale-100.png");
        string str6 = base.CopyImage(PlayerSettings.WSAImageType.PhoneSmallTile, PlayerSettings.WSAImageScale._140, null, "SmallTile.scale-140.png");
        string str7 = base.CopyImage(PlayerSettings.WSAImageType.PhoneSmallTile, PlayerSettings.WSAImageScale._240, Utility.CombinePath(str, "SmallTile.png"), "SmallTile.scale-240.png");
        string[] textArray2 = new string[] { str5, str6, str7 };
        base._images.phoneSmallTile = base.CheckImageConsistencyAndGetName(textArray2);
        string str8 = base.CopyImage(PlayerSettings.WSAImageType.PhoneMediumTile, PlayerSettings.WSAImageScale._100, null, "MediumTile.scale-100.png");
        string str9 = base.CopyImage(PlayerSettings.WSAImageType.PhoneMediumTile, PlayerSettings.WSAImageScale._140, null, "MediumTile.scale-140.png");
        string str10 = base.CopyImage(PlayerSettings.WSAImageType.PhoneMediumTile, PlayerSettings.WSAImageScale._240, Utility.CombinePath(str, "MediumTile.png"), "MediumTile.scale-240.png");
        string[] textArray3 = new string[] { str8, str9, str10 };
        base._images.phoneMediumTile = base.CheckImageConsistencyAndGetName(textArray3);
        string str11 = base.CopyImage(PlayerSettings.WSAImageType.PhoneWideTile, PlayerSettings.WSAImageScale._100, null, "WideTile.scale-100.png");
        string str12 = base.CopyImage(PlayerSettings.WSAImageType.PhoneWideTile, PlayerSettings.WSAImageScale._140, null, "WideTile.scale-140.png");
        string str13 = base.CopyImage(PlayerSettings.WSAImageType.PhoneWideTile, PlayerSettings.WSAImageScale._240, null, "WideTile.scale-240.png");
        string[] textArray4 = new string[] { str11, str12, str13 };
        base._images.phoneWideTile = base.CheckImageConsistencyAndGetName(textArray4);
        string str14 = base.CopyImage(PlayerSettings.WSAImageType.PhoneSplashScreen, PlayerSettings.WSAImageScale._100, null, "SplashScreen.scale-100.png");
        string str15 = base.CopyImage(PlayerSettings.WSAImageType.PhoneSplashScreen, PlayerSettings.WSAImageScale._140, null, "SplashScreen.scale-140.png");
        string str16 = base.CopyImage(PlayerSettings.WSAImageType.PhoneSplashScreen, PlayerSettings.WSAImageScale._240, Utility.CombinePath(str, base.GetDefaultSplashScreenImage()), "SplashScreen.scale-240.png");
        string[] textArray5 = new string[] { str14, str15, str16 };
        base._images.phoneSplashScreenImage = base.CheckImageConsistencyAndGetName(textArray5);
        string str17 = base.CopyImage(PlayerSettings.WSAImageType.PackageLogo, PlayerSettings.WSAImageScale._100, null, "StoreLogo.scale-100.png");
        string str18 = base.CopyImage(PlayerSettings.WSAImageType.PackageLogo, PlayerSettings.WSAImageScale._140, null, "StoreLogo.scale-140.png");
        string str19 = base.CopyImage(PlayerSettings.WSAImageType.PackageLogo, PlayerSettings.WSAImageScale._240, Utility.CombinePath(str, "StoreLogo.png"), "StoreLogo.scale-240.png");
        string[] textArray6 = new string[] { str17, str18, str19 };
        base._images.phoneStoreLogo = base.CheckImageConsistencyAndGetName(textArray6);
    }

    protected override ManifestWSA CreateManifestBuilder()
    {
        return new ManifestPhone81();
    }

    protected override string GetAssemblyConverterPlatform()
    {
        return "wp81";
    }

    protected override string GetPlayerFilesSourceDirectory()
    {
        return base.GetPlayerFilesSourceDirectory("WindowsPhone81");
    }

    protected override string GetPlayerFilesTargetDirectory()
    {
        return base.GetPlayerFilesTargetDirectory("WindowsPhone81");
    }

    protected override string GetReferenceAssembliesDirectory()
    {
        if (_referenceAssembliesDirectory == null)
        {
            _referenceAssembliesDirectory = PostProcessWSA.GetReferenceAssembliesDirectory(WSASDK.PhoneSDK81);
        }
        return _referenceAssembliesDirectory;
    }

    protected override string GetResourceCompilerPath()
    {
        return Path.Combine(MicrosoftCSharpCompiler.GetWindowsKitDirectory(WSASDK.SDK81), @"bin\x86\rc.exe");
    }

    protected override string GetSDKNotFoundErrorMessage()
    {
        return "Make sure Visual Studio 2013 and Windows Phone SDK 8.1 is installed.";
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
        return new string[] { @"Phone\UnityEngine.dll", @"Phone\WinRTLegacy.dll" };
    }

    protected override IEnumerable<string> GetUnityPluginOverwrites()
    {
        if (<>f__am$cache0 == null)
        {
            <>f__am$cache0 = new Func<string, string>(null, (IntPtr) <GetUnityPluginOverwrites>m__0);
        }
        return Enumerable.Select<string, string>(base.GetUnityPluginOverwrites(), <>f__am$cache0);
    }

    public static bool HasManifestPhoneIdentity(string path)
    {
        try
        {
            XElement element = XDocument.Load(path).Element("{http://schemas.microsoft.com/appx/2010/manifest}Package");
            if (element == null)
            {
                return false;
            }
            return (element.Element("{http://schemas.microsoft.com/appx/2014/phone/manifest}PhoneIdentity") != null);
        }
        catch
        {
            return false;
        }
    }
}

