using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public class Il2CppTypeCollectorComponent : IIl2CppTypeCollectorReaderService, IIl2CppTypeCollectorWriterService, IDisposable
	{
		private readonly Dictionary<Il2CppTypeData, int> _data;

		[Inject]
		public static IIl2CppGenericInstCollectorWriterService Il2CppGenericInstCollector;

		[Inject]
		public static INamingService Naming;

		public IDictionary<Il2CppTypeData, int> Items
		{
			get
			{
				return this._data;
			}
		}

		public Il2CppTypeCollectorComponent()
		{
			this._data = new Dictionary<Il2CppTypeData, int>(new Il2CppTypeDataComparer());
		}

		public void Add(TypeReference type, int attrs = 0)
		{
			type = Il2CppTypeCollectorComponent.Naming.RemoveModifiers(type);
			Il2CppTypeData key = new Il2CppTypeData(type, attrs);
			object data = this._data;
			lock (data)
			{
				if (this._data.ContainsKey(key))
				{
					return;
				}
				this._data.Add(key, this._data.Count);
			}
			GenericInstanceType genericInstanceType = type as GenericInstanceType;
			if (genericInstanceType != null)
			{
				Il2CppTypeCollectorComponent.Il2CppGenericInstCollector.Add(genericInstanceType.GenericArguments);
			}
			ArrayType arrayType = type as ArrayType;
			if (arrayType != null)
			{
				this.Add(arrayType.ElementType, 0);
			}
			ByReferenceType byReferenceType = type as ByReferenceType;
			if (byReferenceType != null)
			{
				this.Add(byReferenceType.ElementType, 0);
			}
		}

		public int GetOrCreateIndex(TypeReference type, int attrs = 0)
		{
			Il2CppTypeData key = new Il2CppTypeData(Il2CppTypeCollectorComponent.Naming.RemoveModifiers(type), attrs);
			int num;
			int result;
			if (this._data.TryGetValue(key, out num))
			{
				result = num;
			}
			else
			{
				this.Add(type, attrs);
				result = this._data[key];
			}
			return result;
		}

		public int GetIndex(TypeReference type, int attrs = 0)
		{
			Il2CppTypeData key = new Il2CppTypeData(Il2CppTypeCollectorComponent.Naming.RemoveModifiers(type), attrs);
			int result;
			if (this._data.TryGetValue(key, out result))
			{
				return result;
			}
			throw new InvalidOperationException(string.Format("Il2CppTypeIndexFor type {0} does not exist.", type.FullName));
		}

		public void Dispose()
		{
			this._data.Clear();
		}
	}
}
