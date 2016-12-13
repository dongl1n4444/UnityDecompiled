using Mono.Cecil;
using System;
using System.Linq;

namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
	public class BlittableStructMarshalInfoWriter : DefaultMarshalInfoWriter
	{
		private readonly TypeDefinition _type;

		private readonly MarshalType _marshalType;

		private readonly MarshaledType[] _marshaledTypes;

		public override MarshaledType[] MarshaledTypes
		{
			get
			{
				return this._marshaledTypes;
			}
		}

		public override int NativeSizeWithoutPointers
		{
			get
			{
				return (from f in MarshalingUtils.GetFieldMarshalInfoWriters(this._type, this._marshalType)
				select f.NativeSizeWithoutPointers).Sum();
			}
		}

		public BlittableStructMarshalInfoWriter(TypeDefinition type, MarshalType marshalType) : base(type)
		{
			this._type = type;
			this._marshalType = marshalType;
			string text = DefaultMarshalInfoWriter.Naming.ForVariable(this._type);
			this._marshaledTypes = new MarshaledType[]
			{
				new MarshaledType(text, text)
			};
		}
	}
}
