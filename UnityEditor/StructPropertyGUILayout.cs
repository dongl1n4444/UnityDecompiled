namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class StructPropertyGUILayout
    {
        internal static void GenericStruct(SerializedProperty property, params GUILayoutOption[] options)
        {
            float minHeight = 16f + (16f * GetChildrenCount(property));
            StructPropertyGUI.GenericStruct(GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, minHeight, minHeight, EditorStyles.layerMaskField, options), property);
        }

        internal static int GetChildrenCount(SerializedProperty property)
        {
            int num = 0;
            SerializedProperty x = property.Copy();
            SerializedProperty endProperty = x.GetEndProperty();
            while (!SerializedProperty.EqualContents(x, endProperty))
            {
                num++;
                x.NextVisible(true);
            }
            return num;
        }
    }
}

