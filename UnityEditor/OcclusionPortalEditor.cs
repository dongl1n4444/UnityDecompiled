namespace UnityEditor
{
    using System;
    using UnityEditor.IMGUI.Controls;
    using UnityEditorInternal;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(OcclusionPortal))]
    internal class OcclusionPortalEditor : Editor
    {
        private readonly BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle(s_HandleControlIDHint);
        private SerializedProperty m_Center;
        private SerializedProperty m_Size;
        private static readonly int s_HandleControlIDHint = typeof(OcclusionPortalEditor).Name.GetHashCode();

        private Bounds GetWorldBounds(Vector3 center, Vector3 size)
        {
            Bounds bounds = new Bounds(center, size);
            Vector3 max = bounds.max;
            Vector3 min = bounds.min;
            Matrix4x4 localToWorldMatrix = ((OcclusionPortal) base.target).transform.localToWorldMatrix;
            Bounds bounds2 = new Bounds(localToWorldMatrix.MultiplyPoint3x4(new Vector3(max.x, max.y, max.z)), Vector3.zero);
            bounds2.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(new Vector3(max.x, max.y, max.z)));
            bounds2.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(new Vector3(max.x, max.y, min.z)));
            bounds2.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(new Vector3(max.x, min.y, max.z)));
            bounds2.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(new Vector3(min.x, max.y, max.z)));
            bounds2.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(new Vector3(max.x, min.y, min.z)));
            bounds2.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(new Vector3(min.x, max.y, min.z)));
            bounds2.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(new Vector3(min.x, min.y, max.z)));
            bounds2.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(new Vector3(min.x, min.y, min.z)));
            return bounds2;
        }

        protected virtual void OnEnable()
        {
            this.m_Center = base.serializedObject.FindProperty("m_Center");
            this.m_Size = base.serializedObject.FindProperty("m_Size");
            this.m_BoundsHandle.SetColor(Handles.s_ColliderHandleColor);
        }

        public override void OnInspectorGUI()
        {
            UnityEditorInternal.EditMode.DoEditModeInspectorModeButton(UnityEditorInternal.EditMode.SceneViewEditMode.Collider, "Edit Bounds", PrimitiveBoundsHandle.editModeButton, this.GetWorldBounds(this.m_Center.vector3Value, this.m_Size.vector3Value), this);
            base.OnInspectorGUI();
        }

        protected virtual void OnSceneGUI()
        {
            if ((UnityEditorInternal.EditMode.editMode == UnityEditorInternal.EditMode.SceneViewEditMode.Collider) && UnityEditorInternal.EditMode.IsOwner(this))
            {
                OcclusionPortal target = base.target as OcclusionPortal;
                SerializedObject obj2 = new SerializedObject(target);
                obj2.Update();
                using (new Handles.DrawingScope(target.transform.localToWorldMatrix))
                {
                    SerializedProperty property = obj2.FindProperty(this.m_Center.propertyPath);
                    SerializedProperty property2 = obj2.FindProperty(this.m_Size.propertyPath);
                    this.m_BoundsHandle.center = property.vector3Value;
                    this.m_BoundsHandle.size = property2.vector3Value;
                    EditorGUI.BeginChangeCheck();
                    this.m_BoundsHandle.DrawHandle();
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.vector3Value = this.m_BoundsHandle.center;
                        property2.vector3Value = this.m_BoundsHandle.size;
                        obj2.ApplyModifiedProperties();
                    }
                }
            }
        }
    }
}

