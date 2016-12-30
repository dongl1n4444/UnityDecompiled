namespace UnityEditor
{
    using System;
    using UnityEditor.IMGUI.Controls;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(OcclusionPortal))]
    internal class OcclusionPortalEditor : Editor
    {
        private readonly BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle(s_HandleControlIDHint);
        private SerializedProperty m_Center;
        private SerializedProperty m_Size;
        private Bounds m_WorldBounds;
        private static readonly int s_HandleControlIDHint = typeof(OcclusionPortalEditor).Name.GetHashCode();

        protected virtual void OnDisable()
        {
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.RecalculateWorldBounds));
        }

        protected virtual void OnEnable()
        {
            this.m_Center = base.serializedObject.FindProperty("m_Center");
            this.m_Size = base.serializedObject.FindProperty("m_Size");
            this.m_BoundsHandle.SetColor(Handles.s_ColliderHandleColor);
            this.RecalculateWorldBounds();
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.RecalculateWorldBounds));
        }

        public override void OnInspectorGUI()
        {
            EditMode.DoEditModeInspectorModeButton(EditMode.SceneViewEditMode.Collider, "Edit Bounds", PrimitiveBoundsHandle.editModeButton, this.m_WorldBounds, this);
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                this.RecalculateWorldBounds();
            }
        }

        protected virtual void OnSceneGUI()
        {
            if ((EditMode.editMode == EditMode.SceneViewEditMode.Collider) && EditMode.IsOwner(this))
            {
                OcclusionPortal target = base.target as OcclusionPortal;
                base.serializedObject.Update();
                using (new Handles.MatrixScope(target.transform.localToWorldMatrix))
                {
                    this.m_BoundsHandle.center = this.m_Center.vector3Value;
                    this.m_BoundsHandle.size = this.m_Size.vector3Value;
                    EditorGUI.BeginChangeCheck();
                    this.m_BoundsHandle.DrawHandle();
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.m_Center.vector3Value = this.m_BoundsHandle.center;
                        this.m_Size.vector3Value = this.m_BoundsHandle.size;
                        base.serializedObject.ApplyModifiedProperties();
                        this.RecalculateWorldBounds();
                    }
                }
            }
        }

        private void RecalculateWorldBounds()
        {
            base.serializedObject.Update();
            Bounds bounds = new Bounds(this.m_Center.vector3Value, this.m_Size.vector3Value);
            this.m_WorldBounds = new Bounds();
            Vector3 max = bounds.max;
            Vector3 min = bounds.min;
            Matrix4x4 localToWorldMatrix = (base.target as OcclusionPortal).transform.localToWorldMatrix;
            this.m_WorldBounds.Encapsulate((Vector3) (localToWorldMatrix * new Vector3(max.x, max.y, max.z)));
            this.m_WorldBounds.Encapsulate((Vector3) (localToWorldMatrix * new Vector3(max.x, max.y, min.z)));
            this.m_WorldBounds.Encapsulate((Vector3) (localToWorldMatrix * new Vector3(max.x, min.y, max.z)));
            this.m_WorldBounds.Encapsulate((Vector3) (localToWorldMatrix * new Vector3(min.x, max.y, max.z)));
            this.m_WorldBounds.Encapsulate((Vector3) (localToWorldMatrix * new Vector3(max.x, min.y, min.z)));
            this.m_WorldBounds.Encapsulate((Vector3) (localToWorldMatrix * new Vector3(min.x, max.y, min.z)));
            this.m_WorldBounds.Encapsulate((Vector3) (localToWorldMatrix * new Vector3(min.x, min.y, max.z)));
            this.m_WorldBounds.Encapsulate((Vector3) (localToWorldMatrix * new Vector3(min.x, min.y, min.z)));
        }
    }
}

