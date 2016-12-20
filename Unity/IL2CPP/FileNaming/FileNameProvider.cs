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

        private string BaseFileName(TypeReference type)
        {
            return _typeShortNameGenerator.GetUniqueShortName(ShortNameGenerator.NonUniqueShortNameFor(type), type);
        }

        public string ForMethodDeclarations(TypeReference type)
        {
            return string.Format("{0}MethodDeclarations.h", this.BaseFileName(type));
        }

        public string ForModule(ModuleDefinition module)
        {
            return Path.GetFileNameWithoutExtension(module.FullyQualifiedName);
        }

        public string ForTypeDefinition(TypeReference type)
        {
            return string.Format("{0}.h", this.BaseFileName(type));
        }

        public static IFileNameProvider Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}

