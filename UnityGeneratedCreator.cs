using System;
using System.Collections.Generic;
using UnityEditor;

internal abstract class UnityGeneratedCreator
{
    private readonly IDictionary<UIOrientation, string> unityOrientationToMs = new Dictionary<UIOrientation, string>();

    protected UnityGeneratedCreator()
    {
    }

    internal abstract string[] Create(string directory);
    protected virtual string GetDefaultFullScreenCode()
    {
        if (EditorUserBuildSettings.wsaSDK == WSASDK.UWP)
        {
            if (PlayerSettings.defaultIsFullScreen)
            {
                return "ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;";
            }
            return "ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;";
        }
        return "";
    }

    protected string GetDisplaySetupCode()
    {
        string initialOrientationsCode = this.GetInitialOrientationsCode();
        string defaultFullScreenCode = this.GetDefaultFullScreenCode();
        if (defaultFullScreenCode != "")
        {
            initialOrientationsCode = initialOrientationsCode + "\r\n" + this.IndentCodeLine() + defaultFullScreenCode;
        }
        return initialOrientationsCode;
    }

    protected virtual string GetInitialOrientationsCode()
    {
        string str;
        if (this.unityOrientationToMs.Count == 0)
        {
            this.unityOrientationToMs[UIOrientation.Portrait] = "DisplayOrientations.Portrait";
            this.unityOrientationToMs[UIOrientation.PortraitUpsideDown] = "DisplayOrientations.PortraitFlipped";
            this.unityOrientationToMs[UIOrientation.LandscapeRight] = "DisplayOrientations.LandscapeFlipped";
            this.unityOrientationToMs[UIOrientation.LandscapeLeft] = "DisplayOrientations.Landscape";
        }
        if (PlayerSettings.defaultInterfaceOrientation == UIOrientation.AutoRotation)
        {
            List<string> list = new List<string>();
            if (PlayerSettings.allowedAutorotateToLandscapeLeft)
            {
                list.Add(this.unityOrientationToMs[UIOrientation.LandscapeLeft]);
            }
            if (PlayerSettings.allowedAutorotateToLandscapeRight)
            {
                list.Add(this.unityOrientationToMs[UIOrientation.LandscapeRight]);
            }
            if (PlayerSettings.allowedAutorotateToPortrait)
            {
                list.Add(this.unityOrientationToMs[UIOrientation.Portrait]);
            }
            if (PlayerSettings.allowedAutorotateToPortraitUpsideDown)
            {
                list.Add(this.unityOrientationToMs[UIOrientation.PortraitUpsideDown]);
            }
            if (list.Count == 0)
            {
                return "";
            }
            str = list[0];
            for (int i = 1; i < list.Count; i++)
            {
                str = str + "|" + list[i];
            }
        }
        else
        {
            str = this.unityOrientationToMs[PlayerSettings.defaultInterfaceOrientation];
        }
        return string.Format("DisplayInformation.AutoRotationPreferences = {0};", str);
    }

    protected abstract string IndentCodeLine();

    internal static UnityGeneratedCreator Cpp
    {
        get
        {
            return new UnityGeneratedCreatorCpp();
        }
    }

    internal static UnityGeneratedCreator CSharp
    {
        get
        {
            return new UnityGeneratedCreatorCSharp();
        }
    }
}

