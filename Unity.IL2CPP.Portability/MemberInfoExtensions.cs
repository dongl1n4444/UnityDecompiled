using System;
using System.Reflection;

namespace Unity.IL2CPP.Portability
{
	public static class MemberInfoExtensions
	{
		public static int GetMetadataTokenPortable(this MemberInfo info)
		{
			return info.MetadataToken;
		}
	}
}
