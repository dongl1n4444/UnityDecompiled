namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Collections;

    internal static class UnsafeUtility
    {
        public static IntPtr AddressOf<T>(ref T output) where T: struct => 
            output;

        public static int AlignOf<T>() where T: struct => 
            4;

        public static void CopyPtrToStructure<T>(IntPtr ptr, out T output) where T: struct
        {
            output = (T) ptr[0];
        }

        public static void CopyStructureToPtr<T>(ref T output, IntPtr ptr) where T: struct
        {
            ptr[0] = output;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Free(IntPtr memory, Allocator label);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void LogError(string msg, string filename, int linenumber);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern IntPtr Malloc(int size, int alignment, Allocator label);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void MemCpy(IntPtr destination, IntPtr source, int size);
        public static unsafe T ReadArrayElement<T>(IntPtr source, int index) => 
            *(((T*) (source + index)));

        public static int SizeOf<T>() where T: struct => 
            SizeOfStruct(typeof(T));

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int SizeOfStruct(System.Type type);
        public static void WriteArrayElement<T>(IntPtr destination, int index, T value)
        {
            destination[index] = value;
        }
    }
}

