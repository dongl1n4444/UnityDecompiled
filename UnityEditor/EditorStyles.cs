namespace UnityEditor
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Common GUIStyles used for EditorGUI controls.</para>
    /// </summary>
    public sealed class EditorStyles
    {
        private GUIStyle m_AssetLabel;
        private GUIStyle m_AssetLabelIcon;
        private GUIStyle m_AssetLabelPartial;
        internal Font m_BoldFont;
        private GUIStyle m_BoldLabel;
        private GUIStyle m_CenteredGreyMiniLabel;
        private GUIStyle m_ColorField;
        private GUIStyle m_ColorPickerBox;
        private GUIStyle m_DropDownList;
        private GUIStyle m_Foldout;
        private GUIStyle m_FoldoutPreDrop;
        private GUIStyle m_FoldoutSelected;
        private GUIStyle m_HelpBox;
        private GUIStyle m_IconButton;
        private GUIStyle m_InspectorBig;
        private GUIStyle m_InspectorDefaultMargins;
        private GUIStyle m_InspectorFullWidthMargins;
        private GUIStyle m_InspectorTitlebar;
        private GUIStyle m_InspectorTitlebarText;
        private Vector2 m_KnobSize = new Vector2(40f, 40f);
        internal GUIStyle m_Label;
        private GUIStyle m_LargeLabel;
        private GUIStyle m_LayerMaskField;
        private GUIStyle m_LinkLabel;
        internal Font m_MiniBoldFont;
        private GUIStyle m_MiniBoldLabel;
        private GUIStyle m_MiniButton;
        private GUIStyle m_MiniButtonLeft;
        private GUIStyle m_MiniButtonMid;
        private GUIStyle m_MiniButtonRight;
        internal Font m_MiniFont;
        private Vector2 m_MiniKnobSize = new Vector2(29f, 29f);
        private GUIStyle m_MiniLabel;
        private GUIStyle m_MiniTextField;
        private GUIStyle m_MinMaxHorizontalSliderThumb;
        private GUIStyle m_NotificationBackground;
        private GUIStyle m_NotificationText;
        private GUIStyle m_NumberField;
        private GUIStyle m_ObjectField;
        private GUIStyle m_ObjectFieldMiniThumb;
        private GUIStyle m_ObjectFieldThumb;
        private GUIStyle m_Popup;
        private GUIStyle m_ProgressBarBack;
        private GUIStyle m_ProgressBarBar;
        private GUIStyle m_ProgressBarText;
        private GUIStyle m_RadioButton;
        private GUIStyle m_SearchField;
        private GUIStyle m_SearchFieldCancelButton;
        private GUIStyle m_SearchFieldCancelButtonEmpty;
        private GUIStyle m_SelectionRect;
        internal Font m_StandardFont;
        internal GUIStyle m_TextArea;
        internal GUIStyle m_TextField;
        private GUIStyle m_TextFieldDropDown;
        private GUIStyle m_TextFieldDropDownText;
        private GUIStyle m_Toggle;
        private GUIStyle m_ToggleGroup;
        private GUIStyle m_ToggleMixed;
        private GUIStyle m_Toolbar;
        private GUIStyle m_ToolbarButton;
        private GUIStyle m_ToolbarDropDown;
        private GUIStyle m_ToolbarPopup;
        private GUIStyle m_ToolbarSearchField;
        private GUIStyle m_ToolbarSearchFieldCancelButton;
        private GUIStyle m_ToolbarSearchFieldCancelButtonEmpty;
        private GUIStyle m_ToolbarSearchFieldPopup;
        private GUIStyle m_ToolbarTextField;
        private GUIStyle m_Tooltip;
        private GUIStyle m_WhiteBoldLabel;
        private GUIStyle m_WhiteLabel;
        private GUIStyle m_WhiteLargeLabel;
        private GUIStyle m_WhiteMiniLabel;
        private GUIStyle m_WordWrappedLabel;
        private GUIStyle m_WordWrappedMiniLabel;
        private static EditorStyles[] s_CachedStyles = new EditorStyles[2];
        internal static EditorStyles s_Current;

        private GUIStyle GetStyle(string styleName)
        {
            GUIStyle error = GUI.skin.FindStyle(styleName);
            if (error == null)
            {
                error = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
            }
            if (error == null)
            {
                Debug.LogError("Missing built-in guistyle " + styleName);
                error = GUISkin.error;
            }
            return error;
        }

        private void InitSharedStyles()
        {
            this.m_ColorPickerBox = this.GetStyle("ColorPickerBox");
            this.m_InspectorBig = this.GetStyle("In BigTitle");
            this.m_MiniLabel = this.GetStyle("miniLabel");
            this.m_LargeLabel = this.GetStyle("LargeLabel");
            this.m_BoldLabel = this.GetStyle("BoldLabel");
            this.m_MiniBoldLabel = this.GetStyle("MiniBoldLabel");
            this.m_WordWrappedLabel = this.GetStyle("WordWrappedLabel");
            this.m_WordWrappedMiniLabel = this.GetStyle("WordWrappedMiniLabel");
            this.m_WhiteLabel = this.GetStyle("WhiteLabel");
            this.m_WhiteMiniLabel = this.GetStyle("WhiteMiniLabel");
            this.m_WhiteLargeLabel = this.GetStyle("WhiteLargeLabel");
            this.m_WhiteBoldLabel = this.GetStyle("WhiteBoldLabel");
            this.m_MiniTextField = this.GetStyle("MiniTextField");
            this.m_RadioButton = this.GetStyle("Radio");
            this.m_MiniButton = this.GetStyle("miniButton");
            this.m_MiniButtonLeft = this.GetStyle("miniButtonLeft");
            this.m_MiniButtonMid = this.GetStyle("miniButtonMid");
            this.m_MiniButtonRight = this.GetStyle("miniButtonRight");
            this.m_Toolbar = this.GetStyle("toolbar");
            this.m_ToolbarButton = this.GetStyle("toolbarbutton");
            this.m_ToolbarPopup = this.GetStyle("toolbarPopup");
            this.m_ToolbarDropDown = this.GetStyle("toolbarDropDown");
            this.m_ToolbarTextField = this.GetStyle("toolbarTextField");
            this.m_ToolbarSearchField = this.GetStyle("ToolbarSeachTextField");
            this.m_ToolbarSearchFieldPopup = this.GetStyle("ToolbarSeachTextFieldPopup");
            this.m_ToolbarSearchFieldCancelButton = this.GetStyle("ToolbarSeachCancelButton");
            this.m_ToolbarSearchFieldCancelButtonEmpty = this.GetStyle("ToolbarSeachCancelButtonEmpty");
            this.m_SearchField = this.GetStyle("SearchTextField");
            this.m_SearchFieldCancelButton = this.GetStyle("SearchCancelButton");
            this.m_SearchFieldCancelButtonEmpty = this.GetStyle("SearchCancelButtonEmpty");
            this.m_HelpBox = this.GetStyle("HelpBox");
            this.m_AssetLabel = this.GetStyle("AssetLabel");
            this.m_AssetLabelPartial = this.GetStyle("AssetLabel Partial");
            this.m_AssetLabelIcon = this.GetStyle("AssetLabel Icon");
            this.m_SelectionRect = this.GetStyle("selectionRect");
            this.m_MinMaxHorizontalSliderThumb = this.GetStyle("MinMaxHorizontalSliderThumb");
            this.m_DropDownList = this.GetStyle("DropDownButton");
            this.m_BoldFont = this.GetStyle("BoldLabel").font;
            this.m_StandardFont = this.GetStyle("Label").font;
            this.m_MiniFont = this.GetStyle("MiniLabel").font;
            this.m_MiniBoldFont = this.GetStyle("MiniBoldLabel").font;
            this.m_ProgressBarBack = this.GetStyle("ProgressBarBack");
            this.m_ProgressBarBar = this.GetStyle("ProgressBarBar");
            this.m_ProgressBarText = this.GetStyle("ProgressBarText");
            this.m_FoldoutPreDrop = this.GetStyle("FoldoutPreDrop");
            this.m_InspectorTitlebar = this.GetStyle("IN Title");
            this.m_InspectorTitlebarText = this.GetStyle("IN TitleText");
            this.m_ToggleGroup = this.GetStyle("BoldToggle");
            this.m_Tooltip = this.GetStyle("Tooltip");
            this.m_NotificationText = this.GetStyle("NotificationText");
            this.m_NotificationBackground = this.GetStyle("NotificationBackground");
            this.m_Popup = this.m_LayerMaskField = this.GetStyle("MiniPopup");
            this.m_TextField = this.m_NumberField = this.GetStyle("textField");
            this.m_Label = this.GetStyle("ControlLabel");
            this.m_ObjectField = this.GetStyle("ObjectField");
            this.m_ObjectFieldThumb = this.GetStyle("ObjectFieldThumb");
            this.m_ObjectFieldMiniThumb = this.GetStyle("ObjectFieldMiniThumb");
            this.m_Toggle = this.GetStyle("Toggle");
            this.m_ToggleMixed = this.GetStyle("ToggleMixed");
            this.m_ColorField = this.GetStyle("ColorField");
            this.m_Foldout = this.GetStyle("Foldout");
            this.m_FoldoutSelected = GUIStyle.none;
            this.m_IconButton = this.GetStyle("IconButton");
            this.m_TextFieldDropDown = this.GetStyle("TextFieldDropDown");
            this.m_TextFieldDropDownText = this.GetStyle("TextFieldDropDownText");
            this.m_LinkLabel = new GUIStyle(this.m_Label);
            this.m_LinkLabel.normal.textColor = new Color(0.25f, 0.5f, 0.9f, 1f);
            this.m_LinkLabel.stretchWidth = false;
            this.m_TextArea = new GUIStyle(this.m_TextField);
            this.m_TextArea.wordWrap = true;
            this.m_InspectorDefaultMargins = new GUIStyle();
            this.m_InspectorDefaultMargins.padding = new RectOffset(14, 4, 0, 0);
            this.m_InspectorFullWidthMargins = new GUIStyle();
            this.m_InspectorFullWidthMargins.padding = new RectOffset(5, 4, 0, 0);
            this.m_CenteredGreyMiniLabel = new GUIStyle(this.m_MiniLabel);
            this.m_CenteredGreyMiniLabel.alignment = TextAnchor.MiddleCenter;
            this.m_CenteredGreyMiniLabel.normal.textColor = Color.grey;
        }

        internal static void UpdateSkinCache()
        {
            UpdateSkinCache(EditorGUIUtility.skinIndex);
        }

        internal static void UpdateSkinCache(int skinIndex)
        {
            if (GUIUtility.s_SkinMode != 0)
            {
                if (s_CachedStyles[skinIndex] == null)
                {
                    s_CachedStyles[skinIndex] = new EditorStyles();
                    s_CachedStyles[skinIndex].InitSharedStyles();
                }
                s_Current = s_CachedStyles[skinIndex];
                EditorGUIUtility.s_FontIsBold = -1;
                EditorGUIUtility.SetBoldDefaultFont(false);
            }
        }

        internal static GUIStyle assetLabel =>
            s_Current.m_AssetLabel;

        internal static GUIStyle assetLabelIcon =>
            s_Current.m_AssetLabelIcon;

        internal static GUIStyle assetLabelPartial =>
            s_Current.m_AssetLabelPartial;

        /// <summary>
        /// <para>Bold font.</para>
        /// </summary>
        public static Font boldFont =>
            s_Current.m_BoldFont;

        /// <summary>
        /// <para>Style for bold label.</para>
        /// </summary>
        public static GUIStyle boldLabel =>
            s_Current.m_BoldLabel;

        /// <summary>
        /// <para>Style for label with small font which is centered and grey.</para>
        /// </summary>
        public static GUIStyle centeredGreyMiniLabel =>
            s_Current.m_CenteredGreyMiniLabel;

        /// <summary>
        /// <para>Style used for headings for Color fields.</para>
        /// </summary>
        public static GUIStyle colorField =>
            s_Current.m_ColorField;

        internal static GUIStyle colorPickerBox =>
            s_Current.m_ColorPickerBox;

        internal static GUIStyle dropDownList =>
            s_Current.m_DropDownList;

        /// <summary>
        /// <para>Style used for headings for EditorGUI.Foldout.</para>
        /// </summary>
        public static GUIStyle foldout =>
            s_Current.m_Foldout;

        /// <summary>
        /// <para>Style used for headings for EditorGUI.Foldout.</para>
        /// </summary>
        public static GUIStyle foldoutPreDrop =>
            s_Current.m_FoldoutPreDrop;

        internal static GUIStyle foldoutSelected =>
            s_Current.m_FoldoutSelected;

        /// <summary>
        /// <para>Style used for background box for EditorGUI.HelpBox.</para>
        /// </summary>
        public static GUIStyle helpBox =>
            s_Current.m_HelpBox;

        internal static GUIStyle iconButton =>
            s_Current.m_IconButton;

        internal static GUIStyle inspectorBig =>
            s_Current.m_InspectorBig;

        /// <summary>
        /// <para>Wrap content in a vertical group with this style to get the default margins used in the Inspector.</para>
        /// </summary>
        public static GUIStyle inspectorDefaultMargins =>
            s_Current.m_InspectorDefaultMargins;

        /// <summary>
        /// <para>Wrap content in a vertical group with this style to get full width margins in the Inspector.</para>
        /// </summary>
        public static GUIStyle inspectorFullWidthMargins =>
            s_Current.m_InspectorFullWidthMargins;

        internal static GUIStyle inspectorTitlebar =>
            s_Current.m_InspectorTitlebar;

        internal static GUIStyle inspectorTitlebarText =>
            s_Current.m_InspectorTitlebarText;

        internal static Vector2 knobSize =>
            s_Current.m_KnobSize;

        /// <summary>
        /// <para>Style used for the labelled on all EditorGUI overloads that take a prefix label.</para>
        /// </summary>
        public static GUIStyle label =>
            s_Current.m_Label;

        /// <summary>
        /// <para>Style for label with large font.</para>
        /// </summary>
        public static GUIStyle largeLabel =>
            s_Current.m_LargeLabel;

        /// <summary>
        /// <para>Style used for headings for Layer masks.</para>
        /// </summary>
        public static GUIStyle layerMaskField =>
            s_Current.m_LayerMaskField;

        internal static GUIStyle linkLabel =>
            s_Current.m_LinkLabel;

        /// <summary>
        /// <para>Mini Bold font.</para>
        /// </summary>
        public static Font miniBoldFont =>
            s_Current.m_MiniBoldFont;

        /// <summary>
        /// <para>Style for mini bold label.</para>
        /// </summary>
        public static GUIStyle miniBoldLabel =>
            s_Current.m_MiniBoldLabel;

        /// <summary>
        /// <para>Style used for a standalone small button.</para>
        /// </summary>
        public static GUIStyle miniButton =>
            s_Current.m_MiniButton;

        /// <summary>
        /// <para>Style used for the leftmost button in a horizontal button group.</para>
        /// </summary>
        public static GUIStyle miniButtonLeft =>
            s_Current.m_MiniButtonLeft;

        /// <summary>
        /// <para>Style used for the middle buttons in a horizontal group.</para>
        /// </summary>
        public static GUIStyle miniButtonMid =>
            s_Current.m_MiniButtonMid;

        /// <summary>
        /// <para>Style used for the rightmost button in a horizontal group.</para>
        /// </summary>
        public static GUIStyle miniButtonRight =>
            s_Current.m_MiniButtonRight;

        /// <summary>
        /// <para>Mini font.</para>
        /// </summary>
        public static Font miniFont =>
            s_Current.m_MiniFont;

        internal static Vector2 miniKnobSize =>
            s_Current.m_MiniKnobSize;

        /// <summary>
        /// <para>Style for label with small font.</para>
        /// </summary>
        public static GUIStyle miniLabel =>
            s_Current.m_MiniLabel;

        /// <summary>
        /// <para>Smaller text field.</para>
        /// </summary>
        public static GUIStyle miniTextField =>
            s_Current.m_MiniTextField;

        internal static GUIStyle minMaxHorizontalSliderThumb =>
            s_Current.m_MinMaxHorizontalSliderThumb;

        internal static GUIStyle notificationBackground =>
            s_Current.m_NotificationBackground;

        internal static GUIStyle notificationText =>
            s_Current.m_NotificationText;

        /// <summary>
        /// <para>Style used for field editors for numbers.</para>
        /// </summary>
        public static GUIStyle numberField =>
            s_Current.m_NumberField;

        /// <summary>
        /// <para>Style used for headings for object fields.</para>
        /// </summary>
        public static GUIStyle objectField =>
            s_Current.m_ObjectField;

        /// <summary>
        /// <para>Style used for object fields that have a thumbnail (e.g Textures). </para>
        /// </summary>
        public static GUIStyle objectFieldMiniThumb =>
            s_Current.m_ObjectFieldMiniThumb;

        /// <summary>
        /// <para>Style used for headings for the Select button in object fields.</para>
        /// </summary>
        public static GUIStyle objectFieldThumb =>
            s_Current.m_ObjectFieldThumb;

        /// <summary>
        /// <para>Style used for EditorGUI.Popup, EditorGUI.EnumPopup,.</para>
        /// </summary>
        public static GUIStyle popup =>
            s_Current.m_Popup;

        internal static GUIStyle progressBarBack =>
            s_Current.m_ProgressBarBack;

        internal static GUIStyle progressBarBar =>
            s_Current.m_ProgressBarBar;

        internal static GUIStyle progressBarText =>
            s_Current.m_ProgressBarText;

        /// <summary>
        /// <para>Style used for a radio button.</para>
        /// </summary>
        public static GUIStyle radioButton =>
            s_Current.m_RadioButton;

        internal static GUIStyle searchField =>
            s_Current.m_SearchField;

        internal static GUIStyle searchFieldCancelButton =>
            s_Current.m_SearchFieldCancelButton;

        internal static GUIStyle searchFieldCancelButtonEmpty =>
            s_Current.m_SearchFieldCancelButtonEmpty;

        internal static GUIStyle selectionRect =>
            s_Current.m_SelectionRect;

        /// <summary>
        /// <para>Standard font.</para>
        /// </summary>
        public static Font standardFont =>
            s_Current.m_StandardFont;

        [Obsolete("structHeadingLabel is deprecated, use EditorStyles.label instead.")]
        public static GUIStyle structHeadingLabel =>
            s_Current.m_Label;

        /// <summary>
        /// <para>Style used for EditorGUI.TextArea.</para>
        /// </summary>
        public static GUIStyle textArea =>
            s_Current.m_TextArea;

        /// <summary>
        /// <para>Style used for EditorGUI.TextField.</para>
        /// </summary>
        public static GUIStyle textField =>
            s_Current.m_TextField;

        internal static GUIStyle textFieldDropDown =>
            s_Current.m_TextFieldDropDown;

        internal static GUIStyle textFieldDropDownText =>
            s_Current.m_TextFieldDropDownText;

        /// <summary>
        /// <para>Style used for headings for EditorGUI.Toggle.</para>
        /// </summary>
        public static GUIStyle toggle =>
            s_Current.m_Toggle;

        /// <summary>
        /// <para>Style used for headings for EditorGUILayout.BeginToggleGroup.</para>
        /// </summary>
        public static GUIStyle toggleGroup =>
            s_Current.m_ToggleGroup;

        internal static GUIStyle toggleMixed =>
            s_Current.m_ToggleMixed;

        /// <summary>
        /// <para>Toolbar background from top of windows.</para>
        /// </summary>
        public static GUIStyle toolbar =>
            s_Current.m_Toolbar;

        /// <summary>
        /// <para>Style for Button and Toggles in toolbars.</para>
        /// </summary>
        public static GUIStyle toolbarButton =>
            s_Current.m_ToolbarButton;

        /// <summary>
        /// <para>Toolbar Dropdown.</para>
        /// </summary>
        public static GUIStyle toolbarDropDown =>
            s_Current.m_ToolbarDropDown;

        /// <summary>
        /// <para>Toolbar Popup.</para>
        /// </summary>
        public static GUIStyle toolbarPopup =>
            s_Current.m_ToolbarPopup;

        internal static GUIStyle toolbarSearchField =>
            s_Current.m_ToolbarSearchField;

        internal static GUIStyle toolbarSearchFieldCancelButton =>
            s_Current.m_ToolbarSearchFieldCancelButton;

        internal static GUIStyle toolbarSearchFieldCancelButtonEmpty =>
            s_Current.m_ToolbarSearchFieldCancelButtonEmpty;

        internal static GUIStyle toolbarSearchFieldPopup =>
            s_Current.m_ToolbarSearchFieldPopup;

        /// <summary>
        /// <para>Toolbar text field.</para>
        /// </summary>
        public static GUIStyle toolbarTextField =>
            s_Current.m_ToolbarTextField;

        internal static GUIStyle tooltip =>
            s_Current.m_Tooltip;

        /// <summary>
        /// <para>Style for white bold label.</para>
        /// </summary>
        public static GUIStyle whiteBoldLabel =>
            s_Current.m_WhiteBoldLabel;

        /// <summary>
        /// <para>Style for white label.</para>
        /// </summary>
        public static GUIStyle whiteLabel =>
            s_Current.m_WhiteLabel;

        /// <summary>
        /// <para>Style for white large label.</para>
        /// </summary>
        public static GUIStyle whiteLargeLabel =>
            s_Current.m_WhiteLargeLabel;

        /// <summary>
        /// <para>Style for white mini label.</para>
        /// </summary>
        public static GUIStyle whiteMiniLabel =>
            s_Current.m_WhiteMiniLabel;

        /// <summary>
        /// <para>Style for word wrapped label.</para>
        /// </summary>
        public static GUIStyle wordWrappedLabel =>
            s_Current.m_WordWrappedLabel;

        /// <summary>
        /// <para>Style for word wrapped mini label.</para>
        /// </summary>
        public static GUIStyle wordWrappedMiniLabel =>
            s_Current.m_WordWrappedMiniLabel;
    }
}

