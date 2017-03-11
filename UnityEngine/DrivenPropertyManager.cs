namespace UnityEngine
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    internal sealed class DrivenPropertyManager
    {
        [Conditional("UNITY_EDITOR")]
        public static void RegisterProperty(UnityEngine.Object driver, UnityEngine.Object target, string propertyPath)
        {
            if (driver == null)
            {
                throw new ArgumentNullException("driver");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (propertyPath == null)
            {
                throw new ArgumentNullException("propertyPath");
            }
            RegisterPropertyInternal(driver, target, propertyPath);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void RegisterPropertyInternal(UnityEngine.Object driver, UnityEngine.Object target, string propertyPath);
        [Conditional("UNITY_EDITOR")]
        public static void UnregisterProperties(UnityEngine.Object driver)
        {
            if (driver == null)
            {
                throw new ArgumentNullException("driver");
            }
            UnregisterPropertiesInternal(driver);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void UnregisterPropertiesInternal(UnityEngine.Object driver);
        [Conditional("UNITY_EDITOR")]
        public static void UnregisterProperty(UnityEngine.Object driver, UnityEngine.Object target, string propertyPath)
        {
            if (driver == null)
            {
                throw new ArgumentNullException("driver");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (propertyPath == null)
            {
                throw new ArgumentNullException("propertyPath");
            }
            UnregisterPropertyInternal(driver, target, propertyPath);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void UnregisterPropertyInternal(UnityEngine.Object driver, UnityEngine.Object target, string propertyPath);
    }
}

