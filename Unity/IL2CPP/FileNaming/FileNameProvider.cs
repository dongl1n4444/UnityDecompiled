namespace Unity.IL2CPP.FileNaming
{
    using Mono.Cecil;
    using System;
    using System.IO;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class FileNameProvider : IFileNameProvider
    {
        private static readonly IFileNameProvider _instance = new FileNameProvider();
        [Inject]
        private static UniqueShortNameGenerator _typeShortNameGenerator = null;
        [Inject]
        public static INamingService Naming;

        private string BaseFileName(TypeReference type) => 
            _typeShortNameGenerator.GetUniqueShortName(ShortNameGenerator.NonUniqueShortNameFor(type), type);

        public string ForMethodDeclarations(TypeReference type) => 
            $"{this.BaseFileName(type)}MethodDeclarations.h";

        public string ForModule(ModuleDefinition module)
        {
            if (module.FullyQualifiedName == null)
            {
            }
            return Path.GetFileNameWithoutExtension(module.Name);
        }

        public string ForTypeDefinition(TypeReference type) => 
            $"{this.BaseFileName(type)}.h";

        public static IFileNameProvider Instance =>
            _instance;
    }
}

