namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// <para>PropertyDrawer for Navigation.</para>
    /// </summary>
    [CustomPropertyDrawer(typeof(Navigation), true)]
    public class NavigationDrawer : PropertyDrawer
    {
        private static Styles s_Styles = null;

        private static Navigation.Mode GetNavigationMode(SerializedProperty navigation)
        {
            return (Navigation.Mode) navigation.enumValueIndex;
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            SerializedProperty navigation = prop.FindPropertyRelative("m_Mode");
            if (navigation == null)
            {
                return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            }
            switch (GetNavigationMode(navigation))
            {
                case Navigation.Mode.None:
                    return EditorGUIUtility.singleLineHeight;

                case Navigation.Mode.Explicit:
                    return ((5f * EditorGUIUtility.singleLineHeight) + (5f * EditorGUIUtility.standardVerticalSpacing));
            }
            return (EditorGUIUtility.singleLineHeight + (1f * EditorGUIUtility.standardVerticalSpacing));
        }

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            Rect position = pos;
            position.height = EditorGUIUtility.singleLineHeight;
            SerializedProperty navigation = prop.FindPropertyRelative("m_Mode");
            Navigation.Mode navigationMode = GetNavigationMode(navigation);
            EditorGUI.PropertyField(position, navigation, s_Styles.navigationContent);
            EditorGUI.indentLevel++;
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (navigationMode == Navigation.Mode.Explicit)
            {
                SerializedProperty property = prop.FindPropertyRelative("m_SelectOnUp");
                SerializedProperty property3 = prop.FindPropertyRelative("m_SelectOnDown");
                SerializedProperty property4 = prop.FindPropertyRelative("m_SelectOnLeft");
                SerializedProperty property5 = prop.FindPropertyRelative("m_SelectOnRight");
                EditorGUI.PropertyField(position, property);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, property3);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, property4);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, property5);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            EditorGUI.indentLevel--;
        }

        private class Styles
        {
            public readonly GUIContent navigationContent = new GUIContent("Navigation");
        }
    }
}

