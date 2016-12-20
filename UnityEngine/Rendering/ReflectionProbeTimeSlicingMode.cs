namespace UnityEngine.Rendering
{
    using System;

    /// <summary>
    /// <para>When a probe's ReflectionProbe.refreshMode is set to ReflectionProbeRefreshMode.EveryFrame, this enum specify whether or not Unity should update the probe's cubemap over several frames or update the whole cubemap in one frame.
    /// Updating a probe's cubemap is a costly operation. Unity needs to render the entire scene for each face of the cubemap, as well as perform special blurring in order to get glossy reflections. The impact on frame rate can be significant. Time-slicing helps maintaning a more constant frame rate during these updates by performing the rendering over several frames.</para>
    /// </summary>
    public enum ReflectionProbeTimeSlicingMode
    {
        AllFacesAtOnce,
        IndividualFaces,
        NoTimeSlicing
    }
}

