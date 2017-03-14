namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using NiceIO;
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.Portability;

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
            if (!base.ErrorOccurred)
            {
                using (Unity.IL2CPP.Portability.StreamWriter writer = new Unity.IL2CPP.Portability.StreamWriter(this._filename.ToString(), Encoding.UTF8))
                {
                    if (this.IsHeaderFile())
                    {
                        writer.WriteLine("#pragma once\n");
                    }
                    this.WriteCollectedIncludes(writer);
                    base.Writer.Flush();
                    base.Writer.BaseStream.Seek(0L, SeekOrigin.Begin);
                    base.Writer.BaseStream.CopyTo(writer.BaseStream);
                    writer.Flush();
                }
            }
            base.Dispose();
        }

        private bool IsHeaderFile()
        {
            string[] extensions = new string[] { "h", "hh", "hpp" };
            return this._filename.HasExtension(extensions);
        }

        private void WriteCollectedIncludes(System.IO.StreamWriter writer)
        {
            <WriteCollectedIncludes>c__AnonStorey0 storey = new <WriteCollectedIncludes>c__AnonStorey0();
            storey.includesToSkip = new string[] { "\"il2cpp-config.h\"", "<alloca.h>", "<malloc.h>" };
            writer.WriteLine("#include \"il2cpp-config.h\"\n");
            writer.WriteLine("#ifndef _MSC_VER");
            writer.WriteLine("# include <alloca.h>");
            writer.WriteLine("#else");
            writer.WriteLine("# include <malloc.h>");
            writer.WriteLine("#endif");
            writer.WriteLine();
            foreach (string str in base.Includes.Where<string>(new Func<string, bool>(storey.<>m__0)))
            {
                writer.WriteLine("#include {0}", str);
            }
            writer.WriteLine();
            foreach (string str2 in base.Includes.Where<string>(new Func<string, bool>(storey.<>m__1)))
            {
                writer.WriteLine("#include {0}", str2);
            }
            writer.WriteLine();
            foreach (TypeReference reference in base.ForwardDeclarations)
            {
                if (CodeGenOptions.EmitComments)
                {
                    writer.WriteLine("// {0}", reference.FullName);
                }
                writer.WriteLine("struct {0};", CppCodeWriter.Naming.ForType(reference));
            }
            foreach (string str3 in base.RawTypeForwardDeclarations)
            {
                writer.WriteLine(str3 + ';');
            }
            writer.WriteLine();
            foreach (ArrayType type in base.ArrayTypes)
            {
                TypeDefinitionWriter.WriteArrayTypeDefinition(type, new CodeWriter(writer));
            }
            writer.WriteLine();
            foreach (string str4 in base.RawMethodForwardDeclarations)
            {
                writer.WriteLine(str4 + ';');
            }
            writer.WriteLine();
            foreach (MethodReference reference2 in base.SharedMethods)
            {
                WriteSharedMethodDeclaration(writer, reference2);
            }
            writer.WriteLine();
            foreach (MethodReference reference3 in base.Methods)
            {
                WriteMethodDeclaration(writer, reference3);
            }
            writer.Flush();
        }

        private static void WriteMethodDeclaration(System.IO.StreamWriter writer, MethodReference method)
        {
            GenericInstanceType declaringType = method.DeclaringType as GenericInstanceType;
            if ((declaringType != null) && GenericsUtilities.CheckForMaximumRecursion(declaringType))
            {
                Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
                writer.WriteLine("{0};", MethodSignatureWriter.GetMethodSignature(CppCodeWriter.Naming.ForMethodNameOnly(method), CppCodeWriter.Naming.ForVariable(resolver.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveReturnTypeIfNeeded(method))), MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithTypeAndName, false, true, false), "extern \"C\"", string.Empty));
            }
            else
            {
                if (CodeGenOptions.EmitComments)
                {
                    writer.WriteLine("// {0}", method.FullName);
                }
                if (CppCodeWriter.GenericSharingAnalysis.CanShareMethod(method))
                {
                    object[] arg = new object[] { CppCodeWriter.Naming.ForMethodNameOnly(method), MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithName, false, true, false), MethodSignatureWriter.GetMethodPointer(method), CppCodeWriter.Naming.ForMethod(CppCodeWriter.GenericSharingAnalysis.GetSharedMethod(method)) + "_gshared", MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithName, false, true, false) };
                    writer.WriteLine("#define {0}({1}) (({2}){3})({4})", arg);
                }
                else
                {
                    writer.WriteLine(MethodSignatureWriter.GetMethodSignatureRaw(method) + " IL2CPP_METHOD_ATTR;");
                }
            }
        }

        private static void WriteSharedMethodDeclaration(System.IO.StreamWriter writer, MethodReference method)
        {
            GenericInstanceType declaringType = method.DeclaringType as GenericInstanceType;
            if ((declaringType != null) && GenericsUtilities.CheckForMaximumRecursion(declaringType))
            {
                Unity.IL2CPP.ILPreProcessor.TypeResolver resolver = new Unity.IL2CPP.ILPreProcessor.TypeResolver(method.DeclaringType as GenericInstanceType, method as GenericInstanceMethod);
                writer.WriteLine("{0};", MethodSignatureWriter.GetMethodSignature(CppCodeWriter.Naming.ForMethodNameOnly(method), CppCodeWriter.Naming.ForVariable(resolver.Resolve(Unity.IL2CPP.GenericParameterResolver.ResolveReturnTypeIfNeeded(method))), MethodSignatureWriter.FormatParameters(method, ParameterFormat.WithTypeAndName, false, true, false), "extern \"C\"", string.Empty));
            }
            else
            {
                if (!CppCodeWriter.GenericSharingAnalysis.IsSharedMethod(method))
                {
                    throw new InvalidOperationException();
                }
                if (CodeGenOptions.EmitComments)
                {
                    writer.WriteLine("// {0}", method.FullName);
                }
                writer.WriteLine("{0};", MethodSignatureWriter.GetSharedMethodSignatureRaw(method));
            }
        }

        [CompilerGenerated]
        private sealed class <WriteCollectedIncludes>c__AnonStorey0
        {
            internal string[] includesToSkip;

            internal bool <>m__0(string i) => 
                (!this.includesToSkip.Contains<string>(i) && i.StartsWith("<"));

            internal bool <>m__1(string i) => 
                (!this.includesToSkip.Contains<string>(i) && !i.StartsWith("<"));
        }
    }
}

