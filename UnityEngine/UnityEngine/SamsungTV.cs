using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	/// <summary>
	///   <para>Interface into SamsungTV specific functionality.</para>
	/// </summary>
	public sealed class SamsungTV
	{
		/// <summary>
		///   <para>Types of input the remote's touchpad can produce.</para>
		/// </summary>
		public enum TouchPadMode
		{
			/// <summary>
			///   <para>Remote dependent. On 2013 TVs dpad directions are determined by swiping in the correlating direction. On 2014 and 2015 TVs the remote has dpad buttons.</para>
			/// </summary>
			Dpad,
			/// <summary>
			///   <para>Touchpad works like an analog joystick. Not supported on the 2015 TV as there is no touchpad.</para>
			/// </summary>
			Joystick,
			/// <summary>
			///   <para>Touchpad controls a remote curosr like a laptop's touchpad. This can be replaced by airmouse functionality which is available on 2014 and 2015 TVs.</para>
			/// </summary>
			Mouse
		}

		/// <summary>
		///   <para>Types of input the gesture camera can produce.</para>
		/// </summary>
		public enum GestureMode
		{
			/// <summary>
			///   <para>No gesture input from the camera.</para>
			/// </summary>
			Off,
			/// <summary>
			///   <para>Hands control the mouse pointer.</para>
			/// </summary>
			Mouse,
			/// <summary>
			///   <para>Two hands control two joystick axes.</para>
			/// </summary>
			Joystick
		}

		/// <summary>
		///   <para>Types of input the gamepad can produce.</para>
		/// </summary>
		public enum GamePadMode
		{
			/// <summary>
			///   <para>Joystick style input.</para>
			/// </summary>
			Default,
			/// <summary>
			///   <para>Mouse style input.</para>
			/// </summary>
			Mouse
		}

		/// <summary>
		///   <para>Access to TV specific information.</para>
		/// </summary>
		public sealed class OpenAPI
		{
			public enum OpenAPIServerType
			{
				Operating,
				Development,
				Developing,
				Invalid
			}

			/// <summary>
			///   <para>The server type. Possible values:
			/// Developing, Development, Invalid, Operating.</para>
			/// </summary>
			public static extern SamsungTV.OpenAPI.OpenAPIServerType serverType
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			/// <summary>
			///   <para>Get local time on TV.</para>
			/// </summary>
			public static extern string timeOnTV
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			/// <summary>
			///   <para>Get UID from TV.</para>
			/// </summary>
			public static extern string uid
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string dUid
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}
		}

		/// <summary>
		///   <para>The type of input the remote's touch pad produces.</para>
		/// </summary>
		public static SamsungTV.TouchPadMode touchPadMode
		{
			get
			{
				return SamsungTV.GetTouchPadMode();
			}
			set
			{
				if (!SamsungTV.SetTouchPadMode(value))
				{
					throw new ArgumentException("Fail to set touchPadMode.");
				}
			}
		}

		/// <summary>
		///   <para>Changes the type of input the gesture camera produces.</para>
		/// </summary>
		public static extern SamsungTV.GestureMode gestureMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns true if there is an air mouse available.</para>
		/// </summary>
		public static extern bool airMouseConnected
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns true if the camera sees a hand.</para>
		/// </summary>
		public static extern bool gestureWorking
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Changes the type of input the gamepad produces.</para>
		/// </summary>
		public static extern SamsungTV.GamePadMode gamePadMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SamsungTV.TouchPadMode GetTouchPadMode();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetTouchPadMode(SamsungTV.TouchPadMode value);

		/// <summary>
		///   <para>Set the system language that is returned by Application.SystemLanguage.</para>
		/// </summary>
		/// <param name="language"></param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetSystemLanguage(SystemLanguage language);
	}
}
