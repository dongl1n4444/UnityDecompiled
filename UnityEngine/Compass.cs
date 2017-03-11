namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Interface into compass functionality.</para>
    /// </summary>
    public sealed class Compass
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_rawVector(out Vector3 value);

        /// <summary>
        /// <para>Used to enable or disable compass. Note, that if you want Input.compass.trueHeading property to contain a valid value, you must also enable location updates by calling Input.location.Start().</para>
        /// </summary>
        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Accuracy of heading reading in degrees.</para>
        /// </summary>
        public float headingAccuracy { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The heading in degrees relative to the magnetic North Pole. (Read Only)</para>
        /// </summary>
        public float magneticHeading { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The raw geomagnetic data measured in microteslas. (Read Only)</para>
        /// </summary>
        public Vector3 rawVector
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_rawVector(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>Timestamp (in seconds since 1970) when the heading was last time updated. (Read Only)</para>
        /// </summary>
        public double timestamp { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>The heading in degrees relative to the geographic North Pole. (Read Only)</para>
        /// </summary>
        public float trueHeading { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

