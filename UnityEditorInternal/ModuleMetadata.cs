namespace UnityEditorInternal
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine.Scripting;

    internal sealed class ModuleMetadata
    {
        [CompilerGenerated]
        private static Func<int, UnityType> <>f__am$cache0;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string GetICallModule(string icall);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int[] GetModuleClasses(string moduleName);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string[] GetModuleNames();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetModuleStrippable(string moduleName);
        public static UnityType[] GetModuleTypes(string moduleName)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = id => UnityType.FindTypeByPersistentTypeID(id);
            }
            return Enumerable.Select<int, UnityType>(GetModuleClasses(moduleName), <>f__am$cache0).ToArray<UnityType>();
        }
    }
}

