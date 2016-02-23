using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	/// <summary>
	///   <para>Renders particles on to the screen (Shuriken).</para>
	/// </summary>
	public sealed class ParticleSystemRenderer : Renderer
	{
		/// <summary>
		///   <para>How particles are drawn.</para>
		/// </summary>
		public extern ParticleSystemRenderMode renderMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How much are the particles stretched in their direction of motion.</para>
		/// </summary>
		public extern float lengthScale
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How much are the particles stretched depending on "how fast they move".</para>
		/// </summary>
		public extern float velocityScale
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How much are the particles stretched depending on the Camera's speed.</para>
		/// </summary>
		public extern float cameraVelocityScale
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How much are billboard particle normals oriented towards the camera.</para>
		/// </summary>
		public extern float normalDirection
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Control the direction that particles face.</para>
		/// </summary>
		public extern ParticleSystemRenderSpace alignment
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Modify the pivot point used for rotating particles.</para>
		/// </summary>
		public Vector3 pivot
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_pivot(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_pivot(ref value);
			}
		}

		/// <summary>
		///   <para>Sort particles within a system.</para>
		/// </summary>
		public extern ParticleSystemSortMode sortMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Biases particle system sorting amongst other transparencies.</para>
		/// </summary>
		public extern float sortingFudge
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Clamp the minimum particle size.</para>
		/// </summary>
		public extern float minParticleSize
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Clamp the maximum particle size.</para>
		/// </summary>
		public extern float maxParticleSize
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Mesh used as particle instead of billboarded texture.</para>
		/// </summary>
		public extern Mesh mesh
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal extern bool editorEnabled
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
		private extern void INTERNAL_get_pivot(out Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_pivot(ref Vector3 value);
	}
}
