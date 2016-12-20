namespace UnityEditor
{
    using System;
    using UnityEditor.Animations;
    using UnityEngine;
    using UnityEngine.Networking;

    [CustomEditor(typeof(NetworkAnimator), true), CanEditMultipleObjects]
    public class NetworkAnimatorEditor : Editor
    {
        private SerializedProperty m_AnimatorProperty;
        private NetworkAnimator m_AnimSync;
        [NonSerialized]
        private bool m_Initialized;

        private void DrawControls()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.m_AnimatorProperty, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_AnimSync.ResetParameterOptions();
            }
            if (this.m_AnimSync.animator != null)
            {
                AnimatorController runtimeAnimatorController = this.m_AnimSync.animator.runtimeAnimatorController as AnimatorController;
                if (runtimeAnimatorController != null)
                {
                    EditorGUI.indentLevel++;
                    int index = 0;
                    foreach (AnimatorControllerParameter parameter in runtimeAnimatorController.parameters)
                    {
                        bool parameterAutoSend = this.m_AnimSync.GetParameterAutoSend(index);
                        bool flag2 = EditorGUILayout.Toggle(parameter.name, parameterAutoSend, new GUILayoutOption[0]);
                        if (flag2 != parameterAutoSend)
                        {
                            this.m_AnimSync.SetParameterAutoSend(index, flag2);
                            EditorUtility.SetDirty(base.target);
                        }
                        index++;
                    }
                    EditorGUI.indentLevel--;
                }
                if (Application.isPlaying)
                {
                    EditorGUILayout.Separator();
                    if (this.m_AnimSync.param0 != "")
                    {
                        EditorGUILayout.LabelField("Param 0", this.m_AnimSync.param0, new GUILayoutOption[0]);
                    }
                    if (this.m_AnimSync.param1 != "")
                    {
                        EditorGUILayout.LabelField("Param 1", this.m_AnimSync.param1, new GUILayoutOption[0]);
                    }
                    if (this.m_AnimSync.param2 != "")
                    {
                        EditorGUILayout.LabelField("Param 2", this.m_AnimSync.param2, new GUILayoutOption[0]);
                    }
                    if (this.m_AnimSync.param3 != "")
                    {
                        EditorGUILayout.LabelField("Param 3", this.m_AnimSync.param3, new GUILayoutOption[0]);
                    }
                    if (this.m_AnimSync.param4 != "")
                    {
                        EditorGUILayout.LabelField("Param 4", this.m_AnimSync.param4, new GUILayoutOption[0]);
                    }
                }
            }
        }

        private void Init()
        {
            if (!this.m_Initialized)
            {
                this.m_Initialized = true;
                this.m_AnimSync = base.target as NetworkAnimator;
                this.m_AnimatorProperty = base.serializedObject.FindProperty("m_Animator");
            }
        }

        public override void OnInspectorGUI()
        {
            this.Init();
            base.serializedObject.Update();
            this.DrawControls();
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

