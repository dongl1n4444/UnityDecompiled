namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Low-level graphics library.</para>
    /// </summary>
    public sealed class GL
    {
        /// <summary>
        /// <para>Mode for Begin: draw line strip.</para>
        /// </summary>
        public const int LINE_STRIP = 2;
        /// <summary>
        /// <para>Mode for Begin: draw lines.</para>
        /// </summary>
        public const int LINES = 1;
        /// <summary>
        /// <para>Mode for Begin: draw quads.</para>
        /// </summary>
        public const int QUADS = 7;
        /// <summary>
        /// <para>Mode for Begin: draw triangle strip.</para>
        /// </summary>
        public const int TRIANGLE_STRIP = 5;
        /// <summary>
        /// <para>Mode for Begin: draw triangles.</para>
        /// </summary>
        public const int TRIANGLES = 4;

        /// <summary>
        /// <para>Begin drawing 3D primitives.</para>
        /// </summary>
        /// <param name="mode">Primitives to draw: can be TRIANGLES, TRIANGLE_STRIP, QUADS or LINES.</param>
        public static void Begin(int mode)
        {
            BeginInternal(mode);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void BeginInternal(int mode);
        [ExcludeFromDocs]
        public static void Clear(bool clearDepth, bool clearColor, UnityEngine.Color backgroundColor)
        {
            float depth = 1f;
            Clear(clearDepth, clearColor, backgroundColor, depth);
        }

        /// <summary>
        /// <para>Clear the current render buffer.</para>
        /// </summary>
        /// <param name="clearDepth">Should the depth buffer be cleared?</param>
        /// <param name="clearColor">Should the color buffer be cleared?</param>
        /// <param name="backgroundColor">The color to clear with, used only if clearColor is true.</param>
        /// <param name="depth">The depth to clear Z buffer with, used only if clearDepth is true.</param>
        public static void Clear(bool clearDepth, bool clearColor, UnityEngine.Color backgroundColor, [DefaultValue("1.0f")] float depth)
        {
            Internal_Clear(clearDepth, clearColor, backgroundColor, depth);
        }

        /// <summary>
        /// <para>Clear the current render buffer with camera's skybox.</para>
        /// </summary>
        /// <param name="clearDepth">Should the depth buffer be cleared?</param>
        /// <param name="camera">Camera to get projection parameters and skybox from.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void ClearWithSkybox(bool clearDepth, Camera camera);
        /// <summary>
        /// <para>Sets current vertex color.</para>
        /// </summary>
        /// <param name="c"></param>
        public static void Color(UnityEngine.Color c)
        {
            INTERNAL_CALL_Color(ref c);
        }

        /// <summary>
        /// <para>End drawing 3D primitives.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void End();
        /// <summary>
        /// <para>Sends queued-up commands in the driver's command buffer to the GPU.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Flush();
        /// <summary>
        /// <para>Compute GPU projection matrix from camera's projection matrix.</para>
        /// </summary>
        /// <param name="proj">Source projection matrix.</param>
        /// <param name="renderIntoTexture">Will this projection be used for rendering into a RenderTexture?</param>
        /// <returns>
        /// <para>Adjusted projection matrix for the current graphics API.</para>
        /// </returns>
        public static Matrix4x4 GetGPUProjectionMatrix(Matrix4x4 proj, bool renderIntoTexture)
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_GetGPUProjectionMatrix(ref proj, renderIntoTexture, out matrixx);
            return matrixx;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Color(ref UnityEngine.Color c);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetGPUProjectionMatrix(ref Matrix4x4 proj, bool renderIntoTexture, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_Clear(bool clearDepth, bool clearColor, ref UnityEngine.Color backgroundColor, float depth);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_LoadProjectionMatrix(ref Matrix4x4 mat);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_MultiTexCoord(int unit, ref Vector3 v);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_MultMatrix(ref Matrix4x4 mat);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_TexCoord(ref Vector3 v);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Vertex(ref Vector3 v);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Viewport(ref Rect pixelRect);
        private static void Internal_Clear(bool clearDepth, bool clearColor, UnityEngine.Color backgroundColor, float depth)
        {
            INTERNAL_CALL_Internal_Clear(clearDepth, clearColor, ref backgroundColor, depth);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_modelview(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_set_modelview(ref Matrix4x4 value);
        /// <summary>
        /// <para>Invalidate the internally cached render state.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void InvalidateState();
        /// <summary>
        /// <para>Send a user-defined event to a native code plugin.</para>
        /// </summary>
        /// <param name="eventID">User defined id to send to the callback.</param>
        /// <param name="callback">Native code callback to queue for Unity's renderer to invoke.</param>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("IssuePluginEvent(eventID) is deprecated. Use IssuePluginEvent(callback, eventID) instead."), GeneratedByOldBindingsGenerator]
        public static extern void IssuePluginEvent(int eventID);
        /// <summary>
        /// <para>Send a user-defined event to a native code plugin.</para>
        /// </summary>
        /// <param name="eventID">User defined id to send to the callback.</param>
        /// <param name="callback">Native code callback to queue for Unity's renderer to invoke.</param>
        public static void IssuePluginEvent(IntPtr callback, int eventID)
        {
            if (callback == IntPtr.Zero)
            {
                throw new ArgumentException("Null callback specified.");
            }
            IssuePluginEventInternal(callback, eventID);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void IssuePluginEventInternal(IntPtr callback, int eventID);
        /// <summary>
        /// <para>Load the identity matrix to the current modelview matrix.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void LoadIdentity();
        /// <summary>
        /// <para>Helper function to set up an ortho perspective transform.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void LoadOrtho();
        /// <summary>
        /// <para>Setup a matrix for pixel-correct rendering.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void LoadPixelMatrix();
        /// <summary>
        /// <para>Setup a matrix for pixel-correct rendering.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="top"></param>
        public static void LoadPixelMatrix(float left, float right, float bottom, float top)
        {
            LoadPixelMatrixArgs(left, right, bottom, top);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void LoadPixelMatrixArgs(float left, float right, float bottom, float top);
        /// <summary>
        /// <para>Load an arbitrary matrix to the current projection matrix.</para>
        /// </summary>
        /// <param name="mat"></param>
        public static void LoadProjectionMatrix(Matrix4x4 mat)
        {
            INTERNAL_CALL_LoadProjectionMatrix(ref mat);
        }

        /// <summary>
        /// <para>Sets current texture coordinate (v.x,v.y,v.z) to the actual texture unit.</para>
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="v"></param>
        public static void MultiTexCoord(int unit, Vector3 v)
        {
            INTERNAL_CALL_MultiTexCoord(unit, ref v);
        }

        /// <summary>
        /// <para>Sets current texture coordinate (x,y) for the actual texture unit.</para>
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void MultiTexCoord2(int unit, float x, float y);
        /// <summary>
        /// <para>Sets current texture coordinate (x,y,z) to the actual texture unit.</para>
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void MultiTexCoord3(int unit, float x, float y, float z);
        /// <summary>
        /// <para>Multiplies the current modelview matrix with the one specified.</para>
        /// </summary>
        /// <param name="mat"></param>
        public static void MultMatrix(Matrix4x4 mat)
        {
            INTERNAL_CALL_MultMatrix(ref mat);
        }

        /// <summary>
        /// <para>Restores both projection and modelview matrices off the top of the matrix stack.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void PopMatrix();
        /// <summary>
        /// <para>Saves both projection and modelview matrices to the matrix stack.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void PushMatrix();
        /// <summary>
        /// <para>Resolves the render target for subsequent operations sampling from it.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void RenderTargetBarrier();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, Obsolete("Use invertCulling property")]
        public static extern void SetRevertBackfacing(bool revertBackFaces);
        /// <summary>
        /// <para>Sets current texture coordinate (v.x,v.y,v.z) for all texture units.</para>
        /// </summary>
        /// <param name="v"></param>
        public static void TexCoord(Vector3 v)
        {
            INTERNAL_CALL_TexCoord(ref v);
        }

        /// <summary>
        /// <para>Sets current texture coordinate (x,y) for all texture units.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void TexCoord2(float x, float y);
        /// <summary>
        /// <para>Sets current texture coordinate (x,y,z) for all texture units.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void TexCoord3(float x, float y, float z);
        /// <summary>
        /// <para>Submit a vertex.</para>
        /// </summary>
        /// <param name="v"></param>
        public static void Vertex(Vector3 v)
        {
            INTERNAL_CALL_Vertex(ref v);
        }

        /// <summary>
        /// <para>Submit a vertex.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Vertex3(float x, float y, float z);
        /// <summary>
        /// <para>Set the rendering viewport.</para>
        /// </summary>
        /// <param name="pixelRect"></param>
        public static void Viewport(Rect pixelRect)
        {
            INTERNAL_CALL_Viewport(ref pixelRect);
        }

        /// <summary>
        /// <para>Select whether to invert the backface culling (true) or not (false).</para>
        /// </summary>
        public static bool invertCulling { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The current modelview matrix.</para>
        /// </summary>
        public static Matrix4x4 modelview
        {
            get
            {
                Matrix4x4 matrixx;
                INTERNAL_get_modelview(out matrixx);
                return matrixx;
            }
            set
            {
                INTERNAL_set_modelview(ref value);
            }
        }

        /// <summary>
        /// <para>Controls whether Linear-to-sRGB color conversion is performed while rendering.</para>
        /// </summary>
        public static bool sRGBWrite { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Should rendering be done in wireframe?</para>
        /// </summary>
        public static bool wireframe { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

