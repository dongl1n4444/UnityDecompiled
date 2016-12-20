namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Rendering;
    using UnityEngine.Serialization;

    /// <summary>
    /// <para>A component for masking children elements.</para>
    /// </summary>
    [AddComponentMenu("UI/Mask", 13), ExecuteInEditMode, RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class Mask : UIBehaviour, ICanvasRaycastFilter, IMaterialModifier
    {
        [NonSerialized]
        private Graphic m_Graphic;
        [NonSerialized]
        private Material m_MaskMaterial;
        [NonSerialized]
        private RectTransform m_RectTransform;
        [SerializeField, FormerlySerializedAs("m_ShowGraphic")]
        private bool m_ShowMaskGraphic = true;
        [NonSerialized]
        private Material m_UnmaskMaterial;

        protected Mask()
        {
        }

        /// <summary>
        /// <para>See: IMaterialModifier.</para>
        /// </summary>
        /// <param name="baseMaterial"></param>
        public virtual Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!this.MaskEnabled())
            {
                return baseMaterial;
            }
            Transform stopAfter = MaskUtilities.FindRootSortOverrideCanvas(base.transform);
            int stencilDepth = MaskUtilities.GetStencilDepth(base.transform, stopAfter);
            if (stencilDepth >= 8)
            {
                Debug.LogError("Attempting to use a stencil mask with depth > 8", base.gameObject);
                return baseMaterial;
            }
            int num2 = ((int) 1) << stencilDepth;
            if (num2 == 1)
            {
                Material material2 = StencilMaterial.Add(baseMaterial, 1, StencilOp.Replace, CompareFunction.Always, !this.m_ShowMaskGraphic ? ((ColorWriteMask) 0) : ColorWriteMask.All);
                StencilMaterial.Remove(this.m_MaskMaterial);
                this.m_MaskMaterial = material2;
                Material material3 = StencilMaterial.Add(baseMaterial, 1, StencilOp.Zero, CompareFunction.Always, 0);
                StencilMaterial.Remove(this.m_UnmaskMaterial);
                this.m_UnmaskMaterial = material3;
                this.graphic.canvasRenderer.popMaterialCount = 1;
                this.graphic.canvasRenderer.SetPopMaterial(this.m_UnmaskMaterial, 0);
                return this.m_MaskMaterial;
            }
            Material material4 = StencilMaterial.Add(baseMaterial, num2 | (num2 - 1), StencilOp.Replace, CompareFunction.Equal, !this.m_ShowMaskGraphic ? ((ColorWriteMask) 0) : ColorWriteMask.All, num2 - 1, num2 | (num2 - 1));
            StencilMaterial.Remove(this.m_MaskMaterial);
            this.m_MaskMaterial = material4;
            this.graphic.canvasRenderer.hasPopInstruction = true;
            Material material5 = StencilMaterial.Add(baseMaterial, num2 - 1, StencilOp.Replace, CompareFunction.Equal, 0, num2 - 1, num2 | (num2 - 1));
            StencilMaterial.Remove(this.m_UnmaskMaterial);
            this.m_UnmaskMaterial = material5;
            this.graphic.canvasRenderer.popMaterialCount = 1;
            this.graphic.canvasRenderer.SetPopMaterial(this.m_UnmaskMaterial, 0);
            return this.m_MaskMaterial;
        }

        /// <summary>
        /// <para>See:ICanvasRaycastFilter.</para>
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="eventCamera"></param>
        public virtual bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return (!base.isActiveAndEnabled || RectTransformUtility.RectangleContainsScreenPoint(this.rectTransform, sp, eventCamera));
        }

        /// <summary>
        /// <para>See:IMask.</para>
        /// </summary>
        public virtual bool MaskEnabled()
        {
            return (this.IsActive() && (this.graphic != null));
        }

        /// <summary>
        /// <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (this.graphic != null)
            {
                this.graphic.SetMaterialDirty();
                this.graphic.canvasRenderer.hasPopInstruction = false;
                this.graphic.canvasRenderer.popMaterialCount = 0;
            }
            StencilMaterial.Remove(this.m_MaskMaterial);
            this.m_MaskMaterial = null;
            StencilMaterial.Remove(this.m_UnmaskMaterial);
            this.m_UnmaskMaterial = null;
            MaskUtilities.NotifyStencilStateChanged(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (this.graphic != null)
            {
                this.graphic.canvasRenderer.hasPopInstruction = true;
                this.graphic.SetMaterialDirty();
            }
            MaskUtilities.NotifyStencilStateChanged(this);
        }

        /// <summary>
        /// <para>See:IGraphicEnabledDisabled.</para>
        /// </summary>
        [Obsolete("Not used anymore.")]
        public virtual void OnSiblingGraphicEnabledDisabled()
        {
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (this.IsActive())
            {
                if (this.graphic != null)
                {
                    this.graphic.SetMaterialDirty();
                }
                MaskUtilities.NotifyStencilStateChanged(this);
            }
        }

        /// <summary>
        /// <para>The graphic associated with the Mask.</para>
        /// </summary>
        public Graphic graphic
        {
            get
            {
                Graphic graphic;
                if (this.m_Graphic != null)
                {
                    graphic = this.m_Graphic;
                }
                else
                {
                    graphic = this.m_Graphic = base.GetComponent<Graphic>();
                }
                return graphic;
            }
        }

        /// <summary>
        /// <para>Cached RectTransform.</para>
        /// </summary>
        public RectTransform rectTransform
        {
            get
            {
                RectTransform rectTransform;
                if (this.m_RectTransform != null)
                {
                    rectTransform = this.m_RectTransform;
                }
                else
                {
                    rectTransform = this.m_RectTransform = base.GetComponent<RectTransform>();
                }
                return rectTransform;
            }
        }

        /// <summary>
        /// <para>Show the graphic that is associated with the Mask render area.</para>
        /// </summary>
        public bool showMaskGraphic
        {
            get
            {
                return this.m_ShowMaskGraphic;
            }
            set
            {
                if (this.m_ShowMaskGraphic != value)
                {
                    this.m_ShowMaskGraphic = value;
                    if (this.graphic != null)
                    {
                        this.graphic.SetMaterialDirty();
                    }
                }
            }
        }
    }
}

