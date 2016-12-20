namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// <para>PropertyDrawer for FontData.</para>
    /// </summary>
    [CustomPropertyDrawer(typeof(FontData), true)]
    public class FontDataDrawer : PropertyDrawer
    {
        private const int kAlignmentButtonWidth = 20;
        private SerializedProperty m_AlignByGeometry;
        private float m_AlignByGeometryHeight = 0f;
        private SerializedProperty m_Alignment;
        private float m_EncodingHeight = 0f;
        private SerializedProperty m_Font;
        private float m_FontFieldfHeight = 0f;
        private SerializedProperty m_FontSize;
        private float m_FontSizeHeight = 0f;
        private SerializedProperty m_FontStyle;
        private float m_FontStyleHeight = 0f;
        private SerializedProperty m_HorizontalOverflow;
        private float m_HorizontalOverflowHeight = 0f;
        private SerializedProperty m_LineSpacing;
        private float m_LineSpacingHeight = 0f;
        private SerializedProperty m_ResizeTextForBestFit;
        private float m_ResizeTextForBestFitHeight = 0f;
        private SerializedProperty m_ResizeTextMaxSize;
        private float m_ResizeTextMaxSizeHeight = 0f;
        private SerializedProperty m_ResizeTextMinSize;
        private float m_ResizeTextMinSizeHeight = 0f;
        private SerializedProperty m_SupportEncoding;
        private SerializedProperty m_VerticalOverflow;
        private float m_VerticalOverflowHeight = 0f;
        private static int s_TextAlignmentHash = "DoTextAligmentControl".GetHashCode();

        private static void DoHorizontalAligmentControl(Rect position, SerializedProperty alignment)
        {
            HorizontalTextAligment horizontalAlignment = GetHorizontalAlignment((TextAnchor) alignment.intValue);
            bool flag = horizontalAlignment == HorizontalTextAligment.Left;
            bool flag2 = horizontalAlignment == HorizontalTextAligment.Center;
            bool flag3 = horizontalAlignment == HorizontalTextAligment.Right;
            if (alignment.hasMultipleDifferentValues)
            {
                foreach (UnityEngine.Object obj2 in alignment.serializedObject.targetObjects)
                {
                    Text text = obj2 as Text;
                    horizontalAlignment = GetHorizontalAlignment(text.alignment);
                    flag = flag || (horizontalAlignment == HorizontalTextAligment.Left);
                    flag2 = flag2 || (horizontalAlignment == HorizontalTextAligment.Center);
                    flag3 = flag3 || (horizontalAlignment == HorizontalTextAligment.Right);
                }
            }
            position.width = 20f;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, flag, !flag ? Styles.m_LeftAlignText : Styles.m_LeftAlignTextActive, Styles.alignmentButtonLeft);
            if (EditorGUI.EndChangeCheck())
            {
                SetHorizontalAlignment(alignment, HorizontalTextAligment.Left);
            }
            position.x += position.width;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, flag2, !flag2 ? Styles.m_CenterAlignText : Styles.m_CenterAlignTextActive, Styles.alignmentButtonMid);
            if (EditorGUI.EndChangeCheck())
            {
                SetHorizontalAlignment(alignment, HorizontalTextAligment.Center);
            }
            position.x += position.width;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, flag3, !flag3 ? Styles.m_RightAlignText : Styles.m_RightAlignTextActive, Styles.alignmentButtonRight);
            if (EditorGUI.EndChangeCheck())
            {
                SetHorizontalAlignment(alignment, HorizontalTextAligment.Right);
            }
        }

        private void DoTextAligmentControl(Rect position, SerializedProperty alignment)
        {
            GUIContent label = new GUIContent("Alignment");
            int id = GUIUtility.GetControlID(s_TextAlignmentHash, FocusType.Keyboard, position);
            EditorGUIUtility.SetIconSize(new Vector2(15f, 15f));
            EditorGUI.BeginProperty(position, label, alignment);
            Rect rect = EditorGUI.PrefixLabel(position, id, label);
            float width = 60f;
            float num3 = Mathf.Clamp((float) (rect.width - (width * 2f)), (float) 2f, (float) 10f);
            Rect rect2 = new Rect(rect.x, rect.y, width, rect.height);
            Rect rect3 = new Rect(rect2.xMax + num3, rect.y, width, rect.height);
            DoHorizontalAligmentControl(rect2, alignment);
            DoVerticalAligmentControl(rect3, alignment);
            EditorGUI.EndProperty();
            EditorGUIUtility.SetIconSize(Vector2.zero);
        }

        private static void DoVerticalAligmentControl(Rect position, SerializedProperty alignment)
        {
            VerticalTextAligment verticalAlignment = GetVerticalAlignment((TextAnchor) alignment.intValue);
            bool flag = verticalAlignment == VerticalTextAligment.Top;
            bool flag2 = verticalAlignment == VerticalTextAligment.Middle;
            bool flag3 = verticalAlignment == VerticalTextAligment.Bottom;
            if (alignment.hasMultipleDifferentValues)
            {
                foreach (UnityEngine.Object obj2 in alignment.serializedObject.targetObjects)
                {
                    Text text = obj2 as Text;
                    verticalAlignment = GetVerticalAlignment(text.alignment);
                    flag = flag || (verticalAlignment == VerticalTextAligment.Top);
                    flag2 = flag2 || (verticalAlignment == VerticalTextAligment.Middle);
                    flag3 = flag3 || (verticalAlignment == VerticalTextAligment.Bottom);
                }
            }
            position.width = 20f;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, flag, !flag ? Styles.m_TopAlignText : Styles.m_TopAlignTextActive, Styles.alignmentButtonLeft);
            if (EditorGUI.EndChangeCheck())
            {
                SetVerticalAlignment(alignment, VerticalTextAligment.Top);
            }
            position.x += position.width;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, flag2, !flag2 ? Styles.m_MiddleAlignText : Styles.m_MiddleAlignTextActive, Styles.alignmentButtonMid);
            if (EditorGUI.EndChangeCheck())
            {
                SetVerticalAlignment(alignment, VerticalTextAligment.Middle);
            }
            position.x += position.width;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, flag3, !flag3 ? Styles.m_BottomAlignText : Styles.m_BottomAlignTextActive, Styles.alignmentButtonRight);
            if (EditorGUI.EndChangeCheck())
            {
                SetVerticalAlignment(alignment, VerticalTextAligment.Bottom);
            }
        }

        private static bool EditorToggle(Rect position, bool value, GUIContent content, GUIStyle style)
        {
            int id = GUIUtility.GetControlID("AlignToggle".GetHashCode(), FocusType.Keyboard, position);
            Event current = Event.current;
            if (((GUIUtility.keyboardControl == id) && (current.type == EventType.KeyDown)) && (((current.keyCode == KeyCode.Space) || (current.keyCode == KeyCode.Return)) || (current.keyCode == KeyCode.KeypadEnter)))
            {
                value = !value;
                current.Use();
                GUI.changed = true;
            }
            if (((current.type == EventType.KeyDown) && (Event.current.button == 0)) && position.Contains(Event.current.mousePosition))
            {
                GUIUtility.keyboardControl = id;
                EditorGUIUtility.editingTextField = false;
                HandleUtility.Repaint();
            }
            return GUI.Toggle(position, id, value, content, style);
        }

        private static TextAnchor GetAnchor(VerticalTextAligment verticalTextAligment, HorizontalTextAligment horizontalTextAligment)
        {
            if (horizontalTextAligment != HorizontalTextAligment.Left)
            {
                if (horizontalTextAligment == HorizontalTextAligment.Center)
                {
                    if (verticalTextAligment != VerticalTextAligment.Bottom)
                    {
                        if (verticalTextAligment == VerticalTextAligment.Middle)
                        {
                            return TextAnchor.MiddleCenter;
                        }
                        return TextAnchor.UpperCenter;
                    }
                    return TextAnchor.LowerCenter;
                }
                if (verticalTextAligment != VerticalTextAligment.Bottom)
                {
                    if (verticalTextAligment == VerticalTextAligment.Middle)
                    {
                        return TextAnchor.MiddleRight;
                    }
                }
                else
                {
                    return TextAnchor.LowerRight;
                }
                return TextAnchor.UpperRight;
            }
            if (verticalTextAligment != VerticalTextAligment.Bottom)
            {
                if (verticalTextAligment == VerticalTextAligment.Middle)
                {
                    return TextAnchor.MiddleLeft;
                }
                return TextAnchor.UpperLeft;
            }
            return TextAnchor.LowerLeft;
        }

        private static HorizontalTextAligment GetHorizontalAlignment(TextAnchor ta)
        {
            switch (ta)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.MiddleLeft:
                case TextAnchor.LowerLeft:
                    return HorizontalTextAligment.Left;

                case TextAnchor.UpperCenter:
                case TextAnchor.MiddleCenter:
                case TextAnchor.LowerCenter:
                    return HorizontalTextAligment.Center;

                case TextAnchor.UpperRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.LowerRight:
                    return HorizontalTextAligment.Right;
            }
            return HorizontalTextAligment.Left;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            this.Init(property);
            this.m_FontFieldfHeight = EditorGUI.GetPropertyHeight(this.m_Font);
            this.m_FontStyleHeight = EditorGUI.GetPropertyHeight(this.m_FontStyle);
            this.m_FontSizeHeight = EditorGUI.GetPropertyHeight(this.m_FontSize);
            this.m_LineSpacingHeight = EditorGUI.GetPropertyHeight(this.m_LineSpacing);
            this.m_EncodingHeight = EditorGUI.GetPropertyHeight(this.m_SupportEncoding);
            this.m_ResizeTextForBestFitHeight = EditorGUI.GetPropertyHeight(this.m_ResizeTextForBestFit);
            this.m_ResizeTextMinSizeHeight = EditorGUI.GetPropertyHeight(this.m_ResizeTextMinSize);
            this.m_ResizeTextMaxSizeHeight = EditorGUI.GetPropertyHeight(this.m_ResizeTextMaxSize);
            this.m_HorizontalOverflowHeight = EditorGUI.GetPropertyHeight(this.m_HorizontalOverflow);
            this.m_VerticalOverflowHeight = EditorGUI.GetPropertyHeight(this.m_VerticalOverflow);
            this.m_AlignByGeometryHeight = EditorGUI.GetPropertyHeight(this.m_AlignByGeometry);
            float num = (((((((((this.m_FontFieldfHeight + this.m_FontStyleHeight) + this.m_FontSizeHeight) + this.m_LineSpacingHeight) + this.m_EncodingHeight) + this.m_ResizeTextForBestFitHeight) + this.m_HorizontalOverflowHeight) + this.m_VerticalOverflowHeight) + (EditorGUIUtility.singleLineHeight * 3f)) + (EditorGUIUtility.standardVerticalSpacing * 10f)) + this.m_AlignByGeometryHeight;
            if (this.m_ResizeTextForBestFit.boolValue)
            {
                num += (this.m_ResizeTextMinSizeHeight + this.m_ResizeTextMaxSizeHeight) + (EditorGUIUtility.standardVerticalSpacing * 2f);
            }
            return num;
        }

        private static VerticalTextAligment GetVerticalAlignment(TextAnchor ta)
        {
            switch (ta)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.UpperCenter:
                case TextAnchor.UpperRight:
                    return VerticalTextAligment.Top;

                case TextAnchor.MiddleLeft:
                case TextAnchor.MiddleCenter:
                case TextAnchor.MiddleRight:
                    return VerticalTextAligment.Middle;

                case TextAnchor.LowerLeft:
                case TextAnchor.LowerCenter:
                case TextAnchor.LowerRight:
                    return VerticalTextAligment.Bottom;
            }
            return VerticalTextAligment.Top;
        }

        /// <summary>
        /// <para>Initialize the serialized properties for the drawer.</para>
        /// </summary>
        /// <param name="property"></param>
        protected void Init(SerializedProperty property)
        {
            this.m_SupportEncoding = property.FindPropertyRelative("m_RichText");
            this.m_Font = property.FindPropertyRelative("m_Font");
            this.m_FontSize = property.FindPropertyRelative("m_FontSize");
            this.m_LineSpacing = property.FindPropertyRelative("m_LineSpacing");
            this.m_FontStyle = property.FindPropertyRelative("m_FontStyle");
            this.m_ResizeTextForBestFit = property.FindPropertyRelative("m_BestFit");
            this.m_ResizeTextMinSize = property.FindPropertyRelative("m_MinSize");
            this.m_ResizeTextMaxSize = property.FindPropertyRelative("m_MaxSize");
            this.m_HorizontalOverflow = property.FindPropertyRelative("m_HorizontalOverflow");
            this.m_VerticalOverflow = property.FindPropertyRelative("m_VerticalOverflow");
            this.m_Alignment = property.FindPropertyRelative("m_Alignment");
            this.m_AlignByGeometry = property.FindPropertyRelative("m_AlignByGeometry");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            this.Init(property);
            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect, "Character", EditorStyles.boldLabel);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.indentLevel++;
            Font objectReferenceValue = this.m_Font.objectReferenceValue as Font;
            rect.height = this.m_FontFieldfHeight;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, this.m_Font);
            if (EditorGUI.EndChangeCheck())
            {
                objectReferenceValue = this.m_Font.objectReferenceValue as Font;
                if ((objectReferenceValue != null) && !objectReferenceValue.dynamic)
                {
                    this.m_FontSize.intValue = objectReferenceValue.fontSize;
                }
            }
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = this.m_FontStyleHeight;
            using (new EditorGUI.DisabledScope((!this.m_Font.hasMultipleDifferentValues && (objectReferenceValue != null)) && !objectReferenceValue.dynamic))
            {
                EditorGUI.PropertyField(rect, this.m_FontStyle);
            }
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = this.m_FontSizeHeight;
            EditorGUI.PropertyField(rect, this.m_FontSize);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = this.m_LineSpacingHeight;
            EditorGUI.PropertyField(rect, this.m_LineSpacing);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = this.m_EncodingHeight;
            EditorGUI.PropertyField(rect, this.m_SupportEncoding, Styles.m_EncodingContent);
            EditorGUI.indentLevel--;
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect, "Paragraph", EditorStyles.boldLabel);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.indentLevel++;
            rect.height = EditorGUIUtility.singleLineHeight;
            this.DoTextAligmentControl(rect, this.m_Alignment);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = this.m_HorizontalOverflowHeight;
            EditorGUI.PropertyField(rect, this.m_AlignByGeometry);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = this.m_HorizontalOverflowHeight;
            EditorGUI.PropertyField(rect, this.m_HorizontalOverflow);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = this.m_VerticalOverflowHeight;
            EditorGUI.PropertyField(rect, this.m_VerticalOverflow);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = this.m_ResizeTextMaxSizeHeight;
            EditorGUI.PropertyField(rect, this.m_ResizeTextForBestFit);
            if (this.m_ResizeTextForBestFit.boolValue)
            {
                EditorGUILayout.EndFadeGroup();
                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                rect.height = this.m_ResizeTextMinSizeHeight;
                EditorGUI.PropertyField(rect, this.m_ResizeTextMinSize);
                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                rect.height = this.m_ResizeTextMaxSizeHeight;
                EditorGUI.PropertyField(rect, this.m_ResizeTextMaxSize);
            }
            EditorGUI.indentLevel--;
        }

        private static void SetHorizontalAlignment(SerializedProperty alignment, HorizontalTextAligment horizontalAlignment)
        {
            foreach (UnityEngine.Object obj2 in alignment.serializedObject.targetObjects)
            {
                Text objectToUndo = obj2 as Text;
                VerticalTextAligment verticalAlignment = GetVerticalAlignment(objectToUndo.alignment);
                Undo.RecordObject(objectToUndo, "Horizontal Alignment");
                objectToUndo.alignment = GetAnchor(verticalAlignment, horizontalAlignment);
                EditorUtility.SetDirty(obj2);
            }
        }

        private static void SetVerticalAlignment(SerializedProperty alignment, VerticalTextAligment verticalAlignment)
        {
            foreach (UnityEngine.Object obj2 in alignment.serializedObject.targetObjects)
            {
                Text objectToUndo = obj2 as Text;
                HorizontalTextAligment horizontalAlignment = GetHorizontalAlignment(objectToUndo.alignment);
                Undo.RecordObject(objectToUndo, "Vertical Alignment");
                objectToUndo.alignment = GetAnchor(verticalAlignment, horizontalAlignment);
                EditorUtility.SetDirty(obj2);
            }
        }

        private enum HorizontalTextAligment
        {
            Left,
            Center,
            Right
        }

        private static class Styles
        {
            public static GUIStyle alignmentButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
            public static GUIStyle alignmentButtonMid = new GUIStyle(EditorStyles.miniButtonMid);
            public static GUIStyle alignmentButtonRight = new GUIStyle(EditorStyles.miniButtonRight);
            public static GUIContent m_BottomAlignText = EditorGUIUtility.IconContent("GUISystem/align_vertically_bottom", "Bottom Align");
            public static GUIContent m_BottomAlignTextActive = EditorGUIUtility.IconContent("GUISystem/align_vertically_bottom_active", "Bottom Align");
            public static GUIContent m_CenterAlignText = EditorGUIUtility.IconContent("GUISystem/align_horizontally_center", "Center Align");
            public static GUIContent m_CenterAlignTextActive = EditorGUIUtility.IconContent("GUISystem/align_horizontally_center_active", "Center Align");
            public static GUIContent m_EncodingContent = new GUIContent("Rich Text", "Use emoticons and colors");
            public static GUIContent m_LeftAlignText = EditorGUIUtility.IconContent("GUISystem/align_horizontally_left", "Left Align");
            public static GUIContent m_LeftAlignTextActive = EditorGUIUtility.IconContent("GUISystem/align_horizontally_left_active", "Left Align");
            public static GUIContent m_MiddleAlignText = EditorGUIUtility.IconContent("GUISystem/align_vertically_center", "Middle Align");
            public static GUIContent m_MiddleAlignTextActive = EditorGUIUtility.IconContent("GUISystem/align_vertically_center_active", "Middle Align");
            public static GUIContent m_RightAlignText = EditorGUIUtility.IconContent("GUISystem/align_horizontally_right", "Right Align");
            public static GUIContent m_RightAlignTextActive = EditorGUIUtility.IconContent("GUISystem/align_horizontally_right_active", "Right Align");
            public static GUIContent m_TopAlignText = EditorGUIUtility.IconContent("GUISystem/align_vertically_top", "Top Align");
            public static GUIContent m_TopAlignTextActive = EditorGUIUtility.IconContent("GUISystem/align_vertically_top_active", "Top Align");

            static Styles()
            {
                GUIStyle[] styles = new GUIStyle[] { alignmentButtonLeft, alignmentButtonMid, alignmentButtonRight };
                FixAlignmentButtonStyles(styles);
            }

            private static void FixAlignmentButtonStyles(params GUIStyle[] styles)
            {
                foreach (GUIStyle style in styles)
                {
                    style.padding.left = 2;
                    style.padding.right = 2;
                }
            }
        }

        private enum VerticalTextAligment
        {
            Top,
            Middle,
            Bottom
        }
    }
}

