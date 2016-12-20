namespace UnityEngine
{
    using System;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>SharedBetweenAnimatorsAttribute is an attribute that specify that this StateMachineBehaviour should be instantiate only once and shared among all Animator instance. This attribute reduce the memory footprint for each controller instance.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false), RequiredByNativeCode]
    public sealed class SharedBetweenAnimatorsAttribute : Attribute
    {
    }
}

