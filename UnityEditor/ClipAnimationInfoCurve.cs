namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>Stores a curve and its name that will be used to create additionnal curves during the import process.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ClipAnimationInfoCurve
    {
        /// <summary>
        /// <para>The name of the animation curve.</para>
        /// </summary>
        public string name;
        /// <summary>
        /// <para>The animation curve.</para>
        /// </summary>
        public AnimationCurve curve;
    }
}

