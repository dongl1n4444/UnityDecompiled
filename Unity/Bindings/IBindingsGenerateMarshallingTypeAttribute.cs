namespace Unity.Bindings
{
    using System;

    internal interface IBindingsGenerateMarshallingTypeAttribute : IBindingsNameProviderAttribute, IBindingsAttribute
    {
        NativeStructGenerateOption GenerateMarshallingType { get; set; }
    }
}

