namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential, Size=1)]
    internal struct TableScoper : IDisposable
    {
        public TableScoper(int indent, drawDelegate func)
        {
            EditorGUI.indentLevel += indent;
            int indentLevel = EditorGUI.indentLevel;
            float pixels = EditorGUI.indent;
            EditorGUI.indentLevel = 0;
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(pixels);
            using (new EditorGUILayout.VerticalScope(new GUILayoutOption[0]))
            {
                func();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUI.indentLevel = indentLevel;
            EditorGUI.indentLevel -= indent;
        }

        public void Dispose()
        {
        }
        internal delegate void drawDelegate();
    }
}

