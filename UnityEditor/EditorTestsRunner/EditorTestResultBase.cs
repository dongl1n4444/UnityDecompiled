namespace UnityEditor.EditorTestsRunner
{
    using System;
    using System.Collections.Generic;

    internal abstract class EditorTestResultBase
    {
        protected List<EditorTestResultBase> m_Children = new List<EditorTestResultBase>();
        protected Action<EditorTestResult> m_OnResultUpdate;
        protected EditorTestResultGroup m_ParentGroup;

        protected EditorTestResultBase()
        {
        }

        public void SetParent(EditorTestResultGroup parent)
        {
            this.m_ParentGroup = parent;
            if (this.m_ParentGroup != null)
            {
                this.m_ParentGroup.m_Children.Add(this);
            }
        }

        public void SetResultChangedCallback(Action<EditorTestResult> resultUpdated)
        {
            this.m_OnResultUpdate = resultUpdated;
        }
    }
}

