namespace UnityEngine.Experimental.Rendering
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Describes which subset of visible objects to render.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct InputFilter
    {
        /// <summary>
        /// <para>Render objects that have material render queue larger or equal to this.</para>
        /// </summary>
        public int renderQueueMin;
        /// <summary>
        /// <para>Render objects that have material render queue smaller or equal to this.</para>
        /// </summary>
        public int renderQueueMax;
        /// <summary>
        /// <para>Only render objects in the given layer mask.</para>
        /// </summary>
        public int layerMask;
        /// <summary>
        /// <para>Default input filter: render all objects.</para>
        /// </summary>
        public static InputFilter Default() => 
            new InputFilter { 
                renderQueueMin = 0,
                renderQueueMax = 0x1388,
                layerMask = -1
            };

        /// <summary>
        /// <para>Set to render only opaque objects.</para>
        /// </summary>
        public void SetQueuesOpaque()
        {
            this.renderQueueMin = 0;
            this.renderQueueMax = 0x9c4;
        }

        /// <summary>
        /// <para>Set to render only transparent objects.</para>
        /// </summary>
        public void SetQueuesTransparent()
        {
            this.renderQueueMin = 0x9c5;
            this.renderQueueMax = 0x1388;
        }
    }
}

