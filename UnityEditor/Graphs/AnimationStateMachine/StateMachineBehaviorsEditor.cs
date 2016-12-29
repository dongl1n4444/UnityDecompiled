namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditor.Graphs;
    using UnityEditorInternal;
    using UnityEngine;

    internal class StateMachineBehaviorsEditor
    {
        private Editor[] m_BehavioursEditor;
        private UnityEditor.Animations.AnimatorController m_ControllerContext;
        private int m_LayerIndexContext;
        private UnityEngine.Object m_Target;
        private static Styles s_Styles;

        public StateMachineBehaviorsEditor(AnimatorState state, Editor host)
        {
            this.m_BehavioursEditor = null;
            this.m_Target = state;
        }

        public StateMachineBehaviorsEditor(AnimatorStateMachine stateMachine, Editor host)
        {
            this.m_BehavioursEditor = null;
            this.m_Target = stateMachine;
        }

        protected void AddBehaviourButton()
        {
            bool flag2;
            EditorGUILayout.Space();
            Rect position = GUILayoutUtility.GetRect(styles.addBehaviourLabel, styles.addBehaviourButtonStyle, null);
            position.x += (position.width - 230f) / 2f;
            position.width = 230f;
            bool flag = UnityEditor.Animations.AnimatorController.CanAddStateMachineBehaviours();
            using (new EditorGUI.DisabledScope(!flag))
            {
                flag2 = EditorGUI.ButtonMouseDown(position, styles.addBehaviourLabel, FocusType.Passive, styles.addBehaviourButtonStyle);
            }
            if (flag2)
            {
                UnityEngine.Object[] targets = new UnityEngine.Object[] { this.m_Target };
                if (AddStateMachineBehaviourComponentWindow.Show(position, this.m_ControllerContext, this.m_LayerIndexContext, targets))
                {
                    GUIUtility.ExitGUI();
                }
            }
            EditorGUILayout.Space();
            if (!flag)
            {
                EditorGUILayout.HelpBox("Please fix compile errors before creating new state machine behaviours", MessageType.Error, true);
            }
        }

        protected void BuildEditorList(StateMachineBehaviour[] behaviours)
        {
            this.m_BehavioursEditor = new Editor[behaviours.Length];
            for (int i = 0; i < behaviours.Length; i++)
            {
                if ((behaviours[i] != null) && (behaviours[i] != null))
                {
                    this.m_BehavioursEditor[i] = Editor.CreateEditor(behaviours[i]);
                }
                else
                {
                    InvalidStateMachineBehaviour targetObject = ScriptableObject.CreateInstance<InvalidStateMachineBehaviour>();
                    targetObject.monoScript = this.GetBehaviourMonoScript(i);
                    if (targetObject.monoScript != null)
                    {
                        targetObject.name = targetObject.monoScript.name;
                    }
                    targetObject.hideFlags = HideFlags.HideAndDontSave;
                    targetObject.controller = this.m_ControllerContext;
                    targetObject.state = this.state;
                    targetObject.stateMachine = this.stateMachine;
                    targetObject.layerIndex = this.m_LayerIndexContext;
                    targetObject.behaviourIndex = i;
                    this.m_BehavioursEditor[i] = Editor.CreateEditor(targetObject);
                }
            }
        }

        protected MonoScript GetBehaviourMonoScript(int index)
        {
            if (this.state != null)
            {
                return ((this.m_ControllerContext == null) ? this.state.GetBehaviourMonoScript(index) : this.m_ControllerContext.GetBehaviourMonoScript(this.state, this.m_LayerIndexContext, index));
            }
            if (this.stateMachine != null)
            {
                return this.stateMachine.GetBehaviourMonoScript(index);
            }
            return null;
        }

        protected bool IsEditorsValid(StateMachineBehaviour[] behaviours)
        {
            if ((this.m_BehavioursEditor == null) || ((this.m_BehavioursEditor != null) && (this.m_BehavioursEditor.Length != behaviours.Length)))
            {
                return false;
            }
            for (int i = 0; i < this.m_BehavioursEditor.Length; i++)
            {
                if (this.m_BehavioursEditor[i].target != behaviours[i])
                {
                    return false;
                }
            }
            return true;
        }

        public void OnDestroy()
        {
        }

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
            this.m_ControllerContext = AnimatorControllerTool.tool?.animatorController;
            this.m_LayerIndexContext = (AnimatorControllerTool.tool == null) ? -1 : AnimatorControllerTool.tool.selectedLayerIndex;
        }

        public void OnInspectorGUI()
        {
            StateMachineBehaviour[] effectiveBehaviours = this.effectiveBehaviours;
            if (!this.IsEditorsValid(effectiveBehaviours))
            {
                this.BuildEditorList(effectiveBehaviours);
            }
            EditorGUI.BeginChangeCheck();
            foreach (Editor editor in this.m_BehavioursEditor)
            {
                EditorGUI.BeginChangeCheck();
                bool foldout = true;
                bool flag2 = !(editor.target is InvalidStateMachineBehaviour);
                if (flag2)
                {
                    foldout = InternalEditorUtility.GetIsInspectorExpanded(editor.target);
                }
                foldout = EditorGUILayout.InspectorTitlebar(foldout, editor.target, true);
                if (flag2 && EditorGUI.EndChangeCheck())
                {
                    InternalEditorUtility.SetIsInspectorExpanded(editor.target, foldout);
                }
                if (foldout || !flag2)
                {
                    editor.OnInspectorGUI();
                }
            }
            this.AddBehaviourButton();
        }

        protected StateMachineBehaviour[] effectiveBehaviours
        {
            get
            {
                if (this.state != null)
                {
                    return ((this.m_ControllerContext == null) ? this.state.behaviours : this.m_ControllerContext.GetStateEffectiveBehaviours(this.state, this.m_LayerIndexContext));
                }
                if (this.stateMachine != null)
                {
                    return this.stateMachine.behaviours;
                }
                return null;
            }
        }

        public AnimatorState state =>
            (this.m_Target as AnimatorState);

        public AnimatorStateMachine stateMachine =>
            (this.m_Target as AnimatorStateMachine);

        internal static Styles styles
        {
            get
            {
                Styles styles;
                if (s_Styles != null)
                {
                    styles = s_Styles;
                }
                else
                {
                    styles = s_Styles = new Styles();
                }
                return styles;
            }
        }

        internal class Styles
        {
            public GUIStyle addBehaviourButtonStyle = "LargeButton";
            public readonly GUIContent addBehaviourLabel = new GUIContent("Add Behaviour");
        }
    }
}

