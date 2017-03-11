namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditor.Graphs;
    using UnityEditorInternal;
    using UnityEngine;

    internal class SourceNodeTransitionEditor
    {
        [CompilerGenerated]
        private static Converter<TransitionEditionContext, AnimatorStateTransition> <>f__am$cache0;
        [CompilerGenerated]
        private static Converter<TransitionEditionContext, AnimatorStateTransition> <>f__am$cache1;
        [CompilerGenerated]
        private static Converter<TransitionEditionContext, AnimatorTransition> <>f__am$cache2;
        [CompilerGenerated]
        private static Converter<TransitionEditionContext, AnimatorTransition> <>f__am$cache3;
        [CompilerGenerated]
        private static ReorderableList.HeaderCallbackDelegate <>f__mg$cache0;
        private AnimatorStateMachine m_ActiveStateMachine;
        private UnityEditor.Animations.AnimatorController m_Controller;
        private Editor m_Host;
        private bool m_LockList;
        private AnimatorState m_State;
        private AnimatorStateMachine m_StateMachine;
        private AnimatorTransitionInspectorBase m_TransitionInspector;
        private ReorderableList m_TransitionList;
        private List<TransitionEditionContext> m_Transitions;
        private UnityEditor.Graphs.AnimationStateMachine.TransitionType m_Type;

        public SourceNodeTransitionEditor(AnimatorState state, Editor host)
        {
            this.m_LockList = false;
            this.m_State = null;
            this.m_StateMachine = null;
            this.m_Type = UnityEditor.Graphs.AnimationStateMachine.TransitionType.eState;
            this.m_Host = host;
            this.m_State = state;
        }

        public SourceNodeTransitionEditor(UnityEditor.Graphs.AnimationStateMachine.TransitionType type, Editor host)
        {
            this.m_LockList = false;
            this.m_State = null;
            this.m_StateMachine = null;
            this.m_Type = type;
            this.m_Host = host;
        }

        public SourceNodeTransitionEditor(AnimatorStateMachine stateMachine, UnityEditor.Graphs.AnimationStateMachine.TransitionType type, Editor host)
        {
            this.m_LockList = false;
            this.m_State = null;
            this.m_StateMachine = null;
            this.m_Type = type;
            this.m_Host = host;
            this.m_StateMachine = stateMachine;
        }

        public void AcquireActiveStateMachine()
        {
            this.m_ActiveStateMachine = (AnimatorControllerTool.tool == null) ? null : ((this.m_Type != UnityEditor.Graphs.AnimationStateMachine.TransitionType.eAnyState) ? AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine : AnimatorControllerTool.tool.stateMachineGraph.rootStateMachine);
            if (this.m_ActiveStateMachine != null)
            {
                this.ResetTransitionList();
            }
        }

        public void AcquireController()
        {
            this.m_Controller = AnimatorControllerTool.tool?.animatorController;
            if (this.m_Controller != null)
            {
                this.m_Controller.OnAnimatorControllerDirty = (Action) Delegate.Combine(this.m_Controller.OnAnimatorControllerDirty, new Action(this.ResetTransitionList));
            }
        }

        private void GetTransitionContexts()
        {
            this.m_Transitions = new List<TransitionEditionContext>();
            switch (this.m_Type)
            {
                case UnityEditor.Graphs.AnimationStateMachine.TransitionType.eState:
                    if (this.m_State != null)
                    {
                        AnimatorStateTransition[] transitions = this.m_State.transitions;
                        foreach (AnimatorStateTransition transition2 in transitions)
                        {
                            this.m_Transitions.Add(new TransitionEditionContext(transition2, this.m_State, null, null));
                        }
                        break;
                    }
                    break;

                case UnityEditor.Graphs.AnimationStateMachine.TransitionType.eAnyState:
                    if (this.m_ActiveStateMachine != null)
                    {
                        AnimatorStateTransition[] anyStateTransitions = this.m_ActiveStateMachine.anyStateTransitions;
                        foreach (AnimatorStateTransition transition in anyStateTransitions)
                        {
                            this.m_Transitions.Add(new TransitionEditionContext(transition, null, null, this.m_ActiveStateMachine));
                        }
                    }
                    break;

                case UnityEditor.Graphs.AnimationStateMachine.TransitionType.eStateMachine:
                    if (this.m_ActiveStateMachine != null)
                    {
                        AnimatorTransition[] stateMachineTransitions = this.m_ActiveStateMachine.GetStateMachineTransitions(this.m_StateMachine);
                        foreach (AnimatorTransition transition3 in stateMachineTransitions)
                        {
                            this.m_Transitions.Add(new TransitionEditionContext(transition3, null, this.m_StateMachine, this.m_ActiveStateMachine));
                        }
                    }
                    break;

                case UnityEditor.Graphs.AnimationStateMachine.TransitionType.eEntry:
                    if (this.m_StateMachine != null)
                    {
                        AnimatorTransition[] entryTransitions = this.m_StateMachine.entryTransitions;
                        foreach (AnimatorTransition transition4 in entryTransitions)
                        {
                            this.m_Transitions.Add(new TransitionEditionContext(transition4, null, null, this.m_StateMachine));
                        }
                        break;
                    }
                    break;
            }
        }

        public bool HasPreviewGUI() => 
            ((this.m_TransitionInspector != null) && this.m_TransitionInspector.HasPreviewGUI());

        public void OnDestroy()
        {
            UnityEngine.Object.DestroyImmediate(this.m_TransitionInspector);
        }

        public void OnDisable()
        {
            AnimatorControllerTool.graphDirtyCallback = (Action) Delegate.Remove(AnimatorControllerTool.graphDirtyCallback, new Action(this.OnGraphDirty));
            if (this.m_Controller != null)
            {
                this.m_Controller.OnAnimatorControllerDirty = (Action) Delegate.Remove(this.m_Controller.OnAnimatorControllerDirty, new Action(this.ResetTransitionList));
            }
        }

        public void OnEnable()
        {
            AnimatorControllerTool.graphDirtyCallback = (Action) Delegate.Combine(AnimatorControllerTool.graphDirtyCallback, new Action(this.OnGraphDirty));
            this.AcquireActiveStateMachine();
            this.AcquireController();
        }

        private void OnGraphDirty()
        {
            this.ResetTransitionList();
        }

        public void OnInspectorGUI()
        {
            if (this.m_TransitionList == null)
            {
                this.ResetTransitionList();
            }
            if (this.m_ActiveStateMachine == null)
            {
                this.AcquireActiveStateMachine();
            }
            if (this.m_Controller == null)
            {
                this.AcquireController();
            }
            EditorGUI.BeginChangeCheck();
            this.m_TransitionList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                AnimatorControllerTool.tool.RebuildGraph();
                GUIUtility.ExitGUI();
            }
            if (this.m_TransitionInspector != null)
            {
                this.m_TransitionInspector.OnInspectorGUI();
            }
        }

        public void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            this.m_TransitionInspector.OnInteractivePreviewGUI(r, background);
        }

        public void OnPreviewSettings()
        {
            this.m_TransitionInspector.OnPreviewSettings();
        }

        private void OnTransitionElement(Rect rect, int index, bool selected, bool focused)
        {
            AnimatorTransitionInspectorBase.DrawTransitionElementCommon(rect, this.m_Transitions[index], selected, focused);
        }

        private void RemoveTransition(ReorderableList transitionList)
        {
            UnityEngine.Object.DestroyImmediate(this.m_TransitionInspector);
            this.m_Transitions[this.m_TransitionList.index].Remove(true);
            this.ResetTransitionList();
        }

        private void ReorderTransition(ReorderableList list)
        {
            this.m_LockList = true;
            switch (this.m_Type)
            {
                case UnityEditor.Graphs.AnimationStateMachine.TransitionType.eState:
                    Undo.RegisterCompleteObjectUndo(this.m_State, "Reorder transition");
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = t => t.transition as AnimatorStateTransition;
                    }
                    this.m_State.transitions = Array.ConvertAll<TransitionEditionContext, AnimatorStateTransition>(this.m_Transitions.ToArray(), <>f__am$cache1);
                    break;

                case UnityEditor.Graphs.AnimationStateMachine.TransitionType.eAnyState:
                    Undo.RegisterCompleteObjectUndo(this.m_ActiveStateMachine, "Reorder transition");
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = t => t.transition as AnimatorStateTransition;
                    }
                    this.m_ActiveStateMachine.anyStateTransitions = Array.ConvertAll<TransitionEditionContext, AnimatorStateTransition>(this.m_Transitions.ToArray(), <>f__am$cache0);
                    break;

                case UnityEditor.Graphs.AnimationStateMachine.TransitionType.eStateMachine:
                    Undo.RegisterCompleteObjectUndo(this.m_ActiveStateMachine, "Reorder transition");
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = t => t.transition as AnimatorTransition;
                    }
                    this.m_ActiveStateMachine.SetStateMachineTransitions(this.m_StateMachine, Array.ConvertAll<TransitionEditionContext, AnimatorTransition>(this.m_Transitions.ToArray(), <>f__am$cache2));
                    break;

                case UnityEditor.Graphs.AnimationStateMachine.TransitionType.eEntry:
                    Undo.RegisterCompleteObjectUndo(this.m_StateMachine, "Reorder transition");
                    if (<>f__am$cache3 == null)
                    {
                        <>f__am$cache3 = t => t.transition as AnimatorTransition;
                    }
                    this.m_StateMachine.entryTransitions = Array.ConvertAll<TransitionEditionContext, AnimatorTransition>(this.m_Transitions.ToArray(), <>f__am$cache3);
                    break;
            }
            this.m_LockList = false;
        }

        private void ResetTransitionList()
        {
            if (!this.m_LockList)
            {
                this.GetTransitionContexts();
                this.m_TransitionList = new ReorderableList(this.m_Transitions, typeof(TransitionEditionContext), true, true, false, true);
                if (<>f__mg$cache0 == null)
                {
                    <>f__mg$cache0 = new ReorderableList.HeaderCallbackDelegate(AnimatorTransitionInspectorBase.DrawTransitionHeaderCommon);
                }
                this.m_TransitionList.drawHeaderCallback = <>f__mg$cache0;
                this.m_TransitionList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.OnTransitionElement);
                this.m_TransitionList.onSelectCallback = new ReorderableList.SelectCallbackDelegate(this.SelectTransition);
                this.m_TransitionList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.ReorderTransition);
                this.m_TransitionList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemoveTransition);
                this.m_TransitionList.displayAdd = false;
                this.m_Host.Repaint();
            }
        }

        private void SelectTransition(ReorderableList list)
        {
            AnimatorTransitionBase transition = this.m_Transitions[list.index].transition;
            if (transition != null)
            {
                if ((this.m_Type == UnityEditor.Graphs.AnimationStateMachine.TransitionType.eState) || (this.m_Type == UnityEditor.Graphs.AnimationStateMachine.TransitionType.eAnyState))
                {
                    this.m_TransitionInspector = Editor.CreateEditor(transition as AnimatorStateTransition) as AnimatorStateTransitionInspector;
                }
                else
                {
                    this.m_TransitionInspector = Editor.CreateEditor(transition as AnimatorTransition) as AnimatorTransitionInspector;
                }
                this.m_TransitionInspector.SetTransitionContext(this.m_Transitions[list.index]);
                this.m_TransitionInspector.showTransitionList = false;
            }
        }
    }
}

