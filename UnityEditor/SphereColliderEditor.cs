namespace UnityEditor
{
    using System;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    [CustomEditor(typeof(SphereCollider)), CanEditMultipleObjects]
    internal class SphereColliderEditor : PrimitiveCollider3DEditor
    {
        private readonly SphereBoundsHandle m_BoundsHandle = new SphereBoundsHandle(s_HandleControlIDHint);
        private SerializedProperty m_Center;
        private SerializedProperty m_Radius;
        private static readonly int s_HandleControlIDHint = typeof(SphereColliderEditor).Name.GetHashCode();

        protected override void CopyColliderPropertiesToHandle()
        {
            SphereCollider target = (SphereCollider) base.target;
            this.m_BoundsHandle.center = base.TransformColliderCenterToHandleSpace(target.transform, target.center);
            this.m_BoundsHandle.radius = target.radius * this.GetRadiusScaleFactor();
        }

        protected override void CopyHandlePropertiesToCollider()
        {
            SphereCollider target = (SphereCollider) base.target;
            target.center = base.TransformHandleCenterToColliderSpace(target.transform, this.m_BoundsHandle.center);
            float radiusScaleFactor = this.GetRadiusScaleFactor();
            target.radius = !Mathf.Approximately(radiusScaleFactor, 0f) ? (this.m_BoundsHandle.radius / this.GetRadiusScaleFactor()) : 0f;
        }

        private float GetRadiusScaleFactor()
        {
            float a = 0f;
            Vector3 lossyScale = ((SphereCollider) base.target).transform.lossyScale;
            for (int i = 0; i < 3; i++)
            {
                a = Mathf.Max(a, Mathf.Abs(lossyScale[i]));
            }
            return a;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Center = base.serializedObject.FindProperty("m_Center");
            this.m_Radius = base.serializedObject.FindProperty("m_Radius");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            base.InspectorEditButtonGUI();
            EditorGUILayout.PropertyField(base.m_IsTrigger, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(base.m_Material, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Center, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Radius, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }

        protected override PrimitiveBoundsHandle boundsHandle =>
            this.m_BoundsHandle;
    }
}

