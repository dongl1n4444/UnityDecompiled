using Mono.Cecil;
using Mono.Collections.Generic;
using NiceIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP
{
	internal class DriverWriter
	{
		private static AssemblyDefinition _executable;

		private static MethodDefinition _entryPoint;

		[Inject]
		public static INamingService Naming;

		[Inject]
		public static ITypeProviderService TypeProvider;

		public DriverWriter(AssemblyDefinition executable)
		{
			DriverWriter._executable = executable;
			DriverWriter._entryPoint = DriverWriter._executable.EntryPoint;
		}

		public void Write(NPath outputDir)
		{
			using (SourceCodeWriter sourceCodeWriter = new SourceCodeWriter(outputDir.Combine(new string[]
			{
				"driver.cpp"
			})))
			{
				this.WriteIncludes(sourceCodeWriter);
				this.WriteMainInvoker(sourceCodeWriter);
				this.WriteEntryPoint(sourceCodeWriter);
				this.WritePlatformSpecificEntryPoints(sourceCodeWriter);
			}
		}

		private void WriteIncludes(CppCodeWriter writer)
		{
			writer.AddCodeGenIncludes();
			writer.WriteLine("#if IL2CPP_TARGET_WINDOWS_DESKTOP");
			writer.WriteLine("#include \"Windows.h\"");
			writer.WriteLine("#endif");
			writer.WriteLine();
		}

		private void WriteMainInvoker(CppCodeWriter writer)
		{
			MethodWriter.WriteMethodWithMetadataInitialization(writer, "int MainInvoker(int argc, const Il2CppNativeChar* const* argv)", "MainInvoker", delegate(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
			{
				this.WriteMainInvokerBody(bodyWriter, MethodWriter.GetDefaultRuntimeMetadataAccess(null, metadataUsage, methodUsage));
			}, "MainInvoker");
			writer.WriteLine();
		}

		private void WriteMainInvokerBody(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			writer.WriteLine("try");
			using (new BlockWriter(writer, false))
			{
				this.WriteMainInvocation(writer, metadataAccess);
			}
			writer.WriteLine("catch (const Il2CppExceptionWrapper& e)");
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine("il2cpp_codegen_write_to_stderr(\"Unhandled Exception: \");");
				writer.WriteLine("il2cpp_codegen_write_to_stderr(il2cpp_codegen_format_exception(e.ex).c_str());");
				writer.WriteLine("il2cpp_codegen_abort();");
			}
		}

		private void WriteMainInvocation(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
		{
			if (this.ValidateMainMethod(writer))
			{
				writer.AddIncludeForMethodDeclarations(DriverWriter._entryPoint.DeclaringType);
				List<string> list = new List<string>();
				list.Add(DriverWriter.Naming.Null);
				if (DriverWriter._entryPoint.Parameters.Count > 0)
				{
					ArrayType arrayType = (ArrayType)DriverWriter._entryPoint.Parameters[0].ParameterType;
					writer.AddIncludeForTypeDefinition(arrayType);
					writer.WriteLine("{0} args = {1};", new object[]
					{
						DriverWriter.Naming.ForVariable(arrayType),
						Emit.NewSZArray(arrayType, "argc - 1", metadataAccess)
					});
					writer.WriteLine();
					writer.WriteLine("for (int i = 1; i < argc; i++)");
					using (new BlockWriter(writer, false))
					{
						writer.WriteLine("DECLARE_NATIVE_C_STRING_AS_STRING_VIEW_OF_IL2CPP_CHARS(argumentUtf16, argv[i]);");
						writer.WriteLine("{0} argument = il2cpp_codegen_string_new_utf16(argumentUtf16);", new object[]
						{
							DriverWriter.Naming.ForVariable(DriverWriter.TypeProvider.SystemString)
						});
						writer.WriteStatement(Emit.StoreArrayElement("args", "i - 1", "argument", false));
					}
					writer.WriteLine();
					list.Add("args");
				}
				if (MethodSignatureWriter.NeedsHiddenMethodInfo(DriverWriter._entryPoint, MethodCallType.Normal, false))
				{
					list.Add(metadataAccess.HiddenMethodInfo(DriverWriter._entryPoint));
				}
				if ((DriverWriter._entryPoint.DeclaringType.Attributes & TypeAttributes.BeforeFieldInit) == TypeAttributes.NotPublic)
				{
					writer.WriteLine("IL2CPP_RUNTIME_CLASS_INIT({0});", new object[]
					{
						metadataAccess.TypeInfoFor(DriverWriter._entryPoint.DeclaringType)
					});
				}
				TypeResolver typeResolverForMethodToCall = TypeResolver.For(DriverWriter._entryPoint.DeclaringType, DriverWriter._entryPoint);
				string methodCallExpression = MethodBodyWriter.GetMethodCallExpression(null, DriverWriter._entryPoint, DriverWriter._entryPoint, typeResolverForMethodToCall, MethodCallType.Normal, metadataAccess, new VTableBuilder(), list, false, null);
				MetadataType metadataType = DriverWriter._entryPoint.ReturnType.MetadataType;
				if (metadataType != MetadataType.Void)
				{
					if (metadataType != MetadataType.Int32)
					{
						if (metadataType == MetadataType.UInt32)
						{
							writer.WriteLine("uint32_t exitCode = {0};", new object[]
							{
								methodCallExpression
							});
							writer.WriteLine("return static_cast<int>(exitCode);");
						}
					}
					else
					{
						writer.WriteLine("return {0};", new object[]
						{
							methodCallExpression
						});
					}
				}
				else
				{
					writer.WriteStatement(methodCallExpression);
					writer.WriteLine("return 0;");
				}
			}
		}

		private bool ValidateMainMethod(CppCodeWriter writer)
		{
			bool result;
			if (DriverWriter._entryPoint == null)
			{
				string text = string.Format("Entry point not found in assembly '{0}'.", DriverWriter._executable.FullName);
				writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_missing_method_exception(\"{0}\")", new object[]
				{
					text
				}));
				result = false;
			}
			else if (DriverWriter._entryPoint.HasThis)
			{
				string text2 = "Entry point must be static.";
				writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_missing_method_exception(\"{0}\")", new object[]
				{
					text2
				}));
				result = false;
			}
			else
			{
				TypeReference returnType = DriverWriter._entryPoint.ReturnType;
				if (returnType.MetadataType != MetadataType.Void && returnType.MetadataType != MetadataType.Int32 && returnType.MetadataType != MetadataType.UInt32)
				{
					string text3 = "Entry point must have a return type of void, integer, or unsigned integer.";
					writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_missing_method_exception(\"{0}\")", new object[]
					{
						text3
					}));
					result = false;
				}
				else
				{
					Collection<ParameterDefinition> parameters = DriverWriter._entryPoint.Parameters;
					bool flag = parameters.Count < 2 && !DriverWriter._entryPoint.HasGenericParameters;
					if (flag && parameters.Count == 1)
					{
						ParameterDefinition parameterDefinition = parameters[0];
						ArrayType arrayType = parameterDefinition.ParameterType as ArrayType;
						if (arrayType == null || !arrayType.IsVector)
						{
							flag = false;
						}
						else
						{
							TypeReference elementType = arrayType.ElementType;
							if (elementType.MetadataType != MetadataType.String)
							{
								flag = false;
							}
						}
					}
					if (!flag)
					{
						string text4 = string.Format("Entry point method for type '{0}' has invalid signature.", DriverWriter._entryPoint.DeclaringType.FullName);
						writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_missing_method_exception(\"{0}\")", new object[]
						{
							text4
						}));
						result = false;
					}
					else if (DriverWriter._entryPoint.DeclaringType.HasGenericParameters)
					{
						string text5 = string.Format("Entry point method is defined on a generic type '{0}'.", DriverWriter._entryPoint.DeclaringType.FullName);
						writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_missing_method_exception(\"{0}\")", new object[]
						{
							text5
						}));
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
			return result;
		}

		private void WriteEntryPoint(CppCodeWriter writer)
		{
			writer.WriteLine("int EntryPoint(int argc, const Il2CppNativeChar* const* argv)");
			using (new BlockWriter(writer, false))
			{
				DriverWriter.WriteWindowsMessageBoxHook(writer);
				this.WriteSetCommandLineArgumentsAndInitIl2Cpp(writer);
				this.WriteSetConfiguration(writer);
				writer.WriteLine();
				writer.WriteLine("int exitCode = MainInvoker(argc, argv);");
				writer.WriteLine();
				writer.WriteLine("il2cpp_shutdown();");
				writer.WriteLine("return exitCode;");
			}
			writer.WriteLine();
		}

		private void WriteSetConfiguration(CppCodeWriter writer)
		{
			writer.WriteLine();
			writer.WriteLine("#if IL2CPP_TARGET_WINDOWS");
			writer.WriteLine("il2cpp_set_config_utf16(argv[0]);");
			writer.WriteLine("#else");
			writer.WriteLine("il2cpp_set_config(argv[0]);");
			writer.WriteLine("#endif");
		}

		private void WriteSetCommandLineArgumentsAndInitIl2Cpp(CppCodeWriter writer)
		{
			writer.Dedent(1);
			writer.WriteLine("#if IL2CPP_TARGET_WINDOWS");
			writer.Indent(1);
			writer.WriteLine("il2cpp_set_commandline_arguments_utf16(argc, argv, NULL);");
			writer.WriteLine("il2cpp_init_utf16(argv[0]);");
			writer.Dedent(1);
			writer.WriteLine("#else");
			writer.Indent(1);
			writer.WriteLine("il2cpp_set_commandline_arguments(argc, argv, NULL);");
			writer.WriteLine("il2cpp_init(argv[0]);");
			writer.Dedent(1);
			writer.WriteLine("#endif");
			writer.Indent(1);
		}

		private static void WriteWindowsMessageBoxHook(CppCodeWriter writer)
		{
			if (Debugger.IsAttached)
			{
				writer.WriteLine("#if IL2CPP_TARGET_WINDOWS_DESKTOP");
				writer.WriteLine("MessageBoxW(NULL, L\"Attach\", L\"Attach\", MB_OK);");
				writer.WriteLine("#endif");
				writer.WriteLine();
			}
		}

		private void WritePlatformSpecificEntryPoints(CppCodeWriter writer)
		{
			writer.WriteLine("#if IL2CPP_TARGET_WINDOWS");
			writer.WriteLine();
			writer.WriteLine("#if !IL2CPP_TARGET_WINDOWS_DESKTOP");
			writer.WriteLine("#include \"ActivateApp.h\"");
			writer.WriteLine("#endif");
			writer.WriteLine();
			writer.WriteLine("int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nShowCmd)");
			using (new BlockWriter(writer, false))
			{
				writer.Dedent(1);
				writer.WriteLine("#if IL2CPP_TARGET_WINDOWS_DESKTOP");
				writer.Indent(1);
				writer.WriteLine("int argc;");
				writer.WriteLine("wchar_t** argv = CommandLineToArgvW(GetCommandLineW(), &argc);");
				writer.WriteLine("int returnValue = EntryPoint(argc, argv);");
				writer.WriteLine("LocalFree(argv);");
				writer.WriteLine("return returnValue;");
				writer.Dedent(1);
				writer.WriteLine("#else");
				writer.Indent(1);
				writer.WriteLine("return WinRT::Activate(EntryPoint);");
				writer.Dedent(1);
				writer.WriteLine("#endif");
				writer.Indent(1);
			}
			writer.WriteLine();
			writer.WriteLine("#else");
			writer.WriteLine();
			writer.WriteLine("int main(int argc, const char* argv[])");
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine("return EntryPoint(argc, argv);");
			}
			writer.WriteLine();
			writer.WriteLine("#endif");
		}
	}
}
