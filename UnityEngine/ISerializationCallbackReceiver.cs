namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public interface ISerializationCallbackReceiver
    {
        /// <summary>
        /// <para>Implement this method to receive a callback after Unity de-serializes your object.</para>
        /// </summary>
        void OnAfterDeserialize();
        /// <summary>
        /// <para>Implement this method to receive a callback before Unity serializes your object.</para>
        /// </summary>
        void OnBeforeSerialize();
    }
}

