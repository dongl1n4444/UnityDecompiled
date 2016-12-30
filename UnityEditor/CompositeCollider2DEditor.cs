namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    [CustomEditor(typeof(CompositeCollider2D)), CanEditMultipleObjects]
    internal class CompositeCollider2DEditor : Collider2DEditorBase
    {
        [CompilerGenerated]
        private static Func<Object, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Object, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<Object, bool> <>f__am$cache2;
        private SerializedProperty m_GenerationType;
        private SerializedProperty m_GeometryType;
        private readonly AnimBool m_ShowManualGenerationButton = new AnimBool();
        private SerializedProperty m_VertexDistance;

        public override void OnDisable()
        {
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_GeometryType = base.serializedObject.FindProperty("m_GeometryType");
            this.m_GenerationType = base.serializedObject.FindProperty("m_GenerationType");
            this.m_VertexDistance = base.serializedObject.FindProperty("m_VertexDistance");
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = x => (x as CompositeCollider2D).generationType != CompositeCollider2D.GenerationType.Manual;
            }
            this.m_ShowManualGenerationButton.value = Enumerable.Where<Object>(base.targets, <>f__am$cache0).Count<Object>() == 0;
            this.m_ShowManualGenerationButton.valueChanged.AddListener(new UnityAction(this.Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(this.m_GeometryType, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_GenerationType, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_VertexDistance, new GUILayoutOption[0]);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = x => (x as CompositeCollider2D).generationType != CompositeCollider2D.GenerationType.Manual;
            }
            this.m_ShowManualGenerationButton.target = Enumerable.Where<Object>(base.targets, <>f__am$cache1).Count<Object>() == 0;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowManualGenerationButton.faded) && GUILayout.Button("Regenerate Collider", new GUILayoutOption[0]))
            {
                foreach (Object obj2 in base.targets)
                {
                    (obj2 as CompositeCollider2D).GenerateGeometry();
                }
            }
            EditorGUILayout.EndFadeGroup();
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = x => (x as CompositeCollider2D).generationType == CompositeCollider2D.GenerationType.Asynchronous;
            }
            if (Enumerable.Where<Object>(base.targets, <>f__am$cache2).Count<Object>() > 0)
            {
                EditorGUILayout.HelpBox("Asynchronous generation is not supported yet.  Synchronous generation will be used.", MessageType.Info);
            }
            base.serializedObject.ApplyModifiedProperties();
            base.FinalizeInspectorGUI();
        }
    }
}

