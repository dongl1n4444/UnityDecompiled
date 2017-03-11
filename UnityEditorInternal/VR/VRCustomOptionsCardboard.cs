namespace UnityEditorInternal.VR
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class VRCustomOptionsCardboard : VRCustomOptionsGoogleVR
    {
        private SerializedProperty m_EnableTransitionView;
        private static GUIContent s_EnableTransitionVewLabel = new GUIContent("Enable Tansition View");

        public override Rect Draw(Rect rect)
        {
            rect = base.Draw(rect);
            rect.height = EditorGUIUtility.singleLineHeight;
            GUIContent label = EditorGUI.BeginProperty(rect, s_EnableTransitionVewLabel, this.m_EnableTransitionView);
            EditorGUI.BeginChangeCheck();
            bool flag = EditorGUI.Toggle(rect, label, this.m_EnableTransitionView.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_EnableTransitionView.boolValue = flag;
            }
            EditorGUI.EndProperty();
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            return rect;
        }

        public override float GetHeight() => 
            ((base.GetHeight() + EditorGUIUtility.singleLineHeight) + EditorGUIUtility.standardVerticalSpacing);

        public override void Initialize(SerializedObject settings)
        {
            this.Initialize(settings, "cardboard");
        }

        public override void Initialize(SerializedObject settings, string propertyName)
        {
            base.Initialize(settings, propertyName);
            this.m_EnableTransitionView = base.FindPropertyAssert("enableTransitionView");
        }
    }
}

