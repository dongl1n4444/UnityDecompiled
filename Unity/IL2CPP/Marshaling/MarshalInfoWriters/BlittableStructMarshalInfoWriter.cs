namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.Marshaling;

    public class BlittableStructMarshalInfoWriter : DefaultMarshalInfoWriter
    {
        private readonly MarshaledType[] _marshaledTypes;
        private readonly MarshalType _marshalType;
        private readonly TypeDefinition _type;
        [CompilerGenerated]
        private static Func<DefaultMarshalInfoWriter, int> <>f__am$cache0;

        public BlittableStructMarshalInfoWriter(TypeDefinition type, MarshalType marshalType) : base(type)
        {
            this._type = type;
            this._marshalType = marshalType;
            string name = DefaultMarshalInfoWriter.Naming.ForVariable(this._type);
            this._marshaledTypes = new MarshaledType[] { new MarshaledType(name, name) };
        }

        public override MarshaledType[] MarshaledTypes =>
            this._marshaledTypes;

        public override int NativeSizeWithoutPointers
        {
            get
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = f => f.NativeSizeWithoutPointers;
                }
                return MarshalingUtils.GetFieldMarshalInfoWriters(this._type, this._marshalType).Select<DefaultMarshalInfoWriter, int>(<>f__am$cache0).Sum();
            }
        }
    }
}

