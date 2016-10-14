using System;

namespace Unity.IL2CPP.Metadata.Fields
{
	public class FieldMarshaledSize
	{
		public readonly int _fieldIndex;

		public readonly int _typeIndex;

		public readonly int _size;

		public FieldMarshaledSize(int fieldIndex, int typeIndex, int size)
		{
			this._fieldIndex = fieldIndex;
			this._typeIndex = typeIndex;
			this._size = size;
		}

		public override string ToString()
		{
			return string.Format("{{ {0}, {1}, {2} }}", this._fieldIndex, this._typeIndex, this._size);
		}
	}
}
