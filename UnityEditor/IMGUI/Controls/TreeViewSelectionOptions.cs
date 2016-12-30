namespace UnityEditor.IMGUI.Controls
{
    using System;

    /// <summary>
    /// <para>Enum used by the TreeView.SetSelection method.</para>
    /// </summary>
    [Flags]
    public enum TreeViewSelectionOptions
    {
        None,
        FireSelectionChanged,
        RevealAndFrame
    }
}

