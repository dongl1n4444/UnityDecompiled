namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(CapsuleCollider))]
    internal class CapsuleColliderEditor : PrimitiveCollider3DEditor
    {
        private readonly CapsuleBoundsHandle m_BoundsHandle = new CapsuleBoundsHandle(s_HandleControlIDHint);
        private SerializedProperty m_Center;
        private SerializedProperty m_Direction;
        private SerializedProperty m_Height;
        private SerializedProperty m_Radius;
        private static readonly int s_HandleControlIDHint = typeof(CapsuleColliderEditor).Name.GetHashCode();

        protected override void CopyColliderPropertiesToHandle()
        {
            float num;
            CapsuleCollider target = (CapsuleCollider) base.target;
            this.m_BoundsHandle.center = base.TransformColliderCenterToHandleSpace(target.transform, target.center);
            Vector3 vector = this.GetCapsuleColliderHandleScale(target.transform.lossyScale, target.direction, out num);
            this.m_BoundsHandle.height = target.height * Mathf.Abs(vector[target.direction]);
            this.m_BoundsHandle.radius = target.radius * num;
            switch (target.direction)
            {
                case 0:
                    this.m_BoundsHandle.heightAxis = CapsuleBoundsHandle.HeightAxis.X;
                    break;

                case 1:
                    this.m_BoundsHandle.heightAxis = CapsuleBoundsHandle.HeightAxis.Y;
                    break;

                case 2:
                    this.m_BoundsHandle.heightAxis = CapsuleBoundsHandle.HeightAxis.Z;
                    break;
            }
        }

        protected override void CopyHandlePropertiesToCollider()
        {
            float num;
            CapsuleCollider target = (CapsuleCollider) base.target;
            target.center = base.TransformHandleCenterToColliderSpace(target.transform, this.m_BoundsHandle.center);
            Vector3 scaleVector = this.GetCapsuleColliderHandleScale(target.transform.lossyScale, target.direction, out num);
            scaleVector = base.InvertScaleVector(scaleVector);
            target.height = this.m_BoundsHandle.height * Mathf.Abs(scaleVector[target.direction]);
            target.radius = this.m_BoundsHandle.radius / num;
        }

        private Vector3 GetCapsuleColliderHandleScale(Vector3 lossyScale, int capsuleDirection, out float radiusScaleFactor)
        {
            radiusScaleFactor = 0f;
            for (int i = 0; i < 3; i++)
            {
                if (i != capsuleDirection)
                {
                    radiusScaleFactor = Mathf.Max(radiusScaleFactor, Mathf.Abs(lossyScale[i]));
                }
            }
            for (int j = 0; j < 3; j++)
            {
                if (j != capsuleDirection)
                {
                    lossyScale[j] = Mathf.Sign(lossyScale[j]) * radiusScaleFactor;
                }
            }
            return lossyScale;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Center = base.serializedObject.FindProperty("m_Center");
            this.m_Radius = base.serializedObject.FindProperty("m_Radius");
            this.m_Height = base.serializedObject.FindProperty("m_Height");
            this.m_Direction = base.serializedObject.FindProperty("m_Direction");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            base.InspectorEditButtonGUI();
            EditorGUILayout.PropertyField(base.m_IsTrigger, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(base.m_Material, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Center, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Radius, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Height, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Direction, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }

        protected override PrimitiveBoundsHandle boundsHandle =>
            this.m_BoundsHandle;
    }
}

