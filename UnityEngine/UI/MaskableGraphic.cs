namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering;

    /// <summary>
    /// <para>A Graphic that is capable of being masked out.</para>
    /// </summary>
    public abstract class MaskableGraphic : Graphic, IClippable, IMaskable, IMaterialModifier
    {
        private readonly Vector3[] m_Corners = new Vector3[4];
        [NonSerialized, Obsolete("Not used anymore.", true)]
        protected bool m_IncludeForMasking = false;
        [NonSerialized]
        private bool m_Maskable = true;
        [NonSerialized]
        protected Material m_MaskMaterial;
        [SerializeField]
        private CullStateChangedEvent m_OnCullStateChanged = new CullStateChangedEvent();
        [NonSerialized]
        private RectMask2D m_ParentMask;
        [NonSerialized, Obsolete("Not used anymore", true)]
        protected bool m_ShouldRecalculate = true;
        [NonSerialized]
        protected bool m_ShouldRecalculateStencil = true;
        [NonSerialized]
        protected int m_StencilValue;

        protected MaskableGraphic()
        {
        }

        /// <summary>
        /// <para>See IClippable.Cull.</para>
        /// </summary>
        /// <param name="clipRect"></param>
        /// <param name="validRect"></param>
        public virtual void Cull(Rect clipRect, bool validRect)
        {
            bool cull = !validRect || !clipRect.Overlaps(this.rootCanvasRect, true);
            this.UpdateCull(cull);
        }

        /// <summary>
        /// <para>See IMaterialModifier.GetModifiedMaterial.</para>
        /// </summary>
        /// <param name="baseMaterial"></param>
        public virtual Material GetModifiedMaterial(Material baseMaterial)
        {
            Material baseMat = baseMaterial;
            if (this.m_ShouldRecalculateStencil)
            {
                Transform stopAfter = MaskUtilities.FindRootSortOverrideCanvas(base.transform);
                this.m_StencilValue = !this.maskable ? 0 : MaskUtilities.GetStencilDepth(base.transform, stopAfter);
                this.m_ShouldRecalculateStencil = false;
            }
            Mask component = base.GetComponent<Mask>();
            if ((this.m_StencilValue <= 0) || ((component != null) && component.IsActive()))
            {
                return baseMat;
            }
            Material material2 = StencilMaterial.Add(baseMat, (((int) 1) << this.m_StencilValue) - 1, StencilOp.Keep, CompareFunction.Equal, ColorWriteMask.All, (((int) 1) << this.m_StencilValue) - 1, 0);
            StencilMaterial.Remove(this.m_MaskMaterial);
            this.m_MaskMaterial = material2;
            return this.m_MaskMaterial;
        }

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();
            if (base.isActiveAndEnabled)
            {
                this.m_ShouldRecalculateStencil = true;
                this.UpdateClipParent();
                this.SetMaterialDirty();
            }
        }

        /// <summary>
        /// <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            this.m_ShouldRecalculateStencil = true;
            this.SetMaterialDirty();
            this.UpdateClipParent();
            StencilMaterial.Remove(this.m_MaskMaterial);
            this.m_MaskMaterial = null;
            if (base.GetComponent<Mask>() != null)
            {
                MaskUtilities.NotifyStencilStateChanged(this);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_ShouldRecalculateStencil = true;
            this.UpdateClipParent();
            this.SetMaterialDirty();
            if (base.GetComponent<Mask>() != null)
            {
                MaskUtilities.NotifyStencilStateChanged(this);
            }
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            if (base.isActiveAndEnabled)
            {
                this.m_ShouldRecalculateStencil = true;
                this.UpdateClipParent();
                this.SetMaterialDirty();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            this.m_ShouldRecalculateStencil = true;
            this.UpdateClipParent();
            this.SetMaterialDirty();
        }

        /// <summary>
        /// <para>See: IMaskable.</para>
        /// </summary>
        [Obsolete("Not used anymore.", true)]
        public virtual void ParentMaskStateChanged()
        {
        }

        /// <summary>
        /// <para>See: IClippable.RecalculateClipping.</para>
        /// </summary>
        public virtual void RecalculateClipping()
        {
            this.UpdateClipParent();
        }

        /// <summary>
        /// <para>See: IMaskable.RecalculateMasking.</para>
        /// </summary>
        public virtual void RecalculateMasking()
        {
            this.m_ShouldRecalculateStencil = true;
            this.SetMaterialDirty();
        }

        /// <summary>
        /// <para>See IClippable.SetClipRect.</para>
        /// </summary>
        /// <param name="clipRect"></param>
        /// <param name="validRect"></param>
        public virtual void SetClipRect(Rect clipRect, bool validRect)
        {
            if (validRect)
            {
                base.canvasRenderer.EnableRectClipping(clipRect);
            }
            else
            {
                base.canvasRenderer.DisableRectClipping();
            }
        }

        GameObject IClippable.get_gameObject() => 
            base.gameObject;

        private void UpdateClipParent()
        {
            RectMask2D maskd = (!this.maskable || !this.IsActive()) ? null : MaskUtilities.GetRectMaskForClippable(this);
            if ((this.m_ParentMask != null) && ((maskd != this.m_ParentMask) || !maskd.IsActive()))
            {
                this.m_ParentMask.RemoveClippable(this);
                this.UpdateCull(false);
            }
            if ((maskd != null) && maskd.IsActive())
            {
                maskd.AddClippable(this);
            }
            this.m_ParentMask = maskd;
        }

        private void UpdateCull(bool cull)
        {
            bool flag = base.canvasRenderer.cull != cull;
            base.canvasRenderer.cull = cull;
            if (flag)
            {
                this.m_OnCullStateChanged.Invoke(cull);
                this.SetVerticesDirty();
            }
        }

        /// <summary>
        /// <para>Does this graphic allow masking.</para>
        /// </summary>
        public bool maskable
        {
            get => 
                this.m_Maskable;
            set
            {
                if (value != this.m_Maskable)
                {
                    this.m_Maskable = value;
                    this.m_ShouldRecalculateStencil = true;
                    this.SetMaterialDirty();
                }
            }
        }

        /// <summary>
        /// <para>Callback issued when culling changes.</para>
        /// </summary>
        public CullStateChangedEvent onCullStateChanged
        {
            get => 
                this.m_OnCullStateChanged;
            set
            {
                this.m_OnCullStateChanged = value;
            }
        }

        private Rect rootCanvasRect
        {
            get
            {
                base.rectTransform.GetWorldCorners(this.m_Corners);
                if (base.canvas != null)
                {
                    Canvas rootCanvas = base.canvas.rootCanvas;
                    for (int i = 0; i < 4; i++)
                    {
                        this.m_Corners[i] = rootCanvas.transform.InverseTransformPoint(this.m_Corners[i]);
                    }
                }
                return new Rect(this.m_Corners[0].x, this.m_Corners[0].y, this.m_Corners[2].x - this.m_Corners[0].x, this.m_Corners[2].y - this.m_Corners[0].y);
            }
        }

        [Serializable]
        public class CullStateChangedEvent : UnityEvent<bool>
        {
        }
    }
}

