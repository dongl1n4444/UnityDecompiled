namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.AnimatedValues;
    using UnityEditor.Modules;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    internal sealed class PlayerSettingsSplashScreenEditor
    {
        [CompilerGenerated]
        private static ReorderableList.CanRemoveCallbackDelegate <>f__mg$cache0;
        private static readonly Color k_DarkLogoColor = new Color(0.13f, 0.17f, 0.21f);
        private static readonly float k_DefaultLogoTime = 2f;
        private static readonly float k_LogoListElementHeight = 72f;
        private static readonly float k_LogoListFooterHeight = 20f;
        private static readonly float k_LogoListLogoFieldHeight = 64f;
        private static readonly float k_LogoListPropertyLabelWidth = 100f;
        private static readonly float k_LogoListPropertyMinWidth = 230f;
        private static readonly float k_LogoListUnityLogoMaxWidth = 220f;
        private static readonly float k_LogoListUnityLogoMinWidth = 64f;
        private static readonly float k_MaxLogoTime = 10f;
        private static readonly float k_MinLogoTime = 2f;
        private static readonly float k_MinPersonalEditionOverlayOpacity = 0.5f;
        private static readonly float k_MinProEditionOverlayOpacity = 0f;
        private static readonly Texts k_Texts = new Texts();
        private ReorderableList m_LogoList;
        private PlayerSettingsEditor m_Owner;
        private SerializedProperty m_ResolutionDialogBanner;
        private readonly AnimBool m_ShowAnimationControlsAnimator = new AnimBool();
        private readonly AnimBool m_ShowBackgroundColorAnimator = new AnimBool();
        private readonly AnimBool m_ShowLogoControlsAnimator = new AnimBool();
        private SerializedProperty m_ShowUnitySplashLogo;
        private SerializedProperty m_ShowUnitySplashScreen;
        private SerializedProperty m_SplashScreenAnimation;
        private SerializedProperty m_SplashScreenBackgroundAnimationZoom;
        private SerializedProperty m_SplashScreenBackgroundColor;
        private SerializedProperty m_SplashScreenBackgroundLandscape;
        private SerializedProperty m_SplashScreenBackgroundPortrait;
        private SerializedProperty m_SplashScreenDrawMode;
        private SerializedProperty m_SplashScreenLogoAnimationZoom;
        private SerializedProperty m_SplashScreenLogos;
        private SerializedProperty m_SplashScreenLogoStyle;
        private SerializedProperty m_SplashScreenOverlayOpacity;
        private float m_TotalLogosDuration;
        private SerializedProperty m_VirtualRealitySplashScreen;
        private static Sprite s_UnityLogo;

        public PlayerSettingsSplashScreenEditor(PlayerSettingsEditor owner)
        {
            this.m_Owner = owner;
        }

        private void AddUnityLogoToLogosList()
        {
            for (int i = 0; i < this.m_SplashScreenLogos.arraySize; i++)
            {
                if (((Sprite) this.m_SplashScreenLogos.GetArrayElementAtIndex(i).FindPropertyRelative("logo").objectReferenceValue) == s_UnityLogo)
                {
                    return;
                }
            }
            this.m_SplashScreenLogos.InsertArrayElementAtIndex(0);
            SerializedProperty arrayElementAtIndex = this.m_SplashScreenLogos.GetArrayElementAtIndex(0);
            SerializedProperty property4 = arrayElementAtIndex.FindPropertyRelative("logo");
            SerializedProperty property5 = arrayElementAtIndex.FindPropertyRelative("duration");
            property4.objectReferenceValue = s_UnityLogo;
            property5.floatValue = k_DefaultLogoTime;
        }

        private void BuiltinCustomSplashScreenGUI()
        {
            EditorGUILayout.LabelField(k_Texts.splashTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
            using (new EditorGUI.DisabledScope(!licenseAllowsDisabling))
            {
                EditorGUILayout.PropertyField(this.m_ShowUnitySplashScreen, k_Texts.showSplash, new GUILayoutOption[0]);
                if (!this.m_ShowUnitySplashScreen.boolValue)
                {
                    return;
                }
            }
            if (GUI.Button(EditorGUI.PrefixLabel(GUILayoutUtility.GetRect(k_Texts.previewSplash, "button"), new GUIContent(" ")), k_Texts.previewSplash))
            {
                SplashScreen.Begin();
                GameView mainGameView = GameView.GetMainGameView();
                if (mainGameView != null)
                {
                    mainGameView.Focus();
                }
                GameView.RepaintAll();
            }
            EditorGUILayout.PropertyField(this.m_SplashScreenLogoStyle, k_Texts.splashStyle, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_SplashScreenAnimation, k_Texts.animate, new GUILayoutOption[0]);
            this.m_ShowAnimationControlsAnimator.target = this.m_SplashScreenAnimation.intValue == 2;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowAnimationControlsAnimator.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Slider(this.m_SplashScreenLogoAnimationZoom, 0f, 1f, k_Texts.logoZoom, new GUILayoutOption[0]);
                EditorGUILayout.Slider(this.m_SplashScreenBackgroundAnimationZoom, 0f, 1f, k_Texts.backgroundZoom, new GUILayoutOption[0]);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(k_Texts.logosTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
            using (new EditorGUI.DisabledScope(!Application.HasProLicense()))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.m_ShowUnitySplashLogo, k_Texts.showLogo, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    if (!this.m_ShowUnitySplashLogo.boolValue)
                    {
                        this.RemoveUnityLogoFromLogosList();
                    }
                    else if (this.m_SplashScreenDrawMode.intValue == 1)
                    {
                        this.AddUnityLogoToLogosList();
                    }
                }
                this.m_ShowLogoControlsAnimator.target = this.m_ShowUnitySplashLogo.boolValue;
            }
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowLogoControlsAnimator.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                int intValue = this.m_SplashScreenDrawMode.intValue;
                EditorGUILayout.PropertyField(this.m_SplashScreenDrawMode, k_Texts.drawMode, new GUILayoutOption[0]);
                if (intValue != this.m_SplashScreenDrawMode.intValue)
                {
                    if (this.m_SplashScreenDrawMode.intValue == 0)
                    {
                        this.RemoveUnityLogoFromLogosList();
                    }
                    else
                    {
                        this.AddUnityLogoToLogosList();
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
            this.m_LogoList.DoLayoutList();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(k_Texts.backgroundTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUILayout.Slider(this.m_SplashScreenOverlayOpacity, !Application.HasProLicense() ? k_MinPersonalEditionOverlayOpacity : k_MinProEditionOverlayOpacity, 1f, k_Texts.overlayOpacity, new GUILayoutOption[0]);
            this.m_ShowBackgroundColorAnimator.target = this.m_SplashScreenBackgroundLandscape.objectReferenceValue == null;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowBackgroundColorAnimator.faded))
            {
                EditorGUILayout.PropertyField(this.m_SplashScreenBackgroundColor, k_Texts.backgroundColor, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            ObjectReferencePropertyField<Sprite>(this.m_SplashScreenBackgroundLandscape, k_Texts.backgroundImage);
            if (GUI.changed && (this.m_SplashScreenBackgroundLandscape.objectReferenceValue == null))
            {
                this.m_SplashScreenBackgroundPortrait.objectReferenceValue = null;
            }
            using (new EditorGUI.DisabledScope(this.m_SplashScreenBackgroundLandscape.objectReferenceValue == null))
            {
                ObjectReferencePropertyField<Sprite>(this.m_SplashScreenBackgroundPortrait, k_Texts.backgroundPortraitImage);
            }
        }

        private void DrawElementUnityLogo(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty property = this.m_SplashScreenLogos.GetArrayElementAtIndex(index).FindPropertyRelative("duration");
            float num = Mathf.Clamp(rect.width - k_LogoListPropertyMinWidth, k_LogoListUnityLogoMinWidth, k_LogoListUnityLogoMaxWidth);
            float height = num / (((float) s_UnityLogo.texture.width) / ((float) s_UnityLogo.texture.height));
            Rect position = new Rect(rect.x, rect.y + ((rect.height - height) / 2f), k_LogoListUnityLogoMaxWidth, height);
            Color color = GUI.color;
            GUI.color = (this.m_SplashScreenLogoStyle.intValue != 0) ? Color.white : k_DarkLogoColor;
            GUI.Label(position, s_UnityLogo.texture);
            GUI.color = color;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = k_LogoListPropertyLabelWidth;
            Rect totalPosition = new Rect(rect.x + num, (rect.y + EditorGUIUtility.standardVerticalSpacing) + EditorGUIUtility.singleLineHeight, rect.width - num, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginChangeCheck();
            GUIContent label = EditorGUI.BeginProperty(totalPosition, k_Texts.logoDuration, property);
            float num4 = EditorGUI.Slider(totalPosition, label, property.floatValue, k_MinLogoTime, k_MaxLogoTime);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = num4;
            }
            EditorGUI.EndProperty();
            EditorGUIUtility.labelWidth = labelWidth;
            this.m_TotalLogosDuration += property.floatValue;
        }

        private void DrawLogoListElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.height -= EditorGUIUtility.standardVerticalSpacing;
            SerializedProperty arrayElementAtIndex = this.m_SplashScreenLogos.GetArrayElementAtIndex(index);
            SerializedProperty property2 = arrayElementAtIndex.FindPropertyRelative("logo");
            if (((Sprite) property2.objectReferenceValue) == s_UnityLogo)
            {
                this.DrawElementUnityLogo(rect, index, isActive, isFocused);
            }
            else
            {
                float num = Mathf.Clamp(rect.width - k_LogoListPropertyMinWidth, k_LogoListUnityLogoMinWidth, k_LogoListUnityLogoMaxWidth);
                Rect position = new Rect(rect.x, rect.y + ((rect.height - k_LogoListLogoFieldHeight) / 2f), k_LogoListUnityLogoMinWidth, k_LogoListLogoFieldHeight);
                EditorGUI.BeginChangeCheck();
                Object obj2 = EditorGUI.ObjectField(position, GUIContent.none, (Sprite) property2.objectReferenceValue, typeof(Sprite), false);
                if (EditorGUI.EndChangeCheck())
                {
                    property2.objectReferenceValue = obj2;
                }
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = k_LogoListPropertyLabelWidth;
                Rect rect3 = new Rect(rect.x + num, (rect.y + EditorGUIUtility.standardVerticalSpacing) + EditorGUIUtility.singleLineHeight, rect.width - num, EditorGUIUtility.singleLineHeight);
                EditorGUI.BeginChangeCheck();
                SerializedProperty property3 = arrayElementAtIndex.FindPropertyRelative("duration");
                float num3 = EditorGUI.Slider(rect3, k_Texts.logoDuration, property3.floatValue, k_MinLogoTime, k_MaxLogoTime);
                if (EditorGUI.EndChangeCheck())
                {
                    property3.floatValue = num3;
                }
                EditorGUIUtility.labelWidth = labelWidth;
                this.m_TotalLogosDuration += property3.floatValue;
            }
        }

        private void DrawLogoListFooterCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Splash Screen Duration: " + Mathf.Max(k_MinLogoTime, this.m_TotalLogosDuration).ToString(), EditorStyles.miniBoldLabel);
            ReorderableList.defaultBehaviours.DrawFooter(rect, this.m_LogoList);
        }

        private void DrawLogoListHeaderCallback(Rect rect)
        {
            this.m_TotalLogosDuration = 0f;
            EditorGUI.LabelField(rect, "Logos");
        }

        internal static Color GetSplashScreenActualBackgroundColor()
        {
            Color color;
            INTERNAL_CALL_GetSplashScreenActualBackgroundColor(out color);
            return color;
        }

        internal static Texture2D GetSplashScreenActualBackgroundImage(Rect windowRect) => 
            INTERNAL_CALL_GetSplashScreenActualBackgroundImage(ref windowRect);

        internal static Rect GetSplashScreenActualUVs(Rect windowRect)
        {
            Rect rect;
            INTERNAL_CALL_GetSplashScreenActualUVs(ref windowRect, out rect);
            return rect;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetSplashScreenActualBackgroundColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern Texture2D INTERNAL_CALL_GetSplashScreenActualBackgroundImage(ref Rect windowRect);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetSplashScreenActualUVs(ref Rect windowRect, out Rect value);
        private static void ObjectReferencePropertyField<T>(SerializedProperty property, GUIContent label) where T: Object
        {
            EditorGUI.BeginChangeCheck();
            Rect totalPosition = EditorGUILayout.GetControlRect(true, 64f, EditorStyles.objectFieldThumb, new GUILayoutOption[0]);
            label = EditorGUI.BeginProperty(totalPosition, label, property);
            Object obj2 = EditorGUI.ObjectField(totalPosition, label, (T) property.objectReferenceValue, typeof(T), false);
            if (EditorGUI.EndChangeCheck())
            {
                property.objectReferenceValue = obj2;
                GUI.changed = true;
            }
            EditorGUI.EndProperty();
        }

        public void OnEnable()
        {
            this.m_ResolutionDialogBanner = this.m_Owner.FindPropertyAssert("resolutionDialogBanner");
            this.m_ShowUnitySplashLogo = this.m_Owner.FindPropertyAssert("m_ShowUnitySplashLogo");
            this.m_ShowUnitySplashScreen = this.m_Owner.FindPropertyAssert("m_ShowUnitySplashScreen");
            this.m_SplashScreenAnimation = this.m_Owner.FindPropertyAssert("m_SplashScreenAnimation");
            this.m_SplashScreenBackgroundAnimationZoom = this.m_Owner.FindPropertyAssert("m_SplashScreenBackgroundAnimationZoom");
            this.m_SplashScreenBackgroundColor = this.m_Owner.FindPropertyAssert("m_SplashScreenBackgroundColor");
            this.m_SplashScreenBackgroundLandscape = this.m_Owner.FindPropertyAssert("splashScreenBackgroundSourceLandscape");
            this.m_SplashScreenBackgroundPortrait = this.m_Owner.FindPropertyAssert("splashScreenBackgroundSourcePortrait");
            this.m_SplashScreenDrawMode = this.m_Owner.FindPropertyAssert("m_SplashScreenDrawMode");
            this.m_SplashScreenLogoAnimationZoom = this.m_Owner.FindPropertyAssert("m_SplashScreenLogoAnimationZoom");
            this.m_SplashScreenLogos = this.m_Owner.FindPropertyAssert("m_SplashScreenLogos");
            this.m_SplashScreenLogoStyle = this.m_Owner.FindPropertyAssert("m_SplashScreenLogoStyle");
            this.m_SplashScreenOverlayOpacity = this.m_Owner.FindPropertyAssert("m_SplashScreenOverlayOpacity");
            this.m_VirtualRealitySplashScreen = this.m_Owner.FindPropertyAssert("m_VirtualRealitySplashScreen");
            this.m_LogoList = new ReorderableList(this.m_Owner.serializedObject, this.m_SplashScreenLogos, true, true, true, true);
            this.m_LogoList.elementHeight = k_LogoListElementHeight;
            this.m_LogoList.footerHeight = k_LogoListFooterHeight;
            this.m_LogoList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.OnLogoListAddCallback);
            this.m_LogoList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawLogoListHeaderCallback);
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new ReorderableList.CanRemoveCallbackDelegate(PlayerSettingsSplashScreenEditor.OnLogoListCanRemoveCallback);
            }
            this.m_LogoList.onCanRemoveCallback = <>f__mg$cache0;
            this.m_LogoList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawLogoListElementCallback);
            this.m_LogoList.drawFooterCallback = new ReorderableList.FooterCallbackDelegate(this.DrawLogoListFooterCallback);
            this.m_ShowAnimationControlsAnimator.value = this.m_SplashScreenAnimation.intValue == 2;
            this.m_ShowAnimationControlsAnimator.valueChanged.AddListener(new UnityAction(this.m_Owner.Repaint));
            this.m_ShowBackgroundColorAnimator.value = this.m_SplashScreenBackgroundLandscape.objectReferenceValue == null;
            this.m_ShowBackgroundColorAnimator.valueChanged.AddListener(new UnityAction(this.m_Owner.Repaint));
            this.m_ShowLogoControlsAnimator.value = this.m_ShowUnitySplashLogo.boolValue;
            this.m_ShowLogoControlsAnimator.valueChanged.AddListener(new UnityAction(this.m_Owner.Repaint));
            if (s_UnityLogo == null)
            {
                s_UnityLogo = Resources.GetBuiltinResource<Sprite>("UnitySplash-cube.png");
            }
        }

        private void OnLogoListAddCallback(ReorderableList list)
        {
            int arraySize = this.m_SplashScreenLogos.arraySize;
            this.m_SplashScreenLogos.InsertArrayElementAtIndex(this.m_SplashScreenLogos.arraySize);
            SerializedProperty arrayElementAtIndex = this.m_SplashScreenLogos.GetArrayElementAtIndex(arraySize);
            SerializedProperty property2 = arrayElementAtIndex.FindPropertyRelative("logo");
            SerializedProperty property3 = arrayElementAtIndex.FindPropertyRelative("duration");
            property2.objectReferenceValue = null;
            property3.floatValue = k_DefaultLogoTime;
        }

        private static bool OnLogoListCanRemoveCallback(ReorderableList list)
        {
            Sprite objectReferenceValue = (Sprite) list.serializedProperty.GetArrayElementAtIndex(list.index).FindPropertyRelative("logo").objectReferenceValue;
            return (objectReferenceValue != s_UnityLogo);
        }

        private void RemoveUnityLogoFromLogosList()
        {
            for (int i = 0; i < this.m_SplashScreenLogos.arraySize; i++)
            {
                if (((Sprite) this.m_SplashScreenLogos.GetArrayElementAtIndex(i).FindPropertyRelative("logo").objectReferenceValue) == s_UnityLogo)
                {
                    this.m_SplashScreenLogos.DeleteArrayElementAtIndex(i);
                    i--;
                }
            }
        }

        public void SplashSectionGUI(BuildPlayerWindow.BuildPlatform platform, BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
        {
            GUI.changed = false;
            if (this.m_Owner.BeginSettingsBox(2, k_Texts.title))
            {
                if (targetGroup == BuildTargetGroup.Standalone)
                {
                    ObjectReferencePropertyField<Texture2D>(this.m_ResolutionDialogBanner, k_Texts.configDialogBanner);
                    EditorGUILayout.Space();
                }
                if (this.m_Owner.m_VRSettings.TargetGroupSupportsVirtualReality(targetGroup))
                {
                    ObjectReferencePropertyField<Texture2D>(this.m_VirtualRealitySplashScreen, k_Texts.vrSplashScreen);
                }
                if (TargetSupportsOptionalBuiltinSplashScreen(targetGroup, settingsExtension))
                {
                    this.BuiltinCustomSplashScreenGUI();
                }
                if (settingsExtension != null)
                {
                    settingsExtension.SplashSectionGUI();
                }
                if (this.m_ShowUnitySplashScreen.boolValue)
                {
                    this.m_Owner.ShowSharedNote();
                }
            }
            this.m_Owner.EndSettingsBox();
        }

        private static bool TargetSupportsOptionalBuiltinSplashScreen(BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
        {
            if (settingsExtension != null)
            {
                return settingsExtension.CanShowUnitySplashScreen();
            }
            return (targetGroup == BuildTargetGroup.Standalone);
        }

        public static bool licenseAllowsDisabling { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        private class Texts
        {
            public GUIContent animate = EditorGUIUtility.TextContent("Animation");
            public GUIContent backgroundColor = EditorGUIUtility.TextContent("Background Color|Background color when no background image is used.");
            public GUIContent backgroundImage = EditorGUIUtility.TextContent("Background Image|Image to be used in landscape and portrait(when portrait image is not set).");
            public GUIContent backgroundPortraitImage = EditorGUIUtility.TextContent("Alternate Portrait Image*|Optional image to be used in portrait mode.");
            public GUIContent backgroundTitle = EditorGUIUtility.TextContent("Background*");
            public GUIContent backgroundZoom = EditorGUIUtility.TextContent("Background Zoom");
            public GUIContent configDialogBanner = EditorGUIUtility.TextContent("Application Config Dialog Banner");
            public GUIContent drawMode = EditorGUIUtility.TextContent("Draw Mode");
            public GUIContent logoDuration = EditorGUIUtility.TextContent("Logo Duration|The time the logo will be shown for.");
            public GUIContent logosTitle = EditorGUIUtility.TextContent("Logos*");
            public GUIContent logoZoom = EditorGUIUtility.TextContent("Logo Zoom");
            public GUIContent overlayOpacity = EditorGUIUtility.TextContent("Overlay Opacity|Overlay strength applied to improve logo visibility.");
            public GUIContent previewSplash = EditorGUIUtility.TextContent("Preview|Preview the splash screen in the game view.");
            public GUIContent showLogo = EditorGUIUtility.TextContent("Show Unity Logo");
            public GUIContent showSplash = EditorGUIUtility.TextContent("Show Splash Screen");
            public GUIContent splashStyle = EditorGUIUtility.TextContent("Splash Style");
            public GUIContent splashTitle = EditorGUIUtility.TextContent("Splash Screen");
            public GUIContent title = EditorGUIUtility.TextContent("Splash Image");
            public GUIContent vrSplashScreen = EditorGUIUtility.TextContent("Virtual Reality Splash Image");
        }
    }
}

