namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// <para>Base class for effects that modify the generated Mesh.</para>
    /// </summary>
    [ExecuteInEditMode]
    public abstract class BaseMeshEffect : UIBehaviour, IMeshModifier
    {
        [NonSerialized]
        private Graphic m_Graphic;

        protected BaseMeshEffect()
        {
        }

        /// <summary>
        /// <para>See:IMeshModifier.</para>
        /// </summary>
        /// <param name="mesh"></param>
        public virtual void ModifyMesh(Mesh mesh)
        {
            using (VertexHelper helper = new VertexHelper(mesh))
            {
                this.ModifyMesh(helper);
                helper.FillMesh(mesh);
            }
        }

        public abstract void ModifyMesh(VertexHelper vh);
        protected override void OnDidApplyAnimationProperties()
        {
            if (this.graphic != null)
            {
                this.graphic.SetVerticesDirty();
            }
            base.OnDidApplyAnimationProperties();
        }

        /// <summary>
        /// <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            if (this.graphic != null)
            {
                this.graphic.SetVerticesDirty();
            }
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (this.graphic != null)
            {
                this.graphic.SetVerticesDirty();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (this.graphic != null)
            {
                this.graphic.SetVerticesDirty();
            }
        }

        protected Graphic graphic
        {
            get
            {
                if (this.m_Graphic == null)
                {
                    this.m_Graphic = base.GetComponent<Graphic>();
                }
                return this.m_Graphic;
            }
        }
    }
}

