namespace UnityEngine.Playables
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Playable used to mix AnimationPlayables when used in Layers.</para>
    /// </summary>
    [RequiredByNativeCode]
    public class AnimationLayerMixerPlayable : AnimationPlayable
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_IsLayerAdditiveInternal(ref PlayableHandle handle, uint layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetLayerAdditiveInternal(ref PlayableHandle handle, uint layerIndex, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetLayerMaskFromAvatarMaskInternal(ref PlayableHandle handle, uint layerIndex, AvatarMask mask);
        /// <summary>
        /// <para>Returns true if the layer is additive.</para>
        /// </summary>
        /// <param name="layerIndex">The layer's index.</param>
        /// <returns>
        /// <para>Returns true if the layer is additive.</para>
        /// </returns>
        public bool IsLayerAdditive(uint layerIndex)
        {
            if (layerIndex >= this.handle.inputCount)
            {
                throw new ArgumentOutOfRangeException("layerIndex", $"layerIndex {layerIndex} must be in the range of 0 to {this.handle.inputCount - 1}.");
            }
            return IsLayerAdditiveInternal(ref this.handle, layerIndex);
        }

        private static bool IsLayerAdditiveInternal(ref PlayableHandle handle, uint layerIndex) => 
            INTERNAL_CALL_IsLayerAdditiveInternal(ref handle, layerIndex);

        /// <summary>
        /// <para>Specifies whether a layer is additive or not. Additive layers are blended with previous layers.</para>
        /// </summary>
        /// <param name="layerIndex">The layer's index.</param>
        /// <param name="value">Whether or not the layer is additive. Pass true if the layer is additive, and false if it is not.</param>
        public void SetLayerAdditive(uint layerIndex, bool value)
        {
            if (layerIndex >= this.handle.inputCount)
            {
                throw new ArgumentOutOfRangeException("layerIndex", $"layerIndex {layerIndex} must be in the range of 0 to {this.handle.inputCount - 1}.");
            }
            SetLayerAdditiveInternal(ref this.handle, layerIndex, value);
        }

        private static void SetLayerAdditiveInternal(ref PlayableHandle handle, uint layerIndex, bool value)
        {
            INTERNAL_CALL_SetLayerAdditiveInternal(ref handle, layerIndex, value);
        }

        /// <summary>
        /// <para>Sets the layer's current mask.</para>
        /// </summary>
        /// <param name="layerIndex">The layer's index.</param>
        /// <param name="mask">The AvatarMask used to create the new LayerMask.</param>
        public void SetLayerMaskFromAvatarMask(uint layerIndex, AvatarMask mask)
        {
            if (layerIndex >= this.handle.inputCount)
            {
                throw new ArgumentOutOfRangeException("layerIndex", $"layerIndex {layerIndex} must be in the range of 0 to {this.handle.inputCount - 1}.");
            }
            if (mask == null)
            {
                throw new ArgumentNullException("mask");
            }
            SetLayerMaskFromAvatarMaskInternal(ref this.handle, layerIndex, mask);
        }

        private static void SetLayerMaskFromAvatarMaskInternal(ref PlayableHandle handle, uint layerIndex, AvatarMask mask)
        {
            INTERNAL_CALL_SetLayerMaskFromAvatarMaskInternal(ref handle, layerIndex, mask);
        }
    }
}

