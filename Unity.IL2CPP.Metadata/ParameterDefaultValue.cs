using System;

namespace Unity.IL2CPP.Metadata
{
	public class ParameterDefaultValue
	{
		public readonly int _parameterIndex;

		public readonly int _typeIndex;

		public readonly int _dataIndex;

		public ParameterDefaultValue(int parameterIndex, int typeIndex, int dataIndex)
		{
			this._parameterIndex = parameterIndex;
			this._typeIndex = typeIndex;
			this._dataIndex = dataIndex;
		}

		public override string ToString()
		{
			return string.Format("{{ {0}, {1}, {2} }}", this._parameterIndex, this._typeIndex, this._dataIndex);
		}
	}
}
