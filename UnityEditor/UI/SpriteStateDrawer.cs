namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// <para>PropertyDrawer for SpriteState.</para>
    /// </summary>
    [CustomPropertyDrawer(typeof(SpriteState), true)]
    public class SpriteStateDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) => 
            ((3f * EditorGUIUtility.singleLineHeight) + (2f * EditorGUIUtility.standardVerticalSpacing));

        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            Rect position = rect;
            position.height = EditorGUIUtility.singleLineHeight;
            SerializedProperty property = prop.FindPropertyRelative("m_HighlightedSprite");
            SerializedProperty property2 = prop.FindPropertyRelative("m_PressedSprite");
            SerializedProperty property3 = prop.FindPropertyRelative("m_DisabledSprite");
            EditorGUI.PropertyField(position, property);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property2);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property3);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}

