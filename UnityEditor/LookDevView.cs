namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEditor.Rendering;
    using UnityEditor.SceneManagement;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering;

    [EditorWindowTitle(title="Look Dev", useTypeNameAsIconName=true)]
    internal class LookDevView : EditorWindow, IHasCustomMenu
    {
        private float kDefaultSceneHeight = -500f;
        private float kLineHeight = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        private float kReferenceScale = 1080f;
        private float m_BlendFactorCircleRadius = 0.01f;
        private float m_BlendFactorCircleSelectionRadius = 0.03f;
        private CameraControllerStandard m_CameraController = new CameraControllerStandard();
        private bool m_CaptureRD = false;
        private static string m_configAssetPath = "Library/LookDevConfig.asset";
        private Rect m_ControlWindowRect;
        private LookDevEditionContext m_CurrentDragContext = LookDevEditionContext.None;
        private float m_CurrentObjRotationOffset = 0f;
        private float m_DirBias = 0.01f;
        private float m_DirNormalBias = 0.4f;
        private bool m_DisplayDebugGizmo = false;
        private Rect m_DisplayRect;
        private float m_EnvRotationAcc = 0f;
        private RenderTexture m_FinalCompositionTexture = null;
        public static Color32 m_FirstViewGizmoColor;
        private bool m_ForceGizmoRenderSelector = false;
        private float m_GizmoCircleRadius = 0.014f;
        private float m_GizmoCircleRadiusSelected = 0.03f;
        private LookDevOperationType m_GizmoRenderMode = LookDevOperationType.None;
        private float m_GizmoThickness = 0.0028f;
        private float m_GizmoThicknessSelected = 0.015f;
        private int m_hotControlID = 0;
        private LookDevConfig m_LookDevConfig = null;
        private LookDevEnvironmentLibrary m_LookDevEnvLibrary = null;
        private LookDevEnvironmentWindow m_LookDevEnvWindow = null;
        private bool[] m_LookDevModeToggles = new bool[5];
        private LookDevOperationType m_LookDevOperationType = LookDevOperationType.None;
        [SerializeField]
        private LookDevEnvironmentLibrary m_LookDevUserEnvLibrary = null;
        private float m_ObjRotationAcc = 0f;
        private Vector2 m_OnMouseDownOffsetToGizmo;
        private Rect[] m_PreviewRects = new Rect[3];
        private PreviewContext[] m_PreviewUtilityContexts = new PreviewContext[2];
        private GUIContent m_RenderdocContent;
        private GUIContent m_ResetEnvironment;
        private Vector4 m_ScreenRatio;
        public static Color32 m_SecondViewGizmoColor;
        private bool m_ShowLookDevEnvWindow = false;
        private GUIContent m_SyncLightVertical;
        private static Styles s_Styles = null;

        public LookDevView()
        {
            for (int i = 0; i < 5; i++)
            {
                this.m_LookDevModeToggles[i] = false;
            }
            base.wantsMouseMove = true;
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            if (RenderDoc.IsInstalled() && !RenderDoc.IsLoaded())
            {
                menu.AddItem(new GUIContent("Load RenderDoc"), false, new GenericMenu.MenuFunction(this.LoadRenderDoc));
            }
        }

        private void Cleanup()
        {
            LookDevResources.Cleanup();
            this.m_LookDevConfig.Cleanup();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if ((this.m_PreviewUtilityContexts[i] != null) && (this.m_PreviewUtilityContexts[i].m_PreviewUtility[j] != null))
                    {
                        this.m_PreviewUtilityContexts[i].m_PreviewUtility[j].Cleanup();
                        this.m_PreviewUtilityContexts[i].m_PreviewUtility[j] = null;
                    }
                }
            }
            if (this.m_FinalCompositionTexture != null)
            {
                Object.DestroyImmediate(this.m_FinalCompositionTexture);
                this.m_FinalCompositionTexture = null;
            }
        }

        private void CleanupDeletedHDRI()
        {
            this.m_LookDevEnvLibrary.CleanupDeletedHDRI();
        }

        private float ComputeLookDevEnvWindowHeight()
        {
            return this.m_DisplayRect.height;
        }

        private float ComputeLookDevEnvWindowWidth()
        {
            bool flag = (this.m_DisplayRect.height - 5f) < (146f * this.m_LookDevEnvLibrary.hdriCount);
            return (250f + (!flag ? 5f : 19f));
        }

        public void CreateNewLibrary(string assetPath)
        {
            AssetDatabase.CreateAsset(Object.Instantiate<LookDevEnvironmentLibrary>(this.envLibrary), assetPath);
            this.envLibrary = AssetDatabase.LoadAssetAtPath(assetPath, typeof(LookDevEnvironmentLibrary)) as LookDevEnvironmentLibrary;
        }

        private void DoAdditionalGUI()
        {
            if (this.m_LookDevConfig.lookDevMode == LookDevMode.SideBySide)
            {
                int num = 0x20;
                GUILayout.BeginArea(new Rect((this.m_PreviewRects[2].width - num) / 2f, (this.m_PreviewRects[2].height - num) / 2f, (float) num, (float) num));
                EditorGUI.BeginChangeCheck();
                bool sideBySideCameraLinked = this.m_LookDevConfig.sideBySideCameraLinked;
                bool flag2 = GUILayout.Toggle(sideBySideCameraLinked, this.GetGUIContentLink(sideBySideCameraLinked), styles.sToolBarButton, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_LookDevConfig.sideBySideCameraLinked = flag2;
                    if (flag2)
                    {
                        CameraState cameraStateIn = (this.m_LookDevConfig.currentEditionContext != LookDevEditionContext.Left) ? this.m_LookDevConfig.cameraStateRight : this.m_LookDevConfig.cameraStateLeft;
                        ((this.m_LookDevConfig.currentEditionContext != LookDevEditionContext.Left) ? this.m_LookDevConfig.cameraStateLeft : this.m_LookDevConfig.cameraStateRight).Copy(cameraStateIn);
                    }
                }
                GUILayout.EndArea();
            }
        }

        private void DoControlWindow()
        {
            if (this.m_LookDevConfig.showControlWindows)
            {
                float width = 68f;
                float num2 = 150f;
                float num3 = 30f;
                GUILayout.BeginArea(this.m_ControlWindowRect, styles.sBigTitleInnerStyle);
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(this.kLineHeight) };
                GUILayout.BeginHorizontal(options);
                GUILayout.FlexibleSpace();
                bool flag = false;
                EditorGUI.BeginChangeCheck();
                flag = GUILayout.Toggle(this.m_LookDevModeToggles[0], styles.sSingleMode1, styles.sToolBarButton, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    this.UpdateLookDevModeToggle(LookDevMode.Single1, flag);
                    this.m_LookDevConfig.UpdateFocus(LookDevEditionContext.Left);
                    base.Repaint();
                }
                EditorGUI.BeginChangeCheck();
                flag = GUILayout.Toggle(this.m_LookDevModeToggles[1], styles.sSingleMode2, styles.sToolBarButton, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    this.UpdateLookDevModeToggle(LookDevMode.Single2, flag);
                    this.m_LookDevConfig.UpdateFocus(LookDevEditionContext.Right);
                    base.Repaint();
                }
                EditorGUI.BeginChangeCheck();
                flag = GUILayout.Toggle(this.m_LookDevModeToggles[2], styles.sSideBySideMode, styles.sToolBarButton, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    this.UpdateLookDevModeToggle(LookDevMode.SideBySide, flag);
                }
                EditorGUI.BeginChangeCheck();
                flag = GUILayout.Toggle(this.m_LookDevModeToggles[3], styles.sSplitMode, styles.sToolBarButton, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    this.UpdateLookDevModeToggle(LookDevMode.Split, flag);
                }
                EditorGUI.BeginChangeCheck();
                flag = GUILayout.Toggle(this.m_LookDevModeToggles[4], styles.sZoneMode, styles.sToolBarButton, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    this.UpdateLookDevModeToggle(LookDevMode.Zone, flag);
                }
                GUILayout.EndHorizontal();
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Height(this.kLineHeight) };
                GUILayout.BeginHorizontal(optionArray2);
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(width) };
                GUILayout.Label(LookDevViewsWindow.styles.sExposure, this.GetPropertyLabelStyle(LookDevProperty.ExposureValue), optionArray3);
                float exposureValue = this.m_LookDevConfig.currentLookDevContext.exposureValue;
                EditorGUI.BeginChangeCheck();
                float max = Mathf.Round(this.m_LookDevConfig.exposureRange);
                GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.Width(num2) };
                GUILayoutOption[] optionArray5 = new GUILayoutOption[] { GUILayout.Width(num3) };
                exposureValue = Mathf.Clamp(EditorGUILayout.FloatField(Mathf.Clamp(GUILayout.HorizontalSlider((float) Math.Round((double) exposureValue, (exposureValue >= 0f) ? 2 : 1), -max, max, optionArray4), -max, max), optionArray5), -max, max);
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_LookDevConfig.UpdateFloatProperty(LookDevProperty.ExposureValue, exposureValue);
                }
                bool flag2 = false;
                EditorGUI.BeginChangeCheck();
                bool flag3 = this.m_LookDevConfig.IsPropertyLinked(LookDevProperty.ExposureValue);
                flag2 = GUILayout.Toggle(flag3, this.GetGUIContentLink(flag3), styles.sToolBarButton, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_LookDevConfig.UpdatePropertyLink(LookDevProperty.ExposureValue, flag2);
                }
                GUILayout.EndHorizontal();
                GUILayoutOption[] optionArray6 = new GUILayoutOption[] { GUILayout.Height(this.kLineHeight) };
                GUILayout.BeginHorizontal(optionArray6);
                using (new EditorGUI.DisabledScope(this.m_LookDevEnvLibrary.hdriList.Count <= 1))
                {
                    GUILayoutOption[] optionArray7 = new GUILayoutOption[] { GUILayout.Width(width) };
                    GUILayout.Label(LookDevViewsWindow.styles.sEnvironment, this.GetPropertyLabelStyle(LookDevProperty.HDRI), optionArray7);
                    int currentHDRIIndex = -1;
                    if (this.m_LookDevEnvLibrary.hdriList.Count > 1)
                    {
                        int num7 = this.m_LookDevEnvLibrary.hdriList.Count - 1;
                        currentHDRIIndex = this.m_LookDevConfig.currentLookDevContext.currentHDRIIndex;
                        EditorGUI.BeginChangeCheck();
                        GUILayoutOption[] optionArray8 = new GUILayoutOption[] { GUILayout.Width(num2) };
                        currentHDRIIndex = (int) GUILayout.HorizontalSlider((float) currentHDRIIndex, 0f, (float) num7, optionArray8);
                        GUILayoutOption[] optionArray9 = new GUILayoutOption[] { GUILayout.Width(num3) };
                        currentHDRIIndex = Mathf.Clamp(EditorGUILayout.IntField(currentHDRIIndex, optionArray9), 0, num7);
                        if (EditorGUI.EndChangeCheck())
                        {
                            this.m_LookDevConfig.UpdateIntProperty(LookDevProperty.HDRI, currentHDRIIndex);
                        }
                    }
                    else
                    {
                        GUILayoutOption[] optionArray10 = new GUILayoutOption[] { GUILayout.Width(num2) };
                        GUILayout.HorizontalSlider(0f, 0f, 0f, optionArray10);
                        GUILayoutOption[] optionArray11 = new GUILayoutOption[] { GUILayout.Width(num3) };
                        GUILayout.Label(LookDevViewsWindow.styles.sZero, EditorStyles.miniLabel, optionArray11);
                    }
                }
                bool flag4 = false;
                EditorGUI.BeginChangeCheck();
                bool flag5 = this.m_LookDevConfig.IsPropertyLinked(LookDevProperty.HDRI);
                flag4 = GUILayout.Toggle(flag5, this.GetGUIContentLink(flag5), styles.sToolBarButton, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_LookDevConfig.UpdatePropertyLink(LookDevProperty.HDRI, flag4);
                }
                GUILayout.EndHorizontal();
                GUILayoutOption[] optionArray12 = new GUILayoutOption[] { GUILayout.Height(this.kLineHeight) };
                GUILayout.BeginHorizontal(optionArray12);
                GUILayoutOption[] optionArray13 = new GUILayoutOption[] { GUILayout.Width(width) };
                GUILayout.Label(LookDevViewsWindow.styles.sShadingMode, this.GetPropertyLabelStyle(LookDevProperty.ShadingMode), optionArray13);
                int shadingMode = this.m_LookDevConfig.currentLookDevContext.shadingMode;
                EditorGUI.BeginChangeCheck();
                GUILayoutOption[] optionArray14 = new GUILayoutOption[] { GUILayout.Width((num3 + num2) + 4f) };
                shadingMode = EditorGUILayout.IntPopup("", shadingMode, LookDevViewsWindow.styles.sShadingModeStrings, LookDevViewsWindow.styles.sShadingModeValues, optionArray14);
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_LookDevConfig.UpdateIntProperty(LookDevProperty.ShadingMode, shadingMode);
                }
                bool flag6 = false;
                EditorGUI.BeginChangeCheck();
                bool flag7 = this.m_LookDevConfig.IsPropertyLinked(LookDevProperty.ShadingMode);
                flag6 = GUILayout.Toggle(flag7, this.GetGUIContentLink(flag7), styles.sToolBarButton, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_LookDevConfig.UpdatePropertyLink(LookDevProperty.ShadingMode, flag6);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }

        private void DoGizmoDebug()
        {
            if (this.m_DisplayDebugGizmo)
            {
                int num = 7;
                float width = 150f;
                float height = this.kLineHeight * num;
                float num4 = 60f;
                float num5 = 90f;
                GUILayout.BeginArea(new Rect((base.position.width - width) - 10f, (base.position.height - height) - 10f, width, height), styles.sBigTitleInnerStyle);
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(num5) };
                GUILayout.Label("Thickness", EditorStyles.miniLabel, options);
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(num4) };
                this.m_GizmoThickness = Mathf.Clamp(EditorGUILayout.FloatField(this.m_GizmoThickness, optionArray2), 0f, 1f);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(num5) };
                GUILayout.Label("ThicknessSelected", EditorStyles.miniLabel, optionArray3);
                GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.Width(num4) };
                this.m_GizmoThicknessSelected = Mathf.Clamp(EditorGUILayout.FloatField(this.m_GizmoThicknessSelected, optionArray4), 0f, 1f);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayoutOption[] optionArray5 = new GUILayoutOption[] { GUILayout.Width(num5) };
                GUILayout.Label("Radius", EditorStyles.miniLabel, optionArray5);
                GUILayoutOption[] optionArray6 = new GUILayoutOption[] { GUILayout.Width(num4) };
                this.m_GizmoCircleRadius = Mathf.Clamp(EditorGUILayout.FloatField(this.m_GizmoCircleRadius, optionArray6), 0f, 1f);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayoutOption[] optionArray7 = new GUILayoutOption[] { GUILayout.Width(num5) };
                GUILayout.Label("RadiusSelected", EditorStyles.miniLabel, optionArray7);
                GUILayoutOption[] optionArray8 = new GUILayoutOption[] { GUILayout.Width(num4) };
                this.m_GizmoCircleRadiusSelected = Mathf.Clamp(EditorGUILayout.FloatField(this.m_GizmoCircleRadiusSelected, optionArray8), 0f, 1f);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayoutOption[] optionArray9 = new GUILayoutOption[] { GUILayout.Width(num5) };
                GUILayout.Label("BlendRadius", EditorStyles.miniLabel, optionArray9);
                GUILayoutOption[] optionArray10 = new GUILayoutOption[] { GUILayout.Width(num4) };
                this.m_BlendFactorCircleRadius = Mathf.Clamp(EditorGUILayout.FloatField(this.m_BlendFactorCircleRadius, optionArray10), 0f, 1f);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayoutOption[] optionArray11 = new GUILayoutOption[] { GUILayout.Width(num5) };
                GUILayout.Label("Selected", EditorStyles.miniLabel, optionArray11);
                this.m_ForceGizmoRenderSelector = GUILayout.Toggle(this.m_ForceGizmoRenderSelector, "", new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Reset Gizmo", new GUILayoutOption[0]))
                {
                    this.m_LookDevConfig.gizmo.Update(new Vector2(0f, 0f), 0.2f, 0f);
                }
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }

        private void DoToolbarGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(120f) };
            if (EditorGUI.ButtonMouseDown(GUILayoutUtility.GetRect(LookDevSettingsWindow.styles.sTitle, EditorStyles.toolbarDropDown, options), LookDevSettingsWindow.styles.sTitle, FocusType.Passive, EditorStyles.toolbarDropDown))
            {
                PopupWindow.Show(GUILayoutUtility.topLevel.GetLast(), new LookDevSettingsWindow(this));
                GUIUtility.ExitGUI();
            }
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(120f) };
            if (EditorGUI.ButtonMouseDown(GUILayoutUtility.GetRect(LookDevViewsWindow.styles.sTitle, EditorStyles.toolbarDropDown, optionArray2), LookDevViewsWindow.styles.sTitle, FocusType.Passive, EditorStyles.toolbarDropDown))
            {
                PopupWindow.Show(GUILayoutUtility.topLevel.GetLast(), new LookDevViewsWindow(this));
                GUIUtility.ExitGUI();
            }
            this.m_LookDevConfig.enableShadowCubemap = GUILayout.Toggle(this.m_LookDevConfig.enableShadowCubemap, LookDevSettingsWindow.styles.sEnableShadowIcon, styles.sToolBarButton, new GUILayoutOption[0]);
            this.m_LookDevConfig.rotateObjectMode = GUILayout.Toggle(this.m_LookDevConfig.rotateObjectMode, LookDevSettingsWindow.styles.sEnableObjRotationIcon, styles.sToolBarButton, new GUILayoutOption[0]);
            this.m_LookDevConfig.rotateEnvMode = GUILayout.Toggle(this.m_LookDevConfig.rotateEnvMode, LookDevSettingsWindow.styles.sEnableEnvRotationIcon, styles.sToolBarButton, new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (this.m_ShowLookDevEnvWindow)
            {
                if (GUILayout.Button(this.m_SyncLightVertical, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                {
                    Undo.RecordObject(this.m_LookDevEnvLibrary, "Synchronize lights");
                    int currentHDRIIndex = this.m_LookDevConfig.currentLookDevContext.currentHDRIIndex;
                    for (int i = 0; i < this.m_LookDevEnvLibrary.hdriList.Count; i++)
                    {
                        CubemapInfo local1 = this.m_LookDevEnvLibrary.hdriList[i];
                        local1.angleOffset += (this.m_LookDevEnvLibrary.hdriList[currentHDRIIndex].shadowInfo.longitude + this.m_LookDevEnvLibrary.hdriList[currentHDRIIndex].angleOffset) - (this.m_LookDevEnvLibrary.hdriList[i].shadowInfo.longitude + this.m_LookDevEnvLibrary.hdriList[i].angleOffset);
                        this.m_LookDevEnvLibrary.hdriList[i].angleOffset = (this.m_LookDevEnvLibrary.hdriList[i].angleOffset + 360f) % 360f;
                    }
                    this.m_LookDevEnvLibrary.dirty = true;
                    GUIUtility.ExitGUI();
                }
                if (GUILayout.Button(this.m_ResetEnvironment, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                {
                    Undo.RecordObject(this.m_LookDevEnvLibrary, "Reset environment");
                    for (int j = 0; j < this.m_LookDevEnvLibrary.hdriList.Count; j++)
                    {
                        this.m_LookDevEnvLibrary.hdriList[j].angleOffset = 0f;
                    }
                    this.m_LookDevEnvLibrary.dirty = true;
                    GUIUtility.ExitGUI();
                }
            }
            if (RenderDoc.IsLoaded())
            {
                using (new EditorGUI.DisabledScope(!RenderDoc.IsSupported()))
                {
                    if (GUILayout.Button(this.m_RenderdocContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                    {
                        this.m_CaptureRD = true;
                        GUIUtility.ExitGUI();
                    }
                }
            }
            if (EditorGUI.ButtonMouseDown(GUILayoutUtility.GetRect(LookDevEnvironmentWindow.styles.sTitle, EditorStyles.iconButton), LookDevEnvironmentWindow.styles.sTitle, FocusType.Passive, EditorStyles.iconButton))
            {
                this.m_ShowLookDevEnvWindow = !this.m_ShowLookDevEnvWindow;
            }
            if (this.m_ShowLookDevEnvWindow)
            {
                Rect gUIRect = new Rect {
                    x = 0f,
                    y = 0f,
                    width = this.ComputeLookDevEnvWindowWidth(),
                    height = this.ComputeLookDevEnvWindowHeight()
                };
                Rect positionInLookDev = new Rect {
                    x = this.m_DisplayRect.width - this.ComputeLookDevEnvWindowWidth(),
                    y = this.m_DisplayRect.y,
                    width = this.ComputeLookDevEnvWindowWidth(),
                    height = this.ComputeLookDevEnvWindowHeight()
                };
                this.m_LookDevEnvWindow.SetRects(positionInLookDev, gUIRect, this.m_DisplayRect);
                GUILayout.Window(0, positionInLookDev, new GUI.WindowFunction(this.m_LookDevEnvWindow.OnGUI), "", styles.sBigTitleInnerStyle, new GUILayoutOption[0]);
            }
            GUILayout.EndHorizontal();
        }

        public static void DrawFullScreenQuad(Rect previewRect)
        {
            GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Viewport(previewRect);
            GL.Begin(7);
            GL.TexCoord2(0f, 0f);
            GL.Vertex3(0f, 0f, 0f);
            GL.TexCoord2(0f, 1f);
            GL.Vertex3(0f, 1f, 0f);
            GL.TexCoord2(1f, 1f);
            GL.Vertex3(1f, 1f, 0f);
            GL.TexCoord2(1f, 0f);
            GL.Vertex3(1f, 0f, 0f);
            GL.End();
            GL.PopMatrix();
            GL.sRGBWrite = false;
        }

        public void Frame()
        {
            this.Frame(true);
        }

        public void Frame(bool animate)
        {
            this.Frame(LookDevEditionContext.Left, animate);
            this.Frame(LookDevEditionContext.Right, animate);
        }

        private void Frame(LookDevEditionContext context, bool animate)
        {
            GameObject go = this.m_LookDevConfig.currentObject[(int) context];
            if (go != null)
            {
                Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
                this.GetRenderableBoundsRecurse(ref bounds, go);
                float num = bounds.extents.magnitude * 1.5f;
                if (num == 0f)
                {
                    num = 10f;
                }
                CameraState state = this.m_LookDevConfig.cameraState[(int) context];
                if (animate)
                {
                    state.pivot.target = bounds.center;
                    state.viewSize.target = Mathf.Abs((float) (num * 2.2f));
                }
                else
                {
                    state.pivot.value = bounds.center;
                    state.viewSize.value = Mathf.Abs((float) (num * 2.2f));
                }
            }
            this.m_CurrentObjRotationOffset = 0f;
        }

        private float GetBlendFactorMaxGizmoDistance()
        {
            return ((this.m_LookDevConfig.gizmo.length - this.m_GizmoCircleRadius) - this.m_BlendFactorCircleRadius);
        }

        private LookDevEditionContext GetEditionContext(Vector2 position)
        {
            if (!this.m_PreviewRects[2].Contains(position))
            {
                return LookDevEditionContext.None;
            }
            switch (this.m_LookDevConfig.lookDevMode)
            {
                case LookDevMode.Single1:
                    return LookDevEditionContext.Left;

                case LookDevMode.Single2:
                    return LookDevEditionContext.Right;

                case LookDevMode.SideBySide:
                    if (!this.m_PreviewRects[0].Contains(position))
                    {
                        return LookDevEditionContext.Right;
                    }
                    return LookDevEditionContext.Left;

                case LookDevMode.Split:
                {
                    Vector2 normalizedCoordinates = this.GetNormalizedCoordinates(position, this.m_PreviewRects[2]);
                    if (Vector3.Dot(new Vector3(normalizedCoordinates.x, normalizedCoordinates.y, 1f), (Vector3) this.m_LookDevConfig.gizmo.plane) <= 0f)
                    {
                        return LookDevEditionContext.Right;
                    }
                    return LookDevEditionContext.Left;
                }
                case LookDevMode.Zone:
                    if ((Vector2.Distance(this.GetNormalizedCoordinates(position, this.m_PreviewRects[2]), this.m_LookDevConfig.gizmo.point2) - (this.m_LookDevConfig.gizmo.length * 2f)) <= 0f)
                    {
                        return LookDevEditionContext.Right;
                    }
                    return LookDevEditionContext.Left;
            }
            return LookDevEditionContext.Left;
        }

        private LookDevOperationType GetGizmoZoneOperation(Vector2 mousePosition, Rect previewRect)
        {
            Vector2 normalizedCoordinates = this.GetNormalizedCoordinates(mousePosition, previewRect);
            Vector3 lhs = new Vector3(normalizedCoordinates.x, normalizedCoordinates.y, 1f);
            float num2 = Mathf.Abs(Vector3.Dot(lhs, (Vector3) this.m_LookDevConfig.gizmo.plane));
            float num3 = Vector2.Distance(normalizedCoordinates, this.m_LookDevConfig.gizmo.center);
            float num5 = (Vector3.Dot(lhs, (Vector3) this.m_LookDevConfig.gizmo.planeOrtho) <= 0f) ? -1f : 1f;
            Vector2 vector3 = new Vector2(this.m_LookDevConfig.gizmo.planeOrtho.x, this.m_LookDevConfig.gizmo.planeOrtho.y);
            LookDevOperationType none = LookDevOperationType.None;
            if ((num2 < this.m_GizmoCircleRadiusSelected) && (num3 < (this.m_LookDevConfig.gizmo.length + this.m_GizmoCircleRadiusSelected)))
            {
                if (num2 < this.m_GizmoThicknessSelected)
                {
                    none = LookDevOperationType.GizmoTranslation;
                }
                Vector2 b = this.m_LookDevConfig.gizmo.center + ((Vector2) ((num5 * vector3) * this.m_LookDevConfig.gizmo.length));
                if (Vector2.Distance(normalizedCoordinates, b) <= this.m_GizmoCircleRadiusSelected)
                {
                    none = (num5 <= 0f) ? LookDevOperationType.GizmoRotationZone2 : LookDevOperationType.GizmoRotationZone1;
                }
                float blendFactorMaxGizmoDistance = this.GetBlendFactorMaxGizmoDistance();
                float num8 = (this.GetBlendFactorMaxGizmoDistance() + this.m_BlendFactorCircleRadius) - this.m_BlendFactorCircleSelectionRadius;
                float f = this.m_LookDevConfig.dualViewBlendFactor * this.GetBlendFactorMaxGizmoDistance();
                Vector2 vector7 = this.m_LookDevConfig.gizmo.center - ((Vector2) (vector3 * f));
                float num10 = Mathf.Lerp(this.m_BlendFactorCircleRadius, this.m_BlendFactorCircleSelectionRadius, Mathf.Clamp((float) ((blendFactorMaxGizmoDistance - Mathf.Abs(f)) / (blendFactorMaxGizmoDistance - num8)), (float) 0f, (float) 1f));
                Vector2 vector8 = normalizedCoordinates - vector7;
                if (vector8.magnitude < num10)
                {
                    none = LookDevOperationType.BlendFactor;
                }
            }
            return none;
        }

        private GUIContent GetGUIContentLink(bool active)
        {
            return (!active ? styles.sLinkInactive : styles.sLinkActive);
        }

        private Vector2 GetNormalizedCoordinates(Vector2 mousePosition, Rect previewRect)
        {
            Vector2 vector = new Vector3((mousePosition.x - previewRect.x) / previewRect.width, (mousePosition.y - previewRect.y) / previewRect.height);
            vector.x = ((vector.x * 2f) - 1f) * this.m_ScreenRatio.x;
            vector.y = -((vector.y * 2f) - 1f) * this.m_ScreenRatio.y;
            return vector;
        }

        private GUIStyle GetPropertyLabelStyle(LookDevProperty property)
        {
            if ((this.m_LookDevConfig.IsPropertyLinked(property) || (this.m_LookDevConfig.lookDevMode == LookDevMode.Single1)) || (this.m_LookDevConfig.lookDevMode == LookDevMode.Single2))
            {
                return styles.sPropertyLabelStyle[2];
            }
            return styles.sPropertyLabelStyle[this.m_LookDevConfig.currentEditionContextIndex];
        }

        private void GetRenderableBoundsRecurse(ref Bounds bounds, GameObject go)
        {
            MeshRenderer component = go.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            MeshFilter filter = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
            if (((component != null) && (filter != null)) && (filter.sharedMesh != null))
            {
                if (bounds.extents == Vector3.zero)
                {
                    bounds = component.bounds;
                }
                else
                {
                    bounds.Encapsulate(component.bounds);
                }
            }
            SkinnedMeshRenderer renderer2 = go.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
            if ((renderer2 != null) && (renderer2.sharedMesh != null))
            {
                if (bounds.extents == Vector3.zero)
                {
                    bounds = renderer2.bounds;
                }
                else
                {
                    bounds.Encapsulate(renderer2.bounds);
                }
            }
            SpriteRenderer renderer3 = go.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            if ((renderer3 != null) && (renderer3.sprite != null))
            {
                if (bounds.extents == Vector3.zero)
                {
                    bounds = renderer3.bounds;
                }
                else
                {
                    bounds.Encapsulate(renderer3.bounds);
                }
            }
            IEnumerator enumerator = go.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    this.GetRenderableBoundsRecurse(ref bounds, current.gameObject);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        private void GetShaderConstantsFromNormalizedSH(SphericalHarmonicsL2 ambientProbe, Vector4[] outCoefficients)
        {
            for (int i = 0; i < 3; i++)
            {
                outCoefficients[i].x = ambientProbe[i, 3];
                outCoefficients[i].y = ambientProbe[i, 1];
                outCoefficients[i].z = ambientProbe[i, 2];
                outCoefficients[i].w = ambientProbe[i, 0] - ambientProbe[i, 6];
                outCoefficients[i + 3].x = ambientProbe[i, 4];
                outCoefficients[i + 3].y = ambientProbe[i, 5];
                outCoefficients[i + 3].z = ambientProbe[i, 6] * 3f;
                outCoefficients[i + 3].w = ambientProbe[i, 7];
            }
            outCoefficients[6].x = ambientProbe[0, 8];
            outCoefficients[6].y = ambientProbe[1, 8];
            outCoefficients[6].z = ambientProbe[2, 8];
            outCoefficients[6].w = 1f;
        }

        private void HandleCamera()
        {
            if ((this.m_LookDevOperationType == LookDevOperationType.None) && !this.m_ControlWindowRect.Contains(Event.current.mousePosition))
            {
                int currentEditionContextIndex = this.m_LookDevConfig.currentEditionContextIndex;
                int index = (currentEditionContextIndex + 1) % 2;
                this.m_CameraController.Update(this.m_LookDevConfig.cameraState[currentEditionContextIndex], this.m_PreviewUtilityContexts[this.m_LookDevConfig.currentEditionContextIndex].m_PreviewUtility[0].m_Camera);
                if ((((this.m_LookDevConfig.lookDevMode == LookDevMode.Single1) || (this.m_LookDevConfig.lookDevMode == LookDevMode.Single2)) || (this.m_LookDevConfig.lookDevMode == LookDevMode.SideBySide)) && this.m_LookDevConfig.sideBySideCameraLinked)
                {
                    this.m_LookDevConfig.cameraState[index].Copy(this.m_LookDevConfig.cameraState[currentEditionContextIndex]);
                }
                if (((this.m_CameraController.currentViewTool == ViewTool.None) && ((Event.current.type == EventType.KeyUp) && (Event.current.keyCode == KeyCode.F))) && !EditorGUIUtility.editingTextField)
                {
                    this.Frame(this.m_LookDevConfig.currentEditionContext, true);
                    Event.current.Use();
                }
                for (int i = 0; i < 3; i++)
                {
                    this.m_LookDevConfig.cameraState[0].UpdateCamera(this.m_PreviewUtilityContexts[0].m_PreviewUtility[i].m_Camera);
                    this.m_LookDevConfig.cameraState[1].UpdateCamera(this.m_PreviewUtilityContexts[1].m_PreviewUtility[i].m_Camera);
                }
            }
        }

        private void HandleDragging()
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.Repaint:
                    return;

                case EventType.DragUpdated:
                {
                    bool flag3 = false;
                    foreach (Object obj4 in DragAndDrop.objectReferences)
                    {
                        Cubemap cubemap3 = obj4 as Cubemap;
                        if (cubemap3 != null)
                        {
                            flag3 = true;
                        }
                        Material material2 = obj4 as Material;
                        if ((material2 != null) && material2.shader.name.Contains("Skybox/Cubemap"))
                        {
                            flag3 = true;
                        }
                        GameObject go = obj4 as GameObject;
                        if ((go != null) && GameObjectInspector.HasRenderableParts(go))
                        {
                            flag3 = true;
                        }
                        LookDevEnvironmentLibrary library2 = obj4 as LookDevEnvironmentLibrary;
                        if (library2 != null)
                        {
                            flag3 = true;
                        }
                    }
                    DragAndDrop.visualMode = !flag3 ? DragAndDropVisualMode.Rejected : DragAndDropVisualMode.Link;
                    this.m_CurrentDragContext = this.GetEditionContext(Event.current.mousePosition);
                    current.Use();
                    return;
                }
                case EventType.DragPerform:
                {
                    bool flag = false;
                    if (this.m_PreviewRects[2].Contains(current.mousePosition))
                    {
                        foreach (Object obj2 in DragAndDrop.objectReferences)
                        {
                            Cubemap cubemap = obj2 as Cubemap;
                            if (cubemap != null)
                            {
                                this.UpdateFocus(Event.current.mousePosition);
                                this.UpdateContextWithCurrentHDRI(cubemap);
                            }
                            Material material = obj2 as Material;
                            if ((material != null) && material.shader.name.Contains("Skybox/Cubemap"))
                            {
                                Cubemap texture = material.GetTexture("_Tex") as Cubemap;
                                if (texture != null)
                                {
                                    this.UpdateFocus(Event.current.mousePosition);
                                    this.UpdateContextWithCurrentHDRI(texture);
                                }
                            }
                            GameObject obj3 = obj2 as GameObject;
                            if ((obj3 != null) && (!flag && GameObjectInspector.HasRenderableParts(obj3)))
                            {
                                this.UpdateFocus(Event.current.mousePosition);
                                Undo.RecordObject(this.m_LookDevConfig, "Set current preview object");
                                bool flag2 = this.m_LookDevConfig.SetCurrentPreviewObject(obj3);
                                this.Frame(this.m_LookDevConfig.currentEditionContext, false);
                                if (flag2)
                                {
                                    this.Frame((this.m_LookDevConfig.currentEditionContext != LookDevEditionContext.Left) ? LookDevEditionContext.Left : LookDevEditionContext.Right, false);
                                }
                                flag = true;
                            }
                            LookDevEnvironmentLibrary library = obj2 as LookDevEnvironmentLibrary;
                            if (library != null)
                            {
                                this.envLibrary = library;
                            }
                        }
                    }
                    DragAndDrop.AcceptDrag();
                    this.m_CurrentDragContext = LookDevEditionContext.None;
                    this.m_LookDevEnvWindow.CancelSelection();
                    current.Use();
                    return;
                }
                case EventType.DragExited:
                    this.m_CurrentDragContext = LookDevEditionContext.None;
                    return;
            }
        }

        public void HandleKeyboardShortcut()
        {
            if ((Event.current.type != EventType.Layout) && !EditorGUIUtility.editingTextField)
            {
                if ((Event.current.type == EventType.KeyUp) && (Event.current.keyCode == KeyCode.RightArrow))
                {
                    this.m_LookDevConfig.UpdateIntProperty(LookDevProperty.HDRI, Math.Min((int) (this.m_LookDevConfig.currentLookDevContext.currentHDRIIndex + 1), (int) (this.m_LookDevEnvLibrary.hdriList.Count - 1)));
                    Event.current.Use();
                }
                else if ((Event.current.type == EventType.KeyUp) && (Event.current.keyCode == KeyCode.LeftArrow))
                {
                    this.m_LookDevConfig.UpdateIntProperty(LookDevProperty.HDRI, Math.Max(this.m_LookDevConfig.currentLookDevContext.currentHDRIIndex - 1, 0));
                    Event.current.Use();
                }
                if ((Event.current.type == EventType.KeyUp) && (Event.current.keyCode == KeyCode.R))
                {
                    this.m_LookDevConfig.ResynchronizeObjects();
                    Event.current.Use();
                }
            }
        }

        private void HandleMouseInput()
        {
            Event current = Event.current;
            this.m_hotControlID = GUIUtility.GetControlID(FocusType.Passive);
            switch (current.GetTypeForControl(this.m_hotControlID))
            {
                case EventType.MouseDown:
                    if (((this.m_LookDevConfig.lookDevMode == LookDevMode.Split) || (this.m_LookDevConfig.lookDevMode == LookDevMode.Zone)) && (current.button == 0))
                    {
                        this.m_LookDevOperationType = this.GetGizmoZoneOperation(Event.current.mousePosition, this.m_PreviewRects[2]);
                        this.m_OnMouseDownOffsetToGizmo = this.GetNormalizedCoordinates(Event.current.mousePosition, this.m_PreviewRects[2]) - this.m_LookDevConfig.gizmo.center;
                    }
                    if (this.m_LookDevOperationType == LookDevOperationType.None)
                    {
                        if (current.shift && (current.button == 0))
                        {
                            this.m_LookDevOperationType = LookDevOperationType.RotateLight;
                        }
                        else if (current.control && (current.button == 0))
                        {
                            this.m_LookDevOperationType = LookDevOperationType.RotateEnvironment;
                        }
                    }
                    if (!this.IsOperatingGizmo() && !this.m_ControlWindowRect.Contains(Event.current.mousePosition))
                    {
                        this.UpdateFocus(Event.current.mousePosition);
                    }
                    GUIUtility.hotControl = this.m_hotControlID;
                    break;

                case EventType.MouseUp:
                    if ((this.m_LookDevOperationType == LookDevOperationType.BlendFactor) && (Mathf.Abs(this.m_LookDevConfig.dualViewBlendFactor) < (this.m_GizmoCircleRadiusSelected / (this.m_LookDevConfig.gizmo.length - this.m_GizmoCircleRadius))))
                    {
                        this.m_LookDevConfig.dualViewBlendFactor = 0f;
                    }
                    this.m_LookDevOperationType = LookDevOperationType.None;
                    if (this.m_LookDevEnvWindow != null)
                    {
                        Cubemap currentSelection = this.m_LookDevEnvWindow.GetCurrentSelection();
                        if (currentSelection != null)
                        {
                            this.UpdateFocus(Event.current.mousePosition);
                            this.UpdateContextWithCurrentHDRI(currentSelection);
                            this.m_LookDevEnvWindow.CancelSelection();
                            this.m_CurrentDragContext = LookDevEditionContext.None;
                            base.Repaint();
                        }
                    }
                    GUIUtility.hotControl = 0;
                    break;

                case EventType.MouseMove:
                    this.m_GizmoRenderMode = this.GetGizmoZoneOperation(Event.current.mousePosition, this.m_PreviewRects[2]);
                    base.Repaint();
                    break;

                case EventType.MouseDrag:
                {
                    if (this.m_LookDevOperationType != LookDevOperationType.RotateEnvironment)
                    {
                        if ((this.m_LookDevOperationType == LookDevOperationType.RotateLight) && this.m_LookDevConfig.enableShadowCubemap)
                        {
                            ShadowInfo shadowInfo = this.m_LookDevEnvLibrary.hdriList[this.m_LookDevConfig.currentLookDevContext.currentHDRIIndex].shadowInfo;
                            shadowInfo.latitude -= current.delta.y * 0.6f;
                            shadowInfo.longitude -= current.delta.x * 0.6f;
                            base.Repaint();
                        }
                        break;
                    }
                    float num = ((this.m_LookDevConfig.currentLookDevContext.envRotation + ((current.delta.x / Mathf.Min(base.position.width, base.position.height)) * 140f)) + 720f) % 720f;
                    this.m_LookDevConfig.UpdateFloatProperty(LookDevProperty.EnvRotation, num);
                    Event.current.Use();
                    break;
                }
            }
            if ((Event.current.rawType == EventType.MouseUp) && (this.m_LookDevEnvWindow.GetCurrentSelection() != null))
            {
                this.m_LookDevEnvWindow.CancelSelection();
            }
            if (this.m_LookDevOperationType == LookDevOperationType.GizmoTranslation)
            {
                Vector2 center = this.GetNormalizedCoordinates(Event.current.mousePosition, this.m_PreviewRects[2]) - this.m_OnMouseDownOffsetToGizmo;
                Vector2 normalizedCoordinates = this.GetNormalizedCoordinates(new Vector2(this.m_DisplayRect.x, this.m_PreviewRects[2].y + this.m_DisplayRect.height), this.m_PreviewRects[2]);
                Vector2 vector6 = this.GetNormalizedCoordinates(new Vector2(this.m_DisplayRect.x + this.m_DisplayRect.width, this.m_PreviewRects[2].y), this.m_PreviewRects[2]);
                float num2 = 0.05f;
                center.x = Mathf.Clamp(center.x, normalizedCoordinates.x + num2, vector6.x - num2);
                center.y = Mathf.Clamp(center.y, normalizedCoordinates.y + num2, vector6.y - num2);
                this.m_LookDevConfig.gizmo.Update(center, this.m_LookDevConfig.gizmo.length, this.m_LookDevConfig.gizmo.angle);
                base.Repaint();
            }
            if ((this.m_LookDevOperationType == LookDevOperationType.GizmoRotationZone1) || (this.m_LookDevOperationType == LookDevOperationType.GizmoRotationZone2))
            {
                Vector2 vector8;
                Vector2 vector9;
                Vector2 vector7 = this.GetNormalizedCoordinates(Event.current.mousePosition, this.m_PreviewRects[2]);
                float num3 = 0.3926991f;
                if (this.m_LookDevOperationType == LookDevOperationType.GizmoRotationZone1)
                {
                    vector9 = vector7;
                    vector8 = this.m_LookDevConfig.gizmo.point2;
                }
                else
                {
                    vector9 = vector7;
                    vector8 = this.m_LookDevConfig.gizmo.point1;
                }
                Vector2 vector10 = vector8 - vector9;
                float magnitude = vector10.magnitude;
                float num6 = ((Mathf.Min(base.position.width, base.position.height) / this.kReferenceScale) * 2f) * 0.9f;
                if (magnitude > num6)
                {
                    Vector2 vector11 = vector9 - vector8;
                    vector11.Normalize();
                    vector9 = vector8 + ((Vector2) (vector11 * num6));
                }
                if (Event.current.shift)
                {
                    Vector3 rhs = new Vector3(-1f, 0f, vector8.x);
                    float num7 = Vector3.Dot(new Vector3(vector7.x, vector7.y, 1f), rhs);
                    float f = 0.01745329f * Vector2.Angle(new Vector2(0f, 1f), vector7 - vector8);
                    if (num7 > 0f)
                    {
                        f = 6.283185f - f;
                    }
                    f = ((int) (f / num3)) * num3;
                    Vector2 vector13 = vector7 - vector8;
                    float num9 = vector13.magnitude;
                    vector9 = vector8 + ((Vector2) (new Vector2(Mathf.Sin(f), Mathf.Cos(f)) * num9));
                }
                if (this.m_LookDevOperationType == LookDevOperationType.GizmoRotationZone1)
                {
                    this.m_LookDevConfig.gizmo.Update(vector9, vector8);
                }
                else
                {
                    this.m_LookDevConfig.gizmo.Update(vector8, vector9);
                }
                base.Repaint();
            }
            if (this.m_LookDevOperationType == LookDevOperationType.BlendFactor)
            {
                Vector2 vector14 = this.GetNormalizedCoordinates(Event.current.mousePosition, this.m_PreviewRects[2]);
                float num10 = -Vector3.Dot(new Vector3(vector14.x, vector14.y, 1f), (Vector3) this.m_LookDevConfig.gizmo.planeOrtho) / this.GetBlendFactorMaxGizmoDistance();
                this.m_LookDevConfig.dualViewBlendFactor = Mathf.Clamp(num10, -1f, 1f);
                base.Repaint();
            }
        }

        private void Initialize()
        {
            LookDevResources.Initialize();
            this.LoadLookDevConfig();
            if (this.m_PreviewUtilityContexts[0] == null)
            {
                for (int i = 0; i < 2; i++)
                {
                    this.m_PreviewUtilityContexts[i] = new PreviewContext();
                    for (int j = 0; j < 3; j++)
                    {
                        this.m_PreviewUtilityContexts[i].m_PreviewUtility[j].m_CameraFieldOfView = 30f;
                        this.m_PreviewUtilityContexts[i].m_PreviewUtility[j].m_Camera.cullingMask = ((int) 1) << Camera.PreviewCullingLayer;
                    }
                }
                if (QualitySettings.activeColorSpace == ColorSpace.Gamma)
                {
                    Debug.LogWarning("Look Dev is designed for linear color space. Currently project is set to gamma color space. This can be changed in player settings.");
                }
                if (EditorGraphicsSettings.GetCurrentTierSettings().renderingPath != RenderingPath.DeferredShading)
                {
                    Debug.LogWarning("Look Dev switched rendering mode to deferred shading for display.");
                }
                if (!Camera.main.hdr)
                {
                    Debug.LogWarning("Look Dev switched HDR mode on for display.");
                }
            }
            if (this.m_LookDevEnvLibrary.hdriList.Count == 0)
            {
                this.UpdateContextWithCurrentHDRI(LookDevResources.m_DefaultHDRI);
            }
            if (this.m_LookDevEnvWindow == null)
            {
                this.m_LookDevEnvWindow = new LookDevEnvironmentWindow(this);
            }
        }

        private bool IsOperatingGizmo()
        {
            return ((((this.m_LookDevOperationType == LookDevOperationType.BlendFactor) || (this.m_LookDevOperationType == LookDevOperationType.GizmoRotationZone1)) || (this.m_LookDevOperationType == LookDevOperationType.GizmoRotationZone2)) || (this.m_LookDevOperationType == LookDevOperationType.GizmoTranslation));
        }

        private void LoadLookDevConfig()
        {
            if (this.m_LookDevConfig == null)
            {
                LookDevConfig config = new ScriptableObjectSaveLoadHelper<LookDevConfig>("asset", SaveType.Text).Load(m_configAssetPath);
                if (config == null)
                {
                    this.m_LookDevConfig = ScriptableObject.CreateInstance<LookDevConfig>();
                }
                else
                {
                    this.m_LookDevConfig = config;
                }
            }
            this.m_LookDevConfig.SetLookDevView(this);
            this.m_LookDevConfig.UpdateCurrentObjectArray();
            if (this.m_LookDevEnvLibrary == null)
            {
                if (this.m_LookDevUserEnvLibrary != null)
                {
                    this.m_LookDevEnvLibrary = Object.Instantiate<LookDevEnvironmentLibrary>(this.m_LookDevUserEnvLibrary);
                }
                else
                {
                    this.envLibrary = null;
                }
            }
            this.m_LookDevEnvLibrary.SetLookDevView(this);
        }

        private void LoadRenderDoc()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                RenderDoc.Load();
                ShaderUtil.RecreateGfxDevice();
            }
        }

        public void OnDestroy()
        {
            this.SaveLookDevConfig();
            this.Cleanup();
        }

        public void OnDisable()
        {
            this.SaveLookDevConfig();
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnUndoRedo));
            EditorApplication.editorApplicationQuit = (UnityAction) Delegate.Remove(EditorApplication.editorApplicationQuit, new UnityAction(this.OnQuit));
        }

        public void OnEnable()
        {
            m_FirstViewGizmoColor = !EditorGUIUtility.isProSkin ? new Color32(0, 0x7f, 0xff, 0xff) : new Color32(0, 0xcc, 0xcc, 0xff);
            m_SecondViewGizmoColor = !EditorGUIUtility.isProSkin ? new Color32(0xff, 0x7f, 0, 0xff) : new Color32(0xff, 0x6b, 0x21, 0xff);
            this.LoadLookDevConfig();
            base.autoRepaintOnSceneChange = true;
            base.titleContent = base.GetLocalizedTitleContent();
            this.m_RenderdocContent = EditorGUIUtility.IconContent("renderdoc", "Capture|Capture the current view and open in RenderDoc");
            this.m_SyncLightVertical = EditorGUIUtility.IconContent("LookDevCenterLight", "Sync|Sync all light vertically with current light position in current selected HDRI");
            this.m_ResetEnvironment = EditorGUIUtility.IconContent("LookDevResetEnv", "Reset|Reset all environment");
            this.UpdateLookDevModeToggle(this.m_LookDevConfig.lookDevMode, true);
            this.m_LookDevConfig.cameraStateCommon.rotation.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_LookDevConfig.cameraStateCommon.pivot.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_LookDevConfig.cameraStateCommon.viewSize.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_LookDevConfig.cameraStateLeft.rotation.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_LookDevConfig.cameraStateLeft.pivot.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_LookDevConfig.cameraStateLeft.viewSize.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_LookDevConfig.cameraStateRight.rotation.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_LookDevConfig.cameraStateRight.pivot.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_LookDevConfig.cameraStateRight.viewSize.valueChanged.AddListener(new UnityAction(this.Repaint));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnUndoRedo));
            EditorApplication.editorApplicationQuit = (UnityAction) Delegate.Combine(EditorApplication.editorApplicationQuit, new UnityAction(this.OnQuit));
        }

        private void OnGUI()
        {
            if ((Event.current.type == EventType.Repaint) && this.m_CaptureRD)
            {
            }
            this.Initialize();
            this.CleanupDeletedHDRI();
            base.BeginWindows();
            this.m_DisplayRect = new Rect(0f, this.kLineHeight, base.position.width, base.position.height - this.kLineHeight);
            this.UpdateViewSpecific();
            this.DoToolbarGUI();
            this.HandleDragging();
            this.RenderPreview();
            this.DoControlWindow();
            this.DoAdditionalGUI();
            this.DoGizmoDebug();
            this.HandleMouseInput();
            this.HandleCamera();
            this.HandleKeyboardShortcut();
            if ((this.m_LookDevConfig.currentObject[0] == null) && (this.m_LookDevConfig.currentObject[1] == null))
            {
                Color color = GUI.color;
                GUI.color = Color.gray;
                Vector2 vector = GUI.skin.label.CalcSize(styles.sDragAndDropObjsText);
                Rect position = new Rect((this.m_DisplayRect.width * 0.5f) - (vector.x * 0.5f), (this.m_DisplayRect.height * 0.2f) - (vector.y * 0.5f), vector.x, vector.y);
                GUI.Label(position, styles.sDragAndDropObjsText);
                GUI.color = color;
            }
            base.EndWindows();
            if (Event.current.type == EventType.Repaint)
            {
                if ((this.m_LookDevEnvWindow != null) && (this.m_LookDevEnvWindow.GetCurrentSelection() != null))
                {
                    this.m_CurrentDragContext = this.GetEditionContext(Event.current.mousePosition);
                    GUI.DrawTexture(new Rect(Event.current.mousePosition.x - this.m_LookDevEnvWindow.GetSelectedPositionOffset().x, Event.current.mousePosition.y - this.m_LookDevEnvWindow.GetSelectedPositionOffset().y, 250f, 125f), LookDevResources.m_SelectionTexture, ScaleMode.ScaleToFit, true);
                }
                else
                {
                    this.m_CurrentDragContext = LookDevEditionContext.None;
                }
            }
            if ((Event.current.type == EventType.Repaint) && this.m_CaptureRD)
            {
                this.m_CaptureRD = false;
            }
        }

        private void OnQuit()
        {
            this.SaveLookDevConfig();
        }

        private void OnUndoRedo()
        {
            base.Repaint();
        }

        public static void OpenInLookDevTool(Object go)
        {
            LookDevView window = EditorWindow.GetWindow<LookDevView>();
            window.m_LookDevConfig.SetCurrentPreviewObject(go as GameObject, LookDevEditionContext.Left);
            window.m_LookDevConfig.SetCurrentPreviewObject(go as GameObject, LookDevEditionContext.Right);
            window.Frame(LookDevEditionContext.Left, false);
            window.Repaint();
        }

        private void RenderCompositing(Rect previewRect, PreviewContext previewContext0, PreviewContext previewContext1, bool dualView)
        {
            Vector4 vector = new Vector4(this.m_LookDevConfig.gizmo.center.x, this.m_LookDevConfig.gizmo.center.y, 0f, 0f);
            Vector4 vector4 = new Vector4(this.m_LookDevConfig.gizmo.point2.x, this.m_LookDevConfig.gizmo.point2.y, 0f, 0f);
            Vector4 vector7 = new Vector4(this.m_GizmoThickness, this.m_GizmoThicknessSelected, 0f, 0f);
            Vector4 vector8 = new Vector4(this.m_GizmoCircleRadius, this.m_GizmoCircleRadiusSelected, 0f, 0f);
            int index = (this.m_LookDevConfig.lookDevMode != LookDevMode.Single2) ? 0 : 1;
            int num2 = (this.m_LookDevConfig.lookDevMode != LookDevMode.Single1) ? 1 : 0;
            float y = ((this.m_LookDevConfig.lookDevContexts[index].shadingMode != -1) && (this.m_LookDevConfig.lookDevContexts[index].shadingMode != 2)) ? 0f : this.m_LookDevConfig.lookDevContexts[index].exposureValue;
            float z = ((this.m_LookDevConfig.lookDevContexts[num2].shadingMode != -1) && (this.m_LookDevConfig.lookDevContexts[num2].shadingMode != 2)) ? 0f : this.m_LookDevConfig.lookDevContexts[num2].exposureValue;
            float x = (this.m_CurrentDragContext != LookDevEditionContext.Left) ? ((this.m_CurrentDragContext != LookDevEditionContext.Right) ? 0f : -1f) : 1f;
            CubemapInfo info = this.m_LookDevEnvLibrary.hdriList[this.m_LookDevConfig.lookDevContexts[index].currentHDRIIndex];
            CubemapInfo info2 = this.m_LookDevEnvLibrary.hdriList[this.m_LookDevConfig.lookDevContexts[num2].currentHDRIIndex];
            float shadowIntensity = info.shadowInfo.shadowIntensity;
            float w = info2.shadowInfo.shadowIntensity;
            Color shadowColor = info.shadowInfo.shadowColor;
            Color color = info2.shadowInfo.shadowColor;
            Texture texture = previewContext0.m_PreviewResult[0];
            Texture texture2 = previewContext0.m_PreviewResult[1];
            Texture texture3 = previewContext0.m_PreviewResult[2];
            Texture texture4 = previewContext1.m_PreviewResult[0];
            Texture texture5 = previewContext1.m_PreviewResult[1];
            Texture texture6 = previewContext1.m_PreviewResult[2];
            Vector4 vector9 = new Vector4(this.m_LookDevConfig.dualViewBlendFactor, y, z, (this.m_LookDevConfig.currentEditionContext != LookDevEditionContext.Left) ? -1f : 1f);
            Vector4 vector10 = new Vector4(x, !this.m_LookDevConfig.enableToneMap ? -1f : 1f, shadowIntensity, w);
            Vector4 vector11 = new Vector4(1.4f, 1f, 0.5f, 0.5f);
            Vector4 vector12 = new Vector4(0f, 0f, 5.3f, 1f);
            RenderTexture active = RenderTexture.active;
            RenderTexture.active = this.m_FinalCompositionTexture;
            LookDevResources.m_LookDevCompositing.SetTexture("_Tex0Normal", texture);
            LookDevResources.m_LookDevCompositing.SetTexture("_Tex0WithoutSun", texture2);
            LookDevResources.m_LookDevCompositing.SetTexture("_Tex0Shadows", texture3);
            LookDevResources.m_LookDevCompositing.SetColor("_ShadowColor0", shadowColor);
            LookDevResources.m_LookDevCompositing.SetTexture("_Tex1Normal", texture4);
            LookDevResources.m_LookDevCompositing.SetTexture("_Tex1WithoutSun", texture5);
            LookDevResources.m_LookDevCompositing.SetTexture("_Tex1Shadows", texture6);
            LookDevResources.m_LookDevCompositing.SetColor("_ShadowColor1", color);
            LookDevResources.m_LookDevCompositing.SetVector("_CompositingParams", vector9);
            LookDevResources.m_LookDevCompositing.SetVector("_CompositingParams2", vector10);
            LookDevResources.m_LookDevCompositing.SetColor("_FirstViewColor", (Color) m_FirstViewGizmoColor);
            LookDevResources.m_LookDevCompositing.SetColor("_SecondViewColor", (Color) m_SecondViewGizmoColor);
            LookDevResources.m_LookDevCompositing.SetVector("_GizmoPosition", vector);
            LookDevResources.m_LookDevCompositing.SetVector("_GizmoZoneCenter", vector4);
            LookDevResources.m_LookDevCompositing.SetVector("_GizmoSplitPlane", this.m_LookDevConfig.gizmo.plane);
            LookDevResources.m_LookDevCompositing.SetVector("_GizmoSplitPlaneOrtho", this.m_LookDevConfig.gizmo.planeOrtho);
            LookDevResources.m_LookDevCompositing.SetFloat("_GizmoLength", this.m_LookDevConfig.gizmo.length);
            LookDevResources.m_LookDevCompositing.SetVector("_GizmoThickness", vector7);
            LookDevResources.m_LookDevCompositing.SetVector("_GizmoCircleRadius", vector8);
            LookDevResources.m_LookDevCompositing.SetFloat("_BlendFactorCircleRadius", this.m_BlendFactorCircleRadius);
            LookDevResources.m_LookDevCompositing.SetFloat("_GetBlendFactorMaxGizmoDistance", this.GetBlendFactorMaxGizmoDistance());
            LookDevResources.m_LookDevCompositing.SetFloat("_GizmoRenderMode", !this.m_ForceGizmoRenderSelector ? ((float) this.m_GizmoRenderMode) : 4f);
            LookDevResources.m_LookDevCompositing.SetVector("_ScreenRatio", this.m_ScreenRatio);
            LookDevResources.m_LookDevCompositing.SetVector("_ToneMapCoeffs1", vector11);
            LookDevResources.m_LookDevCompositing.SetVector("_ToneMapCoeffs2", vector12);
            LookDevResources.m_LookDevCompositing.SetPass((int) this.m_LookDevConfig.lookDevMode);
            DrawFullScreenQuad(new Rect(0f, 0f, previewRect.width, previewRect.height));
            RenderTexture.active = active;
            GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
            GUI.DrawTexture(previewRect, this.m_FinalCompositionTexture, ScaleMode.StretchToFill, false);
            GL.sRGBWrite = false;
        }

        private void RenderPreview()
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (this.m_LookDevConfig.rotateObjectMode)
                {
                    this.m_ObjRotationAcc = Math.Min((float) (this.m_ObjRotationAcc + (Time.deltaTime * 0.5f)), (float) 1f);
                }
                else
                {
                    this.m_ObjRotationAcc = 0f;
                }
                if (this.m_LookDevConfig.rotateEnvMode)
                {
                    this.m_EnvRotationAcc = Math.Min((float) (this.m_EnvRotationAcc + (Time.deltaTime * 0.5f)), (float) 1f);
                }
                else
                {
                    this.m_EnvRotationAcc = 0f;
                }
                this.m_CurrentObjRotationOffset = (this.m_CurrentObjRotationOffset + ((((Time.deltaTime * 360f) * 0.3f) * this.m_LookDevConfig.objRotationSpeed) * this.m_ObjRotationAcc)) % 360f;
                this.m_LookDevConfig.lookDevContexts[0].envRotation = (this.m_LookDevConfig.lookDevContexts[0].envRotation + ((((Time.deltaTime * 360f) * 0.03f) * this.m_LookDevConfig.envRotationSpeed) * this.m_EnvRotationAcc)) % 720f;
                this.m_LookDevConfig.lookDevContexts[1].envRotation = (this.m_LookDevConfig.lookDevContexts[1].envRotation + ((((Time.deltaTime * 360f) * 0.03f) * this.m_LookDevConfig.envRotationSpeed) * this.m_EnvRotationAcc)) % 720f;
                switch (this.m_LookDevConfig.lookDevMode)
                {
                    case LookDevMode.Single1:
                    case LookDevMode.Single2:
                        this.RenderPreviewSingle();
                        break;

                    case LookDevMode.SideBySide:
                        this.RenderPreviewSideBySide();
                        break;

                    case LookDevMode.Split:
                    case LookDevMode.Zone:
                        this.RenderPreviewDualView();
                        break;
                }
            }
        }

        private void RenderPreviewDualView()
        {
            this.UpdateRenderTexture(this.m_PreviewRects[2]);
            this.RenderScene(this.m_PreviewRects[2], this.m_LookDevConfig.lookDevContexts[0], this.m_PreviewUtilityContexts[0], this.m_LookDevConfig.currentObject[0], this.m_LookDevConfig.cameraState[0], false);
            this.RenderScene(this.m_PreviewRects[2], this.m_LookDevConfig.lookDevContexts[1], this.m_PreviewUtilityContexts[1], this.m_LookDevConfig.currentObject[1], this.m_LookDevConfig.cameraState[1], false);
            this.RenderCompositing(this.m_PreviewRects[2], this.m_PreviewUtilityContexts[0], this.m_PreviewUtilityContexts[1], true);
        }

        private void RenderPreviewSideBySide()
        {
            this.UpdateRenderTexture(this.m_PreviewRects[2]);
            this.RenderScene(this.m_PreviewRects[0], this.m_LookDevConfig.lookDevContexts[0], this.m_PreviewUtilityContexts[0], this.m_LookDevConfig.currentObject[0], this.m_LookDevConfig.cameraState[0], false);
            this.RenderScene(this.m_PreviewRects[1], this.m_LookDevConfig.lookDevContexts[1], this.m_PreviewUtilityContexts[1], this.m_LookDevConfig.currentObject[1], this.m_LookDevConfig.cameraState[1], true);
            this.RenderCompositing(this.m_PreviewRects[2], this.m_PreviewUtilityContexts[0], this.m_PreviewUtilityContexts[1], true);
        }

        private void RenderPreviewSingle()
        {
            int index = (this.m_LookDevConfig.lookDevMode != LookDevMode.Single1) ? 1 : 0;
            this.UpdateRenderTexture(this.m_PreviewRects[2]);
            this.RenderScene(this.m_PreviewRects[2], this.m_LookDevConfig.lookDevContexts[index], this.m_PreviewUtilityContexts[index], this.m_LookDevConfig.currentObject[index], this.m_LookDevConfig.cameraState[index], false);
            this.RenderCompositing(this.m_PreviewRects[2], this.m_PreviewUtilityContexts[index], this.m_PreviewUtilityContexts[index], false);
        }

        private void RenderScene(Rect previewRect, LookDevContext lookDevContext, PreviewContext previewUtilityContext, GameObject currentObject, CameraState originalCameraState, bool secondView)
        {
            bool flag = !this.m_LookDevConfig.enableShadowCubemap || ((this.m_LookDevConfig.enableShadowCubemap && (lookDevContext.shadingMode != -1)) && (lookDevContext.shadingMode != 2));
            previewUtilityContext.m_PreviewResult[2] = !flag ? this.RenderScene(previewRect, lookDevContext, previewUtilityContext, currentObject, originalCameraState, null, PreviewContext.PreviewContextPass.kShadow, secondView) : Texture2D.whiteTexture;
            CubemapInfo cubemapInfo = this.m_LookDevEnvLibrary.hdriList[lookDevContext.currentHDRIIndex];
            previewUtilityContext.m_PreviewResult[0] = this.RenderScene(previewRect, lookDevContext, previewUtilityContext, currentObject, originalCameraState, cubemapInfo, PreviewContext.PreviewContextPass.kView, secondView);
            previewUtilityContext.m_PreviewResult[1] = this.RenderScene(previewRect, lookDevContext, previewUtilityContext, currentObject, originalCameraState, cubemapInfo.cubemapShadowInfo, PreviewContext.PreviewContextPass.kViewWithShadow, secondView);
        }

        private Texture RenderScene(Rect previewRect, LookDevContext lookDevContext, PreviewContext previewUtilityContext, GameObject currentObject, CameraState originalCameraState, CubemapInfo cubemapInfo, PreviewContext.PreviewContextPass contextPass, bool secondView)
        {
            PreviewRenderUtility utility = previewUtilityContext.m_PreviewUtility[(int) contextPass];
            PreviewContextCB tcb = previewUtilityContext.m_PreviewCB[(int) contextPass];
            utility.BeginPreviewHDR(previewRect, styles.sBigTitleInnerStyle);
            bool flag = contextPass == PreviewContext.PreviewContextPass.kShadow;
            DrawCameraMode shadingMode = (DrawCameraMode) lookDevContext.shadingMode;
            bool flag2 = (shadingMode != DrawCameraMode.Normal) && (shadingMode != DrawCameraMode.TexturedWire);
            float shadowDistance = QualitySettings.shadowDistance;
            Vector3 vector = QualitySettings.shadowCascade4Split;
            float angleOffset = this.m_LookDevEnvLibrary.hdriList[lookDevContext.currentHDRIIndex].angleOffset;
            float y = -(lookDevContext.envRotation + angleOffset);
            CameraState state = originalCameraState.Clone();
            Vector3 eulerAngles = state.rotation.value.eulerAngles;
            state.rotation.value = Quaternion.Euler(eulerAngles + new Vector3(0f, y, 0f));
            state.pivot.value = new Vector3(0f, this.kDefaultSceneHeight, 0f);
            state.UpdateCamera(utility.m_Camera);
            utility.m_Camera.renderingPath = RenderingPath.DeferredShading;
            utility.m_Camera.clearFlags = !flag ? CameraClearFlags.Skybox : CameraClearFlags.Color;
            utility.m_Camera.backgroundColor = Color.white;
            utility.m_Camera.hdr = true;
            for (int i = 0; i < 2; i++)
            {
                utility.m_Light[i].enabled = false;
                utility.m_Light[i].intensity = 0f;
                utility.m_Light[i].shadows = LightShadows.None;
            }
            if (((currentObject != null) && flag) && (this.m_LookDevConfig.enableShadowCubemap && !flag2))
            {
                Bounds bounds = new Bounds(currentObject.transform.position, Vector3.zero);
                this.GetRenderableBoundsRecurse(ref bounds, currentObject);
                float num5 = Mathf.Max(bounds.max.x, Mathf.Max(bounds.max.y, bounds.max.z));
                float num6 = (this.m_LookDevConfig.shadowDistance <= 0f) ? (25f * num5) : this.m_LookDevConfig.shadowDistance;
                float num7 = Mathf.Min((float) (num5 * 2f), (float) 20f) / num6;
                QualitySettings.shadowDistance = num6;
                QualitySettings.shadowCascade4Split = new Vector3(Mathf.Clamp(num7, 0f, 1f), Mathf.Clamp((float) (num7 * 2f), (float) 0f, (float) 1f), Mathf.Clamp((float) (num7 * 6f), (float) 0f, (float) 1f));
                ShadowInfo shadowInfo = this.m_LookDevEnvLibrary.hdriList[lookDevContext.currentHDRIIndex].shadowInfo;
                utility.m_Light[0].intensity = 1f;
                utility.m_Light[0].color = Color.white;
                utility.m_Light[0].shadows = LightShadows.Soft;
                utility.m_Light[0].shadowBias = this.m_DirBias;
                utility.m_Light[0].shadowNormalBias = this.m_DirNormalBias;
                utility.m_Light[0].transform.rotation = Quaternion.Euler(shadowInfo.latitude, shadowInfo.longitude, 0f);
                tcb.m_patchGBufferCB.Clear();
                RenderTargetIdentifier[] colors = new RenderTargetIdentifier[] { 10, 11 };
                tcb.m_patchGBufferCB.SetRenderTarget(colors, 2);
                tcb.m_patchGBufferCB.DrawMesh(LookDevResources.m_ScreenQuadMesh, Matrix4x4.identity, LookDevResources.m_GBufferPatchMaterial);
                utility.m_Camera.AddCommandBuffer(CameraEvent.AfterGBuffer, tcb.m_patchGBufferCB);
                if (this.m_LookDevConfig.showBalls)
                {
                    tcb.m_drawBallCB.Clear();
                    RenderTargetIdentifier[] identifierArray2 = new RenderTargetIdentifier[] { 2 };
                    tcb.m_drawBallCB.SetRenderTarget(identifierArray2, 2);
                    tcb.m_drawBallPB.SetVector("_WindowsSize", new Vector4((float) utility.m_Camera.pixelWidth, (float) utility.m_Camera.pixelHeight, !secondView ? 0f : 1f, 0f));
                    tcb.m_drawBallCB.DrawMesh(LookDevResources.m_ScreenQuadMesh, Matrix4x4.identity, LookDevResources.m_DrawBallsMaterial, 0, 1, tcb.m_drawBallPB);
                    utility.m_Camera.AddCommandBuffer(CameraEvent.AfterLighting, tcb.m_drawBallCB);
                }
            }
            Color ambient = new Color(0f, 0f, 0f, 0f);
            DefaultReflectionMode defaultReflectionMode = RenderSettings.defaultReflectionMode;
            AmbientMode ambientMode = RenderSettings.ambientMode;
            Cubemap customReflection = RenderSettings.customReflection;
            Material skybox = RenderSettings.skybox;
            float ambientIntensity = RenderSettings.ambientIntensity;
            SphericalHarmonicsL2 ambientProbe = RenderSettings.ambientProbe;
            float reflectionIntensity = RenderSettings.reflectionIntensity;
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
            Cubemap texture = (cubemapInfo == null) ? null : cubemapInfo.cubemap;
            LookDevResources.m_SkyboxMaterial.SetTexture("_Tex", texture);
            LookDevResources.m_SkyboxMaterial.SetFloat("_Exposure", 1f);
            RenderSettings.customReflection = texture;
            if (((cubemapInfo != null) && !cubemapInfo.alreadyComputed) && !flag)
            {
                RenderSettings.skybox = LookDevResources.m_SkyboxMaterial;
                DynamicGI.UpdateEnvironment();
                cubemapInfo.ambientProbe = RenderSettings.ambientProbe;
                RenderSettings.skybox = skybox;
                cubemapInfo.alreadyComputed = true;
            }
            RenderSettings.ambientProbe = (cubemapInfo == null) ? LookDevResources.m_ZeroAmbientProbe : cubemapInfo.ambientProbe;
            RenderSettings.skybox = LookDevResources.m_SkyboxMaterial;
            RenderSettings.ambientIntensity = 1f;
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.reflectionIntensity = 1f;
            if ((contextPass == PreviewContext.PreviewContextPass.kView) && this.m_LookDevConfig.showBalls)
            {
                Vector4[] outCoefficients = new Vector4[7];
                this.GetShaderConstantsFromNormalizedSH(RenderSettings.ambientProbe, outCoefficients);
                tcb.m_drawBallCB.Clear();
                RenderTargetIdentifier[] identifierArray3 = new RenderTargetIdentifier[] { 10, 11, 12, 2 };
                tcb.m_drawBallCB.SetRenderTarget(identifierArray3, 2);
                tcb.m_drawBallPB.SetVector("_SHAr", outCoefficients[0]);
                tcb.m_drawBallPB.SetVector("_SHAg", outCoefficients[1]);
                tcb.m_drawBallPB.SetVector("_SHAb", outCoefficients[2]);
                tcb.m_drawBallPB.SetVector("_SHBr", outCoefficients[3]);
                tcb.m_drawBallPB.SetVector("_SHBg", outCoefficients[4]);
                tcb.m_drawBallPB.SetVector("_SHBb", outCoefficients[5]);
                tcb.m_drawBallPB.SetVector("_SHC", outCoefficients[6]);
                tcb.m_drawBallPB.SetVector("_WindowsSize", new Vector4((float) utility.m_Camera.pixelWidth, (float) utility.m_Camera.pixelHeight, !secondView ? 0f : 1f, 0f));
                tcb.m_drawBallCB.DrawMesh(LookDevResources.m_ScreenQuadMesh, Matrix4x4.identity, LookDevResources.m_DrawBallsMaterial, 0, 0, tcb.m_drawBallPB);
                utility.m_Camera.AddCommandBuffer(CameraEvent.AfterGBuffer, tcb.m_drawBallCB);
            }
            InternalEditorUtility.SetCustomLighting(utility.m_Light, ambient);
            bool fog = RenderSettings.fog;
            Unsupported.SetRenderSettingsUseFogNoDirty(false);
            Vector3 zero = Vector3.zero;
            Vector3 localPosition = Vector3.zero;
            if (currentObject != null)
            {
                LODGroup component = currentObject.GetComponent(typeof(LODGroup)) as LODGroup;
                if (component != null)
                {
                    component.ForceLOD(lookDevContext.lodIndex);
                }
                this.m_LookDevConfig.SetEnabledRecursive(currentObject, true);
                zero = currentObject.transform.eulerAngles;
                localPosition = currentObject.transform.localPosition;
                currentObject.transform.position = new Vector3(0f, this.kDefaultSceneHeight, 0f);
                currentObject.transform.rotation = Quaternion.identity;
                currentObject.transform.Rotate((float) 0f, y, (float) 0f);
                currentObject.transform.Translate(-originalCameraState.pivot.value);
                currentObject.transform.Rotate((float) 0f, this.m_CurrentObjRotationOffset, (float) 0f);
            }
            if ((shadingMode == DrawCameraMode.TexturedWire) && !flag)
            {
                Handles.ClearCamera(previewRect, utility.m_Camera);
                Handles.DrawCamera(previewRect, utility.m_Camera, shadingMode);
            }
            else
            {
                utility.m_Camera.Render();
            }
            if (currentObject != null)
            {
                currentObject.transform.eulerAngles = zero;
                currentObject.transform.position = localPosition;
                this.m_LookDevConfig.SetEnabledRecursive(currentObject, false);
            }
            if ((flag2 && !flag) && (Event.current.type == EventType.Repaint))
            {
                float scaleFactor = utility.GetScaleFactor(previewRect.width, previewRect.height);
                LookDevResources.m_DeferredOverlayMaterial.SetInt("_DisplayMode", ((int) shadingMode) - 8);
                Graphics.DrawTexture(new Rect(0f, 0f, previewRect.width * scaleFactor, previewRect.height * scaleFactor), EditorGUIUtility.whiteTexture, LookDevResources.m_DeferredOverlayMaterial);
            }
            if (flag)
            {
                utility.m_Camera.RemoveCommandBuffer(CameraEvent.AfterGBuffer, tcb.m_patchGBufferCB);
                if (this.m_LookDevConfig.showBalls)
                {
                    utility.m_Camera.RemoveCommandBuffer(CameraEvent.AfterLighting, tcb.m_drawBallCB);
                }
            }
            else if ((contextPass == PreviewContext.PreviewContextPass.kView) && this.m_LookDevConfig.showBalls)
            {
                utility.m_Camera.RemoveCommandBuffer(CameraEvent.AfterGBuffer, tcb.m_drawBallCB);
            }
            QualitySettings.shadowCascade4Split = vector;
            QualitySettings.shadowDistance = shadowDistance;
            RenderSettings.defaultReflectionMode = defaultReflectionMode;
            RenderSettings.ambientMode = ambientMode;
            RenderSettings.customReflection = customReflection;
            RenderSettings.skybox = skybox;
            RenderSettings.ambientIntensity = ambientIntensity;
            RenderSettings.reflectionIntensity = reflectionIntensity;
            RenderSettings.ambientProbe = ambientProbe;
            Unsupported.SetRenderSettingsUseFogNoDirty(fog);
            InternalEditorUtility.RemoveCustomLighting();
            return utility.EndPreview();
        }

        public void ResetView()
        {
            Undo.RecordObject(this.m_LookDevConfig, "Reset View");
            Object.DestroyImmediate(this.m_LookDevConfig);
            this.m_LookDevConfig = ScriptableObject.CreateInstance<LookDevConfig>();
            this.m_LookDevConfig.SetLookDevView(this);
            this.UpdateLookDevModeToggle(this.m_LookDevConfig.lookDevMode, true);
        }

        public void SaveLookDevConfig()
        {
            ScriptableObjectSaveLoadHelper<LookDevConfig> helper = new ScriptableObjectSaveLoadHelper<LookDevConfig>("asset", SaveType.Text);
            if (this.m_LookDevConfig != null)
            {
                helper.Save(this.m_LookDevConfig, m_configAssetPath);
            }
        }

        public bool SaveLookDevLibrary()
        {
            if (this.m_LookDevUserEnvLibrary != null)
            {
                EditorUtility.CopySerialized(this.m_LookDevEnvLibrary, this.m_LookDevUserEnvLibrary);
                EditorUtility.SetDirty(this.m_LookDevEnvLibrary);
                return true;
            }
            string str = EditorUtility.SaveFilePanelInProject("Save New Environment Library", "New Env Library", "asset", "");
            if (!string.IsNullOrEmpty(str))
            {
                this.CreateNewLibrary(str);
                return true;
            }
            return false;
        }

        public void Update()
        {
            if ((this.m_ObjRotationAcc > 0f) || (this.m_EnvRotationAcc > 0f))
            {
                base.Repaint();
            }
        }

        private void UpdateContextWithCurrentHDRI(Cubemap cubemap)
        {
            <UpdateContextWithCurrentHDRI>c__AnonStorey0 storey = new <UpdateContextWithCurrentHDRI>c__AnonStorey0 {
                cubemap = cubemap
            };
            bool recordUndo = storey.cubemap != LookDevResources.m_DefaultHDRI;
            int num = this.m_LookDevEnvLibrary.hdriList.FindIndex(new Predicate<CubemapInfo>(storey.<>m__0));
            if (num == -1)
            {
                this.m_LookDevEnvLibrary.InsertHDRI(storey.cubemap);
                num = this.m_LookDevEnvLibrary.hdriList.Count - 1;
            }
            this.m_LookDevConfig.UpdateIntProperty(LookDevProperty.HDRI, num, recordUndo);
        }

        public void UpdateFocus(Vector2 position)
        {
            this.m_LookDevConfig.UpdateFocus(this.GetEditionContext(position));
        }

        public void UpdateLookDevModeToggle(LookDevMode lookDevMode, bool value)
        {
            LookDevMode mode = lookDevMode;
            if (value)
            {
                this.m_LookDevModeToggles[(int) lookDevMode] = value;
                for (int i = 0; i < 5; i++)
                {
                    if (i != lookDevMode)
                    {
                        this.m_LookDevModeToggles[i] = false;
                    }
                }
                mode = lookDevMode;
            }
            else
            {
                for (int j = 0; j < 5; j++)
                {
                    if (this.m_LookDevModeToggles[j])
                    {
                        mode = (LookDevMode) j;
                    }
                }
                this.m_LookDevModeToggles[(int) lookDevMode] = true;
                mode = lookDevMode;
            }
            this.m_LookDevConfig.lookDevMode = mode;
            base.Repaint();
        }

        private void UpdatePreviewRects(Rect previewRect)
        {
            this.m_PreviewRects[2] = new Rect(previewRect);
            if (this.m_ShowLookDevEnvWindow)
            {
                this.m_PreviewRects[2].width -= this.ComputeLookDevEnvWindowWidth();
            }
            this.m_PreviewRects[0] = new Rect(this.m_PreviewRects[2].x, this.m_PreviewRects[2].y, this.m_PreviewRects[2].width / 2f, this.m_PreviewRects[2].height);
            this.m_PreviewRects[1] = new Rect(this.m_PreviewRects[2].width / 2f, this.m_PreviewRects[2].y, this.m_PreviewRects[2].width / 2f, this.m_PreviewRects[2].height);
        }

        private void UpdateRenderTexture(Rect rect)
        {
            int width = (int) rect.width;
            int height = (int) rect.height;
            if (((this.m_FinalCompositionTexture == null) || (this.m_FinalCompositionTexture.width != width)) || (this.m_FinalCompositionTexture.height != height))
            {
                if (this.m_FinalCompositionTexture != null)
                {
                    Object.DestroyImmediate(this.m_FinalCompositionTexture);
                    this.m_FinalCompositionTexture = null;
                }
                this.m_FinalCompositionTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
                this.m_FinalCompositionTexture.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        private void UpdateViewSpecific()
        {
            this.UpdatePreviewRects(this.m_DisplayRect);
            this.m_ScreenRatio.Set(this.m_PreviewRects[2].width / this.kReferenceScale, this.m_PreviewRects[2].height / this.kReferenceScale, this.m_PreviewRects[2].width, this.m_PreviewRects[2].height);
            int num = 4;
            float width = 292f;
            float height = (this.kLineHeight * num) + EditorGUIUtility.standardVerticalSpacing;
            this.m_ControlWindowRect = new Rect((this.m_PreviewRects[2].width / 2f) - (width / 2f), (this.m_PreviewRects[2].height - height) - 10f, width, height);
        }

        public LookDevConfig config
        {
            get
            {
                return this.m_LookDevConfig;
            }
        }

        public LookDevEnvironmentLibrary envLibrary
        {
            get
            {
                return this.m_LookDevEnvLibrary;
            }
            set
            {
                if (value == null)
                {
                    this.m_LookDevEnvLibrary = ScriptableObject.CreateInstance<LookDevEnvironmentLibrary>();
                    this.m_LookDevUserEnvLibrary = null;
                }
                else if (value != this.m_LookDevUserEnvLibrary)
                {
                    this.m_LookDevUserEnvLibrary = value;
                    this.m_LookDevEnvLibrary = Object.Instantiate<LookDevEnvironmentLibrary>(value);
                    this.m_LookDevEnvLibrary.SetLookDevView(this);
                }
                int hdriCount = this.m_LookDevEnvLibrary.hdriCount;
                if ((this.m_LookDevConfig.GetIntProperty(LookDevProperty.HDRI, LookDevEditionContext.Left) >= hdriCount) || (this.m_LookDevConfig.GetIntProperty(LookDevProperty.HDRI, LookDevEditionContext.Right) >= hdriCount))
                {
                    this.m_LookDevConfig.UpdatePropertyLink(LookDevProperty.HDRI, true);
                    this.m_LookDevConfig.UpdateIntProperty(LookDevProperty.HDRI, 0);
                }
            }
        }

        public int hotControl
        {
            get
            {
                return this.m_hotControlID;
            }
        }

        public static Styles styles
        {
            get
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                return s_Styles;
            }
        }

        public LookDevEnvironmentLibrary userEnvLibrary
        {
            get
            {
                return this.m_LookDevUserEnvLibrary;
            }
        }

        [CompilerGenerated]
        private sealed class <UpdateContextWithCurrentHDRI>c__AnonStorey0
        {
            internal Cubemap cubemap;

            internal bool <>m__0(CubemapInfo x)
            {
                return (x.cubemap == this.cubemap);
            }
        }

        private class PreviewContext
        {
            public LookDevView.PreviewContextCB[] m_PreviewCB = new LookDevView.PreviewContextCB[3];
            public Texture[] m_PreviewResult = new Texture[3];
            public PreviewRenderUtility[] m_PreviewUtility = new PreviewRenderUtility[3];

            public PreviewContext()
            {
                for (int i = 0; i < 3; i++)
                {
                    this.m_PreviewUtility[i] = new PreviewRenderUtility(true);
                    this.m_PreviewCB[i] = new LookDevView.PreviewContextCB();
                }
            }

            public enum PreviewContextPass
            {
                kView,
                kViewWithShadow,
                kShadow,
                kCount
            }
        }

        private class PreviewContextCB
        {
            public CommandBuffer m_drawBallCB = new CommandBuffer();
            public MaterialPropertyBlock m_drawBallPB;
            public CommandBuffer m_patchGBufferCB;

            public PreviewContextCB()
            {
                this.m_drawBallCB.name = "draw ball";
                this.m_patchGBufferCB = new CommandBuffer();
                this.m_patchGBufferCB.name = "patch gbuffer";
                this.m_drawBallPB = new MaterialPropertyBlock();
            }
        }

        public class Styles
        {
            public readonly GUIStyle sBigTitleInnerStyle = "IN BigTitle inner";
            public readonly GUIContent sDragAndDropObjsText = EditorGUIUtility.TextContent("Drag and drop Prefabs here.");
            public readonly GUIContent sLinkActive = EditorGUIUtility.IconContent("LookDevMirrorViewsActive", "Link|Links the property between the different views");
            public readonly GUIContent sLinkInactive = EditorGUIUtility.IconContent("LookDevMirrorViewsInactive", "Link|Links the property between the different views");
            public readonly GUIStyle[] sPropertyLabelStyle = new GUIStyle[] { new GUIStyle(EditorStyles.miniLabel), new GUIStyle(EditorStyles.miniLabel), new GUIStyle(EditorStyles.miniLabel) };
            public readonly GUIContent sSideBySideMode = EditorGUIUtility.IconContent("LookDevSideBySide", "Side|Side by side comparison view");
            public readonly GUIContent sSingleMode1 = EditorGUIUtility.IconContent("LookDevSingle1", "Single1|Single1 object view");
            public readonly GUIContent sSingleMode2 = EditorGUIUtility.IconContent("LookDevSingle2", "Single2|Single2 object view");
            public readonly GUIContent sSplitMode = EditorGUIUtility.IconContent("LookDevSplit", "Split|Single object split comparison view");
            public readonly GUIStyle sToolBarButton = "toolbarbutton";
            public readonly GUIContent sZoneMode = EditorGUIUtility.IconContent("LookDevZone", "Zone|Single object zone comparison view");

            public Styles()
            {
                this.sPropertyLabelStyle[0].normal.textColor = (Color) LookDevView.m_FirstViewGizmoColor;
                this.sPropertyLabelStyle[1].normal.textColor = (Color) LookDevView.m_SecondViewGizmoColor;
            }
        }
    }
}

