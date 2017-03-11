namespace UnityEditor
{
    using System;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    [CustomEditor(typeof(AreaEffector2D), true), CanEditMultipleObjects]
    internal class AreaEffector2DEditor : Effector2DEditor
    {
        private SerializedProperty m_AngularDrag;
        private SerializedProperty m_Drag;
        private SerializedProperty m_ForceAngle;
        private SerializedProperty m_ForceMagnitude;
        private SerializedProperty m_ForceTarget;
        private SerializedProperty m_ForceVariation;
        private static readonly AnimBool m_ShowDampingRollout = new AnimBool();
        private readonly AnimBool m_ShowForceRollout = new AnimBool();
        private SerializedProperty m_UseGlobalAngle;

        public override void OnDisable()
        {
            base.OnDisable();
            this.m_ShowForceRollout.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            m_ShowDampingRollout.valueChanged.RemoveListener(new UnityAction(this.Repaint));
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_ShowForceRollout.value = true;
            this.m_ShowForceRollout.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_UseGlobalAngle = base.serializedObject.FindProperty("m_UseGlobalAngle");
            this.m_ForceAngle = base.serializedObject.FindProperty("m_ForceAngle");
            this.m_ForceMagnitude = base.serializedObject.FindProperty("m_ForceMagnitude");
            this.m_ForceVariation = base.serializedObject.FindProperty("m_ForceVariation");
            this.m_ForceTarget = base.serializedObject.FindProperty("m_ForceTarget");
            m_ShowDampingRollout.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_Drag = base.serializedObject.FindProperty("m_Drag");
            this.m_AngularDrag = base.serializedObject.FindProperty("m_AngularDrag");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            base.serializedObject.Update();
            this.m_ShowForceRollout.target = EditorGUILayout.Foldout(this.m_ShowForceRollout.target, "Force", true);
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowForceRollout.faded))
            {
                EditorGUILayout.PropertyField(this.m_UseGlobalAngle, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_ForceAngle, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_ForceMagnitude, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_ForceVariation, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_ForceTarget, new GUILayoutOption[0]);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
            m_ShowDampingRollout.target = EditorGUILayout.Foldout(m_ShowDampingRollout.target, "Damping", true);
            if (EditorGUILayout.BeginFadeGroup(m_ShowDampingRollout.faded))
            {
                EditorGUILayout.PropertyField(this.m_Drag, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_AngularDrag, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

