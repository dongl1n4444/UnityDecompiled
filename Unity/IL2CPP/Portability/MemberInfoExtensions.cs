namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class MemberInfoExtensions
    {
        public static int GetMetadataTokenPortable(this MemberInfo info) => 
            info.MetadataToken;
    }
}

