using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Modules;
using UnityEngine;

internal class DesktopStandaloneBuildWindowExtension : DefaultBuildWindowExtension
{
    private GUIContent m_Architecture = EditorGUIUtility.TextContent("Architecture|Build m_Architecture for standalone");
    private BuildTarget[] m_StandaloneSubtargets;
    private GUIContent[] m_StandaloneSubtargetStrings;
    private GUIContent m_StandaloneTarget = EditorGUIUtility.TextContent("Target Platform|Destination platform for standalone build");

    public DesktopStandaloneBuildWindowExtension()
    {
        this.SetupStandaloneSubtargets();
    }

    private static BuildTarget DefaultTargetForPlatform(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.StandaloneOSXUniversal:
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
                return BuildTarget.StandaloneOSXIntel;

            case BuildTarget.StandaloneWindows:
                break;

            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal:
                goto Label_0053;

            default:
                switch (target)
                {
                    case BuildTarget.StandaloneLinux:
                        goto Label_0053;

                    case ((BuildTarget) 0x12):
                        return target;

                    case BuildTarget.StandaloneWindows64:
                        break;

                    default:
                        return target;
                }
                break;
        }
        return BuildTarget.StandaloneWindows;
    Label_0053:
        return BuildTarget.StandaloneLinux;
    }

    public override bool EnabledBuildAndRunButton() => 
        true;

    public override bool EnabledBuildButton() => 
        true;

    private static Dictionary<GUIContent, BuildTarget> GetArchitecturesForPlatform(BuildTarget target)
    {
        Dictionary<GUIContent, BuildTarget> dictionary;
        switch (target)
        {
            case BuildTarget.StandaloneOSXUniversal:
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
                return new Dictionary<GUIContent, BuildTarget> { 
                    { 
                        EditorGUIUtility.TextContent("x86"),
                        BuildTarget.StandaloneOSXIntel
                    },
                    { 
                        EditorGUIUtility.TextContent("x86_64"),
                        BuildTarget.StandaloneOSXIntel64
                    },
                    { 
                        EditorGUIUtility.TextContent("Universal"),
                        BuildTarget.StandaloneOSXUniversal
                    }
                };

            case BuildTarget.StandaloneWindows:
                break;

            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal:
                goto Label_007C;

            default:
                switch (target)
                {
                    case BuildTarget.StandaloneLinux:
                        goto Label_007C;

                    case ((BuildTarget) 0x12):
                        goto Label_0100;

                    case BuildTarget.StandaloneWindows64:
                        break;

                    default:
                        goto Label_0100;
                }
                break;
        }
        return new Dictionary<GUIContent, BuildTarget> { 
            { 
                EditorGUIUtility.TextContent("x86"),
                BuildTarget.StandaloneWindows
            },
            { 
                EditorGUIUtility.TextContent("x86_64"),
                BuildTarget.StandaloneWindows64
            }
        };
    Label_007C:
        dictionary = new Dictionary<GUIContent, BuildTarget>();
        dictionary.Add(EditorGUIUtility.TextContent("x86"), BuildTarget.StandaloneLinux);
        dictionary.Add(EditorGUIUtility.TextContent("x86_64"), BuildTarget.StandaloneLinux64);
        dictionary.Add(EditorGUIUtility.TextContent("x86 + x86_64 (Universal)"), BuildTarget.StandaloneLinuxUniversal);
        return dictionary;
    Label_0100:
        return null;
    }

    private static BuildTarget GetBestStandaloneTarget(BuildTarget selectedTarget)
    {
        if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(selectedTarget)))
        {
            return selectedTarget;
        }
        if ((Application.platform != RuntimePlatform.WindowsEditor) || !ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneWindows)))
        {
            if ((Application.platform == RuntimePlatform.OSXEditor) && ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneOSXIntel)))
            {
                return BuildTarget.StandaloneOSXIntel;
            }
            if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneOSXIntel)))
            {
                return BuildTarget.StandaloneOSXIntel;
            }
            if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneLinux)))
            {
                return BuildTarget.StandaloneLinux;
            }
        }
        return BuildTarget.StandaloneWindows;
    }

    private void SetupStandaloneSubtargets()
    {
        List<BuildTarget> list = new List<BuildTarget>();
        List<GUIContent> list2 = new List<GUIContent>();
        if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneWindows)))
        {
            list.Add(BuildTarget.StandaloneWindows);
            list2.Add(EditorGUIUtility.TextContent("Windows"));
        }
        if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneOSXIntel)))
        {
            list.Add(BuildTarget.StandaloneOSXIntel);
            list2.Add(EditorGUIUtility.TextContent("Mac OS X"));
        }
        if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneLinux)))
        {
            list.Add(BuildTarget.StandaloneLinux);
            list2.Add(EditorGUIUtility.TextContent("Linux"));
        }
        this.m_StandaloneSubtargets = list.ToArray();
        this.m_StandaloneSubtargetStrings = list2.ToArray();
    }

    public override void ShowPlatformBuildOptions()
    {
        BuildTarget bestStandaloneTarget = GetBestStandaloneTarget(EditorUserBuildSettings.selectedStandaloneTarget);
        BuildTarget selectedStandaloneTarget = EditorUserBuildSettings.selectedStandaloneTarget;
        int selectedIndex = Math.Max(0, Array.IndexOf<BuildTarget>(this.m_StandaloneSubtargets, DefaultTargetForPlatform(bestStandaloneTarget)));
        int index = EditorGUILayout.Popup(this.m_StandaloneTarget, selectedIndex, this.m_StandaloneSubtargetStrings, new GUILayoutOption[0]);
        if (index != selectedIndex)
        {
            selectedStandaloneTarget = this.m_StandaloneSubtargets[index];
        }
        else
        {
            Dictionary<GUIContent, BuildTarget> architecturesForPlatform = GetArchitecturesForPlatform(bestStandaloneTarget);
            if (architecturesForPlatform != null)
            {
                GUIContent[] array = new List<GUIContent>(architecturesForPlatform.Keys).ToArray();
                int num3 = 0;
                if (index == selectedIndex)
                {
                    foreach (KeyValuePair<GUIContent, BuildTarget> pair in architecturesForPlatform)
                    {
                        if (((BuildTarget) pair.Value) == bestStandaloneTarget)
                        {
                            num3 = Math.Max(0, Array.IndexOf<GUIContent>(array, pair.Key));
                            break;
                        }
                    }
                }
                num3 = EditorGUILayout.Popup(this.m_Architecture, num3, array, new GUILayoutOption[0]);
                selectedStandaloneTarget = architecturesForPlatform[array[num3]];
            }
        }
        if (selectedStandaloneTarget != EditorUserBuildSettings.selectedStandaloneTarget)
        {
            EditorUserBuildSettings.selectedStandaloneTarget = selectedStandaloneTarget;
            GUIUtility.ExitGUI();
        }
    }
}

