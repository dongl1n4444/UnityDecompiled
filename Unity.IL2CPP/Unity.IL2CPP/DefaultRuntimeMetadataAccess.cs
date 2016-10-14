using Mono.Cecil;
using System;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP
{
	public sealed class DefaultRuntimeMetadataAccess : IRuntimeMetadataAccess
	{
		[Inject]
		public static IIl2CppGenericMethodCollectorWriterService Il2CppGenericMethodCollector;

		[Inject]
		public static INamingService Naming;

		private readonly MetadataUsage _metadataUsage;

		private readonly MethodUsage _methodUsage;

		private readonly TypeResolver _typeResolver;

		public DefaultRuntimeMetadataAccess(MethodReference methodReference, MetadataUsage metadataUsage, MethodUsage methodUsage)
		{
			this._metadataUsage = metadataUsage;
			this._methodUsage = methodUsage;
			if (methodReference != null)
			{
				this._typeResolver = new TypeResolver(methodReference.DeclaringType as GenericInstanceType, methodReference as GenericInstanceMethod);
			}
			else
			{
				this._typeResolver = new TypeResolver();
			}
		}

		public string StaticData(TypeReference type)
		{
			TypeReference type2 = this._typeResolver.Resolve(type);
			this._metadataUsage.AddTypeInfo(type2);
			return DefaultRuntimeMetadataAccess.Naming.ForRuntimeTypeInfo(type2);
		}

		public string TypeInfoFor(TypeReference type)
		{
			TypeReference type2 = this._typeResolver.Resolve(type);
			this._metadataUsage.AddTypeInfo(type2);
			return DefaultRuntimeMetadataAccess.Naming.ForRuntimeTypeInfo(type2);
		}

		public string SizeOf(TypeReference type)
		{
			TypeReference variableType = this._typeResolver.Resolve(type);
			return string.Format("sizeof({0})", DefaultRuntimeMetadataAccess.Naming.ForVariable(variableType));
		}

		public string Il2CppTypeFor(TypeReference type)
		{
			TypeReference type2 = this._typeResolver.Resolve(type, false);
			this._metadataUsage.AddIl2CppType(type2);
			MetadataWriter.TypeRepositoryTypeFor(type2, 0);
			return DefaultRuntimeMetadataAccess.Naming.ForRuntimeIl2CppType(type2);
		}

		public string ArrayInfo(TypeReference elementType)
		{
			ArrayType type = new ArrayType(this._typeResolver.Resolve(elementType));
			this._metadataUsage.AddTypeInfo(type);
			return DefaultRuntimeMetadataAccess.Naming.ForRuntimeTypeInfo(type);
		}

		public string MethodInfo(MethodReference method)
		{
			MethodReference method2 = this._typeResolver.Resolve(method);
			if (method.IsGenericInstance || method.DeclaringType.IsGenericInstance)
			{
				DefaultRuntimeMetadataAccess.Il2CppGenericMethodCollector.Add(method2);
			}
			this._metadataUsage.AddInflatedMethod(method2);
			return DefaultRuntimeMetadataAccess.Naming.ForRuntimeMethodInfo(method2);
		}

		public string HiddenMethodInfo(MethodReference method)
		{
			MethodReference method2 = this._typeResolver.Resolve(method);
			string result;
			if (method.IsGenericInstance || method.DeclaringType.IsGenericInstance)
			{
				DefaultRuntimeMetadataAccess.Il2CppGenericMethodCollector.Add(method2);
				this._metadataUsage.AddInflatedMethod(method2);
				result = DefaultRuntimeMetadataAccess.Naming.ForRuntimeMethodInfo(method2);
			}
			else
			{
				result = DefaultRuntimeMetadataAccess.Naming.Null;
			}
			return result;
		}

		public string Newobj(MethodReference ctor)
		{
			TypeReference type = this._typeResolver.Resolve(ctor.DeclaringType);
			this._metadataUsage.AddTypeInfo(type);
			return DefaultRuntimeMetadataAccess.Naming.ForRuntimeTypeInfo(type);
		}

		public string Method(MethodReference genericMethod)
		{
			this._methodUsage.AddMethod(genericMethod);
			return DefaultRuntimeMetadataAccess.Naming.ForMethod(this._typeResolver.Resolve(genericMethod));
		}

		public string FieldInfo(FieldReference field)
		{
			FieldReference field2 = this._typeResolver.Resolve(field);
			this._metadataUsage.AddFieldInfo(field2);
			return DefaultRuntimeMetadataAccess.Naming.ForRuntimeFieldInfo(field2);
		}

		public string StringLiteral(string literal)
		{
			string result;
			if (literal == null)
			{
				result = DefaultRuntimeMetadataAccess.Naming.Null;
			}
			else
			{
				this._metadataUsage.AddStringLiteral(literal);
				result = DefaultRuntimeMetadataAccess.Naming.ForStringLiteralIdentifier(literal);
			}
			return result;
		}

		public bool NeedsBoxingForValueTypeThis(MethodReference method)
		{
			return false;
		}
	}
}
