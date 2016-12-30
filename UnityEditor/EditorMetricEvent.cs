﻿namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential)]
    internal class EditorMetricEvent : IDisposable
    {
        internal IntPtr m_Ptr;
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern EditorMetricEvent(EditorMetricCollectionType en);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void AddValueStr(string key, string value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void AddValueInt(string key, int value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void AddChildValueBool(string parent, string key, bool value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void AddChildValueInt(string parent, string key, int value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void AddChildValueStr(string parent, string key, string value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Send();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private extern void Destroy();
        public void Dispose()
        {
            this.Destroy();
            this.m_Ptr = IntPtr.Zero;
        }

        ~EditorMetricEvent()
        {
            this.Dispose();
        }

        internal void AddValue(string key, string value)
        {
            this.AddValueStr(key, value);
        }

        internal void AddValue(string key, int value)
        {
            this.AddValueInt(key, value);
        }

        internal void AddChildValue(string parent, string key, int value)
        {
            this.AddChildValueInt(parent, key, value);
        }

        internal void AddChildValue(string parent, string key, string value)
        {
            this.AddChildValueStr(parent, key, value);
        }

        internal void AddChildValue(string parent, string key, bool value)
        {
            this.AddChildValueBool(parent, key, value);
        }
    }
}

