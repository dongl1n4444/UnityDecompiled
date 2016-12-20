namespace Unity.IL2CPP.Marshaling
{
    using Mono.Cecil;
    using System;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ManagedMarshalValue
    {
        [Inject]
        public static INamingService Naming;
        private readonly string _objectVariableName;
        private readonly FieldReference _field;
        private readonly string _indexVariableName;
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
            if (this._indexVariableName != null)
            {
                string str;
                if (this._field != null)
                {
                    str = string.Format("{0}.{1}()", this._objectVariableName, Naming.ForFieldGetter(this._field));
                }
                else
                {
                    str = this._objectVariableName;
                }
                return Emit.LoadArrayElement(str, this._indexVariableName, false);
            }
            if (this._field != null)
            {
                return string.Format("{0}.{1}()", this._objectVariableName, Naming.ForFieldGetter(this._field));
            }
            return this._objectVariableName;
        }

        public string LoadAddress()
        {
            if (this._indexVariableName != null)
            {
                throw new NotSupportedException();
            }
            if (this._field != null)
            {
                return string.Format("{0}.{1}()", this._objectVariableName, Naming.ForFieldAddressGetter(this._field));
            }
            return Naming.AddressOf(this._objectVariableName);
        }

        public string Store(string value)
        {
            if (this._indexVariableName != null)
            {
                string str;
                if (this._field != null)
                {
                    str = string.Format("{0}.{1}()", this._objectVariableName, Naming.ForFieldGetter(this._field));
                }
                else
                {
                    str = this._objectVariableName;
                }
                return string.Format("{0};", Emit.StoreArrayElement(str, this._indexVariableName, value, false));
            }
            if (this._field != null)
            {
                return string.Format("{0}.{1}({2});", this._objectVariableName, Naming.ForFieldSetter(this._field), value);
            }
            return string.Format("{0} = {1};", this._objectVariableName, value);
        }

        public string Store(string format, params object[] args)
        {
            return this.Store(string.Format(format, args));
        }

        public string GetNiceName()
        {
            string str = this._objectVariableName;
            if (this._field != null)
            {
                str = str + "_" + this._field.Name;
            }
            if (this._indexVariableName != null)
            {
                str = str + "_item";
            }
            return Naming.Clean(str.Replace("*", string.Empty));
        }

        public ManagedMarshalValue Dereferenced
        {
            get
            {
                if (this._objectVariableName.StartsWith("&"))
                {
                    return new ManagedMarshalValue(this._objectVariableName.Substring(1), this._field, this._indexVariableName);
                }
                return new ManagedMarshalValue("*" + this._objectVariableName, this._field, this._indexVariableName);
            }
        }
        public override string ToString()
        {
            throw new NotSupportedException();
        }
    }
}

