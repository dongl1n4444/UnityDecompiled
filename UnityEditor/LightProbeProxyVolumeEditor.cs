namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.AnimatedValues;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering;

    [CanEditMultipleObjects, CustomEditor(typeof(LightProbeProxyVolume))]
    internal class LightProbeProxyVolumeEditor : Editor
    {
        internal static Color kGizmoLightProbeProxyVolumeColor = new Color(1f, 0.8980392f, 0.5803922f, 0.5019608f);
        internal static Color kGizmoLightProbeProxyVolumeHandleColor = new Color(1f, 0.8980392f, 0.6666667f, 1f);
        private SerializedProperty m_BoundingBoxMode;
        private SerializedProperty m_BoundingBoxOrigin;
        private SerializedProperty m_BoundingBoxSize;
        private BoxEditor m_BoxEditor = new BoxEditor(true, s_BoxHash);
        private SerializedProperty m_ProbePositionMode;
        private SerializedProperty m_RefreshMode;
        private SerializedProperty m_ResolutionMode;
        private SerializedProperty m_ResolutionProbesPerUnit;
        private SerializedProperty m_ResolutionX;
        private SerializedProperty m_ResolutionY;
        private SerializedProperty m_ResolutionZ;
        private AnimBool m_ShowBoundingBoxOptions = new AnimBool();
        private AnimBool m_ShowComponentUnusedWarning = new AnimBool();
        private AnimBool m_ShowNoLightProbesWarning = new AnimBool();
        private AnimBool m_ShowNoRendererWarning = new AnimBool();
        private AnimBool m_ShowResolutionProbesOption = new AnimBool();
        private AnimBool m_ShowResolutionXYZOptions = new AnimBool();
        private static int s_BoxHash = "LightProbeProxyVolumeEditorHash".GetHashCode();
        private static LightProbeProxyVolumeEditor s_LastInteractedEditor;

        private void DoBoxEditing()
        {
            LightProbeProxyVolume target = (LightProbeProxyVolume) base.target;
            Vector3 sizeCustom = target.sizeCustom;
            Vector3 originCustom = target.originCustom;
            if (this.m_BoxEditor.OnSceneGUI(target.transform.localToWorldMatrix, kGizmoLightProbeProxyVolumeColor, kGizmoLightProbeProxyVolumeHandleColor, true, ref originCustom, ref sizeCustom))
            {
                Undo.RecordObject(target, "Modified Light Probe Proxy Volume AABB");
                Vector3 vector3 = originCustom;
                target.sizeCustom = sizeCustom;
                target.originCustom = vector3;
                EditorUtility.SetDirty(base.target);
            }
        }

        private void DoOriginEditing()
        {
            LightProbeProxyVolume target = (LightProbeProxyVolume) base.target;
            Vector3 position = target.transform.TransformPoint(target.originCustom);
            EditorGUI.BeginChangeCheck();
            Vector3 vector2 = Handles.PositionHandle(position, target.transform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Modified Light Probe Proxy Volume Box Origin");
                target.originCustom = target.transform.InverseTransformPoint(vector2);
                EditorUtility.SetDirty(base.target);
            }
        }

        private void DoToolbar()
        {
            using (new EditorGUI.DisabledScope(this.m_BoundingBoxMode.intValue != 2))
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                UnityEditorInternal.EditMode.SceneViewEditMode editMode = UnityEditorInternal.EditMode.editMode;
                EditorGUI.BeginChangeCheck();
                UnityEditorInternal.EditMode.DoInspectorToolbar(Styles.sceneViewEditModes, Styles.toolContents, this.GetGlobalBounds(), this);
                if (EditorGUI.EndChangeCheck())
                {
                    s_LastInteractedEditor = this;
                }
                if ((editMode != UnityEditorInternal.EditMode.editMode) && (Toolbar.get != null))
                {
                    Toolbar.get.Repaint();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
                string baseSceneEditingToolText = Styles.baseSceneEditingToolText;
                if (this.sceneViewEditing)
                {
                    int index = ArrayUtility.IndexOf<UnityEditorInternal.EditMode.SceneViewEditMode>(Styles.sceneViewEditModes, UnityEditorInternal.EditMode.editMode);
                    if (index >= 0)
                    {
                        baseSceneEditingToolText = Styles.toolNames[index].text;
                    }
                }
                GUILayout.Label(baseSceneEditingToolText, Styles.richTextMiniLabel, new GUILayoutOption[0]);
                GUILayout.EndVertical();
                EditorGUILayout.Space();
            }
        }

        private Bounds GetGlobalBounds()
        {
            if (base.target is LightProbeProxyVolume)
            {
                LightProbeProxyVolume target = (LightProbeProxyVolume) base.target;
                return target.boundsGlobal;
            }
            return new Bounds();
        }

        private bool IsLightProbeVolumeProxyEditMode(UnityEditorInternal.EditMode.SceneViewEditMode editMode) => 
            ((editMode == UnityEditorInternal.EditMode.SceneViewEditMode.LightProbeProxyVolumeBox) || (editMode == UnityEditorInternal.EditMode.SceneViewEditMode.LightProbeProxyVolumeOrigin));

        public void OnDisable()
        {
            this.m_BoxEditor.OnDisable();
        }

        public void OnEnable()
        {
            this.m_ResolutionX = base.serializedObject.FindProperty("m_ResolutionX");
            this.m_ResolutionY = base.serializedObject.FindProperty("m_ResolutionY");
            this.m_ResolutionZ = base.serializedObject.FindProperty("m_ResolutionZ");
            this.m_BoundingBoxSize = base.serializedObject.FindProperty("m_BoundingBoxSize");
            this.m_BoundingBoxOrigin = base.serializedObject.FindProperty("m_BoundingBoxOrigin");
            this.m_BoundingBoxMode = base.serializedObject.FindProperty("m_BoundingBoxMode");
            this.m_ResolutionMode = base.serializedObject.FindProperty("m_ResolutionMode");
            this.m_ResolutionProbesPerUnit = base.serializedObject.FindProperty("m_ResolutionProbesPerUnit");
            this.m_ProbePositionMode = base.serializedObject.FindProperty("m_ProbePositionMode");
            this.m_RefreshMode = base.serializedObject.FindProperty("m_RefreshMode");
            this.m_BoxEditor.OnEnable();
            this.m_BoxEditor.SetAlwaysDisplayHandles(true);
            this.m_BoxEditor.allowNegativeSize = false;
            this.UpdateShowOptions(true);
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            this.UpdateShowOptions(false);
            if (((LightProbeProxyVolume) base.target).GetComponent<Tree>() != null)
            {
                EditorGUILayout.HelpBox(Styles.componentUnsuportedOnTreesNote.text, MessageType.Info);
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.Popup(this.m_RefreshMode, Styles.refreshMode, Styles.refreshModeText, new GUILayoutOption[0]);
                EditorGUILayout.Popup(this.m_BoundingBoxMode, Styles.bbMode, Styles.bbModeText, new GUILayoutOption[0]);
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowBoundingBoxOptions.faded))
                {
                    if (base.targets.Length == 1)
                    {
                        this.DoToolbar();
                    }
                    GUILayout.Label(Styles.bbSettingsText, new GUILayoutOption[0]);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(this.m_BoundingBoxSize, Styles.sizeText, new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_BoundingBoxOrigin, Styles.originText, new GUILayoutOption[0]);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndFadeGroup();
                EditorGUILayout.Space();
                GUILayout.Label(Styles.volumeResolutionText, new GUILayoutOption[0]);
                EditorGUI.indentLevel++;
                EditorGUILayout.Popup(this.m_ResolutionMode, Styles.resMode, Styles.resModeText, new GUILayoutOption[0]);
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowResolutionXYZOptions.faded))
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(40f) };
                    EditorGUILayout.IntPopup(this.m_ResolutionX, Styles.volTextureSizes, Styles.volTextureSizesValues, Styles.resolutionXText, options);
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinWidth(40f) };
                    EditorGUILayout.IntPopup(this.m_ResolutionY, Styles.volTextureSizes, Styles.volTextureSizesValues, Styles.resolutionYText, optionArray2);
                    GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.MinWidth(40f) };
                    EditorGUILayout.IntPopup(this.m_ResolutionZ, Styles.volTextureSizes, Styles.volTextureSizesValues, Styles.resolutionZText, optionArray3);
                }
                EditorGUILayout.EndFadeGroup();
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowResolutionProbesOption.faded))
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    EditorGUILayout.PropertyField(this.m_ResolutionProbesPerUnit, Styles.resProbesPerUnit, new GUILayoutOption[0]);
                    GUILayout.Label(" probes per unit", EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndFadeGroup();
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
                EditorGUILayout.Popup(this.m_ProbePositionMode, Styles.probePositionMode, Styles.probePositionText, new GUILayoutOption[0]);
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowComponentUnusedWarning.faded) && LightProbeProxyVolume.isFeatureSupported)
                {
                    EditorGUILayout.HelpBox(Styles.componentUnusedNote.text, MessageType.Warning);
                }
                EditorGUILayout.EndFadeGroup();
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowNoRendererWarning.faded))
                {
                    EditorGUILayout.HelpBox(Styles.noRendererNode.text, MessageType.Info);
                }
                EditorGUILayout.EndFadeGroup();
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowNoLightProbesWarning.faded))
                {
                    EditorGUILayout.HelpBox(Styles.noLightProbes.text, MessageType.Info);
                }
                EditorGUILayout.EndFadeGroup();
                base.serializedObject.ApplyModifiedProperties();
            }
        }

        public void OnSceneGUI()
        {
            if (this.sceneViewEditing)
            {
                if (this.m_BoundingBoxMode.intValue != 2)
                {
                    UnityEditorInternal.EditMode.QuitEditMode();
                }
                switch (UnityEditorInternal.EditMode.editMode)
                {
                    case UnityEditorInternal.EditMode.SceneViewEditMode.LightProbeProxyVolumeBox:
                        this.DoBoxEditing();
                        break;

                    case UnityEditorInternal.EditMode.SceneViewEditMode.LightProbeProxyVolumeOrigin:
                        this.DoOriginEditing();
                        break;
                }
            }
        }

        [DrawGizmo(GizmoType.Active)]
        private static void RenderBoxGizmo(LightProbeProxyVolume probeProxyVolume, GizmoType gizmoType)
        {
            if ((s_LastInteractedEditor != null) && (s_LastInteractedEditor.sceneViewEditing && (UnityEditorInternal.EditMode.editMode == UnityEditorInternal.EditMode.SceneViewEditMode.LightProbeProxyVolumeBox)))
            {
                Color color = Gizmos.color;
                Gizmos.color = kGizmoLightProbeProxyVolumeColor;
                Vector3 originCustom = probeProxyVolume.originCustom;
                Matrix4x4 matrix = Gizmos.matrix;
                Gizmos.matrix = probeProxyVolume.transform.localToWorldMatrix;
                Gizmos.DrawCube(originCustom, (Vector3) (-1f * probeProxyVolume.sizeCustom));
                Gizmos.matrix = matrix;
                Gizmos.color = color;
            }
        }

        private void SetOptions(AnimBool animBool, bool initialize, bool targetValue)
        {
            if (initialize)
            {
                animBool.value = targetValue;
                animBool.valueChanged.AddListener(new UnityAction(this.Repaint));
            }
            else
            {
                animBool.target = targetValue;
            }
        }

        private void UpdateShowOptions(bool initialize)
        {
            this.SetOptions(this.m_ShowBoundingBoxOptions, initialize, this.boundingBoxOptionsValue);
            this.SetOptions(this.m_ShowComponentUnusedWarning, initialize, this.componentUnusedWarningValue);
            this.SetOptions(this.m_ShowResolutionXYZOptions, initialize, this.resolutionXYZOptionValue);
            this.SetOptions(this.m_ShowResolutionProbesOption, initialize, this.resolutionProbesOptionValue);
            this.SetOptions(this.m_ShowNoRendererWarning, initialize, this.noRendererWarningValue);
            this.SetOptions(this.m_ShowNoLightProbesWarning, initialize, this.noLightProbesWarningValue);
        }

        private bool boundingBoxOptionsValue =>
            (!this.m_BoundingBoxMode.hasMultipleDifferentValues && (this.m_BoundingBoxMode.intValue == 2));

        private bool componentUnusedWarningValue
        {
            get
            {
                Renderer component = ((LightProbeProxyVolume) base.target).GetComponent(typeof(Renderer)) as Renderer;
                bool flag = (component != null) && LightmapEditorSettings.IsLightmappedOrDynamicLightmappedForRendering(component);
                return (((component != null) && (base.targets.Length == 1)) && ((component.lightProbeUsage != LightProbeUsage.UseProxyVolume) || flag));
            }
        }

        private bool noLightProbesWarningValue =>
            ((LightmapSettings.lightProbes == null) || (LightmapSettings.lightProbes.count == 0));

        private bool noRendererWarningValue
        {
            get
            {
                Renderer component = ((LightProbeProxyVolume) base.target).GetComponent(typeof(Renderer)) as Renderer;
                return ((component == null) && (base.targets.Length == 1));
            }
        }

        private bool resolutionProbesOptionValue =>
            (!this.m_ResolutionMode.hasMultipleDifferentValues && (this.m_ResolutionMode.intValue == 0));

        private bool resolutionXYZOptionValue =>
            (!this.m_ResolutionMode.hasMultipleDifferentValues && (this.m_ResolutionMode.intValue == 1));

        private bool sceneViewEditing =>
            (this.IsLightProbeVolumeProxyEditMode(UnityEditorInternal.EditMode.editMode) && UnityEditorInternal.EditMode.IsOwner(this));

        private static class Styles
        {
            public static string baseSceneEditingToolText = "<color=grey>Light Probe Proxy Volume Scene Editing Mode:</color> ";
            public static GUIContent[] bbMode = Enumerable.Select<string, GUIContent>(Enumerable.Select<string, string>(Enum.GetNames(typeof(LightProbeProxyVolume.BoundingBoxMode)), new Func<string, string>(LightProbeProxyVolumeEditor.Styles.<bbMode>m__0)).ToArray<string>(), new Func<string, GUIContent>(LightProbeProxyVolumeEditor.Styles.<bbMode>m__1)).ToArray<GUIContent>();
            public static GUIContent bbModeText = EditorGUIUtility.TextContent("Bounding Box Mode|The mode in which the bounding box is computed. A 3D grid of interpolated light probes will be generated inside this bounding box.\n\nAutomatic Local - the local-space bounding box of the Renderer is used.\n\nAutomatic Global - a bounding box is computed which encloses the current Renderer and all the Renderers down the hierarchy that have the Light Probes property set to Use Proxy Volume. The bounding box will be world-space aligned.\n\nCustom - a custom bounding box is used. The bounding box is specified in the local-space of the game object.");
            public static GUIContent bbSettingsText = EditorGUIUtility.TextContent("Bounding Box Settings");
            public static GUIContent componentUnsuportedOnTreesNote = EditorGUIUtility.TextContent("Tree rendering doesn't support Light Probe Proxy Volume components.");
            public static GUIContent componentUnusedNote = EditorGUIUtility.TextContent("In order to use the component on this game object, the Light Probes property should be set to 'Use Proxy Volume' in Renderer and baked lightmaps should be disabled.");
            public static GUIContent noLightProbes = EditorGUIUtility.TextContent("The scene doesn't contain any light probes. Add light probes using Light Probe Group components (menu: Component->Rendering->Light Probe Group).");
            public static GUIContent noRendererNode = EditorGUIUtility.TextContent("The component is unused by this game object because there is no Renderer component attached.");
            public static GUIContent originText = EditorGUIUtility.TextContent("Origin");
            public static GUIContent[] probePositionMode = Enumerable.Select<string, GUIContent>(Enumerable.Select<string, string>(Enum.GetNames(typeof(LightProbeProxyVolume.ProbePositionMode)), new Func<string, string>(LightProbeProxyVolumeEditor.Styles.<probePositionMode>m__4)).ToArray<string>(), new Func<string, GUIContent>(LightProbeProxyVolumeEditor.Styles.<probePositionMode>m__5)).ToArray<GUIContent>();
            public static GUIContent probePositionText = EditorGUIUtility.TextContent("Probe Position Mode|The mode in which the interpolated probe positions are generated.\n\nCellCorner - divide the volume in cells and generate interpolated probe positions in the corner/edge of the cells.\n\nCellCenter - divide the volume in cells and generate interpolated probe positions in the center of the cells.");
            public static GUIContent[] refreshMode = Enumerable.Select<string, GUIContent>(Enumerable.Select<string, string>(Enum.GetNames(typeof(LightProbeProxyVolume.RefreshMode)), new Func<string, string>(LightProbeProxyVolumeEditor.Styles.<refreshMode>m__6)).ToArray<string>(), new Func<string, GUIContent>(LightProbeProxyVolumeEditor.Styles.<refreshMode>m__7)).ToArray<GUIContent>();
            public static GUIContent refreshModeText = EditorGUIUtility.TextContent("Refresh Mode");
            public static GUIContent[] resMode = Enumerable.Select<string, GUIContent>(Enumerable.Select<string, string>(Enum.GetNames(typeof(LightProbeProxyVolume.ResolutionMode)), new Func<string, string>(LightProbeProxyVolumeEditor.Styles.<resMode>m__2)).ToArray<string>(), new Func<string, GUIContent>(LightProbeProxyVolumeEditor.Styles.<resMode>m__3)).ToArray<GUIContent>();
            public static GUIContent resModeText = EditorGUIUtility.TextContent("Resolution Mode|The mode in which the resolution of the 3D grid of interpolated light probes is specified:\n\nAutomatic - the resolution on each axis is computed using a user-specified number of interpolated light probes per unit area(Density).\n\nCustom - the user can specify a different resolution on each axis.");
            public static GUIContent resolutionXText = new GUIContent("X");
            public static GUIContent resolutionYText = new GUIContent("Y");
            public static GUIContent resolutionZText = new GUIContent("Z");
            public static GUIContent resProbesPerUnit = EditorGUIUtility.TextContent("Density|Density in probes per world unit.");
            public static GUIStyle richTextMiniLabel = new GUIStyle(EditorStyles.miniLabel);
            public static UnityEditorInternal.EditMode.SceneViewEditMode[] sceneViewEditModes = new UnityEditorInternal.EditMode.SceneViewEditMode[] { UnityEditorInternal.EditMode.SceneViewEditMode.LightProbeProxyVolumeBox, UnityEditorInternal.EditMode.SceneViewEditMode.LightProbeProxyVolumeOrigin };
            public static GUIContent sizeText = EditorGUIUtility.TextContent("Size");
            public static GUIContent[] toolContents = new GUIContent[] { EditorGUIUtility.IconContent("EditCollider"), EditorGUIUtility.IconContent("MoveTool", "|Move the selected objects.") };
            public static GUIContent[] toolNames = new GUIContent[] { new GUIContent(baseSceneEditingToolText + "Box Bounds", ""), new GUIContent(baseSceneEditingToolText + "Box Origin", "") };
            public static GUIContent[] volTextureSizes = Enumerable.Select<int, GUIContent>(volTextureSizesValues, new Func<int, GUIContent>(LightProbeProxyVolumeEditor.Styles.<volTextureSizes>m__8)).ToArray<GUIContent>();
            public static int[] volTextureSizesValues = new int[] { 1, 2, 4, 8, 0x10, 0x20 };
            public static GUIContent volumeResolutionText = EditorGUIUtility.TextContent("Proxy Volume Resolution|Specifies the resolution of the 3D grid of interpolated light probes. Higher resolution/density means better lighting but the CPU cost will increase.");

            static Styles()
            {
                richTextMiniLabel.richText = true;
            }

            [CompilerGenerated]
            private static string <bbMode>m__0(string x) => 
                ObjectNames.NicifyVariableName(x);

            [CompilerGenerated]
            private static GUIContent <bbMode>m__1(string x) => 
                new GUIContent(x);

            [CompilerGenerated]
            private static string <probePositionMode>m__4(string x) => 
                ObjectNames.NicifyVariableName(x);

            [CompilerGenerated]
            private static GUIContent <probePositionMode>m__5(string x) => 
                new GUIContent(x);

            [CompilerGenerated]
            private static string <refreshMode>m__6(string x) => 
                ObjectNames.NicifyVariableName(x);

            [CompilerGenerated]
            private static GUIContent <refreshMode>m__7(string x) => 
                new GUIContent(x);

            [CompilerGenerated]
            private static string <resMode>m__2(string x) => 
                ObjectNames.NicifyVariableName(x);

            [CompilerGenerated]
            private static GUIContent <resMode>m__3(string x) => 
                new GUIContent(x);

            [CompilerGenerated]
            private static GUIContent <volTextureSizes>m__8(int n) => 
                new GUIContent(n.ToString());
        }
    }
}

