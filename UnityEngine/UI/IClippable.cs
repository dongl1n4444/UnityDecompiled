namespace UnityEngine.UI
{
    using System;
    using UnityEngine;

    public interface IClippable
    {
        /// <summary>
        /// <para>Clip and cull the IClippable given the clipRect.</para>
        /// </summary>
        /// <param name="clipRect">Rectangle to clip against.</param>
        /// <param name="validRect">Is the Rect valid. If not then the rect has 0 size.</param>
        void Cull(Rect clipRect, bool validRect);
        /// <summary>
        /// <para>Called when the state of a parent IClippable changes.</para>
        /// </summary>
        void RecalculateClipping();
        /// <summary>
        /// <para>Set the clip rect for the IClippable.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validRect"></param>
        void SetClipRect(Rect value, bool validRect);

        /// <summary>
        /// <para>GameObject of the IClippable.</para>
        /// </summary>
        GameObject gameObject { get; }

        /// <summary>
        /// <para>RectTransform of the clippable.</para>
        /// </summary>
        RectTransform rectTransform { get; }
    }
}

