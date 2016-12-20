namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>A Custom Attribute that can be added to member functions of NetworkBehaviour scripts, to make them only run on servers.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ServerAttribute : Attribute
    {
    }
}

