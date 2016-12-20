namespace UnityEditor.WebGL
{
    using System;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class WebGlBuildWindowExtension : DefaultBuildWindowExtension
    {
        public GUIContent debugBuild = EditorGUIUtility.TextContent("Development Build");
        public GUIContent webGLUsePreBuiltUnityEngine = EditorGUIUtility.TextContent("Use pre-built Engine");

        public override bool EnabledBuildAndRunButton()
        {
            return this.EnabledBuildButton();
        }

        public override bool EnabledBuildButton()
        {
            return (this.Is64Bit() && !this.IsLinear());
        }

        private bool Is64Bit()
        {
            return (IntPtr.Size == 8);
        }

        private bool IsLinear()
        {
            return (PlayerSettings.colorSpace == ColorSpace.Linear);
        }

        public override bool ShouldDrawDevelopmentPlayerCheckbox()
        {
            return false;
        }

        public override bool ShouldDrawProfilerCheckbox()
        {
            return this.Is64Bit();
        }

        public override bool ShouldDrawScriptDebuggingCheckbox()
        {
            return false;
        }

        public override void ShowPlatformBuildOptions()
        {
            if (!this.Is64Bit())
            {
                GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
                GUILayout.Label("Building for WebGL requires a 64-bit Unity editor.", EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
                GUILayout.EndVertical();
            }
            else if (this.IsLinear())
            {
                GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
                GUILayout.Label("Building for WebGL requires a gamma color space.", EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
                GUILayout.EndVertical();
            }
            else
            {
                EditorUserBuildSettings.development = EditorGUILayout.Toggle(this.debugBuild, EditorUserBuildSettings.development, new GUILayoutOption[0]);
                if (EditorUserBuildSettings.development)
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
                    GUILayout.Label("Note that WebGL development builds are much larger than release builds and should not be published.", EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
                    GUILayout.EndVertical();
                    EditorUserBuildSettings.webGLUsePreBuiltUnityEngine = EditorGUILayout.Toggle(this.webGLUsePreBuiltUnityEngine, EditorUserBuildSettings.webGLUsePreBuiltUnityEngine, new GUILayoutOption[0]);
                    if (EditorUserBuildSettings.webGLUsePreBuiltUnityEngine)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
                        GUILayout.Label("Note that while this option significantly speeds up the build, it always produces unstripped engine code. The 'Strip Engine Code' optimization option will therefore have no effect in this case.", EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
                        GUILayout.EndVertical();
                    }
                }
            }
        }
    }
}

