namespace Unity.Bindings
{
    using System;

    internal interface IBindingsIsThreadSafeProviderAttribute : IBindingsAttribute
    {
        bool IsThreadSafe { get; set; }
    }
}

