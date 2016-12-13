using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;

namespace Unity.IL2CPP.IoCServices
{
	public interface IStatsService
	{
		long ConversionMilliseconds
		{
			get;
			set;
		}

		int FilesWritten
		{
			get;
			set;
		}

		int TypesConverted
		{
			get;
			set;
		}

		int StringLiterals
		{
			get;
			set;
		}

		bool EnableNullChecksRecording
		{
			get;
			set;
		}

		bool EnableArrayBoundsCheckRecording
		{
			get;
			set;
		}

		bool EnableDivideByZeroCheckRecording
		{
			get;
			set;
		}

		int Methods
		{
			get;
			set;
		}

		int GenericTypeMethods
		{
			get;
			set;
		}

		int GenericMethods
		{
			get;
			set;
		}

		int ShareableMethods
		{
			get;
			set;
		}

		int TailCallsEncountered
		{
			get;
		}

		Dictionary<string, int> NullCheckMethodsCount
		{
			get;
		}

		HashSet<string> NullChecksMethods
		{
			get;
		}

		HashSet<string> ArrayBoundsChecksMethods
		{
			get;
		}

		HashSet<string> DivideByZeroChecksMethods
		{
			get;
		}

		HashSet<string> MemoryBarrierMethods
		{
			get;
		}

		int StringLiteralHashCollisions
		{
			get;
			set;
		}

		int TypeHashCollisions
		{
			get;
			set;
		}

		int MethodHashCollisions
		{
			get;
			set;
		}

		void RecordNullCheckEmitted(MethodDefinition methodDefinition);

		void RecordArrayBoundsCheckEmitted(MethodDefinition methodDefinition);

		void RecordMemoryBarrierEmitted(MethodDefinition methodDefinition);

		void RecordDivideByZeroCheckEmitted(MethodDefinition methodDefinition);

		void RecordTailCall(MethodDefinition methodDefinition);

		void WriteStats(TextWriter writer);

		void RecordStringLiteral(string str);

		void RecordMethod(MethodReference method);

		void RecordMetadataStream(string name, long size);
	}
}
