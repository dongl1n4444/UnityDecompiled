namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Representation of Script assets.</para>
    /// </summary>
    public sealed class MonoScript : TextAsset
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern MonoScript();
        /// <summary>
        /// <para>Returns the MonoScript object containing specified MonoBehaviour.</para>
        /// </summary>
        /// <param name="behaviour">The MonoBehaviour whose MonoScript should be returned.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern MonoScript FromMonoBehaviour(MonoBehaviour behaviour);
        /// <summary>
        /// <para>Returns the MonoScript object containing specified ScriptableObject.</para>
        /// </summary>
        /// <param name="scriptableObject">The ScriptableObject whose MonoScript should be returned.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern MonoScript FromScriptableObject(ScriptableObject scriptableObject);
        /// <summary>
        /// <para>Returns the System.Type object of the class implemented by this script.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern Type GetClass();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern string GetNamespace();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern bool GetScriptTypeWasJustCreatedFromComponentMenu();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void Init(string scriptContents, string className, string nameSpace, string assemblyName, bool isEditorScript);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void SetScriptTypeWasJustCreatedFromComponentMenu();
    }
}

