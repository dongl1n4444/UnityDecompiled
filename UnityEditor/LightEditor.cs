namespace UnityEditor
{
    using System;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering;

    [CustomEditor(typeof(Light)), CanEditMultipleObjects]
    internal class LightEditor : Editor
    {
        internal static Color kGizmoDisabledLight = new Color(0.5294118f, 0.454902f, 0.1960784f, 0.5019608f);
        internal static Color kGizmoLight = new Color(0.9960784f, 0.9921569f, 0.5333334f, 0.5019608f);
        private SerializedProperty m_AreaSizeX;
        private SerializedProperty m_AreaSizeY;
        private SerializedProperty m_BakedShadowAngle;
        private AnimBool m_BakedShadowAngleOptions = new AnimBool();
        private SerializedProperty m_BakedShadowRadius;
        private AnimBool m_BakedShadowRadiusOptions = new AnimBool();
        private SerializedProperty m_BounceIntensity;
        private SerializedProperty m_Color;
        private bool m_CommandBuffersShown = true;
        private SerializedProperty m_Cookie;
        private SerializedProperty m_CookieSize;
        private SerializedProperty m_CullingMask;
        private SerializedProperty m_Flare;
        private SerializedProperty m_Halo;
        private SerializedProperty m_Intensity;
        private SerializedProperty m_Lightmapping;
        private SerializedProperty m_Range;
        private SerializedProperty m_RenderMode;
        private SerializedProperty m_ShadowsBias;
        private SerializedProperty m_ShadowsNearPlane;
        private SerializedProperty m_ShadowsNormalBias;
        private SerializedProperty m_ShadowsResolution;
        private SerializedProperty m_ShadowsStrength;
        private SerializedProperty m_ShadowsType;
        private AnimBool m_ShowAreaOptions = new AnimBool();
        private AnimBool m_ShowBakingWarning = new AnimBool();
        private AnimBool m_ShowCookieWarning = new AnimBool();
        private AnimBool m_ShowDirOptions = new AnimBool();
        private AnimBool m_ShowIndirectWarning = new AnimBool();
        private AnimBool m_ShowPointOptions = new AnimBool();
        private AnimBool m_ShowRuntimeOptions = new AnimBool();
        private AnimBool m_ShowShadowOptions = new AnimBool();
        private AnimBool m_ShowSpotOptions = new AnimBool();
        private SerializedProperty m_SpotAngle;
        private SerializedProperty m_Type;
        private static Styles s_Styles;

        private void CommandBufferGUI()
        {
            if (base.targets.Length == 1)
            {
                Light target = base.target as Light;
                if (target != null)
                {
                    int commandBufferCount = target.commandBufferCount;
                    if (commandBufferCount != 0)
                    {
                        this.m_CommandBuffersShown = GUILayout.Toggle(this.m_CommandBuffersShown, GUIContent.Temp(commandBufferCount + " command buffers"), EditorStyles.foldout, new GUILayoutOption[0]);
                        if (this.m_CommandBuffersShown)
                        {
                            EditorGUI.indentLevel++;
                            foreach (LightEvent event2 in (LightEvent[]) Enum.GetValues(typeof(LightEvent)))
                            {
                                CommandBuffer[] commandBuffers = target.GetCommandBuffers(event2);
                                foreach (CommandBuffer buffer in commandBuffers)
                                {
                                    using (new GUILayout.HorizontalScope(new GUILayoutOption[0]))
                                    {
                                        Rect r = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.miniLabel);
                                        r.xMin += EditorGUI.indent;
                                        Rect removeButtonRect = GetRemoveButtonRect(r);
                                        r.xMax = removeButtonRect.x;
                                        GUI.Label(r, $"{event2}: {buffer.name} ({EditorUtility.FormatBytes(buffer.sizeInBytes)})", EditorStyles.miniLabel);
                                        if (GUI.Button(removeButtonRect, s_Styles.iconRemove, s_Styles.invisibleButton))
                                        {
                                            target.RemoveCommandBuffer(event2, buffer);
                                            SceneView.RepaintAll();
                                            GameView.RepaintAll();
                                            GUIUtility.ExitGUI();
                                        }
                                    }
                                }
                            }
                            using (new GUILayout.HorizontalScope(new GUILayoutOption[0]))
                            {
                                GUILayout.FlexibleSpace();
                                if (GUILayout.Button("Remove all", EditorStyles.miniButton, new GUILayoutOption[0]))
                                {
                                    target.RemoveAllCommandBuffers();
                                    SceneView.RepaintAll();
                                    GameView.RepaintAll();
                                }
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                }
            }
        }

        private static Rect GetRemoveButtonRect(Rect r)
        {
            Vector2 vector = s_Styles.invisibleButton.CalcSize(s_Styles.iconRemove);
            return new Rect(r.xMax - vector.x, r.y + ((int) ((r.height / 2f) - (vector.y / 2f))), vector.x, vector.y);
        }

        private void OnEnable()
        {
            this.m_Type = base.serializedObject.FindProperty("m_Type");
            this.m_Range = base.serializedObject.FindProperty("m_Range");
            this.m_SpotAngle = base.serializedObject.FindProperty("m_SpotAngle");
            this.m_CookieSize = base.serializedObject.FindProperty("m_CookieSize");
            this.m_Color = base.serializedObject.FindProperty("m_Color");
            this.m_Intensity = base.serializedObject.FindProperty("m_Intensity");
            this.m_BounceIntensity = base.serializedObject.FindProperty("m_BounceIntensity");
            this.m_Cookie = base.serializedObject.FindProperty("m_Cookie");
            this.m_ShadowsType = base.serializedObject.FindProperty("m_Shadows.m_Type");
            this.m_ShadowsStrength = base.serializedObject.FindProperty("m_Shadows.m_Strength");
            this.m_ShadowsResolution = base.serializedObject.FindProperty("m_Shadows.m_Resolution");
            this.m_ShadowsBias = base.serializedObject.FindProperty("m_Shadows.m_Bias");
            this.m_ShadowsNormalBias = base.serializedObject.FindProperty("m_Shadows.m_NormalBias");
            this.m_ShadowsNearPlane = base.serializedObject.FindProperty("m_Shadows.m_NearPlane");
            this.m_Halo = base.serializedObject.FindProperty("m_DrawHalo");
            this.m_Flare = base.serializedObject.FindProperty("m_Flare");
            this.m_RenderMode = base.serializedObject.FindProperty("m_RenderMode");
            this.m_CullingMask = base.serializedObject.FindProperty("m_CullingMask");
            this.m_Lightmapping = base.serializedObject.FindProperty("m_Lightmapping");
            this.m_AreaSizeX = base.serializedObject.FindProperty("m_AreaSize.x");
            this.m_AreaSizeY = base.serializedObject.FindProperty("m_AreaSize.y");
            this.m_BakedShadowRadius = base.serializedObject.FindProperty("m_ShadowRadius");
            this.m_BakedShadowAngle = base.serializedObject.FindProperty("m_ShadowAngle");
            this.UpdateShowOptions(true);
        }

        public override void OnInspectorGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            base.serializedObject.Update();
            this.UpdateShowOptions(false);
            EditorGUILayout.PropertyField(this.m_Type, new GUILayoutOption[0]);
            if (EditorGUILayout.BeginFadeGroup(1f - this.m_ShowAreaOptions.faded))
            {
                EditorGUILayout.IntPopup(this.m_Lightmapping, s_Styles.LightmappingModes, s_Styles.LightmappingModeValues, s_Styles.LightmappingModeLabel, new GUILayoutOption[0]);
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowBakingWarning.faded))
                {
                    EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("Enable Baked GI from Lighting window to use Baked or Mixed.").text, MessageType.Warning, false);
                }
                EditorGUILayout.EndFadeGroup();
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();
            float num = 1f - this.m_ShowDirOptions.faded;
            if (EditorGUILayout.BeginFadeGroup(num))
            {
                if (this.m_ShowAreaOptions.target)
                {
                    GUI.enabled = false;
                    string tooltip = "For area lights " + this.m_Range.displayName + " is computed from Width, Height and Intensity";
                    GUIContent label = new GUIContent(this.m_Range.displayName, tooltip);
                    EditorGUILayout.FloatField(label, this.light.range, new GUILayoutOption[0]);
                    GUI.enabled = true;
                }
                else
                {
                    EditorGUILayout.PropertyField(this.m_Range, new GUILayoutOption[0]);
                }
            }
            EditorGUILayout.EndFadeGroup();
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowSpotOptions.faded))
            {
                EditorGUILayout.Slider(this.m_SpotAngle, 1f, 179f, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowAreaOptions.faded))
            {
                EditorGUILayout.PropertyField(this.m_AreaSizeX, EditorGUIUtility.TextContent("Width"), new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_AreaSizeY, EditorGUIUtility.TextContent("Height"), new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.PropertyField(this.m_Color, new GUILayoutOption[0]);
            EditorGUILayout.Slider(this.m_Intensity, 0f, 8f, new GUILayoutOption[0]);
            EditorGUILayout.Slider(this.m_BounceIntensity, 0f, 8f, s_Styles.LightBounceIntensity, new GUILayoutOption[0]);
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowIndirectWarning.faded))
            {
                EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("Currently realtime indirect bounce light shadowing for spot and point lights is not supported.").text, MessageType.Warning, false);
            }
            EditorGUILayout.EndFadeGroup();
            this.ShadowsGUI();
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowRuntimeOptions.faded))
            {
                EditorGUILayout.PropertyField(this.m_Cookie, new GUILayoutOption[0]);
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowCookieWarning.faded))
                {
                    EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("Cookie textures for spot lights should be set to clamp, not repeat, to avoid artifacts.").text, MessageType.Warning, false);
                }
            }
            EditorGUILayout.EndFadeGroup();
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowRuntimeOptions.faded * this.m_ShowDirOptions.faded))
            {
                EditorGUILayout.PropertyField(this.m_CookieSize, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.PropertyField(this.m_Halo, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Flare, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_RenderMode, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_CullingMask, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            if ((SceneView.currentDrawingSceneView != null) && !SceneView.currentDrawingSceneView.m_SceneLighting)
            {
                EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("One of your scene views has lighting disabled, please keep this in mind when editing lighting.").text, MessageType.Warning, false);
            }
            this.CommandBufferGUI();
            base.serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            Light target = (Light) base.target;
            Color color = Handles.color;
            if (target.enabled)
            {
                Handles.color = kGizmoLight;
            }
            else
            {
                Handles.color = kGizmoDisabledLight;
            }
            float range = target.range;
            switch (target.type)
            {
                case LightType.Spot:
                {
                    Color color2 = Handles.color;
                    color2.a = Mathf.Clamp01(color.a * 2f);
                    Handles.color = color2;
                    Vector2 angleAndRange = new Vector2(target.spotAngle, target.range);
                    angleAndRange = Handles.ConeHandle(target.transform.rotation, target.transform.position, angleAndRange, 1f, 1f, true);
                    if (GUI.changed)
                    {
                        Undo.RecordObject(target, "Adjust Spot Light");
                        target.spotAngle = angleAndRange.x;
                        target.range = Mathf.Max(angleAndRange.y, 0.01f);
                    }
                    break;
                }
                case LightType.Point:
                    range = Handles.RadiusHandle(Quaternion.identity, target.transform.position, range, true);
                    if (GUI.changed)
                    {
                        Undo.RecordObject(target, "Adjust Point Light");
                        target.range = range;
                    }
                    break;

                case LightType.Area:
                {
                    EditorGUI.BeginChangeCheck();
                    Vector2 vector2 = Handles.DoRectHandles(target.transform.rotation, target.transform.position, target.areaSize);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Adjust Area Light");
                        target.areaSize = vector2;
                    }
                    break;
                }
            }
            Handles.color = color;
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

        private void ShadowsGUI()
        {
            float num = 1f - this.m_ShowAreaOptions.faded;
            if (EditorGUILayout.BeginFadeGroup(num))
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(this.m_ShadowsType, s_Styles.ShadowType, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUI.indentLevel++;
            num *= this.m_ShowShadowOptions.faded;
            if (EditorGUILayout.BeginFadeGroup(num * this.m_ShowRuntimeOptions.faded))
            {
                if (this.m_Lightmapping.intValue == 1)
                {
                    string[] strArray = new string[] { "No shadows from static objects onto dynamic objects and vice versa.", "No shadows from static objects onto dynamic objects, the main light casts shadows from dynamic objects onto static objects." };
                    EditorGUILayout.HelpBox(strArray[(this.m_Type.intValue == 1) ? 1 : 0], MessageType.Warning, false);
                }
                EditorGUILayout.Slider(this.m_ShadowsStrength, 0f, 1f, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_ShadowsResolution, new GUILayoutOption[0]);
                EditorGUILayout.Slider(this.m_ShadowsBias, 0f, 2f, new GUILayoutOption[0]);
                EditorGUILayout.Slider(this.m_ShadowsNormalBias, 0f, 3f, new GUILayoutOption[0]);
                float leftValue = Mathf.Min((float) (0.01f * this.m_Range.floatValue), (float) 0.1f);
                EditorGUILayout.Slider(this.m_ShadowsNearPlane, leftValue, 10f, s_Styles.ShadowNearPlane, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            if (EditorGUILayout.BeginFadeGroup(num * this.m_BakedShadowRadiusOptions.faded))
            {
                using (new EditorGUI.DisabledScope(this.m_ShadowsType.intValue != 2))
                {
                    EditorGUILayout.PropertyField(this.m_BakedShadowRadius, s_Styles.BakedShadowRadius, new GUILayoutOption[0]);
                }
            }
            EditorGUILayout.EndFadeGroup();
            if (EditorGUILayout.BeginFadeGroup(num * this.m_BakedShadowAngleOptions.faded))
            {
                using (new EditorGUI.DisabledScope(this.m_ShadowsType.intValue != 2))
                {
                    EditorGUILayout.Slider(this.m_BakedShadowAngle, 0f, 90f, s_Styles.BakedShadowAngle, new GUILayoutOption[0]);
                }
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        private void UpdateShowOptions(bool initialize)
        {
            this.SetOptions(this.m_ShowSpotOptions, initialize, this.spotOptionsValue);
            this.SetOptions(this.m_ShowPointOptions, initialize, this.pointOptionsValue);
            this.SetOptions(this.m_ShowDirOptions, initialize, this.dirOptionsValue);
            this.SetOptions(this.m_ShowAreaOptions, initialize, this.areaOptionsValue);
            this.SetOptions(this.m_ShowShadowOptions, initialize, this.shadowOptionsValue);
            this.SetOptions(this.m_ShowIndirectWarning, initialize, this.bounceWarningValue);
            this.SetOptions(this.m_ShowBakingWarning, initialize, this.bakingWarningValue);
            this.SetOptions(this.m_ShowCookieWarning, initialize, this.cookieWarningValue);
            this.SetOptions(this.m_ShowRuntimeOptions, initialize, this.runtimeOptionsValue);
            this.SetOptions(this.m_BakedShadowAngleOptions, initialize, this.bakedShadowAngle);
            this.SetOptions(this.m_BakedShadowRadiusOptions, initialize, this.bakedShadowRadius);
        }

        private bool areaOptionsValue =>
            (this.typeIsSame && (this.light.type == LightType.Area));

        private bool areaWarningValue =>
            (this.typeIsSame && (this.light.type == LightType.Area));

        private bool bakedShadowAngle =>
            ((this.typeIsSame && (this.light.type == LightType.Directional)) && this.isBakedOrMixed);

        private bool bakedShadowRadius =>
            ((this.typeIsSame && ((this.light.type == LightType.Point) || (this.light.type == LightType.Spot))) && this.isBakedOrMixed);

        private bool bakingWarningValue =>
            ((!Lightmapping.bakedGI && this.lightmappingTypeIsSame) && this.isBakedOrMixed);

        private bool bounceWarningValue =>
            (((this.typeIsSame && ((this.light.type == LightType.Point) || (this.light.type == LightType.Spot))) && ((this.lightmappingTypeIsSame && this.isRealtime) && !this.m_BounceIntensity.hasMultipleDifferentValues)) && (this.m_BounceIntensity.floatValue > 0f));

        private Texture cookie =>
            (this.m_Cookie.objectReferenceValue as Texture);

        private bool cookieWarningValue =>
            (((this.typeIsSame && (this.light.type == LightType.Spot)) && (!this.m_Cookie.hasMultipleDifferentValues && (this.cookie != null))) && (this.cookie.wrapMode != TextureWrapMode.Clamp));

        private bool dirOptionsValue =>
            (this.typeIsSame && (this.light.type == LightType.Directional));

        private bool isBakedOrMixed =>
            !this.isRealtime;

        private bool isCompletelyBaked =>
            (this.m_Lightmapping.intValue == 2);

        private bool isRealtime =>
            (this.m_Lightmapping.intValue == 4);

        private Light light =>
            (base.target as Light);

        private bool lightmappingTypeIsSame =>
            !this.m_Lightmapping.hasMultipleDifferentValues;

        private bool pointOptionsValue =>
            (this.typeIsSame && (this.light.type == LightType.Point));

        private bool runtimeOptionsValue =>
            (this.typeIsSame && ((this.light.type != LightType.Area) && !this.isCompletelyBaked));

        private bool shadowOptionsValue =>
            (this.shadowTypeIsSame && (this.light.shadows != LightShadows.None));

        private bool shadowTypeIsSame =>
            !this.m_ShadowsType.hasMultipleDifferentValues;

        private bool spotOptionsValue =>
            (this.typeIsSame && (this.light.type == LightType.Spot));

        private bool typeIsSame =>
            !this.m_Type.hasMultipleDifferentValues;

        private class Styles
        {
            public readonly GUIContent BakedShadowAngle = EditorGUIUtility.TextContent("Baked Shadow Angle");
            public readonly GUIContent BakedShadowRadius = EditorGUIUtility.TextContent("Baked Shadow Radius");
            public readonly GUIContent iconRemove = EditorGUIUtility.IconContent("Toolbar Minus", "Remove command buffer");
            public readonly GUIStyle invisibleButton = "InvisibleButton";
            public readonly GUIContent LightBounceIntensity = EditorGUIUtility.TextContent("Bounce Intensity|Indirect light intensity multiplier.");
            public readonly GUIContent LightmappingModeLabel = EditorGUIUtility.TextContent("Baking");
            public readonly GUIContent[] LightmappingModes = new GUIContent[] { EditorGUIUtility.TextContent("Realtime"), EditorGUIUtility.TextContent("Baked"), EditorGUIUtility.TextContent("Mixed") };
            public readonly int[] LightmappingModeValues = new int[] { 4, 2, 1 };
            public readonly GUIContent ShadowNearPlane = EditorGUIUtility.TextContent("Shadow Near Plane|Shadow near plane, clamped to 0.1 units or 1% of light range, whichever is lower.");
            public readonly GUIContent ShadowType = EditorGUIUtility.TextContent("Shadow Type|Shadow cast options");
        }
    }
}

