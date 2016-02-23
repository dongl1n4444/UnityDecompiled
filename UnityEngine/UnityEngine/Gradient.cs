using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Gradient used for animating colors.</para>
	/// </summary>
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class Gradient
	{
		internal IntPtr m_Ptr;

		/// <summary>
		///   <para>All color keys defined in the gradient.</para>
		/// </summary>
		public extern GradientColorKey[] colorKeys
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>All alpha keys defined in the gradient.</para>
		/// </summary>
		public extern GradientAlphaKey[] alphaKeys
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal Color constantColor
		{
			get
			{
				Color result;
				this.INTERNAL_get_constantColor(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_constantColor(ref value);
			}
		}

		/// <summary>
		///   <para>Create a new Gradient object.</para>
		/// </summary>
		[RequiredByNativeCode]
		public Gradient()
		{
			this.Init();
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Init();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Cleanup();

		~Gradient()
		{
			this.Cleanup();
		}

		/// <summary>
		///   <para>Calculate color at a given time.</para>
		/// </summary>
		/// <param name="time">Time of the key (0 - 1).</param>
		public Color Evaluate(float time)
		{
			Color result;
			Gradient.INTERNAL_CALL_Evaluate(this, time, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Evaluate(Gradient self, float time, out Color value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_constantColor(out Color value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_constantColor(ref Color value);

		/// <summary>
		///   <para>Setup Gradient with an array of color keys and alpha keys.</para>
		/// </summary>
		/// <param name="colorKeys">Color keys of the gradient (maximum 8 color keys).</param>
		/// <param name="alphaKeys">Alpha keys of the gradient (maximum 8 alpha keys).</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetKeys(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys);
	}
}
