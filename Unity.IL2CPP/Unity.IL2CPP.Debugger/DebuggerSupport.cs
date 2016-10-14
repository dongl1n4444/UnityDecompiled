using Mono.Cecil;
using Mono.Cecil.Cil;
using NiceIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Cecil.Visitor;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP.Debugger
{
	public class DebuggerSupport : IDebuggerSupport
	{
		[Inject]
		public static INamingService Naming;

		private int _sequencePointUid;

		private readonly CollectSourceDocumentsVisitor _documentsCollector;

		private readonly HashSet<string> _documents = new HashSet<string>();

		private readonly Dictionary<string, string> _documentToVarName = new Dictionary<string, string>();

		private readonly Dictionary<string, string> _varNameToDocument = new Dictionary<string, string>();

		public DebuggerSupport()
		{
			this._documentsCollector = new CollectSourceDocumentsVisitor(this._documents);
		}

		public void Analyze(AssemblyDefinition assembly)
		{
			if (DebuggerOptions.Enabled)
			{
				assembly.Accept(this._documentsCollector);
			}
		}

		public void GenerateSupportFilesIfNeeded(NPath outputDir)
		{
			using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(outputDir.Combine(new string[]
			{
				"document-debug-info.h"
			})))
			{
				this.WriteDocumentDebugInfoHeader(sourceCodeWriter);
			}
			using (SourceCodeWriter sourceCodeWriter2 = new SourceCodeWriter(outputDir.Combine(new string[]
			{
				"document-debug-info.cpp"
			})))
			{
				this.WriteDocumentDebugInfoSource(sourceCodeWriter2);
			}
		}

		public void WriteDebugMetadataIncludes(CppCodeWriter writer)
		{
			writer.AddInclude("document-debug-info.h");
		}

		public void ExtendMethodInfoInitializer(FieldInitializers initializers, MethodReference methodReference, bool isGeneric)
		{
			initializers.Add("debug_info", (!isGeneric) ? ("&" + DebuggerSupport.Naming.ForDebugMethodInfo(methodReference)) : "0");
		}

		public void WriteDebugMethodInfoDefinitionFor(CppCodeWriter writer, MethodReference methodReference)
		{
			MethodDefinition methodDefinition = methodReference.Resolve();
			if (methodDefinition.HasBody)
			{
				this.WriteDebugLocalAndMethodInfoFor(writer, methodReference, methodDefinition);
			}
			else
			{
				this.WriteEmptyDebugMethodInfoFor(writer, methodReference, methodDefinition);
			}
		}

		private void WriteDebugLocalAndMethodInfoFor(CppCodeWriter writer, MethodReference methodReference, MethodDefinition methodDefinition)
		{
			DebuggerSupport.WriteOffsetTable(writer, methodReference, methodDefinition);
			DebuggerSupport.WriteDebugLocalInfos(writer, methodReference, methodDefinition);
			List<SequencePointInfo> sequencePoints = new List<SequencePointInfo>();
			string variableName = string.Format("{0}_sp", DebuggerSupport.Naming.ForDebugMethodInfo(methodReference));
			DebuggerSupport.WriteSequencePointsTableIfNeeded(writer, variableName, methodDefinition, sequencePoints);
			this.WriteDebugMethodInfos(writer, variableName, sequencePoints, methodReference, methodDefinition);
		}

		private void WriteDebugMethodInfos(CppCodeWriter writer, string variableName, ICollection sequencePoints, MethodReference methodReference, MethodDefinition methodDefinition)
		{
			writer.WriteLine("Il2CppDebugMethodInfo {0} = ", new object[]
			{
				DebuggerSupport.Naming.ForDebugMethodInfo(methodReference)
			});
			writer.WriteArrayInitializer(new object[]
			{
				this.DocumentAddressFor(methodDefinition),
				DebuggerSupport.Naming.ForDebugMethodInfoOffsetTable(methodReference),
				methodDefinition.Body.CodeSize,
				DebuggerSupport.Naming.ForDebugLocalInfo(methodReference),
				sequencePoints.Count,
				0,
				(sequencePoints.Count != 0) ? variableName : "0"
			});
		}

		private static void WriteSequencePointsTableIfNeeded(CppCodeWriter writer, string variableName, MethodDefinition methodDefinition, ICollection<SequencePointInfo> sequencePoints)
		{
			methodDefinition.Accept(new SequencePointsMappingVisitor(delegate(Instruction i, SequencePoint s)
			{
				SequencePointInfo sequencePointInfo = new SequencePointInfo
				{
					Instruction = i,
					SequencePoint = s
				};
				SequencePointInfo sequencePointInfo2 = sequencePoints.LastOrDefault<SequencePointInfo>();
				if (sequencePointInfo2 != null)
				{
					sequencePointInfo2.NextSequencePoint = sequencePointInfo;
				}
				sequencePoints.Add(sequencePointInfo);
			}));
			if (sequencePoints.Count != 0)
			{
				writer.WriteLine("static SequencePointRecord {0}[] = ", new object[]
				{
					variableName
				});
				writer.WriteArrayInitializer((from info in sequencePoints
				select string.Format("{{ {0}, {1}, NULL }}", info.Instruction.Offset, (info.NextSequencePoint != null) ? (info.NextSequencePoint.Instruction.Offset - 1) : methodDefinition.Body.CodeSize)).ToArray<string>());
			}
		}

		private static void WriteDebugLocalInfos(CppCodeWriter writer, MethodReference methodReference, MethodDefinition methodDefinition)
		{
			foreach (VariableDefinition current in methodDefinition.Body.Variables)
			{
				TypeResolver typeResolver = TypeResolver.For(methodReference.DeclaringType);
				TypeReference typeReference = typeResolver.ResolveVariableType(methodReference, current);
				for (TypeSpecification typeSpecification = typeReference as TypeSpecification; typeSpecification != null; typeSpecification = (typeReference as TypeSpecification))
				{
					typeReference = typeSpecification.ElementType;
				}
				writer.WriteLine("Il2CppDebugLocalsInfo {0} = ", new object[]
				{
					DebuggerSupport.Naming.ForDebugMethodLocalInfo(current, methodReference)
				});
				writer.WriteArrayInitializer(new object[]
				{
					DebuggerSupport.Naming.Null,
					DebuggerSupport.Quote(current.Name ?? current.Index.ToString()),
					0,
					0
				});
			}
			writer.WriteArrayInitializer("const Il2CppDebugLocalsInfo*", DebuggerSupport.Naming.ForDebugLocalInfo(methodReference), from v in methodDefinition.Body.Variables
			select DebuggerSupport.Naming.AddressOf(DebuggerSupport.Naming.ForDebugMethodLocalInfo(v, methodReference)), true);
		}

		private static void WriteOffsetTable(CppCodeWriter writer, MethodReference methodReference, MethodDefinition methodDefinition)
		{
			writer.WriteLine("const int32_t {0}[] = ", new object[]
			{
				DebuggerSupport.Naming.ForDebugMethodInfoOffsetTable(methodReference)
			});
			writer.BeginBlock();
			methodDefinition.Accept(new SequencePointsMappingVisitor(delegate(Instruction i, SequencePoint s)
			{
				writer.WriteLine("{0}, {1}, ", new object[]
				{
					i.Offset,
					s.StartLine
				});
			}));
			writer.WriteLine("-1, -1");
			writer.EndBlock(true);
		}

		private void WriteEmptyDebugMethodInfoFor(CppCodeWriter writer, MethodReference methodReference, MethodDefinition methodDefinition)
		{
			writer.WriteLine("Il2CppDebugMethodInfo {0} = ", new object[]
			{
				DebuggerSupport.Naming.ForDebugMethodInfo(methodReference)
			});
			writer.WriteArrayInitializer(new object[]
			{
				this.DocumentAddressFor(methodDefinition),
				0,
				0,
				0
			});
		}

		public void ExtendTypeInfoInitializer(FieldInitializers initializer, TypeReference typeReference)
		{
			initializer.Add("debug_info", "&" + DebuggerSupport.Naming.ForDebugTypeInfos(typeReference));
		}

		public void WriteDebugTypeInfosDeclarationFor(CppCodeWriter writer, TypeReference typeRe)
		{
			writer.Write("extern Il2CppDebugTypeInfo {0};", new object[]
			{
				DebuggerSupport.Naming.ForDebugTypeInfos(typeRe)
			});
		}

		public void WriteDebugTypeInfosDefinitionFor(CppCodeWriter writer, TypeReference typeReference)
		{
			TypeDefinition typeDefinition = typeReference.Resolve();
			writer.WriteLine("Il2CppDebugTypeInfo {0} = {{ {1} }};", new object[]
			{
				DebuggerSupport.Naming.ForDebugTypeInfos(typeReference),
				this.DocumentAddressFor(typeDefinition)
			});
		}

		private string DocumentAddressFor(TypeDefinition typeDefinition)
		{
			string result;
			using (IEnumerator<string> enumerator = DebuggerSupport.DocumentsFor(typeDefinition).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					result = "&" + this.UniqueDocumentVarNameFor(current);
					return result;
				}
			}
			result = "0";
			return result;
		}

		private string DocumentAddressFor(MethodDefinition methodDefinition)
		{
			string result;
			using (IEnumerator<string> enumerator = this.DocumentsFor(methodDefinition).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					result = "&" + this.UniqueDocumentVarNameFor(current);
					return result;
				}
			}
			result = "0";
			return result;
		}

		private static IEnumerable<string> DocumentsFor(TypeDefinition typeDefinition)
		{
			CollectSourceDocumentsVisitor collectSourceDocumentsVisitor = new CollectSourceDocumentsVisitor();
			typeDefinition.Accept(collectSourceDocumentsVisitor);
			return collectSourceDocumentsVisitor.Documents;
		}

		private IEnumerable<string> DocumentsFor(MethodDefinition methodDefinition)
		{
			CollectSourceDocumentsVisitor collectSourceDocumentsVisitor = new CollectSourceDocumentsVisitor();
			methodDefinition.Accept(collectSourceDocumentsVisitor);
			return collectSourceDocumentsVisitor.Documents;
		}

		public void WriteCallStackInformation(CppCodeWriter writer, MethodReference methodReference, IEnumerable<KeyValuePair<string, TypeReference>> locals, IRuntimeMetadataAccess metadataAccess)
		{
			this._sequencePointUid = 0;
			KeyValuePair<string, TypeReference>[] array = locals.ToArray<KeyValuePair<string, TypeReference>>();
			MethodDefinition methodDefinition = methodReference.Resolve();
			DebuggerSupport.WriteDummyNullThisIfNeeded(writer, methodDefinition);
			DebuggerSupport.WriteParamsAddressArray(writer, methodDefinition);
			DebuggerSupport.WriteLocalsAddressArray(writer, methodDefinition, array);
			writer.WriteLine("StackTraceSentry _stackTraceSentry(&{0}, {1}, {2}, {3}, {4}, {5});", new object[]
			{
				metadataAccess.MethodInfo(methodReference),
				DebuggerSupport.ThisAddressFor(methodDefinition),
				DebuggerSupport.ParamsAddressArrayFor(methodDefinition),
				methodDefinition.Parameters.Count,
				DebuggerSupport.LocalsAddressArrayFor(methodDefinition),
				array.Length
			});
		}

		private static void WriteDummyNullThisIfNeeded(CppCodeWriter writer, MethodDefinition methodDefinition)
		{
			if (methodDefinition.IsStaticConstructor())
			{
				writer.WriteLine("void *__dummy_this = 0;");
			}
		}

		private static string ThisAddressFor(MethodDefinition methodDefinition)
		{
			return (!methodDefinition.IsStaticConstructor()) ? "&__this" : "&__dummy_this";
		}

		public void WriteSequencePoint(CppCodeWriter writer, Instruction instruction, bool injected)
		{
			writer.WriteLine("il2cpp_debugger_sequence_point_hit({0}, {1});", new object[]
			{
				(!injected) ? this._sequencePointUid++ : -1,
				instruction.Offset
			});
		}

		public void WriteDebugIncludes(CppCodeWriter writer)
		{
			writer.AddInclude("il2cpp-debugger.h");
		}

		private static void WriteParamsAddressArray(CppCodeWriter writer, IMethodSignature methodDefinition)
		{
			if (methodDefinition.HasParameters)
			{
				writer.WriteArrayInitializer("void*", "__dbg_params", from p in methodDefinition.Parameters
				select "&" + DebuggerSupport.Naming.ForParameterName(p), true);
			}
		}

		private static string ParamsAddressArrayFor(MethodDefinition methodDefinition)
		{
			return methodDefinition.HasParameters ? "__dbg_params" : "0";
		}

		private static void WriteLocalsAddressArray(CppCodeWriter writer, MethodDefinition methodDefinition, IEnumerable<KeyValuePair<string, TypeReference>> localvars)
		{
			if (methodDefinition.HasBody && methodDefinition.Body.Variables.Count != 0)
			{
				writer.WriteArrayInitializer("void*", "__dbg_locals", from v in localvars
				select "&" + v.Key, true);
			}
		}

		private static string LocalsAddressArrayFor(MethodDefinition methodDefinition)
		{
			string result;
			if (!methodDefinition.HasBody || methodDefinition.Body.Variables.Count == 0)
			{
				result = "0";
			}
			else
			{
				result = "__dbg_locals";
			}
			return result;
		}

		private void WriteDocumentDebugInfoHeader(CppCodeWriter writer)
		{
			writer.WriteLine("#pragma once");
			writer.WriteLine();
			writer.AddInclude("il2cpp-config.h");
			writer.AddInclude("class-internals.h");
			writer.WriteLine();
			foreach (string current in this._documents)
			{
				this.WriteDebugDocumentDefinitionFor(writer, current);
			}
		}

		private void WriteDebugDocumentDefinitionFor(CodeWriter writer, string document)
		{
			writer.WriteLine("extern Il2CppDebugDocument {0}; // {1}", new object[]
			{
				this.UniqueDocumentVarNameFor(document),
				document
			});
		}

		private void WriteDocumentDebugInfoSource(CppCodeWriter writer)
		{
			writer.AddInclude("document-debug-info.h");
			writer.WriteLine();
			foreach (string current in this._documents)
			{
				this.WriteDebugDocumentDeclarationFor(writer, current);
			}
		}

		private void WriteDebugDocumentDeclarationFor(CppCodeWriter writer, string document)
		{
			writer.WriteLine("Il2CppDebugDocument {0} = ", new object[]
			{
				this.UniqueDocumentVarNameFor(document)
			});
			writer.WriteArrayInitializer(new object[]
			{
				DebuggerSupport.Quote(DebuggerSupport.StringifyPath(Path.GetDirectoryName(document))),
				DebuggerSupport.Quote(Path.GetFileName(document))
			});
		}

		private static string StringifyPath(string path)
		{
			string text = path.Replace("\\", "\\\\");
			if (!text.EndsWith("\\"))
			{
				text += "\\\\";
			}
			return text;
		}

		private static string Quote(object val)
		{
			return string.Format("\"{0}\"", val);
		}

		private string UniqueDocumentVarNameFor(string document)
		{
			string text;
			if (!this._documentToVarName.TryGetValue(document, out text))
			{
				int num = 0;
				string arg = Path.GetFileNameWithoutExtension(document).ToLower();
				do
				{
					text = string.Format("{0}_{1}_dbgdoc", arg, num++);
				}
				while (this._varNameToDocument.ContainsKey(text));
				this._documentToVarName[document] = text;
				this._varNameToDocument[text] = document;
			}
			return text;
		}

		public void WriteDebugBreak(CppCodeWriter writer)
		{
			writer.WriteLine("il2cpp_debugger_notify_user_break();");
		}
	}
}
