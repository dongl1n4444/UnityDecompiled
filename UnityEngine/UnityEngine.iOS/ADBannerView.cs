using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.iOS
{
	/// <summary>
	///   <para>ADBannerView is a wrapper around the ADBannerView class found in the Apple iAd framework and is only available on iOS.</para>
	/// </summary>
	[RequiredByNativeCode]
	public sealed class ADBannerView
	{
		/// <summary>
		///   <para>Specifies how banner should be layed out on screen.</para>
		/// </summary>
		public enum Layout
		{
			/// <summary>
			///   <para>Traditional Banner: align to screen top.</para>
			/// </summary>
			Top,
			/// <summary>
			///   <para>Traditional Banner: align to screen bottom.</para>
			/// </summary>
			Bottom,
			/// <summary>
			///   <para>Rect Banner: place in top-left corner.</para>
			/// </summary>
			TopLeft = 0,
			/// <summary>
			///   <para>Rect Banner: place in top-right corner.</para>
			/// </summary>
			TopRight = 4,
			/// <summary>
			///   <para>Rect Banner: align to screen top, placing at the center.</para>
			/// </summary>
			TopCenter = 8,
			/// <summary>
			///   <para>Rect Banner: place in bottom-left corner.</para>
			/// </summary>
			BottomLeft = 1,
			/// <summary>
			///   <para>Rect Banner: place in bottom-right corner.</para>
			/// </summary>
			BottomRight = 5,
			/// <summary>
			///   <para>Rect Banner: align to screen bottom, placing at the center.</para>
			/// </summary>
			BottomCenter = 9,
			/// <summary>
			///   <para>Rect Banner: align to screen left, placing at the center.</para>
			/// </summary>
			CenterLeft = 2,
			/// <summary>
			///   <para>Rect Banner: align to screen right, placing at the center.</para>
			/// </summary>
			CenterRight = 6,
			/// <summary>
			///   <para>Rect Banner: place exactly at screen center.</para>
			/// </summary>
			Center = 10,
			/// <summary>
			///   <para>Completely manual positioning.</para>
			/// </summary>
			Manual = -1
		}

		/// <summary>
		///   <para>The type of the banner view.</para>
		/// </summary>
		public enum Type
		{
			/// <summary>
			///   <para>Traditional Banner (it takes full screen width).</para>
			/// </summary>
			Banner,
			/// <summary>
			///   <para>Rect Banner (300x250).</para>
			/// </summary>
			MediumRect
		}

		/// <summary>
		///   <para>Will be fired when banner was clicked.</para>
		/// </summary>
		public delegate void BannerWasClickedDelegate();

		/// <summary>
		///   <para>Will be fired when banner loaded new ad.</para>
		/// </summary>
		public delegate void BannerWasLoadedDelegate();

		/// <summary>
		///   <para>Will be fired when banner ad failed to load.</para>
		/// </summary>
		public delegate void BannerFailedToLoadDelegate();

		private ADBannerView.Layout _layout;

		private IntPtr _bannerView;

		private static bool _AlwaysFalseDummy;

		public static event ADBannerView.BannerWasClickedDelegate onBannerWasClicked
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				ADBannerView.onBannerWasClicked = (ADBannerView.BannerWasClickedDelegate)Delegate.Combine(ADBannerView.onBannerWasClicked, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				ADBannerView.onBannerWasClicked = (ADBannerView.BannerWasClickedDelegate)Delegate.Remove(ADBannerView.onBannerWasClicked, value);
			}
		}

		public static event ADBannerView.BannerWasLoadedDelegate onBannerWasLoaded
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				ADBannerView.onBannerWasLoaded = (ADBannerView.BannerWasLoadedDelegate)Delegate.Combine(ADBannerView.onBannerWasLoaded, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				ADBannerView.onBannerWasLoaded = (ADBannerView.BannerWasLoadedDelegate)Delegate.Remove(ADBannerView.onBannerWasLoaded, value);
			}
		}

		public static event ADBannerView.BannerFailedToLoadDelegate onBannerFailedToLoad
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				ADBannerView.onBannerFailedToLoad = (ADBannerView.BannerFailedToLoadDelegate)Delegate.Combine(ADBannerView.onBannerFailedToLoad, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				ADBannerView.onBannerFailedToLoad = (ADBannerView.BannerFailedToLoadDelegate)Delegate.Remove(ADBannerView.onBannerFailedToLoad, value);
			}
		}

		/// <summary>
		///   <para>Checks if banner contents are loaded.</para>
		/// </summary>
		public bool loaded
		{
			get
			{
				return ADBannerView.Native_BannerAdLoaded(this._bannerView);
			}
		}

		/// <summary>
		///   <para>Banner visibility. Initially banner is not visible.</para>
		/// </summary>
		public bool visible
		{
			get
			{
				return ADBannerView.Native_BannerAdVisible(this._bannerView);
			}
			set
			{
				ADBannerView.Native_ShowBanner(this._bannerView, value);
			}
		}

		/// <summary>
		///   <para>Banner layout.</para>
		/// </summary>
		public ADBannerView.Layout layout
		{
			get
			{
				return this._layout;
			}
			set
			{
				this._layout = value;
				ADBannerView.Native_LayoutBanner(this._bannerView, (int)this._layout);
			}
		}

		/// <summary>
		///   <para>The position of the banner view.</para>
		/// </summary>
		public Vector2 position
		{
			get
			{
				Vector2 v;
				ADBannerView.Native_BannerPosition(this._bannerView, out v);
				return this.OSToScreenCoords(v);
			}
			set
			{
				Vector2 pos = new Vector2(value.x / (float)Screen.width, value.y / (float)Screen.height);
				ADBannerView.Native_MoveBanner(this._bannerView, pos);
			}
		}

		/// <summary>
		///   <para>The size of the banner view.</para>
		/// </summary>
		public Vector2 size
		{
			get
			{
				Vector2 v;
				ADBannerView.Native_BannerSize(this._bannerView, out v);
				return this.OSToScreenCoords(v);
			}
		}

		public ADBannerView(ADBannerView.Type type, ADBannerView.Layout layout)
		{
			if (ADBannerView._AlwaysFalseDummy)
			{
				ADBannerView.FireBannerWasClicked();
				ADBannerView.FireBannerWasLoaded();
				ADBannerView.FireBannerFailedToLoad();
			}
			this._bannerView = ADBannerView.Native_CreateBanner((int)type, (int)layout);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Native_CreateBanner(int type, int layout);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Native_ShowBanner(IntPtr view, bool show);

		private static void Native_MoveBanner(IntPtr view, Vector2 pos)
		{
			ADBannerView.INTERNAL_CALL_Native_MoveBanner(view, ref pos);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Native_MoveBanner(IntPtr view, ref Vector2 pos);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Native_LayoutBanner(IntPtr view, int layout);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Native_BannerTypeAvailable(int type);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Native_BannerPosition(IntPtr view, out Vector2 pos);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Native_BannerSize(IntPtr view, out Vector2 pos);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Native_BannerAdLoaded(IntPtr view);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Native_BannerAdVisible(IntPtr view);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Native_DestroyBanner(IntPtr view);

		public static bool IsAvailable(ADBannerView.Type type)
		{
			return ADBannerView.Native_BannerTypeAvailable((int)type);
		}

		~ADBannerView()
		{
			ADBannerView.Native_DestroyBanner(this._bannerView);
		}

		private Vector2 OSToScreenCoords(Vector2 v)
		{
			return new Vector2(v.x * (float)Screen.width, v.y * (float)Screen.height);
		}

		[RequiredByNativeCode]
		private static void FireBannerWasClicked()
		{
			if (ADBannerView.onBannerWasClicked != null)
			{
				ADBannerView.onBannerWasClicked();
			}
		}

		[RequiredByNativeCode]
		private static void FireBannerWasLoaded()
		{
			if (ADBannerView.onBannerWasLoaded != null)
			{
				ADBannerView.onBannerWasLoaded();
			}
		}

		[RequiredByNativeCode]
		private static void FireBannerFailedToLoad()
		{
			if (ADBannerView.onBannerFailedToLoad != null)
			{
				ADBannerView.onBannerFailedToLoad();
			}
		}
	}
}
