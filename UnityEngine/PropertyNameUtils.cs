namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.Bindings;

    internal class PropertyNameUtils
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int ConflictCountForID(int id);
        public static PropertyName PropertyNameFromString([NativeParameter(Unmarshalled=true)] string name)
        {
            PropertyName name2;
            PropertyNameFromString_Injected(name, out name2);
            return name2;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void PropertyNameFromString_Injected(string name, out PropertyName ret);
        public static string StringFromPropertyName(PropertyName propertyName) => 
            StringFromPropertyName_Injected(ref propertyName);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string StringFromPropertyName_Injected(ref PropertyName propertyName);
    }
}

