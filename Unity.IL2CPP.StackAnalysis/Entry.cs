using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.Common;

namespace Unity.IL2CPP.StackAnalysis
{
	public class Entry
	{
		private readonly HashSet<TypeReference> _types = new HashSet<TypeReference>(new TypeReferenceEqualityComparer());

		public bool NullValue
		{
			get;
			internal set;
		}

		public HashSet<TypeReference> Types
		{
			get
			{
				return this._types;
			}
		}

		public Entry Clone()
		{
			Entry entry = new Entry
			{
				NullValue = this.NullValue
			};
			foreach (TypeReference current in this._types)
			{
				entry.Types.Add(current);
			}
			return entry;
		}

		public static Entry For(TypeReference typeReference)
		{
			return new Entry
			{
				Types = 
				{
					typeReference
				}
			};
		}

		public static Entry ForNull(TypeReference typeReference)
		{
			Entry entry = new Entry
			{
				NullValue = true
			};
			entry.Types.Add(typeReference);
			return entry;
		}
	}
}
