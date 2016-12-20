namespace UnityEditor.Sprites
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Current Sprite Packer job definition.</para>
    /// </summary>
    public sealed class PackerJob
    {
        internal PackerJob()
        {
        }

        /// <summary>
        /// <para>Registers a new atlas.</para>
        /// </summary>
        /// <param name="atlasName"></param>
        /// <param name="settings"></param>
        public void AddAtlas(string atlasName, AtlasSettings settings)
        {
            this.AddAtlas_Internal(atlasName, ref settings);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void AddAtlas_Internal(string atlasName, ref AtlasSettings settings);
        /// <summary>
        /// <para>Assigns a Sprite to an already registered atlas.</para>
        /// </summary>
        /// <param name="atlasName"></param>
        /// <param name="sprite"></param>
        /// <param name="packingMode"></param>
        /// <param name="packingRotation"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void AssignToAtlas(string atlasName, Sprite sprite, SpritePackingMode packingMode, SpritePackingRotation packingRotation);
    }
}

