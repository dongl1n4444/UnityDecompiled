namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Applies "platform" behaviour such as one-way collisions etc.</para>
    /// </summary>
    public sealed class PlatformEffector2D : Effector2D
    {
        /// <summary>
        /// <para>Whether to use one-way collision behaviour or not.</para>
        /// </summary>
        [Obsolete("PlatformEffector2D.oneWay has been deprecated. Use PlatformEffector2D.useOneWay instead (UnityUpgradable) -> useOneWay", true)]
        public bool oneWay
        {
            get => 
                this.useOneWay;
            set
            {
                this.useOneWay = value;
            }
        }

        /// <summary>
        /// <para>The rotational offset angle from the local 'up'.</para>
        /// </summary>
        public float rotationalOffset { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The angle variance centered on the sides of the platform.  Zero angle only matches sides 90-degree to the platform "top".</para>
        /// </summary>
        [Obsolete("PlatformEffector2D.sideAngleVariance has been deprecated. Use PlatformEffector2D.sideArc instead (UnityUpgradable) -> sideArc", true)]
        public float sideAngleVariance
        {
            get => 
                this.sideArc;
            set
            {
                this.sideArc = value;
            }
        }

        /// <summary>
        /// <para>The angle of an arc that defines the sides of the platform centered on the local 'left' and 'right' of the effector. Any collision normals within this arc are considered for the 'side' behaviours.</para>
        /// </summary>
        public float sideArc { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Whether bounce should be used on the platform sides or not.</para>
        /// </summary>
        [Obsolete("PlatformEffector2D.sideBounce has been deprecated. Use PlatformEffector2D.useSideBounce instead (UnityUpgradable) -> useSideBounce", true)]
        public bool sideBounce
        {
            get => 
                this.useSideBounce;
            set
            {
                this.useSideBounce = value;
            }
        }

        /// <summary>
        /// <para>Whether friction should be used on the platform sides or not.</para>
        /// </summary>
        [Obsolete("PlatformEffector2D.sideFriction has been deprecated. Use PlatformEffector2D.useSideFriction instead (UnityUpgradable) -> useSideFriction", true)]
        public bool sideFriction
        {
            get => 
                this.useSideFriction;
            set
            {
                this.useSideFriction = value;
            }
        }

        /// <summary>
        /// <para>The angle of an arc that defines the surface of the platform centered of the local 'up' of the effector.</para>
        /// </summary>
        public float surfaceArc { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should the one-way collision behaviour be used?</para>
        /// </summary>
        public bool useOneWay { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Ensures that all contacts controlled by the one-way behaviour act the same.</para>
        /// </summary>
        public bool useOneWayGrouping { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should bounce be used on the platform sides?</para>
        /// </summary>
        public bool useSideBounce { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should friction be used on the platform sides?</para>
        /// </summary>
        public bool useSideFriction { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

