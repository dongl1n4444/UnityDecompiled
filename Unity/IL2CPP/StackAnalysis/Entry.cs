namespace Unity.IL2CPP.StackAnalysis
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class Entry
    {
        private readonly HashSet<TypeReference> _types = new HashSet<TypeReference>(new TypeReferenceEqualityComparer());
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <NullValue>k__BackingField;

        public Entry Clone()
        {
            Entry entry = new Entry {
                NullValue = this.NullValue
            };
            foreach (TypeReference reference in this._types)
            {
                entry.Types.Add(reference);
            }
            return entry;
        }

        public static Entry For(TypeReference typeReference)
        {
            return new Entry { Types = { typeReference } };
        }

        public static Entry ForNull(TypeReference typeReference)
        {
            Entry entry = new Entry {
                NullValue = true
            };
            entry.Types.Add(typeReference);
            return entry;
        }

        public bool NullValue { get; internal set; }

        public HashSet<TypeReference> Types
        {
            get
            {
                return this._types;
            }
        }
    }
}

