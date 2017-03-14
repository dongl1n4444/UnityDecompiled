namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    internal sealed class AnimationModeDriver : ScriptableObject
    {
        internal IsKeyCallback isKeyCallback;

        [UsedByNativeCode]
        internal bool InvokeIsKeyCallback_Internal(UnityEngine.Object target, string propertyPath) => 
            this.isKeyCallback?.Invoke(target, propertyPath);

        internal delegate bool IsKeyCallback(UnityEngine.Object target, string propertyPath);
    }
}

