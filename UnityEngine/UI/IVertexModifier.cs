namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;

    [Obsolete("Use IMeshModifier instead", true)]
    public interface IVertexModifier
    {
        [Obsolete("use IMeshModifier.ModifyMesh (VertexHelper verts)  instead", true)]
        void ModifyVertices(List<UIVertex> verts);
    }
}

