namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(DDSImporter)), CanEditMultipleObjects]
    internal class DDSImporterInspector : AssetImporterInspector
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            SerializedProperty property = base.serializedObject.FindProperty("m_IsReadable");
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            bool flag = EditorGUILayout.Toggle("IsReadable", property.boolValue, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                property.boolValue = flag;
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            base.ApplyRevertGUI();
            GUILayout.EndHorizontal();
        }

        internal override bool showImportedObject
        {
            get
            {
                return false;
            }
        }
    }
}

