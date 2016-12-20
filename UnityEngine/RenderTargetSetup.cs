namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;

    /// <summary>
    /// <para>Fully describes setup of RenderTarget.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RenderTargetSetup
    {
        /// <summary>
        /// <para>Color Buffers to set.</para>
        /// </summary>
        public RenderBuffer[] color;
        /// <summary>
        /// <para>Depth Buffer to set.</para>
        /// </summary>
        public RenderBuffer depth;
        /// <summary>
        /// <para>Mip Level to render to.</para>
        /// </summary>
        public int mipLevel;
        /// <summary>
        /// <para>Cubemap face to render to.</para>
        /// </summary>
        public CubemapFace cubemapFace;
        /// <summary>
        /// <para>Slice of a Texture3D or Texture2DArray to set as a render target.</para>
        /// </summary>
        public int depthSlice;
        /// <summary>
        /// <para>Load Actions for Color Buffers. It will override any actions set on RenderBuffers themselves.</para>
        /// </summary>
        public RenderBufferLoadAction[] colorLoad;
        /// <summary>
        /// <para>Store Actions for Color Buffers. It will override any actions set on RenderBuffers themselves.</para>
        /// </summary>
        public RenderBufferStoreAction[] colorStore;
        /// <summary>
        /// <para>Load Action for Depth Buffer. It will override any actions set on RenderBuffer itself.</para>
        /// </summary>
        public RenderBufferLoadAction depthLoad;
        /// <summary>
        /// <para>Store Actions for Depth Buffer. It will override any actions set on RenderBuffer itself.</para>
        /// </summary>
        public RenderBufferStoreAction depthStore;
        public RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth, int mip, CubemapFace face, RenderBufferLoadAction[] colorLoad, RenderBufferStoreAction[] colorStore, RenderBufferLoadAction depthLoad, RenderBufferStoreAction depthStore)
        {
            this.color = color;
            this.depth = depth;
            this.mipLevel = mip;
            this.cubemapFace = face;
            this.depthSlice = 0;
            this.colorLoad = colorLoad;
            this.colorStore = colorStore;
            this.depthLoad = depthLoad;
            this.depthStore = depthStore;
        }

        /// <summary>
        /// <para>Constructs RenderTargetSetup.</para>
        /// </summary>
        /// <param name="color">Color Buffer(s) to set.</param>
        /// <param name="depth">Depth Buffer to set.</param>
        /// <param name="mipLevel">Mip Level to render to.</param>
        /// <param name="face">Cubemap face to render to.</param>
        /// <param name="mip"></param>
        public RenderTargetSetup(RenderBuffer color, RenderBuffer depth) : this(bufferArray1, depth)
        {
            RenderBuffer[] bufferArray1 = new RenderBuffer[] { color };
        }

        /// <summary>
        /// <para>Constructs RenderTargetSetup.</para>
        /// </summary>
        /// <param name="color">Color Buffer(s) to set.</param>
        /// <param name="depth">Depth Buffer to set.</param>
        /// <param name="mipLevel">Mip Level to render to.</param>
        /// <param name="face">Cubemap face to render to.</param>
        /// <param name="mip"></param>
        public RenderTargetSetup(RenderBuffer color, RenderBuffer depth, int mipLevel) : this(bufferArray1, depth, mipLevel)
        {
            RenderBuffer[] bufferArray1 = new RenderBuffer[] { color };
        }

        /// <summary>
        /// <para>Constructs RenderTargetSetup.</para>
        /// </summary>
        /// <param name="color">Color Buffer(s) to set.</param>
        /// <param name="depth">Depth Buffer to set.</param>
        /// <param name="mipLevel">Mip Level to render to.</param>
        /// <param name="face">Cubemap face to render to.</param>
        /// <param name="mip"></param>
        public RenderTargetSetup(RenderBuffer color, RenderBuffer depth, int mipLevel, CubemapFace face) : this(bufferArray1, depth, mipLevel, face)
        {
            RenderBuffer[] bufferArray1 = new RenderBuffer[] { color };
        }

        public RenderTargetSetup(RenderBuffer color, RenderBuffer depth, int mipLevel, CubemapFace face, int depthSlice) : this(bufferArray1, depth, mipLevel, face)
        {
            RenderBuffer[] bufferArray1 = new RenderBuffer[] { color };
            this.depthSlice = depthSlice;
        }

        /// <summary>
        /// <para>Constructs RenderTargetSetup.</para>
        /// </summary>
        /// <param name="color">Color Buffer(s) to set.</param>
        /// <param name="depth">Depth Buffer to set.</param>
        /// <param name="mipLevel">Mip Level to render to.</param>
        /// <param name="face">Cubemap face to render to.</param>
        /// <param name="mip"></param>
        public RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth) : this(color, depth, 0, CubemapFace.Unknown)
        {
        }

        /// <summary>
        /// <para>Constructs RenderTargetSetup.</para>
        /// </summary>
        /// <param name="color">Color Buffer(s) to set.</param>
        /// <param name="depth">Depth Buffer to set.</param>
        /// <param name="mipLevel">Mip Level to render to.</param>
        /// <param name="face">Cubemap face to render to.</param>
        /// <param name="mip"></param>
        public RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth, int mipLevel) : this(color, depth, mipLevel, CubemapFace.Unknown)
        {
        }

        /// <summary>
        /// <para>Constructs RenderTargetSetup.</para>
        /// </summary>
        /// <param name="color">Color Buffer(s) to set.</param>
        /// <param name="depth">Depth Buffer to set.</param>
        /// <param name="mipLevel">Mip Level to render to.</param>
        /// <param name="face">Cubemap face to render to.</param>
        /// <param name="mip"></param>
        public RenderTargetSetup(RenderBuffer[] color, RenderBuffer depth, int mip, CubemapFace face) : this(color, depth, mip, face, LoadActions(color), StoreActions(color), depth.loadAction, depth.storeAction)
        {
        }

        internal static RenderBufferLoadAction[] LoadActions(RenderBuffer[] buf)
        {
            RenderBufferLoadAction[] actionArray = new RenderBufferLoadAction[buf.Length];
            for (int i = 0; i < buf.Length; i++)
            {
                actionArray[i] = buf[i].loadAction;
                buf[i].loadAction = RenderBufferLoadAction.Load;
            }
            return actionArray;
        }

        internal static RenderBufferStoreAction[] StoreActions(RenderBuffer[] buf)
        {
            RenderBufferStoreAction[] actionArray = new RenderBufferStoreAction[buf.Length];
            for (int i = 0; i < buf.Length; i++)
            {
                actionArray[i] = buf[i].storeAction;
                buf[i].storeAction = RenderBufferStoreAction.Store;
            }
            return actionArray;
        }
    }
}

