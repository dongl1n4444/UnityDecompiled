namespace UnityEngine.UI
{
    using System;

    public interface ILayoutIgnorer
    {
        /// <summary>
        /// <para>Should this RectTransform be ignored bvy the layout system?</para>
        /// </summary>
        bool ignoreLayout { get; }
    }
}

