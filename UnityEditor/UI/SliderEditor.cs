namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Custom Editor for the Slider Component.</para>
    /// </summary>
    [CustomEditor(typeof(Slider), true), CanEditMultipleObjects]
    public class SliderEditor : SelectableEditor
    {
        private SerializedProperty m_Direction;
        private SerializedProperty m_FillRect;
        private SerializedProperty m_HandleRect;
        private SerializedProperty m_MaxValue;
        private SerializedProperty m_MinValue;
        private SerializedProperty m_OnValueChanged;
        private SerializedProperty m_Value;
        private SerializedProperty m_WholeNumbers;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_FillRect = base.serializedObject.FindProperty("m_FillRect");
            this.m_HandleRect = base.serializedObject.FindProperty("m_HandleRect");
            this.m_Direction = base.serializedObject.FindProperty("m_Direction");
            this.m_MinValue = base.serializedObject.FindProperty("m_MinValue");
            this.m_MaxValue = base.serializedObject.FindProperty("m_MaxValue");
            this.m_WholeNumbers = base.serializedObject.FindProperty("m_WholeNumbers");
            this.m_Value = base.serializedObject.FindProperty("m_Value");
            this.m_OnValueChanged = base.serializedObject.FindProperty("m_OnValueChanged");
        }

        /// <summary>
        /// <para>See Editor.OnInspectorGUI.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_FillRect, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_HandleRect, new GUILayoutOption[0]);
            if ((this.m_FillRect.objectReferenceValue != null) || (this.m_HandleRect.objectReferenceValue != null))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.m_Direction, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    Slider.Direction enumValueIndex = (Slider.Direction) this.m_Direction.enumValueIndex;
                    foreach (UnityEngine.Object obj2 in base.serializedObject.targetObjects)
                    {
                        (obj2 as Slider).SetDirection(enumValueIndex, true);
                    }
                }
                EditorGUILayout.PropertyField(this.m_MinValue, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_MaxValue, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_WholeNumbers, new GUILayoutOption[0]);
                EditorGUILayout.Slider(this.m_Value, this.m_MinValue.floatValue, this.m_MaxValue.floatValue, new GUILayoutOption[0]);
                bool flag = false;
                foreach (UnityEngine.Object obj3 in base.serializedObject.targetObjects)
                {
                    Slider slider2 = obj3 as Slider;
                    switch (slider2.direction)
                    {
                        case Slider.Direction.LeftToRight:
                        case Slider.Direction.RightToLeft:
                            flag = (slider2.navigation.mode != Navigation.Mode.Automatic) && ((slider2.FindSelectableOnLeft() != null) || (slider2.FindSelectableOnRight() != null));
                            break;

                        default:
                            flag = (slider2.navigation.mode != Navigation.Mode.Automatic) && ((slider2.FindSelectableOnDown() != null) || (slider2.FindSelectableOnUp() != null));
                            break;
                    }
                }
                if (flag)
                {
                    EditorGUILayout.HelpBox("The selected slider direction conflicts with navigation. Not all navigation options may work.", MessageType.Warning);
                }
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(this.m_OnValueChanged, new GUILayoutOption[0]);
            }
            else
            {
                EditorGUILayout.HelpBox("Specify a RectTransform for the slider fill or the slider handle or both. Each must have a parent RectTransform that it can slide within.", MessageType.Info);
            }
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

