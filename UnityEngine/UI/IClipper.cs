namespace UnityEngine.UI
{
    using System;

    public interface IClipper
    {
        /// <summary>
        /// <para>Called after layout and before Graphic update of the Canvas update loop.</para>
        /// </summary>
        void PerformClipping();
    }
}

