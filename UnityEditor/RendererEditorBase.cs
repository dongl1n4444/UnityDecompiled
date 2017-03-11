namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Rendering;

    internal class RendererEditorBase : Editor
    {
        protected Probes m_Probes;
        private SerializedProperty m_SortingLayerID;
        private SerializedProperty m_SortingOrder;

        protected void InitializeProbeFields()
        {
            this.m_Probes = new Probes();
            this.m_Probes.Initialize(base.serializedObject);
        }

        public virtual void OnEnable()
        {
            this.m_SortingOrder = base.serializedObject.FindProperty("m_SortingOrder");
            this.m_SortingLayerID = base.serializedObject.FindProperty("m_SortingLayerID");
        }

        protected void RenderCommonProbeFields(bool useMiniStyle)
        {
            bool isDeferredRenderingPath = SceneView.IsUsingDeferredRenderingPath();
            bool isDeferredReflections = isDeferredRenderingPath && (GraphicsSettings.GetShaderMode(BuiltinShaderType.DeferredReflections) != UnityEngine.Rendering.BuiltinShaderMode.Disabled);
            this.m_Probes.RenderReflectionProbeUsage(useMiniStyle, isDeferredRenderingPath, isDeferredReflections);
            this.m_Probes.RenderProbeAnchor(useMiniStyle);
        }

        protected void RenderProbeFields()
        {
            this.m_Probes.OnGUI(base.targets, (Renderer) base.target, false);
        }

        protected void RenderSortingLayerFields()
        {
            EditorGUILayout.Space();
            SortingLayerEditorUtility.RenderSortingLayerFields(this.m_SortingOrder, this.m_SortingLayerID);
        }

        internal class Probes
        {
            [CompilerGenerated]
            private static Func<string, string> <>f__am$cache0;
            [CompilerGenerated]
            private static Func<string, string> <>f__am$cache1;
            [CompilerGenerated]
            private static Func<string, string> <>f__am$cache2;
            [CompilerGenerated]
            private static Func<string, GUIContent> <>f__am$cache3;
            [CompilerGenerated]
            private static Func<string, string> <>f__am$cache4;
            [CompilerGenerated]
            private static Func<string, GUIContent> <>f__am$cache5;
            private List<ReflectionProbeBlendInfo> m_BlendInfo;
            private GUIContent m_DeferredNote = EditorGUIUtility.TextContent("In Deferred Shading, all objects receive shadows and get per-pixel reflection probes.");
            private GUIContent[] m_LightProbeBlendModeOptions;
            private SerializedProperty m_LightProbeUsage;
            private string[] m_LightProbeUsageNames;
            private GUIContent m_LightProbeUsageStyle = EditorGUIUtility.TextContent("Light Probes|Specifies how Light Probes will handle the interpolation of lighting and occlusion. Disabled if the object is set to Lightmap Static.");
            private GUIContent m_LightProbeVolumeNote = EditorGUIUtility.TextContent("A valid Light Probe Proxy Volume component could not be found.");
            private SerializedProperty m_LightProbeVolumeOverride;
            private GUIContent m_LightProbeVolumeOverrideStyle = EditorGUIUtility.TextContent("Proxy Volume Override|If set, the Renderer will use the Light Probe Proxy Volume component from another GameObject.");
            private GUIContent m_LightProbeVolumeUnsupportedNote = EditorGUIUtility.TextContent("The Light Probe Proxy Volume feature is unsupported by the current graphics hardware or API configuration. Simple 'Blend Probes' mode will be used instead.");
            private GUIContent m_LightProbeVolumeUnsupportedOnTreesNote = EditorGUIUtility.TextContent("The Light Probe Proxy Volume feature is not supported on tree rendering. Simple 'Blend Probes' mode will be used instead.");
            private SerializedProperty m_ProbeAnchor;
            private GUIContent m_ProbeAnchorStyle = EditorGUIUtility.TextContent("Anchor Override|Specifies the Transform position that will be used for sampling the light probes and reflection probes.");
            private SerializedProperty m_ReceiveShadows;
            private SerializedProperty m_ReflectionProbeUsage;
            private string[] m_ReflectionProbeUsageNames;
            private GUIContent[] m_ReflectionProbeUsageOptions;
            private GUIContent m_ReflectionProbeUsageStyle = EditorGUIUtility.TextContent("Reflection Probes|Specifies if or how the object is affected by reflections in the Scene.  This property cannot be disabled in deferred rendering modes.");

            public Probes()
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<string, string>(RendererEditorBase.Probes.<m_ReflectionProbeUsageNames>m__0);
                }
                this.m_ReflectionProbeUsageNames = Enumerable.Select<string, string>(Enum.GetNames(typeof(ReflectionProbeUsage)), <>f__am$cache0).ToArray<string>();
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<string, string>(RendererEditorBase.Probes.<m_LightProbeUsageNames>m__1);
                }
                this.m_LightProbeUsageNames = Enumerable.Select<string, string>(Enum.GetNames(typeof(LightProbeUsage)), <>f__am$cache1).ToArray<string>();
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = new Func<string, string>(RendererEditorBase.Probes.<m_ReflectionProbeUsageOptions>m__2);
                }
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = new Func<string, GUIContent>(RendererEditorBase.Probes.<m_ReflectionProbeUsageOptions>m__3);
                }
                this.m_ReflectionProbeUsageOptions = Enumerable.Select<string, GUIContent>(Enumerable.Select<string, string>(Enum.GetNames(typeof(ReflectionProbeUsage)), <>f__am$cache2).ToArray<string>(), <>f__am$cache3).ToArray<GUIContent>();
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = new Func<string, string>(RendererEditorBase.Probes.<m_LightProbeBlendModeOptions>m__4);
                }
                if (<>f__am$cache5 == null)
                {
                    <>f__am$cache5 = new Func<string, GUIContent>(RendererEditorBase.Probes.<m_LightProbeBlendModeOptions>m__5);
                }
                this.m_LightProbeBlendModeOptions = Enumerable.Select<string, GUIContent>(Enumerable.Select<string, string>(Enum.GetNames(typeof(LightProbeUsage)), <>f__am$cache4).ToArray<string>(), <>f__am$cache5).ToArray<GUIContent>();
                this.m_BlendInfo = new List<ReflectionProbeBlendInfo>();
            }

            [CompilerGenerated]
            private static string <m_LightProbeBlendModeOptions>m__4(string x) => 
                ObjectNames.NicifyVariableName(x);

            [CompilerGenerated]
            private static GUIContent <m_LightProbeBlendModeOptions>m__5(string x) => 
                new GUIContent(x);

            [CompilerGenerated]
            private static string <m_LightProbeUsageNames>m__1(string x) => 
                ObjectNames.NicifyVariableName(x);

            [CompilerGenerated]
            private static string <m_ReflectionProbeUsageNames>m__0(string x) => 
                ObjectNames.NicifyVariableName(x);

            [CompilerGenerated]
            private static string <m_ReflectionProbeUsageOptions>m__2(string x) => 
                ObjectNames.NicifyVariableName(x);

            [CompilerGenerated]
            private static GUIContent <m_ReflectionProbeUsageOptions>m__3(string x) => 
                new GUIContent(x);

            internal static string[] GetFieldsStringArray() => 
                new string[] { "m_LightProbeUsage", "m_LightProbeVolumeOverride", "m_ReflectionProbeUsage", "m_ProbeAnchor" };

            internal bool HasValidLightProbeProxyVolumeOverride(Renderer renderer, int selectionCount)
            {
                LightProbeProxyVolume component = renderer.lightProbeProxyVolumeOverride?.GetComponent<LightProbeProxyVolume>();
                return (this.IsUsingLightProbeProxyVolume(selectionCount) && ((component == null) || (component.boundingBoxMode != LightProbeProxyVolume.BoundingBoxMode.AutomaticLocal)));
            }

            internal void Initialize(SerializedObject serializedObject)
            {
                this.m_LightProbeUsage = serializedObject.FindProperty("m_LightProbeUsage");
                this.m_LightProbeVolumeOverride = serializedObject.FindProperty("m_LightProbeVolumeOverride");
                this.m_ReflectionProbeUsage = serializedObject.FindProperty("m_ReflectionProbeUsage");
                this.m_ProbeAnchor = serializedObject.FindProperty("m_ProbeAnchor");
                this.m_ReceiveShadows = serializedObject.FindProperty("m_ReceiveShadows");
            }

            internal bool IsUsingLightProbeProxyVolume(int selectionCount) => 
                (((selectionCount == 1) && (this.m_LightProbeUsage.intValue == 2)) || (((selectionCount > 1) && !this.m_LightProbeUsage.hasMultipleDifferentValues) && (this.m_LightProbeUsage.intValue == 2)));

            internal void OnGUI(UnityEngine.Object[] selection, Renderer renderer, bool useMiniStyle)
            {
                int selectionCount = 1;
                bool isDeferredRenderingPath = SceneView.IsUsingDeferredRenderingPath();
                bool isDeferredReflections = isDeferredRenderingPath && (GraphicsSettings.GetShaderMode(BuiltinShaderType.DeferredReflections) != UnityEngine.Rendering.BuiltinShaderMode.Disabled);
                bool usesLightMaps = false;
                if (selection != null)
                {
                    foreach (UnityEngine.Object obj2 in selection)
                    {
                        if (LightmapEditorSettings.IsLightmappedOrDynamicLightmappedForRendering((Renderer) obj2))
                        {
                            usesLightMaps = true;
                            break;
                        }
                    }
                    selectionCount = selection.Length;
                }
                this.RenderLightProbeUsage(selectionCount, renderer, useMiniStyle, usesLightMaps);
                this.RenderReflectionProbeUsage(useMiniStyle, isDeferredRenderingPath, isDeferredReflections);
                bool flag4 = this.RenderProbeAnchor(useMiniStyle);
                if ((flag4 && (!this.m_ReflectionProbeUsage.hasMultipleDifferentValues && (this.m_ReflectionProbeUsage.intValue != 0))) && !isDeferredReflections)
                {
                    renderer.GetClosestReflectionProbes(this.m_BlendInfo);
                    ShowClosestReflectionProbes(this.m_BlendInfo);
                }
                bool flag6 = !this.m_ReceiveShadows.hasMultipleDifferentValues && this.m_ReceiveShadows.boolValue;
                if ((isDeferredRenderingPath && flag6) || (isDeferredReflections && flag4))
                {
                    EditorGUILayout.HelpBox(this.m_DeferredNote.text, MessageType.Info);
                }
                this.RenderLightProbeProxyVolumeWarningNote(renderer, selectionCount);
            }

            internal void RenderLightProbeProxyVolumeWarningNote(Renderer renderer, int selectionCount)
            {
                if (this.IsUsingLightProbeProxyVolume(selectionCount))
                {
                    if (LightProbeProxyVolume.isFeatureSupported)
                    {
                        LightProbeProxyVolume component = renderer.GetComponent<LightProbeProxyVolume>();
                        bool flag = (renderer.lightProbeProxyVolumeOverride == null) || (renderer.lightProbeProxyVolumeOverride.GetComponent<LightProbeProxyVolume>() == null);
                        if ((component == null) && flag)
                        {
                            EditorGUILayout.HelpBox(this.m_LightProbeVolumeNote.text, MessageType.Warning);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox(this.m_LightProbeVolumeUnsupportedNote.text, MessageType.Warning);
                    }
                }
            }

            internal void RenderLightProbeUsage(int selectionCount, Renderer renderer, bool useMiniStyle, bool usesLightMaps)
            {
                using (new EditorGUI.DisabledScope(usesLightMaps))
                {
                    if (!useMiniStyle)
                    {
                        if (usesLightMaps)
                        {
                            EditorGUILayout.EnumPopup(this.m_LightProbeUsageStyle, LightProbeUsage.Off, new GUILayoutOption[0]);
                        }
                        else
                        {
                            EditorGUILayout.Popup(this.m_LightProbeUsage, this.m_LightProbeBlendModeOptions, this.m_LightProbeUsageStyle, new GUILayoutOption[0]);
                            if (!this.m_LightProbeUsage.hasMultipleDifferentValues && (this.m_LightProbeUsage.intValue == 2))
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(this.m_LightProbeVolumeOverride, this.m_LightProbeVolumeOverrideStyle, new GUILayoutOption[0]);
                                EditorGUI.indentLevel--;
                            }
                        }
                    }
                    else if (usesLightMaps)
                    {
                        ModuleUI.GUIPopup(this.m_LightProbeUsageStyle, 0, this.m_LightProbeUsageNames, new GUILayoutOption[0]);
                    }
                    else
                    {
                        ModuleUI.GUIPopup(this.m_LightProbeUsageStyle, this.m_LightProbeUsage, this.m_LightProbeUsageNames, new GUILayoutOption[0]);
                        if (!this.m_LightProbeUsage.hasMultipleDifferentValues && (this.m_LightProbeUsage.intValue == 2))
                        {
                            EditorGUI.indentLevel++;
                            ModuleUI.GUIObject(this.m_LightProbeVolumeOverrideStyle, this.m_LightProbeVolumeOverride, new GUILayoutOption[0]);
                            EditorGUI.indentLevel--;
                        }
                    }
                }
                if ((renderer.GetComponent<Tree>() != null) && (this.m_LightProbeUsage.intValue == 2))
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.HelpBox(this.m_LightProbeVolumeUnsupportedOnTreesNote.text, MessageType.Warning);
                    EditorGUI.indentLevel--;
                }
            }

            internal bool RenderProbeAnchor(bool useMiniStyle)
            {
                bool flag = !this.m_ReflectionProbeUsage.hasMultipleDifferentValues && (this.m_ReflectionProbeUsage.intValue != 0);
                bool flag2 = !this.m_LightProbeUsage.hasMultipleDifferentValues && (this.m_LightProbeUsage.intValue != 0);
                bool flag3 = flag || flag2;
                if (flag3)
                {
                    if (!useMiniStyle)
                    {
                        EditorGUILayout.PropertyField(this.m_ProbeAnchor, this.m_ProbeAnchorStyle, new GUILayoutOption[0]);
                        return flag3;
                    }
                    ModuleUI.GUIObject(this.m_ProbeAnchorStyle, this.m_ProbeAnchor, new GUILayoutOption[0]);
                }
                return flag3;
            }

            internal void RenderReflectionProbeUsage(bool useMiniStyle, bool isDeferredRenderingPath, bool isDeferredReflections)
            {
                using (new EditorGUI.DisabledScope(isDeferredRenderingPath))
                {
                    if (!useMiniStyle)
                    {
                        if (isDeferredReflections)
                        {
                            EditorGUILayout.EnumPopup(this.m_ReflectionProbeUsageStyle, (this.m_ReflectionProbeUsage.intValue == 0) ? ReflectionProbeUsage.Off : ReflectionProbeUsage.Simple, new GUILayoutOption[0]);
                        }
                        else
                        {
                            EditorGUILayout.Popup(this.m_ReflectionProbeUsage, this.m_ReflectionProbeUsageOptions, this.m_ReflectionProbeUsageStyle, new GUILayoutOption[0]);
                        }
                    }
                    else if (isDeferredReflections)
                    {
                        ModuleUI.GUIPopup(this.m_ReflectionProbeUsageStyle, 3, this.m_ReflectionProbeUsageNames, new GUILayoutOption[0]);
                    }
                    else
                    {
                        ModuleUI.GUIPopup(this.m_ReflectionProbeUsageStyle, this.m_ReflectionProbeUsage, this.m_ReflectionProbeUsageNames, new GUILayoutOption[0]);
                    }
                }
            }

            internal static void ShowClosestReflectionProbes(List<ReflectionProbeBlendInfo> blendInfos)
            {
                float num = 20f;
                float num2 = 70f;
                using (new EditorGUI.DisabledScope(true))
                {
                    for (int i = 0; i < blendInfos.Count; i++)
                    {
                        Rect rect = EditorGUI.IndentedRect(GUILayoutUtility.GetRect((float) 0f, (float) 16f));
                        float num4 = (rect.width - num) - num2;
                        Rect position = rect;
                        position.width = num;
                        GUI.Label(position, "#" + i, EditorStyles.miniLabel);
                        position.x += position.width;
                        position.width = num4;
                        ReflectionProbeBlendInfo info = blendInfos[i];
                        EditorGUI.ObjectField(position, info.probe, typeof(UnityEngine.ReflectionProbe), true);
                        position.x += position.width;
                        position.width = num2;
                        ReflectionProbeBlendInfo info2 = blendInfos[i];
                        GUI.Label(position, "Weight " + info2.weight.ToString("f2"), EditorStyles.miniLabel);
                    }
                }
            }
        }
    }
}

