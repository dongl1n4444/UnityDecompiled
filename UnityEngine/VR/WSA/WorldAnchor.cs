namespace UnityEngine.VR.WSA
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The WorldAnchor component allows a GameObject's position to be locked in physical space.</para>
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public sealed class WorldAnchor : Component
    {
        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event OnTrackingChangedDelegate OnTrackingChanged;

        private WorldAnchor()
        {
        }

        /// <summary>
        /// <para>Retrieve a native pointer to the &lt;a href="https:msdn.microsoft.comen-uslibrarywindowsappswindows.perception.spatial.spatialanchor.aspx"&gt;Windows.Perception.Spatial.SpatialAnchor&lt;a&gt; COM object.
        /// This function calls &lt;a href=" https:msdn.microsoft.comen-uslibrarywindowsdesktopms691379.aspx"&gt;IUnknown::AddRef&lt;a&gt; on the pointer before returning it. The pointer must be released by calling &lt;a href=" https:msdn.microsoft.comen-uslibrarywindowsdesktopms682317.aspx"&gt;IUnknown::Release&lt;a&gt;.</para>
        /// </summary>
        /// <returns>
        /// <para>The native pointer to the &lt;a href=" https:msdn.microsoft.comen-uslibrarywindowsappswindows.perception.spatial.spatialanchor.aspx"&gt;Windows.Perception.Spatial.SpatialAnchor&lt;a&gt; COM object.</para>
        /// </returns>
        public IntPtr GetNativeSpatialAnchorPtr() => 
            this.GetSpatialAnchor_Internal();

        private IntPtr GetSpatialAnchor_Internal()
        {
            IntPtr ptr;
            INTERNAL_CALL_GetSpatialAnchor_Internal(this, out ptr);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetSpatialAnchor_Internal(WorldAnchor self, out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_IsLocated_Internal(WorldAnchor self);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetSpatialAnchor_Internal_FromScript(WorldAnchor self, IntPtr spatialAnchorPtr);
        [RequiredByNativeCode]
        private static void Internal_TriggerEventOnTrackingLost(WorldAnchor self, bool located)
        {
            if ((self != null) && (self.OnTrackingChanged != null))
            {
                self.OnTrackingChanged(self, located);
            }
        }

        private bool IsLocated_Internal() => 
            INTERNAL_CALL_IsLocated_Internal(this);

        /// <summary>
        /// <para>Assigns the &lt;a href="https:msdn.microsoft.comen-uslibrarywindowsappswindows.perception.spatial.spatialanchor.aspx"&gt;Windows.Perception.Spatial.SpatialAnchor&lt;a&gt; COM pointer maintained by this WorldAnchor.</para>
        /// </summary>
        /// <param name="spatialAnchorPtr">A live &lt;a href="https:msdn.microsoft.comen-uslibrarywindowsappswindows.perception.spatial.spatialanchor.aspx"&gt;Windows.Perception.Spatial.SpatialAnchor&lt;a&gt; COM pointer.</param>
        public void SetNativeSpatialAnchorPtr(IntPtr spatialAnchorPtr)
        {
            this.SetSpatialAnchor_Internal_FromScript(spatialAnchorPtr);
        }

        private void SetSpatialAnchor_Internal_FromScript(IntPtr spatialAnchorPtr)
        {
            INTERNAL_CALL_SetSpatialAnchor_Internal_FromScript(this, spatialAnchorPtr);
        }

        /// <summary>
        /// <para>Returns true if this WorldAnchor is located (read only).  A return of false typically indicates a loss of tracking.</para>
        /// </summary>
        public bool isLocated =>
            this.IsLocated_Internal();

        /// <summary>
        /// <para>Event that is fired when this object's tracking state changes.</para>
        /// </summary>
        /// <param name="located">Set to true if the object is locatable.</param>
        /// <param name="self">The WorldAnchor reporting the tracking state change.</param>
        public delegate void OnTrackingChangedDelegate(WorldAnchor self, bool located);
    }
}

