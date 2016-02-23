using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Tizen
{
	/// <summary>
	///   <para>Interface into Tizen specific functionality.</para>
	/// </summary>
	public sealed class Window
	{
		/// <summary>
		///   <para>Get pointer to the native window handle.</para>
		/// </summary>
		public static extern IntPtr windowHandle
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
