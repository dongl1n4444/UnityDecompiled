namespace Unity.IL2CPP.StringLiterals
{
    using System;
    using Unity.IL2CPP;

    public interface IStringLiteralProvider
    {
        int Add(StringMetadataToken literal);
    }
}

