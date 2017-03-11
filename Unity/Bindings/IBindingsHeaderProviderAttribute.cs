namespace Unity.Bindings
{
    using System;

    internal interface IBindingsHeaderProviderAttribute : IBindingsAttribute
    {
        string Header { get; set; }
    }
}

