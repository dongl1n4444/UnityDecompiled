namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    internal interface IExposedPropertyTable
    {
        void ClearReferenceValue(PropertyName id);
        UnityEngine.Object GetReferenceValue(PropertyName id, out bool idValid);
        void SetReferenceValue(PropertyName id, UnityEngine.Object value);
    }
}

