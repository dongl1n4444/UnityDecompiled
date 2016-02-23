using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	/// <summary>
	///   <para>Renders a Sprite for 2D graphics.</para>
	/// </summary>
	public sealed class SpriteRenderer : Renderer
	{
		/// <summary>
		///   <para>The Sprite to render.</para>
		/// </summary>
		public Sprite sprite
		{
			get
			{
				return this.GetSprite_INTERNAL();
			}
			set
			{
				this.SetSprite_INTERNAL(value);
			}
		}

		/// <summary>
		///   <para>Rendering color for the Sprite graphic.</para>
		/// </summary>
		public Color color
		{
			get
			{
				Color result;
				this.INTERNAL_get_color(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_color(ref value);
			}
		}

		/// <summary>
		///   <para>Flips the sprite on the X axis.</para>
		/// </summary>
		public extern bool flipX
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Flips the sprite on the Y axis.</para>
		/// </summary>
		public extern bool flipY
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
		private extern Sprite GetSprite_INTERNAL();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetSprite_INTERNAL(Sprite sprite);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_color(out Color value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_color(ref Color value);
	}
}
