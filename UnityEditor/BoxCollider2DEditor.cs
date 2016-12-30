namespace UnityEditor
{
    using System;
    using UnityEditor.AnimatedValues;
    using UnityEditor.IMGUI.Controls;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;

    [CanEditMultipleObjects, CustomEditor(typeof(BoxCollider2D))]
    internal class BoxCollider2DEditor : Collider2DEditorBase
    {
        private readonly BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle(s_HandleControlIDHint);
        private readonly AnimBool m_ShowCompositeRedundants = new AnimBool();
        private SerializedProperty m_Size;
        private SerializedProperty m_UsedByComposite;
        private static readonly int s_HandleControlIDHint = typeof(BoxCollider2DEditor).Name.GetHashCode();

        public override void OnDisable()
        {
            base.OnDisable();
            this.m_ShowCompositeRedundants.valueChanged.RemoveListener(new UnityAction(this.Repaint));
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Size = base.serializedObject.FindProperty("m_Size");
            this.m_BoundsHandle.axes = PrimitiveBoundsHandle.Axes.Y | PrimitiveBoundsHandle.Axes.X;
            this.m_UsedByComposite = base.serializedObject.FindProperty("m_UsedByComposite");
            base.m_AutoTiling = base.serializedObject.FindProperty("m_AutoTiling");
            this.m_ShowCompositeRedundants.value = !this.m_UsedByComposite.boolValue;
            this.m_ShowCompositeRedundants.valueChanged.AddListener(new UnityAction(this.Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            if (!base.CanEditCollider())
            {
                EditorGUILayout.HelpBox(Collider2DEditorBase.Styles.s_ColliderEditDisableHelp.text, MessageType.Info);
                if (base.editingCollider)
                {
                    EditMode.QuitEditMode();
                }
            }
            else
            {
                base.InspectorEditButtonGUI();
            }
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(this.m_Size, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
            base.FinalizeInspectorGUI();
        }

        protected virtual void OnSceneGUI()
        {
            if (base.editingCollider)
            {
                BoxCollider2D target = (BoxCollider2D) base.target;
                Matrix4x4 localToWorldMatrix = target.transform.localToWorldMatrix;
                localToWorldMatrix.SetRow(0, Vector4.Scale(localToWorldMatrix.GetRow(0), new Vector4(1f, 1f, 0f, 1f)));
                localToWorldMatrix.SetRow(1, Vector4.Scale(localToWorldMatrix.GetRow(1), new Vector4(1f, 1f, 0f, 1f)));
                localToWorldMatrix.SetRow(2, new Vector4(0f, 0f, 1f, target.transform.position.z));
                using (new Handles.MatrixScope(localToWorldMatrix))
                {
                    this.m_BoundsHandle.center = (Vector3) target.offset;
                    this.m_BoundsHandle.size = (Vector3) target.size;
                    this.m_BoundsHandle.SetColor(!target.enabled ? Handles.s_ColliderHandleColorDisabled : Handles.s_ColliderHandleColor);
                    EditorGUI.BeginChangeCheck();
                    this.m_BoundsHandle.DrawHandle();
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, $"Modify {ObjectNames.NicifyVariableName(base.target.GetType().Name)}");
                        target.offset = this.m_BoundsHandle.center;
                        target.size = this.m_BoundsHandle.size;
                    }
                }
            }
        }

        protected override GUIContent editModeButton =>
            PrimitiveBoundsHandle.editModeButton;
    }
}

