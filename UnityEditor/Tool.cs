namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Which tool is active in the editor.</para>
    /// </summary>
    public enum Tool
    {
        /// <summary>
        /// <para>The move tool is active.</para>
        /// </summary>
        Move = 1,
        /// <summary>
        /// <para>No tool is active. Set this to implement your own in-inspector toolbar (like the terrain editor does).</para>
        /// </summary>
        None = -1,
        /// <summary>
        /// <para>The rect tool is active.</para>
        /// </summary>
        Rect = 4,
        /// <summary>
        /// <para>The rotate tool is active.</para>
        /// </summary>
        Rotate = 2,
        /// <summary>
        /// <para>The scale tool is active.</para>
        /// </summary>
        Scale = 3,
        /// <summary>
        /// <para>The view tool is active - Use Tools.viewTool to find out which view tool we're talking about.</para>
        /// </summary>
        View = 0
    }
}

