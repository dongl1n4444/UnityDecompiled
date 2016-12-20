namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class LookDevViewsWindow : PopupWindowContent
    {
        private static float kIconSize = 32f;
        private static float kLabelWidth = 120f;
        private static float kLineHeight = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        private static float kSliderFieldPadding = 5f;
        private static float kSliderFieldWidth = 30f;
        private static float kSliderWidth = 100f;
        private readonly LookDevView m_LookDevView;
        private float m_WindowHeight = ((5f * kLineHeight) + EditorGUIUtility.standardVerticalSpacing);
        private float m_WindowWidth = ((((kLabelWidth + kSliderWidth) + kSliderFieldWidth) + kSliderFieldPadding) + 5f);
        private static Styles s_Styles = new Styles();

        public LookDevViewsWindow(LookDevView lookDevView)
        {
            this.m_LookDevView = lookDevView;
        }

        private void DrawOneView(Rect drawPos, LookDevEditionContext context)
        {
            int index = (int) context;
            bool flag = ((this.m_LookDevView.config.lookDevMode != LookDevMode.Single1) && (context == LookDevEditionContext.Left)) || ((this.m_LookDevView.config.lookDevMode != LookDevMode.Single2) && (context == LookDevEditionContext.Right));
            GUILayout.BeginArea(drawPos);
            GUILayout.Label(styles.sViewTitle[index], styles.sViewTitleStyles[index], new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(this.m_WindowWidth) };
            GUILayout.BeginVertical(options);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Height(kLineHeight) };
            GUILayout.BeginHorizontal(optionArray2);
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(kLabelWidth) };
            GUILayout.Label(styles.sExposure, styles.sMenuItem, optionArray3);
            float floatProperty = this.m_LookDevView.config.GetFloatProperty(LookDevProperty.ExposureValue, context);
            EditorGUI.BeginChangeCheck();
            float max = Mathf.Round(this.m_LookDevView.config.exposureRange);
            GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.Width(kSliderWidth) };
            floatProperty = Mathf.Clamp(GUILayout.HorizontalSlider(floatProperty, -max, max, optionArray4), -max, max);
            GUILayoutOption[] optionArray5 = new GUILayoutOption[] { GUILayout.Width(kSliderFieldWidth) };
            floatProperty = Mathf.Clamp(EditorGUILayout.FloatField((float) Math.Round((double) floatProperty, (floatProperty >= 0f) ? 2 : 1), optionArray5), -max, max);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_LookDevView.config.UpdateFocus(context);
                this.m_LookDevView.config.UpdateFloatProperty(LookDevProperty.ExposureValue, floatProperty);
            }
            GUILayout.EndHorizontal();
            GUILayoutOption[] optionArray6 = new GUILayoutOption[] { GUILayout.Height(kLineHeight) };
            GUILayout.BeginHorizontal(optionArray6);
            int num4 = -1;
            int hdriCount = this.m_LookDevView.envLibrary.hdriCount;
            using (new EditorGUI.DisabledScope(hdriCount <= 1))
            {
                GUILayoutOption[] optionArray7 = new GUILayoutOption[] { GUILayout.Width(kLabelWidth) };
                GUILayout.Label(styles.sEnvironment, styles.sMenuItem, optionArray7);
                if (hdriCount > 1)
                {
                    int num6 = hdriCount - 1;
                    num4 = this.m_LookDevView.config.GetIntProperty(LookDevProperty.HDRI, context);
                    EditorGUI.BeginChangeCheck();
                    GUILayoutOption[] optionArray8 = new GUILayoutOption[] { GUILayout.Width(kSliderWidth) };
                    num4 = (int) GUILayout.HorizontalSlider((float) num4, 0f, (float) num6, optionArray8);
                    GUILayoutOption[] optionArray9 = new GUILayoutOption[] { GUILayout.Width(kSliderFieldWidth) };
                    num4 = Mathf.Clamp(EditorGUILayout.IntField(num4, optionArray9), 0, num6);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.m_LookDevView.config.UpdateFocus(context);
                        this.m_LookDevView.config.UpdateIntProperty(LookDevProperty.HDRI, num4);
                    }
                }
                else
                {
                    GUILayoutOption[] optionArray10 = new GUILayoutOption[] { GUILayout.Width(kSliderWidth) };
                    GUILayout.HorizontalSlider(0f, 0f, 0f, optionArray10);
                    GUILayout.Label(styles.sZero, styles.sMenuItem, new GUILayoutOption[0]);
                }
            }
            GUILayout.EndHorizontal();
            GUILayoutOption[] optionArray11 = new GUILayoutOption[] { GUILayout.Height(kLineHeight) };
            GUILayout.BeginHorizontal(optionArray11);
            GUILayoutOption[] optionArray12 = new GUILayoutOption[] { GUILayout.Width(kLabelWidth) };
            GUILayout.Label(styles.sShadingMode, styles.sMenuItem, optionArray12);
            int intProperty = this.m_LookDevView.config.GetIntProperty(LookDevProperty.ShadingMode, context);
            EditorGUI.BeginChangeCheck();
            GUILayoutOption[] optionArray13 = new GUILayoutOption[] { GUILayout.Width((kSliderFieldWidth + kSliderWidth) + 4f) };
            intProperty = EditorGUILayout.IntPopup("", intProperty, styles.sShadingModeStrings, styles.sShadingModeValues, optionArray13);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_LookDevView.config.UpdateFocus(context);
                this.m_LookDevView.config.UpdateIntProperty(LookDevProperty.ShadingMode, intProperty);
            }
            GUILayout.EndHorizontal();
            GUILayoutOption[] optionArray14 = new GUILayoutOption[] { GUILayout.Height(kLineHeight) };
            GUILayout.BeginHorizontal(optionArray14);
            GUILayoutOption[] optionArray15 = new GUILayoutOption[] { GUILayout.Width(kLabelWidth) };
            GUILayout.Label(styles.sRotation, styles.sMenuItem, optionArray15);
            float num8 = this.m_LookDevView.config.GetFloatProperty(LookDevProperty.EnvRotation, context);
            EditorGUI.BeginChangeCheck();
            GUILayoutOption[] optionArray16 = new GUILayoutOption[] { GUILayout.Width(kSliderWidth) };
            GUILayoutOption[] optionArray17 = new GUILayoutOption[] { GUILayout.Width(kSliderFieldWidth) };
            num8 = Mathf.Clamp(EditorGUILayout.FloatField((float) Math.Round((double) GUILayout.HorizontalSlider(num8, 0f, 720f, optionArray16), 0), optionArray17), 0f, 720f);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_LookDevView.config.UpdateFocus(context);
                this.m_LookDevView.config.UpdateFloatProperty(LookDevProperty.EnvRotation, num8);
            }
            GUILayout.EndHorizontal();
            if (this.NeedLoD())
            {
                GUILayoutOption[] optionArray18 = new GUILayoutOption[] { GUILayout.Height(kLineHeight) };
                GUILayout.BeginHorizontal(optionArray18);
                if (this.m_LookDevView.config.GetObjectLoDCount(context) > 1)
                {
                    int num9 = this.m_LookDevView.config.GetIntProperty(LookDevProperty.LoDIndex, context);
                    GUILayoutOption[] optionArray19 = new GUILayoutOption[] { GUILayout.Width(kLabelWidth) };
                    GUILayout.Label((num9 != -1) ? styles.sLoD : styles.sLoDAuto, styles.sMenuItem, optionArray19);
                    EditorGUI.BeginChangeCheck();
                    int num10 = this.m_LookDevView.config.GetObjectLoDCount(context) - 1;
                    if (((this.m_LookDevView.config.lookDevMode != LookDevMode.Single1) && (this.m_LookDevView.config.lookDevMode != LookDevMode.Single2)) && this.m_LookDevView.config.IsPropertyLinked(LookDevProperty.LoDIndex))
                    {
                        num10 = Math.Min(this.m_LookDevView.config.GetObjectLoDCount(LookDevEditionContext.Left), this.m_LookDevView.config.GetObjectLoDCount(LookDevEditionContext.Right)) - 1;
                    }
                    GUILayoutOption[] optionArray20 = new GUILayoutOption[] { GUILayout.Width(kSliderWidth) };
                    num9 = (int) GUILayout.HorizontalSlider((float) Mathf.Clamp(num9, -1, num10), -1f, (float) num10, optionArray20);
                    GUILayoutOption[] optionArray21 = new GUILayoutOption[] { GUILayout.Width(kSliderFieldWidth) };
                    num9 = EditorGUILayout.IntField(num9, optionArray21);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.m_LookDevView.config.UpdateFocus(context);
                        this.m_LookDevView.config.UpdateIntProperty(LookDevProperty.LoDIndex, num9);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            if (flag)
            {
                GUILayoutOption[] optionArray22 = new GUILayoutOption[] { GUILayout.Width(kIconSize) };
                GUILayout.BeginVertical(optionArray22);
                LookDevProperty[] propertyArray = new LookDevProperty[] { LookDevProperty.ExposureValue };
                int num11 = 4 + (!this.NeedLoD() ? 0 : 1);
                for (int i = 0; i < num11; i++)
                {
                    bool flag2 = false;
                    EditorGUI.BeginChangeCheck();
                    bool flag3 = this.m_LookDevView.config.IsPropertyLinked(propertyArray[i]);
                    GUILayoutOption[] optionArray23 = new GUILayoutOption[] { GUILayout.Height(kLineHeight) };
                    flag2 = GUILayout.Toggle(flag3, this.GetGUIContentLink(flag3), styles.sToolBarButton, optionArray23);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.m_LookDevView.config.UpdatePropertyLink(propertyArray[i], flag2);
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private GUIContent GetGUIContentLink(bool active)
        {
            return (!active ? styles.sLinkInactive : styles.sLinkActive);
        }

        private float GetHeight()
        {
            float windowHeight = this.m_WindowHeight;
            if (this.NeedLoD())
            {
                windowHeight += kLineHeight;
            }
            return windowHeight;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(this.m_WindowWidth + (((this.m_LookDevView.config.lookDevMode != LookDevMode.Single1) && (this.m_LookDevView.config.lookDevMode != LookDevMode.Single2)) ? (this.m_WindowWidth + kIconSize) : 0f), this.GetHeight());
        }

        private bool NeedLoD()
        {
            return ((this.m_LookDevView.config.GetObjectLoDCount(LookDevEditionContext.Left) > 1) || (this.m_LookDevView.config.GetObjectLoDCount(LookDevEditionContext.Right) > 1));
        }

        public override void OnGUI(Rect rect)
        {
            if (this.m_LookDevView.config != null)
            {
                Rect drawPos = new Rect(0f, 0f, rect.width, this.GetHeight());
                this.DrawOneView(drawPos, (this.m_LookDevView.config.lookDevMode != LookDevMode.Single2) ? LookDevEditionContext.Left : LookDevEditionContext.Right);
                drawPos.x += this.m_WindowWidth;
                drawPos.x += kIconSize;
                this.DrawOneView(drawPos, LookDevEditionContext.Right);
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
                return s_Styles;
            }
        }

        public class Styles
        {
            public readonly GUIContent sEnvironment = EditorGUIUtility.TextContent("Environment|Select an environment from the list of currently available environments");
            public readonly GUIContent sExposure = EditorGUIUtility.TextContent("EV|Exposure value: control the brightness of the environment.");
            public readonly GUIStyle sHeaderStyle = EditorStyles.miniLabel;
            public readonly GUIContent sLinkActive = EditorGUIUtility.IconContent("LookDevMirrorViewsActive", "Link|Links the property between the different views");
            public readonly GUIContent sLinkInactive = EditorGUIUtility.IconContent("LookDevMirrorViewsInactive", "Link|Links the property between the different views");
            public readonly GUIContent sLoD = EditorGUIUtility.TextContent("LoD|Choose displayed LoD");
            public readonly GUIContent sLoDAuto = EditorGUIUtility.TextContent("LoD (auto)|Choose displayed LoD");
            public readonly GUIStyle sMenuItem = "MenuItem";
            public readonly GUIContent sRotation = EditorGUIUtility.TextContent("Rotation|Change the rotation of the environment");
            public readonly GUIContent sShadingMode = EditorGUIUtility.TextContent("Shading|Select shading mode");
            public readonly string[] sShadingModeStrings = new string[] { "Shaded", "Shaded Wireframe", "Albedo", "Specular", "Smoothness", "Normal" };
            public readonly int[] sShadingModeValues = new int[] { -1, 2, 8, 9, 10, 11 };
            public readonly GUIContent sTitle = EditorGUIUtility.TextContent("Views");
            public readonly GUIStyle sToolBarButton = "toolbarbutton";
            public readonly GUIContent[] sViewTitle = new GUIContent[] { EditorGUIUtility.TextContent("Main View (1)"), EditorGUIUtility.TextContent("Second View (2)") };
            public readonly GUIStyle[] sViewTitleStyles = new GUIStyle[] { new GUIStyle(EditorStyles.miniLabel), new GUIStyle(EditorStyles.miniLabel) };
            public readonly GUIContent sZero = EditorGUIUtility.TextContent("0");

            public Styles()
            {
                this.sViewTitleStyles[0].normal.textColor = (Color) LookDevView.m_FirstViewGizmoColor;
                this.sViewTitleStyles[1].normal.textColor = (Color) LookDevView.m_SecondViewGizmoColor;
            }
        }
    }
}

