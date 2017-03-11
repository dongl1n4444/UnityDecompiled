namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;

    internal class ColliderEditorBase : Editor
    {
        private static Bounds GetColliderBounds(UnityEngine.Object collider)
        {
            if (collider is Collider2D)
            {
                return (collider as Collider2D).bounds;
            }
            if (collider is Collider)
            {
                return (collider as Collider).bounds;
            }
            return new Bounds();
        }

        protected void InspectorEditButtonGUI()
        {
            UnityEditorInternal.EditMode.DoEditModeInspectorModeButton(UnityEditorInternal.EditMode.SceneViewEditMode.Collider, "Edit Collider", this.editModeButton, GetColliderBounds(base.target), this);
        }

        public virtual void OnDisable()
        {
            UnityEditorInternal.EditMode.onEditModeStartDelegate = (UnityEditorInternal.EditMode.OnEditModeStartFunc) Delegate.Remove(UnityEditorInternal.EditMode.onEditModeStartDelegate, new UnityEditorInternal.EditMode.OnEditModeStartFunc(this.OnEditModeStart));
            UnityEditorInternal.EditMode.onEditModeEndDelegate = (UnityEditorInternal.EditMode.OnEditModeStopFunc) Delegate.Remove(UnityEditorInternal.EditMode.onEditModeEndDelegate, new UnityEditorInternal.EditMode.OnEditModeStopFunc(this.OnEditModeEnd));
        }

        protected virtual void OnEditEnd()
        {
        }

        protected void OnEditModeEnd(Editor editor)
        {
            if (editor == this)
            {
                this.OnEditEnd();
            }
        }

        protected void OnEditModeStart(Editor editor, UnityEditorInternal.EditMode.SceneViewEditMode mode)
        {
            if ((mode == UnityEditorInternal.EditMode.SceneViewEditMode.Collider) && (editor == this))
            {
                this.OnEditStart();
            }
        }

        protected virtual void OnEditStart()
        {
        }

        public virtual void OnEnable()
        {
            UnityEditorInternal.EditMode.onEditModeStartDelegate = (UnityEditorInternal.EditMode.OnEditModeStartFunc) Delegate.Combine(UnityEditorInternal.EditMode.onEditModeStartDelegate, new UnityEditorInternal.EditMode.OnEditModeStartFunc(this.OnEditModeStart));
            UnityEditorInternal.EditMode.onEditModeEndDelegate = (UnityEditorInternal.EditMode.OnEditModeStopFunc) Delegate.Combine(UnityEditorInternal.EditMode.onEditModeEndDelegate, new UnityEditorInternal.EditMode.OnEditModeStopFunc(this.OnEditModeEnd));
        }

        public bool editingCollider =>
            ((UnityEditorInternal.EditMode.editMode == UnityEditorInternal.EditMode.SceneViewEditMode.Collider) && UnityEditorInternal.EditMode.IsOwner(this));

        protected virtual GUIContent editModeButton =>
            EditorGUIUtility.IconContent("EditCollider");
    }
}

