namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// <para>Add this component to a GameObject to make it into a layout element or override values on an existing layout element.</para>
    /// </summary>
    [AddComponentMenu("Layout/Layout Element", 140), RequireComponent(typeof(RectTransform)), ExecuteInEditMode]
    public class LayoutElement : UIBehaviour, ILayoutElement, ILayoutIgnorer
    {
        [SerializeField]
        private float m_FlexibleHeight = -1f;
        [SerializeField]
        private float m_FlexibleWidth = -1f;
        [SerializeField]
        private bool m_IgnoreLayout = false;
        [SerializeField]
        private int m_LayoutPriority = 1;
        [SerializeField]
        private float m_MinHeight = -1f;
        [SerializeField]
        private float m_MinWidth = -1f;
        [SerializeField]
        private float m_PreferredHeight = -1f;
        [SerializeField]
        private float m_PreferredWidth = -1f;

        protected LayoutElement()
        {
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual void CalculateLayoutInputHorizontal()
        {
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual void CalculateLayoutInputVertical()
        {
        }

        protected override void OnBeforeTransformParentChanged()
        {
            this.SetDirty();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            this.SetDirty();
        }

        /// <summary>
        /// <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            this.SetDirty();
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.SetDirty();
        }

        protected override void OnTransformParentChanged()
        {
            this.SetDirty();
        }

        protected override void OnValidate()
        {
            this.SetDirty();
        }

        /// <summary>
        /// <para>Mark the LayoutElement as dirty.</para>
        /// </summary>
        protected void SetDirty()
        {
            if (this.IsActive())
            {
                LayoutRebuilder.MarkLayoutForRebuild(base.transform as RectTransform);
            }
        }

        /// <summary>
        /// <para>The extra relative height this layout element should be allocated if there is additional available space.</para>
        /// </summary>
        public virtual float flexibleHeight
        {
            get => 
                this.m_FlexibleHeight;
            set
            {
                if (SetPropertyUtility.SetStruct<float>(ref this.m_FlexibleHeight, value))
                {
                    this.SetDirty();
                }
            }
        }

        /// <summary>
        /// <para>The extra relative width this layout element should be allocated if there is additional available space.</para>
        /// </summary>
        public virtual float flexibleWidth
        {
            get => 
                this.m_FlexibleWidth;
            set
            {
                if (SetPropertyUtility.SetStruct<float>(ref this.m_FlexibleWidth, value))
                {
                    this.SetDirty();
                }
            }
        }

        /// <summary>
        /// <para>Should this RectTransform be ignored by the layout system?</para>
        /// </summary>
        public virtual bool ignoreLayout
        {
            get => 
                this.m_IgnoreLayout;
            set
            {
                if (SetPropertyUtility.SetStruct<bool>(ref this.m_IgnoreLayout, value))
                {
                    this.SetDirty();
                }
            }
        }

        /// <summary>
        /// <para>Called by the layout system.</para>
        /// </summary>
        public virtual int layoutPriority
        {
            get => 
                this.m_LayoutPriority;
            set
            {
                if (SetPropertyUtility.SetStruct<int>(ref this.m_LayoutPriority, value))
                {
                    this.SetDirty();
                }
            }
        }

        /// <summary>
        /// <para>The minimum height this layout element may be allocated.</para>
        /// </summary>
        public virtual float minHeight
        {
            get => 
                this.m_MinHeight;
            set
            {
                if (SetPropertyUtility.SetStruct<float>(ref this.m_MinHeight, value))
                {
                    this.SetDirty();
                }
            }
        }

        /// <summary>
        /// <para>The minimum width this layout element may be allocated.</para>
        /// </summary>
        public virtual float minWidth
        {
            get => 
                this.m_MinWidth;
            set
            {
                if (SetPropertyUtility.SetStruct<float>(ref this.m_MinWidth, value))
                {
                    this.SetDirty();
                }
            }
        }

        /// <summary>
        /// <para>The preferred height this layout element should be allocated if there is sufficient space.</para>
        /// </summary>
        public virtual float preferredHeight
        {
            get => 
                this.m_PreferredHeight;
            set
            {
                if (SetPropertyUtility.SetStruct<float>(ref this.m_PreferredHeight, value))
                {
                    this.SetDirty();
                }
            }
        }

        /// <summary>
        /// <para>The preferred width this layout element should be allocated if there is sufficient space.</para>
        /// </summary>
        public virtual float preferredWidth
        {
            get => 
                this.m_PreferredWidth;
            set
            {
                if (SetPropertyUtility.SetStruct<float>(ref this.m_PreferredWidth, value))
                {
                    this.SetDirty();
                }
            }
        }
    }
}

