using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.Metadata
{
	public class Il2CppGenericInstCollectorComponent : IIl2CppGenericInstCollectorWriterService, IIl2CppGenericInstCollectorReaderService, IDisposable
	{
		internal class Il2CppGenericInstComparer : EqualityComparer<TypeReference[]>
		{
			public override bool Equals(TypeReference[] x, TypeReference[] y)
			{
				bool result;
				if (x.Length != y.Length)
				{
					result = false;
				}
				else
				{
					for (int i = 0; i < x.Length; i++)
					{
						if (!TypeReferenceEqualityComparer.AreEqual(x[i], y[i], TypeComparisonMode.Exact))
						{
							result = false;
							return result;
						}
					}
					result = true;
				}
				return result;
			}

			public override int GetHashCode(TypeReference[] obj)
			{
				int num = obj.Length;
				for (int i = 0; i < obj.Length; i++)
				{
					num = num * 486187739 + TypeReferenceEqualityComparer.GetHashCodeFor(obj[i]);
				}
				return num;
			}
		}

		[Inject]
		public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;

		private readonly Dictionary<TypeReference[], uint> _data;

		public IDictionary<TypeReference[], uint> Items
		{
			get
			{
				return this._data;
			}
		}

		public Il2CppGenericInstCollectorComponent()
		{
			this._data = new Dictionary<TypeReference[], uint>(new Il2CppGenericInstCollectorComponent.Il2CppGenericInstComparer());
		}

		public void Add(IList<TypeReference> types)
		{
			TypeReference[] array = types.ToArray<TypeReference>();
			object data = this._data;
			lock (data)
			{
				if (this._data.ContainsKey(array))
				{
					return;
				}
				if (!MetadataCacheWriter.TypesDoNotExceedMaximumRecursion(types))
				{
					return;
				}
				this._data.Add(array, (uint)this._data.Count);
			}
			TypeReference[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				TypeReference type = array2[i];
				Il2CppGenericInstCollectorComponent.Il2CppTypeCollector.Add(type, 0);
			}
		}

		public void Dispose()
		{
			this._data.Clear();
		}
	}
}
