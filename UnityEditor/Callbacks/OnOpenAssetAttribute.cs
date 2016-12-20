namespace UnityEditor.Callbacks
{
    using System;
    using UnityEditor;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Callback attribute for opening an asset in Unity (e.g the callback is fired when double clicking an asset in the Project Browser).</para>
    /// </summary>
    [RequiredByNativeCode]
    public sealed class OnOpenAssetAttribute : CallbackOrderAttribute
    {
        public OnOpenAssetAttribute()
        {
            base.m_CallbackOrder = 1;
        }

        public OnOpenAssetAttribute(int callbackOrder)
        {
            base.m_CallbackOrder = callbackOrder;
        }
    }
}

