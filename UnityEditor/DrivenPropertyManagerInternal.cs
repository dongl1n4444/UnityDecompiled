namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    internal sealed class DrivenPropertyManagerInternal
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool IsDriven(UnityEngine.Object target, string propertyPath);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool IsDriving(UnityEngine.Object driver, UnityEngine.Object target, string propertyPath);
    }
}

