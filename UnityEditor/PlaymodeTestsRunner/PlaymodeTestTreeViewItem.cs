namespace UnityEditor.PlaymodeTestsRunner
{
    using System;
    using System.Text;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine.PlaymodeTestsRunner;

    internal abstract class PlaymodeTestTreeViewItem : TreeViewItem
    {
        public string fullName;
        private const int k_ResultTestMaxLength = 0x3a98;
        public string name;
        public TestResult result;

        protected PlaymodeTestTreeViewItem(string name, string fullName, int depth, TreeViewItem parent) : base(fullName.GetHashCode(), depth, parent, name)
        {
            if (name.Length > 100)
            {
                name = name.Substring(0, 100);
            }
            this.name = name.Replace("\n", "");
            this.displayName = this.name;
            this.icon = Icons.s_UnknownImg;
            this.name = name;
            this.fullName = fullName;
        }

        private static int CreateId(string fullName) => 
            fullName.GetHashCode();

        public string GetResultText()
        {
            StringBuilder builder = new StringBuilder(this.name.Trim());
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

        public virtual bool IsVisible(FilteringOptions options) => 
            true;

        protected virtual void ResultUpdated(TestResult result)
        {
        }

        public void SetResult(TestResult result)
        {
            this.result = result;
            result.SetResultChangedCallback(new Action<TestResult>(this.ResultUpdated));
            this.ResultUpdated(result);
        }
    }
}

