namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Text file assets.</para>
    /// </summary>
    public class TextAsset : UnityEngine.Object
    {
        public override string ToString() => 
            this.text;

        /// <summary>
        /// <para>The raw bytes of the text asset. (Read Only)</para>
        /// </summary>
        public byte[] bytes { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The text contents of the .txt file as a string. (Read Only)</para>
        /// </summary>
        public string text { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

