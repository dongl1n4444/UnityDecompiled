namespace UnityEditor.iOS.Xcode
{
    using System;

    internal class ImageResizing
    {
        public int bottom = 0;
        public int centerHeight = 0;
        public ResizeMode centerResizeMode = ResizeMode.Stretch;
        public int centerWidth = 0;
        public int left = 0;
        public int right = 0;
        public int top = 0;
        public SlicingType type = SlicingType.HorizontalAndVertical;

        public enum ResizeMode
        {
            Stretch,
            Tile
        }

        public enum SlicingType
        {
            Horizontal,
            Vertical,
            HorizontalAndVertical
        }
    }
}

