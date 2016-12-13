using System;
using System.Reflection.Emit;

namespace Unity.IL2CPP.Portability
{
	public static class TypeBuilderExtensions
	{
		public static Type CreateTypePortable(this TypeBuilder typeBuilder)
		{
			return typeBuilder.CreateType();
		}
	}
}
