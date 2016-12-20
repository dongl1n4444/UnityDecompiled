namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [Serializable]
    internal sealed class WebViewV8CallbackCSharp
    {
        [SerializeField]
        private IntPtr m_thisDummy;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Callback(string result);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void DestroyCallBack();
        public void OnDestroy()
        {
            this.DestroyCallBack();
        }
    }
}

