namespace Unity.Bindings
{
    using System;

    internal interface IBindingsNameProviderAttribute : IBindingsAttribute
    {
        string Name { get; set; }
    }
}

