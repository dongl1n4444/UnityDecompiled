using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.iOS
{
	/// <summary>
	///   <para>ADInterstitialAd is a wrapper around the ADInterstitialAd class found in the Apple iAd framework and is only available on iPad.</para>
	/// </summary>
	[RequiredByNativeCode]
	public sealed class ADInterstitialAd
	{
		/// <summary>
		///   <para>Will be called when ad is ready to be shown.</para>
		/// </summary>
		public delegate void InterstitialWasLoadedDelegate();

		/// <summary>
		///   <para>Will be called when user did view ad contents: i.e. he went past initial screen. Please note that it is impossible to determine if he clicked on any links on ad sequence that follows initial screen.</para>
		/// </summary>
		public delegate void InterstitialWasViewedDelegate();

		private IntPtr interstitialView;

		private static bool _AlwaysFalseDummy;

		public static event ADInterstitialAd.InterstitialWasLoadedDelegate onInterstitialWasLoaded
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				ADInterstitialAd.onInterstitialWasLoaded = (ADInterstitialAd.InterstitialWasLoadedDelegate)Delegate.Combine(ADInterstitialAd.onInterstitialWasLoaded, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				ADInterstitialAd.onInterstitialWasLoaded = (ADInterstitialAd.InterstitialWasLoadedDelegate)Delegate.Remove(ADInterstitialAd.onInterstitialWasLoaded, value);
			}
		}

		public static event ADInterstitialAd.InterstitialWasViewedDelegate onInterstitialWasViewed
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				ADInterstitialAd.onInterstitialWasViewed = (ADInterstitialAd.InterstitialWasViewedDelegate)Delegate.Combine(ADInterstitialAd.onInterstitialWasViewed, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				ADInterstitialAd.onInterstitialWasViewed = (ADInterstitialAd.InterstitialWasViewedDelegate)Delegate.Remove(ADInterstitialAd.onInterstitialWasViewed, value);
			}
		}

		/// <summary>
		///   <para>Checks if InterstitialAd is available (it is available on iPad since iOS 4.3, and on iPhone since iOS 7.0).</para>
		/// </summary>
		public static bool isAvailable
		{
			get
			{
				return ADInterstitialAd.Native_InterstitialAvailable();
			}
		}

		/// <summary>
		///   <para>Has the interstitial ad object downloaded an advertisement? (Read Only)</para>
		/// </summary>
		public bool loaded
		{
			get
			{
				return ADInterstitialAd.Native_InterstitialAdLoaded(this.interstitialView);
			}
		}

		/// <summary>
		///   <para>Creates an interstitial ad.</para>
		/// </summary>
		/// <param name="autoReload"></param>
		public ADInterstitialAd(bool autoReload)
		{
			this.CtorImpl(autoReload);
		}

		/// <summary>
		///   <para>Creates an interstitial ad.</para>
		/// </summary>
		/// <param name="autoReload"></param>
		public ADInterstitialAd()
		{
			this.CtorImpl(false);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Native_CreateInterstitial(bool autoReload);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Native_ShowInterstitial(IntPtr view);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Native_ReloadInterstitial(IntPtr view);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Native_InterstitialAdLoaded(IntPtr view);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Native_InterstitialAvailable();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Native_DestroyInterstitial(IntPtr view);

		private void CtorImpl(bool autoReload)
		{
			if (ADInterstitialAd._AlwaysFalseDummy)
			{
				ADInterstitialAd.FireInterstitialWasLoaded();
				ADInterstitialAd.FireInterstitialWasViewed();
			}
			this.interstitialView = ADInterstitialAd.Native_CreateInterstitial(autoReload);
		}

		~ADInterstitialAd()
		{
			ADInterstitialAd.Native_DestroyInterstitial(this.interstitialView);
		}

		/// <summary>
		///   <para>Shows full-screen advertisement to user.</para>
		/// </summary>
		public void Show()
		{
			if (this.loaded)
			{
				ADInterstitialAd.Native_ShowInterstitial(this.interstitialView);
			}
			else
			{
				Debug.Log("Calling ADInterstitialAd.Show() when the ad is not loaded");
			}
		}

		/// <summary>
		///   <para>Reload advertisement.</para>
		/// </summary>
		public void ReloadAd()
		{
			ADInterstitialAd.Native_ReloadInterstitial(this.interstitialView);
		}

		[RequiredByNativeCode]
		private static void FireInterstitialWasLoaded()
		{
			if (ADInterstitialAd.onInterstitialWasLoaded != null)
			{
				ADInterstitialAd.onInterstitialWasLoaded();
			}
		}

		[RequiredByNativeCode]
		private static void FireInterstitialWasViewed()
		{
			if (ADInterstitialAd.onInterstitialWasViewed != null)
			{
				ADInterstitialAd.onInterstitialWasViewed();
			}
		}
	}
}
