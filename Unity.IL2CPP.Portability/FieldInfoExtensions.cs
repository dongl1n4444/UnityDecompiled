using System;
using System.Reflection;

namespace Unity.IL2CPP.Portability
{
	public static class FieldInfoExtensions
	{
		public static RuntimeFieldHandle GetFieldHandlePortable(this FieldInfo fieldInfo)
		{
			return fieldInfo.FieldHandle;
		}
	}
}
