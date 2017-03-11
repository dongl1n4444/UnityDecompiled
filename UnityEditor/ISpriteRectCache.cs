namespace UnityEditor
{
    using System;
    using UnityEditor.U2D.Interface;

    internal interface ISpriteRectCache : IUndoableObject
    {
        void AddRect(SpriteRect r);
        void ClearAll();
        bool Contains(SpriteRect spriteRect);
        int GetIndex(SpriteRect spriteRect);
        SpriteRect RectAt(int i);
        void RemoveRect(SpriteRect r);

        int Count { get; }
    }
}

