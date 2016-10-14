using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Marshaling.MarshalInfoWriters;

namespace Unity.IL2CPP.Marshaling.BodyWriters
{
	internal abstract class InteropMarshaler
	{
		[Inject]
		public static INamingService Naming;

		protected readonly TypeResolver _typeResolver;

		protected readonly MarshalType _marshalType;

		protected readonly bool _useUnicodeCharset;

		public InteropMarshaler(TypeResolver typeResolver, MarshalType marshalType, bool useUnicodeCharset)
		{
			this._typeResolver = typeResolver;
			this._marshalType = marshalType;
			this._useUnicodeCharset = useUnicodeCharset;
		}

		public abstract bool CanMarshalAsInputParameter(MarshaledParameter parameter);

		public abstract bool CanMarshalAsOutputParameter(MarshaledParameter parameter);

		public abstract bool CanMarshalAsOutputParameter(MethodReturnType methodReturnType);

		public abstract string GetPrettyCalleeName();

		public abstract string WriteMarshalEmptyInputParameter(CppCodeWriter writer, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess);

		public abstract string WriteMarshalInputParameter(CppCodeWriter writer, MarshaledParameter parameter, IList<MarshaledParameter> parameters, IRuntimeMetadataAccess metadataAccess);

		public abstract void WriteMarshalOutputParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IList<MarshaledParameter> parameters, IRuntimeMetadataAccess metadataAccess);

		public abstract string WriteMarshalReturnValue(CppCodeWriter writer, MethodReturnType methodReturnType, IList<MarshaledParameter> parameters, IRuntimeMetadataAccess metadataAccess);

		public abstract void WriteMarshalCleanupEmptyParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess);

		public abstract void WriteMarshalCleanupParameter(CppCodeWriter writer, string valueName, MarshaledParameter parameter, IRuntimeMetadataAccess metadataAccess);

		public abstract void WriteMarshalCleanupReturnValue(CppCodeWriter writer, MethodReturnType methodReturnType, IRuntimeMetadataAccess metadataAccess);

		public DefaultMarshalInfoWriter MarshalInfoWriterFor(MarshaledParameter parameter)
		{
			return MarshalDataCollector.MarshalInfoWriterFor(parameter.ParameterType, this._marshalType, parameter.MarshalInfo, this._useUnicodeCharset, false);
		}

		public DefaultMarshalInfoWriter MarshalInfoWriterFor(MethodReturnType methodReturnType)
		{
			return MarshalDataCollector.MarshalInfoWriterFor(this._typeResolver.Resolve(methodReturnType.ReturnType), this._marshalType, methodReturnType.MarshalInfo, this._useUnicodeCharset, false);
		}

		protected static Result WriteContentAndCommentIfNeeded<Result>(CppCodeWriter writer, string comment, Func<CppCodeWriter, Result> writeAction)
		{
			Result result2;
			using (InMemoryCodeWriter inMemoryCodeWriter = new InMemoryCodeWriter())
			{
				for (int i = 0; i < writer.IndentationLevel; i++)
				{
					inMemoryCodeWriter.Indent();
				}
				Result result = writeAction(inMemoryCodeWriter);
				for (int j = 0; j < writer.IndentationLevel; j++)
				{
					inMemoryCodeWriter.Dedent();
				}
				inMemoryCodeWriter.Writer.Flush();
				if (inMemoryCodeWriter.Writer.BaseStream.Length > 0L)
				{
					writer.WriteCommentedLine(comment);
					writer.Write(inMemoryCodeWriter);
					writer.WriteLine();
				}
				result2 = result;
			}
			return result2;
		}

		protected static void WriteContentAndCommentIfNeeded(CppCodeWriter writer, string comment, Action<CppCodeWriter> writeAction)
		{
			InteropMarshaler.WriteContentAndCommentIfNeeded<object>(writer, comment, delegate(CppCodeWriter bodyWriter)
			{
				writeAction(bodyWriter);
				return null;
			});
		}
	}
}
