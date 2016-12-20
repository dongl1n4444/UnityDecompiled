namespace UnityEditor.EditorTestsRunner
{
    using System;
    using UnityEditor.EditorTests;

    internal class EditorTestResultGroup : EditorTestResultBase
    {
        private bool m_RefreshResults = true;
        private TestResultState? m_ResultState;

        public EditorTestResultGroup(EditorTestResultGroup parent)
        {
            base.SetParent(parent);
        }

        public void InvalidateGroup(TestResultState? newResult)
        {
            if (!this.m_ResultState.HasValue || ((this.m_ResultState.Value != newResult.GetValueOrDefault()) || !newResult.HasValue))
            {
                this.m_RefreshResults = true;
                if (base.m_ParentGroup != null)
                {
                    base.m_ParentGroup.InvalidateGroup(newResult);
                }
            }
        }

        public void UpdateGroup()
        {
            this.m_RefreshResults = false;
            TestResultState? nullable2 = null;
            TestResultState? nullable = nullable2;
            foreach (EditorTestResultBase base2 in base.m_Children)
            {
                nullable2 = null;
                TestResultState? resultState = nullable2;
                if (base2 is EditorTestResult)
                {
                    EditorTestResult result = base2 as EditorTestResult;
                    if (result.executed)
                    {
                        resultState = new TestResultState?(result.resultState);
                    }
                }
                else if (base2 is EditorTestResultGroup)
                {
                    resultState = (base2 as EditorTestResultGroup).resultState;
                }
                if (resultState.HasValue)
                {
                    switch (resultState.Value)
                    {
                        case TestResultState.Success:
                            nullable = 4;
                            break;

                        case TestResultState.Failure:
                        case TestResultState.Error:
                            nullable = 5;
                            goto Label_0115;

                        case TestResultState.Inconclusive:
                            nullable = 0;
                            break;

                        case TestResultState.Ignored:
                            nullable = 3;
                            break;
                    }
                }
            }
        Label_0115:
            this.m_ResultState = nullable;
        }

        public TestResultState? resultState
        {
            get
            {
                if (this.m_RefreshResults)
                {
                    this.UpdateGroup();
                }
                return this.m_ResultState;
            }
        }
    }
}

