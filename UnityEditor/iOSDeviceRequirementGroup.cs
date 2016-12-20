﻿namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal sealed class iOSDeviceRequirementGroup
    {
        private string m_VariantName;

        internal iOSDeviceRequirementGroup(string variantName)
        {
            this.m_VariantName = variantName;
        }

        public void Add(iOSDeviceRequirement requirement)
        {
            SetOrAddDeviceRequirementForVariantNameImpl(this.m_VariantName, -1, Enumerable.ToArray<string>(requirement.values.Keys), Enumerable.ToArray<string>(requirement.values.Values));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int GetCountForVariantImpl(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetDeviceRequirementForVariantNameImpl(string name, int index, out string[] keys, out string[] values);
        public void RemoveAt(int index)
        {
            RemoveAtImpl(this.m_VariantName, index);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void RemoveAtImpl(string name, int index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void SetOrAddDeviceRequirementForVariantNameImpl(string name, int index, string[] keys, string[] values);

        public int count
        {
            get
            {
                return GetCountForVariantImpl(this.m_VariantName);
            }
        }

        public iOSDeviceRequirement this[int index]
        {
            get
            {
                string[] strArray;
                string[] strArray2;
                GetDeviceRequirementForVariantNameImpl(this.m_VariantName, index, out strArray, out strArray2);
                iOSDeviceRequirement requirement = new iOSDeviceRequirement();
                for (int i = 0; i < strArray.Length; i++)
                {
                    requirement.values.Add(strArray[i], strArray2[i]);
                }
                return requirement;
            }
            set
            {
                SetOrAddDeviceRequirementForVariantNameImpl(this.m_VariantName, index, Enumerable.ToArray<string>(value.values.Keys), Enumerable.ToArray<string>(value.values.Values));
            }
        }
    }
}

