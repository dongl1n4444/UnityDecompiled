namespace Unity.IL2CPP.StringLiterals
{
    using System;
    using System.Collections.ObjectModel;

    public interface IStringLiteralCollection
    {
        int GetIndex(string literal);
        ReadOnlyCollection<string> GetStringLiterals();
    }
}

