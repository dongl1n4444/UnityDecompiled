using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.FileNaming;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP
{
	public abstract class CppCodeWriter : CodeWriter
	{
		private readonly HashSet<string> _includes = new HashSet<string>();

		private readonly HashSet<string> _writtenExterns = new HashSet<string>();

		private readonly HashSet<string> _writtenInternalPInvokeMethodDeclarations = new HashSet<string>();

		private readonly HashSet<TypeReference> _forwardDeclarations = new HashSet<TypeReference>(new TypeReferenceEqualityComparer());

		private readonly HashSet<string> _rawForwardDeclarations = new HashSet<string>();

		private readonly HashSet<GenericInstanceMethod> _genericInstanceMethods = new HashSet<GenericInstanceMethod>(new MethodReferenceComparer());

		[Inject]
		public static INamingService Naming;

		[Inject]
		public static IStatsService StatsService;

		[Inject]
		public static IGenericSharingAnalysisService GenericSharingAnalysis;

		[CompilerGenerated]
		private static Func<TypeReference, bool> <>f__mg$cache0;

		public IEnumerable<string> Includes
		{
			get
			{
				return this._includes;
			}
		}

		public IEnumerable<string> WrittenExterns
		{
			get
			{
				return this._writtenExterns;
			}
		}

		public IEnumerable<TypeReference> ForwardDeclarations
		{
			get
			{
				return this._forwardDeclarations;
			}
		}

		public IEnumerable<string> RawForwardDeclarations
		{
			get
			{
				return this._rawForwardDeclarations;
			}
		}

		public IEnumerable<GenericInstanceMethod> GenericInstanceMethods
		{
			get
			{
				return this._genericInstanceMethods;
			}
		}

		protected CppCodeWriter(StreamWriter stream) : base(stream)
		{
		}

		public void Write(CppCodeWriter other)
		{
			foreach (TypeReference current in other.ForwardDeclarations)
			{
				this.AddForwardDeclaration(current);
			}
			foreach (string current2 in other.RawForwardDeclarations)
			{
				this.AddForwardDeclaration(current2);
			}
			foreach (string current3 in other.Includes)
			{
				this.AddRawInclude(current3);
			}
			foreach (GenericInstanceMethod current4 in other.GenericInstanceMethods)
			{
				this.AddGenericInstanceMethod(current4);
			}
			base.Writer.Flush();
			other.Writer.Flush();
			Stream baseStream = other.Writer.BaseStream;
			long position = baseStream.Position;
			baseStream.Seek(0L, SeekOrigin.Begin);
			baseStream.CopyTo(base.Writer.BaseStream);
			baseStream.Seek(position, SeekOrigin.Begin);
			base.Writer.Flush();
		}

		public void AddInclude(string path)
		{
			this._includes.Add(string.Format("\"{0}\"", path));
		}

		public void AddStdInclude(string path)
		{
			this._includes.Add(string.Format("<{0}>", path));
		}

		public void AddRawInclude(string path)
		{
			this._includes.Add(path);
		}

		public void AddForwardDeclaration(TypeReference typeReference)
		{
			if (typeReference == null)
			{
				throw new ArgumentNullException("typeReference");
			}
			this._forwardDeclarations.Add(CppCodeWriter.GetForwardDeclarationType(typeReference));
		}

		private static TypeReference GetForwardDeclarationType(TypeReference typeReference)
		{
			typeReference = CppCodeWriter.Naming.RemoveModifiers(typeReference);
			PointerType pointerType = typeReference as PointerType;
			TypeReference result;
			if (pointerType != null)
			{
				result = CppCodeWriter.GetForwardDeclarationType(pointerType.ElementType);
			}
			else
			{
				ByReferenceType byReferenceType = typeReference as ByReferenceType;
				if (byReferenceType != null)
				{
					result = CppCodeWriter.GetForwardDeclarationType(byReferenceType.ElementType);
				}
				else
				{
					result = typeReference;
				}
			}
			return result;
		}

		public void AddForwardDeclaration(string declaration)
		{
			if (string.IsNullOrEmpty(declaration))
			{
				throw new ArgumentException("Type forward declaration must not be empty.", "declaration");
			}
			this._rawForwardDeclarations.Add(declaration);
		}

		public void AddGenericInstanceMethod(GenericInstanceMethod genericInstanceMethod)
		{
			if (genericInstanceMethod == null)
			{
				throw new ArgumentNullException("genericInstanceMethod");
			}
			this._genericInstanceMethods.Add(genericInstanceMethod);
		}

		public void AddIncludesForTypeReference(TypeReference typeReference, bool requiresCompleteType = false)
		{
			TypeReference typeReference2 = typeReference;
			if (!typeReference2.ContainsGenericParameters())
			{
				ArrayType arrayType = typeReference2 as ArrayType;
				if (arrayType != null)
				{
					this.AddForwardDeclaration(arrayType);
				}
				GenericInstanceType genericInstanceType = typeReference2 as GenericInstanceType;
				if (genericInstanceType != null)
				{
					TypeReference elementType = genericInstanceType.ElementType;
					if (elementType.IsValueType())
					{
						this.AddIncludeForType(genericInstanceType);
					}
					else
					{
						this.AddForwardDeclaration(genericInstanceType);
					}
				}
				ByReferenceType byReferenceType = typeReference2 as ByReferenceType;
				if (byReferenceType != null)
				{
					typeReference2 = byReferenceType.ElementType;
				}
				PointerType pointerType = typeReference2 as PointerType;
				if (pointerType != null)
				{
					typeReference2 = pointerType.ElementType;
				}
				if (typeReference2.IsPrimitive)
				{
					if (typeReference2.MetadataType == MetadataType.IntPtr || typeReference2.MetadataType == MetadataType.UIntPtr)
					{
						this.AddIncludeForType(typeReference2);
					}
				}
				else
				{
					bool flag = typeReference2.IsValueType();
					if (flag || (requiresCompleteType && !(typeReference2 is TypeSpecification)))
					{
						this.AddIncludeForType(typeReference2);
					}
					if (!flag)
					{
						this.AddForwardDeclaration(typeReference2);
					}
				}
			}
		}

		public void AddIncludeForTypeDefinition(TypeReference typeReference)
		{
			if (typeReference.ContainsGenericParameters())
			{
				if (typeReference.IsGenericParameter)
				{
					return;
				}
				TypeDefinition typeDefinition = typeReference.Resolve();
				if (typeDefinition == null || typeDefinition.IsEnum())
				{
					return;
				}
			}
			TypeReference typeReference2 = CppCodeWriter.Naming.RemoveModifiers(typeReference);
			ByReferenceType byReferenceType = typeReference2 as ByReferenceType;
			if (byReferenceType != null)
			{
				this.AddIncludeForTypeDefinition(byReferenceType.ElementType);
			}
			else
			{
				PointerType pointerType = typeReference2 as PointerType;
				if (pointerType != null)
				{
					this.AddIncludeForTypeDefinition(pointerType.ElementType);
				}
				else
				{
					ArrayType arrayType = typeReference2 as ArrayType;
					if (arrayType != null)
					{
						this.AddIncludeForType(arrayType);
						this.AddIncludeForType(arrayType.ElementType);
					}
					else
					{
						GenericInstanceType genericInstanceType = typeReference2 as GenericInstanceType;
						if (genericInstanceType != null)
						{
							this.AddIncludeForType(genericInstanceType);
						}
						else
						{
							this.AddIncludeForType(typeReference2);
						}
					}
				}
			}
		}

		public void AddCodeGenIncludes()
		{
			this.AddInclude("class-internals.h");
			this.AddInclude("codegen/il2cpp-codegen.h");
		}

		public void AddIncludeOrExternForTypeDefinition(TypeReference type)
		{
			type = CppCodeWriter.Naming.RemoveModifiers(type);
			ByReferenceType byReferenceType = type as ByReferenceType;
			if (byReferenceType != null)
			{
				type = byReferenceType.ElementType;
			}
			PointerType pointerType = type as PointerType;
			if (pointerType != null)
			{
				type = pointerType.ElementType;
			}
			if (!type.IsValueType())
			{
				this.AddForwardDeclaration(type);
			}
			this.AddIncludeForType(type);
		}

		public void AddIncludeForMethodDeclarations(TypeReference type)
		{
			if ((!type.IsInterface() || type.IsComOrWindowsRuntimeInterface()) && !type.IsArray)
			{
				if (!type.HasGenericParameters)
				{
					this.AddInclude(FileNameProvider.Instance.ForMethodDeclarations(type));
				}
			}
		}

		public void AddIncludesForMethodDeclaration(GenericInstanceMethod method)
		{
			TypeResolver typeResolver = new TypeResolver(method.DeclaringType as GenericInstanceType, method);
			this.AddIncludeForType(method.DeclaringType);
			if (method.ReturnType.MetadataType != MetadataType.Void)
			{
				this.AddIncludesForTypeReference(typeResolver.ResolveReturnType(method), false);
			}
			foreach (ParameterDefinition current in method.Parameters)
			{
				this.AddIncludesForTypeReference(typeResolver.ResolveParameterType(method, current), false);
			}
			if (CppCodeWriter.GenericSharingAnalysis.CanShareMethod(method) && !CppCodeWriter.GenericSharingAnalysis.IsSharedMethod(method))
			{
				this.AddIncludesForMethodDeclaration((GenericInstanceMethod)CppCodeWriter.GenericSharingAnalysis.GetSharedMethod(method));
			}
			this.AddGenericInstanceMethod(method);
		}

		private void AddIncludeForType(TypeReference type)
		{
			type = CppCodeWriter.Naming.RemoveModifiers(type);
			if (!type.HasGenericParameters)
			{
				if (!type.IsInterface() || type.IsComOrWindowsRuntimeInterface())
				{
					if (type.IsArray)
					{
						this.AddInclude(FileNameProvider.Instance.ForModule(ArrayUtilities.ModuleDefinitionForElementTypeOf((ArrayType)type)) + "_ArrayTypes.h");
					}
					else
					{
						this.AddInclude(FileNameProvider.Instance.ForTypeDefinition(type));
					}
				}
			}
		}

		public void WriteClangWarningDisables()
		{
			base.WriteLine("#ifdef __clang__");
			base.WriteLine("#pragma clang diagnostic push");
			base.WriteLine("#pragma clang diagnostic ignored \"-Winvalid-offsetof\"");
			base.WriteLine("#pragma clang diagnostic ignored \"-Wunused-variable\"");
			base.WriteLine("#endif");
		}

		public void WriteClangWarningEnables()
		{
			base.WriteLine("#ifdef __clang__");
			base.WriteLine("#pragma clang diagnostic pop");
			base.WriteLine("#endif");
		}

		internal void WriteExternForIl2CppType(TypeReference type)
		{
			string text = CppCodeWriter.Naming.ForIl2CppType(type, 0);
			if (this._writtenExterns.Add(text))
			{
				base.WriteLine(string.Format("extern const Il2CppType {0};", text));
			}
		}

		internal void WriteExternForIl2CppGenericInst(IList<TypeReference> types)
		{
			string text = CppCodeWriter.Naming.ForGenericInst(types);
			if (this._writtenExterns.Add(text))
			{
				base.WriteLine(string.Format("extern const Il2CppGenericInst {0};", text));
			}
		}

		internal void WriteExternForGenericClass(TypeReference type)
		{
			string text = CppCodeWriter.Naming.ForGenericClass(type);
			if (this._writtenExterns.Add(text))
			{
				base.WriteLine(string.Format("extern Il2CppGenericClass {0};", text));
			}
		}

		public static string InitializerStringFor(TypeReference type)
		{
			string result;
			if (type.FullName == "intptr_t" || type.FullName == "uintptr_t" || type.IsEnum())
			{
				result = " = 0";
			}
			else if (type.IsPrimitive)
			{
				string text = CppCodeWriter.InitializerStringForPrimitiveType(type);
				if (text != null)
				{
					result = string.Format(" = {0}", text);
				}
				else
				{
					result = string.Empty;
				}
			}
			else if (!type.IsValueType())
			{
				result = string.Format(" = {0}", CppCodeWriter.Naming.Null);
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		private static bool IsZeroSizeValueType(TypeReference type)
		{
			bool result;
			if (!type.IsValueType())
			{
				result = false;
			}
			else if (type.FullName == "intptr_t" || type.FullName == "uintptr_t")
			{
				result = false;
			}
			else if (type.IsEnum())
			{
				result = false;
			}
			else if (type.IsPrimitive)
			{
				result = false;
			}
			else
			{
				TypeDefinition typeDefinition = type.Resolve();
				if (typeDefinition.Fields.All((FieldDefinition f) => f.IsStatic))
				{
					result = true;
				}
				else
				{
					IEnumerable<TypeReference> arg_10A_0 = from f in typeDefinition.Fields
					where !f.IsStatic
					select f.FieldType;
					if (CppCodeWriter.<>f__mg$cache0 == null)
					{
						CppCodeWriter.<>f__mg$cache0 = new Func<TypeReference, bool>(CppCodeWriter.IsZeroSizeValueType);
					}
					result = arg_10A_0.All(CppCodeWriter.<>f__mg$cache0);
				}
			}
			return result;
		}

		public static string InitializerStringForPrimitiveType(TypeReference type)
		{
			return CppCodeWriter.InitializerStringForPrimitiveType(type.MetadataType);
		}

		public static string InitializerStringForPrimitiveType(MetadataType type)
		{
			string result;
			switch (type)
			{
			case MetadataType.Boolean:
				result = "false";
				break;
			case MetadataType.Char:
			case MetadataType.SByte:
			case MetadataType.Byte:
				result = "0x0";
				break;
			case MetadataType.Int16:
			case MetadataType.UInt16:
			case MetadataType.Int32:
			case MetadataType.UInt32:
			case MetadataType.Int64:
			case MetadataType.UInt64:
				result = "0";
				break;
			case MetadataType.Single:
				result = "0.0f";
				break;
			case MetadataType.Double:
				result = "0.0";
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		public static string InitializerStringForPrimitiveCppType(string typeName)
		{
			string result;
			switch (typeName)
			{
			case "bool":
				result = CppCodeWriter.InitializerStringForPrimitiveType(MetadataType.Boolean);
				return result;
			case "char":
			case "wchar_t":
				result = CppCodeWriter.InitializerStringForPrimitiveType(MetadataType.Char);
				return result;
			case "size_t":
			case "int8_t":
			case "int16_t":
			case "int32_t":
			case "int64_t":
			case "uint8_t":
			case "uint16_t":
			case "uint32_t":
			case "uint64_t":
				result = CppCodeWriter.InitializerStringForPrimitiveType(MetadataType.Int32);
				return result;
			case "double":
				result = CppCodeWriter.InitializerStringForPrimitiveType(MetadataType.Double);
				return result;
			case "float":
				result = CppCodeWriter.InitializerStringForPrimitiveType(MetadataType.Single);
				return result;
			}
			result = null;
			return result;
		}

		public void WriteVariable(TypeReference type, string name)
		{
			if (type.IsGenericParameter())
			{
				throw new ArgumentException("Generic parameter encountered as variable type", "type");
			}
			string text = CppCodeWriter.InitializerStringFor(type);
			string text2 = CppCodeWriter.Naming.ForVariable(type);
			if (!string.IsNullOrEmpty(text))
			{
				base.WriteLine("{0} {1}{2};", new object[]
				{
					text2,
					name,
					text
				});
			}
			else
			{
				base.WriteLine("{0} {1};", new object[]
				{
					text2,
					name
				});
				base.WriteLine("memset(&{0}, 0, sizeof({0}));", new object[]
				{
					name
				});
			}
		}

		public void WriteDefaultReturn(TypeReference type)
		{
			if (type.MetadataType == MetadataType.Void)
			{
				base.WriteLine("return;");
			}
			else
			{
				this.WriteVariable(type, "ret");
				base.WriteLine("return ret;");
			}
		}

		public void WriteInternalCallResolutionStatement(MethodDefinition method)
		{
			string text = method.FullName.Substring(method.FullName.IndexOf(" ") + 1);
			base.WriteLine("typedef {0};", new object[]
			{
				MethodSignatureWriter.GetICallMethodVariable(method)
			});
			base.WriteLine("static {0}_ftn _il2cpp_icall_func;", new object[]
			{
				CppCodeWriter.Naming.ForMethodNameOnly(method)
			});
			base.WriteLine("if (!_il2cpp_icall_func)", new object[]
			{
				CppCodeWriter.Naming.ForMethodNameOnly(method)
			});
			base.WriteLine("_il2cpp_icall_func = ({0}_ftn)il2cpp_codegen_resolve_icall (\"{1}\");", new object[]
			{
				CppCodeWriter.Naming.ForMethodNameOnly(method),
				text
			});
		}

		public void WriteArrayInitializer(params object[] values)
		{
			this.WriteFieldInitializer(from v in values
			select v.ToString());
		}

		public void WriteNullTerminatedArrayInitializer(params object[] values)
		{
			this.WriteFieldInitializer((from v in values
			select v.ToString()).Concat(new string[]
			{
				"NULL"
			}));
		}

		public TableInfo WriteArrayInitializer(string type, string variableName, IEnumerable<string> values, bool nullTerminate = true)
		{
			values = ((!nullTerminate) ? values : values.Concat(new string[]
			{
				"NULL"
			}));
			string[] array = values.ToArray<string>();
			base.WriteLine("{0} {1}[{2}] = ", new object[]
			{
				type,
				variableName,
				array.Length
			});
			this.WriteFieldInitializer(array);
			return new TableInfo(array.Length, type, variableName);
		}

		public void WriteStructInitializer(string type, string variableName, IEnumerable<string> values)
		{
			base.WriteLine("{0} {1} = ", new object[]
			{
				type,
				variableName
			});
			this.WriteFieldInitializer(values);
		}

		protected void WriteFieldInitializer(IEnumerable<string> initializers)
		{
			base.BeginBlock();
			foreach (string current in initializers)
			{
				base.WriteLine("{0},", new object[]
				{
					current
				});
			}
			base.EndBlock(true);
		}

		public void WriteWriteBarrierIfNeeded(TypeReference valueType, string addressExpression, string valueExpression)
		{
			if (!valueType.IsValueType() && !valueType.IsPointer)
			{
				base.WriteLine("Il2CppCodeGenWriteBarrier({0}, {1});", new object[]
				{
					addressExpression,
					valueExpression
				});
			}
		}

		public T WriteIfNotEmpty<T>(Action<CppCodeWriter> writePrefixIfNotEmpty, Func<CppCodeWriter, T> writeContent, Action<CppCodeWriter> writePostfixIfNotEmpty)
		{
			T result;
			using (InMemoryCodeWriter inMemoryCodeWriter = new InMemoryCodeWriter())
			{
				using (InMemoryCodeWriter inMemoryCodeWriter2 = new InMemoryCodeWriter())
				{
					inMemoryCodeWriter.Indent(base.IndentationLevel);
					writePrefixIfNotEmpty(inMemoryCodeWriter);
					inMemoryCodeWriter2.Indent(inMemoryCodeWriter.IndentationLevel);
					T t = writeContent(inMemoryCodeWriter2);
					inMemoryCodeWriter2.Dedent(inMemoryCodeWriter.IndentationLevel);
					inMemoryCodeWriter.Dedent(base.IndentationLevel);
					inMemoryCodeWriter2.Writer.Flush();
					if (inMemoryCodeWriter2.Writer.BaseStream.Length > 0L)
					{
						inMemoryCodeWriter.Writer.Flush();
						this.Write(inMemoryCodeWriter);
						this.Write(inMemoryCodeWriter2);
						int num = inMemoryCodeWriter.IndentationLevel + inMemoryCodeWriter2.IndentationLevel;
						if (num > 0)
						{
							base.Indent(num);
						}
						else if (num < 0)
						{
							base.Dedent(-num);
						}
						if (writePostfixIfNotEmpty != null)
						{
							writePostfixIfNotEmpty(this);
						}
					}
					result = t;
				}
			}
			return result;
		}

		public void WriteIfNotEmpty(Action<CppCodeWriter> writePrefixIfNotEmpty, Action<CppCodeWriter> writeContent, Action<CppCodeWriter> writePostfixIfNotEmpty)
		{
			this.WriteIfNotEmpty<object>(writePrefixIfNotEmpty, delegate(CppCodeWriter bodyWriter)
			{
				writeContent(bodyWriter);
				return null;
			}, writePostfixIfNotEmpty);
		}

		public void WriteInternalPInvokeDeclaration(string methodName, string internalPInvokeDeclaration)
		{
			if (!this._writtenInternalPInvokeMethodDeclarations.Contains(methodName))
			{
				base.WriteLine(internalPInvokeDeclaration);
				this._writtenInternalPInvokeMethodDeclarations.Add(methodName);
			}
		}
	}
}
