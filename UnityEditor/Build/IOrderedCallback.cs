namespace UnityEditor.Build
{
    using System;

    public interface IOrderedCallback
    {
        /// <summary>
        /// <para>Returns the relative callback order for callbacks.  Callbacks with lower values are called before ones with higher values.</para>
        /// </summary>
        int callbackOrder { get; }
    }
}

