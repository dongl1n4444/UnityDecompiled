namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class StructPropertyGUI
    {
        private static void DoChildren(Rect position, SerializedProperty property)
        {
            position.height = 16f;
            EditorGUI.indentLevel++;
            SerializedProperty x = property.Copy();
            SerializedProperty endProperty = x.GetEndProperty();
            x.NextVisible(true);
            while (!SerializedProperty.EqualContents(x, endProperty))
            {
                EditorGUI.PropertyField(position, x);
                position.y += 16f;
                if (!x.NextVisible(false))
                {
                    break;
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        internal static void GenericStruct(Rect position, SerializedProperty property)
        {
            GUI.Label(EditorGUI.IndentedRect(position), property.displayName, EditorStyles.label);
            position.y += 16f;
            DoChildren(position, property);
        }
    }
}

