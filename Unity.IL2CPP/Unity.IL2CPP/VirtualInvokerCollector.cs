using Mono.Cecil;
using NiceIO;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cecil.Visitor;

namespace Unity.IL2CPP
{
	internal class VirtualInvokerCollector
	{
		private class InvokerVisitor : Visitor
		{
			private readonly HashSet<InvokerData> _collection;

			private readonly bool _processGenerics;

			public InvokerVisitor(HashSet<InvokerData> collection, bool processGenerics)
			{
				this._collection = collection;
				this._processGenerics = processGenerics;
			}

			protected override void Visit(MethodDefinition methodDefinition, Context context)
			{
				if (methodDefinition.IsVirtual && this._processGenerics == methodDefinition.HasGenericParameters)
				{
					this._collection.Add(new InvokerData(methodDefinition.ReturnType.MetadataType == MetadataType.Void, methodDefinition.Parameters.Count, false));
				}
			}
		}

		private readonly bool _processGenerics;

		private readonly HashSet<InvokerData> _invokerData = new HashSet<InvokerData>();

		public VirtualInvokerCollector(bool processGenerics = false)
		{
			this._processGenerics = processGenerics;
		}

		public void Process(AssemblyDefinition assembly)
		{
			VirtualInvokerCollector.InvokerVisitor visitor = new VirtualInvokerCollector.InvokerVisitor(this._invokerData, this._processGenerics);
			assembly.Accept(visitor);
		}

		public void Write(NPath path)
		{
			using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(path))
			{
				sourceCodeWriter.WriteLine("#pragma once");
				foreach (InvokerData current in from m in this._invokerData
				orderby m.ParameterCount * 10 + ((!m.VoidReturn) ? 1 : 0)
				select m)
				{
					string text = (!current.VoidReturn) ? "Func" : "Action";
					string text2 = VirtualInvokerCollector.TemplateParametersFor(current);
					if (!string.IsNullOrEmpty(text2))
					{
						sourceCodeWriter.WriteLine("template <{0}>", new object[]
						{
							text2
						});
					}
					if (this._processGenerics)
					{
						sourceCodeWriter.WriteLine("struct GenericVirt{0}Invoker{1}", new object[]
						{
							text,
							current.ParameterCount
						});
					}
					else
					{
						sourceCodeWriter.WriteLine("struct Virt{0}Invoker{1}", new object[]
						{
							text,
							current.ParameterCount
						});
					}
					sourceCodeWriter.BeginBlock();
					sourceCodeWriter.WriteLine("typedef {0} (*{1})({2});", new object[]
					{
						VirtualInvokerCollector.ReturnTypeFor(current),
						text,
						VirtualInvokerCollector.FunctionPointerParametersFor(current)
					});
					sourceCodeWriter.WriteLine();
					sourceCodeWriter.WriteLine("static inline {0} Invoke ({1})", new object[]
					{
						VirtualInvokerCollector.ReturnTypeFor(current),
						VirtualInvokerCollector.InvokeParametersFor(current, this._processGenerics)
					});
					sourceCodeWriter.BeginBlock();
					sourceCodeWriter.WriteLine("VirtualInvokeData invokeData;");
					if (this._processGenerics)
					{
						sourceCodeWriter.WriteLine("il2cpp::vm::Runtime::GetGenericVirtualInvokeData(method, obj, &invokeData);");
					}
					else
					{
						sourceCodeWriter.WriteLine("il2cpp::vm::Runtime::GetVirtualInvokeData (slot, obj, &invokeData);");
					}
					sourceCodeWriter.WriteLine("{0}(({1})invokeData.methodPtr)({2});", new object[]
					{
						(!current.VoidReturn) ? "return " : "",
						text,
						VirtualInvokerCollector.CallParametersFor(current)
					});
					sourceCodeWriter.EndBlock(false);
					sourceCodeWriter.EndBlock(true);
				}
			}
		}

		private static string ReturnTypeFor(InvokerData data)
		{
			return (!data.VoidReturn) ? "R" : "void";
		}

		private static string TemplateParametersFor(InvokerData data)
		{
			string result;
			if (data.VoidReturn && data.ParameterCount == 0)
			{
				result = string.Empty;
			}
			else
			{
				IEnumerable<string> arg_77_0;
				if (data.VoidReturn)
				{
					arg_77_0 = Enumerable.Empty<string>();
				}
				else
				{
					(arg_77_0 = new string[1])[0] = "R";
				}
				result = (from t in arg_77_0.Concat(from i in Enumerable.Range(1, data.ParameterCount)
				select "T" + i)
				select "typename " + t).AggregateWithComma();
			}
			return result;
		}

		private static string CallParametersFor(InvokerData data)
		{
			string[] first = new string[]
			{
				"obj"
			};
			return first.Concat(Enumerable.Range(1, data.ParameterCount).Select((int m, int i) => string.Format("p{0}", i + 1))).Concat(new string[]
			{
				"invokeData.method"
			}).AggregateWithComma();
		}

		private static string InvokeParametersFor(InvokerData data, bool isGeneric)
		{
			string[] arg_38_0;
			if (isGeneric)
			{
				string[] expr_0D = new string[2];
				expr_0D[0] = "const MethodInfo* method";
				arg_38_0 = expr_0D;
				expr_0D[1] = "void* obj";
			}
			else
			{
				string[] expr_28 = new string[2];
				expr_28[0] = "Il2CppMethodSlot slot";
				arg_38_0 = expr_28;
				expr_28[1] = "void* obj";
			}
			string[] first = arg_38_0;
			return first.Concat(Enumerable.Range(1, data.ParameterCount).Select((int m, int i) => string.Format("T{0} p{0}", i + 1))).AggregateWithComma();
		}

		private static string FunctionPointerParametersFor(InvokerData data)
		{
			string[] first = new string[]
			{
				"void*"
			};
			return first.Concat(Enumerable.Range(1, data.ParameterCount).Select((int m, int i) => string.Format("T{0}", i + 1))).Concat(new string[]
			{
				"const MethodInfo*"
			}).AggregateWithComma();
		}
	}
}
