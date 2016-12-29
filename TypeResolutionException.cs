using System;
using Unity.IL2CPP.Common;

internal class TypeResolutionException : Exception
{
    private readonly TypeNameParseInfo _typeNameInfo;

    public TypeResolutionException(TypeNameParseInfo typeNameInfo)
    {
        this._typeNameInfo = typeNameInfo;
    }

    public TypeNameParseInfo TypeNameInfo =>
        this._typeNameInfo;
}

