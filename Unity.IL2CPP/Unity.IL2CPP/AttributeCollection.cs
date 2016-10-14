using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public class AttributeCollection
	{
		public struct AttributeTypeRange
		{
			public readonly int Start;

			public readonly int Count;

			public AttributeTypeRange(int start, int count)
			{
				this.Start = start;
				this.Count = count;
			}
		}

		private readonly Dictionary<string, uint> _indices = new Dictionary<string, uint>();

		private readonly List<AttributeCollection.AttributeTypeRange> _attributeTypeRanges = new List<AttributeCollection.AttributeTypeRange>();

		private readonly List<int> _attributeTypeIndices = new List<int>();

		[Inject]
		public static INamingService Naming;

		[Inject]
		public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;

		public int Count
		{
			get
			{
				return this._indices.Count;
			}
		}

		public AttributeCollection()
		{
			this._indices.Add(AttributeCollection.Naming.Null, 0u);
			this._attributeTypeRanges.Add(new AttributeCollection.AttributeTypeRange(0, 0));
		}

		public string[] GetEntries()
		{
			return (from v in this._indices
			orderby v.Value
			select v.Key).ToArray<string>();
		}

		public uint GetIndex(AssemblyDefinition assembly)
		{
			return this.GetIndex(AttributeCollection.Naming.ForCustomAttributesCacheGenerator(assembly));
		}

		public uint GetIndex(TypeDefinition type)
		{
			return this.GetIndex(AttributeCollection.Naming.ForCustomAttributesCacheGenerator(type));
		}

		public uint GetIndex(FieldDefinition fieldDefinition)
		{
			return this.GetIndex(AttributeCollection.Naming.ForCustomAttributesCacheGenerator(fieldDefinition));
		}

		public uint GetIndex(MethodDefinition methodDefinition)
		{
			return this.GetIndex(AttributeCollection.Naming.ForCustomAttributesCacheGenerator(methodDefinition));
		}

		public uint GetIndex(PropertyDefinition propertyDefinition)
		{
			return this.GetIndex(AttributeCollection.Naming.ForCustomAttributesCacheGenerator(propertyDefinition));
		}

		public uint GetIndex(EventDefinition eventDefinition)
		{
			return this.GetIndex(AttributeCollection.Naming.ForCustomAttributesCacheGenerator(eventDefinition));
		}

		public uint GetIndex(ParameterDefinition parameterDefinition, MethodDefinition methodDefinition)
		{
			return this.GetIndex(AttributeCollection.Naming.ForCustomAttributesCacheGenerator(parameterDefinition, methodDefinition));
		}

		private uint GetIndex(string name)
		{
			uint result;
			this._indices.TryGetValue(name, out result);
			return result;
		}

		public IEnumerable<AttributeCollection.AttributeTypeRange> GetAttributeTypeRanges()
		{
			return this._attributeTypeRanges;
		}

		public IEnumerable<int> GetAttributeTypeIndices()
		{
			return this._attributeTypeIndices;
		}

		public void Add(string name, CustomAttribute[] customAttributes)
		{
			this._indices.Add(name, (uint)this._indices.Count);
			this._attributeTypeRanges.Add(new AttributeCollection.AttributeTypeRange(this._attributeTypeIndices.Count, customAttributes.Length));
			this._attributeTypeIndices.AddRange(from ca in customAttributes
			select AttributeCollection.Il2CppTypeCollector.GetOrCreateIndex(ca.AttributeType, 0));
		}
	}
}
