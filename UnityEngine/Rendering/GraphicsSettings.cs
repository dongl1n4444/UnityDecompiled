namespace UnityEngine.Rendering
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Experimental.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Script interface for.</para>
    /// </summary>
    public sealed class GraphicsSettings : UnityEngine.Object
    {
        /// <summary>
        /// <para>Get custom shader used instead of a built-in shader.</para>
        /// </summary>
        /// <param name="type">Built-in shader type to query custom shader for.</param>
        /// <returns>
        /// <para>The shader used.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern Shader GetCustomShader(BuiltinShaderType type);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern UnityEngine.Object GetGraphicsSettings();
        /// <summary>
        /// <para>Get built-in shader mode.</para>
        /// </summary>
        /// <param name="type">Built-in shader type to query.</param>
        /// <returns>
        /// <para>Mode used for built-in shader.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern BuiltinShaderMode GetShaderMode(BuiltinShaderType type);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_transparencySortAxis(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_set_transparencySortAxis(ref Vector3 value);
        /// <summary>
        /// <para>Set custom shader to use instead of a built-in shader.</para>
        /// </summary>
        /// <param name="type">Built-in shader type to set custom shader to.</param>
        /// <param name="shader">The shader to use.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetCustomShader(BuiltinShaderType type, Shader shader);
        /// <summary>
        /// <para>Set built-in shader mode.</para>
        /// </summary>
        /// <param name="type">Built-in shader type to change.</param>
        /// <param name="mode">Mode to use for built-in shader.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetShaderMode(BuiltinShaderType type, BuiltinShaderMode mode);

        private static ScriptableObject INTERNAL_renderPipelineAsset { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Whether to use a Light's color temperature when calculating the final color of that Light."</para>
        /// </summary>
        public static bool lightsUseColorTemperature { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>If this is true, Light intensity is multiplied against linear color values. If it is false, gamma color values are used.</para>
        /// </summary>
        public static bool lightsUseLinearIntensity { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The RenderPipelineAsset that describes how the Scene should be rendered.</para>
        /// </summary>
        public static RenderPipelineAsset renderPipelineAsset
        {
            get => 
                (INTERNAL_renderPipelineAsset as RenderPipelineAsset);
            set
            {
                INTERNAL_renderPipelineAsset = value;
            }
        }

        /// <summary>
        /// <para>An axis that describes the direction along which the distances of objects are measured for the purpose of sorting.</para>
        /// </summary>
        public static Vector3 transparencySortAxis
        {
            get
            {
                Vector3 vector;
                INTERNAL_get_transparencySortAxis(out vector);
                return vector;
            }
            set
            {
                INTERNAL_set_transparencySortAxis(ref value);
            }
        }

        /// <summary>
        /// <para>Transparent object sorting mode.</para>
        /// </summary>
        public static TransparencySortMode transparencySortMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

