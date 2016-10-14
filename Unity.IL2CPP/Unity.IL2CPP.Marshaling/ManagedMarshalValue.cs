using Mono.Cecil;
using System;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.Marshaling
{
	public struct ManagedMarshalValue
	{
		[Inject]
		public static INamingService Naming;

		private readonly string _objectVariableName;

		private readonly FieldReference _field;

		private readonly string _indexVariableName;

		public ManagedMarshalValue Dereferenced
		{
			get
			{
				return new ManagedMarshalValue("*" + this._objectVariableName, this._field, this._indexVariableName);
			}
		}

		public ManagedMarshalValue(string objectVariableName)
		{
			this._objectVariableName = objectVariableName;
			this._field = null;
			this._indexVariableName = null;
		}

		public ManagedMarshalValue(string objectVariableName, FieldReference field)
		{
			this._objectVariableName = objectVariableName;
			this._field = field;
			this._indexVariableName = null;
		}

		public ManagedMarshalValue(ManagedMarshalValue arrayValue, string indexVariableName)
		{
			this._objectVariableName = arrayValue._objectVariableName;
			this._field = arrayValue._field;
			this._indexVariableName = indexVariableName;
		}

		public ManagedMarshalValue(string objectVariableName, FieldReference field, string indexVariableName)
		{
			this._objectVariableName = objectVariableName;
			this._field = field;
			this._indexVariableName = indexVariableName;
		}

		public string Load()
		{
			string result;
			if (this._indexVariableName != null)
			{
				string array;
				if (this._field != null)
				{
					array = string.Format("{0}.{1}()", this._objectVariableName, ManagedMarshalValue.Naming.ForFieldGetter(this._field));
				}
				else
				{
					array = this._objectVariableName;
				}
				result = Emit.LoadArrayElement(array, this._indexVariableName);
			}
			else if (this._field != null)
			{
				result = string.Format("{0}.{1}()", this._objectVariableName, ManagedMarshalValue.Naming.ForFieldGetter(this._field));
			}
			else
			{
				result = this._objectVariableName;
			}
			return result;
		}

		public string LoadAddress()
		{
			if (this._indexVariableName != null)
			{
				throw new NotSupportedException();
			}
			string result;
			if (this._field != null)
			{
				result = string.Format("{0}.{1}()", this._objectVariableName, ManagedMarshalValue.Naming.ForFieldAddressGetter(this._field));
			}
			else
			{
				result = ManagedMarshalValue.Naming.AddressOf(this._objectVariableName);
			}
			return result;
		}

		public string Store(string value)
		{
			string result;
			if (this._indexVariableName != null)
			{
				string array;
				if (this._field != null)
				{
					array = string.Format("{0}.{1}()", this._objectVariableName, ManagedMarshalValue.Naming.ForFieldGetter(this._field));
				}
				else
				{
					array = this._objectVariableName;
				}
				result = string.Format("{0};", Emit.StoreArrayElement(array, this._indexVariableName, value));
			}
			else if (this._field != null)
			{
				result = string.Format("{0}.{1}({2});", this._objectVariableName, ManagedMarshalValue.Naming.ForFieldSetter(this._field), value);
			}
			else
			{
				result = string.Format("{0} = {1};", this._objectVariableName, value);
			}
			return result;
		}

		public string Store(string format, params object[] args)
		{
			return this.Store(string.Format(format, args));
		}

		public string GetNiceName()
		{
			string text = this._objectVariableName;
			if (this._field != null)
			{
				text = text + "_" + this._field.Name;
			}
			if (this._indexVariableName != null)
			{
				text += "_item";
			}
			return ManagedMarshalValue.Naming.Clean(text.Replace("*", string.Empty));
		}

		public override string ToString()
		{
			throw new NotSupportedException();
		}
	}
}
