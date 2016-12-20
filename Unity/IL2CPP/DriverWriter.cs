namespace Unity.IL2CPP
{
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

    internal class DriverWriter
    {
        private static MethodDefinition _entryPoint;
        private static AssemblyDefinition _executable;
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public DriverWriter(AssemblyDefinition executable)
        {
            _executable = executable;
            _entryPoint = _executable.EntryPoint;
        }

        private bool ValidateMainMethod(CppCodeWriter writer)
        {
            if (_entryPoint == null)
            {
                string str = string.Format("Entry point not found in assembly '{0}'.", _executable.FullName);
                object[] arguments = new object[] { str };
                writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_missing_method_exception(\"{0}\")", arguments));
                return false;
            }
            if (_entryPoint.HasThis)
            {
                string str2 = "Entry point must be static.";
                object[] objArray2 = new object[] { str2 };
                writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_missing_method_exception(\"{0}\")", objArray2));
                return false;
            }
            TypeReference returnType = _entryPoint.ReturnType;
            if (((returnType.MetadataType != MetadataType.Void) && (returnType.MetadataType != MetadataType.Int32)) && (returnType.MetadataType != MetadataType.UInt32))
            {
                string str3 = "Entry point must have a return type of void, integer, or unsigned integer.";
                object[] objArray3 = new object[] { str3 };
                writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_missing_method_exception(\"{0}\")", objArray3));
                return false;
            }
            Collection<ParameterDefinition> parameters = _entryPoint.Parameters;
            bool flag2 = (parameters.Count < 2) && !_entryPoint.HasGenericParameters;
            if (flag2 && (parameters.Count == 1))
            {
                ParameterDefinition definition = parameters[0];
                ArrayType parameterType = definition.ParameterType as ArrayType;
                if ((parameterType == null) || !parameterType.IsVector)
                {
                    flag2 = false;
                }
                else if (parameterType.ElementType.MetadataType != MetadataType.String)
                {
                    flag2 = false;
                }
            }
            if (!flag2)
            {
                string str4 = string.Format("Entry point method for type '{0}' has invalid signature.", _entryPoint.DeclaringType.FullName);
                object[] objArray4 = new object[] { str4 };
                writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_missing_method_exception(\"{0}\")", objArray4));
                return false;
            }
            if (_entryPoint.DeclaringType.HasGenericParameters)
            {
                string str5 = string.Format("Entry point method is defined on a generic type '{0}'.", _entryPoint.DeclaringType.FullName);
                object[] objArray5 = new object[] { str5 };
                writer.WriteStatement(Emit.RaiseManagedException("il2cpp_codegen_get_missing_method_exception(\"{0}\")", objArray5));
                return false;
            }
            return true;
        }

        public void Write(NPath outputDir)
        {
            string[] append = new string[] { "driver.cpp" };
            using (SourceCodeWriter writer = new SourceCodeWriter(outputDir.Combine(append)))
            {
                this.WriteIncludes(writer);
                this.WriteMainInvoker(writer);
                this.WriteEntryPoint(writer);
                this.WritePlatformSpecificEntryPoints(writer);
            }
        }

        private void WriteEntryPoint(CppCodeWriter writer)
        {
            writer.WriteLine("int EntryPoint(int argc, const Il2CppNativeChar* const* argv)");
            using (new BlockWriter(writer, false))
            {
                WriteWindowsMessageBoxHook(writer);
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

        private void WriteIncludes(CppCodeWriter writer)
        {
            writer.AddCodeGenIncludes();
            writer.WriteLine("#if IL2CPP_TARGET_WINDOWS_DESKTOP");
            writer.WriteLine("#include \"Windows.h\"");
            writer.WriteLine("#endif");
            writer.WriteLine();
        }

        private void WriteMainInvocation(CppCodeWriter writer, IRuntimeMetadataAccess metadataAccess)
        {
            if (this.ValidateMainMethod(writer))
            {
                writer.AddIncludeForMethodDeclarations(_entryPoint.DeclaringType);
                List<string> argumentArray = new List<string> {
                    Naming.Null
                };
                if (_entryPoint.Parameters.Count > 0)
                {
                    ArrayType parameterType = (ArrayType) _entryPoint.Parameters[0].ParameterType;
                    writer.AddIncludeForTypeDefinition(parameterType);
                    object[] args = new object[] { Naming.ForVariable(parameterType), Emit.NewSZArray(parameterType, "argc - 1", metadataAccess) };
                    writer.WriteLine("{0} args = {1};", args);
                    writer.WriteLine();
                    writer.WriteLine("for (int i = 1; i < argc; i++)");
                    using (new BlockWriter(writer, false))
                    {
                        writer.WriteLine("DECLARE_NATIVE_C_STRING_AS_STRING_VIEW_OF_IL2CPP_CHARS(argumentUtf16, argv[i]);");
                        object[] objArray2 = new object[] { Naming.ForVariable(TypeProvider.SystemString) };
                        writer.WriteLine("{0} argument = il2cpp_codegen_string_new_utf16(argumentUtf16);", objArray2);
                        writer.WriteStatement(Emit.StoreArrayElement("args", "i - 1", "argument", false));
                    }
                    writer.WriteLine();
                    argumentArray.Add("args");
                }
                if (MethodSignatureWriter.NeedsHiddenMethodInfo(_entryPoint, MethodCallType.Normal, false))
                {
                    argumentArray.Add(metadataAccess.HiddenMethodInfo(_entryPoint));
                }
                if ((_entryPoint.DeclaringType.Attributes & (TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit)) == TypeAttributes.AnsiClass)
                {
                    object[] objArray3 = new object[] { metadataAccess.TypeInfoFor(_entryPoint.DeclaringType) };
                    writer.WriteLine("IL2CPP_RUNTIME_CLASS_INIT({0});", objArray3);
                }
                Unity.IL2CPP.ILPreProcessor.TypeResolver typeResolverForMethodToCall = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(_entryPoint.DeclaringType, _entryPoint);
                string block = MethodBodyWriter.GetMethodCallExpression(null, _entryPoint, _entryPoint, typeResolverForMethodToCall, MethodCallType.Normal, metadataAccess, new VTableBuilder(), argumentArray, false, null);
                switch (_entryPoint.ReturnType.MetadataType)
                {
                    case MetadataType.Void:
                        writer.WriteStatement(block);
                        writer.WriteLine("return 0;");
                        break;

                    case MetadataType.Int32:
                    {
                        object[] objArray4 = new object[] { block };
                        writer.WriteLine("return {0};", objArray4);
                        break;
                    }
                    case MetadataType.UInt32:
                    {
                        object[] objArray5 = new object[] { block };
                        writer.WriteLine("uint32_t exitCode = {0};", objArray5);
                        writer.WriteLine("return static_cast<int>(exitCode);");
                        break;
                    }
                }
            }
        }

        private void WriteMainInvoker(CppCodeWriter writer)
        {
            MethodWriter.WriteMethodWithMetadataInitialization(writer, "int MainInvoker(int argc, const Il2CppNativeChar* const* argv)", "MainInvoker", new Action<CppCodeWriter, MetadataUsage, MethodUsage>(this, (IntPtr) this.<WriteMainInvoker>m__0), "MainInvoker");
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

        private void WriteSetConfiguration(CppCodeWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine("#if IL2CPP_TARGET_WINDOWS");
            writer.WriteLine("il2cpp_set_config_utf16(argv[0]);");
            writer.WriteLine("#else");
            writer.WriteLine("il2cpp_set_config(argv[0]);");
            writer.WriteLine("#endif");
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
    }
}

