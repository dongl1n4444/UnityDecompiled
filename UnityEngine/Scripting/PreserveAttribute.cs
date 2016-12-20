namespace UnityEngine.Scripting
{
    using System;

    /// <summary>
    /// <para>PreserveAttribute prevents byte code stripping from removing a class, method, field, or property.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class, Inherited=false)]
    public class PreserveAttribute : Attribute
    {
    }
}

