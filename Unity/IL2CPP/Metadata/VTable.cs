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

        public Dictionary<TypeReference, int> InterfaceOffsets =>
            this._interfaceOffsets;

        public ReadOnlyCollection<MethodReference> Slots =>
            this._slots;
    }
}

