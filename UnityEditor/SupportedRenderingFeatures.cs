namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>Describes the rendering features supported by a given renderloop.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SupportedRenderingFeatures
    {
        /// <summary>
        /// <para>Supported reflection probe rendering features.</para>
        /// </summary>
        public ReflectionProbe reflectionProbe;
        private static SupportedRenderingFeatures s_Active;
        /// <summary>
        /// <para>The rendering features supported by the active renderloop.</para>
        /// </summary>
        public static SupportedRenderingFeatures active
        {
            get => 
                s_Active;
            set
            {
                s_Active = value;
            }
        }
        /// <summary>
        /// <para>Default rendering features (Read Only).</para>
        /// </summary>
        public static SupportedRenderingFeatures Default =>
            new SupportedRenderingFeatures();
        static SupportedRenderingFeatures()
        {
            s_Active = new SupportedRenderingFeatures();
        }
        /// <summary>
        /// <para>Reflection probe features.</para>
        /// </summary>
        [Flags]
        public enum ReflectionProbe
        {
            None,
            Rotation
        }
    }
}

