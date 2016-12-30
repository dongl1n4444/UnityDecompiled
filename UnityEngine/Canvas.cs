namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Element that can be used for screen rendering.</para>
    /// </summary>
    [NativeClass("UI::Canvas"), RequireComponent(typeof(RectTransform))]
    public sealed class Canvas : Behaviour
    {
        public static  event WillRenderCanvases willRenderCanvases;

        /// <summary>
        /// <para>Force all canvases to update their content.</para>
        /// </summary>
        public static void ForceUpdateCanvases()
        {
            SendWillRenderCanvases();
        }

        /// <summary>
        /// <para>Returns the default material that can be used for rendering normal elements on the Canvas.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern Material GetDefaultCanvasMaterial();
        /// <summary>
        /// <para>Returns the default material that can be used for rendering text elements on the Canvas.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Shared default material now used for text and general UI elements, call Canvas.GetDefaultCanvasMaterial()"), GeneratedByOldBindingsGenerator]
        public static extern Material GetDefaultCanvasTextMaterial();
        /// <summary>
        /// <para>Gets or generates the ETC1 Material.</para>
        /// </summary>
        /// <returns>
        /// <para>The generated ETC1 Material from the Canvas.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern Material GetETC1SupportedCanvasMaterial();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_pixelRect(out Rect value);
        [RequiredByNativeCode]
        private static void SendWillRenderCanvases()
        {
            if (willRenderCanvases != null)
            {
                willRenderCanvases();
            }
        }

        /// <summary>
        /// <para>Cached calculated value based upon SortingLayerID.</para>
        /// </summary>
        public int cachedSortingLayerValue { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Is this the root Canvas?</para>
        /// </summary>
        public bool isRootCanvas { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The normalized grid size that the canvas will split the renderable area into.</para>
        /// </summary>
        public float normalizedSortingGridSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Allows for nested canvases to override pixelPerfect settings inherited from parent canvases.</para>
        /// </summary>
        public bool overridePixelPerfect { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Override the sorting of canvas.</para>
        /// </summary>
        public bool overrideSorting { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Force elements in the canvas to be aligned with pixels. Only applies with renderMode is Screen Space.</para>
        /// </summary>
        public bool pixelPerfect { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Get the render rect for the Canvas.</para>
        /// </summary>
        public Rect pixelRect
        {
            get
            {
                Rect rect;
                this.INTERNAL_get_pixelRect(out rect);
                return rect;
            }
        }

        /// <summary>
        /// <para>How far away from the camera is the Canvas generated.</para>
        /// </summary>
        public float planeDistance { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The number of pixels per unit that is considered the default.</para>
        /// </summary>
        public float referencePixelsPerUnit { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Is the Canvas in World or Overlay mode?</para>
        /// </summary>
        public RenderMode renderMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The render order in which the canvas is being emitted to the scene.</para>
        /// </summary>
        public int renderOrder { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns the Canvas closest to root, by checking through each parent and returning the last canvas found. If no other canvas is found then the canvas will return itself.</para>
        /// </summary>
        public Canvas rootCanvas { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Used to scale the entire canvas, while still making it fit the screen. Only applies with renderMode is Screen Space.</para>
        /// </summary>
        public float scaleFactor { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The normalized grid size that the canvas will split the renderable area into.</para>
        /// </summary>
        [Obsolete("Setting normalizedSize via a int is not supported. Please use normalizedSortingGridSize")]
        public int sortingGridNormalizedSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Unique ID of the Canvas' sorting layer.</para>
        /// </summary>
        public int sortingLayerID { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Name of the Canvas' sorting layer.</para>
        /// </summary>
        public string sortingLayerName { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Canvas' order within a sorting layer.</para>
        /// </summary>
        public int sortingOrder { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>For Overlay mode, display index on which the UI canvas will appear.</para>
        /// </summary>
        public int targetDisplay { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Camera used for sizing the Canvas when in Screen Space - Camera. Also used as the Camera that events will be sent through for a World Space [[Canvas].</para>
        /// </summary>
        public Camera worldCamera { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public delegate void WillRenderCanvases();
    }
}

