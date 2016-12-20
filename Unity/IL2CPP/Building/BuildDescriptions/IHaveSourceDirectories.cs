namespace Unity.IL2CPP.Building.BuildDescriptions
{
    using System.Collections.Generic;

    internal interface IHaveSourceDirectories
    {
        IEnumerable<NPath> SourceDirectories { get; }
    }
}

