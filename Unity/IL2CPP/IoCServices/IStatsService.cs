namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public interface IStatsService
    {
        void RecordArrayBoundsCheckEmitted(MethodDefinition methodDefinition);
        void RecordDivideByZeroCheckEmitted(MethodDefinition methodDefinition);
        void RecordMemoryBarrierEmitted(MethodDefinition methodDefinition);
        void RecordMetadataStream(string name, long size);
        void RecordMethod(MethodReference method);
        void RecordNullCheckEmitted(MethodDefinition methodDefinition);
        void RecordStringLiteral(string str);
        void RecordTailCall(MethodDefinition methodDefinition);
        void WriteStats(TextWriter writer);

        HashSet<string> ArrayBoundsChecksMethods { get; }

        long ConversionMilliseconds { get; set; }

        HashSet<string> DivideByZeroChecksMethods { get; }

        bool EnableArrayBoundsCheckRecording { get; set; }

        bool EnableDivideByZeroCheckRecording { get; set; }

        bool EnableNullChecksRecording { get; set; }

        int FilesWritten { get; set; }

        int GenericMethods { get; set; }

        int GenericTypeMethods { get; set; }

        HashSet<string> MemoryBarrierMethods { get; }

        int MethodHashCollisions { get; set; }

        int Methods { get; set; }

        Dictionary<string, int> NullCheckMethodsCount { get; }

        HashSet<string> NullChecksMethods { get; }

        int ShareableMethods { get; set; }

        int StringLiteralHashCollisions { get; set; }

        int StringLiterals { get; set; }

        int TailCallsEncountered { get; }

        int TypeHashCollisions { get; set; }

        int TypesConverted { get; set; }
    }
}

