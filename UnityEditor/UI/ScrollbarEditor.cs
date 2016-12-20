namespace UnityEditor.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Custom Editor for the Scrollbar Component.</para>
    /// </summary>
    [CustomEditor(typeof(Scrollbar), true), CanEditMultipleObjects]
    public class ScrollbarEditor : SelectableEditor
    {
        private SerializedProperty m_Direction;
        private SerializedProperty m_HandleRect;
        private SerializedProperty m_NumberOfSteps;
        private SerializedProperty m_OnValueChanged;
        private SerializedProperty m_Size;
        private SerializedProperty m_Value;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_HandleRect = base.serializedObject.FindProperty("m_HandleRect");
            this.m_Direction = base.serializedObject.FindProperty("m_Direction");
            this.m_Value = base.serializedObject.FindProperty("m_Value");
            this.m_Size = base.serializedObject.FindProperty("m_Size");
            this.m_NumberOfSteps = base.serializedObject.FindProperty("m_NumberOfSteps");
            this.m_OnValueChanged = base.serializedObject.FindProperty("m_OnValueChanged");
        }

        /// <summary>
        /// <para>See: Editor.OnInspectorGUI.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            base.serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            RectTransform transform = EditorGUILayout.ObjectField("Handle Rect", this.m_HandleRect.objectReferenceValue, typeof(RectTransform), true, new GUILayoutOption[0]) as RectTransform;
            if (EditorGUI.EndChangeCheck())
            {
                List<UnityEngine.Object> list = new List<UnityEngine.Object> {
                    transform
                };
                foreach (UnityEngine.Object obj2 in this.m_HandleRect.serializedObject.targetObjects)
                {
                    MonoBehaviour item = obj2 as MonoBehaviour;
                    if (item != null)
                    {
                        list.Add(item);
                        list.Add(item.GetComponent<RectTransform>());
                    }
                }
                Undo.RecordObjects(list.ToArray(), "Change Handle Rect");
                this.m_HandleRect.objectReferenceValue = transform;
            }
            if (this.m_HandleRect.objectReferenceValue != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.m_Direction, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    Scrollbar.Direction enumValueIndex = (Scrollbar.Direction) this.m_Direction.enumValueIndex;
                    foreach (UnityEngine.Object obj3 in base.serializedObject.targetObjects)
                    {
                        (obj3 as Scrollbar).SetDirection(enumValueIndex, true);
                    }
                }
                EditorGUILayout.PropertyField(this.m_Value, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_Size, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_NumberOfSteps, new GUILayoutOption[0]);
                bool flag = false;
                foreach (UnityEngine.Object obj4 in base.serializedObject.targetObjects)
                {
                    Scrollbar scrollbar2 = obj4 as Scrollbar;
                    switch (scrollbar2.direction)
                    {
                        case Scrollbar.Direction.LeftToRight:
                        case Scrollbar.Direction.RightToLeft:
                            flag = (scrollbar2.navigation.mode != Navigation.Mode.Automatic) && ((scrollbar2.FindSelectableOnLeft() != null) || (scrollbar2.FindSelectableOnRight() != null));
                            break;

                        default:
                            flag = (scrollbar2.navigation.mode != Navigation.Mode.Automatic) && ((scrollbar2.FindSelectableOnDown() != null) || (scrollbar2.FindSelectableOnUp() != null));
                            break;
                    }
                }
                if (flag)
                {
                    EditorGUILayout.HelpBox("The selected scrollbar direction conflicts with navigation. Not all navigation options may work.", MessageType.Warning);
                }
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(this.m_OnValueChanged, new GUILayoutOption[0]);
            }
            else
            {
                EditorGUILayout.HelpBox("Specify a RectTransform for the scrollbar handle. It must have a parent RectTransform that the handle can slide within.", MessageType.Info);
            }
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

