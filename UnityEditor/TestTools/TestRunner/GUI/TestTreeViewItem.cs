namespace UnityEditor.TestTools.TestRunner.GUI
{
    using NUnit.Framework.Internal;
    using System;
    using System.Reflection;
    using System.Text;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine.TestTools.TestRunner.GUI;

    internal class TestTreeViewItem : TreeViewItem
    {
        private const int k_ResultTestMaxLength = 0x3a98;
        internal Test m_Test;
        public MethodInfo method;
        public TestRunnerResult result;
        public Type type;

        public TestTreeViewItem(Test test, int depth, TreeViewItem parent) : base(GetId(test), depth, parent, test.Name)
        {
            this.m_Test = test;
            if (test.TypeInfo != null)
            {
                this.type = test.TypeInfo.Type;
            }
            if (test.Method != null)
            {
                this.method = test.Method.MethodInfo;
            }
            string name = test.Name;
            if (name.Length > 100)
            {
                name = name.Substring(0, 100);
            }
            name = name.Replace("\n", "");
            this.displayName = name;
            this.icon = Icons.s_UnknownImg;
        }

        private static int GetId(Test test) => 
            TestRunnerResult.GetId(test).GetHashCode();

        public string GetResultText()
        {
            string str = $"{this.result.duration:0.000}";
            StringBuilder builder = new StringBuilder($"{this.displayName.Trim()} ({str}s)");
            if (!string.IsNullOrEmpty(this.result.description))
            {
                builder.AppendFormat("\n{0}", this.result.description);
            }
            if (!string.IsNullOrEmpty(this.result.messages))
            {
                builder.Append("\n---\n");
                builder.Append(this.result.messages.Trim());
            }
            if (!string.IsNullOrEmpty(this.result.stacktrace))
            {
                builder.Append("\n---\n");
                builder.Append(this.result.stacktrace.Trim());
            }
            if (builder.Length > 0x3a98)
            {
                builder.Length = 0x3a98;
                builder.AppendFormat("...\n\n---MESSAGE TRUNCATED AT {0} CHARACTERS---", 0x3a98);
            }
            return builder.ToString().Trim();
        }

        private void ResultUpdated(TestRunnerResult result)
        {
            switch (result.resultStatus)
            {
                case TestRunnerResult.ResultStatus.Passed:
                    this.icon = Icons.s_SuccessImg;
                    break;

                case TestRunnerResult.ResultStatus.Failed:
                    this.icon = Icons.s_FailImg;
                    break;

                case TestRunnerResult.ResultStatus.Inconclusive:
                    this.icon = Icons.s_InconclusiveImg;
                    break;

                case TestRunnerResult.ResultStatus.Skipped:
                    this.icon = Icons.s_IgnoreImg;
                    break;

                default:
                    if (result.ignoredOrSkipped)
                    {
                        this.icon = Icons.s_IgnoreImg;
                    }
                    else if (result.notRunnable)
                    {
                        this.icon = Icons.s_FailImg;
                    }
                    else
                    {
                        this.icon = Icons.s_UnknownImg;
                    }
                    break;
            }
        }

        public void SetResult(TestRunnerResult result)
        {
            this.result = result;
            result.SetResultChangedCallback(new Action<TestRunnerResult>(this.ResultUpdated));
            this.ResultUpdated(result);
        }

        public string FullName =>
            this.m_Test.FullName;

        public bool IsGroupNode =>
            this.m_Test.IsSuite;
    }
}

