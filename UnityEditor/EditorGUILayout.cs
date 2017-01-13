namespace UnityEditor
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;

    /// <summary>
    /// <para>Auto-layouted version of EditorGUI.</para>
    /// </summary>
    public sealed class EditorGUILayout
    {
        [CompilerGenerated]
        private static TargetChoiceHandler.TargetChoiceMenuFunction <>f__mg$cache0;
        internal const float kPlatformTabWidth = 30f;
        internal static Rect s_LastRect;
        internal static SavedBool s_SelectedDefault = new SavedBool("Platform.ShownDefaultTab", true);

        /// <summary>
        /// <para>Begins a group that can be be hidden/shown and the transition will be animated.</para>
        /// </summary>
        /// <param name="value">A value between 0 and 1, 0 being hidden, and 1 being fully visible.</param>
        /// <returns>
        /// <para>If the group is visible or not.</para>
        /// </returns>
        public static bool BeginFadeGroup(float value)
        {
            if (value == 0f)
            {
                return false;
            }
            if (value == 1f)
            {
                return true;
            }
            GUILayoutFadeGroup group = (GUILayoutFadeGroup) GUILayoutUtility.BeginLayoutGroup(GUIStyle.none, null, typeof(GUILayoutFadeGroup));
            group.isVertical = true;
            group.resetCoords = true;
            group.fadeValue = value;
            group.wasGUIEnabled = GUI.enabled;
            group.guiColor = GUI.color;
            if (((value != 0f) && (value != 1f)) && (Event.current.type == EventType.MouseDown))
            {
                Event.current.Use();
            }
            EditorGUIUtility.LockContextWidth();
            GUI.BeginGroup(group.rect);
            return !(value == 0f);
        }

        /// <summary>
        /// <para>Begin a horizontal group and get its rect back.</para>
        /// </summary>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        /// properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static Rect BeginHorizontal(params GUILayoutOption[] options) => 
            BeginHorizontal(GUIContent.none, GUIStyle.none, options);

        /// <summary>
        /// <para>Begin a horizontal group and get its rect back.</para>
        /// </summary>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        /// properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static Rect BeginHorizontal(GUIStyle style, params GUILayoutOption[] options) => 
            BeginHorizontal(GUIContent.none, style, options);

        internal static Rect BeginHorizontal(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayoutGroup group = GUILayoutUtility.BeginLayoutGroup(style, options, typeof(GUILayoutGroup));
            group.isVertical = false;
            if ((style != GUIStyle.none) || (content != GUIContent.none))
            {
                GUI.Box(group.rect, GUIContent.none, style);
            }
            return group.rect;
        }

        internal static Vector2 BeginHorizontalScrollView(Vector2 scrollPosition, params GUILayoutOption[] options) => 
            BeginHorizontalScrollView(scrollPosition, false, GUI.skin.horizontalScrollbar, GUI.skin.scrollView, options);

        internal static Vector2 BeginHorizontalScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, GUIStyle horizontalScrollbar, GUIStyle background, params GUILayoutOption[] options)
        {
            GUIScrollGroup group = (GUIScrollGroup) GUILayoutUtility.BeginLayoutGroup(background, null, typeof(GUIScrollGroup));
            if (Event.current.type == EventType.Layout)
            {
                group.resetCoords = true;
                group.isVertical = true;
                group.stretchWidth = 1;
                group.stretchHeight = 1;
                group.verticalScrollbar = GUIStyle.none;
                group.horizontalScrollbar = horizontalScrollbar;
                group.allowHorizontalScroll = true;
                group.allowVerticalScroll = false;
                group.ApplyOptions(options);
            }
            return EditorGUIInternal.DoBeginScrollViewForward(group.rect, scrollPosition, new Rect(0f, 0f, group.clientWidth, group.clientHeight), alwaysShowHorizontal, false, horizontalScrollbar, GUI.skin.verticalScrollbar, background);
        }

        internal static int BeginPlatformGrouping(BuildPlayerWindow.BuildPlatform[] platforms, GUIContent defaultTab) => 
            BeginPlatformGrouping(platforms, defaultTab, GUI.skin.box);

        internal static int BeginPlatformGrouping(BuildPlayerWindow.BuildPlatform[] platforms, GUIContent defaultTab, GUIStyle style)
        {
            int num = -1;
            for (int i = 0; i < platforms.Length; i++)
            {
                if (platforms[i].targetGroup == EditorUserBuildSettings.selectedBuildTargetGroup)
                {
                    num = i;
                }
            }
            if (num == -1)
            {
                s_SelectedDefault.value = true;
                num = 0;
            }
            int index = (defaultTab != null) ? (!s_SelectedDefault.value ? num : -1) : num;
            bool enabled = GUI.enabled;
            GUI.enabled = true;
            EditorGUI.BeginChangeCheck();
            Rect rect = BeginVertical(style, new GUILayoutOption[0]);
            rect.width--;
            int length = platforms.Length;
            int num5 = 0x12;
            GUIStyle toolbarButton = EditorStyles.toolbarButton;
            if ((defaultTab != null) && GUI.Toggle(new Rect(rect.x, rect.y, rect.width - (length * 30f), (float) num5), index == -1, defaultTab, toolbarButton))
            {
                index = -1;
            }
            for (int j = 0; j < length; j++)
            {
                Rect rect2;
                if (defaultTab != null)
                {
                    rect2 = new Rect(rect.xMax - ((length - j) * 30f), rect.y, 30f, (float) num5);
                }
                else
                {
                    int num7 = Mathf.RoundToInt((j * rect.width) / ((float) length));
                    int num8 = Mathf.RoundToInt(((j + 1) * rect.width) / ((float) length));
                    rect2 = new Rect(rect.x + num7, rect.y, (float) (num8 - num7), (float) num5);
                }
                if (GUI.Toggle(rect2, index == j, new GUIContent(platforms[j].smallIcon, platforms[j].tooltip), toolbarButton))
                {
                    index = j;
                }
            }
            GUILayoutUtility.GetRect(10f, (float) num5);
            GUI.enabled = enabled;
            if (EditorGUI.EndChangeCheck())
            {
                if (defaultTab == null)
                {
                    EditorUserBuildSettings.selectedBuildTargetGroup = platforms[index].targetGroup;
                }
                else if (index < 0)
                {
                    s_SelectedDefault.value = true;
                }
                else
                {
                    EditorUserBuildSettings.selectedBuildTargetGroup = platforms[index].targetGroup;
                    s_SelectedDefault.value = false;
                }
                Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(BuildPlayerWindow));
                for (int k = 0; k < objArray.Length; k++)
                {
                    BuildPlayerWindow window = objArray[k] as BuildPlayerWindow;
                    if (window != null)
                    {
                        window.Repaint();
                    }
                }
            }
            return index;
        }

        /// <summary>
        /// <para>Begin an automatically layouted scrollview.</para>
        /// </summary>
        /// <param name="scrollPosition">The position to use display.</param>
        /// <param name="alwayShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
        /// <param name="alwayShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
        /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
        /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
        /// <param name="options"></param>
        /// <param name="alwaysShowHorizontal"></param>
        /// <param name="alwaysShowVertical"></param>
        /// <param name="background"></param>
        /// <returns>
        /// <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
        /// </returns>
        public static Vector2 BeginScrollView(Vector2 scrollPosition, params GUILayoutOption[] options) => 
            BeginScrollView(scrollPosition, false, false, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView, options);

        public static Vector2 BeginScrollView(Vector2 scrollPosition, GUIStyle style, params GUILayoutOption[] options)
        {
            string name = style.name;
            GUIStyle verticalScrollbar = GUI.skin.FindStyle(name + "VerticalScrollbar");
            if (verticalScrollbar == null)
            {
                verticalScrollbar = GUI.skin.verticalScrollbar;
            }
            GUIStyle horizontalScrollbar = GUI.skin.FindStyle(name + "HorizontalScrollbar");
            if (horizontalScrollbar == null)
            {
                horizontalScrollbar = GUI.skin.horizontalScrollbar;
            }
            return BeginScrollView(scrollPosition, false, false, horizontalScrollbar, verticalScrollbar, style, options);
        }

        /// <summary>
        /// <para>Begin an automatically layouted scrollview.</para>
        /// </summary>
        /// <param name="scrollPosition">The position to use display.</param>
        /// <param name="alwayShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
        /// <param name="alwayShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
        /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
        /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
        /// <param name="options"></param>
        /// <param name="alwaysShowHorizontal"></param>
        /// <param name="alwaysShowVertical"></param>
        /// <param name="background"></param>
        /// <returns>
        /// <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
        /// </returns>
        public static Vector2 BeginScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options) => 
            BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView, options);

        /// <summary>
        /// <para>Begin an automatically layouted scrollview.</para>
        /// </summary>
        /// <param name="scrollPosition">The position to use display.</param>
        /// <param name="alwayShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
        /// <param name="alwayShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
        /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
        /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
        /// <param name="options"></param>
        /// <param name="alwaysShowHorizontal"></param>
        /// <param name="alwaysShowVertical"></param>
        /// <param name="background"></param>
        /// <returns>
        /// <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
        /// </returns>
        public static Vector2 BeginScrollView(Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options) => 
            BeginScrollView(scrollPosition, false, false, horizontalScrollbar, verticalScrollbar, GUI.skin.scrollView, options);

        internal static Vector2 BeginScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options) => 
            BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, GUI.skin.scrollView, options);

        /// <summary>
        /// <para>Begin an automatically layouted scrollview.</para>
        /// </summary>
        /// <param name="scrollPosition">The position to use display.</param>
        /// <param name="alwayShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
        /// <param name="alwayShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
        /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
        /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
        /// <param name="options"></param>
        /// <param name="alwaysShowHorizontal"></param>
        /// <param name="alwaysShowVertical"></param>
        /// <param name="background"></param>
        /// <returns>
        /// <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
        /// </returns>
        public static Vector2 BeginScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options)
        {
            GUIScrollGroup group = (GUIScrollGroup) GUILayoutUtility.BeginLayoutGroup(background, null, typeof(GUIScrollGroup));
            if (Event.current.type == EventType.Layout)
            {
                group.resetCoords = true;
                group.isVertical = true;
                group.stretchWidth = 1;
                group.stretchHeight = 1;
                group.verticalScrollbar = verticalScrollbar;
                group.horizontalScrollbar = horizontalScrollbar;
                group.ApplyOptions(options);
            }
            return EditorGUIInternal.DoBeginScrollViewForward(group.rect, scrollPosition, new Rect(0f, 0f, group.clientWidth, group.clientHeight), alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background);
        }

        /// <summary>
        /// <para>Begin a vertical group with a toggle to enable or disable all the controls within at once.</para>
        /// </summary>
        /// <param name="label">Label to show above the toggled controls.</param>
        /// <param name="toggle">Enabled state of the toggle group.</param>
        /// <returns>
        /// <para>The enabled state selected by the user.</para>
        /// </returns>
        public static bool BeginToggleGroup(string label, bool toggle) => 
            BeginToggleGroup(EditorGUIUtility.TempContent(label), toggle);

        /// <summary>
        /// <para>Begin a vertical group with a toggle to enable or disable all the controls within at once.</para>
        /// </summary>
        /// <param name="label">Label to show above the toggled controls.</param>
        /// <param name="toggle">Enabled state of the toggle group.</param>
        /// <returns>
        /// <para>The enabled state selected by the user.</para>
        /// </returns>
        public static bool BeginToggleGroup(GUIContent label, bool toggle)
        {
            toggle = ToggleLeft(label, toggle, EditorStyles.boldLabel, new GUILayoutOption[0]);
            EditorGUI.BeginDisabled(!toggle);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            return toggle;
        }

        /// <summary>
        /// <para>Begin a vertical group and get its rect back.</para>
        /// </summary>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static Rect BeginVertical(params GUILayoutOption[] options) => 
            BeginVertical(GUIContent.none, GUIStyle.none, options);

        /// <summary>
        /// <para>Begin a vertical group and get its rect back.</para>
        /// </summary>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties.
        /// Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static Rect BeginVertical(GUIStyle style, params GUILayoutOption[] options) => 
            BeginVertical(GUIContent.none, style, options);

        internal static Rect BeginVertical(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayoutGroup group = GUILayoutUtility.BeginLayoutGroup(style, options, typeof(GUILayoutGroup));
            group.isVertical = true;
            if ((style != GUIStyle.none) || (content != GUIContent.none))
            {
                GUI.Box(group.rect, GUIContent.none, style);
            }
            return group.rect;
        }

        internal static Vector2 BeginVerticalScrollView(Vector2 scrollPosition, params GUILayoutOption[] options) => 
            BeginVerticalScrollView(scrollPosition, false, GUI.skin.verticalScrollbar, GUI.skin.scrollView, options);

        internal static Vector2 BeginVerticalScrollView(Vector2 scrollPosition, bool alwaysShowVertical, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options)
        {
            GUIScrollGroup group = (GUIScrollGroup) GUILayoutUtility.BeginLayoutGroup(background, null, typeof(GUIScrollGroup));
            if (Event.current.type == EventType.Layout)
            {
                group.resetCoords = true;
                group.isVertical = true;
                group.stretchWidth = 1;
                group.stretchHeight = 1;
                group.verticalScrollbar = verticalScrollbar;
                group.horizontalScrollbar = GUIStyle.none;
                group.allowHorizontalScroll = false;
                group.ApplyOptions(options);
            }
            return EditorGUIInternal.DoBeginScrollViewForward(group.rect, scrollPosition, new Rect(0f, 0f, group.clientWidth, group.clientHeight), false, alwaysShowVertical, GUI.skin.horizontalScrollbar, verticalScrollbar, background);
        }

        internal static bool BitToggleField(string label, SerializedProperty bitFieldProperty, int flag)
        {
            bool flag2 = (bitFieldProperty.intValue & flag) != 0;
            bool flag3 = (bitFieldProperty.hasMultipleDifferentValuesBitwise & flag) != 0;
            EditorGUI.showMixedValue = flag3;
            EditorGUI.BeginChangeCheck();
            flag2 = Toggle(label, flag2, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                if (flag3)
                {
                    flag2 = true;
                }
                flag3 = false;
                int index = -1;
                for (int i = 0; i < 0x20; i++)
                {
                    if (((((int) 1) << i) & flag) != 0)
                    {
                        index = i;
                        break;
                    }
                }
                bitFieldProperty.SetBitAtIndexForAllTargetsImmediate(index, flag2);
            }
            EditorGUI.showMixedValue = false;
            return (flag2 && !flag3);
        }

        /// <summary>
        /// <para>Make Center &amp; Extents field for entering a Bounds.</para>
        /// </summary>
        /// <param name="label">Label to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static Bounds BoundsField(Bounds value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, EditorGUI.GetPropertyHeight(SerializedPropertyType.Bounds, GUIContent.none), EditorStyles.numberField, options);
            return EditorGUI.BoundsField(position, value);
        }

        /// <summary>
        /// <para>Make Center &amp; Extents field for entering a Bounds.</para>
        /// </summary>
        /// <param name="label">Label to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static Bounds BoundsField(string label, Bounds value, params GUILayoutOption[] options) => 
            BoundsField(EditorGUIUtility.TempContent(label), value, options);

        /// <summary>
        /// <para>Make Center &amp; Extents field for entering a Bounds.</para>
        /// </summary>
        /// <param name="label">Label to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static Bounds BoundsField(GUIContent label, Bounds value, params GUILayoutOption[] options)
        {
            bool hasLabel = EditorGUI.LabelHasContent(label);
            float propertyHeight = EditorGUI.GetPropertyHeight(SerializedPropertyType.Bounds, label);
            Rect position = s_LastRect = GetControlRect(hasLabel, propertyHeight, EditorStyles.numberField, options);
            return EditorGUI.BoundsField(position, label, value);
        }

        internal static bool ButtonMouseDown(GUIContent content, FocusType focusType, GUIStyle style, params GUILayoutOption[] options)
        {
            s_LastRect = GUILayoutUtility.GetRect(content, style, options);
            return EditorGUI.ButtonMouseDown(s_LastRect, content, focusType, style);
        }

        internal static Color ColorBrightnessField(GUIContent label, Color value, float minBrightness, float maxBrightness, params GUILayoutOption[] options)
        {
            Rect r = s_LastRect = GetControlRect(true, 16f, EditorStyles.numberField, options);
            return EditorGUI.ColorBrightnessField(r, label, value, minBrightness, maxBrightness);
        }

        /// <summary>
        /// <para>Make a field for selecting a Color.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the field.</param>
        /// <param name="value">The color to edit.</param>
        /// <param name="showEyedropper">If true, the color picker should show the eyedropper control. If false, don't show it.</param>
        /// <param name="showAlpha">If true, allow the user to set an alpha value for the color. If false, hide the alpha component.</param>
        /// <param name="hdr">If true, treat the color as an HDR value. If false, treat it as a standard LDR value.</param>
        /// <param name="hdrConfig">An object that sets the presentation parameters for an HDR color. If not using an HDR color, set this to null.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The color selected by the user.</para>
        /// </returns>
        public static Color ColorField(Color value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, EditorStyles.colorField, options);
            return EditorGUI.ColorField(position, value);
        }

        /// <summary>
        /// <para>Make a field for selecting a Color.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the field.</param>
        /// <param name="value">The color to edit.</param>
        /// <param name="showEyedropper">If true, the color picker should show the eyedropper control. If false, don't show it.</param>
        /// <param name="showAlpha">If true, allow the user to set an alpha value for the color. If false, hide the alpha component.</param>
        /// <param name="hdr">If true, treat the color as an HDR value. If false, treat it as a standard LDR value.</param>
        /// <param name="hdrConfig">An object that sets the presentation parameters for an HDR color. If not using an HDR color, set this to null.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The color selected by the user.</para>
        /// </returns>
        public static Color ColorField(string label, Color value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.colorField, options);
            return EditorGUI.ColorField(position, label, value);
        }

        /// <summary>
        /// <para>Make a field for selecting a Color.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the field.</param>
        /// <param name="value">The color to edit.</param>
        /// <param name="showEyedropper">If true, the color picker should show the eyedropper control. If false, don't show it.</param>
        /// <param name="showAlpha">If true, allow the user to set an alpha value for the color. If false, hide the alpha component.</param>
        /// <param name="hdr">If true, treat the color as an HDR value. If false, treat it as a standard LDR value.</param>
        /// <param name="hdrConfig">An object that sets the presentation parameters for an HDR color. If not using an HDR color, set this to null.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The color selected by the user.</para>
        /// </returns>
        public static Color ColorField(GUIContent label, Color value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.colorField, options);
            return EditorGUI.ColorField(position, label, value);
        }

        /// <summary>
        /// <para>Make a field for selecting a Color.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the field.</param>
        /// <param name="value">The color to edit.</param>
        /// <param name="showEyedropper">If true, the color picker should show the eyedropper control. If false, don't show it.</param>
        /// <param name="showAlpha">If true, allow the user to set an alpha value for the color. If false, hide the alpha component.</param>
        /// <param name="hdr">If true, treat the color as an HDR value. If false, treat it as a standard LDR value.</param>
        /// <param name="hdrConfig">An object that sets the presentation parameters for an HDR color. If not using an HDR color, set this to null.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The color selected by the user.</para>
        /// </returns>
        public static Color ColorField(GUIContent label, Color value, bool showEyedropper, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.colorField, options);
            return EditorGUI.ColorField(position, label, value, showEyedropper, showAlpha, hdr, hdrConfig);
        }

        /// <summary>
        /// <para>Make a field for editing an AnimationCurve.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the field.</param>
        /// <param name="value">The curve to edit.</param>
        /// <param name="color">The color to show the curve with.</param>
        /// <param name="ranges">Optional rectangle that the curve is restrained within.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The curve edited by the user.</para>
        /// </returns>
        public static AnimationCurve CurveField(AnimationCurve value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, EditorStyles.colorField, options);
            return EditorGUI.CurveField(position, value);
        }

        /// <summary>
        /// <para>Make a field for editing an AnimationCurve.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the field.</param>
        /// <param name="value">The curve to edit.</param>
        /// <param name="color">The color to show the curve with.</param>
        /// <param name="ranges">Optional rectangle that the curve is restrained within.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The curve edited by the user.</para>
        /// </returns>
        public static AnimationCurve CurveField(string label, AnimationCurve value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.colorField, options);
            return EditorGUI.CurveField(position, label, value);
        }

        /// <summary>
        /// <para>Make a field for editing an AnimationCurve.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the field.</param>
        /// <param name="value">The curve to edit.</param>
        /// <param name="color">The color to show the curve with.</param>
        /// <param name="ranges">Optional rectangle that the curve is restrained within.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The curve edited by the user.</para>
        /// </returns>
        public static AnimationCurve CurveField(GUIContent label, AnimationCurve value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.colorField, options);
            return EditorGUI.CurveField(position, label, value);
        }

        /// <summary>
        /// <para>Make a field for editing an AnimationCurve.</para>
        /// </summary>
        /// <param name="property">The curve to edit.</param>
        /// <param name="color">The color to show the curve with.</param>
        /// <param name="ranges">Optional rectangle that the curve is restrained within.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="label">Optional label to display in front of the field. Pass [[GUIContent.none] to hide the label.</param>
        public static void CurveField(SerializedProperty property, Color color, Rect ranges, params GUILayoutOption[] options)
        {
            CurveField(property, color, ranges, null, options);
        }

        /// <summary>
        /// <para>Make a field for editing an AnimationCurve.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the field.</param>
        /// <param name="value">The curve to edit.</param>
        /// <param name="color">The color to show the curve with.</param>
        /// <param name="ranges">Optional rectangle that the curve is restrained within.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The curve edited by the user.</para>
        /// </returns>
        public static AnimationCurve CurveField(AnimationCurve value, Color color, Rect ranges, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, EditorStyles.colorField, options);
            return EditorGUI.CurveField(position, value, color, ranges);
        }

        /// <summary>
        /// <para>Make a field for editing an AnimationCurve.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the field.</param>
        /// <param name="value">The curve to edit.</param>
        /// <param name="color">The color to show the curve with.</param>
        /// <param name="ranges">Optional rectangle that the curve is restrained within.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The curve edited by the user.</para>
        /// </returns>
        public static AnimationCurve CurveField(string label, AnimationCurve value, Color color, Rect ranges, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.colorField, options);
            return EditorGUI.CurveField(position, label, value, color, ranges);
        }

        /// <summary>
        /// <para>Make a field for editing an AnimationCurve.</para>
        /// </summary>
        /// <param name="property">The curve to edit.</param>
        /// <param name="color">The color to show the curve with.</param>
        /// <param name="ranges">Optional rectangle that the curve is restrained within.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="label">Optional label to display in front of the field. Pass [[GUIContent.none] to hide the label.</param>
        public static void CurveField(SerializedProperty property, Color color, Rect ranges, GUIContent label, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, EditorStyles.colorField, options);
            EditorGUI.CurveField(position, property, color, ranges, label);
        }

        /// <summary>
        /// <para>Make a field for editing an AnimationCurve.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the field.</param>
        /// <param name="value">The curve to edit.</param>
        /// <param name="color">The color to show the curve with.</param>
        /// <param name="ranges">Optional rectangle that the curve is restrained within.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The curve edited by the user.</para>
        /// </returns>
        public static AnimationCurve CurveField(GUIContent label, AnimationCurve value, Color color, Rect ranges, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.colorField, options);
            return EditorGUI.CurveField(position, label, value, color, ranges);
        }

        internal static int CycleButton(int selected, GUIContent[] options, GUIStyle style)
        {
            if (GUILayout.Button(options[selected], style, new GUILayoutOption[0]))
            {
                selected++;
                if (selected >= options.Length)
                {
                    selected = 0;
                }
            }
            return selected;
        }

        /// <summary>
        /// <para>Make a delayed text field for entering doubles.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">
        /// An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
        /// </param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the double field.</para>
        /// </returns>
        public static double DelayedDoubleField(double value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, EditorStyles.numberField, options);
            return EditorGUI.DelayedDoubleField(position, value);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering doubles.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">
        /// An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
        /// </param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the double field.</para>
        /// </returns>
        public static double DelayedDoubleField(double value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.DelayedDoubleField(position, value, style);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering doubles.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">
        /// An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
        /// </param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the double field.</para>
        /// </returns>
        public static double DelayedDoubleField(string label, double value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.numberField, options);
            return EditorGUI.DelayedDoubleField(position, label, value);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering doubles.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">
        /// An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
        /// </param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the double field.</para>
        /// </returns>
        public static double DelayedDoubleField(GUIContent label, double value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.numberField, options);
            return EditorGUI.DelayedDoubleField(position, label, value);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering doubles.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">
        /// An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
        /// </param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the double field.</para>
        /// </returns>
        public static double DelayedDoubleField(string label, double value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.DelayedDoubleField(position, label, value, style);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering doubles.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">
        /// An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
        /// </param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the double field.</para>
        /// </returns>
        public static double DelayedDoubleField(GUIContent label, double value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.DelayedDoubleField(position, label, value, style);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering floats.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the float field.</para>
        /// </returns>
        public static float DelayedFloatField(float value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, EditorStyles.numberField, options);
            return EditorGUI.DelayedFloatField(position, value);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering floats.</para>
        /// </summary>
        /// <param name="property">The float property to edit.</param>
        /// <param name="label">Optional label to display in front of the float field. Pass GUIContent.none to hide label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void DelayedFloatField(SerializedProperty property, params GUILayoutOption[] options)
        {
            DelayedFloatField(property, null, options);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering floats.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the float field.</para>
        /// </returns>
        public static float DelayedFloatField(float value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.DelayedFloatField(position, value, style);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering floats.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the float field.</para>
        /// </returns>
        public static float DelayedFloatField(string label, float value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.numberField, options);
            return EditorGUI.DelayedFloatField(position, label, value);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering floats.</para>
        /// </summary>
        /// <param name="property">The float property to edit.</param>
        /// <param name="label">Optional label to display in front of the float field. Pass GUIContent.none to hide label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void DelayedFloatField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(EditorGUI.LabelHasContent(label), 16f, EditorStyles.numberField, options);
            EditorGUI.DelayedFloatField(position, property, label);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering floats.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the float field.</para>
        /// </returns>
        public static float DelayedFloatField(GUIContent label, float value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.numberField, options);
            return EditorGUI.DelayedFloatField(position, label, value);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering floats.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the float field.</para>
        /// </returns>
        public static float DelayedFloatField(string label, float value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.DelayedFloatField(position, label, value, style);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering floats.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the float field.</para>
        /// </returns>
        public static float DelayedFloatField(GUIContent label, float value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.DelayedFloatField(position, label, value, style);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the int field.</para>
        /// </returns>
        public static int DelayedIntField(int value, params GUILayoutOption[] options) => 
            DelayedIntField(value, EditorStyles.numberField, options);

        /// <summary>
        /// <para>Make a delayed text field for entering integers.</para>
        /// </summary>
        /// <param name="property">The int property to edit.</param>
        /// <param name="label">Optional label to display in front of the int field. Pass GUIContent.none to hide label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void DelayedIntField(SerializedProperty property, params GUILayoutOption[] options)
        {
            DelayedIntField(property, null, options);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the int field.</para>
        /// </returns>
        public static int DelayedIntField(int value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.DelayedIntField(position, value, style);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the int field.</para>
        /// </returns>
        public static int DelayedIntField(string label, int value, params GUILayoutOption[] options) => 
            DelayedIntField(label, value, EditorStyles.numberField, options);

        /// <summary>
        /// <para>Make a delayed text field for entering integers.</para>
        /// </summary>
        /// <param name="property">The int property to edit.</param>
        /// <param name="label">Optional label to display in front of the int field. Pass GUIContent.none to hide label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void DelayedIntField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(EditorGUI.LabelHasContent(label), 16f, EditorStyles.numberField, options);
            EditorGUI.DelayedIntField(position, property, label);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the int field.</para>
        /// </returns>
        public static int DelayedIntField(GUIContent label, int value, params GUILayoutOption[] options) => 
            DelayedIntField(label, value, EditorStyles.numberField, options);

        /// <summary>
        /// <para>Make a delayed text field for entering integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the int field.</para>
        /// </returns>
        public static int DelayedIntField(string label, int value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.DelayedIntField(position, label, value, style);
        }

        /// <summary>
        /// <para>Make a delayed text field for entering integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the int field.</para>
        /// </returns>
        public static int DelayedIntField(GUIContent label, int value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.DelayedIntField(position, label, value, style);
        }

        /// <summary>
        /// <para>Make a delayed text field.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the text field.</para>
        /// </returns>
        public static string DelayedTextField(string text, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, EditorStyles.textField, options);
            return EditorGUI.DelayedTextField(position, text);
        }

        /// <summary>
        /// <para>Make a delayed text field.</para>
        /// </summary>
        /// <param name="property">The text property to edit.</param>
        /// <param name="label">Optional label to display in front of the int field. Pass GUIContent.none to hide label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void DelayedTextField(SerializedProperty property, params GUILayoutOption[] options)
        {
            DelayedTextField(property, null, options);
        }

        /// <summary>
        /// <para>Make a delayed text field.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the text field.</para>
        /// </returns>
        public static string DelayedTextField(string label, string text, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.textField, options);
            return EditorGUI.DelayedTextField(position, label, text);
        }

        /// <summary>
        /// <para>Make a delayed text field.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the text field.</para>
        /// </returns>
        public static string DelayedTextField(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.DelayedTextField(position, text, style);
        }

        /// <summary>
        /// <para>Make a delayed text field.</para>
        /// </summary>
        /// <param name="property">The text property to edit.</param>
        /// <param name="label">Optional label to display in front of the int field. Pass GUIContent.none to hide label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void DelayedTextField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(EditorGUI.LabelHasContent(label), 16f, EditorStyles.textField, options);
            EditorGUI.DelayedTextField(position, property, label);
        }

        /// <summary>
        /// <para>Make a delayed text field.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the text field.</para>
        /// </returns>
        public static string DelayedTextField(GUIContent label, string text, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.textField, options);
            return EditorGUI.DelayedTextField(position, label, text);
        }

        /// <summary>
        /// <para>Make a delayed text field.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the text field.</para>
        /// </returns>
        public static string DelayedTextField(string label, string text, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.DelayedTextField(position, label, text, style);
        }

        /// <summary>
        /// <para>Make a delayed text field.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user. Note that the return value will not change until the user has pressed enter or focus is moved away from the text field.</para>
        /// </returns>
        public static string DelayedTextField(GUIContent label, string text, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.DelayedTextField(position, label, text, style);
        }

        internal static string DelayedTextFieldDropDown(string text, string[] dropDownElement) => 
            DelayedTextFieldDropDown(GUIContent.none, text, dropDownElement);

        internal static string DelayedTextFieldDropDown(GUIContent label, string text, string[] dropDownElement) => 
            EditorGUI.DelayedTextFieldDropDown(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.textFieldDropDownText), label, text, dropDownElement);

        /// <summary>
        /// <para>Make a text field for entering double values.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static double DoubleField(double value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, EditorStyles.numberField, options);
            return EditorGUI.DoubleField(position, value);
        }

        /// <summary>
        /// <para>Make a text field for entering double values.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static double DoubleField(double value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.DoubleField(position, value, style);
        }

        /// <summary>
        /// <para>Make a text field for entering double values.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static double DoubleField(string label, double value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.numberField, options);
            return EditorGUI.DoubleField(position, label, value);
        }

        /// <summary>
        /// <para>Make a text field for entering double values.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static double DoubleField(GUIContent label, double value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.numberField, options);
            return EditorGUI.DoubleField(position, label, value);
        }

        /// <summary>
        /// <para>Make a text field for entering double values.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static double DoubleField(string label, double value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.DoubleField(position, label, value, style);
        }

        /// <summary>
        /// <para>Make a text field for entering double values.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the double field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static double DoubleField(GUIContent label, double value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.DoubleField(position, label, value, style);
        }

        /// <summary>
        /// <para>Closes a group started with BeginFadeGroup.</para>
        /// </summary>
        public static void EndFadeGroup()
        {
            GUILayoutFadeGroup topLevel = EditorGUILayoutUtilityInternal.topLevel as GUILayoutFadeGroup;
            if (topLevel != null)
            {
                GUI.EndGroup();
                EditorGUIUtility.UnlockContextWidth();
                GUI.enabled = topLevel.wasGUIEnabled;
                GUI.color = topLevel.guiColor;
                GUILayoutUtility.EndLayoutGroup();
            }
        }

        /// <summary>
        /// <para>Close a group started with BeginHorizontal.</para>
        /// </summary>
        public static void EndHorizontal()
        {
            GUILayout.EndHorizontal();
        }

        internal static void EndPlatformGrouping()
        {
            EndVertical();
        }

        /// <summary>
        /// <para>Ends a scrollview started with a call to BeginScrollView.</para>
        /// </summary>
        public static void EndScrollView()
        {
            GUILayout.EndScrollView(true);
        }

        internal static void EndScrollView(bool handleScrollWheel)
        {
            GUILayout.EndScrollView(handleScrollWheel);
        }

        /// <summary>
        /// <para>Close a group started with BeginToggleGroup.</para>
        /// </summary>
        public static void EndToggleGroup()
        {
            GUILayout.EndVertical();
            EditorGUI.EndDisabled();
        }

        /// <summary>
        /// <para>Close a group started with BeginVertical.</para>
        /// </summary>
        public static void EndVertical()
        {
            GUILayout.EndVertical();
        }

        /// <summary>
        /// <para>Make a field for enum based masks.</para>
        /// </summary>
        /// <param name="label">Prefix label for this field.</param>
        /// <param name="enumValue">Enum to use for the flags.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        /// <returns>
        /// <para>The value modified by the user.</para>
        /// </returns>
        public static Enum EnumMaskField(Enum enumValue, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, EditorStyles.popup, options);
            return EditorGUI.EnumMaskField(position, enumValue, EditorStyles.popup);
        }

        /// <summary>
        /// <para>Make a field for enum based masks.</para>
        /// </summary>
        /// <param name="label">Prefix label for this field.</param>
        /// <param name="enumValue">Enum to use for the flags.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        /// <returns>
        /// <para>The value modified by the user.</para>
        /// </returns>
        public static Enum EnumMaskField(Enum enumValue, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.EnumMaskField(position, enumValue, style);
        }

        /// <summary>
        /// <para>Make a field for enum based masks.</para>
        /// </summary>
        /// <param name="label">Prefix label for this field.</param>
        /// <param name="enumValue">Enum to use for the flags.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        /// <returns>
        /// <para>The value modified by the user.</para>
        /// </returns>
        public static Enum EnumMaskField(string label, Enum enumValue, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.popup, options);
            return EditorGUI.EnumMaskField(position, label, enumValue, EditorStyles.popup);
        }

        /// <summary>
        /// <para>Make a field for enum based masks.</para>
        /// </summary>
        /// <param name="label">Prefix label for this field.</param>
        /// <param name="enumValue">Enum to use for the flags.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        /// <returns>
        /// <para>The value modified by the user.</para>
        /// </returns>
        public static Enum EnumMaskField(GUIContent label, Enum enumValue, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.popup, options);
            return EditorGUI.EnumMaskField(position, label, enumValue, EditorStyles.popup);
        }

        /// <summary>
        /// <para>Make a field for enum based masks.</para>
        /// </summary>
        /// <param name="label">Prefix label for this field.</param>
        /// <param name="enumValue">Enum to use for the flags.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        /// <returns>
        /// <para>The value modified by the user.</para>
        /// </returns>
        public static Enum EnumMaskField(string label, Enum enumValue, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.EnumMaskField(position, label, enumValue, style);
        }

        /// <summary>
        /// <para>Make a field for enum based masks.</para>
        /// </summary>
        /// <param name="label">Prefix label for this field.</param>
        /// <param name="enumValue">Enum to use for the flags.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        /// <returns>
        /// <para>The value modified by the user.</para>
        /// </returns>
        public static Enum EnumMaskField(GUIContent label, Enum enumValue, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.EnumMaskField(position, label, enumValue, style);
        }

        /// <summary>
        /// <para>Make an enum popup selection field for a bitmask.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selected">The enum options the field shows.</param>
        /// <param name="options">Optional layout options.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <returns>
        /// <para>The enum options that has been selected by the user.</para>
        /// </returns>
        public static Enum EnumMaskPopup(string label, Enum selected, params GUILayoutOption[] options)
        {
            int num;
            bool flag;
            return EnumMaskPopup(EditorGUIUtility.TempContent(label), selected, out num, out flag, options);
        }

        /// <summary>
        /// <para>Make an enum popup selection field for a bitmask.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selected">The enum options the field shows.</param>
        /// <param name="options">Optional layout options.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <returns>
        /// <para>The enum options that has been selected by the user.</para>
        /// </returns>
        public static Enum EnumMaskPopup(GUIContent label, Enum selected, params GUILayoutOption[] options)
        {
            int num;
            bool flag;
            return EnumMaskPopup(label, selected, out num, out flag, options);
        }

        /// <summary>
        /// <para>Make an enum popup selection field for a bitmask.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selected">The enum options the field shows.</param>
        /// <param name="options">Optional layout options.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <returns>
        /// <para>The enum options that has been selected by the user.</para>
        /// </returns>
        public static Enum EnumMaskPopup(string label, Enum selected, GUIStyle style, params GUILayoutOption[] options)
        {
            int num;
            bool flag;
            return EnumMaskPopup(EditorGUIUtility.TempContent(label), selected, out num, out flag, style, options);
        }

        /// <summary>
        /// <para>Make an enum popup selection field for a bitmask.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selected">The enum options the field shows.</param>
        /// <param name="options">Optional layout options.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <returns>
        /// <para>The enum options that has been selected by the user.</para>
        /// </returns>
        public static Enum EnumMaskPopup(GUIContent label, Enum selected, GUIStyle style, params GUILayoutOption[] options)
        {
            int num;
            bool flag;
            return EnumMaskPopup(label, selected, out num, out flag, style, options);
        }

        internal static Enum EnumMaskPopup(string label, Enum selected, out int changedFlags, out bool changedToValue, params GUILayoutOption[] options) => 
            EnumMaskPopup(EditorGUIUtility.TempContent(label), selected, out changedFlags, out changedToValue, options);

        internal static Enum EnumMaskPopup(GUIContent label, Enum selected, out int changedFlags, out bool changedToValue, params GUILayoutOption[] options) => 
            EnumMaskPopup(label, selected, out changedFlags, out changedToValue, EditorStyles.popup, options);

        internal static Enum EnumMaskPopup(string label, Enum selected, out int changedFlags, out bool changedToValue, GUIStyle style, params GUILayoutOption[] options) => 
            EnumMaskPopup(EditorGUIUtility.TempContent(label), selected, out changedFlags, out changedToValue, style, options);

        internal static Enum EnumMaskPopup(GUIContent label, Enum selected, out int changedFlags, out bool changedToValue, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.EnumMaskPopup(position, label, selected, out changedFlags, out changedToValue, style);
        }

        /// <summary>
        /// <para>Make an enum popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selected">The enum option the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The enum option that has been selected by the user.</para>
        /// </returns>
        public static Enum EnumPopup(Enum selected, params GUILayoutOption[] options) => 
            EnumPopup(selected, EditorStyles.popup, options);

        /// <summary>
        /// <para>Make an enum popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selected">The enum option the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The enum option that has been selected by the user.</para>
        /// </returns>
        public static Enum EnumPopup(Enum selected, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.EnumPopup(position, selected, style);
        }

        /// <summary>
        /// <para>Make an enum popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selected">The enum option the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The enum option that has been selected by the user.</para>
        /// </returns>
        public static Enum EnumPopup(string label, Enum selected, params GUILayoutOption[] options) => 
            EnumPopup(label, selected, EditorStyles.popup, options);

        /// <summary>
        /// <para>Make an enum popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selected">The enum option the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The enum option that has been selected by the user.</para>
        /// </returns>
        public static Enum EnumPopup(GUIContent label, Enum selected, params GUILayoutOption[] options) => 
            EnumPopup(label, selected, EditorStyles.popup, options);

        /// <summary>
        /// <para>Make an enum popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selected">The enum option the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The enum option that has been selected by the user.</para>
        /// </returns>
        public static Enum EnumPopup(string label, Enum selected, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.EnumPopup(position, GUIContent.Temp(label), selected, style);
        }

        /// <summary>
        /// <para>Make an enum popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selected">The enum option the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The enum option that has been selected by the user.</para>
        /// </returns>
        public static Enum EnumPopup(GUIContent label, Enum selected, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.EnumPopup(position, label, selected, style);
        }

        /// <summary>
        /// <para>Make a text field for entering float values.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static float FloatField(float value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, EditorStyles.numberField, options);
            return EditorGUI.FloatField(position, value);
        }

        /// <summary>
        /// <para>Make a text field for entering float values.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static float FloatField(float value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.FloatField(position, value, style);
        }

        /// <summary>
        /// <para>Make a text field for entering float values.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static float FloatField(string label, float value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.numberField, options);
            return EditorGUI.FloatField(position, label, value);
        }

        /// <summary>
        /// <para>Make a text field for entering float values.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static float FloatField(GUIContent label, float value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.numberField, options);
            return EditorGUI.FloatField(position, label, value);
        }

        /// <summary>
        /// <para>Make a text field for entering float values.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static float FloatField(string label, float value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.FloatField(position, label, value, style);
        }

        /// <summary>
        /// <para>Make a text field for entering float values.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the float field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static float FloatField(GUIContent label, float value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.FloatField(position, label, value, style);
        }

        /// <summary>
        /// <para>Make a label with a foldout arrow to the left of it.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="content">The label to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <returns>
        /// <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>
        [ExcludeFromDocs]
        public static bool Foldout(bool foldout, string content)
        {
            GUIStyle style = EditorStyles.foldout;
            return Foldout(foldout, content, style);
        }

        /// <summary>
        /// <para>Make a label with a foldout arrow to the left of it.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="content">The label to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <returns>
        /// <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>
        [ExcludeFromDocs]
        public static bool Foldout(bool foldout, GUIContent content)
        {
            GUIStyle style = EditorStyles.foldout;
            return Foldout(foldout, content, style);
        }

        [ExcludeFromDocs]
        public static bool Foldout(bool foldout, string content, bool toggleOnLabelClick)
        {
            GUIStyle style = EditorStyles.foldout;
            return Foldout(foldout, content, toggleOnLabelClick, style);
        }

        /// <summary>
        /// <para>Make a label with a foldout arrow to the left of it.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="content">The label to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <returns>
        /// <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>
        public static bool Foldout(bool foldout, string content, [DefaultValue("EditorStyles.foldout")] GUIStyle style) => 
            Foldout(foldout, EditorGUIUtility.TempContent(content), false, style);

        [ExcludeFromDocs]
        public static bool Foldout(bool foldout, GUIContent content, bool toggleOnLabelClick)
        {
            GUIStyle style = EditorStyles.foldout;
            return Foldout(foldout, content, toggleOnLabelClick, style);
        }

        /// <summary>
        /// <para>Make a label with a foldout arrow to the left of it.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="content">The label to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <returns>
        /// <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>
        public static bool Foldout(bool foldout, GUIContent content, [DefaultValue("EditorStyles.foldout")] GUIStyle style) => 
            Foldout(foldout, content, false, style);

        /// <summary>
        /// <para>Make a label with a foldout arrow to the left of it.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="content">The label to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <returns>
        /// <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>
        public static bool Foldout(bool foldout, string content, bool toggleOnLabelClick, [DefaultValue("EditorStyles.foldout")] GUIStyle style) => 
            Foldout(foldout, EditorGUIUtility.TempContent(content), toggleOnLabelClick, style);

        /// <summary>
        /// <para>Make a label with a foldout arrow to the left of it.</para>
        /// </summary>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="content">The label to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Whether to toggle the foldout state when the label is clicked.</param>
        /// <returns>
        /// <para>The foldout state selected by the user. If true, you should render sub-objects.</para>
        /// </returns>
        public static bool Foldout(bool foldout, GUIContent content, bool toggleOnLabelClick, [DefaultValue("EditorStyles.foldout")] GUIStyle style) => 
            FoldoutInternal(foldout, content, toggleOnLabelClick, style);

        internal static bool FoldoutInternal(bool foldout, GUIContent content, bool toggleOnLabelClick, GUIStyle style)
        {
            Rect position = s_LastRect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUIUtility.fieldWidth, 16f, 16f, style);
            return EditorGUI.Foldout(position, foldout, content, toggleOnLabelClick, style);
        }

        internal static bool FoldoutTitlebar(bool foldout, GUIContent label) => 
            EditorGUI.FoldoutTitlebar(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.inspectorTitlebar), label, foldout);

        internal static void GameViewSizePopup(GameViewSizeGroupType groupType, int selectedIndex, IGameViewSizeMenuUser gameView, GUIStyle style, params GUILayoutOption[] options)
        {
            s_LastRect = GetControlRect(false, 16f, style, options);
            EditorGUI.GameViewSizePopup(s_LastRect, groupType, selectedIndex, gameView, style);
        }

        /// <summary>
        /// <para>Get a rect for an Editor control.</para>
        /// </summary>
        /// <param name="hasLabel">Optional boolean to specify if the control has a label. Default is true.</param>
        /// <param name="height">The height in pixels of the control. Default is EditorGUIUtility.singleLineHeight.</param>
        /// <param name="style">Optional GUIStyle to use for the control.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static Rect GetControlRect(params GUILayoutOption[] options) => 
            GetControlRect(true, 16f, EditorStyles.layerMaskField, options);

        /// <summary>
        /// <para>Get a rect for an Editor control.</para>
        /// </summary>
        /// <param name="hasLabel">Optional boolean to specify if the control has a label. Default is true.</param>
        /// <param name="height">The height in pixels of the control. Default is EditorGUIUtility.singleLineHeight.</param>
        /// <param name="style">Optional GUIStyle to use for the control.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static Rect GetControlRect(bool hasLabel, params GUILayoutOption[] options) => 
            GetControlRect(hasLabel, 16f, EditorStyles.layerMaskField, options);

        /// <summary>
        /// <para>Get a rect for an Editor control.</para>
        /// </summary>
        /// <param name="hasLabel">Optional boolean to specify if the control has a label. Default is true.</param>
        /// <param name="height">The height in pixels of the control. Default is EditorGUIUtility.singleLineHeight.</param>
        /// <param name="style">Optional GUIStyle to use for the control.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static Rect GetControlRect(bool hasLabel, float height, params GUILayoutOption[] options) => 
            GetControlRect(hasLabel, height, EditorStyles.layerMaskField, options);

        /// <summary>
        /// <para>Get a rect for an Editor control.</para>
        /// </summary>
        /// <param name="hasLabel">Optional boolean to specify if the control has a label. Default is true.</param>
        /// <param name="height">The height in pixels of the control. Default is EditorGUIUtility.singleLineHeight.</param>
        /// <param name="style">Optional GUIStyle to use for the control.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static Rect GetControlRect(bool hasLabel, float height, GUIStyle style, params GUILayoutOption[] options) => 
            GUILayoutUtility.GetRect(!hasLabel ? EditorGUIUtility.fieldWidth : kLabelFloatMinW, kLabelFloatMaxW, height, height, style, options);

        internal static Rect GetSliderRect(bool hasLabel, params GUILayoutOption[] options) => 
            GUILayoutUtility.GetRect(!hasLabel ? EditorGUIUtility.fieldWidth : kLabelFloatMinW, (kLabelFloatMaxW + 5f) + 100f, 16f, 16f, EditorStyles.numberField, options);

        internal static Rect GetToggleRect(bool hasLabel, params GUILayoutOption[] options)
        {
            float num = 10f - EditorGUIUtility.fieldWidth;
            return GUILayoutUtility.GetRect(!hasLabel ? (EditorGUIUtility.fieldWidth + num) : (kLabelFloatMinW + num), kLabelFloatMaxW + num, 16f, 16f, EditorStyles.numberField, options);
        }

        internal static Gradient GradientField(SerializedProperty value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, 16f, 16f, EditorStyles.colorField, options);
            return EditorGUI.GradientField(position, value);
        }

        internal static Gradient GradientField(Gradient value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, kLabelFloatMaxW, 16f, 16f, EditorStyles.colorField, options);
            return EditorGUI.GradientField(position, value);
        }

        internal static Gradient GradientField(string label, SerializedProperty value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, 16f, 16f, EditorStyles.colorField, options);
            return EditorGUI.GradientField(label, position, value);
        }

        internal static Gradient GradientField(string label, Gradient value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, 16f, 16f, EditorStyles.colorField, options);
            return EditorGUI.GradientField(label, position, value);
        }

        internal static Gradient GradientField(GUIContent label, SerializedProperty value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, 16f, 16f, EditorStyles.colorField, options);
            return EditorGUI.GradientField(label, position, value);
        }

        internal static Gradient GradientField(GUIContent label, Gradient value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, 16f, 16f, EditorStyles.colorField, options);
            return EditorGUI.GradientField(label, position, value);
        }

        /// <summary>
        /// <para>Make a help box with a message to the user.</para>
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <param name="type">The type of message.</param>
        /// <param name="wide">If true, the box will cover the whole width of the window; otherwise it will cover the controls part only.</param>
        public static void HelpBox(string message, MessageType type)
        {
            LabelField(GUIContent.none, EditorGUIUtility.TempContent(message, EditorGUIUtility.GetHelpIcon(type)), EditorStyles.helpBox, new GUILayoutOption[0]);
        }

        /// <summary>
        /// <para>Make a help box with a message to the user.</para>
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <param name="type">The type of message.</param>
        /// <param name="wide">If true, the box will cover the whole width of the window; otherwise it will cover the controls part only.</param>
        public static void HelpBox(string message, MessageType type, bool wide)
        {
            LabelField(!wide ? EditorGUIUtility.blankContent : GUIContent.none, EditorGUIUtility.TempContent(message, EditorGUIUtility.GetHelpIcon(type)), EditorStyles.helpBox, new GUILayoutOption[0]);
        }

        internal static Color HexColorTextField(GUIContent label, Color color, bool showAlpha, params GUILayoutOption[] options) => 
            HexColorTextField(label, color, showAlpha, EditorStyles.textField, options);

        internal static Color HexColorTextField(GUIContent label, Color color, bool showAlpha, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect rect = s_LastRect = GetControlRect(true, 16f, EditorStyles.numberField, options);
            return EditorGUI.HexColorTextField(rect, label, color, showAlpha, style);
        }

        internal static bool IconButton(int id, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            s_LastRect = GUILayoutUtility.GetRect(content, style, options);
            return EditorGUI.IconButton(id, s_LastRect, content, style);
        }

        public static void InspectorTitlebar(Object[] targetObjs)
        {
            EditorGUI.InspectorTitlebar(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.inspectorTitlebar), targetObjs);
        }

        /// <summary>
        /// <para>Make an inspector-window-like titlebar.</para>
        /// </summary>
        /// <param name="foldout">The foldout state shown with the arrow.</param>
        /// <param name="targetObj">The object (for example a component) or objects that the titlebar is for.</param>
        /// <param name="targetObjs"></param>
        /// <returns>
        /// <para>The foldout state selected by the user.</para>
        /// </returns>
        public static bool InspectorTitlebar(bool foldout, Object targetObj) => 
            InspectorTitlebar(foldout, targetObj, true);

        /// <summary>
        /// <para>Make an inspector-window-like titlebar.</para>
        /// </summary>
        /// <param name="foldout">The foldout state shown with the arrow.</param>
        /// <param name="targetObj">The object (for example a component) or objects that the titlebar is for.</param>
        /// <param name="targetObjs"></param>
        /// <returns>
        /// <para>The foldout state selected by the user.</para>
        /// </returns>
        public static bool InspectorTitlebar(bool foldout, Object[] targetObjs) => 
            InspectorTitlebar(foldout, targetObjs, true);

        public static bool InspectorTitlebar(bool foldout, Object targetObj, bool expandable) => 
            EditorGUI.InspectorTitlebar(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.inspectorTitlebar), foldout, targetObj, expandable);

        public static bool InspectorTitlebar(bool foldout, Object[] targetObjs, bool expandable) => 
            EditorGUI.InspectorTitlebar(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.inspectorTitlebar), foldout, targetObjs, expandable);

        /// <summary>
        /// <para>Make a text field for entering integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static int IntField(int value, params GUILayoutOption[] options) => 
            IntField(value, EditorStyles.numberField, options);

        /// <summary>
        /// <para>Make a text field for entering integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static int IntField(int value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.IntField(position, value, style);
        }

        /// <summary>
        /// <para>Make a text field for entering integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static int IntField(string label, int value, params GUILayoutOption[] options) => 
            IntField(label, value, EditorStyles.numberField, options);

        /// <summary>
        /// <para>Make a text field for entering integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static int IntField(GUIContent label, int value, params GUILayoutOption[] options) => 
            IntField(label, value, EditorStyles.numberField, options);

        /// <summary>
        /// <para>Make a text field for entering integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static int IntField(string label, int value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.IntField(position, label, value, style);
        }

        /// <summary>
        /// <para>Make a text field for entering integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the int field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static int IntField(GUIContent label, int value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.IntField(position, label, value, style);
        }

        /// <summary>
        /// <para>Make an integer popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selectedValue">The value of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the displayed options the user can choose from.</param>
        /// <param name="optionValues">An array with the values for each option.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value of the option that has been selected by the user.</para>
        /// </returns>
        public static int IntPopup(int selectedValue, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options) => 
            IntPopup(selectedValue, displayedOptions, optionValues, EditorStyles.popup, options);

        /// <summary>
        /// <para>Make an integer popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selectedValue">The value of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the displayed options the user can choose from.</param>
        /// <param name="optionValues">An array with the values for each option.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value of the option that has been selected by the user.</para>
        /// </returns>
        public static int IntPopup(int selectedValue, GUIContent[] displayedOptions, int[] optionValues, params GUILayoutOption[] options) => 
            IntPopup(selectedValue, displayedOptions, optionValues, EditorStyles.popup, options);

        /// <summary>
        /// <para>Make an integer popup selection field.</para>
        /// </summary>
        /// <param name="property">The value of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the displayed options the user can choose from.</param>
        /// <param name="optionValues">An array with the values for each option.</param>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public static void IntPopup(SerializedProperty property, GUIContent[] displayedOptions, int[] optionValues, params GUILayoutOption[] options)
        {
            IntPopup(property, displayedOptions, optionValues, null, options);
        }

        /// <summary>
        /// <para>Make an integer popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selectedValue">The value of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the displayed options the user can choose from.</param>
        /// <param name="optionValues">An array with the values for each option.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value of the option that has been selected by the user.</para>
        /// </returns>
        public static int IntPopup(int selectedValue, string[] displayedOptions, int[] optionValues, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.IntPopup(position, selectedValue, displayedOptions, optionValues, style);
        }

        /// <summary>
        /// <para>Make an integer popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selectedValue">The value of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the displayed options the user can choose from.</param>
        /// <param name="optionValues">An array with the values for each option.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value of the option that has been selected by the user.</para>
        /// </returns>
        public static int IntPopup(int selectedValue, GUIContent[] displayedOptions, int[] optionValues, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.IntPopup(position, GUIContent.none, selectedValue, displayedOptions, optionValues, style);
        }

        /// <summary>
        /// <para>Make an integer popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selectedValue">The value of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the displayed options the user can choose from.</param>
        /// <param name="optionValues">An array with the values for each option.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value of the option that has been selected by the user.</para>
        /// </returns>
        public static int IntPopup(string label, int selectedValue, string[] displayedOptions, int[] optionValues, params GUILayoutOption[] options) => 
            IntPopup(label, selectedValue, displayedOptions, optionValues, EditorStyles.popup, options);

        /// <summary>
        /// <para>Make an integer popup selection field.</para>
        /// </summary>
        /// <param name="property">The value of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the displayed options the user can choose from.</param>
        /// <param name="optionValues">An array with the values for each option.</param>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public static void IntPopup(SerializedProperty property, GUIContent[] displayedOptions, int[] optionValues, GUIContent label, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.popup, options);
            EditorGUI.IntPopup(position, property, displayedOptions, optionValues, label);
        }

        /// <summary>
        /// <para>Make an integer popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selectedValue">The value of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the displayed options the user can choose from.</param>
        /// <param name="optionValues">An array with the values for each option.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value of the option that has been selected by the user.</para>
        /// </returns>
        public static int IntPopup(GUIContent label, int selectedValue, GUIContent[] displayedOptions, int[] optionValues, params GUILayoutOption[] options) => 
            IntPopup(label, selectedValue, displayedOptions, optionValues, EditorStyles.popup, options);

        /// <summary>
        /// <para>Make an integer popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selectedValue">The value of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the displayed options the user can choose from.</param>
        /// <param name="optionValues">An array with the values for each option.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value of the option that has been selected by the user.</para>
        /// </returns>
        public static int IntPopup(string label, int selectedValue, string[] displayedOptions, int[] optionValues, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.IntPopup(position, label, selectedValue, displayedOptions, optionValues, style);
        }

        /// <summary>
        /// <para>Make an integer popup selection field.</para>
        /// </summary>
        /// <param name="property">The value of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the displayed options the user can choose from.</param>
        /// <param name="optionValues">An array with the values for each option.</param>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        [Obsolete("This function is obsolete and the style is not used.")]
        public static void IntPopup(SerializedProperty property, GUIContent[] displayedOptions, int[] optionValues, GUIContent label, GUIStyle style, params GUILayoutOption[] options)
        {
            IntPopup(property, displayedOptions, optionValues, label, options);
        }

        /// <summary>
        /// <para>Make an integer popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selectedValue">The value of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the displayed options the user can choose from.</param>
        /// <param name="optionValues">An array with the values for each option.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value of the option that has been selected by the user.</para>
        /// </returns>
        public static int IntPopup(GUIContent label, int selectedValue, GUIContent[] displayedOptions, int[] optionValues, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.IntPopup(position, label, selectedValue, displayedOptions, optionValues, style);
        }

        /// <summary>
        /// <para>Make a slider the user can drag to change an integer value between a min and a max.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the slider.</param>
        /// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value that has been set by the user.</para>
        /// </returns>
        public static int IntSlider(int value, int leftValue, int rightValue, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetSliderRect(false, options);
            return EditorGUI.IntSlider(position, value, leftValue, rightValue);
        }

        /// <summary>
        /// <para>Make a slider the user can drag to change an integer value between a min and a max.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the slider.</param>
        /// <param name="property">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void IntSlider(SerializedProperty property, int leftValue, int rightValue, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetSliderRect(false, options);
            EditorGUI.IntSlider(position, property, leftValue, rightValue, property.displayName);
        }

        /// <summary>
        /// <para>Make a slider the user can drag to change an integer value between a min and a max.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the slider.</param>
        /// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value that has been set by the user.</para>
        /// </returns>
        public static int IntSlider(string label, int value, int leftValue, int rightValue, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetSliderRect(true, options);
            return EditorGUI.IntSlider(position, label, value, leftValue, rightValue);
        }

        /// <summary>
        /// <para>Make a slider the user can drag to change an integer value between a min and a max.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the slider.</param>
        /// <param name="property">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void IntSlider(SerializedProperty property, int leftValue, int rightValue, string label, params GUILayoutOption[] options)
        {
            IntSlider(property, leftValue, rightValue, EditorGUIUtility.TempContent(label), options);
        }

        /// <summary>
        /// <para>Make a slider the user can drag to change an integer value between a min and a max.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the slider.</param>
        /// <param name="property">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void IntSlider(SerializedProperty property, int leftValue, int rightValue, GUIContent label, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetSliderRect(true, options);
            EditorGUI.IntSlider(position, property, leftValue, rightValue, label);
        }

        /// <summary>
        /// <para>Make a slider the user can drag to change an integer value between a min and a max.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the slider.</param>
        /// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value that has been set by the user.</para>
        /// </returns>
        public static int IntSlider(GUIContent label, int value, int leftValue, int rightValue, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetSliderRect(true, options);
            return EditorGUI.IntSlider(position, label, value, leftValue, rightValue);
        }

        internal static Event KeyEventField(Event e, params GUILayoutOption[] options) => 
            EditorGUI.KeyEventField(GUILayoutUtility.GetRect(EditorGUIUtility.TempContent("[Please press a key]"), GUI.skin.textField, options), e);

        public static float Knob(Vector2 knobSize, float value, float minValue, float maxValue, string unit, Color backgroundColor, Color activeColor, bool showValue, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, knobSize.y, options);
            return EditorGUI.Knob(position, knobSize, value, minValue, maxValue, unit, backgroundColor, activeColor, showValue, GUIUtility.GetControlID("Knob".GetHashCode(), FocusType.Passive, position));
        }

        /// <summary>
        /// <para>Make a label field. (Useful for showing read-only info.)</para>
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="label2">The label to show to the right.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public static void LabelField(string label, params GUILayoutOption[] options)
        {
            LabelField(GUIContent.none, EditorGUIUtility.TempContent(label), EditorStyles.label, options);
        }

        /// <summary>
        /// <para>Make a label field. (Useful for showing read-only info.)</para>
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="label2">The label to show to the right.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public static void LabelField(GUIContent label, params GUILayoutOption[] options)
        {
            LabelField(GUIContent.none, label, EditorStyles.label, options);
        }

        /// <summary>
        /// <para>Make a label field. (Useful for showing read-only info.)</para>
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="label2">The label to show to the right.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public static void LabelField(string label, string label2, params GUILayoutOption[] options)
        {
            LabelField(new GUIContent(label), EditorGUIUtility.TempContent(label2), EditorStyles.label, options);
        }

        /// <summary>
        /// <para>Make a label field. (Useful for showing read-only info.)</para>
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="label2">The label to show to the right.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public static void LabelField(string label, GUIStyle style, params GUILayoutOption[] options)
        {
            LabelField(GUIContent.none, EditorGUIUtility.TempContent(label), style, options);
        }

        /// <summary>
        /// <para>Make a label field. (Useful for showing read-only info.)</para>
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="label2">The label to show to the right.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public static void LabelField(GUIContent label, GUIContent label2, params GUILayoutOption[] options)
        {
            LabelField(label, label2, EditorStyles.label, options);
        }

        /// <summary>
        /// <para>Make a label field. (Useful for showing read-only info.)</para>
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="label2">The label to show to the right.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public static void LabelField(GUIContent label, GUIStyle style, params GUILayoutOption[] options)
        {
            LabelField(GUIContent.none, label, style, options);
        }

        /// <summary>
        /// <para>Make a label field. (Useful for showing read-only info.)</para>
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="label2">The label to show to the right.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public static void LabelField(string label, string label2, GUIStyle style, params GUILayoutOption[] options)
        {
            LabelField(new GUIContent(label), EditorGUIUtility.TempContent(label2), style, options);
        }

        /// <summary>
        /// <para>Make a label field. (Useful for showing read-only info.)</para>
        /// </summary>
        /// <param name="label">Label in front of the label field.</param>
        /// <param name="label2">The label to show to the right.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="style"></param>
        public static void LabelField(GUIContent label, GUIContent label2, GUIStyle style, params GUILayoutOption[] options)
        {
            if (!style.wordWrap)
            {
                Rect position = s_LastRect = GetControlRect(true, 16f, options);
                EditorGUI.LabelField(position, label, label2, style);
            }
            else
            {
                BeginHorizontal(new GUILayoutOption[0]);
                PrefixLabel(label, style);
                Rect rect2 = GUILayoutUtility.GetRect(label2, style, options);
                int indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                EditorGUI.LabelField(rect2, label2, style);
                EditorGUI.indentLevel = indentLevel;
                EndHorizontal();
            }
        }

        /// <summary>
        /// <para>Make a layer selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="layer">The layer shown in the field.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The layer selected by the user.</para>
        /// </returns>
        public static int LayerField(int layer, params GUILayoutOption[] options) => 
            LayerField(layer, EditorStyles.popup, options);

        /// <summary>
        /// <para>Make a layer selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="layer">The layer shown in the field.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The layer selected by the user.</para>
        /// </returns>
        public static int LayerField(int layer, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.LayerField(position, layer, style);
        }

        /// <summary>
        /// <para>Make a layer selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="layer">The layer shown in the field.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The layer selected by the user.</para>
        /// </returns>
        public static int LayerField(string label, int layer, params GUILayoutOption[] options) => 
            LayerField(EditorGUIUtility.TempContent(label), layer, EditorStyles.popup, options);

        /// <summary>
        /// <para>Make a layer selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="layer">The layer shown in the field.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The layer selected by the user.</para>
        /// </returns>
        public static int LayerField(GUIContent label, int layer, params GUILayoutOption[] options) => 
            LayerField(label, layer, EditorStyles.popup, options);

        /// <summary>
        /// <para>Make a layer selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="layer">The layer shown in the field.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The layer selected by the user.</para>
        /// </returns>
        public static int LayerField(string label, int layer, GUIStyle style, params GUILayoutOption[] options) => 
            LayerField(EditorGUIUtility.TempContent(label), layer, style, options);

        /// <summary>
        /// <para>Make a layer selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="layer">The layer shown in the field.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The layer selected by the user.</para>
        /// </returns>
        public static int LayerField(GUIContent label, int layer, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.LayerField(position, label, layer, style);
        }

        internal static void LayerMaskField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, options);
            EditorGUI.LayerMaskField(position, property, label);
        }

        internal static bool LinkLabel(string label, params GUILayoutOption[] options) => 
            LinkLabel(EditorGUIUtility.TempContent(label), options);

        internal static bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GUILayoutUtility.GetRect(label, EditorStyles.linkLabel, options);
            Handles.BeginGUI();
            Handles.color = EditorStyles.linkLabel.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();
            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            return GUI.Button(position, label, EditorStyles.linkLabel);
        }

        /// <summary>
        /// <para>Make a text field for entering long integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the long field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static long LongField(long value, params GUILayoutOption[] options) => 
            LongField(value, EditorStyles.numberField, options);

        /// <summary>
        /// <para>Make a text field for entering long integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the long field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static long LongField(long value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.LongField(position, value, style);
        }

        /// <summary>
        /// <para>Make a text field for entering long integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the long field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static long LongField(string label, long value, params GUILayoutOption[] options) => 
            LongField(label, value, EditorStyles.numberField, options);

        /// <summary>
        /// <para>Make a text field for entering long integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the long field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static long LongField(GUIContent label, long value, params GUILayoutOption[] options) => 
            LongField(label, value, EditorStyles.numberField, options);

        /// <summary>
        /// <para>Make a text field for entering long integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the long field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static long LongField(string label, long value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.LongField(position, label, value, style);
        }

        /// <summary>
        /// <para>Make a text field for entering long integers.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the long field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static long LongField(GUIContent label, long value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.LongField(position, label, value, style);
        }

        /// <summary>
        /// <para>Make a field for masks.</para>
        /// </summary>
        /// <param name="label">Prefix label of the field.</param>
        /// <param name="mask">The current mask to display.</param>
        /// <param name="displayedOption">A string array containing the labels for each flag.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="displayedOptions"></param>
        /// <param name="style"></param>
        /// <returns>
        /// <para>The value modified by the user.</para>
        /// </returns>
        public static int MaskField(int mask, string[] displayedOptions, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, EditorStyles.popup, options);
            return EditorGUI.MaskField(position, mask, displayedOptions, EditorStyles.popup);
        }

        /// <summary>
        /// <para>Make a field for masks.</para>
        /// </summary>
        /// <param name="label">Prefix label of the field.</param>
        /// <param name="mask">The current mask to display.</param>
        /// <param name="displayedOption">A string array containing the labels for each flag.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="displayedOptions"></param>
        /// <param name="style"></param>
        /// <returns>
        /// <para>The value modified by the user.</para>
        /// </returns>
        public static int MaskField(int mask, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.MaskField(position, mask, displayedOptions, style);
        }

        /// <summary>
        /// <para>Make a field for masks.</para>
        /// </summary>
        /// <param name="label">Prefix label of the field.</param>
        /// <param name="mask">The current mask to display.</param>
        /// <param name="displayedOption">A string array containing the labels for each flag.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="displayedOptions"></param>
        /// <param name="style"></param>
        /// <returns>
        /// <para>The value modified by the user.</para>
        /// </returns>
        public static int MaskField(string label, int mask, string[] displayedOptions, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.popup, options);
            return EditorGUI.MaskField(position, label, mask, displayedOptions, EditorStyles.popup);
        }

        /// <summary>
        /// <para>Make a field for masks.</para>
        /// </summary>
        /// <param name="label">Prefix label of the field.</param>
        /// <param name="mask">The current mask to display.</param>
        /// <param name="displayedOption">A string array containing the labels for each flag.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="displayedOptions"></param>
        /// <param name="style"></param>
        /// <returns>
        /// <para>The value modified by the user.</para>
        /// </returns>
        public static int MaskField(GUIContent label, int mask, string[] displayedOptions, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.popup, options);
            return EditorGUI.MaskField(position, label, mask, displayedOptions, EditorStyles.popup);
        }

        /// <summary>
        /// <para>Make a field for masks.</para>
        /// </summary>
        /// <param name="label">Prefix label of the field.</param>
        /// <param name="mask">The current mask to display.</param>
        /// <param name="displayedOption">A string array containing the labels for each flag.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="displayedOptions"></param>
        /// <param name="style"></param>
        /// <returns>
        /// <para>The value modified by the user.</para>
        /// </returns>
        public static int MaskField(string label, int mask, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.MaskField(position, label, mask, displayedOptions, style);
        }

        /// <summary>
        /// <para>Make a field for masks.</para>
        /// </summary>
        /// <param name="label">Prefix label of the field.</param>
        /// <param name="mask">The current mask to display.</param>
        /// <param name="displayedOption">A string array containing the labels for each flag.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <param name="displayedOptions"></param>
        /// <param name="style"></param>
        /// <returns>
        /// <para>The value modified by the user.</para>
        /// </returns>
        public static int MaskField(GUIContent label, int mask, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.MaskField(position, label, mask, displayedOptions, style);
        }

        internal static Object MiniThumbnailObjectField(GUIContent label, Object obj, Type objType, EditorGUI.ObjectFieldValidator validator, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, options);
            return EditorGUI.MiniThumbnailObjectField(position, label, obj, objType, validator);
        }

        public static void MinMaxSlider(ref float minValue, ref float maxValue, float minLimit, float maxLimit, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetSliderRect(false, options);
            EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, minLimit, maxLimit);
        }

        public static void MinMaxSlider(string label, ref float minValue, ref float maxValue, float minLimit, float maxLimit, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetSliderRect(true, options);
            EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, minLimit, maxLimit);
        }

        public static void MinMaxSlider(GUIContent label, ref float minValue, ref float maxValue, float minLimit, float maxLimit, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetSliderRect(true, options);
            EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, minLimit, maxLimit);
        }

        internal static void MultiSelectionObjectTitleBar(Object[] objects)
        {
            string t = objects[0].name + " (" + ObjectNames.NicifyVariableName(ObjectNames.GetTypeName(objects[0])) + ")";
            if (objects.Length > 1)
            {
                string str2 = t;
                object[] objArray1 = new object[] { str2, " and ", objects.Length - 1, " other", (objects.Length <= 2) ? "" : "s" };
                t = string.Concat(objArray1);
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(16f) };
            GUILayout.Label(EditorGUIUtility.TempContent(t, AssetPreview.GetMiniThumbnail(objects[0])), EditorStyles.boldLabel, options);
        }

        /// <summary>
        /// <para>Make a field to receive any object type.</para>
        /// </summary>
        /// <param name="property">The object reference property the field shows.</param>
        /// <param name="objType">The type of the objects that can be assigned.</param>
        /// <param name="label">Optional label in front of the field. Pass GUIContent.none to hide the label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        /// properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void ObjectField(SerializedProperty property, params GUILayoutOption[] options)
        {
            ObjectField(property, (GUIContent) null, options);
        }

        /// <summary>
        /// <para>Make a field to receive any object type.</para>
        /// </summary>
        /// <param name="property">The object reference property the field shows.</param>
        /// <param name="objType">The type of the objects that can be assigned.</param>
        /// <param name="label">Optional label in front of the field. Pass GUIContent.none to hide the label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        /// properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void ObjectField(SerializedProperty property, Type objType, params GUILayoutOption[] options)
        {
            ObjectField(property, objType, null, options);
        }

        /// <summary>
        /// <para>Make a field to receive any object type.</para>
        /// </summary>
        /// <param name="property">The object reference property the field shows.</param>
        /// <param name="objType">The type of the objects that can be assigned.</param>
        /// <param name="label">Optional label in front of the field. Pass GUIContent.none to hide the label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        /// properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void ObjectField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.objectField, options);
            EditorGUI.ObjectField(position, property, label);
        }

        /// <summary>
        /// <para>Make a field to receive any object type.</para>
        /// </summary>
        /// <param name="obj">The object the field shows.</param>
        /// <param name="objType">The type of the objects that can be assigned.</param>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="options">An optional list of layout options that specify extra layout properties. Any values passed in here will override settings defined by the style.
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        [Obsolete("Check the docs for the usage of the new parameter 'allowSceneObjects'.")]
        public static Object ObjectField(Object obj, Type objType, params GUILayoutOption[] options) => 
            ObjectField(obj, objType, true, options);

        /// <summary>
        /// <para>Make a field to receive any object type.</para>
        /// </summary>
        /// <param name="obj">The object the field shows.</param>
        /// <param name="objType">The type of the objects that can be assigned.</param>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="options">An optional list of layout options that specify extra layout properties. Any values passed in here will override settings defined by the style.
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        [Obsolete("Check the docs for the usage of the new parameter 'allowSceneObjects'.")]
        public static Object ObjectField(string label, Object obj, Type objType, params GUILayoutOption[] options) => 
            ObjectField(label, obj, objType, true, options);

        /// <summary>
        /// <para>Make a field to receive any object type.</para>
        /// </summary>
        /// <param name="property">The object reference property the field shows.</param>
        /// <param name="objType">The type of the objects that can be assigned.</param>
        /// <param name="label">Optional label in front of the field. Pass GUIContent.none to hide the label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        /// properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void ObjectField(SerializedProperty property, Type objType, GUIContent label, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.objectField, options);
            EditorGUI.ObjectField(position, property, objType, label);
        }

        /// <summary>
        /// <para>Make a field to receive any object type.</para>
        /// </summary>
        /// <param name="obj">The object the field shows.</param>
        /// <param name="objType">The type of the objects that can be assigned.</param>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="options">An optional list of layout options that specify extra layout properties. Any values passed in here will override settings defined by the style.
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        [Obsolete("Check the docs for the usage of the new parameter 'allowSceneObjects'.")]
        public static Object ObjectField(GUIContent label, Object obj, Type objType, params GUILayoutOption[] options) => 
            ObjectField(label, obj, objType, true, options);

        /// <summary>
        /// <para>Make a field to receive any object type.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="obj">The object the field shows.</param>
        /// <param name="objType">The type of the objects that can be assigned.</param>
        /// <param name="allowSceneObjects">Allow assigning scene objects. See Description for more info.</param>
        /// <param name="options">An optional list of layout options that specify extra layout properties. Any values passed in here will override settings defined by the style.
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The object that has been set by the user.</para>
        /// </returns>
        public static Object ObjectField(Object obj, Type objType, bool allowSceneObjects, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, options);
            return EditorGUI.ObjectField(position, obj, objType, allowSceneObjects);
        }

        /// <summary>
        /// <para>Make a field to receive any object type.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="obj">The object the field shows.</param>
        /// <param name="objType">The type of the objects that can be assigned.</param>
        /// <param name="allowSceneObjects">Allow assigning scene objects. See Description for more info.</param>
        /// <param name="options">An optional list of layout options that specify extra layout properties. Any values passed in here will override settings defined by the style.
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The object that has been set by the user.</para>
        /// </returns>
        public static Object ObjectField(string label, Object obj, Type objType, bool allowSceneObjects, params GUILayoutOption[] options) => 
            ObjectField(EditorGUIUtility.TempContent(label), obj, objType, allowSceneObjects, options);

        /// <summary>
        /// <para>Make a field to receive any object type.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="obj">The object the field shows.</param>
        /// <param name="objType">The type of the objects that can be assigned.</param>
        /// <param name="allowSceneObjects">Allow assigning scene objects. See Description for more info.</param>
        /// <param name="options">An optional list of layout options that specify extra layout properties. Any values passed in here will override settings defined by the style.
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The object that has been set by the user.</para>
        /// </returns>
        public static Object ObjectField(GUIContent label, Object obj, Type objType, bool allowSceneObjects, params GUILayoutOption[] options)
        {
            float num;
            if (EditorGUIUtility.HasObjectThumbnail(objType))
            {
                num = 64f;
            }
            else
            {
                num = 16f;
            }
            Rect position = s_LastRect = GetControlRect(true, num, options);
            return EditorGUI.ObjectField(position, label, obj, objType, allowSceneObjects);
        }

        /// <summary>
        /// <para>Make a text field where the user can enter a password.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the password field.</param>
        /// <param name="password">The password to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The password entered by the user.</para>
        /// </returns>
        public static string PasswordField(string password, params GUILayoutOption[] options) => 
            PasswordField(password, EditorStyles.textField, options);

        /// <summary>
        /// <para>Make a text field where the user can enter a password.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the password field.</param>
        /// <param name="password">The password to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The password entered by the user.</para>
        /// </returns>
        public static string PasswordField(string label, string password, params GUILayoutOption[] options) => 
            PasswordField(EditorGUIUtility.TempContent(label), password, EditorStyles.textField, options);

        /// <summary>
        /// <para>Make a text field where the user can enter a password.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the password field.</param>
        /// <param name="password">The password to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The password entered by the user.</para>
        /// </returns>
        public static string PasswordField(string password, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.PasswordField(position, password, style);
        }

        /// <summary>
        /// <para>Make a text field where the user can enter a password.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the password field.</param>
        /// <param name="password">The password to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The password entered by the user.</para>
        /// </returns>
        public static string PasswordField(GUIContent label, string password, params GUILayoutOption[] options) => 
            PasswordField(label, password, EditorStyles.textField, options);

        /// <summary>
        /// <para>Make a text field where the user can enter a password.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the password field.</param>
        /// <param name="password">The password to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The password entered by the user.</para>
        /// </returns>
        public static string PasswordField(string label, string password, GUIStyle style, params GUILayoutOption[] options) => 
            PasswordField(EditorGUIUtility.TempContent(label), password, style, options);

        /// <summary>
        /// <para>Make a text field where the user can enter a password.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the password field.</param>
        /// <param name="password">The password to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The password entered by the user.</para>
        /// </returns>
        public static string PasswordField(GUIContent label, string password, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.PasswordField(position, label, password, style);
        }

        /// <summary>
        /// <para>Make a generic popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selectedIndex">The index of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the options shown in the popup.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The index of the option that has been selected by the user.</para>
        /// </returns>
        public static int Popup(int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options) => 
            Popup(selectedIndex, displayedOptions, EditorStyles.popup, options);

        /// <summary>
        /// <para>Make a generic popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selectedIndex">The index of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the options shown in the popup.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The index of the option that has been selected by the user.</para>
        /// </returns>
        public static int Popup(int selectedIndex, GUIContent[] displayedOptions, params GUILayoutOption[] options) => 
            Popup(selectedIndex, displayedOptions, EditorStyles.popup, options);

        internal static void Popup(SerializedProperty property, GUIContent[] displayedOptions, params GUILayoutOption[] options)
        {
            Popup(property, displayedOptions, null, options);
        }

        /// <summary>
        /// <para>Make a generic popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selectedIndex">The index of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the options shown in the popup.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The index of the option that has been selected by the user.</para>
        /// </returns>
        public static int Popup(int selectedIndex, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.Popup(position, selectedIndex, displayedOptions, style);
        }

        /// <summary>
        /// <para>Make a generic popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selectedIndex">The index of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the options shown in the popup.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The index of the option that has been selected by the user.</para>
        /// </returns>
        public static int Popup(int selectedIndex, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.Popup(position, selectedIndex, displayedOptions, style);
        }

        /// <summary>
        /// <para>Make a generic popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selectedIndex">The index of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the options shown in the popup.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The index of the option that has been selected by the user.</para>
        /// </returns>
        public static int Popup(string label, int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options) => 
            Popup(label, selectedIndex, displayedOptions, EditorStyles.popup, options);

        internal static void Popup(SerializedProperty property, GUIContent[] displayedOptions, GUIContent label, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.popup, options);
            EditorGUI.Popup(position, property, displayedOptions, label);
        }

        /// <summary>
        /// <para>Make a generic popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selectedIndex">The index of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the options shown in the popup.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The index of the option that has been selected by the user.</para>
        /// </returns>
        public static int Popup(GUIContent label, int selectedIndex, GUIContent[] displayedOptions, params GUILayoutOption[] options) => 
            Popup(label, selectedIndex, displayedOptions, EditorStyles.popup, options);

        /// <summary>
        /// <para>Make a generic popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selectedIndex">The index of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the options shown in the popup.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The index of the option that has been selected by the user.</para>
        /// </returns>
        public static int Popup(string label, int selectedIndex, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.Popup(position, label, selectedIndex, displayedOptions, style);
        }

        /// <summary>
        /// <para>Make a generic popup selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="selectedIndex">The index of the option the field shows.</param>
        /// <param name="displayedOptions">An array with the options shown in the popup.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The index of the option that has been selected by the user.</para>
        /// </returns>
        public static int Popup(GUIContent label, int selectedIndex, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.Popup(position, label, selectedIndex, displayedOptions, style);
        }

        internal static float PowerSlider(string label, float value, float leftValue, float rightValue, float power, params GUILayoutOption[] options) => 
            PowerSlider(EditorGUIUtility.TempContent(label), value, leftValue, rightValue, power, options);

        internal static float PowerSlider(GUIContent label, float value, float leftValue, float rightValue, float power, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetSliderRect(true, options);
            return EditorGUI.PowerSlider(position, label, value, leftValue, rightValue, power);
        }

        /// <summary>
        /// <para>Make a label in front of some control.</para>
        /// </summary>
        /// <param name="label">Label to show to the left of the control.</param>
        /// <param name="followingStyle"></param>
        /// <param name="labelStyle"></param>
        [ExcludeFromDocs]
        public static void PrefixLabel(string label)
        {
            GUIStyle followingStyle = "Button";
            PrefixLabel(label, followingStyle);
        }

        /// <summary>
        /// <para>Make a label in front of some control.</para>
        /// </summary>
        /// <param name="label">Label to show to the left of the control.</param>
        /// <param name="followingStyle"></param>
        /// <param name="labelStyle"></param>
        [ExcludeFromDocs]
        public static void PrefixLabel(GUIContent label)
        {
            GUIStyle followingStyle = "Button";
            PrefixLabel(label, followingStyle);
        }

        /// <summary>
        /// <para>Make a label in front of some control.</para>
        /// </summary>
        /// <param name="label">Label to show to the left of the control.</param>
        /// <param name="followingStyle"></param>
        /// <param name="labelStyle"></param>
        public static void PrefixLabel(string label, [DefaultValue("\"Button\"")] GUIStyle followingStyle)
        {
            PrefixLabel(EditorGUIUtility.TempContent(label), followingStyle, EditorStyles.label);
        }

        /// <summary>
        /// <para>Make a label in front of some control.</para>
        /// </summary>
        /// <param name="label">Label to show to the left of the control.</param>
        /// <param name="followingStyle"></param>
        /// <param name="labelStyle"></param>
        public static void PrefixLabel(GUIContent label, [DefaultValue("\"Button\"")] GUIStyle followingStyle)
        {
            PrefixLabel(label, followingStyle, EditorStyles.label);
        }

        /// <summary>
        /// <para>Make a label in front of some control.</para>
        /// </summary>
        /// <param name="label">Label to show to the left of the control.</param>
        /// <param name="followingStyle"></param>
        /// <param name="labelStyle"></param>
        public static void PrefixLabel(string label, GUIStyle followingStyle, GUIStyle labelStyle)
        {
            PrefixLabel(EditorGUIUtility.TempContent(label), followingStyle, labelStyle);
        }

        /// <summary>
        /// <para>Make a label in front of some control.</para>
        /// </summary>
        /// <param name="label">Label to show to the left of the control.</param>
        /// <param name="followingStyle"></param>
        /// <param name="labelStyle"></param>
        public static void PrefixLabel(GUIContent label, GUIStyle followingStyle, GUIStyle labelStyle)
        {
            PrefixLabelInternal(label, followingStyle, labelStyle);
        }

        internal static void PrefixLabelInternal(GUIContent label, GUIStyle followingStyle, GUIStyle labelStyle)
        {
            float left = followingStyle.margin.left;
            if (!EditorGUI.LabelHasContent(label))
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                GUILayoutUtility.GetRect(EditorGUI.indent - left, 16f, followingStyle, options);
            }
            else
            {
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                Rect totalPosition = GUILayoutUtility.GetRect(EditorGUIUtility.labelWidth - left, 16f, followingStyle, optionArray2);
                totalPosition.xMin += EditorGUI.indent;
                EditorGUI.HandlePrefixLabel(totalPosition, totalPosition, label, 0, labelStyle);
            }
        }

        internal static void PropertiesField(GUIContent label, SerializedProperty[] properties, GUIContent[] propertyLabels, float propertyLabelsWidth, params GUILayoutOption[] options)
        {
            bool hasLabel = EditorGUI.LabelHasContent(label);
            float height = (16f * properties.Length) + (0 * (properties.Length - 1));
            Rect position = s_LastRect = GetControlRect(hasLabel, height, EditorStyles.numberField, options);
            EditorGUI.PropertiesField(position, label, properties, propertyLabels, propertyLabelsWidth);
        }

        /// <summary>
        /// <para>Make a field for SerializedProperty.</para>
        /// </summary>
        /// <param name="property">The SerializedProperty to make a field for.</param>
        /// <param name="label">Optional label to use. If not specified the label of the property itself is used. Use GUIContent.none to not display a label at all.</param>
        /// <param name="includeChildren">If true the property including children is drawn; otherwise only the control itself (such as only a foldout but nothing below it).</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>True if the property has children and is expanded and includeChildren was set to false; otherwise false.</para>
        /// </returns>
        public static bool PropertyField(SerializedProperty property, params GUILayoutOption[] options) => 
            PropertyField(property, null, false, options);

        /// <summary>
        /// <para>Make a field for SerializedProperty.</para>
        /// </summary>
        /// <param name="property">The SerializedProperty to make a field for.</param>
        /// <param name="label">Optional label to use. If not specified the label of the property itself is used. Use GUIContent.none to not display a label at all.</param>
        /// <param name="includeChildren">If true the property including children is drawn; otherwise only the control itself (such as only a foldout but nothing below it).</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>True if the property has children and is expanded and includeChildren was set to false; otherwise false.</para>
        /// </returns>
        public static bool PropertyField(SerializedProperty property, bool includeChildren, params GUILayoutOption[] options) => 
            PropertyField(property, null, includeChildren, options);

        /// <summary>
        /// <para>Make a field for SerializedProperty.</para>
        /// </summary>
        /// <param name="property">The SerializedProperty to make a field for.</param>
        /// <param name="label">Optional label to use. If not specified the label of the property itself is used. Use GUIContent.none to not display a label at all.</param>
        /// <param name="includeChildren">If true the property including children is drawn; otherwise only the control itself (such as only a foldout but nothing below it).</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>True if the property has children and is expanded and includeChildren was set to false; otherwise false.</para>
        /// </returns>
        public static bool PropertyField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options) => 
            PropertyField(property, label, false, options);

        /// <summary>
        /// <para>Make a field for SerializedProperty.</para>
        /// </summary>
        /// <param name="property">The SerializedProperty to make a field for.</param>
        /// <param name="label">Optional label to use. If not specified the label of the property itself is used. Use GUIContent.none to not display a label at all.</param>
        /// <param name="includeChildren">If true the property including children is drawn; otherwise only the control itself (such as only a foldout but nothing below it).</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>True if the property has children and is expanded and includeChildren was set to false; otherwise false.</para>
        /// </returns>
        public static bool PropertyField(SerializedProperty property, GUIContent label, bool includeChildren, params GUILayoutOption[] options) => 
            ScriptAttributeUtility.GetHandler(property).OnGUILayout(property, label, includeChildren, options);

        /// <summary>
        /// <para>Make an X, Y, W &amp; H field for entering a Rect.</para>
        /// </summary>
        /// <param name="label">Label to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static Rect RectField(Rect value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, EditorGUI.GetPropertyHeight(SerializedPropertyType.Rect, GUIContent.none), EditorStyles.numberField, options);
            return EditorGUI.RectField(position, value);
        }

        /// <summary>
        /// <para>Make an X, Y, W &amp; H field for entering a Rect.</para>
        /// </summary>
        /// <param name="label">Label to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static Rect RectField(string label, Rect value, params GUILayoutOption[] options) => 
            RectField(EditorGUIUtility.TempContent(label), value, options);

        /// <summary>
        /// <para>Make an X, Y, W &amp; H field for entering a Rect.</para>
        /// </summary>
        /// <param name="label">Label to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static Rect RectField(GUIContent label, Rect value, params GUILayoutOption[] options)
        {
            bool hasLabel = EditorGUI.LabelHasContent(label);
            float propertyHeight = EditorGUI.GetPropertyHeight(SerializedPropertyType.Rect, label);
            Rect position = s_LastRect = GetControlRect(hasLabel, propertyHeight, EditorStyles.numberField, options);
            return EditorGUI.RectField(position, label, value);
        }

        /// <summary>
        /// <para>Make a selectable label field. (Useful for showing read-only info that can be copy-pasted.)</para>
        /// </summary>
        /// <param name="text">The text to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void SelectableLabel(string text, params GUILayoutOption[] options)
        {
            SelectableLabel(text, EditorStyles.label, options);
        }

        /// <summary>
        /// <para>Make a selectable label field. (Useful for showing read-only info that can be copy-pasted.)</para>
        /// </summary>
        /// <param name="text">The text to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void SelectableLabel(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 32f, style, options);
            EditorGUI.SelectableLabel(position, text, style);
        }

        public static void Separator()
        {
            Space();
        }

        /// <summary>
        /// <para>Make a slider the user can drag to change a value between a min and a max.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the slider.</param>
        /// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value that has been set by the user.</para>
        /// </returns>
        public static float Slider(float value, float leftValue, float rightValue, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetSliderRect(false, options);
            return EditorGUI.Slider(position, value, leftValue, rightValue);
        }

        /// <summary>
        /// <para>Make a slider the user can drag to change a value between a min and a max.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the slider.</param>
        /// <param name="property">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void Slider(SerializedProperty property, float leftValue, float rightValue, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetSliderRect(false, options);
            EditorGUI.Slider(position, property, leftValue, rightValue);
        }

        /// <summary>
        /// <para>Make a slider the user can drag to change a value between a min and a max.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the slider.</param>
        /// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value that has been set by the user.</para>
        /// </returns>
        public static float Slider(string label, float value, float leftValue, float rightValue, params GUILayoutOption[] options) => 
            Slider(EditorGUIUtility.TempContent(label), value, leftValue, rightValue, options);

        /// <summary>
        /// <para>Make a slider the user can drag to change a value between a min and a max.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the slider.</param>
        /// <param name="property">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void Slider(SerializedProperty property, float leftValue, float rightValue, string label, params GUILayoutOption[] options)
        {
            Slider(property, leftValue, rightValue, EditorGUIUtility.TempContent(label), options);
        }

        /// <summary>
        /// <para>Make a slider the user can drag to change a value between a min and a max.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the slider.</param>
        /// <param name="property">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static void Slider(SerializedProperty property, float leftValue, float rightValue, GUIContent label, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetSliderRect(true, options);
            EditorGUI.Slider(position, property, leftValue, rightValue, label);
        }

        /// <summary>
        /// <para>Make a slider the user can drag to change a value between a min and a max.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the slider.</param>
        /// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value that has been set by the user.</para>
        /// </returns>
        public static float Slider(GUIContent label, float value, float leftValue, float rightValue, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetSliderRect(true, options);
            return EditorGUI.Slider(position, label, value, leftValue, rightValue);
        }

        internal static void SortingLayerField(GUIContent label, SerializedProperty layerID, GUIStyle style)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, new GUILayoutOption[0]);
            EditorGUI.SortingLayerField(position, label, layerID, style, EditorStyles.label);
        }

        internal static void SortingLayerField(GUIContent label, SerializedProperty layerID, GUIStyle style, GUIStyle labelStyle)
        {
            s_LastRect = GetControlRect(false, 16f, style, new GUILayoutOption[0]);
            EditorGUI.SortingLayerField(s_LastRect, label, layerID, style, labelStyle);
        }

        /// <summary>
        /// <para>Make a small space between the previous control and the following.</para>
        /// </summary>
        public static void Space()
        {
            GUILayoutUtility.GetRect((float) 6f, (float) 6f);
        }

        /// <summary>
        /// <para>Make a tag selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="tag">The tag the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The tag selected by the user.</para>
        /// </returns>
        public static string TagField(string tag, params GUILayoutOption[] options) => 
            TagField(tag, EditorStyles.popup, options);

        /// <summary>
        /// <para>Make a tag selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="tag">The tag the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The tag selected by the user.</para>
        /// </returns>
        public static string TagField(string label, string tag, params GUILayoutOption[] options) => 
            TagField(EditorGUIUtility.TempContent(label), tag, EditorStyles.popup, options);

        /// <summary>
        /// <para>Make a tag selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="tag">The tag the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The tag selected by the user.</para>
        /// </returns>
        public static string TagField(string tag, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.TagField(position, tag, style);
        }

        /// <summary>
        /// <para>Make a tag selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="tag">The tag the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The tag selected by the user.</para>
        /// </returns>
        public static string TagField(GUIContent label, string tag, params GUILayoutOption[] options) => 
            TagField(label, tag, EditorStyles.popup, options);

        /// <summary>
        /// <para>Make a tag selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="tag">The tag the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The tag selected by the user.</para>
        /// </returns>
        public static string TagField(string label, string tag, GUIStyle style, params GUILayoutOption[] options) => 
            TagField(EditorGUIUtility.TempContent(label), tag, style, options);

        /// <summary>
        /// <para>Make a tag selection field.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the field.</param>
        /// <param name="tag">The tag the field shows.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The tag selected by the user.</para>
        /// </returns>
        public static string TagField(GUIContent label, string tag, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.TagField(position, label, tag, style);
        }

        internal static void TargetChoiceField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new TargetChoiceHandler.TargetChoiceMenuFunction(TargetChoiceHandler.SetToValueOfTarget);
            }
            TargetChoiceField(property, label, <>f__mg$cache0, options);
        }

        internal static void TargetChoiceField(SerializedProperty property, GUIContent label, TargetChoiceHandler.TargetChoiceMenuFunction func, params GUILayoutOption[] options)
        {
            EditorGUI.TargetChoiceField(GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, kLabelFloatMaxW, 16f, 16f, EditorStyles.popup, options), property, label, func);
        }

        /// <summary>
        /// <para>Make a text area.</para>
        /// </summary>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The text entered by the user.</para>
        /// </returns>
        public static string TextArea(string text, params GUILayoutOption[] options) => 
            TextArea(text, EditorStyles.textField, options);

        /// <summary>
        /// <para>Make a text area.</para>
        /// </summary>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The text entered by the user.</para>
        /// </returns>
        public static string TextArea(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GUILayoutUtility.GetRect(EditorGUIUtility.TempContent(text), style, options);
            return EditorGUI.TextArea(position, text, style);
        }

        /// <summary>
        /// <para>Make a text field.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the text field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The text entered by the user.</para>
        /// </returns>
        public static string TextField(string text, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, EditorStyles.textField, options);
            return EditorGUI.TextField(position, text);
        }

        /// <summary>
        /// <para>Make a text field.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the text field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The text entered by the user.</para>
        /// </returns>
        public static string TextField(string label, string text, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.textField, options);
            return EditorGUI.TextField(position, label, text);
        }

        /// <summary>
        /// <para>Make a text field.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the text field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The text entered by the user.</para>
        /// </returns>
        public static string TextField(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, style, options);
            return EditorGUI.TextField(position, text, style);
        }

        /// <summary>
        /// <para>Make a text field.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the text field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The text entered by the user.</para>
        /// </returns>
        public static string TextField(GUIContent label, string text, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, EditorStyles.textField, options);
            return EditorGUI.TextField(position, label, text);
        }

        /// <summary>
        /// <para>Make a text field.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the text field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The text entered by the user.</para>
        /// </returns>
        public static string TextField(string label, string text, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.TextField(position, label, text, style);
        }

        /// <summary>
        /// <para>Make a text field.</para>
        /// </summary>
        /// <param name="label">Optional label to display in front of the text field.</param>
        /// <param name="text">The text to edit.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The text entered by the user.</para>
        /// </returns>
        public static string TextField(GUIContent label, string text, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, 16f, style, options);
            return EditorGUI.TextField(position, label, text, style);
        }

        internal static string TextFieldDropDown(string text, string[] dropDownElement) => 
            TextFieldDropDown(GUIContent.none, text, dropDownElement);

        internal static string TextFieldDropDown(GUIContent label, string text, string[] dropDownElement) => 
            EditorGUI.TextFieldDropDown(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.textField), label, text, dropDownElement);

        /// <summary>
        /// <para>Make a toggle.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the toggle.</param>
        /// <param name="value">The shown state of the toggle.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        /// properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// 
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The selected state of the toggle.</para>
        /// </returns>
        public static bool Toggle(bool value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetToggleRect(false, options);
            return EditorGUI.Toggle(position, value);
        }

        /// <summary>
        /// <para>Make a toggle.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the toggle.</param>
        /// <param name="value">The shown state of the toggle.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        /// properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// 
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The selected state of the toggle.</para>
        /// </returns>
        public static bool Toggle(bool value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetToggleRect(false, options);
            return EditorGUI.Toggle(position, value, style);
        }

        /// <summary>
        /// <para>Make a toggle.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the toggle.</param>
        /// <param name="value">The shown state of the toggle.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        /// properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// 
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The selected state of the toggle.</para>
        /// </returns>
        public static bool Toggle(string label, bool value, params GUILayoutOption[] options) => 
            Toggle(EditorGUIUtility.TempContent(label), value, options);

        /// <summary>
        /// <para>Make a toggle.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the toggle.</param>
        /// <param name="value">The shown state of the toggle.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        /// properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// 
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The selected state of the toggle.</para>
        /// </returns>
        public static bool Toggle(GUIContent label, bool value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetToggleRect(true, options);
            return EditorGUI.Toggle(position, label, value);
        }

        /// <summary>
        /// <para>Make a toggle.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the toggle.</param>
        /// <param name="value">The shown state of the toggle.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        /// properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// 
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The selected state of the toggle.</para>
        /// </returns>
        public static bool Toggle(string label, bool value, GUIStyle style, params GUILayoutOption[] options) => 
            Toggle(EditorGUIUtility.TempContent(label), value, style, options);

        /// <summary>
        /// <para>Make a toggle.</para>
        /// </summary>
        /// <param name="label">Optional label in front of the toggle.</param>
        /// <param name="value">The shown state of the toggle.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        /// properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// 
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The selected state of the toggle.</para>
        /// </returns>
        public static bool Toggle(GUIContent label, bool value, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetToggleRect(true, options);
            return EditorGUI.Toggle(position, label, value, style);
        }

        /// <summary>
        /// <para>Make a toggle field where the toggle is to the left and the label immediately to the right of it.</para>
        /// </summary>
        /// <param name="label">Label to display next to the toggle.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="labelStyle">Optional GUIStyle to use for the label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static bool ToggleLeft(string label, bool value, params GUILayoutOption[] options) => 
            ToggleLeft(EditorGUIUtility.TempContent(label), value, options);

        /// <summary>
        /// <para>Make a toggle field where the toggle is to the left and the label immediately to the right of it.</para>
        /// </summary>
        /// <param name="label">Label to display next to the toggle.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="labelStyle">Optional GUIStyle to use for the label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static bool ToggleLeft(GUIContent label, bool value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, options);
            return EditorGUI.ToggleLeft(position, label, value);
        }

        /// <summary>
        /// <para>Make a toggle field where the toggle is to the left and the label immediately to the right of it.</para>
        /// </summary>
        /// <param name="label">Label to display next to the toggle.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="labelStyle">Optional GUIStyle to use for the label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static bool ToggleLeft(string label, bool value, GUIStyle labelStyle, params GUILayoutOption[] options) => 
            ToggleLeft(EditorGUIUtility.TempContent(label), value, labelStyle, options);

        /// <summary>
        /// <para>Make a toggle field where the toggle is to the left and the label immediately to the right of it.</para>
        /// </summary>
        /// <param name="label">Label to display next to the toggle.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="labelStyle">Optional GUIStyle to use for the label.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        public static bool ToggleLeft(GUIContent label, bool value, GUIStyle labelStyle, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, options);
            return EditorGUI.ToggleLeft(position, label, value, labelStyle);
        }

        internal static bool ToggleTitlebar(bool foldout, GUIContent label, ref bool toggleValue) => 
            EditorGUI.ToggleTitlebar(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.inspectorTitlebar), label, foldout, ref toggleValue);

        internal static bool ToggleTitlebar(bool foldout, GUIContent label, SerializedProperty property)
        {
            bool boolValue = property.boolValue;
            EditorGUI.BeginChangeCheck();
            foldout = EditorGUI.ToggleTitlebar(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.inspectorTitlebar), label, foldout, ref boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.boolValue = boolValue;
            }
            return foldout;
        }

        internal static string ToolbarSearchField(string text, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GUILayoutUtility.GetRect(0f, kLabelFloatMaxW * 1.5f, 16f, 16f, EditorStyles.toolbarSearchField, options);
            int searchMode = 0;
            return EditorGUI.ToolbarSearchField(position, null, ref searchMode, text);
        }

        internal static string ToolbarSearchField(string text, string[] searchModes, ref int searchMode, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GUILayoutUtility.GetRect(0f, kLabelFloatMaxW * 1.5f, 16f, 16f, EditorStyles.toolbarSearchField, options);
            return EditorGUI.ToolbarSearchField(position, searchModes, ref searchMode, text);
        }

        /// <summary>
        /// <para>Make an X &amp; Y field for entering a Vector2.</para>
        /// </summary>
        /// <param name="label">Label to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// </param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static Vector2 Vector2Field(string label, Vector2 value, params GUILayoutOption[] options) => 
            Vector2Field(EditorGUIUtility.TempContent(label), value, options);

        /// <summary>
        /// <para>Make an X &amp; Y field for entering a Vector2.</para>
        /// </summary>
        /// <param name="label">Label to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// </param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static Vector2 Vector2Field(GUIContent label, Vector2 value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2, label), EditorStyles.numberField, options);
            return EditorGUI.Vector2Field(position, label, value);
        }

        /// <summary>
        /// <para>Make an X, Y &amp; Z field for entering a Vector3.</para>
        /// </summary>
        /// <param name="label">Label to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        /// properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static Vector3 Vector3Field(string label, Vector3 value, params GUILayoutOption[] options) => 
            Vector3Field(EditorGUIUtility.TempContent(label), value, options);

        /// <summary>
        /// <para>Make an X, Y &amp; Z field for entering a Vector3.</para>
        /// </summary>
        /// <param name="label">Label to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting
        /// properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static Vector3 Vector3Field(GUIContent label, Vector3 value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label), EditorStyles.numberField, options);
            return EditorGUI.Vector3Field(position, label, value);
        }

        /// <summary>
        /// <para>Make an X, Y, Z &amp; W field for entering a Vector4.</para>
        /// </summary>
        /// <param name="label">Label to display above the field.</param>
        /// <param name="value">The value to edit.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
        /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
        /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>
        /// <para>The value entered by the user.</para>
        /// </returns>
        public static Vector4 Vector4Field(string label, Vector4 value, params GUILayoutOption[] options) => 
            Vector4Field(EditorGUIUtility.TempContent(label), value, options);

        public static Vector4 Vector4Field(GUIContent label, Vector4 value, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(true, EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector4, label), EditorStyles.numberField, options);
            return EditorGUI.Vector4Field(position, label, value);
        }

        internal static void VUMeterHorizontal(float value, float peak, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, EditorStyles.numberField, options);
            EditorGUI.VUMeter.HorizontalMeter(position, value, peak, EditorGUI.VUMeter.horizontalVUTexture, Color.grey);
        }

        internal static void VUMeterHorizontal(float value, ref EditorGUI.VUMeter.SmoothingData data, params GUILayoutOption[] options)
        {
            Rect position = s_LastRect = GetControlRect(false, 16f, EditorStyles.numberField, options);
            EditorGUI.VUMeter.HorizontalMeter(position, value, ref data, EditorGUI.VUMeter.horizontalVUTexture, Color.grey);
        }

        internal static float kLabelFloatMaxW =>
            ((EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth) + 5f);

        internal static float kLabelFloatMinW =>
            ((EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth) + 5f);

        /// <summary>
        /// <para>Begins a group that can be be hidden/shown and the transition will be animated.</para>
        /// </summary>
        public class FadeGroupScope : GUI.Scope
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private bool <visible>k__BackingField;

            /// <summary>
            /// <para>Create a new FadeGroupScope and begin the corresponding group.</para>
            /// </summary>
            /// <param name="value">A value between 0 and 1, 0 being hidden, and 1 being fully visible.</param>
            public FadeGroupScope(float value)
            {
                this.visible = EditorGUILayout.BeginFadeGroup(value);
            }

            protected override void CloseScope()
            {
                EditorGUILayout.EndFadeGroup();
            }

            /// <summary>
            /// <para>Whether the group is visible.</para>
            /// </summary>
            public bool visible { get; protected set; }
        }

        /// <summary>
        /// <para>Disposable helper class for managing BeginHorizontal / EndHorizontal.</para>
        /// </summary>
        public class HorizontalScope : GUI.Scope
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private Rect <rect>k__BackingField;

            /// <summary>
            /// <para>Create a new HorizontalScope and begin the corresponding horizontal group.</para>
            /// </summary>
            /// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
            /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
            /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
            /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
            public HorizontalScope(params GUILayoutOption[] options)
            {
                this.rect = EditorGUILayout.BeginHorizontal(options);
            }

            /// <summary>
            /// <para>Create a new HorizontalScope and begin the corresponding horizontal group.</para>
            /// </summary>
            /// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
            /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
            /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
            /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
            public HorizontalScope(GUIStyle style, params GUILayoutOption[] options)
            {
                this.rect = EditorGUILayout.BeginHorizontal(style, options);
            }

            internal HorizontalScope(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
            {
                this.rect = EditorGUILayout.BeginHorizontal(content, style, options);
            }

            protected override void CloseScope()
            {
                EditorGUILayout.EndHorizontal();
            }

            /// <summary>
            /// <para>The rect of the horizontal group.</para>
            /// </summary>
            public Rect rect { get; protected set; }
        }

        internal class HorizontalScrollViewScope : GUI.Scope
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private bool <handleScrollWheel>k__BackingField;
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private Vector2 <scrollPosition>k__BackingField;

            public HorizontalScrollViewScope(Vector2 scrollPosition, params GUILayoutOption[] options)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = EditorGUILayout.BeginHorizontalScrollView(scrollPosition, options);
            }

            public HorizontalScrollViewScope(Vector2 scrollPosition, bool alwaysShowHorizontal, GUIStyle horizontalScrollbar, GUIStyle background, params GUILayoutOption[] options)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = EditorGUILayout.BeginHorizontalScrollView(scrollPosition, alwaysShowHorizontal, horizontalScrollbar, background, options);
            }

            protected override void CloseScope()
            {
                EditorGUILayout.EndScrollView(this.handleScrollWheel);
            }

            public bool handleScrollWheel { get; set; }

            public Vector2 scrollPosition { get; protected set; }
        }

        /// <summary>
        /// <para>Disposable helper class for managing BeginScrollView / EndScrollView.</para>
        /// </summary>
        public class ScrollViewScope : GUI.Scope
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private bool <handleScrollWheel>k__BackingField;
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private Vector2 <scrollPosition>k__BackingField;

            /// <summary>
            /// <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
            /// </summary>
            /// <param name="scrollPosition">The scroll position to use.</param>
            /// <param name="alwaysShowHorizontal">Whether to always show the horizontal scrollbar. If false, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
            /// <param name="alwaysShowVertical">Whether to always show the vertical scrollbar. If false, it is only shown when the content inside the ScrollView is higher than the scrollview itself.</param>
            /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
            /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
            /// <param name="options"></param>
            /// <param name="style"></param>
            /// <param name="background"></param>
            public ScrollViewScope(Vector2 scrollPosition, params GUILayoutOption[] options)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, options);
            }

            /// <summary>
            /// <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
            /// </summary>
            /// <param name="scrollPosition">The scroll position to use.</param>
            /// <param name="alwaysShowHorizontal">Whether to always show the horizontal scrollbar. If false, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
            /// <param name="alwaysShowVertical">Whether to always show the vertical scrollbar. If false, it is only shown when the content inside the ScrollView is higher than the scrollview itself.</param>
            /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
            /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
            /// <param name="options"></param>
            /// <param name="style"></param>
            /// <param name="background"></param>
            public ScrollViewScope(Vector2 scrollPosition, GUIStyle style, params GUILayoutOption[] options)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, style, options);
            }

            /// <summary>
            /// <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
            /// </summary>
            /// <param name="scrollPosition">The scroll position to use.</param>
            /// <param name="alwaysShowHorizontal">Whether to always show the horizontal scrollbar. If false, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
            /// <param name="alwaysShowVertical">Whether to always show the vertical scrollbar. If false, it is only shown when the content inside the ScrollView is higher than the scrollview itself.</param>
            /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
            /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
            /// <param name="options"></param>
            /// <param name="style"></param>
            /// <param name="background"></param>
            public ScrollViewScope(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, options);
            }

            /// <summary>
            /// <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
            /// </summary>
            /// <param name="scrollPosition">The scroll position to use.</param>
            /// <param name="alwaysShowHorizontal">Whether to always show the horizontal scrollbar. If false, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
            /// <param name="alwaysShowVertical">Whether to always show the vertical scrollbar. If false, it is only shown when the content inside the ScrollView is higher than the scrollview itself.</param>
            /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
            /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
            /// <param name="options"></param>
            /// <param name="style"></param>
            /// <param name="background"></param>
            public ScrollViewScope(Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, horizontalScrollbar, verticalScrollbar, options);
            }

            internal ScrollViewScope(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, options);
            }

            /// <summary>
            /// <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
            /// </summary>
            /// <param name="scrollPosition">The scroll position to use.</param>
            /// <param name="alwaysShowHorizontal">Whether to always show the horizontal scrollbar. If false, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
            /// <param name="alwaysShowVertical">Whether to always show the vertical scrollbar. If false, it is only shown when the content inside the ScrollView is higher than the scrollview itself.</param>
            /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
            /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
            /// <param name="options"></param>
            /// <param name="style"></param>
            /// <param name="background"></param>
            public ScrollViewScope(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background, options);
            }

            protected override void CloseScope()
            {
                EditorGUILayout.EndScrollView(this.handleScrollWheel);
            }

            /// <summary>
            /// <para>Whether this ScrollView should handle scroll wheel events. (default: true).</para>
            /// </summary>
            public bool handleScrollWheel { get; set; }

            /// <summary>
            /// <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
            /// </summary>
            public Vector2 scrollPosition { get; protected set; }
        }

        /// <summary>
        /// <para>Begin a vertical group with a toggle to enable or disable all the controls within at once.</para>
        /// </summary>
        public class ToggleGroupScope : GUI.Scope
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private bool <enabled>k__BackingField;

            /// <summary>
            /// <para></para>
            /// </summary>
            /// <param name="label">Label to show above the toggled controls.</param>
            /// <param name="toggle">Enabled state of the toggle group.</param>
            public ToggleGroupScope(string label, bool toggle)
            {
                this.enabled = EditorGUILayout.BeginToggleGroup(label, toggle);
            }

            /// <summary>
            /// <para></para>
            /// </summary>
            /// <param name="label">Label to show above the toggled controls.</param>
            /// <param name="toggle">Enabled state of the toggle group.</param>
            public ToggleGroupScope(GUIContent label, bool toggle)
            {
                this.enabled = EditorGUILayout.BeginToggleGroup(label, toggle);
            }

            protected override void CloseScope()
            {
                EditorGUILayout.EndToggleGroup();
            }

            /// <summary>
            /// <para>The enabled state selected by the user.</para>
            /// </summary>
            public bool enabled { get; protected set; }
        }

        /// <summary>
        /// <para>Disposable helper class for managing BeginVertical / EndVertical.</para>
        /// </summary>
        public class VerticalScope : GUI.Scope
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Rect <rect>k__BackingField;

            /// <summary>
            /// <para>Create a new VerticalScope and begin the corresponding vertical group.</para>
            /// </summary>
            /// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
            /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
            /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
            /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
            public VerticalScope(params GUILayoutOption[] options)
            {
                this.rect = EditorGUILayout.BeginVertical(options);
            }

            /// <summary>
            /// <para>Create a new VerticalScope and begin the corresponding vertical group.</para>
            /// </summary>
            /// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
            /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
            /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, 
            /// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
            public VerticalScope(GUIStyle style, params GUILayoutOption[] options)
            {
                this.rect = EditorGUILayout.BeginVertical(style, options);
            }

            internal VerticalScope(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
            {
                this.rect = EditorGUILayout.BeginVertical(content, style, options);
            }

            protected override void CloseScope()
            {
                EditorGUILayout.EndVertical();
            }

            /// <summary>
            /// <para>The rect of the vertical group.</para>
            /// </summary>
            public Rect rect { get; protected set; }
        }

        internal class VerticalScrollViewScope : GUI.Scope
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private bool <handleScrollWheel>k__BackingField;
            [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
            private Vector2 <scrollPosition>k__BackingField;

            public VerticalScrollViewScope(Vector2 scrollPosition, params GUILayoutOption[] options)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = EditorGUILayout.BeginVerticalScrollView(scrollPosition, options);
            }

            public VerticalScrollViewScope(Vector2 scrollPosition, bool alwaysShowVertical, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = EditorGUILayout.BeginVerticalScrollView(scrollPosition, alwaysShowVertical, verticalScrollbar, background, options);
            }

            protected override void CloseScope()
            {
                EditorGUILayout.EndScrollView(this.handleScrollWheel);
            }

            public bool handleScrollWheel { get; set; }

            public Vector2 scrollPosition { get; protected set; }
        }
    }
}

