namespace Unity.IL2CPP.StringLiterals
{
    using System;
    using System.Collections.ObjectModel;
    using Unity.IL2CPP;

    public interface IStringLiteralCollection
    {
        int GetIndex(StringMetadataToken literal);
        ReadOnlyCollection<string> GetStringLiterals();
        ReadOnlyCollection<StringMetadataToken> GetStringMetadataTokens();
    }
}

