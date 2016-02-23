using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Built-in shader types used by Rendering.GraphicsSettings.</para>
	/// </summary>
	public enum BuiltinShaderType
	{
		/// <summary>
		///   <para>Shader used for deferred shading calculations.</para>
		/// </summary>
		DeferredShading,
		/// <summary>
		///   <para>Shader used for deferred reflection probes.</para>
		/// </summary>
		DeferredReflections,
		/// <summary>
		///   <para>Shader used for legacy deferred lighting calculations.</para>
		/// </summary>
		LegacyDeferredLighting
	}
}
