namespace UnityEngine.iOS
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>ADBannerView is a wrapper around the ADBannerView class found in the Apple iAd framework and is only available on iOS.</para>
    /// </summary>
    [Obsolete("iOS.ADBannerView class is obsolete, Apple iAD service discontinued", true)]
    public sealed class ADBannerView
    {
        public static  event BannerFailedToLoadDelegate onBannerFailedToLoad
        {
            add
            {
            }
            remove
            {
            }
        }

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

        /// <summary>
        /// <para>Banner layout.</para>
        /// </summary>
        public Layout layout
        {
            get => 
                Layout.Top;
            set
            {
            }
        }

        /// <summary>
        /// <para>Checks if banner contents are loaded.</para>
        /// </summary>
        public bool loaded =>
            false;

        /// <summary>
        /// <para>The position of the banner view.</para>
        /// </summary>
        public Vector2 position
        {
            get => 
                new Vector2();
            set
            {
            }
        }

        /// <summary>
        /// <para>The size of the banner view.</para>
        /// </summary>
        public Vector2 size =>
            new Vector2();

        /// <summary>
        /// <para>Banner visibility. Initially banner is not visible.</para>
        /// </summary>
        public bool visible
        {
            get => 
                false;
            set
            {
            }
        }

        /// <summary>
        /// <para>Will be fired when banner ad failed to load.</para>
        /// </summary>
        public delegate void BannerFailedToLoadDelegate();

        /// <summary>
        /// <para>Will be fired when banner was clicked.</para>
        /// </summary>
        public delegate void BannerWasClickedDelegate();

        /// <summary>
        /// <para>Will be fired when banner loaded new ad.</para>
        /// </summary>
        public delegate void BannerWasLoadedDelegate();

        /// <summary>
        /// <para>Specifies how banner should be layed out on screen.</para>
        /// </summary>
        public enum Layout
        {
            /// <summary>
            /// <para>Traditional Banner: align to screen bottom.</para>
            /// </summary>
            Bottom = 1,
            /// <summary>
            /// <para>Rect Banner: align to screen bottom, placing at the center.</para>
            /// </summary>
            BottomCenter = 9,
            /// <summary>
            /// <para>Rect Banner: place in bottom-left corner.</para>
            /// </summary>
            BottomLeft = 1,
            /// <summary>
            /// <para>Rect Banner: place in bottom-right corner.</para>
            /// </summary>
            BottomRight = 5,
            /// <summary>
            /// <para>Rect Banner: place exactly at screen center.</para>
            /// </summary>
            Center = 10,
            /// <summary>
            /// <para>Rect Banner: align to screen left, placing at the center.</para>
            /// </summary>
            CenterLeft = 2,
            /// <summary>
            /// <para>Rect Banner: align to screen right, placing at the center.</para>
            /// </summary>
            CenterRight = 6,
            /// <summary>
            /// <para>Completely manual positioning.</para>
            /// </summary>
            Manual = -1,
            /// <summary>
            /// <para>Traditional Banner: align to screen top.</para>
            /// </summary>
            Top = 0,
            /// <summary>
            /// <para>Rect Banner: align to screen top, placing at the center.</para>
            /// </summary>
            TopCenter = 8,
            /// <summary>
            /// <para>Rect Banner: place in top-left corner.</para>
            /// </summary>
            TopLeft = 0,
            /// <summary>
            /// <para>Rect Banner: place in top-right corner.</para>
            /// </summary>
            TopRight = 4
        }

        /// <summary>
        /// <para>The type of the banner view.</para>
        /// </summary>
        public enum Type
        {
            Banner,
            MediumRect
        }
    }
}

