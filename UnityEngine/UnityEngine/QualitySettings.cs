using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	/// <summary>
	///   <para>Script interface for.</para>
	/// </summary>
	public sealed class QualitySettings : Object
	{
		/// <summary>
		///   <para>The indexed list of available Quality Settings.</para>
		/// </summary>
		public static extern string[] names
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Use GetQualityLevel and SetQualityLevel")]
		public static extern QualityLevel currentLevel
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The maximum number of pixel lights that should affect any object.</para>
		/// </summary>
		public static extern int pixelLightCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Directional light shadow projection.</para>
		/// </summary>
		public static extern ShadowProjection shadowProjection
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Number of cascades to use for directional light shadows.</para>
		/// </summary>
		public static extern int shadowCascades
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Shadow drawing distance.</para>
		/// </summary>
		public static extern float shadowDistance
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Offset shadow frustum near plane.</para>
		/// </summary>
		public static extern float shadowNearPlaneOffset
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The normalized cascade distribution for a 2 cascade setup. The value defines the position of the cascade with respect to Zero.</para>
		/// </summary>
		public static extern float shadowCascade2Split
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The normalized cascade start position for a 4 cascade setup. Each member of the vector defines the normalized position of the coresponding cascade with respect to Zero.</para>
		/// </summary>
		public static Vector3 shadowCascade4Split
		{
			get
			{
				Vector3 result;
				QualitySettings.INTERNAL_get_shadowCascade4Split(out result);
				return result;
			}
			set
			{
				QualitySettings.INTERNAL_set_shadowCascade4Split(ref value);
			}
		}

		/// <summary>
		///   <para>A texture size limit applied to all textures.</para>
		/// </summary>
		public static extern int masterTextureLimit
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Global anisotropic filtering mode.</para>
		/// </summary>
		public static extern AnisotropicFiltering anisotropicFiltering
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Global multiplier for the LOD's switching distance.</para>
		/// </summary>
		public static extern float lodBias
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>A maximum LOD level. All LOD groups.</para>
		/// </summary>
		public static extern int maximumLODLevel
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Budget for how many ray casts can be performed per frame for approximate collision testing.</para>
		/// </summary>
		public static extern int particleRaycastBudget
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Use a two-pass shader for the vegetation in the terrain engine.</para>
		/// </summary>
		public static extern bool softVegetation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Enables realtime reflection probes.</para>
		/// </summary>
		public static extern bool realtimeReflectionProbes
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>If enabled, billboards will face towards camera position rather than camera orientation.</para>
		/// </summary>
		public static extern bool billboardsFaceCameraPosition
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Maximum number of frames queued up by graphics driver.</para>
		/// </summary>
		public static extern int maxQueuedFrames
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The VSync Count.</para>
		/// </summary>
		public static extern int vSyncCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Set The AA Filtering option.</para>
		/// </summary>
		public static extern int antiAliasing
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Desired color space (Read Only).</para>
		/// </summary>
		public static extern ColorSpace desiredColorSpace
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Active color space (Read Only).</para>
		/// </summary>
		public static extern ColorSpace activeColorSpace
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Blend weights.</para>
		/// </summary>
		public static extern BlendWeights blendWeights
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Async texture upload provides timesliced async texture upload on the render thread with tight control over memory and timeslicing. There are no allocations except for the ones which driver has to do. To read data and upload texture data a ringbuffer whose size can be controlled is re-used.
		///
		/// Use asyncUploadTimeSlice to set the time-slice in milliseconds for asynchronous texture uploads per 
		/// frame. Minimum value is 1 and maximum is 33.</para>
		/// </summary>
		public static extern int asyncUploadTimeSlice
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Async texture upload provides timesliced async texture upload on the render thread with tight control over memory and timeslicing. There are no allocations except for the ones which driver has to do. To read data and upload texture data a ringbuffer whose size can be controlled is re-used.
		///
		/// Use asyncUploadBufferSize to set the buffer size for asynchronous texture uploads. The size is in megabytes. Minimum value is 2 and maximum is 512. Although the buffer will resize automatically to fit the largest texture currently loading, it is recommended to set the value approximately to the size of biggest texture used in the scene to avoid re-sizing of the buffer which can incur performance cost.</para>
		/// </summary>
		public static extern int asyncUploadBufferSize
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns the current graphics quality level.</para>
		/// </summary>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetQualityLevel();

		/// <summary>
		///   <para>Sets a new graphics quality level.</para>
		/// </summary>
		/// <param name="index">Quality index to set.</param>
		/// <param name="applyExpensiveChanges">Should expensive changes be applied (Anti-aliasing etc).</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetQualityLevel(int index, [DefaultValue("true")] bool applyExpensiveChanges);

		[ExcludeFromDocs]
		public static void SetQualityLevel(int index)
		{
			bool applyExpensiveChanges = true;
			QualitySettings.SetQualityLevel(index, applyExpensiveChanges);
		}

		/// <summary>
		///   <para>Increase the current quality level.</para>
		/// </summary>
		/// <param name="applyExpensiveChanges">Should expensive changes be applied (Anti-aliasing etc).</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void IncreaseLevel([DefaultValue("false")] bool applyExpensiveChanges);

		[ExcludeFromDocs]
		public static void IncreaseLevel()
		{
			bool applyExpensiveChanges = false;
			QualitySettings.IncreaseLevel(applyExpensiveChanges);
		}

		/// <summary>
		///   <para>Decrease the current quality level.</para>
		/// </summary>
		/// <param name="applyExpensiveChanges">Should expensive changes be applied (Anti-aliasing etc).</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DecreaseLevel([DefaultValue("false")] bool applyExpensiveChanges);

		[ExcludeFromDocs]
		public static void DecreaseLevel()
		{
			bool applyExpensiveChanges = false;
			QualitySettings.DecreaseLevel(applyExpensiveChanges);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_shadowCascade4Split(out Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_shadowCascade4Split(ref Vector3 value);
	}
}
