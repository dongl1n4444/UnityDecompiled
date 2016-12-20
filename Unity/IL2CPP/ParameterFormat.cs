namespace Unity.IL2CPP
{
    using System;

    public enum ParameterFormat
    {
        WithTypeAndName,
        WithTypeAndNameNoThis,
        WithType,
        WithTypeNoThis,
        WithName,
        WithTypeAndNameThisObject,
        WithTypeThisObject,
        WithNameNoThis,
        WithNameCastThis,
        WithNameUnboxThis
    }
}

