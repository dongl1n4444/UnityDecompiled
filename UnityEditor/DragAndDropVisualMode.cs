namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Visual indication mode for Drag &amp; Drop operation.</para>
    /// </summary>
    public enum DragAndDropVisualMode
    {
        /// <summary>
        /// <para>Copy dragged objects.</para>
        /// </summary>
        Copy = 1,
        /// <summary>
        /// <para>Generic drag operation.</para>
        /// </summary>
        Generic = 4,
        /// <summary>
        /// <para>Link dragged objects to target.</para>
        /// </summary>
        Link = 2,
        /// <summary>
        /// <para>Move dragged objects.</para>
        /// </summary>
        Move = 0x10,
        /// <summary>
        /// <para>No indication (drag should not be performed).</para>
        /// </summary>
        None = 0,
        /// <summary>
        /// <para>Rejected drag operation.</para>
        /// </summary>
        Rejected = 0x20
    }
}

