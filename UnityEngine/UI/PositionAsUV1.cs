namespace UnityEngine.UI
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>An IVertexModifier which sets the raw vertex position into UV1 of the generated verts.</para>
    /// </summary>
    [AddComponentMenu("UI/Effects/Position As UV1", 0x10)]
    public class PositionAsUV1 : BaseMeshEffect
    {
        protected PositionAsUV1()
        {
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            UIVertex vertex = new UIVertex();
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);
                vertex.uv1 = new Vector2(vertex.position.x, vertex.position.y);
                vh.SetUIVertex(vertex, i);
            }
        }
    }
}

