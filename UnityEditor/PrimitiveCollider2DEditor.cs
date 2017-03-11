namespace UnityEditor
{
    using System;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal abstract class PrimitiveCollider2DEditor : Collider2DEditorBase
    {
        protected PrimitiveCollider2DEditor()
        {
        }

        protected abstract void CopyColliderSizeToHandle();
        protected abstract bool CopyHandleSizeToCollider();
        protected virtual Quaternion GetHandleRotation() => 
            Quaternion.identity;

        public override void OnEnable()
        {
            base.OnEnable();
            this.boundsHandle.axes = PrimitiveBoundsHandle.Axes.Y | PrimitiveBoundsHandle.Axes.X;
        }

        protected virtual void OnSceneGUI()
        {
            if (base.editingCollider)
            {
                Collider2D target = (Collider2D) base.target;
                if (!Mathf.Approximately(target.transform.lossyScale.sqrMagnitude, 0f))
                {
                    using (new Handles.DrawingScope(Matrix4x4.TRS(target.transform.position, this.GetHandleRotation(), Vector3.one)))
                    {
                        Matrix4x4 localToWorldMatrix = target.transform.localToWorldMatrix;
                        this.boundsHandle.center = this.ProjectOntoWorldPlane((Vector3) (Handles.inverseMatrix * (localToWorldMatrix * target.offset)));
                        this.CopyColliderSizeToHandle();
                        this.boundsHandle.SetColor(!target.enabled ? Handles.s_ColliderHandleColorDisabled : Handles.s_ColliderHandleColor);
                        EditorGUI.BeginChangeCheck();
                        this.boundsHandle.DrawHandle();
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(target, $"Modify {ObjectNames.NicifyVariableName(base.target.GetType().Name)}");
                            if (this.CopyHandleSizeToCollider())
                            {
                                target.offset = (Vector2) (localToWorldMatrix.inverse * this.ProjectOntoColliderPlane((Vector3) (Handles.matrix * this.boundsHandle.center), localToWorldMatrix));
                            }
                        }
                    }
                }
            }
        }

        protected Vector3 ProjectOntoColliderPlane(Vector3 worldVector, Matrix4x4 colliderTransformMatrix)
        {
            float num;
            Plane plane = new Plane(Vector3.Cross((Vector3) (colliderTransformMatrix * Vector3.right), (Vector3) (colliderTransformMatrix * Vector3.up)), Vector3.zero);
            Ray ray = new Ray(worldVector, Vector3.forward);
            if (!plane.Raycast(ray, out num))
            {
                ray.direction = Vector3.back;
                plane.Raycast(ray, out num);
            }
            return ray.GetPoint(num);
        }

        protected Vector3 ProjectOntoWorldPlane(Vector3 worldVector)
        {
            worldVector.z = 0f;
            return worldVector;
        }

        protected abstract PrimitiveBoundsHandle boundsHandle { get; }

        protected override GUIContent editModeButton =>
            PrimitiveBoundsHandle.editModeButton;
    }
}

