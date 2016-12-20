namespace Unity.UNetWeaver
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Mdb;
    using Mono.Cecil.Pdb;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal class Helpers
    {
        public static string DestinationFileFor(string outputDir, string assemblyPath)
        {
            string fileName = Path.GetFileName(assemblyPath);
            return Path.Combine(outputDir, fileName);
        }

        public static ISymbolReaderProvider GetSymbolReaderProvider(string inputFile)
        {
            string str = inputFile.Substring(0, inputFile.Length - 4);
            if (File.Exists(str + ".pdb"))
            {
                Console.WriteLine("Symbols will be read from " + str + ".pdb");
                return new PdbReaderProvider();
            }
            if (File.Exists(str + ".dll.mdb"))
            {
                Console.WriteLine("Symbols will be read from " + str + ".dll.mdb");
                return new MdbReaderProvider();
            }
            Console.WriteLine("No symbols for " + inputFile);
            return null;
        }

        public static WriterParameters GetWriterParameters(Mono.Cecil.ReaderParameters readParams)
        {
            WriterParameters parameters = new WriterParameters();
            if (readParams.SymbolReaderProvider is PdbReaderProvider)
            {
                parameters.SymbolWriterProvider = new PdbWriterProvider();
                return parameters;
            }
            if (readParams.SymbolReaderProvider is MdbReaderProvider)
            {
                parameters.SymbolWriterProvider = new MdbWriterProvider();
            }
            return parameters;
        }

        public static TypeReference MakeGenericType(TypeReference self, params TypeReference[] arguments)
        {
            if (self.GenericParameters.Count != arguments.Length)
            {
                throw new ArgumentException();
            }
            GenericInstanceType type = new GenericInstanceType(self);
            foreach (TypeReference reference in arguments)
            {
                type.GenericArguments.Add(reference);
            }
            return type;
        }

        public static MethodReference MakeHostInstanceGeneric(MethodReference self, params TypeReference[] arguments)
        {
            MethodReference owner = new MethodReference(self.Name, self.ReturnType, MakeGenericType(self.DeclaringType, arguments)) {
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention
            };
            foreach (ParameterDefinition definition in self.Parameters)
            {
                owner.Parameters.Add(new ParameterDefinition(definition.ParameterType));
            }
            foreach (GenericParameter parameter in self.GenericParameters)
            {
                owner.GenericParameters.Add(new GenericParameter(parameter.Name, owner));
            }
            return owner;
        }

        public static Mono.Cecil.ReaderParameters ReaderParameters(string assemblyPath, IEnumerable<string> extraPaths, IAssemblyResolver assemblyResolver, string unityEngineDLLPath, string unityUNetDLLPath)
        {
            Mono.Cecil.ReaderParameters parameters = new Mono.Cecil.ReaderParameters();
            if (assemblyResolver == null)
            {
                assemblyResolver = new DefaultAssemblyResolver();
            }
            AddSearchDirectoryHelper helper = new AddSearchDirectoryHelper(assemblyResolver);
            helper.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));
            helper.AddSearchDirectory(UnityEngineDLLDirectoryName());
            helper.AddSearchDirectory(Path.GetDirectoryName(unityEngineDLLPath));
            helper.AddSearchDirectory(Path.GetDirectoryName(unityUNetDLLPath));
            if (extraPaths != null)
            {
                foreach (string str in extraPaths)
                {
                    helper.AddSearchDirectory(str);
                }
            }
            parameters.AssemblyResolver = assemblyResolver;
            parameters.SymbolReaderProvider = GetSymbolReaderProvider(assemblyPath);
            return parameters;
        }

        public static string UnityEngineDLLDirectoryName()
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            return ((directoryName == null) ? null : directoryName.Replace(@"file:\", ""));
        }

        private class AddSearchDirectoryHelper
        {
            private readonly AddSearchDirectoryDelegate _addSearchDirectory;

            public AddSearchDirectoryHelper(IAssemblyResolver assemblyResolver)
            {
                System.Type[] types = new System.Type[] { typeof(string) };
                MethodInfo method = assemblyResolver.GetType().GetMethod("AddSearchDirectory", BindingFlags.Public | BindingFlags.Instance, null, types, null);
                if (method == null)
                {
                    throw new Exception("Assembly resolver doesn't implement AddSearchDirectory method.");
                }
                this._addSearchDirectory = (AddSearchDirectoryDelegate) Delegate.CreateDelegate(typeof(AddSearchDirectoryDelegate), assemblyResolver, method);
            }

            public void AddSearchDirectory(string directory)
            {
                this._addSearchDirectory(directory);
            }

            private delegate void AddSearchDirectoryDelegate(string directory);
        }
    }
}

