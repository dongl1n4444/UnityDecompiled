namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    [CanEditMultipleObjects, CustomEditor(typeof(CompositeCollider2D))]
    internal class CompositeCollider2DEditor : Collider2DEditorBase
    {
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache4;
        private SerializedProperty m_EdgeRadius;
        private SerializedProperty m_GenerationType;
        private SerializedProperty m_GeometryType;
        private readonly AnimBool m_ShowEdgeRadius = new AnimBool();
        private readonly AnimBool m_ShowManualGenerationButton = new AnimBool();
        private SerializedProperty m_VertexDistance;

        public override void OnDisable()
        {
            this.m_ShowEdgeRadius.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_ShowManualGenerationButton.valueChanged.RemoveListener(new UnityAction(this.Repaint));
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_GeometryType = base.serializedObject.FindProperty("m_GeometryType");
            this.m_GenerationType = base.serializedObject.FindProperty("m_GenerationType");
            this.m_VertexDistance = base.serializedObject.FindProperty("m_VertexDistance");
            this.m_EdgeRadius = base.serializedObject.FindProperty("m_EdgeRadius");
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = x => (x as CompositeCollider2D).geometryType == CompositeCollider2D.GeometryType.Polygons;
            }
            this.m_ShowEdgeRadius.value = Enumerable.Where<UnityEngine.Object>(base.targets, <>f__am$cache0).Count<UnityEngine.Object>() == 0;
            this.m_ShowEdgeRadius.valueChanged.AddListener(new UnityAction(this.Repaint));
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = x => (x as CompositeCollider2D).generationType != CompositeCollider2D.GenerationType.Manual;
            }
            this.m_ShowManualGenerationButton.value = Enumerable.Where<UnityEngine.Object>(base.targets, <>f__am$cache1).Count<UnityEngine.Object>() == 0;
            this.m_ShowManualGenerationButton.valueChanged.AddListener(new UnityAction(this.Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(this.m_GeometryType, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_GenerationType, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_VertexDistance, new GUILayoutOption[0]);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = x => (x as CompositeCollider2D).generationType != CompositeCollider2D.GenerationType.Manual;
            }
            this.m_ShowManualGenerationButton.target = Enumerable.Where<UnityEngine.Object>(base.targets, <>f__am$cache2).Count<UnityEngine.Object>() == 0;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowManualGenerationButton.faded) && GUILayout.Button("Regenerate Collider", new GUILayoutOption[0]))
            {
                foreach (UnityEngine.Object obj2 in base.targets)
                {
                    (obj2 as CompositeCollider2D).GenerateGeometry();
                }
            }
            EditorGUILayout.EndFadeGroup();
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = x => (x as CompositeCollider2D).geometryType == CompositeCollider2D.GeometryType.Polygons;
            }
            this.m_ShowEdgeRadius.target = Enumerable.Where<UnityEngine.Object>(base.targets, <>f__am$cache3).Count<UnityEngine.Object>() == 0;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowEdgeRadius.faded))
            {
                EditorGUILayout.PropertyField(this.m_EdgeRadius, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = x => (((x as CompositeCollider2D).geometryType == CompositeCollider2D.GeometryType.Outlines) && ((x as CompositeCollider2D).attachedRigidbody != null)) && ((x as CompositeCollider2D).attachedRigidbody.bodyType == RigidbodyType2D.Dynamic);
            }
            if (Enumerable.Where<UnityEngine.Object>(base.targets, <>f__am$cache4).Count<UnityEngine.Object>() > 0)
            {
                EditorGUILayout.HelpBox("Outline geometry is composed of edges and will not preserve the original collider's center-of-mass or rotational inertia.  The CompositeCollider2D is attached to a Dynamic Rigidbody2D so you may need to explicitly set these if they are required.", MessageType.Info);
            }
            base.serializedObject.ApplyModifiedProperties();
            base.FinalizeInspectorGUI();
        }
    }
}

