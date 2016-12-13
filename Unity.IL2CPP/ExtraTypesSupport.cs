using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cecil.Visitor;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.GenericsCollection;

namespace Unity.IL2CPP
{
	public struct ExtraTypesSupport
	{
		private readonly GenericContextFreeVisitor _visitor;

		private readonly IEnumerable<AssemblyDefinition> _usedAssemblies;

		public ExtraTypesSupport(InflatedCollectionCollector collectionCollector, IEnumerable<AssemblyDefinition> usedAssemblies)
		{
			this._usedAssemblies = usedAssemblies;
			this._visitor = new GenericContextFreeVisitor(collectionCollector);
		}

		public bool AddType(string typeName, TypeNameParseInfo typeNameInfo)
		{
			bool result;
			try
			{
				this.TypeReferenceFor(typeNameInfo);
				result = true;
			}
			catch (TypeResolutionException)
			{
				result = false;
			}
			return result;
		}

		private TypeReference TypeReferenceFor(TypeNameParseInfo typeNameInfo)
		{
			TypeReference typeReference = this.GetTypeByName(ExtraTypesSupport.CecilElementTypeNameFor(typeNameInfo), typeNameInfo.Assembly);
			if (typeReference == null)
			{
				throw new TypeResolutionException(typeNameInfo);
			}
			if (typeNameInfo.HasGenericArguments)
			{
				GenericInstanceType genericInstanceType = new GenericInstanceType(typeReference);
				foreach (TypeNameParseInfo current in typeNameInfo.TypeArguments)
				{
					genericInstanceType.GenericArguments.Add(this.TypeReferenceFor(current));
				}
				genericInstanceType.Accept(this._visitor);
				typeReference = genericInstanceType;
			}
			if (typeNameInfo.IsPointer)
			{
				throw new TypeResolutionException(typeNameInfo);
			}
			if (typeNameInfo.IsArray)
			{
				ArrayType arrayType = new ArrayType(typeReference, typeNameInfo.Ranks[0]);
				for (int i = 1; i < typeNameInfo.Ranks.Length; i++)
				{
					arrayType = new ArrayType(arrayType, typeNameInfo.Ranks[i]);
				}
				arrayType.Accept(this._visitor);
				typeReference = arrayType;
			}
			return typeReference;
		}

		private static string CecilElementTypeNameFor(TypeNameParseInfo typeNameInfo)
		{
			string result;
			if (!typeNameInfo.IsNested)
			{
				result = typeNameInfo.ElementTypeName;
			}
			else
			{
				string text = typeNameInfo.Name;
				if (!string.IsNullOrEmpty(typeNameInfo.Namespace))
				{
					text = typeNameInfo.Namespace + "." + text;
				}
				result = typeNameInfo.Nested.Aggregate(text, (string c, string n) => c + "/" + n);
			}
			return result;
		}

		private TypeReference GetTypeByName(string name, AssemblyNameParseInfo assembly)
		{
			TypeReference result;
			if (assembly == null || string.IsNullOrEmpty(assembly.Name))
			{
				result = (from a in this._usedAssemblies
				select a.MainModule.GetType(name)).FirstOrDefault((TypeDefinition t) => t != null);
			}
			else
			{
				AssemblyDefinition assemblyDefinition = this._usedAssemblies.FirstOrDefault((AssemblyDefinition a) => a.Name.Name == assembly.Name);
				if (assemblyDefinition == null)
				{
					result = null;
				}
				else
				{
					result = assemblyDefinition.MainModule.GetType(name);
				}
			}
			return result;
		}
	}
}
