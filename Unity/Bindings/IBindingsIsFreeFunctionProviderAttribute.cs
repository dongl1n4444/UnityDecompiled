namespace Unity.Bindings
{
    using System;

    internal interface IBindingsIsFreeFunctionProviderAttribute : IBindingsAttribute
    {
        bool IsFreeFunction { get; set; }
    }
}

