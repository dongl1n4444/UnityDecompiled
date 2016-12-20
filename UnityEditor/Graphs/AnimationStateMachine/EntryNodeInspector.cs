namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEngine;

    [CustomEditor(typeof(EntryNode))]
    internal class EntryNodeInspector : Editor
    {
        private SerializedProperty m_DefaultState;
        private SourceNodeTransitionEditor m_TransitionsEditor = null;

        private void Init()
        {
            if (this.m_TransitionsEditor == null)
            {
                this.m_TransitionsEditor = new SourceNodeTransitionEditor((base.target as EntryNode).undoableObject as AnimatorStateMachine, UnityEditor.Graphs.AnimationStateMachine.TransitionType.eEntry, this);
            }
        }

        public void OnDestroy()
        {
            this.m_TransitionsEditor.OnDestroy();
        }

        public void OnDisable()
        {
            this.m_TransitionsEditor.OnDisable();
        }

        public void OnEnable()
        {
            this.Init();
            this.m_TransitionsEditor.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            this.Init();
            GUI.enabled = true;
            EntryNode target = base.target as EntryNode;
            if (target.stateMachine != null)
            {
                AnimatorState defaultState = target.stateMachine.defaultState;
                EditorGUILayout.LabelField("Default state ", (defaultState == null) ? "Not set" : defaultState.name, EditorStyles.boldLabel, new GUILayoutOption[0]);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                this.m_TransitionsEditor.OnInspectorGUI();
            }
        }
    }
}

