namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.U2D.Interface;

    internal interface ISpriteEditor
    {
        void ClearProgressBar();
        void DisplayProgressBar(string title, string content, float progress);
        ITexture2D GetReadableTexture2D();
        void HandleSpriteSelection();
        void RequestRepaint();
        void SetDataModified();

        bool editingDisabled { get; }

        bool enableMouseMoveEvent { set; }

        ITexture2D previewTexture { get; }

        SpriteRect selectedSpriteRect { get; set; }

        ITexture2D selectedTexture { get; }

        ISpriteRectCache spriteRects { get; }

        Rect windowDimension { get; }
    }
}

