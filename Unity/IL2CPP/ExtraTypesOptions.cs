namespace Unity.IL2CPP
{
    using System;
    using Unity.Options;

    [ProgramOptions(Group="extra-types", CollectionSeparator="|")]
    public sealed class ExtraTypesOptions
    {
        [HideFromHelp]
        public static string[] File;
        [HideFromHelp]
        public static string[] Name;
    }
}

