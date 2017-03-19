namespace UnityEditor
{
    using System;
    using UnityEditor.AnimatedValues;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;

    [CanEditMultipleObjects, CustomEditor(typeof(Animator))]
    internal class AnimatorInspector : Editor
    {
        private SerializedProperty m_ApplyRootMotion;
        private SerializedProperty m_Avatar;
        private SerializedProperty m_CullingMode;
        private bool m_IsRootPositionOrRotationControlledByCurves;
        private AnimBool m_ShowWarningMessage = new AnimBool();
        private SerializedProperty m_UpdateMode;
        private SerializedProperty m_WarningMessage;
        private static Styles styles;

        private void Init()
        {
            if (styles == null)
            {
                styles = new Styles();
            }
            this.InitShowOptions();
        }

        private void InitShowOptions()
        {
            this.m_ShowWarningMessage.value = this.IsWarningMessageEmpty;
            this.m_ShowWarningMessage.valueChanged.AddListener(new UnityAction(this.Repaint));
        }

        private void OnEnable()
        {
            this.m_Avatar = base.serializedObject.FindProperty("m_Avatar");
            this.m_ApplyRootMotion = base.serializedObject.FindProperty("m_ApplyRootMotion");
            this.m_CullingMode = base.serializedObject.FindProperty("m_CullingMode");
            this.m_UpdateMode = base.serializedObject.FindProperty("m_UpdateMode");
            this.m_WarningMessage = base.serializedObject.FindProperty("m_WarningMessage");
            this.Init();
        }

        public override void OnInspectorGUI()
        {
            bool flag = base.targets.Length > 1;
            bool flag2 = false;
            bool flag3 = false;
            Animator target = base.target as Animator;
            base.serializedObject.UpdateIfRequiredOrScript();
            this.UpdateShowOptions();
            EditorGUI.BeginChangeCheck();
            RuntimeAnimatorController controller = EditorGUILayout.ObjectField("Controller", target.runtimeAnimatorController, typeof(RuntimeAnimatorController), false, new GUILayoutOption[0]) as RuntimeAnimatorController;
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Animator animator2 in base.targets)
                {
                    Undo.RecordObject(animator2, "Changed AnimatorController");
                    animator2.runtimeAnimatorController = controller;
                }
                AnimationWindowUtility.ControllerChanged();
            }
            EditorGUILayout.PropertyField(this.m_Avatar, new GUILayoutOption[0]);
            if (target.supportsOnAnimatorMove && !flag)
            {
                EditorGUILayout.LabelField("Apply Root Motion", "Handled by Script", new GUILayoutOption[0]);
            }
            else
            {
                EditorGUILayout.PropertyField(this.m_ApplyRootMotion, styles.applyRootMotion, new GUILayoutOption[0]);
                if (Event.current.type == EventType.Layout)
                {
                    this.m_IsRootPositionOrRotationControlledByCurves = target.isRootPositionOrRotationControlledByCurves;
                }
                if (!this.m_ApplyRootMotion.boolValue && this.m_IsRootPositionOrRotationControlledByCurves)
                {
                    EditorGUILayout.HelpBox("Root position or rotation are controlled by curves", MessageType.Info, true);
                }
            }
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.m_UpdateMode, styles.updateMode, new GUILayoutOption[0]);
            flag3 = EditorGUI.EndChangeCheck();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.m_CullingMode, styles.cullingMode, new GUILayoutOption[0]);
            flag2 = EditorGUI.EndChangeCheck();
            if (!flag)
            {
                EditorGUILayout.HelpBox(target.GetStats(), MessageType.Info, true);
            }
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowWarningMessage.faded))
            {
                EditorGUILayout.HelpBox(this.WarningMessage, MessageType.Warning, true);
            }
            EditorGUILayout.EndFadeGroup();
            base.serializedObject.ApplyModifiedProperties();
            foreach (Animator animator3 in base.targets)
            {
                if (flag2)
                {
                    animator3.OnCullingModeChanged();
                }
                if (flag3)
                {
                    animator3.OnUpdateModeChanged();
                }
            }
        }

        private void UpdateShowOptions()
        {
            this.m_ShowWarningMessage.target = this.IsWarningMessageEmpty;
        }

        private bool IsWarningMessageEmpty =>
            ((this.m_WarningMessage != null) && (this.m_WarningMessage.stringValue.Length > 0));

        private string WarningMessage =>
            ((this.m_WarningMessage == null) ? "" : this.m_WarningMessage.stringValue);

        private class Styles
        {
            public GUIContent applyRootMotion = new GUIContent(EditorGUIUtility.TextContent("Apply Root Motion"));
            public GUIContent cullingMode = new GUIContent(EditorGUIUtility.TextContent("Culling Mode"));
            public GUIContent updateMode = new GUIContent(EditorGUIUtility.TextContent("Update Mode"));

            public Styles()
            {
                this.applyRootMotion.tooltip = "Automatically move the object using the root motion from the animations";
                this.updateMode.tooltip = "Controls when and how often the Animator is updated";
                this.cullingMode.tooltip = "Controls what is updated when the object has been culled";
            }
        }
    }
}

