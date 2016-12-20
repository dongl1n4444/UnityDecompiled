namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(AnyStateNode))]
    internal class AnyStateNodeInspector : Editor
    {
        private SourceNodeTransitionEditor m_TransitionsEditor = null;

        public override bool HasPreviewGUI()
        {
            return ((this.m_TransitionsEditor != null) && this.m_TransitionsEditor.HasPreviewGUI());
        }

        private void Init()
        {
            if (this.m_TransitionsEditor == null)
            {
                this.m_TransitionsEditor = new SourceNodeTransitionEditor(UnityEditor.Graphs.AnimationStateMachine.TransitionType.eAnyState, this);
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
            this.m_TransitionsEditor.OnInspectorGUI();
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            if (this.m_TransitionsEditor != null)
            {
                this.m_TransitionsEditor.OnInteractivePreviewGUI(r, background);
            }
        }

        public override void OnPreviewSettings()
        {
            if (this.m_TransitionsEditor != null)
            {
                this.m_TransitionsEditor.OnPreviewSettings();
            }
        }
    }
}

