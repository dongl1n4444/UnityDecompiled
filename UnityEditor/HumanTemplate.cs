namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class HumanTemplate : Object
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern HumanTemplate();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void ClearTemplate();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string Find(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Insert(string name, string templateName);
    }
}

