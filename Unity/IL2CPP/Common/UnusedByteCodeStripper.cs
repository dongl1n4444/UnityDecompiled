namespace Unity.IL2CPP.Common
{
    using NiceIO;
    using System;

    public class UnusedByteCodeStripper
    {
        public static bool Available
        {
            get
            {
                return (Il2CppDependencies.HasUnusedByteCodeStripper || UnitySourceCode.Available);
            }
        }

        public static NPath Path
        {
            get
            {
                if (Il2CppDependencies.HasUnusedByteCodeStripper)
                {
                    return Il2CppDependencies.UnusedByteCodeStripper;
                }
                if (!UnitySourceCode.Available)
                {
                    throw new InvalidOperationException("Could not locate UnusedByteCodeStripper");
                }
                return UnitySourceCode.Paths.UnusedBytecodeStripper;
            }
        }
    }
}

