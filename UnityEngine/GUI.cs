namespace UnityEngine
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;
    using UnityEngineInternal;

    /// <summary>
    /// <para>The GUI class is the interface for Unity's GUI with manual positioning.</para>
    /// </summary>
    public class GUI
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static DateTime <nextScrollStepTime>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static int <scrollTroughSide>k__BackingField;
        private static readonly int s_BeginGroupHash = "BeginGroup".GetHashCode();
        private static readonly int s_BoxHash = "Box".GetHashCode();
        private static readonly int s_ButtonGridHash = "ButtonGrid".GetHashCode();
        private static int s_HotTextField = -1;
        private static readonly int s_RepeatButtonHash = "repeatButton".GetHashCode();
        private static int s_ScrollControlId;
        private static float s_ScrollStepSize = 10f;
        private static readonly int s_ScrollviewHash = "scrollView".GetHashCode();
        private static GenericStack s_ScrollViewStates = new GenericStack();
        private static GUISkin s_Skin;
        private static readonly int s_SliderHash = "Slider".GetHashCode();
        private static readonly int s_ToggleHash = "Toggle".GetHashCode();
        internal static Rect s_ToolTipRect;

        static GUI()
        {
            nextScrollStepTime = DateTime.Now;
        }

        public static void BeginClip(Rect position)
        {
            GUIUtility.CheckOnGUI();
            GUIClip.Push(position, Vector2.zero, Vector2.zero, false);
        }

        public static void BeginClip(Rect position, Vector2 scrollOffset, Vector2 renderOffset, bool resetOffset)
        {
            GUIUtility.CheckOnGUI();
            GUIClip.Push(position, scrollOffset, renderOffset, resetOffset);
        }

        /// <summary>
        /// <para>Begin a group. Must be matched with a call to EndGroup.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the group.</param>
        /// <param name="text">Text to display on the group.</param>
        /// <param name="image">Texture to display on the group.</param>
        /// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
        /// <param name="style">The style to use for the background.</param>
        public static void BeginGroup(Rect position)
        {
            BeginGroup(position, GUIContent.none, GUIStyle.none);
        }

        /// <summary>
        /// <para>Begin a group. Must be matched with a call to EndGroup.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the group.</param>
        /// <param name="text">Text to display on the group.</param>
        /// <param name="image">Texture to display on the group.</param>
        /// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
        /// <param name="style">The style to use for the background.</param>
        public static void BeginGroup(Rect position, string text)
        {
            BeginGroup(position, GUIContent.Temp(text), GUIStyle.none);
        }

        /// <summary>
        /// <para>Begin a group. Must be matched with a call to EndGroup.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the group.</param>
        /// <param name="text">Text to display on the group.</param>
        /// <param name="image">Texture to display on the group.</param>
        /// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
        /// <param name="style">The style to use for the background.</param>
        public static void BeginGroup(Rect position, GUIContent content)
        {
            BeginGroup(position, content, GUIStyle.none);
        }

        /// <summary>
        /// <para>Begin a group. Must be matched with a call to EndGroup.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the group.</param>
        /// <param name="text">Text to display on the group.</param>
        /// <param name="image">Texture to display on the group.</param>
        /// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
        /// <param name="style">The style to use for the background.</param>
        public static void BeginGroup(Rect position, GUIStyle style)
        {
            BeginGroup(position, GUIContent.none, style);
        }

        /// <summary>
        /// <para>Begin a group. Must be matched with a call to EndGroup.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the group.</param>
        /// <param name="text">Text to display on the group.</param>
        /// <param name="image">Texture to display on the group.</param>
        /// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
        /// <param name="style">The style to use for the background.</param>
        public static void BeginGroup(Rect position, Texture image)
        {
            BeginGroup(position, GUIContent.Temp(image), GUIStyle.none);
        }

        /// <summary>
        /// <para>Begin a group. Must be matched with a call to EndGroup.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the group.</param>
        /// <param name="text">Text to display on the group.</param>
        /// <param name="image">Texture to display on the group.</param>
        /// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
        /// <param name="style">The style to use for the background.</param>
        public static void BeginGroup(Rect position, string text, GUIStyle style)
        {
            BeginGroup(position, GUIContent.Temp(text), style);
        }

        /// <summary>
        /// <para>Begin a group. Must be matched with a call to EndGroup.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the group.</param>
        /// <param name="text">Text to display on the group.</param>
        /// <param name="image">Texture to display on the group.</param>
        /// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
        /// <param name="style">The style to use for the background.</param>
        public static void BeginGroup(Rect position, GUIContent content, GUIStyle style)
        {
            GUIUtility.CheckOnGUI();
            int controlID = GUIUtility.GetControlID(s_BeginGroupHash, FocusType.Passive);
            if ((content != GUIContent.none) || (style != GUIStyle.none))
            {
                if (Event.current.type == EventType.Repaint)
                {
                    style.Draw(position, content, controlID);
                }
                else if (position.Contains(Event.current.mousePosition))
                {
                    GUIUtility.mouseUsed = true;
                }
            }
            GUIClip.Push(position, Vector2.zero, Vector2.zero, false);
        }

        /// <summary>
        /// <para>Begin a group. Must be matched with a call to EndGroup.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the group.</param>
        /// <param name="text">Text to display on the group.</param>
        /// <param name="image">Texture to display on the group.</param>
        /// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
        /// <param name="style">The style to use for the background.</param>
        public static void BeginGroup(Rect position, Texture image, GUIStyle style)
        {
            BeginGroup(position, GUIContent.Temp(image), style);
        }

        /// <summary>
        /// <para>Begin a scrolling view inside your GUI.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the ScrollView.</param>
        /// <param name="scrollPosition">The pixel distance that the view is scrolled in the X and Y directions.</param>
        /// <param name="viewRect">The rectangle used inside the scrollview.</param>
        /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
        /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
        /// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when viewRect is wider than position.</param>
        /// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when viewRect is taller than position.</param>
        /// <returns>
        /// <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
        /// </returns>
        public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect) => 
            BeginScrollView(position, scrollPosition, viewRect, false, false, skin.horizontalScrollbar, skin.verticalScrollbar, skin.scrollView);

        /// <summary>
        /// <para>Begin a scrolling view inside your GUI.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the ScrollView.</param>
        /// <param name="scrollPosition">The pixel distance that the view is scrolled in the X and Y directions.</param>
        /// <param name="viewRect">The rectangle used inside the scrollview.</param>
        /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
        /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
        /// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when viewRect is wider than position.</param>
        /// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when viewRect is taller than position.</param>
        /// <returns>
        /// <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
        /// </returns>
        public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical) => 
            BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, skin.horizontalScrollbar, skin.verticalScrollbar, skin.scrollView);

        /// <summary>
        /// <para>Begin a scrolling view inside your GUI.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the ScrollView.</param>
        /// <param name="scrollPosition">The pixel distance that the view is scrolled in the X and Y directions.</param>
        /// <param name="viewRect">The rectangle used inside the scrollview.</param>
        /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
        /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
        /// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when viewRect is wider than position.</param>
        /// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when viewRect is taller than position.</param>
        /// <returns>
        /// <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
        /// </returns>
        public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar) => 
            BeginScrollView(position, scrollPosition, viewRect, false, false, horizontalScrollbar, verticalScrollbar, skin.scrollView);

        /// <summary>
        /// <para>Begin a scrolling view inside your GUI.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the ScrollView.</param>
        /// <param name="scrollPosition">The pixel distance that the view is scrolled in the X and Y directions.</param>
        /// <param name="viewRect">The rectangle used inside the scrollview.</param>
        /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
        /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
        /// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when viewRect is wider than position.</param>
        /// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when viewRect is taller than position.</param>
        /// <returns>
        /// <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
        /// </returns>
        public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar) => 
            BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, skin.scrollView);

        internal static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background)
        {
            GUIUtility.CheckOnGUI();
            if ((Event.current.type == EventType.DragUpdated) && position.Contains(Event.current.mousePosition))
            {
                if (Mathf.Abs((float) (Event.current.mousePosition.y - position.y)) < 8f)
                {
                    scrollPosition.y -= 16f;
                    InternalRepaintEditorWindow();
                }
                else if (Mathf.Abs((float) (Event.current.mousePosition.y - position.yMax)) < 8f)
                {
                    scrollPosition.y += 16f;
                    InternalRepaintEditorWindow();
                }
            }
            int controlID = GUIUtility.GetControlID(s_ScrollviewHash, FocusType.Passive);
            ScrollViewState stateObject = (ScrollViewState) GUIUtility.GetStateObject(typeof(ScrollViewState), controlID);
            if (stateObject.apply)
            {
                scrollPosition = stateObject.scrollPosition;
                stateObject.apply = false;
            }
            stateObject.position = position;
            stateObject.scrollPosition = scrollPosition;
            stateObject.visibleRect = stateObject.viewRect = viewRect;
            stateObject.visibleRect.width = position.width;
            stateObject.visibleRect.height = position.height;
            s_ScrollViewStates.Push(stateObject);
            Rect screenRect = new Rect(position);
            switch (Event.current.type)
            {
                case EventType.Layout:
                    GUIUtility.GetControlID(s_SliderHash, FocusType.Passive);
                    GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
                    GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
                    GUIUtility.GetControlID(s_SliderHash, FocusType.Passive);
                    GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
                    GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
                    break;

                case EventType.Used:
                    break;

                default:
                {
                    bool flag = alwaysShowVertical;
                    bool flag2 = alwaysShowHorizontal;
                    if (flag2 || (viewRect.width > screenRect.width))
                    {
                        stateObject.visibleRect.height = (position.height - horizontalScrollbar.fixedHeight) + horizontalScrollbar.margin.top;
                        screenRect.height -= horizontalScrollbar.fixedHeight + horizontalScrollbar.margin.top;
                        flag2 = true;
                    }
                    if (flag || (viewRect.height > screenRect.height))
                    {
                        stateObject.visibleRect.width = (position.width - verticalScrollbar.fixedWidth) + verticalScrollbar.margin.left;
                        screenRect.width -= verticalScrollbar.fixedWidth + verticalScrollbar.margin.left;
                        flag = true;
                        if (!flag2 && (viewRect.width > screenRect.width))
                        {
                            stateObject.visibleRect.height = (position.height - horizontalScrollbar.fixedHeight) + horizontalScrollbar.margin.top;
                            screenRect.height -= horizontalScrollbar.fixedHeight + horizontalScrollbar.margin.top;
                            flag2 = true;
                        }
                    }
                    if ((Event.current.type == EventType.Repaint) && (background != GUIStyle.none))
                    {
                        background.Draw(position, position.Contains(Event.current.mousePosition), false, flag2 && flag, false);
                    }
                    if (flag2 && (horizontalScrollbar != GUIStyle.none))
                    {
                        scrollPosition.x = HorizontalScrollbar(new Rect(position.x, position.yMax - horizontalScrollbar.fixedHeight, screenRect.width, horizontalScrollbar.fixedHeight), scrollPosition.x, Mathf.Min(screenRect.width, viewRect.width), 0f, viewRect.width, horizontalScrollbar);
                    }
                    else
                    {
                        GUIUtility.GetControlID(s_SliderHash, FocusType.Passive);
                        GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
                        GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
                        if (horizontalScrollbar != GUIStyle.none)
                        {
                            scrollPosition.x = 0f;
                        }
                        else
                        {
                            scrollPosition.x = Mathf.Clamp(scrollPosition.x, 0f, Mathf.Max((float) (viewRect.width - position.width), (float) 0f));
                        }
                    }
                    if (flag && (verticalScrollbar != GUIStyle.none))
                    {
                        scrollPosition.y = VerticalScrollbar(new Rect(screenRect.xMax + verticalScrollbar.margin.left, screenRect.y, verticalScrollbar.fixedWidth, screenRect.height), scrollPosition.y, Mathf.Min(screenRect.height, viewRect.height), 0f, viewRect.height, verticalScrollbar);
                    }
                    else
                    {
                        GUIUtility.GetControlID(s_SliderHash, FocusType.Passive);
                        GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
                        GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
                        if (verticalScrollbar != GUIStyle.none)
                        {
                            scrollPosition.y = 0f;
                        }
                        else
                        {
                            scrollPosition.y = Mathf.Clamp(scrollPosition.y, 0f, Mathf.Max((float) (viewRect.height - position.height), (float) 0f));
                        }
                    }
                    break;
                }
            }
            GUIClip.Push(screenRect, new Vector2(Mathf.Round(-scrollPosition.x - viewRect.x), Mathf.Round(-scrollPosition.y - viewRect.y)), Vector2.zero, false);
            return scrollPosition;
        }

        internal static void BeginWindows(int skinMode, int editorWindowInstanceID)
        {
            GUILayoutGroup topLevel = GUILayoutUtility.current.topLevel;
            GenericStack layoutGroups = GUILayoutUtility.current.layoutGroups;
            GUILayoutGroup windows = GUILayoutUtility.current.windows;
            Matrix4x4 matrix = GUI.matrix;
            Internal_BeginWindows();
            GUI.matrix = matrix;
            GUILayoutUtility.current.topLevel = topLevel;
            GUILayoutUtility.current.layoutGroups = layoutGroups;
            GUILayoutUtility.current.windows = windows;
        }

        /// <summary>
        /// <para>Create a Box on the GUI Layer. A Box can contain text, an image, or a combination of these along with an optional tooltip, through using a GUIContent parameter. You may also use a GUIStyle to adjust the layout of items in a box, text colour and other properties.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the box.</param>
        /// <param name="text">Text to display on the box.</param>
        /// <param name="image">Texture to display on the box.</param>
        /// <param name="content">Text, image and tooltip for this box.</param>
        /// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
        public static void Box(Rect position, string text)
        {
            Box(position, GUIContent.Temp(text), s_Skin.box);
        }

        /// <summary>
        /// <para>Create a Box on the GUI Layer. A Box can contain text, an image, or a combination of these along with an optional tooltip, through using a GUIContent parameter. You may also use a GUIStyle to adjust the layout of items in a box, text colour and other properties.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the box.</param>
        /// <param name="text">Text to display on the box.</param>
        /// <param name="image">Texture to display on the box.</param>
        /// <param name="content">Text, image and tooltip for this box.</param>
        /// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
        public static void Box(Rect position, GUIContent content)
        {
            Box(position, content, s_Skin.box);
        }

        /// <summary>
        /// <para>Create a Box on the GUI Layer. A Box can contain text, an image, or a combination of these along with an optional tooltip, through using a GUIContent parameter. You may also use a GUIStyle to adjust the layout of items in a box, text colour and other properties.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the box.</param>
        /// <param name="text">Text to display on the box.</param>
        /// <param name="image">Texture to display on the box.</param>
        /// <param name="content">Text, image and tooltip for this box.</param>
        /// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
        public static void Box(Rect position, Texture image)
        {
            Box(position, GUIContent.Temp(image), s_Skin.box);
        }

        /// <summary>
        /// <para>Create a Box on the GUI Layer. A Box can contain text, an image, or a combination of these along with an optional tooltip, through using a GUIContent parameter. You may also use a GUIStyle to adjust the layout of items in a box, text colour and other properties.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the box.</param>
        /// <param name="text">Text to display on the box.</param>
        /// <param name="image">Texture to display on the box.</param>
        /// <param name="content">Text, image and tooltip for this box.</param>
        /// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
        public static void Box(Rect position, string text, GUIStyle style)
        {
            Box(position, GUIContent.Temp(text), style);
        }

        /// <summary>
        /// <para>Create a Box on the GUI Layer. A Box can contain text, an image, or a combination of these along with an optional tooltip, through using a GUIContent parameter. You may also use a GUIStyle to adjust the layout of items in a box, text colour and other properties.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the box.</param>
        /// <param name="text">Text to display on the box.</param>
        /// <param name="image">Texture to display on the box.</param>
        /// <param name="content">Text, image and tooltip for this box.</param>
        /// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
        public static void Box(Rect position, GUIContent content, GUIStyle style)
        {
            GUIUtility.CheckOnGUI();
            int controlID = GUIUtility.GetControlID(s_BoxHash, FocusType.Passive);
            if (Event.current.type == EventType.Repaint)
            {
                style.Draw(position, content, controlID);
            }
        }

        /// <summary>
        /// <para>Create a Box on the GUI Layer. A Box can contain text, an image, or a combination of these along with an optional tooltip, through using a GUIContent parameter. You may also use a GUIStyle to adjust the layout of items in a box, text colour and other properties.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the box.</param>
        /// <param name="text">Text to display on the box.</param>
        /// <param name="image">Texture to display on the box.</param>
        /// <param name="content">Text, image and tooltip for this box.</param>
        /// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
        public static void Box(Rect position, Texture image, GUIStyle style)
        {
            Box(position, GUIContent.Temp(image), style);
        }

        /// <summary>
        /// <para>Bring a specific window to back of the floating windows.</para>
        /// </summary>
        /// <param name="windowID">The identifier used when you created the window in the Window call.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void BringWindowToBack(int windowID);
        /// <summary>
        /// <para>Bring a specific window to front of the floating windows.</para>
        /// </summary>
        /// <param name="windowID">The identifier used when you created the window in the Window call.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void BringWindowToFront(int windowID);
        /// <summary>
        /// <para>Make a single press button. The user clicks them and something happens immediately.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>true when the users clicks the button.</para>
        /// </returns>
        public static bool Button(Rect position, string text) => 
            Button(position, GUIContent.Temp(text), s_Skin.button);

        /// <summary>
        /// <para>Make a single press button. The user clicks them and something happens immediately.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>true when the users clicks the button.</para>
        /// </returns>
        public static bool Button(Rect position, GUIContent content) => 
            Button(position, content, s_Skin.button);

        /// <summary>
        /// <para>Make a single press button. The user clicks them and something happens immediately.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>true when the users clicks the button.</para>
        /// </returns>
        public static bool Button(Rect position, Texture image) => 
            Button(position, GUIContent.Temp(image), s_Skin.button);

        /// <summary>
        /// <para>Make a single press button. The user clicks them and something happens immediately.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>true when the users clicks the button.</para>
        /// </returns>
        public static bool Button(Rect position, string text, GUIStyle style) => 
            Button(position, GUIContent.Temp(text), style);

        /// <summary>
        /// <para>Make a single press button. The user clicks them and something happens immediately.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>true when the users clicks the button.</para>
        /// </returns>
        public static bool Button(Rect position, GUIContent content, GUIStyle style)
        {
            GUIUtility.CheckOnGUI();
            return DoButton(position, content, style.m_Ptr);
        }

        /// <summary>
        /// <para>Make a single press button. The user clicks them and something happens immediately.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>true when the users clicks the button.</para>
        /// </returns>
        public static bool Button(Rect position, Texture image, GUIStyle style) => 
            Button(position, GUIContent.Temp(image), style);

        private static Rect[] CalcMouseRects(Rect position, int count, int xCount, float elemWidth, float elemHeight, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle, bool addBorders)
        {
            int num = 0;
            int num2 = 0;
            float xMin = position.xMin;
            float yMin = position.yMin;
            GUIStyle style2 = style;
            Rect[] rectArray = new Rect[count];
            if (count > 1)
            {
                style2 = firstStyle;
            }
            for (int i = 0; i < count; i++)
            {
                if (!addBorders)
                {
                    rectArray[i] = new Rect(xMin, yMin, elemWidth, elemHeight);
                }
                else
                {
                    rectArray[i] = style2.margin.Add(new Rect(xMin, yMin, elemWidth, elemHeight));
                }
                rectArray[i].width = Mathf.Round(rectArray[i].xMax) - Mathf.Round(rectArray[i].x);
                rectArray[i].x = Mathf.Round(rectArray[i].x);
                GUIStyle style3 = midStyle;
                if (i == (count - 2))
                {
                    style3 = lastStyle;
                }
                xMin += elemWidth + Mathf.Max(style2.margin.right, style3.margin.left);
                num2++;
                if (num2 >= xCount)
                {
                    num++;
                    num2 = 0;
                    yMin += elemHeight + Mathf.Max(style.margin.top, style.margin.bottom);
                    xMin = position.xMin;
                }
            }
            return rectArray;
        }

        internal static int CalcTotalHorizSpacing(int xCount, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle)
        {
            if (xCount < 2)
            {
                return 0;
            }
            if (xCount == 2)
            {
                return Mathf.Max(firstStyle.margin.right, lastStyle.margin.left);
            }
            int num2 = Mathf.Max(midStyle.margin.left, midStyle.margin.right);
            return ((Mathf.Max(firstStyle.margin.right, midStyle.margin.left) + Mathf.Max(midStyle.margin.right, lastStyle.margin.left)) + (num2 * (xCount - 3)));
        }

        internal static bool CalculateScaledTextureRects(Rect position, ScaleMode scaleMode, float imageAspect, ref Rect outScreenRect, ref Rect outSourceRect)
        {
            float num = position.width / position.height;
            bool flag = false;
            if (scaleMode != ScaleMode.StretchToFill)
            {
                if (scaleMode != ScaleMode.ScaleAndCrop)
                {
                    if (scaleMode != ScaleMode.ScaleToFit)
                    {
                        return flag;
                    }
                    if (num > imageAspect)
                    {
                        float num4 = imageAspect / num;
                        outScreenRect = new Rect(position.xMin + ((position.width * (1f - num4)) * 0.5f), position.yMin, num4 * position.width, position.height);
                        outSourceRect = new Rect(0f, 0f, 1f, 1f);
                        return true;
                    }
                    float num5 = num / imageAspect;
                    outScreenRect = new Rect(position.xMin, position.yMin + ((position.height * (1f - num5)) * 0.5f), position.width, num5 * position.height);
                    outSourceRect = new Rect(0f, 0f, 1f, 1f);
                    return true;
                }
            }
            else
            {
                outScreenRect = position;
                outSourceRect = new Rect(0f, 0f, 1f, 1f);
                return true;
            }
            if (num > imageAspect)
            {
                float height = imageAspect / num;
                outScreenRect = position;
                outSourceRect = new Rect(0f, (1f - height) * 0.5f, 1f, height);
                return true;
            }
            float width = num / imageAspect;
            outScreenRect = position;
            outSourceRect = new Rect(0.5f - (width * 0.5f), 0f, width, 1f);
            return true;
        }

        [RequiredByNativeCode]
        internal static void CallWindowDelegate(WindowFunction func, int id, GUISkin _skin, int forceRect, float width, float height, GUIStyle style)
        {
            GUILayoutUtility.SelectIDList(id, true);
            GUISkin skin = GUI.skin;
            if (Event.current.type == EventType.Layout)
            {
                if (forceRect != 0)
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(width), GUILayout.Height(height) };
                    GUILayoutUtility.BeginWindow(id, style, options);
                }
                else
                {
                    GUILayoutUtility.BeginWindow(id, style, null);
                }
            }
            else
            {
                GUILayoutUtility.BeginWindow(id, GUIStyle.none, null);
            }
            GUI.skin = _skin;
            func(id);
            if (Event.current.type == EventType.Layout)
            {
                GUILayoutUtility.Layout();
            }
            GUI.skin = skin;
        }

        internal static void CleanupRoots()
        {
            s_Skin = null;
            GUIUtility.CleanupRoots();
            GUILayoutUtility.CleanupRoots();
            GUISkin.CleanupRoots();
            GUIStyle.CleanupRoots();
        }

        protected static Vector2 DoBeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background) => 
            BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background);

        private static bool DoButton(Rect position, GUIContent content, IntPtr style) => 
            INTERNAL_CALL_DoButton(ref position, content, style);

        private static int DoButtonGrid(Rect position, int selected, GUIContent[] contents, int xCount, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle)
        {
            GUIUtility.CheckOnGUI();
            int length = contents.Length;
            if (length != 0)
            {
                if (xCount <= 0)
                {
                    UnityEngine.Debug.LogWarning("You are trying to create a SelectionGrid with zero or less elements to be displayed in the horizontal direction. Set xCount to a positive value.");
                    return selected;
                }
                int controlID = GUIUtility.GetControlID(s_ButtonGridHash, FocusType.Passive, position);
                int num4 = length / xCount;
                if ((length % xCount) != 0)
                {
                    num4++;
                }
                float num5 = CalcTotalHorizSpacing(xCount, style, firstStyle, midStyle, lastStyle);
                float num6 = Mathf.Max(style.margin.top, style.margin.bottom) * (num4 - 1);
                float elemWidth = (position.width - num5) / ((float) xCount);
                float elemHeight = (position.height - num6) / ((float) num4);
                if (style.fixedWidth != 0f)
                {
                    elemWidth = style.fixedWidth;
                }
                if (style.fixedHeight != 0f)
                {
                    elemHeight = style.fixedHeight;
                }
                switch (Event.current.GetTypeForControl(controlID))
                {
                    case EventType.MouseDown:
                        if (position.Contains(Event.current.mousePosition) && (GetButtonGridMouseSelection(CalcMouseRects(position, length, xCount, elemWidth, elemHeight, style, firstStyle, midStyle, lastStyle, false), Event.current.mousePosition, true) != -1))
                        {
                            GUIUtility.hotControl = controlID;
                            Event.current.Use();
                        }
                        return selected;

                    case EventType.MouseUp:
                    {
                        if (GUIUtility.hotControl != controlID)
                        {
                            return selected;
                        }
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                        int num9 = GetButtonGridMouseSelection(CalcMouseRects(position, length, xCount, elemWidth, elemHeight, style, firstStyle, midStyle, lastStyle, false), Event.current.mousePosition, true);
                        changed = true;
                        return num9;
                    }
                    case EventType.MouseMove:
                    case EventType.KeyDown:
                    case EventType.KeyUp:
                    case EventType.ScrollWheel:
                        return selected;

                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == controlID)
                        {
                            Event.current.Use();
                        }
                        return selected;

                    case EventType.Repaint:
                    {
                        GUIStyle style2 = null;
                        GUIClip.Push(position, Vector2.zero, Vector2.zero, false);
                        position = new Rect(0f, 0f, position.width, position.height);
                        Rect[] buttonRects = CalcMouseRects(position, length, xCount, elemWidth, elemHeight, style, firstStyle, midStyle, lastStyle, false);
                        int index = GetButtonGridMouseSelection(buttonRects, Event.current.mousePosition, controlID == GUIUtility.hotControl);
                        bool flag = position.Contains(Event.current.mousePosition);
                        GUIUtility.mouseUsed |= flag;
                        for (int i = 0; i < length; i++)
                        {
                            GUIStyle style3 = null;
                            if (i != 0)
                            {
                                style3 = midStyle;
                            }
                            else
                            {
                                style3 = firstStyle;
                            }
                            if (i == (length - 1))
                            {
                                style3 = lastStyle;
                            }
                            if (length == 1)
                            {
                                style3 = style;
                            }
                            if (i != selected)
                            {
                                style3.Draw(buttonRects[i], contents[i], ((i == index) && (enabled || (controlID == GUIUtility.hotControl))) && ((controlID == GUIUtility.hotControl) || (GUIUtility.hotControl == 0)), (controlID == GUIUtility.hotControl) && enabled, false, false);
                            }
                            else
                            {
                                style2 = style3;
                            }
                        }
                        if ((selected < length) && (selected > -1))
                        {
                            style2.Draw(buttonRects[selected], contents[selected], ((selected == index) && (enabled || (controlID == GUIUtility.hotControl))) && ((controlID == GUIUtility.hotControl) || (GUIUtility.hotControl == 0)), controlID == GUIUtility.hotControl, true, false);
                        }
                        if (index >= 0)
                        {
                            tooltip = contents[index].tooltip;
                        }
                        GUIClip.Pop();
                        return selected;
                    }
                }
            }
            return selected;
        }

        private static void DoLabel(Rect position, GUIContent content, IntPtr style)
        {
            INTERNAL_CALL_DoLabel(ref position, content, style);
        }

        private static Rect DoModalWindow(int id, Rect clientRect, WindowFunction func, GUIContent content, GUIStyle style, GUISkin skin)
        {
            Rect rect;
            INTERNAL_CALL_DoModalWindow(id, ref clientRect, func, content, style, skin, out rect);
            return rect;
        }

        private static bool DoRepeatButton(Rect position, GUIContent content, GUIStyle style, FocusType focusType)
        {
            GUIUtility.CheckOnGUI();
            int controlID = GUIUtility.GetControlID(s_RepeatButtonHash, focusType, position);
            EventType typeForControl = Event.current.GetTypeForControl(controlID);
            if (typeForControl != EventType.MouseDown)
            {
                if (typeForControl != EventType.MouseUp)
                {
                    if (typeForControl != EventType.Repaint)
                    {
                        return false;
                    }
                    style.Draw(position, content, controlID);
                    return ((controlID == GUIUtility.hotControl) && position.Contains(Event.current.mousePosition));
                }
            }
            else
            {
                if (position.Contains(Event.current.mousePosition))
                {
                    GUIUtility.hotControl = controlID;
                    Event.current.Use();
                }
                return false;
            }
            if (GUIUtility.hotControl == controlID)
            {
                GUIUtility.hotControl = 0;
                Event.current.Use();
                return position.Contains(Event.current.mousePosition);
            }
            return false;
        }

        internal static void DoSetSkin(GUISkin newSkin)
        {
            if (newSkin == null)
            {
                newSkin = GUIUtility.GetDefaultSkin();
            }
            s_Skin = newSkin;
            newSkin.MakeCurrent();
        }

        internal static void DoTextField(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style)
        {
            DoTextField(position, id, content, multiline, maxLength, style, null);
        }

        internal static void DoTextField(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, string secureText)
        {
            DoTextField(position, id, content, multiline, maxLength, style, secureText, '\0');
        }

        internal static void DoTextField(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, string secureText, char maskChar)
        {
            GUIUtility.CheckOnGUI();
            if ((maxLength >= 0) && (content.text.Length > maxLength))
            {
                content.text = content.text.Substring(0, maxLength);
            }
            TextEditor stateObject = (TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), id);
            stateObject.text = content.text;
            stateObject.SaveBackup();
            stateObject.position = position;
            stateObject.style = style;
            stateObject.multiline = multiline;
            stateObject.controlID = id;
            stateObject.DetectFocusChange();
            if (TouchScreenKeyboard.isSupported)
            {
                HandleTextFieldEventForTouchscreen(position, id, content, multiline, maxLength, style, secureText, maskChar, stateObject);
            }
            else
            {
                HandleTextFieldEventForDesktop(position, id, content, multiline, maxLength, style, stateObject);
            }
            stateObject.UpdateScrollOffsetIfNeeded(Event.current);
        }

        internal static bool DoToggle(Rect position, int id, bool value, GUIContent content, IntPtr style) => 
            INTERNAL_CALL_DoToggle(ref position, id, value, content, style);

        private static Rect DoWindow(int id, Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style, GUISkin skin, bool forceRectOnLayout)
        {
            Rect rect;
            INTERNAL_CALL_DoWindow(id, ref clientRect, func, title, style, skin, forceRectOnLayout, out rect);
            return rect;
        }

        /// <summary>
        /// <para>If you want to have the entire window background to act as a drag area, use the version of DragWindow that takes no parameters and put it at the end of the window function.</para>
        /// </summary>
        public static void DragWindow()
        {
            DragWindow(new Rect(0f, 0f, 10000f, 10000f));
        }

        /// <summary>
        /// <para>Make a window draggable.</para>
        /// </summary>
        /// <param name="position">The part of the window that can be dragged. This is clipped to the actual window.</param>
        public static void DragWindow(Rect position)
        {
            INTERNAL_CALL_DragWindow(ref position);
        }

        /// <summary>
        /// <para>Draw a texture within a rectangle.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to draw the texture within.</param>
        /// <param name="image">Texture to display.</param>
        /// <param name="scaleMode">How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.</param>
        /// <param name="alphaBlend">Whether to apply alpha blending when drawing the image (enabled by default).</param>
        /// <param name="imageAspect">Aspect ratio to use for the source image. If 0 (the default), the aspect ratio from the image is used.  Pass in w/h for the desired aspect ratio.  This allows the aspect ratio of the source image to be adjusted without changing the pixel width and height.</param>
        public static void DrawTexture(Rect position, Texture image)
        {
            DrawTexture(position, image, ScaleMode.StretchToFill);
        }

        /// <summary>
        /// <para>Draw a texture within a rectangle.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to draw the texture within.</param>
        /// <param name="image">Texture to display.</param>
        /// <param name="scaleMode">How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.</param>
        /// <param name="alphaBlend">Whether to apply alpha blending when drawing the image (enabled by default).</param>
        /// <param name="imageAspect">Aspect ratio to use for the source image. If 0 (the default), the aspect ratio from the image is used.  Pass in w/h for the desired aspect ratio.  This allows the aspect ratio of the source image to be adjusted without changing the pixel width and height.</param>
        public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode)
        {
            DrawTexture(position, image, scaleMode, true);
        }

        /// <summary>
        /// <para>Draw a texture within a rectangle.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to draw the texture within.</param>
        /// <param name="image">Texture to display.</param>
        /// <param name="scaleMode">How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.</param>
        /// <param name="alphaBlend">Whether to apply alpha blending when drawing the image (enabled by default).</param>
        /// <param name="imageAspect">Aspect ratio to use for the source image. If 0 (the default), the aspect ratio from the image is used.  Pass in w/h for the desired aspect ratio.  This allows the aspect ratio of the source image to be adjusted without changing the pixel width and height.</param>
        public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend)
        {
            DrawTexture(position, image, scaleMode, alphaBlend, 0f);
        }

        /// <summary>
        /// <para>Draw a texture within a rectangle.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to draw the texture within.</param>
        /// <param name="image">Texture to display.</param>
        /// <param name="scaleMode">How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.</param>
        /// <param name="alphaBlend">Whether to apply alpha blending when drawing the image (enabled by default).</param>
        /// <param name="imageAspect">Aspect ratio to use for the source image. If 0 (the default), the aspect ratio from the image is used.  Pass in w/h for the desired aspect ratio.  This allows the aspect ratio of the source image to be adjusted without changing the pixel width and height.</param>
        public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend, float imageAspect)
        {
            GUIUtility.CheckOnGUI();
            if (Event.current.type == EventType.Repaint)
            {
                if (image == null)
                {
                    UnityEngine.Debug.LogWarning("null texture passed to GUI.DrawTexture");
                }
                else
                {
                    if (imageAspect == 0f)
                    {
                        imageAspect = ((float) image.width) / ((float) image.height);
                    }
                    Material material = !alphaBlend ? blitMaterial : blendMaterial;
                    InternalDrawTextureArguments arguments = new InternalDrawTextureArguments {
                        texture = image,
                        leftBorder = 0,
                        rightBorder = 0,
                        topBorder = 0,
                        bottomBorder = 0,
                        color = color,
                        mat = material
                    };
                    CalculateScaledTextureRects(position, scaleMode, imageAspect, ref arguments.screenRect, ref arguments.sourceRect);
                    Graphics.DrawTexture(ref arguments);
                }
            }
        }

        /// <summary>
        /// <para>Draw a texture within a rectangle with the given texture coordinates. Use this function for clipping or tiling the image within the given rectangle.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to draw the texture within.</param>
        /// <param name="image">Texture to display.</param>
        /// <param name="texCoords">How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.</param>
        /// <param name="alphaBlend">Whether to alpha blend the image on to the display (the default). If false, the picture is drawn on to the display.</param>
        public static void DrawTextureWithTexCoords(Rect position, Texture image, Rect texCoords)
        {
            DrawTextureWithTexCoords(position, image, texCoords, true);
        }

        /// <summary>
        /// <para>Draw a texture within a rectangle with the given texture coordinates. Use this function for clipping or tiling the image within the given rectangle.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to draw the texture within.</param>
        /// <param name="image">Texture to display.</param>
        /// <param name="texCoords">How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.</param>
        /// <param name="alphaBlend">Whether to alpha blend the image on to the display (the default). If false, the picture is drawn on to the display.</param>
        public static void DrawTextureWithTexCoords(Rect position, Texture image, Rect texCoords, bool alphaBlend)
        {
            GUIUtility.CheckOnGUI();
            if (Event.current.type == EventType.Repaint)
            {
                Material material = !alphaBlend ? blitMaterial : blendMaterial;
                InternalDrawTextureArguments arguments = new InternalDrawTextureArguments {
                    texture = image,
                    leftBorder = 0,
                    rightBorder = 0,
                    topBorder = 0,
                    bottomBorder = 0,
                    color = color,
                    mat = material,
                    screenRect = position,
                    sourceRect = texCoords
                };
                Graphics.DrawTexture(ref arguments);
            }
        }

        public static void EndClip()
        {
            GUIUtility.CheckOnGUI();
            GUIClip.Pop();
        }

        /// <summary>
        /// <para>End a group.</para>
        /// </summary>
        public static void EndGroup()
        {
            GUIUtility.CheckOnGUI();
            GUIClip.Pop();
        }

        /// <summary>
        /// <para>Ends a scrollview started with a call to BeginScrollView.</para>
        /// </summary>
        /// <param name="handleScrollWheel"></param>
        public static void EndScrollView()
        {
            EndScrollView(true);
        }

        /// <summary>
        /// <para>Ends a scrollview started with a call to BeginScrollView.</para>
        /// </summary>
        /// <param name="handleScrollWheel"></param>
        public static void EndScrollView(bool handleScrollWheel)
        {
            GUIUtility.CheckOnGUI();
            ScrollViewState state = (ScrollViewState) s_ScrollViewStates.Peek();
            GUIClip.Pop();
            s_ScrollViewStates.Pop();
            if ((handleScrollWheel && (Event.current.type == EventType.ScrollWheel)) && state.position.Contains(Event.current.mousePosition))
            {
                state.scrollPosition.x = Mathf.Clamp((float) (state.scrollPosition.x + (Event.current.delta.x * 20f)), (float) 0f, (float) (state.viewRect.width - state.visibleRect.width));
                state.scrollPosition.y = Mathf.Clamp((float) (state.scrollPosition.y + (Event.current.delta.y * 20f)), (float) 0f, (float) (state.viewRect.height - state.visibleRect.height));
                state.apply = true;
                Event.current.Use();
            }
        }

        internal static void EndWindows()
        {
            GUILayoutGroup topLevel = GUILayoutUtility.current.topLevel;
            GenericStack layoutGroups = GUILayoutUtility.current.layoutGroups;
            GUILayoutGroup windows = GUILayoutUtility.current.windows;
            Internal_EndWindows();
            GUILayoutUtility.current.topLevel = topLevel;
            GUILayoutUtility.current.layoutGroups = layoutGroups;
            GUILayoutUtility.current.windows = windows;
        }

        internal static void FindStyles(ref GUIStyle style, out GUIStyle firstStyle, out GUIStyle midStyle, out GUIStyle lastStyle, string first, string mid, string last)
        {
            if (style == null)
            {
                style = skin.button;
            }
            string name = style.name;
            midStyle = skin.FindStyle(name + mid);
            if (midStyle == null)
            {
                midStyle = style;
            }
            firstStyle = skin.FindStyle(name + first);
            if (firstStyle == null)
            {
                firstStyle = midStyle;
            }
            lastStyle = skin.FindStyle(name + last);
            if (lastStyle == null)
            {
                lastStyle = midStyle;
            }
        }

        /// <summary>
        /// <para>Move keyboard focus to a named control.</para>
        /// </summary>
        /// <param name="name">Name set using SetNextControlName.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void FocusControl(string name);
        /// <summary>
        /// <para>Make a window become the active window.</para>
        /// </summary>
        /// <param name="windowID">The identifier used when you created the window in the Window call.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void FocusWindow(int windowID);
        private static int GetButtonGridMouseSelection(Rect[] buttonRects, Vector2 mousePos, bool findNearest)
        {
            for (int i = 0; i < buttonRects.Length; i++)
            {
                if (buttonRects[i].Contains(mousePos))
                {
                    return i;
                }
            }
            if (!findNearest)
            {
                return -1;
            }
            float num3 = 1E+07f;
            int num4 = -1;
            for (int j = 0; j < buttonRects.Length; j++)
            {
                Rect rect = buttonRects[j];
                Vector2 vector = new Vector2(Mathf.Clamp(mousePos.x, rect.xMin, rect.xMax), Mathf.Clamp(mousePos.y, rect.yMin, rect.yMax));
                Vector2 vector2 = mousePos - vector;
                float sqrMagnitude = vector2.sqrMagnitude;
                if (sqrMagnitude < num3)
                {
                    num4 = j;
                    num3 = sqrMagnitude;
                }
            }
            return num4;
        }

        /// <summary>
        /// <para>Get the name of named control that has focus.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetNameOfFocusedControl();
        internal static ScrollViewState GetTopScrollView()
        {
            if (s_ScrollViewStates.Count != 0)
            {
                return (ScrollViewState) s_ScrollViewStates.Peek();
            }
            return null;
        }

        private static void HandleTextFieldEventForDesktop(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, TextEditor editor)
        {
            Event current = Event.current;
            bool flag = false;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition))
                    {
                        GUIUtility.hotControl = id;
                        GUIUtility.keyboardControl = id;
                        editor.m_HasFocus = true;
                        editor.MoveCursorToPosition(Event.current.mousePosition);
                        if ((Event.current.clickCount == 2) && skin.settings.doubleClickSelectsWord)
                        {
                            editor.SelectCurrentWord();
                            editor.DblClickSnap(TextEditor.DblClickSnapping.WORDS);
                            editor.MouseDragSelectsWholeWords(true);
                        }
                        if ((Event.current.clickCount == 3) && skin.settings.tripleClickSelectsLine)
                        {
                            editor.SelectCurrentParagraph();
                            editor.MouseDragSelectsWholeWords(true);
                            editor.DblClickSnap(TextEditor.DblClickSnapping.PARAGRAPHS);
                        }
                        current.Use();
                    }
                    goto Label_028F;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id)
                    {
                        editor.MouseDragSelectsWholeWords(false);
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    goto Label_028F;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl != id)
                    {
                        goto Label_028F;
                    }
                    if (!current.shift)
                    {
                        editor.SelectToPosition(Event.current.mousePosition);
                        break;
                    }
                    editor.MoveCursorToPosition(Event.current.mousePosition);
                    break;

                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == id)
                    {
                        if (editor.HandleKeyEvent(current))
                        {
                            current.Use();
                            flag = true;
                            content.text = editor.text;
                        }
                        else
                        {
                            if ((current.keyCode == KeyCode.Tab) || (current.character == '\t'))
                            {
                                return;
                            }
                            char character = current.character;
                            if (((character == '\n') && !multiline) && !current.alt)
                            {
                                return;
                            }
                            Font font = style.font;
                            if (font == null)
                            {
                                font = skin.font;
                            }
                            if (font.HasCharacter(character) || (character == '\n'))
                            {
                                editor.Insert(character);
                                flag = true;
                            }
                            else if (character == '\0')
                            {
                                if (Input.compositionString.Length > 0)
                                {
                                    editor.ReplaceSelection("");
                                    flag = true;
                                }
                                current.Use();
                            }
                        }
                        goto Label_028F;
                    }
                    return;

                case EventType.Repaint:
                    if (GUIUtility.keyboardControl == id)
                    {
                        editor.DrawCursor(content.text);
                    }
                    else
                    {
                        style.Draw(position, content, id, false);
                    }
                    goto Label_028F;

                default:
                    goto Label_028F;
            }
            current.Use();
        Label_028F:
            if (GUIUtility.keyboardControl == id)
            {
                GUIUtility.textFieldInput = true;
            }
            if (flag)
            {
                changed = true;
                content.text = editor.text;
                if ((maxLength >= 0) && (content.text.Length > maxLength))
                {
                    content.text = content.text.Substring(0, maxLength);
                }
                current.Use();
            }
        }

        private static void HandleTextFieldEventForTouchscreen(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, string secureText, char maskChar, TextEditor editor)
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition))
                    {
                        GUIUtility.hotControl = id;
                        if ((s_HotTextField != -1) && (s_HotTextField != id))
                        {
                            TextEditor stateObject = (TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), s_HotTextField);
                            stateObject.keyboardOnScreen = null;
                        }
                        s_HotTextField = id;
                        if (GUIUtility.keyboardControl != id)
                        {
                            GUIUtility.keyboardControl = id;
                        }
                        editor.keyboardOnScreen = TouchScreenKeyboard.Open((secureText == null) ? content.text : secureText, TouchScreenKeyboardType.Default, true, multiline, secureText != null);
                        current.Use();
                    }
                    break;

                case EventType.Repaint:
                {
                    if (editor.keyboardOnScreen != null)
                    {
                        content.text = editor.keyboardOnScreen.text;
                        if ((maxLength >= 0) && (content.text.Length > maxLength))
                        {
                            content.text = content.text.Substring(0, maxLength);
                        }
                        if (editor.keyboardOnScreen.done)
                        {
                            editor.keyboardOnScreen = null;
                            changed = true;
                        }
                    }
                    string text = content.text;
                    if (secureText != null)
                    {
                        content.text = PasswordFieldGetStrToShow(text, maskChar);
                    }
                    style.Draw(position, content, id, false);
                    content.text = text;
                    break;
                }
            }
        }

        /// <summary>
        /// <para>Make a horizontal scrollbar. Scrollbars are what you use to scroll through a document. Most likely, you want to use scrollViews instead.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the scrollbar.</param>
        /// <param name="value">The position between min and max.</param>
        /// <param name="size">How much can we see?</param>
        /// <param name="leftValue">The value at the left end of the scrollbar.</param>
        /// <param name="rightValue">The value at the right end of the scrollbar.</param>
        /// <param name="style">The style to use for the scrollbar background. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The modified value. This can be changed by the user by dragging the scrollbar, or clicking the arrows at the end.</para>
        /// </returns>
        public static float HorizontalScrollbar(Rect position, float value, float size, float leftValue, float rightValue) => 
            Scroller(position, value, size, leftValue, rightValue, skin.horizontalScrollbar, skin.horizontalScrollbarThumb, skin.horizontalScrollbarLeftButton, skin.horizontalScrollbarRightButton, true);

        /// <summary>
        /// <para>Make a horizontal scrollbar. Scrollbars are what you use to scroll through a document. Most likely, you want to use scrollViews instead.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the scrollbar.</param>
        /// <param name="value">The position between min and max.</param>
        /// <param name="size">How much can we see?</param>
        /// <param name="leftValue">The value at the left end of the scrollbar.</param>
        /// <param name="rightValue">The value at the right end of the scrollbar.</param>
        /// <param name="style">The style to use for the scrollbar background. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The modified value. This can be changed by the user by dragging the scrollbar, or clicking the arrows at the end.</para>
        /// </returns>
        public static float HorizontalScrollbar(Rect position, float value, float size, float leftValue, float rightValue, GUIStyle style) => 
            Scroller(position, value, size, leftValue, rightValue, style, skin.GetStyle(style.name + "thumb"), skin.GetStyle(style.name + "leftbutton"), skin.GetStyle(style.name + "rightbutton"), true);

        /// <summary>
        /// <para>A horizontal slider the user can drag to change a value between a min and a max.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the slider.</param>
        /// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="slider">The GUIStyle to use for displaying the dragging area. If left out, the horizontalSlider style from the current GUISkin is used.</param>
        /// <param name="thumb">The GUIStyle to use for displaying draggable thumb. If left out, the horizontalSliderThumb style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The value that has been set by the user.</para>
        /// </returns>
        public static float HorizontalSlider(Rect position, float value, float leftValue, float rightValue) => 
            Slider(position, value, 0f, leftValue, rightValue, skin.horizontalSlider, skin.horizontalSliderThumb, true, 0);

        /// <summary>
        /// <para>A horizontal slider the user can drag to change a value between a min and a max.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the slider.</param>
        /// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="slider">The GUIStyle to use for displaying the dragging area. If left out, the horizontalSlider style from the current GUISkin is used.</param>
        /// <param name="thumb">The GUIStyle to use for displaying draggable thumb. If left out, the horizontalSliderThumb style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The value that has been set by the user.</para>
        /// </returns>
        public static float HorizontalSlider(Rect position, float value, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb) => 
            Slider(position, value, 0f, leftValue, rightValue, slider, thumb, true, 0);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void InitializeGUIClipTexture();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_BeginWindows();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_DoButton(ref Rect position, GUIContent content, IntPtr style);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_DoLabel(ref Rect position, GUIContent content, IntPtr style);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_DoModalWindow(int id, ref Rect clientRect, WindowFunction func, GUIContent content, GUIStyle style, GUISkin skin, out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_DoToggle(ref Rect position, int id, bool value, GUIContent content, IntPtr style);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_DoWindow(int id, ref Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style, GUISkin skin, bool forceRectOnLayout, out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_DragWindow(ref Rect position);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_EndWindows();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_get_backgroundColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_get_color(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_get_contentColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string Internal_GetMouseTooltip();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string Internal_GetTooltip();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_set_backgroundColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_set_color(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_set_contentColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_SetTooltip(string value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void InternalRepaintEditorWindow();
        /// <summary>
        /// <para>Make a text or texture label on screen.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the label.</param>
        /// <param name="text">Text to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
        public static void Label(Rect position, string text)
        {
            Label(position, GUIContent.Temp(text), s_Skin.label);
        }

        /// <summary>
        /// <para>Make a text or texture label on screen.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the label.</param>
        /// <param name="text">Text to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
        public static void Label(Rect position, GUIContent content)
        {
            Label(position, content, s_Skin.label);
        }

        /// <summary>
        /// <para>Make a text or texture label on screen.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the label.</param>
        /// <param name="text">Text to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
        public static void Label(Rect position, Texture image)
        {
            Label(position, GUIContent.Temp(image), s_Skin.label);
        }

        /// <summary>
        /// <para>Make a text or texture label on screen.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the label.</param>
        /// <param name="text">Text to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
        public static void Label(Rect position, string text, GUIStyle style)
        {
            Label(position, GUIContent.Temp(text), style);
        }

        /// <summary>
        /// <para>Make a text or texture label on screen.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the label.</param>
        /// <param name="text">Text to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
        public static void Label(Rect position, GUIContent content, GUIStyle style)
        {
            GUIUtility.CheckOnGUI();
            DoLabel(position, content, style.m_Ptr);
        }

        /// <summary>
        /// <para>Make a text or texture label on screen.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the label.</param>
        /// <param name="text">Text to display on the label.</param>
        /// <param name="image">Texture to display on the label.</param>
        /// <param name="content">Text, image and tooltip for this label.</param>
        /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
        public static void Label(Rect position, Texture image, GUIStyle style)
        {
            Label(position, GUIContent.Temp(image), style);
        }

        public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, string text)
        {
            GUIUtility.CheckOnGUI();
            return DoModalWindow(id, clientRect, func, GUIContent.Temp(text), skin.window, skin);
        }

        public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, GUIContent content)
        {
            GUIUtility.CheckOnGUI();
            return DoModalWindow(id, clientRect, func, content, skin.window, skin);
        }

        public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, Texture image)
        {
            GUIUtility.CheckOnGUI();
            return DoModalWindow(id, clientRect, func, GUIContent.Temp(image), skin.window, skin);
        }

        public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, string text, GUIStyle style)
        {
            GUIUtility.CheckOnGUI();
            return DoModalWindow(id, clientRect, func, GUIContent.Temp(text), style, skin);
        }

        public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, GUIContent content, GUIStyle style)
        {
            GUIUtility.CheckOnGUI();
            return DoModalWindow(id, clientRect, func, content, style, skin);
        }

        public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, Texture image, GUIStyle style)
        {
            GUIUtility.CheckOnGUI();
            return DoModalWindow(id, clientRect, func, GUIContent.Temp(image), style, skin);
        }

        /// <summary>
        /// <para>Make a text field where the user can enter a password.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the text field.</param>
        /// <param name="password">Password to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
        /// <param name="maskChar">Character to mask the password with.</param>
        /// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
        /// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The edited password.</para>
        /// </returns>
        public static string PasswordField(Rect position, string password, char maskChar) => 
            PasswordField(position, password, maskChar, -1, skin.textField);

        /// <summary>
        /// <para>Make a text field where the user can enter a password.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the text field.</param>
        /// <param name="password">Password to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
        /// <param name="maskChar">Character to mask the password with.</param>
        /// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
        /// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The edited password.</para>
        /// </returns>
        public static string PasswordField(Rect position, string password, char maskChar, int maxLength) => 
            PasswordField(position, password, maskChar, maxLength, skin.textField);

        /// <summary>
        /// <para>Make a text field where the user can enter a password.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the text field.</param>
        /// <param name="password">Password to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
        /// <param name="maskChar">Character to mask the password with.</param>
        /// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
        /// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The edited password.</para>
        /// </returns>
        public static string PasswordField(Rect position, string password, char maskChar, GUIStyle style) => 
            PasswordField(position, password, maskChar, -1, style);

        /// <summary>
        /// <para>Make a text field where the user can enter a password.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the text field.</param>
        /// <param name="password">Password to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
        /// <param name="maskChar">Character to mask the password with.</param>
        /// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
        /// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The edited password.</para>
        /// </returns>
        public static string PasswordField(Rect position, string password, char maskChar, int maxLength, GUIStyle style)
        {
            GUIUtility.CheckOnGUI();
            GUIContent content = GUIContent.Temp(PasswordFieldGetStrToShow(password, maskChar));
            bool changed = GUI.changed;
            GUI.changed = false;
            if (TouchScreenKeyboard.isSupported)
            {
                DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard), content, false, maxLength, style, password, maskChar);
            }
            else
            {
                DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), content, false, maxLength, style);
            }
            string str = !GUI.changed ? password : content.text;
            GUI.changed |= changed;
            return str;
        }

        internal static string PasswordFieldGetStrToShow(string password, char maskChar) => 
            (((Event.current.type != EventType.Repaint) && (Event.current.type != EventType.MouseDown)) ? password : "".PadRight(password.Length, maskChar));

        /// <summary>
        /// <para>Make a button that is active as long as the user holds it down.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>True when the users clicks the button.</para>
        /// </returns>
        public static bool RepeatButton(Rect position, string text) => 
            DoRepeatButton(position, GUIContent.Temp(text), s_Skin.button, FocusType.Passive);

        /// <summary>
        /// <para>Make a button that is active as long as the user holds it down.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>True when the users clicks the button.</para>
        /// </returns>
        public static bool RepeatButton(Rect position, GUIContent content) => 
            DoRepeatButton(position, content, s_Skin.button, FocusType.Passive);

        /// <summary>
        /// <para>Make a button that is active as long as the user holds it down.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>True when the users clicks the button.</para>
        /// </returns>
        public static bool RepeatButton(Rect position, Texture image) => 
            DoRepeatButton(position, GUIContent.Temp(image), s_Skin.button, FocusType.Passive);

        /// <summary>
        /// <para>Make a button that is active as long as the user holds it down.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>True when the users clicks the button.</para>
        /// </returns>
        public static bool RepeatButton(Rect position, string text, GUIStyle style) => 
            DoRepeatButton(position, GUIContent.Temp(text), style, FocusType.Passive);

        /// <summary>
        /// <para>Make a button that is active as long as the user holds it down.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>True when the users clicks the button.</para>
        /// </returns>
        public static bool RepeatButton(Rect position, GUIContent content, GUIStyle style) => 
            DoRepeatButton(position, content, style, FocusType.Passive);

        /// <summary>
        /// <para>Make a button that is active as long as the user holds it down.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>True when the users clicks the button.</para>
        /// </returns>
        public static bool RepeatButton(Rect position, Texture image, GUIStyle style) => 
            DoRepeatButton(position, GUIContent.Temp(image), style, FocusType.Passive);

        internal static float Scroller(Rect position, float value, float size, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, GUIStyle leftButton, GUIStyle rightButton, bool horiz)
        {
            Rect rect;
            Rect rect2;
            Rect rect3;
            GUIUtility.CheckOnGUI();
            int id = GUIUtility.GetControlID(s_SliderHash, FocusType.Passive, position);
            if (horiz)
            {
                rect = new Rect(position.x + leftButton.fixedWidth, position.y, (position.width - leftButton.fixedWidth) - rightButton.fixedWidth, position.height);
                rect2 = new Rect(position.x, position.y, leftButton.fixedWidth, position.height);
                rect3 = new Rect(position.xMax - rightButton.fixedWidth, position.y, rightButton.fixedWidth, position.height);
            }
            else
            {
                rect = new Rect(position.x, position.y + leftButton.fixedHeight, position.width, (position.height - leftButton.fixedHeight) - rightButton.fixedHeight);
                rect2 = new Rect(position.x, position.y, position.width, leftButton.fixedHeight);
                rect3 = new Rect(position.x, position.yMax - rightButton.fixedHeight, position.width, rightButton.fixedHeight);
            }
            value = Slider(rect, value, size, leftValue, rightValue, slider, thumb, horiz, id);
            bool flag = false;
            if (Event.current.type == EventType.MouseUp)
            {
                flag = true;
            }
            if (ScrollerRepeatButton(id, rect2, leftButton))
            {
                value -= s_ScrollStepSize * ((leftValue >= rightValue) ? -1f : 1f);
            }
            if (ScrollerRepeatButton(id, rect3, rightButton))
            {
                value += s_ScrollStepSize * ((leftValue >= rightValue) ? -1f : 1f);
            }
            if (flag && (Event.current.type == EventType.Used))
            {
                s_ScrollControlId = 0;
            }
            if (leftValue < rightValue)
            {
                value = Mathf.Clamp(value, leftValue, rightValue - size);
                return value;
            }
            value = Mathf.Clamp(value, rightValue, leftValue - size);
            return value;
        }

        internal static bool ScrollerRepeatButton(int scrollerID, Rect rect, GUIStyle style)
        {
            bool flag = false;
            if (DoRepeatButton(rect, GUIContent.none, style, FocusType.Passive))
            {
                bool flag2 = s_ScrollControlId != scrollerID;
                s_ScrollControlId = scrollerID;
                if (flag2)
                {
                    flag = true;
                    nextScrollStepTime = DateTime.Now.AddMilliseconds(250.0);
                }
                else if (DateTime.Now >= nextScrollStepTime)
                {
                    flag = true;
                    nextScrollStepTime = DateTime.Now.AddMilliseconds(30.0);
                }
                if (Event.current.type == EventType.Repaint)
                {
                    InternalRepaintEditorWindow();
                }
            }
            return flag;
        }

        /// <summary>
        /// <para>Scrolls all enclosing scrollviews so they try to make position visible.</para>
        /// </summary>
        /// <param name="position"></param>
        public static void ScrollTo(Rect position)
        {
            ScrollViewState topScrollView = GetTopScrollView();
            if (topScrollView != null)
            {
                topScrollView.ScrollTo(position);
            }
        }

        public static bool ScrollTowards(Rect position, float maxDelta) => 
            GetTopScrollView()?.ScrollTowards(position, maxDelta);

        /// <summary>
        /// <para>Make a grid of buttons.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the grid.</param>
        /// <param name="selected">The index of the selected grid button.</param>
        /// <param name="texts">An array of strings to show on the grid buttons.</param>
        /// <param name="images">An array of textures on the grid buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the grid button.</param>
        /// <param name="xCount">How many elements to fit in the horizontal direction. The controls will be scaled to fit unless the style defines a fixedWidth to use.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="content"></param>
        /// <returns>
        /// <para>The index of the selected button.</para>
        /// </returns>
        public static int SelectionGrid(Rect position, int selected, string[] texts, int xCount) => 
            SelectionGrid(position, selected, GUIContent.Temp(texts), xCount, null);

        /// <summary>
        /// <para>Make a grid of buttons.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the grid.</param>
        /// <param name="selected">The index of the selected grid button.</param>
        /// <param name="texts">An array of strings to show on the grid buttons.</param>
        /// <param name="images">An array of textures on the grid buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the grid button.</param>
        /// <param name="xCount">How many elements to fit in the horizontal direction. The controls will be scaled to fit unless the style defines a fixedWidth to use.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="content"></param>
        /// <returns>
        /// <para>The index of the selected button.</para>
        /// </returns>
        public static int SelectionGrid(Rect position, int selected, GUIContent[] content, int xCount) => 
            SelectionGrid(position, selected, content, xCount, null);

        /// <summary>
        /// <para>Make a grid of buttons.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the grid.</param>
        /// <param name="selected">The index of the selected grid button.</param>
        /// <param name="texts">An array of strings to show on the grid buttons.</param>
        /// <param name="images">An array of textures on the grid buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the grid button.</param>
        /// <param name="xCount">How many elements to fit in the horizontal direction. The controls will be scaled to fit unless the style defines a fixedWidth to use.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="content"></param>
        /// <returns>
        /// <para>The index of the selected button.</para>
        /// </returns>
        public static int SelectionGrid(Rect position, int selected, Texture[] images, int xCount) => 
            SelectionGrid(position, selected, GUIContent.Temp(images), xCount, null);

        /// <summary>
        /// <para>Make a grid of buttons.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the grid.</param>
        /// <param name="selected">The index of the selected grid button.</param>
        /// <param name="texts">An array of strings to show on the grid buttons.</param>
        /// <param name="images">An array of textures on the grid buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the grid button.</param>
        /// <param name="xCount">How many elements to fit in the horizontal direction. The controls will be scaled to fit unless the style defines a fixedWidth to use.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="content"></param>
        /// <returns>
        /// <para>The index of the selected button.</para>
        /// </returns>
        public static int SelectionGrid(Rect position, int selected, string[] texts, int xCount, GUIStyle style) => 
            SelectionGrid(position, selected, GUIContent.Temp(texts), xCount, style);

        /// <summary>
        /// <para>Make a grid of buttons.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the grid.</param>
        /// <param name="selected">The index of the selected grid button.</param>
        /// <param name="texts">An array of strings to show on the grid buttons.</param>
        /// <param name="images">An array of textures on the grid buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the grid button.</param>
        /// <param name="xCount">How many elements to fit in the horizontal direction. The controls will be scaled to fit unless the style defines a fixedWidth to use.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="content"></param>
        /// <returns>
        /// <para>The index of the selected button.</para>
        /// </returns>
        public static int SelectionGrid(Rect position, int selected, GUIContent[] contents, int xCount, GUIStyle style)
        {
            if (style == null)
            {
                style = s_Skin.button;
            }
            return DoButtonGrid(position, selected, contents, xCount, style, style, style, style);
        }

        /// <summary>
        /// <para>Make a grid of buttons.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the grid.</param>
        /// <param name="selected">The index of the selected grid button.</param>
        /// <param name="texts">An array of strings to show on the grid buttons.</param>
        /// <param name="images">An array of textures on the grid buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the grid button.</param>
        /// <param name="xCount">How many elements to fit in the horizontal direction. The controls will be scaled to fit unless the style defines a fixedWidth to use.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="content"></param>
        /// <returns>
        /// <para>The index of the selected button.</para>
        /// </returns>
        public static int SelectionGrid(Rect position, int selected, Texture[] images, int xCount, GUIStyle style) => 
            SelectionGrid(position, selected, GUIContent.Temp(images), xCount, style);

        /// <summary>
        /// <para>Set the name of the next control.</para>
        /// </summary>
        /// <param name="name"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetNextControlName(string name);
        public static float Slider(Rect position, float value, float size, float start, float end, GUIStyle sliderStyle, GUIStyle thumbStyle, bool horiz, int id)
        {
            GUIUtility.CheckOnGUI();
            if (id == 0)
            {
                id = GUIUtility.GetControlID(s_SliderHash, FocusType.Passive, position);
            }
            SliderHandler handler = new SliderHandler(position, value, size, start, end, sliderStyle, thumbStyle, horiz, id);
            return handler.Handle();
        }

        /// <summary>
        /// <para>Make a Multi-line text area where the user can edit a string.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the text field.</param>
        /// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
        /// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
        /// <param name="style">The style to use. If left out, the textArea style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The edited string.</para>
        /// </returns>
        public static string TextArea(Rect position, string text)
        {
            GUIContent content = GUIContent.Temp(text);
            DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), content, true, -1, skin.textArea);
            return content.text;
        }

        /// <summary>
        /// <para>Make a Multi-line text area where the user can edit a string.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the text field.</param>
        /// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
        /// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
        /// <param name="style">The style to use. If left out, the textArea style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The edited string.</para>
        /// </returns>
        public static string TextArea(Rect position, string text, int maxLength)
        {
            GUIContent content = GUIContent.Temp(text);
            DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), content, true, maxLength, skin.textArea);
            return content.text;
        }

        /// <summary>
        /// <para>Make a Multi-line text area where the user can edit a string.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the text field.</param>
        /// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
        /// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
        /// <param name="style">The style to use. If left out, the textArea style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The edited string.</para>
        /// </returns>
        public static string TextArea(Rect position, string text, GUIStyle style)
        {
            GUIContent content = GUIContent.Temp(text);
            DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), content, true, -1, style);
            return content.text;
        }

        /// <summary>
        /// <para>Make a Multi-line text area where the user can edit a string.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the text field.</param>
        /// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
        /// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
        /// <param name="style">The style to use. If left out, the textArea style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The edited string.</para>
        /// </returns>
        public static string TextArea(Rect position, string text, int maxLength, GUIStyle style)
        {
            GUIContent content = GUIContent.Temp(text);
            DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), content, false, maxLength, style);
            return content.text;
        }

        private static string TextArea(Rect position, GUIContent content, int maxLength, GUIStyle style)
        {
            GUIContent content2 = GUIContent.Temp(content.text, content.image);
            DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), content2, false, maxLength, style);
            return content2.text;
        }

        /// <summary>
        /// <para>Make a single-line text field where the user can edit a string.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the text field.</param>
        /// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
        /// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
        /// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The edited string.</para>
        /// </returns>
        public static string TextField(Rect position, string text)
        {
            GUIContent content = GUIContent.Temp(text);
            DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), content, false, -1, skin.textField);
            return content.text;
        }

        /// <summary>
        /// <para>Make a single-line text field where the user can edit a string.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the text field.</param>
        /// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
        /// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
        /// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The edited string.</para>
        /// </returns>
        public static string TextField(Rect position, string text, int maxLength)
        {
            GUIContent content = GUIContent.Temp(text);
            DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), content, false, maxLength, skin.textField);
            return content.text;
        }

        /// <summary>
        /// <para>Make a single-line text field where the user can edit a string.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the text field.</param>
        /// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
        /// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
        /// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The edited string.</para>
        /// </returns>
        public static string TextField(Rect position, string text, GUIStyle style)
        {
            GUIContent content = GUIContent.Temp(text);
            DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), content, false, -1, style);
            return content.text;
        }

        /// <summary>
        /// <para>Make a single-line text field where the user can edit a string.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the text field.</param>
        /// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
        /// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
        /// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The edited string.</para>
        /// </returns>
        public static string TextField(Rect position, string text, int maxLength, GUIStyle style)
        {
            GUIContent content = GUIContent.Temp(text);
            DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), content, false, maxLength, style);
            return content.text;
        }

        /// <summary>
        /// <para>Make an on/off toggle button.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="value">Is this button on or off?</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the toggle style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The new value of the button.</para>
        /// </returns>
        public static bool Toggle(Rect position, bool value, string text) => 
            Toggle(position, value, GUIContent.Temp(text), s_Skin.toggle);

        /// <summary>
        /// <para>Make an on/off toggle button.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="value">Is this button on or off?</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the toggle style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The new value of the button.</para>
        /// </returns>
        public static bool Toggle(Rect position, bool value, GUIContent content) => 
            Toggle(position, value, content, s_Skin.toggle);

        /// <summary>
        /// <para>Make an on/off toggle button.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="value">Is this button on or off?</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the toggle style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The new value of the button.</para>
        /// </returns>
        public static bool Toggle(Rect position, bool value, Texture image) => 
            Toggle(position, value, GUIContent.Temp(image), s_Skin.toggle);

        /// <summary>
        /// <para>Make an on/off toggle button.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="value">Is this button on or off?</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the toggle style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The new value of the button.</para>
        /// </returns>
        public static bool Toggle(Rect position, bool value, string text, GUIStyle style) => 
            Toggle(position, value, GUIContent.Temp(text), style);

        /// <summary>
        /// <para>Make an on/off toggle button.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="value">Is this button on or off?</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the toggle style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The new value of the button.</para>
        /// </returns>
        public static bool Toggle(Rect position, bool value, GUIContent content, GUIStyle style)
        {
            GUIUtility.CheckOnGUI();
            return DoToggle(position, GUIUtility.GetControlID(s_ToggleHash, FocusType.Passive, position), value, content, style.m_Ptr);
        }

        /// <summary>
        /// <para>Make an on/off toggle button.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="value">Is this button on or off?</param>
        /// <param name="text">Text to display on the button.</param>
        /// <param name="image">Texture to display on the button.</param>
        /// <param name="content">Text, image and tooltip for this button.</param>
        /// <param name="style">The style to use. If left out, the toggle style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The new value of the button.</para>
        /// </returns>
        public static bool Toggle(Rect position, bool value, Texture image, GUIStyle style) => 
            Toggle(position, value, GUIContent.Temp(image), style);

        public static bool Toggle(Rect position, int id, bool value, GUIContent content, GUIStyle style)
        {
            GUIUtility.CheckOnGUI();
            return DoToggle(position, id, value, content, style.m_Ptr);
        }

        /// <summary>
        /// <para>Make a toolbar.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the toolbar.</param>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="texts">An array of strings to show on the toolbar buttons.</param>
        /// <param name="images">An array of textures on the toolbar buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the toolbar buttons.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="content"></param>
        /// <returns>
        /// <para>The index of the selected button.</para>
        /// </returns>
        public static int Toolbar(Rect position, int selected, string[] texts) => 
            Toolbar(position, selected, GUIContent.Temp(texts), s_Skin.button);

        /// <summary>
        /// <para>Make a toolbar.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the toolbar.</param>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="texts">An array of strings to show on the toolbar buttons.</param>
        /// <param name="images">An array of textures on the toolbar buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the toolbar buttons.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="content"></param>
        /// <returns>
        /// <para>The index of the selected button.</para>
        /// </returns>
        public static int Toolbar(Rect position, int selected, GUIContent[] content) => 
            Toolbar(position, selected, content, s_Skin.button);

        /// <summary>
        /// <para>Make a toolbar.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the toolbar.</param>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="texts">An array of strings to show on the toolbar buttons.</param>
        /// <param name="images">An array of textures on the toolbar buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the toolbar buttons.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="content"></param>
        /// <returns>
        /// <para>The index of the selected button.</para>
        /// </returns>
        public static int Toolbar(Rect position, int selected, Texture[] images) => 
            Toolbar(position, selected, GUIContent.Temp(images), s_Skin.button);

        /// <summary>
        /// <para>Make a toolbar.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the toolbar.</param>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="texts">An array of strings to show on the toolbar buttons.</param>
        /// <param name="images">An array of textures on the toolbar buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the toolbar buttons.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="content"></param>
        /// <returns>
        /// <para>The index of the selected button.</para>
        /// </returns>
        public static int Toolbar(Rect position, int selected, string[] texts, GUIStyle style) => 
            Toolbar(position, selected, GUIContent.Temp(texts), style);

        /// <summary>
        /// <para>Make a toolbar.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the toolbar.</param>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="texts">An array of strings to show on the toolbar buttons.</param>
        /// <param name="images">An array of textures on the toolbar buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the toolbar buttons.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="content"></param>
        /// <returns>
        /// <para>The index of the selected button.</para>
        /// </returns>
        public static int Toolbar(Rect position, int selected, GUIContent[] contents, GUIStyle style)
        {
            GUIStyle style2;
            GUIStyle style3;
            GUIStyle style4;
            GUIUtility.CheckOnGUI();
            FindStyles(ref style, out style2, out style3, out style4, "left", "mid", "right");
            return DoButtonGrid(position, selected, contents, contents.Length, style, style2, style3, style4);
        }

        /// <summary>
        /// <para>Make a toolbar.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the toolbar.</param>
        /// <param name="selected">The index of the selected button.</param>
        /// <param name="texts">An array of strings to show on the toolbar buttons.</param>
        /// <param name="images">An array of textures on the toolbar buttons.</param>
        /// <param name="contents">An array of text, image and tooltips for the toolbar buttons.</param>
        /// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
        /// <param name="content"></param>
        /// <returns>
        /// <para>The index of the selected button.</para>
        /// </returns>
        public static int Toolbar(Rect position, int selected, Texture[] images, GUIStyle style) => 
            Toolbar(position, selected, GUIContent.Temp(images), style);

        /// <summary>
        /// <para>Remove focus from all windows.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void UnfocusWindow();
        /// <summary>
        /// <para>Make a vertical scrollbar. Scrollbars are what you use to scroll through a document. Most likely, you want to use scrollViews instead.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the scrollbar.</param>
        /// <param name="value">The position between min and max.</param>
        /// <param name="size">How much can we see?</param>
        /// <param name="topValue">The value at the top of the scrollbar.</param>
        /// <param name="bottomValue">The value at the bottom of the scrollbar.</param>
        /// <param name="style">The style to use for the scrollbar background. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The modified value. This can be changed by the user by dragging the scrollbar, or clicking the arrows at the end.</para>
        /// </returns>
        public static float VerticalScrollbar(Rect position, float value, float size, float topValue, float bottomValue) => 
            Scroller(position, value, size, topValue, bottomValue, skin.verticalScrollbar, skin.verticalScrollbarThumb, skin.verticalScrollbarUpButton, skin.verticalScrollbarDownButton, false);

        /// <summary>
        /// <para>Make a vertical scrollbar. Scrollbars are what you use to scroll through a document. Most likely, you want to use scrollViews instead.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the scrollbar.</param>
        /// <param name="value">The position between min and max.</param>
        /// <param name="size">How much can we see?</param>
        /// <param name="topValue">The value at the top of the scrollbar.</param>
        /// <param name="bottomValue">The value at the bottom of the scrollbar.</param>
        /// <param name="style">The style to use for the scrollbar background. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The modified value. This can be changed by the user by dragging the scrollbar, or clicking the arrows at the end.</para>
        /// </returns>
        public static float VerticalScrollbar(Rect position, float value, float size, float topValue, float bottomValue, GUIStyle style) => 
            Scroller(position, value, size, topValue, bottomValue, style, skin.GetStyle(style.name + "thumb"), skin.GetStyle(style.name + "upbutton"), skin.GetStyle(style.name + "downbutton"), false);

        /// <summary>
        /// <para>A vertical slider the user can drag to change a value between a min and a max.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the slider.</param>
        /// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="topValue">The value at the top end of the slider.</param>
        /// <param name="bottomValue">The value at the bottom end of the slider.</param>
        /// <param name="slider">The GUIStyle to use for displaying the dragging area. If left out, the horizontalSlider style from the current GUISkin is used.</param>
        /// <param name="thumb">The GUIStyle to use for displaying draggable thumb. If left out, the horizontalSliderThumb style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The value that has been set by the user.</para>
        /// </returns>
        public static float VerticalSlider(Rect position, float value, float topValue, float bottomValue) => 
            Slider(position, value, 0f, topValue, bottomValue, skin.verticalSlider, skin.verticalSliderThumb, false, 0);

        /// <summary>
        /// <para>A vertical slider the user can drag to change a value between a min and a max.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the slider.</param>
        /// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
        /// <param name="topValue">The value at the top end of the slider.</param>
        /// <param name="bottomValue">The value at the bottom end of the slider.</param>
        /// <param name="slider">The GUIStyle to use for displaying the dragging area. If left out, the horizontalSlider style from the current GUISkin is used.</param>
        /// <param name="thumb">The GUIStyle to use for displaying draggable thumb. If left out, the horizontalSliderThumb style from the current GUISkin is used.</param>
        /// <returns>
        /// <para>The value that has been set by the user.</para>
        /// </returns>
        public static float VerticalSlider(Rect position, float value, float topValue, float bottomValue, GUIStyle slider, GUIStyle thumb) => 
            Slider(position, value, 0f, topValue, bottomValue, slider, thumb, false, 0);

        public static Rect Window(int id, Rect clientRect, WindowFunction func, string text)
        {
            GUIUtility.CheckOnGUI();
            return DoWindow(id, clientRect, func, GUIContent.Temp(text), skin.window, skin, true);
        }

        public static Rect Window(int id, Rect clientRect, WindowFunction func, GUIContent content)
        {
            GUIUtility.CheckOnGUI();
            return DoWindow(id, clientRect, func, content, skin.window, skin, true);
        }

        public static Rect Window(int id, Rect clientRect, WindowFunction func, Texture image)
        {
            GUIUtility.CheckOnGUI();
            return DoWindow(id, clientRect, func, GUIContent.Temp(image), skin.window, skin, true);
        }

        public static Rect Window(int id, Rect clientRect, WindowFunction func, string text, GUIStyle style)
        {
            GUIUtility.CheckOnGUI();
            return DoWindow(id, clientRect, func, GUIContent.Temp(text), style, skin, true);
        }

        public static Rect Window(int id, Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style)
        {
            GUIUtility.CheckOnGUI();
            return DoWindow(id, clientRect, func, title, style, skin, true);
        }

        public static Rect Window(int id, Rect clientRect, WindowFunction func, Texture image, GUIStyle style)
        {
            GUIUtility.CheckOnGUI();
            return DoWindow(id, clientRect, func, GUIContent.Temp(image), style, skin, true);
        }

        /// <summary>
        /// <para>Global tinting color for all background elements rendered by the GUI.</para>
        /// </summary>
        public static Color backgroundColor
        {
            get
            {
                Color color;
                INTERNAL_get_backgroundColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_backgroundColor(ref value);
            }
        }

        internal static Material blendMaterial { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal static Material blitMaterial { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Returns true if any controls changed the value of the input data.</para>
        /// </summary>
        public static bool changed { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Global tinting color for the GUI.</para>
        /// </summary>
        public static Color color
        {
            get
            {
                Color color;
                INTERNAL_get_color(out color);
                return color;
            }
            set
            {
                INTERNAL_set_color(ref value);
            }
        }

        /// <summary>
        /// <para>Tinting color for all text rendered by the GUI.</para>
        /// </summary>
        public static Color contentColor
        {
            get
            {
                Color color;
                INTERNAL_get_contentColor(out color);
                return color;
            }
            set
            {
                INTERNAL_set_contentColor(ref value);
            }
        }

        /// <summary>
        /// <para>The sorting depth of the currently executing GUI behaviour.</para>
        /// </summary>
        public static int depth { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Is the GUI enabled?</para>
        /// </summary>
        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The GUI transform matrix.</para>
        /// </summary>
        public static Matrix4x4 matrix
        {
            get => 
                GUIClip.GetMatrix();
            set
            {
                GUIClip.SetMatrix(value);
            }
        }

        protected static string mouseTooltip =>
            Internal_GetMouseTooltip();

        internal static DateTime nextScrollStepTime
        {
            [CompilerGenerated]
            get => 
                <nextScrollStepTime>k__BackingField;
            [CompilerGenerated]
            set
            {
                <nextScrollStepTime>k__BackingField = value;
            }
        }

        internal static int scrollTroughSide
        {
            [CompilerGenerated]
            get => 
                <scrollTroughSide>k__BackingField;
            [CompilerGenerated]
            set
            {
                <scrollTroughSide>k__BackingField = value;
            }
        }

        /// <summary>
        /// <para>The global skin to use.</para>
        /// </summary>
        public static GUISkin skin
        {
            get
            {
                GUIUtility.CheckOnGUI();
                return s_Skin;
            }
            set
            {
                GUIUtility.CheckOnGUI();
                DoSetSkin(value);
            }
        }

        /// <summary>
        /// <para>The tooltip of the control the mouse is currently over, or which has keyboard focus. (Read Only).</para>
        /// </summary>
        public static string tooltip
        {
            get
            {
                string str = Internal_GetTooltip();
                if (str != null)
                {
                    return str;
                }
                return "";
            }
            set
            {
                Internal_SetTooltip(value);
            }
        }

        protected static Rect tooltipRect
        {
            get => 
                s_ToolTipRect;
            set
            {
                s_ToolTipRect = value;
            }
        }

        internal static bool usePageScrollbars { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public class ClipScope : GUI.Scope
        {
            public ClipScope(Rect position)
            {
                GUI.BeginClip(position);
            }

            protected override void CloseScope()
            {
                GUI.EndClip();
            }
        }

        /// <summary>
        /// <para>Disposable helper class for managing BeginGroup / EndGroup.</para>
        /// </summary>
        public class GroupScope : GUI.Scope
        {
            /// <summary>
            /// <para>Create a new GroupScope and begin the corresponding group.</para>
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the group.</param>
            /// <param name="text">Text to display on the group.</param>
            /// <param name="image">Texture to display on the group.</param>
            /// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
            /// <param name="style">The style to use for the background.</param>
            public GroupScope(Rect position)
            {
                GUI.BeginGroup(position);
            }

            /// <summary>
            /// <para>Create a new GroupScope and begin the corresponding group.</para>
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the group.</param>
            /// <param name="text">Text to display on the group.</param>
            /// <param name="image">Texture to display on the group.</param>
            /// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
            /// <param name="style">The style to use for the background.</param>
            public GroupScope(Rect position, string text)
            {
                GUI.BeginGroup(position, text);
            }

            /// <summary>
            /// <para>Create a new GroupScope and begin the corresponding group.</para>
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the group.</param>
            /// <param name="text">Text to display on the group.</param>
            /// <param name="image">Texture to display on the group.</param>
            /// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
            /// <param name="style">The style to use for the background.</param>
            public GroupScope(Rect position, GUIContent content)
            {
                GUI.BeginGroup(position, content);
            }

            /// <summary>
            /// <para>Create a new GroupScope and begin the corresponding group.</para>
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the group.</param>
            /// <param name="text">Text to display on the group.</param>
            /// <param name="image">Texture to display on the group.</param>
            /// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
            /// <param name="style">The style to use for the background.</param>
            public GroupScope(Rect position, GUIStyle style)
            {
                GUI.BeginGroup(position, style);
            }

            /// <summary>
            /// <para>Create a new GroupScope and begin the corresponding group.</para>
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the group.</param>
            /// <param name="text">Text to display on the group.</param>
            /// <param name="image">Texture to display on the group.</param>
            /// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
            /// <param name="style">The style to use for the background.</param>
            public GroupScope(Rect position, Texture image)
            {
                GUI.BeginGroup(position, image);
            }

            /// <summary>
            /// <para>Create a new GroupScope and begin the corresponding group.</para>
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the group.</param>
            /// <param name="text">Text to display on the group.</param>
            /// <param name="image">Texture to display on the group.</param>
            /// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
            /// <param name="style">The style to use for the background.</param>
            public GroupScope(Rect position, string text, GUIStyle style)
            {
                GUI.BeginGroup(position, text, style);
            }

            /// <summary>
            /// <para>Create a new GroupScope and begin the corresponding group.</para>
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the group.</param>
            /// <param name="text">Text to display on the group.</param>
            /// <param name="image">Texture to display on the group.</param>
            /// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
            /// <param name="style">The style to use for the background.</param>
            public GroupScope(Rect position, Texture image, GUIStyle style)
            {
                GUI.BeginGroup(position, image, style);
            }

            protected override void CloseScope()
            {
                GUI.EndGroup();
            }
        }

        public abstract class Scope : IDisposable
        {
            private bool m_Disposed;

            protected Scope()
            {
            }

            protected abstract void CloseScope();
            public void Dispose()
            {
                if (!this.m_Disposed)
                {
                    this.m_Disposed = true;
                    if (!GUIUtility.guiIsExiting)
                    {
                        this.CloseScope();
                    }
                }
            }

            ~Scope()
            {
                if (!this.m_Disposed)
                {
                    UnityEngine.Debug.LogError("Scope was not disposed! You should use the 'using' keyword or manually call Dispose.");
                }
            }
        }

        /// <summary>
        /// <para>Disposable helper class for managing BeginScrollView / EndScrollView.</para>
        /// </summary>
        public class ScrollViewScope : GUI.Scope
        {
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private bool <handleScrollWheel>k__BackingField;
            [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Vector2 <scrollPosition>k__BackingField;

            /// <summary>
            /// <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the ScrollView.</param>
            /// <param name="scrollPosition">The pixel distance that the view is scrolled in the X and Y directions.</param>
            /// <param name="viewRect">The rectangle used inside the scrollview.</param>
            /// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when clientRect is wider than position.</param>
            /// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when clientRect is taller than position.</param>
            /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
            /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
            public ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect);
            }

            /// <summary>
            /// <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the ScrollView.</param>
            /// <param name="scrollPosition">The pixel distance that the view is scrolled in the X and Y directions.</param>
            /// <param name="viewRect">The rectangle used inside the scrollview.</param>
            /// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when clientRect is wider than position.</param>
            /// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when clientRect is taller than position.</param>
            /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
            /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
            public ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical);
            }

            /// <summary>
            /// <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the ScrollView.</param>
            /// <param name="scrollPosition">The pixel distance that the view is scrolled in the X and Y directions.</param>
            /// <param name="viewRect">The rectangle used inside the scrollview.</param>
            /// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when clientRect is wider than position.</param>
            /// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when clientRect is taller than position.</param>
            /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
            /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
            public ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, horizontalScrollbar, verticalScrollbar);
            }

            /// <summary>
            /// <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the ScrollView.</param>
            /// <param name="scrollPosition">The pixel distance that the view is scrolled in the X and Y directions.</param>
            /// <param name="viewRect">The rectangle used inside the scrollview.</param>
            /// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when clientRect is wider than position.</param>
            /// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when clientRect is taller than position.</param>
            /// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
            /// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
            public ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar);
            }

            internal ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background)
            {
                this.handleScrollWheel = true;
                this.scrollPosition = GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background);
            }

            protected override void CloseScope()
            {
                GUI.EndScrollView(this.handleScrollWheel);
            }

            /// <summary>
            /// <para>Whether this ScrollView should handle scroll wheel events. (default: true).</para>
            /// </summary>
            public bool handleScrollWheel { get; set; }

            /// <summary>
            /// <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
            /// </summary>
            public Vector2 scrollPosition { get; private set; }
        }

        internal sealed class ScrollViewState
        {
            public bool apply = false;
            public bool hasScrollTo = false;
            public Rect position;
            public Vector2 scrollPosition;
            public Rect viewRect;
            public Rect visibleRect;

            internal Vector2 ScrollNeeded(Rect position)
            {
                Rect visibleRect = this.visibleRect;
                visibleRect.x += this.scrollPosition.x;
                visibleRect.y += this.scrollPosition.y;
                float num = position.width - this.visibleRect.width;
                if (num > 0f)
                {
                    position.width -= num;
                    position.x += num * 0.5f;
                }
                num = position.height - this.visibleRect.height;
                if (num > 0f)
                {
                    position.height -= num;
                    position.y += num * 0.5f;
                }
                Vector2 zero = Vector2.zero;
                if (position.xMax > visibleRect.xMax)
                {
                    zero.x += position.xMax - visibleRect.xMax;
                }
                else if (position.xMin < visibleRect.xMin)
                {
                    zero.x -= visibleRect.xMin - position.xMin;
                }
                if (position.yMax > visibleRect.yMax)
                {
                    zero.y += position.yMax - visibleRect.yMax;
                }
                else if (position.yMin < visibleRect.yMin)
                {
                    zero.y -= visibleRect.yMin - position.yMin;
                }
                Rect viewRect = this.viewRect;
                viewRect.width = Mathf.Max(viewRect.width, this.visibleRect.width);
                viewRect.height = Mathf.Max(viewRect.height, this.visibleRect.height);
                zero.x = Mathf.Clamp(zero.x, viewRect.xMin - this.scrollPosition.x, (viewRect.xMax - this.visibleRect.width) - this.scrollPosition.x);
                zero.y = Mathf.Clamp(zero.y, viewRect.yMin - this.scrollPosition.y, (viewRect.yMax - this.visibleRect.height) - this.scrollPosition.y);
                return zero;
            }

            internal void ScrollTo(Rect position)
            {
                this.ScrollTowards(position, float.PositiveInfinity);
            }

            internal bool ScrollTowards(Rect position, float maxDelta)
            {
                Vector2 vector = this.ScrollNeeded(position);
                if (vector.sqrMagnitude < 0.0001f)
                {
                    return false;
                }
                if (maxDelta != 0f)
                {
                    if (vector.magnitude > maxDelta)
                    {
                        vector = (Vector2) (vector.normalized * maxDelta);
                    }
                    this.scrollPosition += vector;
                    this.apply = true;
                }
                return true;
            }
        }

        /// <summary>
        /// <para>Callback to draw GUI within a window (used with GUI.Window).</para>
        /// </summary>
        /// <param name="id"></param>
        public delegate void WindowFunction(int id);
    }
}

