namespace UnityEngine.WSA
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>This event occurs when window completes activation or deactivation, it also fires up when you snap and unsnap the application.</para>
    /// </summary>
    /// <param name="state"></param>
    public delegate void WindowActivated(WindowActivationState state);
}

