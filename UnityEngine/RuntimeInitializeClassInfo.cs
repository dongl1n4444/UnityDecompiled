namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal sealed class RuntimeInitializeClassInfo
    {
        private string m_AssemblyName;
        private string m_ClassName;
        private string[] m_MethodNames;
        private RuntimeInitializeLoadType[] m_LoadTypes;
        internal string assemblyName
        {
            get => 
                this.m_AssemblyName;
            set
            {
                this.m_AssemblyName = value;
            }
        }
        internal string className
        {
            get => 
                this.m_ClassName;
            set
            {
                this.m_ClassName = value;
            }
        }
        internal string[] methodNames
        {
            get => 
                this.m_MethodNames;
            set
            {
                this.m_MethodNames = value;
            }
        }
        internal RuntimeInitializeLoadType[] loadTypes
        {
            get => 
                this.m_LoadTypes;
            set
            {
                this.m_LoadTypes = value;
            }
        }
    }
}

