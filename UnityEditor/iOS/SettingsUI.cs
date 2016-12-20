namespace UnityEditor.iOS
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class SettingsUI
    {
        private static readonly GUIContent[] kArchitectureDescriptions;
        private static readonly Architecture[] kArchitectureOrder;

        static SettingsUI()
        {
            Architecture[] architectureArray1 = new Architecture[3];
            architectureArray1[1] = Architecture.ARM64;
            architectureArray1[2] = Architecture.Universal;
            kArchitectureOrder = architectureArray1;
            kArchitectureDescriptions = new GUIContent[] { EditorGUIUtility.TextContent("ARMv7"), EditorGUIUtility.TextContent("ARM64"), EditorGUIUtility.TextContent("Universal") };
        }

        public static void ShowArchitectureButton(BuildTargetGroup target)
        {
            bool flag;
            if (target == BuildTargetGroup.tvOS)
            {
                flag = PlayerSettings.tvOS.sdkVersion == tvOSSdkVersion.Simulator;
            }
            else
            {
                flag = PlayerSettings.iOS.sdkVersion == iOSSdkVersion.SimulatorSDK;
            }
            int scriptingBackend = (int) PlayerSettings.GetScriptingBackend(target);
            if (!flag)
            {
                int num3;
                int architecture = PlayerSettings.GetArchitecture(target);
                if (scriptingBackend == 1)
                {
                    if (target == BuildTargetGroup.tvOS)
                    {
                        num3 = 1;
                        PlayerSettingsEditor.BuildDisabledEnumPopup(new GUIContent("ARM64"), EditorGUIUtility.TextContent("Architecture"));
                    }
                    else
                    {
                        num3 = PlayerSettingsEditor.BuildEnumPopup<Architecture>(EditorGUIUtility.TextContent("Architecture"), architecture, kArchitectureOrder, kArchitectureDescriptions);
                    }
                }
                else
                {
                    num3 = 0;
                    PlayerSettingsEditor.BuildDisabledEnumPopup(new GUIContent("ARMv7"), EditorGUIUtility.TextContent("Architecture"));
                }
                if (num3 != architecture)
                {
                    PlayerSettings.SetArchitecture(target, num3);
                }
            }
            else if (scriptingBackend == 1)
            {
                PlayerSettingsEditor.BuildDisabledEnumPopup(EditorGUIUtility.TextContent("x86_64"), EditorGUIUtility.TextContent("Architecture"));
            }
            else
            {
                PlayerSettingsEditor.BuildDisabledEnumPopup(EditorGUIUtility.TextContent("i386"), EditorGUIUtility.TextContent("Architecture"));
            }
        }

        public static void Texture2DField(SerializedProperty property, GUIContent content)
        {
            property.objectReferenceValue = EditorGUILayout.ObjectField(content, property.objectReferenceValue, typeof(Texture2D), false, new GUILayoutOption[0]);
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}

