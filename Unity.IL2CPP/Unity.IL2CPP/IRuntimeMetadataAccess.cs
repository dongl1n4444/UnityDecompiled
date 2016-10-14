using Mono.Cecil;
using System;

namespace Unity.IL2CPP
{
	public interface IRuntimeMetadataAccess
	{
		string StaticData(TypeReference type);

		string TypeInfoFor(TypeReference type);

		string SizeOf(TypeReference type);

		string ArrayInfo(TypeReference elementType);

		string Newobj(MethodReference ctor);

		string Il2CppTypeFor(TypeReference type);

		string Method(MethodReference genericMethod);

		string MethodInfo(MethodReference method);

		string HiddenMethodInfo(MethodReference method);

		string FieldInfo(FieldReference field);

		string StringLiteral(string literal);

		bool NeedsBoxingForValueTypeThis(MethodReference method);
	}
}
