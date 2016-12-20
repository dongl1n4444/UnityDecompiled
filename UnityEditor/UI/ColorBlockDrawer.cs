namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// <para>PropertyDrawer for ColorBlock.</para>
    /// </summary>
    [CustomPropertyDrawer(typeof(ColorBlock), true)]
    public class ColorBlockDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return ((6f * EditorGUIUtility.singleLineHeight) + (5f * EditorGUIUtility.standardVerticalSpacing));
        }

        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            Rect position = rect;
            position.height = EditorGUIUtility.singleLineHeight;
            SerializedProperty property = prop.FindPropertyRelative("m_NormalColor");
            SerializedProperty property2 = prop.FindPropertyRelative("m_HighlightedColor");
            SerializedProperty property3 = prop.FindPropertyRelative("m_PressedColor");
            SerializedProperty property4 = prop.FindPropertyRelative("m_DisabledColor");
            SerializedProperty property5 = prop.FindPropertyRelative("m_ColorMultiplier");
            SerializedProperty property6 = prop.FindPropertyRelative("m_FadeDuration");
            EditorGUI.PropertyField(position, property);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property2);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property3);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property4);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property5);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property6);
        }
    }
}

