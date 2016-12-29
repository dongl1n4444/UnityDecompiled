namespace UnityEditor.Advertisements
{
    using System;
    using UnityEngine;
    using UnityEngine.Connect;

    /// <summary>
    /// <para>Editor API for the Unity Services editor feature. Normally UnityAds is enabled from the Services window, but if writing your own editor extension, this API can be used.</para>
    /// </summary>
    public static class AdvertisementSettings
    {
        /// <summary>
        /// <para>Gets the game identifier specified for a runtime platform.</para>
        /// </summary>
        /// <param name="platform"></param>
        /// <returns>
        /// <para>The platform specific game identifier.</para>
        /// </returns>
        public static string GetGameId(RuntimePlatform platform) => 
            UnityAdsSettings.GetGameId(platform);

        /// <summary>
        /// <para>Returns if a specific platform is enabled.</para>
        /// </summary>
        /// <param name="platform"></param>
        /// <returns>
        /// <para>Boolean for the platform.</para>
        /// </returns>
        public static bool IsPlatformEnabled(RuntimePlatform platform) => 
            UnityAdsSettings.IsPlatformEnabled(platform);

        /// <summary>
        /// <para>Sets the game identifier for the specified platform.</para>
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="gameId"></param>
        public static void SetGameId(RuntimePlatform platform, string gameId)
        {
            UnityAdsSettings.SetGameId(platform, gameId);
        }

        /// <summary>
        /// <para>Enable the specific platform.</para>
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="value"></param>
        public static void SetPlatformEnabled(RuntimePlatform platform, bool value)
        {
            UnityAdsSettings.SetPlatformEnabled(platform, value);
        }

        /// <summary>
        /// <para>Global boolean for enabling or disabling the advertisement feature.</para>
        /// </summary>
        public static bool enabled
        {
            get => 
                UnityAdsSettings.enabled;
            set
            {
                UnityAdsSettings.enabled = value;
            }
        }

        /// <summary>
        /// <para>Controls if the advertisement system should be initialized immediately on startup.</para>
        /// </summary>
        public static bool initializeOnStartup
        {
            get => 
                UnityAdsSettings.initializeOnStartup;
            set
            {
                UnityAdsSettings.initializeOnStartup = value;
            }
        }

        /// <summary>
        /// <para>Controls if testing advertisements are used instead of production advertisements.</para>
        /// </summary>
        public static bool testMode
        {
            get => 
                UnityAdsSettings.testMode;
            set
            {
                UnityAdsSettings.testMode = value;
            }
        }
    }
}

