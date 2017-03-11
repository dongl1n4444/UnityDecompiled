namespace UnityEditor.Build
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class BuildPlatform
    {
        public bool forceShowTarget;
        public string name;
        public Texture2D smallIcon;
        public BuildTargetGroup targetGroup;
        public GUIContent title;
        public string tooltip;

        public BuildPlatform(string locTitle, string iconId, BuildTargetGroup targetGroup, bool forceShowTarget) : this(locTitle, "", iconId, targetGroup, forceShowTarget)
        {
        }

        public BuildPlatform(string locTitle, string tooltip, string iconId, BuildTargetGroup targetGroup, bool forceShowTarget)
        {
            this.targetGroup = targetGroup;
            this.name = (targetGroup == BuildTargetGroup.Unknown) ? "" : BuildPipeline.GetBuildTargetGroupName(this.defaultTarget);
            this.title = EditorGUIUtility.TextContentWithIcon(locTitle, iconId);
            this.smallIcon = EditorGUIUtility.IconContent(iconId + ".Small").image as Texture2D;
            this.tooltip = tooltip;
            this.forceShowTarget = forceShowTarget;
        }

        public BuildTarget defaultTarget
        {
            get
            {
                switch (this.targetGroup)
                {
                    case BuildTargetGroup.WebGL:
                        return BuildTarget.WebGL;

                    case BuildTargetGroup.WSA:
                        return BuildTarget.WSAPlayer;

                    case BuildTargetGroup.Tizen:
                        return BuildTarget.Tizen;

                    case BuildTargetGroup.PSP2:
                        return BuildTarget.PSP2;

                    case BuildTargetGroup.PS4:
                        return BuildTarget.PS4;

                    case BuildTargetGroup.XboxOne:
                        return BuildTarget.XboxOne;

                    case BuildTargetGroup.SamsungTV:
                        return BuildTarget.SamsungTV;

                    case BuildTargetGroup.N3DS:
                        return BuildTarget.N3DS;

                    case BuildTargetGroup.WiiU:
                        return BuildTarget.WiiU;

                    case BuildTargetGroup.tvOS:
                        return BuildTarget.tvOS;

                    case BuildTargetGroup.Facebook:
                        return BuildTarget.StandaloneWindows64;

                    case BuildTargetGroup.Switch:
                        return BuildTarget.Switch;

                    case BuildTargetGroup.Standalone:
                        return BuildTarget.StandaloneWindows;

                    case BuildTargetGroup.iPhone:
                        return BuildTarget.iOS;

                    case BuildTargetGroup.Android:
                        return BuildTarget.Android;
                }
                return BuildTarget.iPhone;
            }
        }
    }
}

