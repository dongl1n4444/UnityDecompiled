namespace UnityEditor.UI
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Editor for the LayoutElement component.</para>
    /// </summary>
    [CustomEditor(typeof(LayoutElement), true), CanEditMultipleObjects]
    public class LayoutElementEditor : Editor
    {
        [CompilerGenerated]
        private static Func<RectTransform, float> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<RectTransform, float> <>f__am$cache1;
        private SerializedProperty m_FlexibleHeight;
        private SerializedProperty m_FlexibleWidth;
        private SerializedProperty m_IgnoreLayout;
        private SerializedProperty m_MinHeight;
        private SerializedProperty m_MinWidth;
        private SerializedProperty m_PreferredHeight;
        private SerializedProperty m_PreferredWidth;

        private void LayoutElementField(SerializedProperty property, Func<RectTransform, float> defaultValue)
        {
            Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
            GUIContent label = EditorGUI.BeginProperty(controlRect, null, property);
            Rect rect2 = EditorGUI.PrefixLabel(controlRect, label);
            Rect position = rect2;
            position.width = 16f;
            Rect rect4 = rect2;
            rect4.xMin += 16f;
            EditorGUI.BeginChangeCheck();
            bool flag = EditorGUI.ToggleLeft(position, GUIContent.none, property.floatValue >= 0f);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = !flag ? -1f : defaultValue.Invoke((base.target as LayoutElement).transform as RectTransform);
            }
            if (!property.hasMultipleDifferentValues && (property.floatValue >= 0f))
            {
                EditorGUIUtility.labelWidth = 4f;
                EditorGUI.BeginChangeCheck();
                float b = EditorGUI.FloatField(rect4, new GUIContent(" "), property.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = Mathf.Max(0f, b);
                }
                EditorGUIUtility.labelWidth = 0f;
            }
            EditorGUI.EndProperty();
        }

        private void LayoutElementField(SerializedProperty property, float defaultValue)
        {
            <LayoutElementField>c__AnonStorey0 storey = new <LayoutElementField>c__AnonStorey0 {
                defaultValue = defaultValue
            };
            this.LayoutElementField(property, new Func<RectTransform, float>(storey, (IntPtr) this.<>m__0));
        }

        protected virtual void OnEnable()
        {
            this.m_IgnoreLayout = base.serializedObject.FindProperty("m_IgnoreLayout");
            this.m_MinWidth = base.serializedObject.FindProperty("m_MinWidth");
            this.m_MinHeight = base.serializedObject.FindProperty("m_MinHeight");
            this.m_PreferredWidth = base.serializedObject.FindProperty("m_PreferredWidth");
            this.m_PreferredHeight = base.serializedObject.FindProperty("m_PreferredHeight");
            this.m_FlexibleWidth = base.serializedObject.FindProperty("m_FlexibleWidth");
            this.m_FlexibleHeight = base.serializedObject.FindProperty("m_FlexibleHeight");
        }

        /// <summary>
        /// <para>See: Editor.OnInspectorGUI.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_IgnoreLayout, new GUILayoutOption[0]);
            if (!this.m_IgnoreLayout.boolValue)
            {
                EditorGUILayout.Space();
                this.LayoutElementField(this.m_MinWidth, (float) 0f);
                this.LayoutElementField(this.m_MinHeight, (float) 0f);
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<RectTransform, float>(null, (IntPtr) <OnInspectorGUI>m__0);
                }
                this.LayoutElementField(this.m_PreferredWidth, <>f__am$cache0);
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = new Func<RectTransform, float>(null, (IntPtr) <OnInspectorGUI>m__1);
                }
                this.LayoutElementField(this.m_PreferredHeight, <>f__am$cache1);
                this.LayoutElementField(this.m_FlexibleWidth, (float) 1f);
                this.LayoutElementField(this.m_FlexibleHeight, (float) 1f);
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        [CompilerGenerated]
        private sealed class <LayoutElementField>c__AnonStorey0
        {
            internal float defaultValue;

            internal float <>m__0(RectTransform _) => 
                this.defaultValue;
        }
    }
}

