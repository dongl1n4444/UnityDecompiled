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

        protected override void CopyHandleSizeToCollider()
        {
            CircleCollider2D target = (CircleCollider2D) base.target;
            target.radius = this.m_BoundsHandle.radius / this.GetRadiusScaleFactor();
        }

        private float GetRadiusScaleFactor()
        {
            Vector3 lossyScale = ((Component) base.target).transform.lossyScale;
            float introduced2 = Mathf.Abs(lossyScale.x);
            return ((introduced2 <= Mathf.Abs(lossyScale.y)) ? lossyScale.y : lossyScale.x);
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

        protected override PrimitiveBoundsHandle boundHandle =>
            this.m_BoundsHandle;
    }
}

