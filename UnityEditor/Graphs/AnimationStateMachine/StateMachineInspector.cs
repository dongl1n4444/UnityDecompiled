namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditor.Graphs;
    using UnityEngine;

    [CustomEditor(typeof(AnimatorStateMachine)), CanEditMultipleObjects]
    internal class StateMachineInspector : Editor
    {
        private AnimatorStateMachine m_ActiveStateMachine;
        private StateMachineBehaviorsEditor m_BehavioursEditor = null;
        private SerializedProperty m_Name;
        private AnimatorStateMachine m_RootStateMachine;
        private SourceNodeTransitionEditor m_TransitionEditor = null;

        public override bool HasPreviewGUI()
        {
            return ((this.m_TransitionEditor != null) && this.m_TransitionEditor.HasPreviewGUI());
        }

        private void Init()
        {
            if ((this.m_TransitionEditor == null) && (this.m_ActiveStateMachine != (base.target as AnimatorStateMachine)))
            {
                this.m_TransitionEditor = new SourceNodeTransitionEditor(base.target as AnimatorStateMachine, UnityEditor.Graphs.AnimationStateMachine.TransitionType.eStateMachine, this);
            }
            if (this.m_BehavioursEditor == null)
            {
                this.m_BehavioursEditor = new StateMachineBehaviorsEditor(base.target as AnimatorStateMachine, this);
            }
        }

        public void OnDestroy()
        {
            if (this.m_TransitionEditor != null)
            {
                this.m_TransitionEditor.OnDestroy();
            }
            this.m_BehavioursEditor.OnDestroy();
        }

        public void OnDisable()
        {
            if (this.m_TransitionEditor != null)
            {
                this.m_TransitionEditor.OnDisable();
            }
            this.m_BehavioursEditor.OnDisable();
        }

        public void OnEnable()
        {
            this.m_Name = base.serializedObject.FindProperty("m_Name");
            this.m_RootStateMachine = ((AnimatorControllerTool.tool == null) || (AnimatorControllerTool.tool.animatorController == null)) ? null : AnimatorControllerTool.tool.animatorController.layers[AnimatorControllerTool.tool.selectedLayerIndex].stateMachine;
            this.m_ActiveStateMachine = ((AnimatorControllerTool.tool == null) || (AnimatorControllerTool.tool.stateMachineGraph == null)) ? null : AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine;
            this.Init();
            if (this.m_TransitionEditor != null)
            {
                this.m_TransitionEditor.OnEnable();
            }
            this.m_BehavioursEditor.OnEnable();
        }

        internal override void OnHeaderTitleGUI(Rect titleRect, string header)
        {
            base.serializedObject.Update();
            Rect position = titleRect;
            position.height = 16f;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = this.m_Name.hasMultipleDifferentValues;
            string str = EditorGUI.DelayedTextField(position, this.m_Name.stringValue, EditorStyles.textField);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(str))
            {
                foreach (UnityEngine.Object obj2 in base.targets)
                {
                    ObjectNames.SetNameSmart(obj2, (this.m_RootStateMachine == null) ? str : this.m_RootStateMachine.MakeUniqueStateMachineName(str));
                }
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            if (this.ShouldShowTransitionEditor())
            {
                this.m_TransitionEditor.OnInspectorGUI();
            }
            this.m_BehavioursEditor.OnInspectorGUI();
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            if (this.m_TransitionEditor != null)
            {
                this.m_TransitionEditor.OnInteractivePreviewGUI(r, background);
            }
        }

        public override void OnPreviewSettings()
        {
            if (this.m_TransitionEditor != null)
            {
                this.m_TransitionEditor.OnPreviewSettings();
            }
        }

        private bool ShouldShowTransitionEditor()
        {
            return (((this.m_TransitionEditor != null) && !base.serializedObject.isEditingMultipleObjects) && (((AnimatorControllerTool.tool != null) && (AnimatorControllerTool.tool.stateMachineGraph != null)) && (AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine != (base.target as AnimatorStateMachine))));
        }
    }
}

