namespace UnityEngine.Analytics
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential)]
    internal class CustomEventData : IDisposable
    {
        [NonSerialized]
        internal IntPtr m_Ptr;
        private CustomEventData()
        {
        }

        public CustomEventData(string name)
        {
            this.InternalCreate(name);
        }

        ~CustomEventData()
        {
            this.InternalDestroy();
        }

        public void Dispose()
        {
            this.InternalDestroy();
            GC.SuppressFinalize(this);
        }

        public bool Add(string key, string value) => 
            this.AddString(key, value);

        public bool Add(string key, bool value) => 
            this.AddBool(key, value);

        public bool Add(string key, char value) => 
            this.AddChar(key, value);

        public bool Add(string key, byte value) => 
            this.AddByte(key, value);

        public bool Add(string key, sbyte value) => 
            this.AddSByte(key, value);

        public bool Add(string key, short value) => 
            this.AddInt16(key, value);

        public bool Add(string key, ushort value) => 
            this.AddUInt16(key, value);

        public bool Add(string key, int value) => 
            this.AddInt32(key, value);

        public bool Add(string key, uint value) => 
            this.AddUInt32(key, value);

        public bool Add(string key, long value) => 
            this.AddInt64(key, value);

        public bool Add(string key, ulong value) => 
            this.AddUInt64(key, value);

        public bool Add(string key, float value) => 
            this.AddDouble(key, (double) Convert.ToDecimal(value));

        public bool Add(string key, double value) => 
            this.AddDouble(key, value);

        public bool Add(string key, decimal value) => 
            this.AddDouble(key, (double) Convert.ToDecimal(value));

        public bool Add(IDictionary<string, object> eventData)
        {
            foreach (KeyValuePair<string, object> pair in eventData)
            {
                string key = pair.Key;
                object obj2 = pair.Value;
                if (obj2 == null)
                {
                    this.Add(key, "null");
                }
                else
                {
                    System.Type type = obj2.GetType();
                    if (type == typeof(string))
                    {
                        this.Add(key, (string) obj2);
                    }
                    else if (type == typeof(char))
                    {
                        this.Add(key, (char) obj2);
                    }
                    else if (type == typeof(sbyte))
                    {
                        this.Add(key, (sbyte) obj2);
                    }
                    else if (type == typeof(byte))
                    {
                        this.Add(key, (byte) obj2);
                    }
                    else if (type == typeof(short))
                    {
                        this.Add(key, (short) obj2);
                    }
                    else if (type == typeof(ushort))
                    {
                        this.Add(key, (ushort) obj2);
                    }
                    else if (type == typeof(int))
                    {
                        this.Add(key, (int) obj2);
                    }
                    else if (type == typeof(uint))
                    {
                        this.Add(pair.Key, (uint) obj2);
                    }
                    else if (type == typeof(long))
                    {
                        this.Add(key, (long) obj2);
                    }
                    else if (type == typeof(ulong))
                    {
                        this.Add(key, (ulong) obj2);
                    }
                    else if (type == typeof(bool))
                    {
                        this.Add(key, (bool) obj2);
                    }
                    else if (type == typeof(float))
                    {
                        this.Add(key, (float) obj2);
                    }
                    else if (type == typeof(double))
                    {
                        this.Add(key, (double) obj2);
                    }
                    else if (type == typeof(decimal))
                    {
                        this.Add(key, (decimal) obj2);
                    }
                    else
                    {
                        if (!type.IsValueType)
                        {
                            throw new ArgumentException($"Invalid type: {type} passed");
                        }
                        this.Add(key, obj2.ToString());
                    }
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void InternalCreate(string name);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        internal extern void InternalDestroy();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool AddString(string key, string value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool AddBool(string key, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool AddChar(string key, char value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool AddByte(string key, byte value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool AddSByte(string key, sbyte value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool AddInt16(string key, short value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool AddUInt16(string key, ushort value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool AddInt32(string key, int value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool AddUInt32(string key, uint value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool AddInt64(string key, long value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool AddUInt64(string key, ulong value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern bool AddDouble(string key, double value);
    }
}

