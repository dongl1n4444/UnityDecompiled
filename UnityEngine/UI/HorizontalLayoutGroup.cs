namespace UnityEngine.UI
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Layout child layout elements side by side.</para>
    /// </summary>
    [AddComponentMenu("Layout/Horizontal Layout Group", 150)]
    public class HorizontalLayoutGroup : HorizontalOrVerticalLayoutGroup
    {
        protected HorizontalLayoutGroup()
        {
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            base.CalcAlongAxis(0, false);
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public override void CalculateLayoutInputVertical()
        {
            base.CalcAlongAxis(1, false);
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            base.SetChildrenAlongAxis(0, false);
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public override void SetLayoutVertical()
        {
            base.SetChildrenAlongAxis(1, false);
        }
    }
}

