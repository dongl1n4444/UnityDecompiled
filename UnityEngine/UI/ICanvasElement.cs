namespace UnityEngine.UI
{
    using System;
    using UnityEngine;

    public interface ICanvasElement
    {
        /// <summary>
        /// <para>Callback sent when this ICanvasElement has completed Graphic rebuild.</para>
        /// </summary>
        void GraphicUpdateComplete();
        /// <summary>
        /// <para>Return true if the element is considered destroyed.
        /// Used if the native representation has been destroyed.</para>
        /// </summary>
        bool IsDestroyed();
        /// <summary>
        /// <para>Callback sent when this ICanvasElement has completed layout.</para>
        /// </summary>
        void LayoutComplete();
        /// <summary>
        /// <para>Rebuild the element for the given stage.</para>
        /// </summary>
        /// <param name="executing">Stage being rebuild.</param>
        void Rebuild(CanvasUpdate executing);

        /// <summary>
        /// <para>Get the transform associated with the ICanvasElement.</para>
        /// </summary>
        Transform transform { get; }
    }
}

