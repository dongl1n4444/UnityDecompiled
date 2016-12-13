using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.IL2CPP.Portability
{
	public static class CustomAttributeDataPortable
	{
		public static IList<CustomAttributeData> GetCustomAttributesPortable(MemberInfo target)
		{
			return CustomAttributeData.GetCustomAttributes(target);
		}
	}
}
