namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>Adds an outline to a graphic using IVertexModifier.</para>
    /// </summary>
    [AddComponentMenu("UI/Effects/Shadow", 14)]
    public class Shadow : BaseMeshEffect
    {
        private const float kMaxEffectDistance = 600f;
        [SerializeField]
        private Color m_EffectColor = new Color(0f, 0f, 0f, 0.5f);
        [SerializeField]
        private Vector2 m_EffectDistance = new Vector2(1f, -1f);
        [SerializeField]
        private bool m_UseGraphicAlpha = true;

        protected Shadow()
        {
        }

        protected void ApplyShadow(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            this.ApplyShadowZeroAlloc(verts, color, start, end, x, y);
        }

        protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            int num = (verts.Count + end) - start;
            if (verts.Capacity < num)
            {
                verts.Capacity = num;
            }
            for (int i = start; i < end; i++)
            {
                UIVertex item = verts[i];
                verts.Add(item);
                Vector3 position = item.position;
                position.x += x;
                position.y += y;
                item.position = position;
                Color32 color2 = color;
                if (this.m_UseGraphicAlpha)
                {
                    UIVertex vertex2 = verts[i];
                    color2.a = (byte) ((color2.a * vertex2.color.a) / 0xff);
                }
                item.color = color2;
                verts[i] = item;
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (this.IsActive())
            {
                List<UIVertex> stream = ListPool<UIVertex>.Get();
                vh.GetUIVertexStream(stream);
                this.ApplyShadow(stream, this.effectColor, 0, stream.Count, this.effectDistance.x, this.effectDistance.y);
                vh.Clear();
                vh.AddUIVertexTriangleStream(stream);
                ListPool<UIVertex>.Release(stream);
            }
        }

        protected override void OnValidate()
        {
            this.effectDistance = this.m_EffectDistance;
            base.OnValidate();
        }

        /// <summary>
        /// <para>Color for the effect.</para>
        /// </summary>
        public Color effectColor
        {
            get => 
                this.m_EffectColor;
            set
            {
                this.m_EffectColor = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }

        /// <summary>
        /// <para>How far is the shadow from the graphic.</para>
        /// </summary>
        public Vector2 effectDistance
        {
            get => 
                this.m_EffectDistance;
            set
            {
                if (value.x > 600f)
                {
                    value.x = 600f;
                }
                if (value.x < -600f)
                {
                    value.x = -600f;
                }
                if (value.y > 600f)
                {
                    value.y = 600f;
                }
                if (value.y < -600f)
                {
                    value.y = -600f;
                }
                if (this.m_EffectDistance != value)
                {
                    this.m_EffectDistance = value;
                    if (base.graphic != null)
                    {
                        base.graphic.SetVerticesDirty();
                    }
                }
            }
        }

        /// <summary>
        /// <para>Should the shadow inherit the alpha from the graphic?</para>
        /// </summary>
        public bool useGraphicAlpha
        {
            get => 
                this.m_UseGraphicAlpha;
            set
            {
                this.m_UseGraphicAlpha = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }
    }
}

