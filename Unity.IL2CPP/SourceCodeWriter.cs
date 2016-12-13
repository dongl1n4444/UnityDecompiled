using Mono.Cecil;
using NiceIO;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.Portability;

namespace Unity.IL2CPP
{
	public class SourceCodeWriter : InMemoryCodeWriter
	{
		private readonly NPath _filename;

		public SourceCodeWriter(NPath filename)
		{
			CppCodeWriter.StatsService.FilesWritten++;
			this._filename = filename;
			base.AddInclude("il2cpp-config.h");
			base.AddStdInclude("alloca.h");
			base.AddStdInclude("malloc.h");
		}

		public override void Dispose()
		{
			using (Unity.IL2CPP.Portability.StreamWriter streamWriter = new Unity.IL2CPP.Portability.StreamWriter(this._filename.ToString(), Encoding.UTF8))
			{
				if (this.IsHeaderFile())
				{
					streamWriter.WriteLine("#pragma once\n");
				}
				this.WriteCollectedIncludes(streamWriter);
				base.Writer.Flush();
				base.Writer.BaseStream.Seek(0L, SeekOrigin.Begin);
				base.Writer.BaseStream.CopyTo(streamWriter.BaseStream);
				streamWriter.Flush();
			}
			base.Dispose();
		}

		private bool IsHeaderFile()
		{
			return this._filename.HasExtension(new string[]
			{
				"h",
				"hh",
				"hpp"
			});
		}

		private void WriteCollectedIncludes(System.IO.StreamWriter writer)
		{
			string[] includesToSkip = new string[]
			{
				"\"il2cpp-config.h\"",
				"<alloca.h>",
				"<malloc.h>"
			};
			writer.WriteLine("#include \"il2cpp-config.h\"\n");
			writer.WriteLine("#ifndef _MSC_VER");
			writer.WriteLine("# include <alloca.h>");
			writer.WriteLine("#else");
			writer.WriteLine("# include <malloc.h>");
			writer.WriteLine("#endif");
			writer.WriteLine();
			foreach (string current in from i in base.Includes
			where !includesToSkip.Contains(i) && i.StartsWith("<")
			select i)
			{
				writer.WriteLine("#include {0}", current);
			}
			writer.WriteLine();
			foreach (TypeReference current2 in base.ForwardDeclarations)
			{
				if (CodeGenOptions.EmitComments)
				{
					writer.WriteLine("// {0}", current2.FullName);
				}
				writer.WriteLine("struct {0};", CppCodeWriter.Naming.ForType(current2));
			}
			foreach (string current3 in base.RawForwardDeclarations)
			{
				writer.WriteLine(current3 + ';');
			}
			writer.WriteLine();
			foreach (string current4 in from i in base.Includes
			where !includesToSkip.Contains(i) && !i.StartsWith("<")
			select i)
			{
				writer.WriteLine("#include {0}", current4);
			}
			writer.WriteLine();
			foreach (GenericInstanceMethod current5 in base.GenericInstanceMethods)
			{
				this.WriteGenericMethod(writer, current5);
			}
			writer.Flush();
		}

		private void WriteGenericMethod(System.IO.StreamWriter writer, GenericInstanceMethod method)
		{
			GenericInstanceType genericInstanceType = method.DeclaringType as GenericInstanceType;
			if (genericInstanceType != null && GenericsUtilities.CheckForMaximumRecursion(genericInstanceType))
			{
				TypeResolver typeResolver = new TypeResolver(method.DeclaringType as GenericInstanceType, method);
				writer.WriteLine("{0};", MethodSignatureWriter.GetMethodSignature(CppCodeWriter.Naming.ForMethodNameOnly(method), CppCodeWriter.Naming.ForVariable(typeResolver.Resolve(GenericParameterResolver.ResolveReturnTypeIfNeeded(method))), MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithTypeAndName, false, true), "extern \"C\"", string.Empty));
			}
			else
			{
				if (CodeGenOptions.EmitComments)
				{
					writer.WriteLine("// {0}", method.FullName);
				}
				MethodReference sharedMethod = CppCodeWriter.GenericSharingAnalysis.GetSharedMethod(method);
				if (CppCodeWriter.GenericSharingAnalysis.IsSharedMethod(method))
				{
					writer.WriteLine("{0};", MethodSignatureWriter.GetSharedMethodSignatureRaw(method));
				}
				string text = MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithName, false, true);
				writer.WriteLine("#define {0}({1}) (({2}){3})({4})", new object[]
				{
					CppCodeWriter.Naming.ForMethodNameOnly(method),
					text,
					MethodSignatureWriter.GetMethodPointer(method),
					CppCodeWriter.Naming.ForMethod(sharedMethod) + "_gshared",
					text
				});
			}
		}
	}
}
