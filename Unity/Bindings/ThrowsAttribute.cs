namespace Unity.Bindings
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    internal class ThrowsAttribute : Attribute, IBindingsAttribute
    {
    }
}

