namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEngine;

    [CustomEditor(typeof(AnimatorStateTransition)), CanEditMultipleObjects]
    internal class AnimatorStateTransitionInspector : AnimatorTransitionInspectorBase
    {
        protected int m_AnyStateSourceIndex = 0;
        protected SerializedProperty m_CanTransitionToSelf;
        protected int m_DstStateIndex = 0;
        protected HashSet<AnimatorStateMachine> m_DstStateMachines;
        protected List<AnimatorState> m_DstStates;
        protected SerializedProperty m_Duration;
        protected SerializedProperty m_ExitTime;
        protected SerializedProperty m_HasExitTime;
        protected SerializedProperty m_HasFixedDuration;
        protected SerializedProperty m_InterruptionSource;
        protected string m_InvalidTransitionMessage;
        protected SerializedProperty m_Offset;
        protected SerializedProperty m_OrderedInterruption;
        protected static bool m_ShowSettings;
        protected TransitionPreview m_TransitionPreview;
        private static Styles styles;

        private void BuildDestinationStatesRecursive(AnimatorTransitionBase transition, AnimatorStateMachine rootStateMachine, AnimatorStateMachine currentStateMachine)
        {
            if (transition.destinationState != null)
            {
                this.m_DstStates.Add(transition.destinationState);
            }
            else if (transition.isExit)
            {
                AnimatorStateMachine machine = rootStateMachine.FindParent(currentStateMachine);
                if (machine != null)
                {
                    AnimatorTransition[] stateMachineTransitions = machine.GetStateMachineTransitions(currentStateMachine);
                    foreach (AnimatorTransition transition2 in stateMachineTransitions)
                    {
                        this.BuildDestinationStatesRecursive(transition2, rootStateMachine, machine);
                    }
                }
            }
            else if ((transition.destinationStateMachine != null) && !this.m_DstStateMachines.Contains(transition.destinationStateMachine))
            {
                this.m_DstStateMachines.Add(transition.destinationStateMachine);
                if (transition.destinationStateMachine.defaultState != null)
                {
                    this.m_DstStates.Add(transition.destinationStateMachine.defaultState);
                }
                AnimatorTransition[] entryTransitions = transition.destinationStateMachine.entryTransitions;
                foreach (AnimatorTransition transition3 in entryTransitions)
                {
                    this.BuildDestinationStatesRecursive(transition3, rootStateMachine, transition.destinationStateMachine);
                }
            }
        }

        protected override void ControllerDirty()
        {
            if (this.m_TransitionPreview != null)
            {
                this.m_TransitionPreview.mustResample = true;
            }
        }

        protected override void DoErrorAndWarning()
        {
            if (!this.m_HasExitTime.boolValue && (base.m_Conditions.arraySize == 0))
            {
                EditorGUILayout.HelpBox("Transition needs at least one condition or an Exit Time to be valid, otherwise it will be ignored.", MessageType.Warning, true);
            }
        }

        protected override void DoPreview()
        {
            this.Init();
            AnimatorStateTransition targetObject = base.m_SerializedTransition.targetObject as AnimatorStateTransition;
            AnimatorState sourceState = this.GetSourceState((base.m_TransitionList == null) ? 0 : base.m_TransitionList.index);
            AnimatorState destinationState = targetObject.destinationState;
            EditorGUILayout.PropertyField(this.m_HasExitTime, styles.hasExitTime, new GUILayoutOption[0]);
            m_ShowSettings = EditorGUILayout.Foldout(m_ShowSettings, "Settings");
            if (m_ShowSettings)
            {
                EditorGUI.indentLevel++;
                bool enabled = GUI.enabled;
                GUI.enabled = this.m_HasExitTime.boolValue;
                EditorGUILayout.PropertyField(this.m_ExitTime, styles.exitTime, new GUILayoutOption[0]);
                GUI.enabled = enabled;
                EditorGUILayout.PropertyField(this.m_HasFixedDuration, styles.hasFixedDuration, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_Duration, !this.m_HasFixedDuration.boolValue ? styles.transitionDurationNormalized : styles.transitionDurationFixed, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_Offset, styles.transitionOffset, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_InterruptionSource, styles.interruptionSource, new GUILayoutOption[0]);
                TransitionInterruptionSource enumValueIndex = (TransitionInterruptionSource) this.m_InterruptionSource.enumValueIndex;
                GUI.enabled = ((enumValueIndex == TransitionInterruptionSource.Source) || (enumValueIndex == TransitionInterruptionSource.SourceThenDestination)) || (enumValueIndex == TransitionInterruptionSource.DestinationThenSource);
                EditorGUILayout.PropertyField(this.m_OrderedInterruption, styles.orderedInterruption, new GUILayoutOption[0]);
                GUI.enabled = enabled;
                if ((sourceState == null) && (destinationState != null))
                {
                    EditorGUILayout.PropertyField(this.m_CanTransitionToSelf, new GUILayoutOption[0]);
                }
                EditorGUI.indentLevel--;
            }
            if (this.IsPreviewable())
            {
                AnimatorStateMachine stateMachine = base.m_Controller.layers[base.m_LayerIndex].stateMachine;
                if (sourceState == null)
                {
                    List<ChildAnimatorState> statesRecursive = stateMachine.statesRecursive;
                    string[] displayedOptions = new string[statesRecursive.Count];
                    int num = 0;
                    foreach (ChildAnimatorState state3 in statesRecursive)
                    {
                        displayedOptions[num++] = state3.state.name;
                    }
                    EditorGUILayout.Space();
                    this.m_AnyStateSourceIndex = EditorGUILayout.Popup("Preview source state", this.m_AnyStateSourceIndex, displayedOptions, new GUILayoutOption[0]);
                    EditorGUILayout.Space();
                    ChildAnimatorState state4 = statesRecursive[this.m_AnyStateSourceIndex];
                    sourceState = state4.state;
                }
                if (destinationState == null)
                {
                    if (this.m_DstStates.Count > 0)
                    {
                        string[] strArray2 = new string[this.m_DstStates.Count];
                        int num2 = 0;
                        foreach (AnimatorState state5 in this.m_DstStates)
                        {
                            strArray2[num2++] = state5.name;
                        }
                        EditorGUILayout.Space();
                        this.m_DstStateIndex = EditorGUILayout.Popup("Preview destination state", this.m_DstStateIndex, strArray2, new GUILayoutOption[0]);
                        if (this.m_DstStates.Count > this.m_DstStateIndex)
                        {
                            destinationState = this.m_DstStates[this.m_DstStateIndex];
                        }
                        else
                        {
                            destinationState = null;
                        }
                        EditorGUILayout.Space();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Cannot preview transition, there is no destination state", MessageType.Warning, true);
                    }
                }
                if ((sourceState != null) && (destinationState != null))
                {
                    this.m_TransitionPreview.SetTransition(targetObject, sourceState, destinationState, base.m_Controller.layers[base.m_LayerIndex], base.m_PreviewObject);
                    this.m_TransitionPreview.DoTransitionPreview();
                }
            }
            else
            {
                EditorGUILayout.HelpBox(this.m_InvalidTransitionMessage, MessageType.Warning, true);
            }
        }

        protected AnimatorState GetSourceState(int index)
        {
            return ((base.m_TransitionContexts[index] == null) ? null : base.m_TransitionContexts[index].sourceState);
        }

        public override bool HasPreviewGUI()
        {
            return ((this.m_TransitionPreview != null) && this.m_TransitionPreview.HasPreviewGUI());
        }

        private void Init()
        {
            if (styles == null)
            {
                styles = new Styles();
            }
        }

        protected override void InitSerializedProperties()
        {
            base.InitSerializedProperties();
            this.m_InterruptionSource = base.m_SerializedTransition.FindProperty("m_InterruptionSource");
            this.m_OrderedInterruption = base.m_SerializedTransition.FindProperty("m_OrderedInterruption");
            this.m_Duration = base.m_SerializedTransition.FindProperty("m_TransitionDuration");
            this.m_Offset = base.m_SerializedTransition.FindProperty("m_TransitionOffset");
            this.m_ExitTime = base.m_SerializedTransition.FindProperty("m_ExitTime");
            this.m_HasExitTime = base.m_SerializedTransition.FindProperty("m_HasExitTime");
            this.m_HasFixedDuration = base.m_SerializedTransition.FindProperty("m_HasFixedDuration");
            this.m_CanTransitionToSelf = base.m_SerializedTransition.FindProperty("m_CanTransitionToSelf");
        }

        protected bool IsPreviewable()
        {
            this.m_InvalidTransitionMessage = "";
            AnimatorStateTransition targetObject = base.m_SerializedTransition.targetObject as AnimatorStateTransition;
            if (base.m_Controller == null)
            {
                this.m_InvalidTransitionMessage = "Cannot preview transition: need an AnimatorController";
                return false;
            }
            AnimatorState sourceState = this.GetSourceState(0);
            Motion motion = (sourceState == null) ? null : base.m_Controller.GetStateEffectiveMotion(sourceState, base.m_LayerIndex);
            Motion motion2 = (targetObject.destinationState == null) ? null : base.m_Controller.GetStateEffectiveMotion(targetObject.destinationState, base.m_LayerIndex);
            bool flag2 = sourceState == null;
            if (((sourceState != null) && (sourceState.speed > -0.01f)) && (sourceState.speed < 0.01f))
            {
                this.m_InvalidTransitionMessage = "Cannot preview transition: source state has a speed between -0.01 and 0.01";
                return false;
            }
            if (((targetObject.destinationState != null) && (targetObject.destinationState.speed > -0.01f)) && (targetObject.destinationState.speed < 0.01f))
            {
                this.m_InvalidTransitionMessage = "Cannot preview transition: destination state has a speed between -0.01 and 0.01";
                return false;
            }
            if (base.m_LayerIndex == 0)
            {
                if (!flag2)
                {
                    if (motion == null)
                    {
                        this.m_InvalidTransitionMessage = "Cannot preview transition: source state does not have motion";
                        return false;
                    }
                }
                else if (motion2 == null)
                {
                    this.m_InvalidTransitionMessage = "Cannot preview AnyState transition:  destination state does not have motion";
                    return false;
                }
            }
            else if ((motion == null) && (motion2 == null))
            {
                this.m_InvalidTransitionMessage = "Cannot preview transition, must at least have a motion on either source or destination state";
                return false;
            }
            return true;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (this.m_TransitionPreview != null)
            {
                this.m_TransitionPreview.OnDestroy();
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (this.m_TransitionPreview != null)
            {
                this.m_TransitionPreview.OnDisable();
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_TransitionPreview = new TransitionPreview();
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            if (this.m_TransitionPreview != null)
            {
                this.m_TransitionPreview.OnInteractivePreviewGUI(r, background);
            }
        }

        public override void OnPreviewSettings()
        {
            if (this.m_TransitionPreview != null)
            {
                this.m_TransitionPreview.OnPreviewSettings();
            }
        }

        protected override void SetTransitionToInspect(AnimatorTransitionBase transition)
        {
            base.SetTransitionToInspect(transition);
            if (base.m_Controller != null)
            {
                AnimatorStateMachine stateMachine = base.m_Controller.layers[base.m_LayerIndex].stateMachine;
                AnimatorState sourceState = this.GetSourceState(base.m_TransitionList.index);
                if (sourceState != null)
                {
                    this.m_DstStates = new List<AnimatorState>();
                    this.m_DstStateMachines = new HashSet<AnimatorStateMachine>();
                    this.BuildDestinationStatesRecursive(transition, stateMachine, sourceState.FindParent(stateMachine));
                }
            }
        }

        private class Styles
        {
            public GUIContent exitTime = new GUIContent(EditorGUIUtility.TextContent("Exit Time|Exit time in normalized time from current state"));
            public GUIContent hasExitTime = new GUIContent(EditorGUIUtility.TextContent("Has Exit Time|Transition has a fixed exit time"));
            public GUIContent hasFixedDuration = new GUIContent(EditorGUIUtility.TextContent("Fixed Duration | Transition duration is independent of state length"));
            public GUIContent interruptionSource = new GUIContent(EditorGUIUtility.TextContent("Interruption Source|Can be interrupted by transitions from"));
            public GUIContent orderedInterruption = new GUIContent(EditorGUIUtility.TextContent("Ordered Interruption|Can only be interrupted by higher priority transitions"));
            public GUIContent transitionDurationFixed = new GUIContent(EditorGUIUtility.TextContent("Transition Duration (s) |Transition duration in seconds"));
            public GUIContent transitionDurationNormalized = new GUIContent(EditorGUIUtility.TextContent("Transition Duration (%) |Transition duration in normalized time from current state"));
            public GUIContent transitionOffset = new GUIContent(EditorGUIUtility.TextContent("Transition Offset|Normalized start time in the next state"));
        }
    }
}

