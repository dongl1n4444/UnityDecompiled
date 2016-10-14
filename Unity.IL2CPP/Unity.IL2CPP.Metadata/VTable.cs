using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Unity.IL2CPP.Metadata
{
	public class VTable
	{
		private readonly ReadOnlyCollection<MethodReference> _slots;

		private readonly Dictionary<TypeReference, int> _interfaceOffsets;

		public ReadOnlyCollection<MethodReference> Slots
		{
			get
			{
				return this._slots;
			}
		}

		public Dictionary<TypeReference, int> InterfaceOffsets
		{
			get
			{
				return this._interfaceOffsets;
			}
		}

		public VTable(ReadOnlyCollection<MethodReference> slots, Dictionary<TypeReference, int> interfaceOffsets)
		{
			this._slots = slots;
			this._interfaceOffsets = interfaceOffsets;
		}
	}
}
