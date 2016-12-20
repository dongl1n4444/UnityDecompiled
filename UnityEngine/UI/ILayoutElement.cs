namespace UnityEngine.UI
{
    using System;

    public interface ILayoutElement
    {
        /// <summary>
        /// <para>The minWidth, preferredWidth, and flexibleWidth values may be calculated in this callback.</para>
        /// </summary>
        void CalculateLayoutInputHorizontal();
        /// <summary>
        /// <para>The minHeight, preferredHeight, and flexibleHeight values may be calculated in this callback.</para>
        /// </summary>
        void CalculateLayoutInputVertical();

        /// <summary>
        /// <para>The extra relative height this layout element should be allocated if there is additional available space.</para>
        /// </summary>
        float flexibleHeight { get; }

        /// <summary>
        /// <para>The extra relative width this layout element should be allocated if there is additional available space.</para>
        /// </summary>
        float flexibleWidth { get; }

        /// <summary>
        /// <para>The layout priority of this component.</para>
        /// </summary>
        int layoutPriority { get; }

        /// <summary>
        /// <para>The minimum height this layout element may be allocated.</para>
        /// </summary>
        float minHeight { get; }

        /// <summary>
        /// <para>The minimum width this layout element may be allocated.</para>
        /// </summary>
        float minWidth { get; }

        /// <summary>
        /// <para>The preferred height this layout element should be allocated if there is sufficient space.</para>
        /// </summary>
        float preferredHeight { get; }

        /// <summary>
        /// <para>The preferred width this layout element should be allocated if there is sufficient space.</para>
        /// </summary>
        float preferredWidth { get; }
    }
}

