namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(CapsuleCollider2D))]
    internal class CapsuleCollider2DEditor : PrimitiveCollider2DEditor
    {
        private readonly CapsuleBoundsHandle m_BoundsHandle = new CapsuleBoundsHandle(s_HandleControlIDHint);
        private SerializedProperty m_Direction;
        private SerializedProperty m_Size;
        private static readonly int s_HandleControlIDHint = typeof(CapsuleCollider2DEditor).Name.GetHashCode();

        protected override void CopyColliderSizeToHandle()
        {
            Vector3 vector;
            Vector3 vector2;
            CapsuleCollider2D target = (CapsuleCollider2D) base.target;
            this.GetHandleVectorsInWorldSpace(target, out vector, out vector2);
            this.m_BoundsHandle.height = vector.magnitude;
            this.m_BoundsHandle.radius = vector2.magnitude * 0.5f;
        }

        protected override bool CopyHandleSizeToCollider()
        {
            Vector3 up;
            Vector3 right;
            CapsuleCollider2D target = (CapsuleCollider2D) base.target;
            if (target.direction == CapsuleDirection2D.Horizontal)
            {
                up = Vector3.up;
                right = Vector3.right;
            }
            else
            {
                up = Vector3.right;
                right = Vector3.up;
            }
            Vector3 vector3 = (Vector3) (Handles.matrix * (right * this.m_BoundsHandle.height));
            Vector3 vector4 = (Vector3) (Handles.matrix * ((up * this.m_BoundsHandle.radius) * 2f));
            Matrix4x4 localToWorldMatrix = target.transform.localToWorldMatrix;
            Vector3 worldVector = (Vector3) (base.ProjectOntoWorldPlane((Vector3) (localToWorldMatrix * up)).normalized * vector4.magnitude);
            Vector3 vector7 = (Vector3) (base.ProjectOntoWorldPlane((Vector3) (localToWorldMatrix * right)).normalized * vector3.magnitude);
            worldVector = base.ProjectOntoColliderPlane(worldVector, localToWorldMatrix);
            vector7 = base.ProjectOntoColliderPlane(vector7, localToWorldMatrix);
            Vector2 size = target.size;
            target.size = (Vector2) (localToWorldMatrix.inverse * (worldVector + vector7));
            return (target.size != size);
        }

        protected override Quaternion GetHandleRotation()
        {
            Vector3 vector;
            Vector3 vector2;
            this.GetHandleVectorsInWorldSpace(base.target as CapsuleCollider2D, out vector2, out vector);
            return Quaternion.LookRotation(Vector3.forward, vector2);
        }

        private void GetHandleVectorsInWorldSpace(CapsuleCollider2D collider, out Vector3 handleHeightVector, out Vector3 handleDiameterVector)
        {
            Matrix4x4 localToWorldMatrix = collider.transform.localToWorldMatrix;
            Vector3 vector = base.ProjectOntoWorldPlane((Vector3) (localToWorldMatrix * (Vector3.right * collider.size.x)));
            Vector3 vector3 = base.ProjectOntoWorldPlane((Vector3) (localToWorldMatrix * (Vector3.up * collider.size.y)));
            if (collider.direction == CapsuleDirection2D.Horizontal)
            {
                handleDiameterVector = vector3;
                handleHeightVector = vector;
            }
            else
            {
                handleDiameterVector = vector;
                handleHeightVector = vector3;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Size = base.serializedObject.FindProperty("m_Size");
            this.m_Direction = base.serializedObject.FindProperty("m_Direction");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            base.InspectorEditButtonGUI();
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(this.m_Size, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Direction, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
            base.FinalizeInspectorGUI();
        }

        protected override PrimitiveBoundsHandle boundsHandle =>
            this.m_BoundsHandle;
    }
}

