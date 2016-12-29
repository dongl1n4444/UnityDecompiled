namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    [Obsolete("ADBannerView class is obsolete, Apple iAD service discontinued", true)]
    public sealed class ADBannerView
    {
        public static  event BannerWasClickedDelegate onBannerWasClicked
        {
            add
            {
            }
            remove
            {
            }
        }

        public static  event BannerWasLoadedDelegate onBannerWasLoaded
        {
            add
            {
            }
            remove
            {
            }
        }

        public ADBannerView(Type type, Layout layout)
        {
        }

        public static bool IsAvailable(Type type) => 
            false;

        public Layout layout
        {
            get => 
                Layout.Top;
            set
            {
            }
        }

        public bool loaded =>
            false;

        public Vector2 position
        {
            get => 
                new Vector2();
            set
            {
            }
        }

        public Vector2 size =>
            new Vector2();

        public bool visible
        {
            get => 
                false;
            set
            {
            }
        }

        public delegate void BannerWasClickedDelegate();

        public delegate void BannerWasLoadedDelegate();

        public enum Layout
        {
            Bottom = 1,
            BottomCenter = 9,
            BottomLeft = 1,
            BottomRight = 5,
            Center = 10,
            CenterLeft = 2,
            CenterRight = 6,
            Manual = -1,
            Top = 0,
            TopCenter = 8,
            TopLeft = 0,
            TopRight = 4
        }

        public enum Type
        {
            Banner,
            MediumRect
        }
    }
}

