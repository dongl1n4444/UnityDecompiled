namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;

    /// <summary>
    /// <para>Color or depth buffer part of a RenderTexture.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RenderBuffer
    {
        internal int m_RenderTextureInstanceID;
        internal IntPtr m_BufferPtr;
        internal void SetLoadAction(RenderBufferLoadAction action)
        {
            RenderBufferHelper.SetLoadAction(out this, (int) action);
        }

        internal void SetStoreAction(RenderBufferStoreAction action)
        {
            RenderBufferHelper.SetStoreAction(out this, (int) action);
        }

        internal RenderBufferLoadAction loadAction
        {
            get => 
                ((RenderBufferLoadAction) RenderBufferHelper.GetLoadAction(out this));
            set
            {
                this.SetLoadAction(value);
            }
        }
        internal RenderBufferStoreAction storeAction
        {
            get => 
                ((RenderBufferStoreAction) RenderBufferHelper.GetStoreAction(out this));
            set
            {
                this.SetStoreAction(value);
            }
        }
        /// <summary>
        /// <para>Returns native RenderBuffer. Be warned this is not native Texture, but rather pointer to unity struct that can be used with native unity API. Currently such API exists only on iOS.</para>
        /// </summary>
        public IntPtr GetNativeRenderBufferPtr() => 
            RenderBufferHelper.GetNativeRenderBufferPtr(this.m_BufferPtr);
    }
}

