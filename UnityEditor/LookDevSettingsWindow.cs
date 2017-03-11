namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class LookDevSettingsWindow : PopupWindowContent
    {
        private const float kIconHorizontalPadding = 3f;
        private const float kIconSize = 16f;
        private readonly LookDevView m_LookDevView;
        private readonly float m_WindowHeight = 560f;
        private const float m_WindowWidth = 180f;
        private static Styles s_Styles = null;

        public LookDevSettingsWindow(LookDevView lookDevView)
        {
            this.m_LookDevView = lookDevView;
        }

        private void DrawHeader(GUIContent label)
        {
            GUILayout.Label(label, EditorStyles.miniLabel, new GUILayoutOption[0]);
        }

        private void DrawSeparator()
        {
            GUILayout.Space(3f);
            GUILayout.Label(GUIContent.none, styles.sSeparator, new GUILayoutOption[0]);
        }

        public override Vector2 GetWindowSize() => 
            new Vector2(180f, this.m_WindowHeight);

        public override void OnGUI(Rect rect)
        {
            if (this.m_LookDevView != null)
            {
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                EditorGUIUtility.labelWidth = 130f;
                EditorGUIUtility.fieldWidth = 35f;
                this.DrawHeader(styles.sMultiView);
                for (int i = 0; i < 5; i++)
                {
                    EditorGUI.BeginChangeCheck();
                    bool flag = GUILayout.Toggle(this.m_LookDevView.config.lookDevMode == i, styles.sMultiViewMode[i], styles.sMenuItem, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.m_LookDevView.UpdateLookDevModeToggle((LookDevMode) i, flag);
                        this.m_LookDevView.Repaint();
                        GUIUtility.ExitGUI();
                    }
                }
                this.DrawSeparator();
                this.DrawHeader(styles.sCamera);
                if (GUILayout.Button(styles.sResetCamera, styles.sMenuItem, new GUILayoutOption[0]))
                {
                    this.m_LookDevView.Frame();
                }
                this.m_LookDevView.config.enableToneMap = GUILayout.Toggle(this.m_LookDevView.config.enableToneMap, styles.sEnableToneMap, styles.sMenuItem, new GUILayoutOption[0]);
                EditorGUI.BeginChangeCheck();
                float num2 = EditorGUILayout.IntSlider(styles.sExposureRange, (int) this.m_LookDevView.config.exposureRange, 1, 0x20, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(this.m_LookDevView.config, "Change exposure range");
                    this.m_LookDevView.config.exposureRange = num2;
                }
                this.DrawSeparator();
                this.DrawHeader(styles.sLighting);
                EditorGUI.BeginChangeCheck();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                this.m_LookDevView.config.enableShadowCubemap = GUILayout.Toggle(this.m_LookDevView.config.enableShadowCubemap, styles.sEnableShadows, styles.sMenuItem, new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_LookDevView.Repaint();
                }
                EditorGUI.BeginChangeCheck();
                float num3 = EditorGUILayout.Slider(styles.sShadowDistance, this.m_LookDevView.config.shadowDistance, 0f, 1000f, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(this.m_LookDevView.config, "Change shadow distance");
                    this.m_LookDevView.config.shadowDistance = num3;
                }
                this.DrawSeparator();
                this.DrawHeader(styles.sAnimation);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                this.m_LookDevView.config.rotateObjectMode = GUILayout.Toggle(this.m_LookDevView.config.rotateObjectMode, styles.sRotateObjectMode, styles.sMenuItem, new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                this.m_LookDevView.config.rotateEnvMode = GUILayout.Toggle(this.m_LookDevView.config.rotateEnvMode, styles.sRotateEnvMode, styles.sMenuItem, new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                EditorGUI.BeginChangeCheck();
                float num4 = EditorGUILayout.Slider(styles.sObjRotationSpeed, this.m_LookDevView.config.objRotationSpeed, -5f, 5f, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(this.m_LookDevView.config, "Change rotation speed");
                    this.m_LookDevView.config.objRotationSpeed = num4;
                }
                EditorGUI.BeginChangeCheck();
                float num5 = EditorGUILayout.Slider(styles.sEnvRotationSpeed, this.m_LookDevView.config.envRotationSpeed, -5f, 5f, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(this.m_LookDevView.config, "Change env speed");
                    this.m_LookDevView.config.envRotationSpeed = num5;
                }
                this.DrawSeparator();
                this.DrawHeader(styles.sViewport);
                if (GUILayout.Button(styles.sResetView, styles.sMenuItem, new GUILayoutOption[0]))
                {
                    this.m_LookDevView.ResetView();
                }
                this.DrawSeparator();
                this.DrawHeader(styles.sEnvLibrary);
                using (new EditorGUI.DisabledScope(!this.m_LookDevView.envLibrary.dirty))
                {
                    if (GUILayout.Button(styles.sSaveCurrentLibrary, styles.sMenuItem, new GUILayoutOption[0]))
                    {
                        base.editorWindow.Close();
                        if (this.m_LookDevView.SaveLookDevLibrary())
                        {
                            this.m_LookDevView.envLibrary.dirty = false;
                        }
                        GUIUtility.ExitGUI();
                    }
                }
                if (GUILayout.Button(styles.sCreateNewLibrary, styles.sMenuItem, new GUILayoutOption[0]))
                {
                    base.editorWindow.Close();
                    string str = EditorUtility.SaveFilePanelInProject("Save New Environment Library", "New Env Library", "asset", "");
                    if (!string.IsNullOrEmpty(str))
                    {
                        this.m_LookDevView.CreateNewLibrary(str);
                    }
                    GUIUtility.ExitGUI();
                }
                EditorGUI.BeginChangeCheck();
                LookDevEnvironmentLibrary library = EditorGUILayout.ObjectField(this.m_LookDevView.userEnvLibrary, typeof(LookDevEnvironmentLibrary), false, new GUILayoutOption[0]) as LookDevEnvironmentLibrary;
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_LookDevView.envLibrary = library;
                }
                this.DrawSeparator();
                this.DrawHeader(styles.sMisc);
                this.m_LookDevView.config.showBalls = GUILayout.Toggle(this.m_LookDevView.config.showBalls, styles.sShowBalls, styles.sMenuItem, new GUILayoutOption[0]);
                this.m_LookDevView.config.showControlWindows = GUILayout.Toggle(this.m_LookDevView.config.showControlWindows, styles.sShowControlWindows, styles.sMenuItem, new GUILayoutOption[0]);
                EditorGUI.BeginChangeCheck();
                bool flag2 = GUILayout.Toggle(this.m_LookDevView.config.allowDifferentObjects, styles.sAllowDifferentObjects, styles.sMenuItem, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_LookDevView.config.allowDifferentObjects = flag2;
                }
                if (GUILayout.Button(styles.sResyncObjects, styles.sMenuItem, new GUILayoutOption[0]))
                {
                    this.m_LookDevView.config.ResynchronizeObjects();
                }
                GUILayout.EndVertical();
                if (Event.current.type == EventType.MouseMove)
                {
                    Event.current.Use();
                }
                if ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape))
                {
                    base.editorWindow.Close();
                    GUIUtility.ExitGUI();
                }
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

        public class Styles
        {
            public readonly GUIContent sAllowDifferentObjects = EditorGUIUtility.TextContent("Allow Different Objects");
            public readonly GUIContent sAnimation = EditorGUIUtility.TextContent("Animation");
            public readonly GUIContent sCamera = EditorGUIUtility.TextContent("Camera");
            public readonly GUIContent sCreateNewLibrary = EditorGUIUtility.TextContent("Save as new library");
            public readonly GUIContent sEnableAutoExp = EditorGUIUtility.TextContent("Enable Auto Exposure");
            public readonly GUIContent sEnableEnvRotationIcon = EditorGUIUtility.IconContent("LookDevEnvRotation", "EnvRotation|Toggles environment rotation on and off");
            public readonly Texture sEnableEnvRotationTexture = EditorGUIUtility.FindTexture("LookDevEnvRotation");
            public readonly GUIContent sEnableObjRotationIcon = EditorGUIUtility.IconContent("LookDevObjRotation", "ObjRotation|Toggles object rotation (turntable) on and off");
            public readonly Texture sEnableObjRotationTexture = EditorGUIUtility.FindTexture("LookDevObjRotation");
            public readonly GUIContent sEnableShadowIcon = EditorGUIUtility.IconContent("LookDevShadow", "Shadow|Toggles shadows on and off");
            public readonly GUIContent sEnableShadows = EditorGUIUtility.TextContent("Enable Shadows");
            public readonly Texture sEnableShadowTexture = EditorGUIUtility.FindTexture("LookDevShadow");
            public readonly GUIContent sEnableToneMap = EditorGUIUtility.TextContent("Enable Tone Mapping");
            public readonly GUIContent sEnvLibrary = EditorGUIUtility.TextContent("Environment Library");
            public readonly GUIContent sEnvRotationSpeed = EditorGUIUtility.TextContent("Rotate Env. speed");
            public readonly GUIContent sExposureRange = EditorGUIUtility.TextContent("Exposure Range");
            public readonly GUIContent sLighting = EditorGUIUtility.TextContent("Lighting");
            public readonly GUIStyle sMenuItem = "MenuItem";
            public readonly GUIContent sMisc = EditorGUIUtility.TextContent("Misc");
            public readonly GUIContent sMultiView = EditorGUIUtility.TextContent("Multi-view");
            public readonly GUIContent[] sMultiViewMode = new GUIContent[] { EditorGUIUtility.TextContent("Single1"), EditorGUIUtility.TextContent("Single2"), EditorGUIUtility.TextContent("Side by side"), EditorGUIUtility.TextContent("Split-screen"), EditorGUIUtility.TextContent("Zone") };
            public readonly Texture[] sMultiViewTextures = new Texture[] { EditorGUIUtility.FindTexture("LookDevSingle1"), EditorGUIUtility.FindTexture("LookDevSingle2"), EditorGUIUtility.FindTexture("LookDevSideBySide"), EditorGUIUtility.FindTexture("LookDevSplit"), EditorGUIUtility.FindTexture("LookDevZone") };
            public readonly GUIContent sObjRotationSpeed = EditorGUIUtility.TextContent("Rotate Objects speed");
            public readonly GUIContent sResetCamera = EditorGUIUtility.TextContent("Fit View        F");
            public readonly GUIContent sResetView = EditorGUIUtility.TextContent("Reset View");
            public readonly GUIContent sResyncObjects = EditorGUIUtility.TextContent("Resynchronize Objects");
            public readonly GUIContent sRotateEnvMode = EditorGUIUtility.TextContent("Rotate environment");
            public readonly GUIContent sRotateObjectMode = EditorGUIUtility.TextContent("Rotate Objects");
            public readonly GUIContent sSaveCurrentLibrary = EditorGUIUtility.TextContent("Save current library");
            public readonly GUIStyle sSeparator = "sv_iconselector_sep";
            public readonly GUIContent sShadowDistance = EditorGUIUtility.TextContent("Shadow distance");
            public readonly GUIContent sShowBalls = EditorGUIUtility.TextContent("Show Chrome/grey balls");
            public readonly GUIContent sShowControlWindows = EditorGUIUtility.TextContent("Show Controls");
            public readonly GUIContent sTitle = EditorGUIUtility.TextContent("Settings");
            public readonly GUIContent sViewport = EditorGUIUtility.TextContent("Viewport");
        }

        private enum UINumElement
        {
            UINumButton = 6,
            UINumDrawHeader = 6,
            UINumSeparator = 7,
            UINumSlider = 4,
            UINumToggle = 12,
            UITotalElement = 0x23
        }
    }
}

