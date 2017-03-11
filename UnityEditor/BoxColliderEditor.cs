namespace UnityEditor
{
    using System;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    [CustomEditor(typeof(BoxCollider)), CanEditMultipleObjects]
    internal class BoxColliderEditor : PrimitiveCollider3DEditor
    {
        private readonly BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle(s_HandleControlIDHint);
        private SerializedProperty m_Center;
        private SerializedProperty m_Size;
        private static readonly int s_HandleControlIDHint = typeof(BoxColliderEditor).Name.GetHashCode();

        protected override void CopyColliderPropertiesToHandle()
        {
            BoxCollider target = (BoxCollider) base.target;
            this.m_BoundsHandle.center = base.TransformColliderCenterToHandleSpace(target.transform, target.center);
            this.m_BoundsHandle.size = Vector3.Scale(target.size, target.transform.lossyScale);
        }

        protected override void CopyHandlePropertiesToCollider()
        {
            BoxCollider target = (BoxCollider) base.target;
            target.center = base.TransformHandleCenterToColliderSpace(target.transform, this.m_BoundsHandle.center);
            Vector3 vector = Vector3.Scale(this.m_BoundsHandle.size, base.InvertScaleVector(target.transform.lossyScale));
            float x = Mathf.Abs(vector.x);
            float y = Mathf.Abs(vector.y);
            vector = new Vector3(x, y, Mathf.Abs(vector.z));
            target.size = vector;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Center = base.serializedObject.FindProperty("m_Center");
            this.m_Size = base.serializedObject.FindProperty("m_Size");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            base.InspectorEditButtonGUI();
            EditorGUILayout.PropertyField(base.m_IsTrigger, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(base.m_Material, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Center, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Size, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }

        protected override PrimitiveBoundsHandle boundsHandle =>
            this.m_BoundsHandle;
    }
}

