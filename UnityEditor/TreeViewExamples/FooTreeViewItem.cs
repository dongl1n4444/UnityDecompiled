namespace UnityEditor.TreeViewExamples
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor.IMGUI.Controls;

    internal class FooTreeViewItem : TreeViewItem
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BackendData.Foo <foo>k__BackingField;

        public FooTreeViewItem(int id, int depth, TreeViewItem parent, string displayName, BackendData.Foo foo) : base(id, depth, parent, displayName)
        {
            this.foo = foo;
        }

        public BackendData.Foo foo { get; private set; }
    }
}

