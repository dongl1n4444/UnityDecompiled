namespace UnityEngine.UI
{
    using System;

    public interface ILayoutController
    {
        /// <summary>
        /// <para>Callback invoked by the auto layout system which handles horizontal aspects of the layout.</para>
        /// </summary>
        void SetLayoutHorizontal();
        /// <summary>
        /// <para>Callback invoked by the auto layout system which handles vertical aspects of the layout.</para>
        /// </summary>
        void SetLayoutVertical();
    }
}

