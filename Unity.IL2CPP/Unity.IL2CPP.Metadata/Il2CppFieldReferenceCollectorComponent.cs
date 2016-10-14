using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.Metadata
{
	public class Il2CppFieldReferenceCollectorComponent : IIl2CppFieldReferenceCollectorWriterService, IIl2CppFieldReferenceCollectorReaderService, IDisposable
	{
		[Inject]
		public static IIl2CppTypeCollectorWriterService Il2CppTypeCollector;

		private readonly Dictionary<FieldReference, uint> _fields = new Dictionary<FieldReference, uint>();

		public ReadOnlyDictionary<FieldReference, uint> Fields
		{
			get
			{
				return this._fields.AsReadOnly<FieldReference, uint>();
			}
		}

		public uint GetOrCreateIndex(FieldReference field)
		{
			uint count;
			uint result;
			if (this._fields.TryGetValue(field, out count))
			{
				result = count;
			}
			else
			{
				count = (uint)this._fields.Count;
				this._fields.Add(field, count);
				Il2CppFieldReferenceCollectorComponent.Il2CppTypeCollector.Add(field.DeclaringType, 0);
				result = count;
			}
			return result;
		}

		public void Dispose()
		{
			this._fields.Clear();
		}
	}
}
