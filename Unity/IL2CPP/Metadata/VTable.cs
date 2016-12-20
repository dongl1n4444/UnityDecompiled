namespace Unity.IL2CPP.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class VTable
    {
        private readonly Dictionary<TypeReference, int> _interfaceOffsets;
        private readonly ReadOnlyCollection<MethodReference> _slots;

        public VTable(ReadOnlyCollection<MethodReference> slots, Dictionary<TypeReference, int> interfaceOffsets)
        {
            this._slots = slots;
            this._interfaceOffsets = interfaceOffsets;
        }

        public Dictionary<TypeReference, int> InterfaceOffsets
        {
            get
            {
                return this._interfaceOffsets;
            }
        }

        public ReadOnlyCollection<MethodReference> Slots
        {
            get
            {
                return this._slots;
            }
        }
    }
}

