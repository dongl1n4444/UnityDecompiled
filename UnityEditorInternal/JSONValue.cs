namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct JSONValue
    {
        private object data;
        public JSONValue(object o)
        {
            this.data = o;
        }

        public bool IsString()
        {
            return (this.data is string);
        }

        public bool IsFloat()
        {
            return (this.data is float);
        }

        public bool IsList()
        {
            return (this.data is List<JSONValue>);
        }

        public bool IsDict()
        {
            return (this.data is Dictionary<string, JSONValue>);
        }

        public bool IsBool()
        {
            return (this.data is bool);
        }

        public bool IsNull()
        {
            return (this.data == null);
        }

        public static implicit operator JSONValue(string s)
        {
            return new JSONValue(s);
        }

        public static implicit operator JSONValue(float s)
        {
            return new JSONValue(s);
        }

        public static implicit operator JSONValue(bool s)
        {
            return new JSONValue(s);
        }

        public static implicit operator JSONValue(int s)
        {
            return new JSONValue((float) s);
        }

        public object AsObject()
        {
            return this.data;
        }

        public string AsString(bool nothrow)
        {
            if (this.data is string)
            {
                return (string) this.data;
            }
            if (!nothrow)
            {
                throw new JSONTypeException("Tried to read non-string json value as string");
            }
            return "";
        }

        public string AsString()
        {
            return this.AsString(false);
        }

        public float AsFloat(bool nothrow)
        {
            if (this.data is float)
            {
                return (float) this.data;
            }
            if (!nothrow)
            {
                throw new JSONTypeException("Tried to read non-float json value as float");
            }
            return 0f;
        }

        public float AsFloat()
        {
            return this.AsFloat(false);
        }

        public bool AsBool(bool nothrow)
        {
            if (this.data is bool)
            {
                return (bool) this.data;
            }
            if (!nothrow)
            {
                throw new JSONTypeException("Tried to read non-bool json value as bool");
            }
            return false;
        }

        public bool AsBool()
        {
            return this.AsBool(false);
        }

        public List<JSONValue> AsList(bool nothrow)
        {
            if (this.data is List<JSONValue>)
            {
                return (List<JSONValue>) this.data;
            }
            if (!nothrow)
            {
                throw new JSONTypeException("Tried to read " + this.data.GetType().Name + " json value as list");
            }
            return null;
        }

        public List<JSONValue> AsList()
        {
            return this.AsList(false);
        }

        public Dictionary<string, JSONValue> AsDict(bool nothrow)
        {
            if (this.data is Dictionary<string, JSONValue>)
            {
                return (Dictionary<string, JSONValue>) this.data;
            }
            if (!nothrow)
            {
                throw new JSONTypeException("Tried to read non-dictionary json value as dictionary");
            }
            return null;
        }

        public Dictionary<string, JSONValue> AsDict()
        {
            return this.AsDict(false);
        }

        public static JSONValue NewString(string val)
        {
            return new JSONValue(val);
        }

        public static JSONValue NewFloat(float val)
        {
            return new JSONValue(val);
        }

        public static JSONValue NewDict()
        {
            return new JSONValue(new Dictionary<string, JSONValue>());
        }

        public static JSONValue NewList()
        {
            return new JSONValue(new List<JSONValue>());
        }

        public static JSONValue NewBool(bool val)
        {
            return new JSONValue(val);
        }

        public static JSONValue NewNull()
        {
            return new JSONValue(null);
        }

        public JSONValue this[string index]
        {
            get
            {
                return this.AsDict()[index];
            }
            set
            {
                if (this.data == null)
                {
                    this.data = new Dictionary<string, JSONValue>();
                }
                this.AsDict()[index] = value;
            }
        }
        public bool ContainsKey(string index)
        {
            if (!this.IsDict())
            {
                return false;
            }
            return this.AsDict().ContainsKey(index);
        }

        public JSONValue Get(string key)
        {
            if (!this.IsDict())
            {
                return new JSONValue(null);
            }
            JSONValue value3 = this;
            char[] separator = new char[] { '.' };
            foreach (string str in key.Split(separator))
            {
                if (!value3.ContainsKey(str))
                {
                    return new JSONValue(null);
                }
                value3 = value3[str];
            }
            return value3;
        }

        public void Set(string key, string value)
        {
            if (value == null)
            {
                this[key] = NewNull();
            }
            else
            {
                this[key] = NewString(value);
            }
        }

        public void Set(string key, float value)
        {
            this[key] = NewFloat(value);
        }

        public void Set(string key, bool value)
        {
            this[key] = NewBool(value);
        }

        public void Add(string value)
        {
            List<JSONValue> list = this.AsList();
            if (value == null)
            {
                list.Add(NewNull());
            }
            else
            {
                list.Add(NewString(value));
            }
        }

        public void Add(float value)
        {
            this.AsList().Add(NewFloat(value));
        }

        public void Add(bool value)
        {
            this.AsList().Add(NewBool(value));
        }

        public override string ToString()
        {
            if (this.IsString())
            {
                return ("\"" + EncodeString(this.AsString()) + "\"");
            }
            if (this.IsFloat())
            {
                return this.AsFloat().ToString();
            }
            if (this.IsList())
            {
                string str2 = "[";
                string str3 = "";
                foreach (JSONValue value2 in this.AsList())
                {
                    str2 = str2 + str3 + value2.ToString();
                    str3 = ", ";
                }
                return (str2 + "]");
            }
            if (this.IsDict())
            {
                string str4 = "{";
                string str5 = "";
                foreach (KeyValuePair<string, JSONValue> pair in this.AsDict())
                {
                    string str6 = str4;
                    object[] objArray1 = new object[] { str6, str5, '"', EncodeString(pair.Key), "\" : ", pair.Value.ToString() };
                    str4 = string.Concat(objArray1);
                    str5 = ", ";
                }
                return (str4 + "}");
            }
            if (this.IsBool())
            {
                return (!this.AsBool() ? "false" : "true");
            }
            if (!this.IsNull())
            {
                throw new JSONTypeException("Cannot serialize json value of unknown type");
            }
            return "null";
        }

        private static string EncodeString(string str)
        {
            str = str.Replace("\"", "\\\"");
            str = str.Replace(@"\", @"\\");
            str = str.Replace("\b", @"\b");
            str = str.Replace("\f", @"\f");
            str = str.Replace("\n", @"\n");
            str = str.Replace("\r", @"\r");
            str = str.Replace("\t", @"\t");
            return str;
        }
    }
}

