namespace UnityEditor
{
    using System;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(CircleCollider2D))]
    internal class CircleCollider2DEditor : PrimitiveCollider2DEditor
    {
        private readonly SphereBoundsHandle m_BoundsHandle = new SphereBoundsHandle(s_HandleControlIDHint);
        private SerializedProperty m_Radius;
        private static readonly int s_HandleControlIDHint = typeof(CircleCollider2DEditor).Name.GetHashCode();

        protected override void CopyColliderSizeToHandle()
        {
            CircleCollider2D target = (CircleCollider2D) base.target;
            this.m_BoundsHandle.radius = target.radius * this.GetRadiusScaleFactor();
        }

        protected override bool CopyHandleSizeToCollider()
        {
            CircleCollider2D target = (CircleCollider2D) base.target;
            float radius = target.radius;
            float radiusScaleFactor = this.GetRadiusScaleFactor();
            target.radius = !Mathf.Approximately(radiusScaleFactor, 0f) ? (this.m_BoundsHandle.radius / this.GetRadiusScaleFactor()) : 0f;
            return !(target.radius == radius);
        }

        private float GetRadiusScaleFactor()
        {
            Vector3 lossyScale = ((Component) base.target).transform.lossyScale;
            float a = Mathf.Abs(lossyScale.x);
            return Mathf.Max(a, Mathf.Abs(lossyScale.y));
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Radius = base.serializedObject.FindProperty("m_Radius");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            base.InspectorEditButtonGUI();
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(this.m_Radius, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
            base.FinalizeInspectorGUI();
        }

        protected override PrimitiveBoundsHandle boundsHandle =>
            this.m_BoundsHandle;
    }
}

