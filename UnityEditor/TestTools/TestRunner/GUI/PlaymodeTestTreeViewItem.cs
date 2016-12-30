namespace UnityEditor.TestTools.TestRunner.GUI
{
    using NUnit.Framework.Internal;
    using System;
    using System.Text;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine.TestTools.TestRunner.GUI;

    internal abstract class PlaymodeTestTreeViewItem : TreeViewItem
    {
        public string fullName;
        private const int k_ResultTestMaxLength = 0x3a98;
        internal Test m_Test;
        public string name;
        public TestRunnerResult result;

        protected PlaymodeTestTreeViewItem(Test test, int depth, TreeViewItem parent) : base(test.FullName.GetHashCode(), depth, parent, test.Name)
        {
            this.m_Test = test;
            this.name = test.Name;
            this.fullName = test.FullName;
            if (this.name.Length > 100)
            {
                this.name = this.name.Substring(0, 100);
            }
            this.name = this.name.Replace("\n", "");
            this.displayName = this.name;
            this.icon = Icons.s_UnknownImg;
        }

        public string GetResultText()
        {
            string str = $"{this.result.duration:0.000}";
            StringBuilder builder = new StringBuilder($"{this.name.Trim()} ({str}s)");
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

        protected virtual void ResultUpdated(TestRunnerResult result)
        {
        }

        public void SetResult(TestRunnerResult result)
        {
            this.result = result;
            result.SetResultChangedCallback(new Action<TestRunnerResult>(this.ResultUpdated));
            this.ResultUpdated(result);
        }
    }
}

