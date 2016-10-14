using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	public class StatsComponent : IStatsService, IDisposable
	{
		private const double BytesInKilobyte = 1024.0;

		private int _totalNullChecks;

		private readonly Dictionary<string, int> _nullCheckMethodsCount = new Dictionary<string, int>();

		private readonly HashSet<string> _nullChecksMethods = new HashSet<string>();

		private readonly HashSet<string> _arrayBoundsChecksMethods = new HashSet<string>();

		private readonly HashSet<string> _divideByZeroChecksMethods = new HashSet<string>();

		private readonly HashSet<string> _memoryBarrierMethods = new HashSet<string>();

		private long _metadataTotal;

		private readonly Dictionary<string, long> _metadataStreams = new Dictionary<string, long>();

		public long ConversionMilliseconds
		{
			get;
			set;
		}

		public int FilesWritten
		{
			get;
			set;
		}

		public int TypesConverted
		{
			get;
			set;
		}

		public int StringLiterals
		{
			get;
			set;
		}

		public int Methods
		{
			get;
			set;
		}

		public int GenericTypeMethods
		{
			get;
			set;
		}

		public int GenericMethods
		{
			get;
			set;
		}

		public int ShareableMethods
		{
			get;
			set;
		}

		public bool EnableNullChecksRecording
		{
			get;
			set;
		}

		public bool EnableArrayBoundsCheckRecording
		{
			get;
			set;
		}

		public bool EnableDivideByZeroCheckRecording
		{
			get;
			set;
		}

		public Dictionary<string, int> NullCheckMethodsCount
		{
			get
			{
				return this._nullCheckMethodsCount;
			}
		}

		public HashSet<string> NullChecksMethods
		{
			get
			{
				return this._nullChecksMethods;
			}
		}

		public HashSet<string> ArrayBoundsChecksMethods
		{
			get
			{
				return this._arrayBoundsChecksMethods;
			}
		}

		public HashSet<string> DivideByZeroChecksMethods
		{
			get
			{
				return this._divideByZeroChecksMethods;
			}
		}

		public HashSet<string> MemoryBarrierMethods
		{
			get
			{
				return this._memoryBarrierMethods;
			}
		}

		public int StringLiteralHashCollisions
		{
			get;
			set;
		}

		public int TypeHashCollisions
		{
			get;
			set;
		}

		public int MethodHashCollisions
		{
			get;
			set;
		}

		public void RecordNullCheckEmitted(MethodDefinition methodDefinition)
		{
			this._totalNullChecks++;
			if (this.EnableNullChecksRecording)
			{
				string fullName = methodDefinition.FullName;
				if (!this._nullCheckMethodsCount.ContainsKey(fullName))
				{
					this._nullCheckMethodsCount[fullName] = 0;
				}
				Dictionary<string, int> nullCheckMethodsCount;
				string key;
				(nullCheckMethodsCount = this._nullCheckMethodsCount)[key = fullName] = nullCheckMethodsCount[key] + 1;
				this._nullChecksMethods.Add(fullName);
			}
		}

		public void RecordArrayBoundsCheckEmitted(MethodDefinition methodDefinition)
		{
			if (this.EnableArrayBoundsCheckRecording)
			{
				this._arrayBoundsChecksMethods.Add(methodDefinition.FullName);
			}
		}

		public void RecordDivideByZeroCheckEmitted(MethodDefinition methodDefinition)
		{
			if (this.EnableDivideByZeroCheckRecording)
			{
				this._divideByZeroChecksMethods.Add(methodDefinition.FullName);
			}
		}

		public void RecordMemoryBarrierEmitted(MethodDefinition methodDefinition)
		{
			this._memoryBarrierMethods.Add(methodDefinition.FullName);
		}

		public void WriteStats(TextWriter writer)
		{
			writer.WriteLine("----- il2cpp Statistics -----");
			writer.WriteLine("General:");
			writer.WriteLine("\tConversion Time: {0} s", (double)this.ConversionMilliseconds / 1000.0);
			writer.WriteLine("\tFiles Written: {0}", this.FilesWritten);
			writer.WriteLine("\tString Literals: {0}", this.StringLiterals);
			writer.WriteLine("Methods:");
			writer.WriteLine("\tTotal Methods: {0}", this.Methods);
			writer.WriteLine("\tNon-Generic Methods: {0}", this.Methods - (this.GenericTypeMethods + this.GenericMethods));
			writer.WriteLine("\tGeneric Type Methods: {0}", this.GenericTypeMethods);
			writer.WriteLine("\tGeneric Methods: {0}", this.GenericMethods);
			writer.WriteLine("\tShared Methods: {0}", this.ShareableMethods);
			writer.WriteLine("Metadata:");
			writer.WriteLine("\tTotal: {0:N1} kb", (double)this._metadataTotal / 1024.0);
			foreach (KeyValuePair<string, long> current in this._metadataStreams)
			{
				writer.WriteLine("\t{0}: {1:N1} kb", current.Key, (double)current.Value / 1024.0);
			}
			writer.WriteLine("Codegen:");
			writer.WriteLine("\tNullChecks : {0}", this._totalNullChecks);
			writer.WriteLine("Hashing:");
			writer.WriteLine("\tString Literal Hash Collisions : {0}", this.StringLiteralHashCollisions);
			writer.WriteLine("\tType Hash Collisions : {0}", this.TypeHashCollisions);
			writer.WriteLine("\tMethod Hash Collisions : {0}", this.MethodHashCollisions);
			writer.WriteLine();
		}

		public void RecordStringLiteral(string str)
		{
			this.StringLiterals++;
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

		public void RecordMetadataStream(string name, long size)
		{
			this._metadataTotal += size;
			this._metadataStreams.Add(name, size);
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
	}
}
