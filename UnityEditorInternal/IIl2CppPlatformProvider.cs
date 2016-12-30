namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEditor.BuildReporting;

    internal interface IIl2CppPlatformProvider
    {
        Il2CppNativeCodeBuilder CreateIl2CppNativeCodeBuilder();
        INativeCompiler CreateNativeCompiler();

        BuildReport buildReport { get; }

        bool developmentMode { get; }

        bool emitNullChecks { get; }

        bool enableArrayBoundsCheck { get; }

        bool enableDivideByZeroCheck { get; }

        bool enableStackTraces { get; }

        string il2CppFolder { get; }

        string[] includePaths { get; }

        string[] libraryPaths { get; }

        string moduleStrippingInformationFolder { get; }

        string nativeLibraryFileName { get; }

        bool supportsEngineStripping { get; }

        BuildTarget target { get; }
    }
}

