namespace UnityEngine.UI
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Layout child layout elements below each other.</para>
    /// </summary>
    [AddComponentMenu("Layout/Vertical Layout Group", 0x97)]
    public class VerticalLayoutGroup : HorizontalOrVerticalLayoutGroup
    {
        protected VerticalLayoutGroup()
        {
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            base.CalcAlongAxis(0, true);
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public override void CalculateLayoutInputVertical()
        {
            base.CalcAlongAxis(1, true);
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            base.SetChildrenAlongAxis(0, true);
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public override void SetLayoutVertical()
        {
            base.SetChildrenAlongAxis(1, true);
        }
    }
}

