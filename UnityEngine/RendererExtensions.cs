namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Extension methods to the Renderer class, used only for the UpdateGIMaterials method used by the Global Illumination System.</para>
    /// </summary>
    public static class RendererExtensions
    {
        /// <summary>
        /// <para>Schedules an update of the albedo and emissive Textures of a system that contains the Renderer.</para>
        /// </summary>
        /// <param name="renderer"></param>
        public static void UpdateGIMaterials(this Renderer renderer)
        {
            UpdateGIMaterialsForRenderer(renderer);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void UpdateGIMaterialsForRenderer(Renderer renderer);
    }
}

