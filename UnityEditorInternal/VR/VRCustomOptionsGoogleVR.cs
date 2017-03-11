namespace UnityEditorInternal.VR
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class VRCustomOptionsGoogleVR : VRCustomOptions
    {
        private SerializedProperty m_DepthFormat;
        private static GUIContent s_DepthFormatLabel = new GUIContent("Depth Format");
        private static GUIContent[] s_DepthOptions = new GUIContent[] { new GUIContent("16-bit depth"), new GUIContent("24-bit depth"), new GUIContent("24-bit depth | 8-bit stencil") };

        public override Rect Draw(Rect rect)
        {
            rect.y += EditorGUIUtility.standardVerticalSpacing;
            rect.height = EditorGUIUtility.singleLineHeight;
            GUIContent label = EditorGUI.BeginProperty(rect, s_DepthFormatLabel, this.m_DepthFormat);
            EditorGUI.BeginChangeCheck();
            int num = EditorGUI.Popup(rect, label, this.m_DepthFormat.intValue, s_DepthOptions);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_DepthFormat.intValue = num;
            }
            EditorGUI.EndProperty();
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            return rect;
        }

        public override float GetHeight() => 
            (EditorGUIUtility.singleLineHeight + (EditorGUIUtility.standardVerticalSpacing * 2f));

        public override void Initialize(SerializedObject settings, string propertyName)
        {
            base.Initialize(settings, propertyName);
            this.m_DepthFormat = base.FindPropertyAssert("depthFormat");
        }
    }
}

