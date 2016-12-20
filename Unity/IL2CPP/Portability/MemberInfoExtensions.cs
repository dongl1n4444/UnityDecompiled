namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class MemberInfoExtensions
    {
        [Extension]
        public static int GetMetadataTokenPortable(MemberInfo info)
        {
            return info.MetadataToken;
        }
    }
}

