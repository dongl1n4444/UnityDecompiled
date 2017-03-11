namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public interface IStatsService
    {
        void AddCppFileCacheHits(int count);
        void AddCppTotalFiles(int count);
        void RecordArrayBoundsCheckEmitted(MethodDefinition methodDefinition);
        void RecordArrayComCallableWrapper();
        void RecordComCallableWrapper();
        void RecordDivideByZeroCheckEmitted(MethodDefinition methodDefinition);
        void RecordForwardedToBaseClassComCallableWrapperMethod();
        void RecordImplementedComCallableWrapperMethod();
        void RecordMemoryBarrierEmitted(MethodDefinition methodDefinition);
        void RecordMetadataStream(string name, long size);
        void RecordMethod(MethodReference method);
        void RecordNativeToManagedInterfaceAdapter();
        void RecordNullCheckEmitted(MethodDefinition methodDefinition);
        void RecordStringLiteral(string str);
        void RecordStrippedComCallableWrapperMethod();
        void RecordTailCall(MethodDefinition methodDefinition);
        void RecordWindowsRuntimeBoxedType();
        void RecordWindowsRuntimeTypeWithName();
        void WriteStats(TextWriter writer);

        HashSet<string> ArrayBoundsChecksMethods { get; }

        int ArrayComCallableWrappers { get; }

        int ComCallableWrappers { get; }

        long ConversionMilliseconds { get; set; }

        int CppCacheHitPercentage { get; }

        int CppFileCacheHits { get; }

        int CppFilesCompiled { get; }

        int CppTotalFiles { get; }

        HashSet<string> DivideByZeroChecksMethods { get; }

        bool EnableArrayBoundsCheckRecording { get; set; }

        bool EnableDivideByZeroCheckRecording { get; set; }

        bool EnableNullChecksRecording { get; set; }

        int FilesWritten { get; set; }

        int ForwardedToBaseClassComCallableWrapperMethods { get; }

        int GenericMethods { get; set; }

        int GenericTypeMethods { get; set; }

        int ImplementedComCallableWrapperMethods { get; }

        HashSet<string> MemoryBarrierMethods { get; }

        int MethodHashCollisions { get; set; }

        int Methods { get; set; }

        int NativeToManagedInterfaceAdapters { get; }

        Dictionary<string, int> NullCheckMethodsCount { get; }

        HashSet<string> NullChecksMethods { get; }

        int ShareableMethods { get; set; }

        int StringLiteralHashCollisions { get; set; }

        int StringLiterals { get; set; }

        int StrippedComCallableWrapperMethods { get; }

        int TailCallsEncountered { get; }

        int TypeHashCollisions { get; set; }

        int TypesConverted { get; set; }

        int WindowsRuntimeBoxedTypes { get; }

        int WindowsRuntimeTypesWithNames { get; }
    }
}

