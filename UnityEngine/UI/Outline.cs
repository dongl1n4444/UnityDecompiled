namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// <para>Adds an outline to a graphic using IVertexModifier.</para>
    /// </summary>
    [AddComponentMenu("UI/Effects/Outline", 15)]
    public class Outline : Shadow
    {
        protected Outline()
        {
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (this.IsActive())
            {
                List<UIVertex> stream = ListPool<UIVertex>.Get();
                vh.GetUIVertexStream(stream);
                int num = stream.Count * 5;
                if (stream.Capacity < num)
                {
                    stream.Capacity = num;
                }
                int start = 0;
                int count = stream.Count;
                base.ApplyShadowZeroAlloc(stream, base.effectColor, start, stream.Count, base.effectDistance.x, base.effectDistance.y);
                start = count;
                count = stream.Count;
                base.ApplyShadowZeroAlloc(stream, base.effectColor, start, stream.Count, base.effectDistance.x, -base.effectDistance.y);
                start = count;
                count = stream.Count;
                base.ApplyShadowZeroAlloc(stream, base.effectColor, start, stream.Count, -base.effectDistance.x, base.effectDistance.y);
                start = count;
                count = stream.Count;
                base.ApplyShadowZeroAlloc(stream, base.effectColor, start, stream.Count, -base.effectDistance.x, -base.effectDistance.y);
                vh.Clear();
                vh.AddUIVertexTriangleStream(stream);
                ListPool<UIVertex>.Release(stream);
            }
        }
    }
}

