namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Types of UnityGUI input and processing events.</para>
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// <para>User has right-clicked (or control-clicked on the mac).</para>
        /// </summary>
        ContextClick = 0x10,
        /// <summary>
        /// <para>Editor only: drag &amp; drop operation exited.</para>
        /// </summary>
        DragExited = 15,
        dragPerform = 10,
        /// <summary>
        /// <para>Editor only: drag &amp; drop operation performed.</para>
        /// </summary>
        DragPerform = 10,
        dragUpdated = 9,
        /// <summary>
        /// <para>Editor only: drag &amp; drop operation updated.</para>
        /// </summary>
        DragUpdated = 9,
        /// <summary>
        /// <para>Execute a special command (eg. copy &amp; paste).</para>
        /// </summary>
        ExecuteCommand = 14,
        ignore = 11,
        /// <summary>
        /// <para>Event should be ignored.</para>
        /// </summary>
        Ignore = 11,
        keyDown = 4,
        /// <summary>
        /// <para>A keyboard key was pressed.</para>
        /// </summary>
        KeyDown = 4,
        keyUp = 5,
        /// <summary>
        /// <para>A keyboard key was released.</para>
        /// </summary>
        KeyUp = 5,
        layout = 8,
        /// <summary>
        /// <para>A layout event.</para>
        /// </summary>
        Layout = 8,
        mouseDown = 0,
        /// <summary>
        /// <para>Mouse button was pressed.</para>
        /// </summary>
        MouseDown = 0,
        mouseDrag = 3,
        /// <summary>
        /// <para>Mouse was dragged.</para>
        /// </summary>
        MouseDrag = 3,
        /// <summary>
        /// <para>Mouse entered a window (Editor views only).</para>
        /// </summary>
        MouseEnterWindow = 20,
        /// <summary>
        /// <para>Mouse left a window (Editor views only).</para>
        /// </summary>
        MouseLeaveWindow = 0x15,
        mouseMove = 2,
        /// <summary>
        /// <para>Mouse was moved (Editor views only).</para>
        /// </summary>
        MouseMove = 2,
        mouseUp = 1,
        /// <summary>
        /// <para>Mouse button was released.</para>
        /// </summary>
        MouseUp = 1,
        repaint = 7,
        /// <summary>
        /// <para>A repaint event. One is sent every frame.</para>
        /// </summary>
        Repaint = 7,
        scrollWheel = 6,
        /// <summary>
        /// <para>The scroll wheel was moved.</para>
        /// </summary>
        ScrollWheel = 6,
        used = 12,
        /// <summary>
        /// <para>Already processed event.</para>
        /// </summary>
        Used = 12,
        /// <summary>
        /// <para>Validates a special command (e.g. copy &amp; paste).</para>
        /// </summary>
        ValidateCommand = 13
    }
}

