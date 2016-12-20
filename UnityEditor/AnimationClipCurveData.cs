namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>An AnimationClipCurveData object contains all the information needed to identify a specific curve in an AnimationClip. The curve animates a specific property of a component  material attached to a game object  animated bone.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class AnimationClipCurveData
    {
        /// <summary>
        /// <para>The path of the game object / bone being animated.</para>
        /// </summary>
        public string path;
        /// <summary>
        /// <para>The type of the component / material being animated.</para>
        /// </summary>
        public Type type;
        /// <summary>
        /// <para>The name of the property being animated.</para>
        /// </summary>
        public string propertyName;
        /// <summary>
        /// <para>The actual animation curve.</para>
        /// </summary>
        public AnimationCurve curve;
        internal int classID;
        internal int scriptInstanceID;
        public AnimationClipCurveData()
        {
        }

        public AnimationClipCurveData(EditorCurveBinding binding)
        {
            this.path = binding.path;
            this.type = binding.type;
            this.propertyName = binding.propertyName;
            this.curve = null;
            this.classID = binding.m_ClassID;
            this.scriptInstanceID = binding.m_ScriptInstanceID;
        }
    }
}

