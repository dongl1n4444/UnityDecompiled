using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.Metadata
{
	public class Il2CppGenericMethodCollectorComponent : IIl2CppGenericMethodCollectorWriterService, IIl2CppGenericMethodCollectorReaderService, IDisposable
	{
		private readonly Dictionary<MethodReference, uint> _data;

		[Inject]
		public static IIl2CppGenericInstCollectorWriterService Il2CppGenericInstCollector;

		public IDictionary<MethodReference, uint> Items
		{
			get
			{
				return this._data;
			}
		}

		public Il2CppGenericMethodCollectorComponent()
		{
			this._data = new Dictionary<MethodReference, uint>(new MethodReferenceComparer());
		}

		public void Add(MethodReference method)
		{
			object data = this._data;
			lock (data)
			{
				if (this._data.ContainsKey(method))
				{
					return;
				}
				if (!MetadataCacheWriter.TypeDoesNotExceedMaximumRecursion(method.DeclaringType))
				{
					return;
				}
				if (method.IsGenericInstance && !MetadataCacheWriter.TypesDoNotExceedMaximumRecursion(((GenericInstanceMethod)method).GenericArguments))
				{
					return;
				}
				this._data.Add(method, (uint)this._data.Count);
			}
			if (method.DeclaringType.IsGenericInstance)
			{
				GenericInstanceType genericInstanceType = (GenericInstanceType)method.DeclaringType;
				Il2CppGenericMethodCollectorComponent.Il2CppGenericInstCollector.Add(genericInstanceType.GenericArguments);
			}
			if (method.IsGenericInstance)
			{
				GenericInstanceMethod genericInstanceMethod = (GenericInstanceMethod)method;
				Il2CppGenericMethodCollectorComponent.Il2CppGenericInstCollector.Add(genericInstanceMethod.GenericArguments);
			}
		}

		public bool HasIndex(MethodReference method)
		{
			return this._data.ContainsKey(method);
		}

		public uint GetIndex(MethodReference method)
		{
			return this._data[method];
		}

		public void Dispose()
		{
			this._data.Clear();
		}
	}
}
