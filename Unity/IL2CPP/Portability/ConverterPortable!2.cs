namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate TOutput ConverterPortable<in TInput, out TOutput>(TInput input);
}

