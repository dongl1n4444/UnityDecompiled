namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Serializable]
    internal sealed class WebViewV8CallbackCSharp
    {
        [SerializeField]
        private IntPtr m_thisDummy;

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void Callback(string result);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void DestroyCallBack();
        public void OnDestroy()
        {
            this.DestroyCallBack();
        }
    }
}

