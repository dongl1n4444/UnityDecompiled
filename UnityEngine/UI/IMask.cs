namespace UnityEngine.UI
{
    using System;
    using UnityEngine;

    [Obsolete("Not supported anymore.", true)]
    public interface IMask
    {
        /// <summary>
        /// <para>Is the mask enabled.</para>
        /// </summary>
        bool Enabled();

        /// <summary>
        /// <para>Return the RectTransform associated with this mask.</para>
        /// </summary>
        RectTransform rectTransform { get; }
    }
}

