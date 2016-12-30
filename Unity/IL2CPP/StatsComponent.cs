namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.IoCServices;

    public class StatsComponent : IStatsService, IDisposable
    {
        private readonly HashSet<string> _arrayBoundsChecksMethods = new HashSet<string>();
        private int _arrayComCallableWrappers;
        private int _comCallableWrappers;
        private readonly HashSet<string> _divideByZeroChecksMethods = new HashSet<string>();
        private int _forwardedToBaseClassComCallableWrapperMethods;
        private int _implementedComCallableWrapperMethods;
        private readonly HashSet<string> _memoryBarrierMethods = new HashSet<string>();
        private readonly Dictionary<string, long> _metadataStreams = new Dictionary<string, long>();
        private long _metadataTotal;
        private readonly HashSet<string> _methodsWithTailCalls = new HashSet<string>();
        private readonly Dictionary<string, int> _nullCheckMethodsCount = new Dictionary<string, int>();
        private readonly HashSet<string> _nullChecksMethods = new HashSet<string>();
        private int _strippedComCallableWrapperMethods;
        private int _totalNullChecks;
        private int _windowsRuntimeBoxedTypes;
        private int _windowsRuntimeTypesWithNames;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long <ConversionMilliseconds>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <CppFileCacheHits>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <CppTotalFiles>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <EnableArrayBoundsCheckRecording>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <EnableDivideByZeroCheckRecording>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool <EnableNullChecksRecording>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <FilesWritten>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <GenericMethods>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <GenericTypeMethods>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <MethodHashCollisions>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <Methods>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <ShareableMethods>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <StringLiteralHashCollisions>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <StringLiterals>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <TypeHashCollisions>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <TypesConverted>k__BackingField;
        private const double BytesInKilobyte = 1024.0;

        public void AddCppFileCacheHits(int count)
        {
            this.CppFileCacheHits += count;
        }

        public void AddCppTotalFiles(int count)
        {
            this.CppTotalFiles += count;
        }

        public void Dispose()
        {
            this.ConversionMilliseconds = 0L;
            this.FilesWritten = 0;
            this.TypesConverted = 0;
            this.StringLiterals = 0;
            this.StringLiteralHashCollisions = 0;
            this.TypeHashCollisions = 0;
            this.MethodHashCollisions = 0;
            this.Methods = 0;
            this.GenericTypeMethods = 0;
            this.GenericMethods = 0;
            this.ShareableMethods = 0;
            this._metadataTotal = 0L;
            this._nullChecksMethods.Clear();
            this._arrayBoundsChecksMethods.Clear();
            this._nullCheckMethodsCount.Clear();
            this._memoryBarrierMethods.Clear();
            this._metadataStreams.Clear();
        }

        public void RecordArrayBoundsCheckEmitted(MethodDefinition methodDefinition)
        {
            if (this.EnableArrayBoundsCheckRecording)
            {
                this._arrayBoundsChecksMethods.Add(methodDefinition.FullName);
            }
        }

        public void RecordArrayComCallableWrapper()
        {
            this._arrayComCallableWrappers++;
        }

        public void RecordComCallableWrapper()
        {
            this._comCallableWrappers++;
        }

        public void RecordDivideByZeroCheckEmitted(MethodDefinition methodDefinition)
        {
            if (this.EnableDivideByZeroCheckRecording)
            {
                this._divideByZeroChecksMethods.Add(methodDefinition.FullName);
            }
        }

        public void RecordForwardedToBaseClassComCallableWrapperMethod()
        {
            this._forwardedToBaseClassComCallableWrapperMethods++;
        }

        public void RecordImplementedComCallableWrapperMethod()
        {
            this._implementedComCallableWrapperMethods++;
        }

        public void RecordMemoryBarrierEmitted(MethodDefinition methodDefinition)
        {
            this._memoryBarrierMethods.Add(methodDefinition.FullName);
        }

        public void RecordMetadataStream(string name, long size)
        {
            this._metadataTotal += size;
            this._metadataStreams.Add(name, size);
        }

        public void RecordMethod(MethodReference method)
        {
            this.Methods++;
            if (method.DeclaringType.IsGenericInstance)
            {
                this.GenericTypeMethods++;
            }
            if (method.IsGenericInstance)
            {
                this.GenericMethods++;
            }
        }

        public void RecordNullCheckEmitted(MethodDefinition methodDefinition)
        {
            this._totalNullChecks++;
            if (this.EnableNullChecksRecording)
            {
                Dictionary<string, int> dictionary;
                string str2;
                string fullName = methodDefinition.FullName;
                if (!this._nullCheckMethodsCount.ContainsKey(fullName))
                {
                    this._nullCheckMethodsCount[fullName] = 0;
                }
                (dictionary = this._nullCheckMethodsCount)[str2 = fullName] = dictionary[str2] + 1;
                this._nullChecksMethods.Add(fullName);
            }
        }

        public void RecordStringLiteral(string str)
        {
            this.StringLiterals++;
        }

        public void RecordStrippedComCallableWrapperMethod()
        {
            this._strippedComCallableWrapperMethods++;
        }

        public void RecordTailCall(MethodDefinition methodDefinition)
        {
            this._methodsWithTailCalls.Add(methodDefinition.FullName);
        }

        public void RecordWindowsRuntimeBoxedType()
        {
            this._windowsRuntimeBoxedTypes++;
        }

        public void RecordWindowsRuntimeTypeWithName()
        {
            this._windowsRuntimeTypesWithNames++;
        }

        public void WriteStats(TextWriter writer)
        {
            writer.WriteLine("----- il2cpp Statistics -----");
            writer.WriteLine("General:");
            writer.WriteLine("\tConversion Time: {0} s", ((double) this.ConversionMilliseconds) / 1000.0);
            writer.WriteLine("\tFiles Written: {0}", this.FilesWritten);
            writer.WriteLine("\tString Literals: {0}", this.StringLiterals);
            writer.WriteLine("Methods:");
            writer.WriteLine("\tTotal Methods: {0}", this.Methods);
            writer.WriteLine("\tNon-Generic Methods: {0}", this.Methods - (this.GenericTypeMethods + this.GenericMethods));
            writer.WriteLine("\tGeneric Type Methods: {0}", this.GenericTypeMethods);
            writer.WriteLine("\tGeneric Methods: {0}", this.GenericMethods);
            writer.WriteLine("\tShared Methods: {0}", this.ShareableMethods);
            writer.WriteLine("\tMethods with Tail Calls : {0}", this._methodsWithTailCalls.Count);
            writer.WriteLine("Metadata:");
            writer.WriteLine("\tTotal: {0:N1} kb", ((double) this._metadataTotal) / 1024.0);
            foreach (KeyValuePair<string, long> pair in this._metadataStreams)
            {
                writer.WriteLine("\t{0}: {1:N1} kb", pair.Key, ((double) pair.Value) / 1024.0);
            }
            writer.WriteLine("Codegen:");
            writer.WriteLine("\tNullChecks : {0}", this._totalNullChecks);
            writer.WriteLine("Hashing:");
            writer.WriteLine("\tString Literal Hash Collisions : {0}", this.StringLiteralHashCollisions);
            writer.WriteLine("\tType Hash Collisions : {0}", this.TypeHashCollisions);
            writer.WriteLine("\tMethod Hash Collisions : {0}", this.MethodHashCollisions);
            writer.WriteLine("Interop:");
            writer.WriteLine($"	Windows Runtime boxed types : {this.WindowsRuntimeBoxedTypes}");
            writer.WriteLine($"	Windows Runtime types with names : {this.WindowsRuntimeTypesWithNames}");
            writer.WriteLine($"	Array COM callable wrappers : {this.ArrayComCallableWrappers}");
            writer.WriteLine($"	COM callable wrappers : {this.ComCallableWrappers}");
            writer.WriteLine($"	COM callable wrapper methods that were implemented : {this.ImplementedComCallableWrapperMethods}");
            writer.WriteLine($"	COM callable wrapper methods that were stripped : {this.StrippedComCallableWrapperMethods}");
            writer.WriteLine($"	COM callable wrapper methods that were forwarded to call base class method : {this.ForwardedToBaseClassComCallableWrapperMethods}");
            if (this.CppTotalFiles > 0)
            {
                writer.WriteLine("Compilation:");
                writer.WriteLine("\tTotal Files: {0}", this.CppTotalFiles);
                writer.WriteLine("\tFiles Compiled: {0}", this.CppFilesCompiled);
                writer.WriteLine("\tCache Hits: {0}", this.CppFileCacheHits);
                writer.WriteLine("\tCache Hit %: {0}", this.CppCacheHitPercentage);
            }
            writer.WriteLine();
        }

        public HashSet<string> ArrayBoundsChecksMethods =>
            this._arrayBoundsChecksMethods;

        public int ArrayComCallableWrappers =>
            this._arrayComCallableWrappers;

        public int ComCallableWrappers =>
            this._comCallableWrappers;

        public long ConversionMilliseconds { get; set; }

        public int CppCacheHitPercentage =>
            ((this.CppFileCacheHits / this.CppTotalFiles) * 100);

        public int CppFileCacheHits { get; private set; }

        public int CppFilesCompiled =>
            (this.CppTotalFiles - this.CppFileCacheHits);

        public int CppTotalFiles { get; private set; }

        public HashSet<string> DivideByZeroChecksMethods =>
            this._divideByZeroChecksMethods;

        public bool EnableArrayBoundsCheckRecording { get; set; }

        public bool EnableDivideByZeroCheckRecording { get; set; }

        public bool EnableNullChecksRecording { get; set; }

        public int FilesWritten { get; set; }

        public int ForwardedToBaseClassComCallableWrapperMethods =>
            this._forwardedToBaseClassComCallableWrapperMethods;

        public int GenericMethods { get; set; }

        public int GenericTypeMethods { get; set; }

        public int ImplementedComCallableWrapperMethods =>
            this._implementedComCallableWrapperMethods;

        public HashSet<string> MemoryBarrierMethods =>
            this._memoryBarrierMethods;

        public int MethodHashCollisions { get; set; }

        public int Methods { get; set; }

        public Dictionary<string, int> NullCheckMethodsCount =>
            this._nullCheckMethodsCount;

        public HashSet<string> NullChecksMethods =>
            this._nullChecksMethods;

        public int ShareableMethods { get; set; }

        public int StringLiteralHashCollisions { get; set; }

        public int StringLiterals { get; set; }

        public int StrippedComCallableWrapperMethods =>
            this._strippedComCallableWrapperMethods;

        public int TailCallsEncountered =>
            this._methodsWithTailCalls.Count;

        public int TypeHashCollisions { get; set; }

        public int TypesConverted { get; set; }

        public int WindowsRuntimeBoxedTypes =>
            this._windowsRuntimeBoxedTypes;

        public int WindowsRuntimeTypesWithNames =>
            this._windowsRuntimeTypesWithNames;
    }
}

