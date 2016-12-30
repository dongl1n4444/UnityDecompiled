namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class SortingLayerEditorUtility
    {
        public static void RenderSortingLayerFields(SerializedProperty sortingOrder, SerializedProperty sortingLayer)
        {
            EditorGUILayout.SortingLayerField(Styles.m_SortingLayerStyle, sortingLayer, EditorStyles.popup, EditorStyles.label);
            EditorGUILayout.PropertyField(sortingOrder, Styles.m_SortingOrderStyle, new GUILayoutOption[0]);
        }

        public static void RenderSortingLayerFields(Rect r, SerializedProperty sortingOrder, SerializedProperty sortingLayer)
        {
            EditorGUI.SortingLayerField(r, Styles.m_SortingLayerStyle, sortingLayer, EditorStyles.popup, EditorStyles.label);
            r.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(r, sortingOrder, Styles.m_SortingOrderStyle);
        }

        private static class Styles
        {
            public static GUIContent m_SortingLayerStyle = EditorGUIUtility.TextContent("Sorting Layer|Name of the Renderer's sorting layer");
            public static GUIContent m_SortingOrderStyle = EditorGUIUtility.TextContent("Order in Layer|Renderer's order within a sorting layer");
        }
    }
}

