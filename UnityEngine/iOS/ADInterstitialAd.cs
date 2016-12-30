namespace UnityEngine.iOS
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>ADInterstitialAd is a wrapper around the ADInterstitialAd class found in the Apple iAd framework and is only available on iPad.</para>
    /// </summary>
    [Obsolete("iOS.ADInterstitialAd class is obsolete, Apple iAD service discontinued", true)]
    public sealed class ADInterstitialAd
    {
        public static  event InterstitialWasLoadedDelegate onInterstitialWasLoaded
        {
            add
            {
            }
            remove
            {
            }
        }

        public static  event InterstitialWasViewedDelegate onInterstitialWasViewed
        {
            add
            {
            }
            remove
            {
            }
        }

        /// <summary>
        /// <para>Creates an interstitial ad.</para>
        /// </summary>
        /// <param name="autoReload"></param>
        public ADInterstitialAd()
        {
        }

        /// <summary>
        /// <para>Creates an interstitial ad.</para>
        /// </summary>
        /// <param name="autoReload"></param>
        public ADInterstitialAd(bool autoReload)
        {
        }

        /// <summary>
        /// <para>Reload advertisement.</para>
        /// </summary>
        public void ReloadAd()
        {
        }

        /// <summary>
        /// <para>Shows full-screen advertisement to user.</para>
        /// </summary>
        public void Show()
        {
        }

        /// <summary>
        /// <para>Checks if InterstitialAd is available (it is available on iPad since iOS 4.3, and on iPhone since iOS 7.0).</para>
        /// </summary>
        public static bool isAvailable =>
            false;

        /// <summary>
        /// <para>Has the interstitial ad object downloaded an advertisement? (Read Only)</para>
        /// </summary>
        public bool loaded =>
            false;

        /// <summary>
        /// <para>Will be called when ad is ready to be shown.</para>
        /// </summary>
        public delegate void InterstitialWasLoadedDelegate();

        /// <summary>
        /// <para>Will be called when user did view ad contents: i.e. he went past initial screen. Please note that it is impossible to determine if he clicked on any links on ad sequence that follows initial screen.</para>
        /// </summary>
        public delegate void InterstitialWasViewedDelegate();
    }
}

