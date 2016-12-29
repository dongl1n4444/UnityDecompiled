namespace Unity.IL2CPP.Building.ToolChains.MsvcVersions.VisualStudioAPI
{
    using System;

    [Flags]
    public enum InstanceState : uint
    {
        Complete = 0xffffffff,
        Local = 1,
        None = 0,
        NoRebootRequired = 4,
        Registered = 2
    }
}

