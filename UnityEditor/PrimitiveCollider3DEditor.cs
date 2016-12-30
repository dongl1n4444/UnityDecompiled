namespace UnityEditor
{
    using System;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal abstract class PrimitiveCollider3DEditor : Collider3DEditorBase
    {
        protected PrimitiveCollider3DEditor()
        {
        }

        protected abstract void CopyColliderPropertiesToHandle();
        protected abstract void CopyHandlePropertiesToCollider();
        protected Vector3 InvertScaleVector(Vector3 scaleVector)
        {
            for (int i = 0; i < 3; i++)
            {
                scaleVector[i] = 1f / scaleVector[i];
            }
            return scaleVector;
        }

        protected virtual void OnSceneGUI()
        {
            if (base.editingCollider)
            {
                Collider target = (Collider) base.target;
                using (new Handles.MatrixScope(Matrix4x4.TRS(target.transform.position, target.transform.rotation, Vector3.one)))
                {
                    this.CopyColliderPropertiesToHandle();
                    this.boundsHandle.SetColor(!target.enabled ? Handles.s_ColliderHandleColorDisabled : Handles.s_ColliderHandleColor);
                    EditorGUI.BeginChangeCheck();
                    this.boundsHandle.DrawHandle();
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, $"Modify {ObjectNames.NicifyVariableName(base.target.GetType().Name)}");
                        this.CopyHandlePropertiesToCollider();
                    }
                }
            }
        }

        protected Vector3 TransformColliderCenterToHandleSpace(Transform colliderTransform, Vector3 colliderCenter) => 
            ((Vector3) (Handles.inverseMatrix * (colliderTransform.localToWorldMatrix * colliderCenter)));

        protected Vector3 TransformHandleCenterToColliderSpace(Transform colliderTransform, Vector3 handleCenter) => 
            ((Vector3) (colliderTransform.localToWorldMatrix.inverse * (Handles.matrix * handleCenter)));

        protected abstract PrimitiveBoundsHandle boundsHandle { get; }

        protected override GUIContent editModeButton =>
            PrimitiveBoundsHandle.editModeButton;
    }
}

