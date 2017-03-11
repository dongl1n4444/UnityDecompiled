namespace UnityEditor.Build
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;

    internal class BuildPlatforms
    {
        public BuildPlatform[] buildPlatforms;
        private static readonly BuildPlatforms s_Instance = new BuildPlatforms();

        internal BuildPlatforms()
        {
            List<BuildPlatform> list = new List<BuildPlatform> {
                new BuildPlatform("PC, Mac & Linux Standalone", "BuildSettings.Standalone", BuildTargetGroup.Standalone, true),
                new BuildPlatform("iOS", "BuildSettings.iPhone", BuildTargetGroup.iPhone, true),
                new BuildPlatform("tvOS", "BuildSettings.tvOS", BuildTargetGroup.tvOS, true),
                new BuildPlatform("Android", "BuildSettings.Android", BuildTargetGroup.Android, true),
                new BuildPlatform("Tizen", "BuildSettings.Tizen", BuildTargetGroup.Tizen, true),
                new BuildPlatform("Xbox One", "BuildSettings.XboxOne", BuildTargetGroup.XboxOne, true),
                new BuildPlatform("PS Vita", "BuildSettings.PSP2", BuildTargetGroup.PSP2, true),
                new BuildPlatform("PS4", "BuildSettings.PS4", BuildTargetGroup.PS4, true),
                new BuildPlatform("Wii U", "BuildSettings.WiiU", BuildTargetGroup.WiiU, false),
                new BuildPlatform("Windows Store", "BuildSettings.Metro", BuildTargetGroup.WSA, true),
                new BuildPlatform("WebGL", "BuildSettings.WebGL", BuildTargetGroup.WebGL, true),
                new BuildPlatform("Samsung TV", "BuildSettings.SamsungTV", BuildTargetGroup.SamsungTV, true),
                new BuildPlatform("Nintendo 3DS", "BuildSettings.N3DS", BuildTargetGroup.N3DS, false),
                new BuildPlatform("Facebook", "BuildSettings.Facebook", BuildTargetGroup.Facebook, true),
                new BuildPlatform("Nintendo Switch", "BuildSettings.Switch", BuildTargetGroup.Switch, false)
            };
            foreach (BuildPlatform platform in list)
            {
                platform.tooltip = BuildPipeline.GetBuildTargetGroupDisplayName(platform.targetGroup) + " settings";
            }
            this.buildPlatforms = list.ToArray();
        }

        public BuildPlatform BuildPlatformFromTargetGroup(BuildTargetGroup group)
        {
            int index = this.BuildPlatformIndexFromTargetGroup(group);
            return ((index == -1) ? null : this.buildPlatforms[index]);
        }

        public int BuildPlatformIndexFromTargetGroup(BuildTargetGroup group)
        {
            for (int i = 0; i < this.buildPlatforms.Length; i++)
            {
                if (group == this.buildPlatforms[i].targetGroup)
                {
                    return i;
                }
            }
            return -1;
        }

        public string GetBuildTargetDisplayName(BuildTarget target)
        {
            foreach (BuildPlatform platform in this.buildPlatforms)
            {
                if (platform.defaultTarget == target)
                {
                    return platform.title.text;
                }
            }
            switch (target)
            {
                case BuildTarget.StandaloneOSXUniversal:
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                    return "Mac OS X";

                case BuildTarget.StandaloneWindows:
                    break;

                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    goto Label_00A1;

                default:
                    switch (target)
                    {
                        case BuildTarget.StandaloneLinux:
                            goto Label_00A1;

                        case ((BuildTarget) 0x12):
                            goto Label_00AC;

                        case BuildTarget.StandaloneWindows64:
                            break;

                        default:
                            goto Label_00AC;
                    }
                    break;
            }
            return "Windows";
        Label_00A1:
            return "Linux";
        Label_00AC:
            return "Unsupported Target";
        }

        public string GetModuleDisplayName(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
        {
            if (buildTargetGroup == BuildTargetGroup.Facebook)
            {
                return BuildPipeline.GetBuildTargetGroupDisplayName(buildTargetGroup);
            }
            return this.GetBuildTargetDisplayName(buildTarget);
        }

        public List<BuildPlatform> GetValidPlatforms() => 
            this.GetValidPlatforms(false);

        public List<BuildPlatform> GetValidPlatforms(bool includeMetaPlatforms)
        {
            List<BuildPlatform> list = new List<BuildPlatform>();
            foreach (BuildPlatform platform in this.buildPlatforms)
            {
                if (((platform.targetGroup == BuildTargetGroup.Standalone) || BuildPipeline.IsBuildTargetSupported(platform.targetGroup, platform.defaultTarget)) && ((platform.targetGroup != BuildTargetGroup.Facebook) || includeMetaPlatforms))
                {
                    list.Add(platform);
                }
            }
            return list;
        }

        public static BuildPlatforms instance =>
            s_Instance;
    }
}

