namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>A script interface for a.</para>
    /// </summary>
    public sealed class Projector : Behaviour
    {
        /// <summary>
        /// <para>The aspect ratio of the projection.</para>
        /// </summary>
        public float aspectRatio { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The far clipping plane distance.</para>
        /// </summary>
        public float farClipPlane { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The field of view of the projection in degrees.</para>
        /// </summary>
        public float fieldOfView { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Which object layers are ignored by the projector.</para>
        /// </summary>
        public int ignoreLayers { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [Obsolete("Property isOrthoGraphic has been deprecated. Use orthographic instead (UnityUpgradable) -> orthographic", true), EditorBrowsable(EditorBrowsableState.Never)]
        public bool isOrthoGraphic
        {
            get => 
                false;
            set
            {
            }
        }

        /// <summary>
        /// <para>The material that will be projected onto every object.</para>
        /// </summary>
        public Material material { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The near clipping plane distance.</para>
        /// </summary>
        public float nearClipPlane { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Is the projection orthographic (true) or perspective (false)?</para>
        /// </summary>
        public bool orthographic { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Projection's half-size when in orthographic mode.</para>
        /// </summary>
        public float orthographicSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property orthoGraphicSize has been deprecated. Use orthographicSize instead (UnityUpgradable) -> orthographicSize", true)]
        public float orthoGraphicSize
        {
            get => 
                -1f;
            set
            {
            }
        }
    }
}

