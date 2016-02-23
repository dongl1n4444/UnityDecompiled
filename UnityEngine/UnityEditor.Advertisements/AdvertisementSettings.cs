using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace UnityEditor.Advertisements
{
	/// <summary>
	///   <para>Editor API for the Unity Services editor feature. Normally UnityAds is enabled from the Services window, but if writing your own editor extension, this API can be used.</para>
	/// </summary>
	public static class AdvertisementSettings
	{
		/// <summary>
		///   <para>Global boolean for enabling or disabling the advertisement feature.</para>
		/// </summary>
		public static bool enabled
		{
			get
			{
				return UnityAdsManager.enabled;
			}
			set
			{
				UnityAdsManager.enabled = value;
			}
		}

		/// <summary>
		///   <para>Controls if the advertisement system should be initialized immediately on startup.</para>
		/// </summary>
		public static bool initializeOnStartup
		{
			get
			{
				return UnityAdsManager.initializeOnStartup;
			}
			set
			{
				UnityAdsManager.initializeOnStartup = value;
			}
		}

		/// <summary>
		///   <para>Controls if testing advertisements are used instead of production advertisements.</para>
		/// </summary>
		public static bool testMode
		{
			get
			{
				return UnityAdsManager.testMode;
			}
			set
			{
				UnityAdsManager.testMode = value;
			}
		}

		/// <summary>
		///   <para>Returns if a specific platform is enabled.</para>
		/// </summary>
		/// <param name="platform"></param>
		/// <returns>
		///   <para>Boolean for the platform.</para>
		/// </returns>
		public static bool IsPlatformEnabled(RuntimePlatform platform)
		{
			return UnityAdsManager.IsPlatformEnabled(platform);
		}

		/// <summary>
		///   <para>Enable the specific platform.</para>
		/// </summary>
		/// <param name="platform"></param>
		/// <param name="value"></param>
		public static void SetPlatformEnabled(RuntimePlatform platform, bool value)
		{
			UnityAdsManager.SetPlatformEnabled(platform, value);
		}

		/// <summary>
		///   <para>Gets the game identifier specified for a runtime platform.</para>
		/// </summary>
		/// <param name="platform"></param>
		/// <returns>
		///   <para>The platform specific game identifier.</para>
		/// </returns>
		public static string GetGameId(RuntimePlatform platform)
		{
			return UnityAdsManager.GetGameId(platform);
		}

		/// <summary>
		///   <para>Sets the game identifier for the specified platform.</para>
		/// </summary>
		/// <param name="platform"></param>
		/// <param name="gameId"></param>
		public static void SetGameId(RuntimePlatform platform, string gameId)
		{
			UnityAdsManager.SetGameId(platform, gameId);
		}
	}
}
