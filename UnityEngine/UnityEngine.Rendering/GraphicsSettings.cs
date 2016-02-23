using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Script interface for.</para>
	/// </summary>
	public sealed class GraphicsSettings : Object
	{
		/// <summary>
		///   <para>Set built-in shader mode.</para>
		/// </summary>
		/// <param name="type">Built-in shader type to change.</param>
		/// <param name="mode">Mode to use for built-in shader.</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShaderMode(BuiltinShaderType type, BuiltinShaderMode mode);

		/// <summary>
		///   <para>Get built-in shader mode.</para>
		/// </summary>
		/// <param name="type">Built-in shader type to query.</param>
		/// <returns>
		///   <para>Mode used for built-in shader.</para>
		/// </returns>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern BuiltinShaderMode GetShaderMode(BuiltinShaderType type);

		/// <summary>
		///   <para>Set custom shader to use instead of a built-in shader.</para>
		/// </summary>
		/// <param name="type">Built-in shader type to set custom shader to.</param>
		/// <param name="shader">The shader to use.</param>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetCustomShader(BuiltinShaderType type, Shader shader);

		/// <summary>
		///   <para>Get custom shader used instead of a built-in shader.</para>
		/// </summary>
		/// <param name="type">Built-in shader type to query custom shader for.</param>
		/// <returns>
		///   <para>The shader used.</para>
		/// </returns>
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Shader GetCustomShader(BuiltinShaderType type);
	}
}
