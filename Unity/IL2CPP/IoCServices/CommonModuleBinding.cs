namespace Unity.IL2CPP.IoCServices
{
    using System;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.Metadata;
    using Unity.IL2CPP.StringLiterals;

    public static class CommonModuleBinding
    {
        public static void Bind(Container container)
        {
            container.BindSingleton<IStatsService, StatsComponent>();
            container.BindSingleton<IGenericSharingAnalysisService, GenericSharingAnalysisComponent>();
            container.BindSingleton<INamingService, NamingComponent>();
            container.BindSingleton<IIcallMappingService, ICallMappingComponent>();
            container.BindSingleton<UniqueShortNameGenerator, UniqueShortNameGenerator>();
            container.BindSingleton<IVirtualCallCollectorService, VirtualCallCollector>();
            container.BindSingleton<IAssemblyDependencies, AssemblyDependenciesComponent>();
            container.BindSingleton<IGuidProvider, GuidProviderComponent>();
            container.BindSingleton<ISourceAnnotationWriter, SourceAnnotationWriterComponent>();
            Type[] serviceTypes = new Type[] { typeof(ITypeProviderInitializerService), typeof(ITypeProviderService) };
            container.BindMultiSingleton(serviceTypes, typeof(TypeProviderComponent));
            Type[] typeArray2 = new Type[] { typeof(IRuntimeInvokerCollectorAdderService), typeof(IRuntimeInvokerCollectorReaderService), typeof(IRuntimeInvokerCollectorWriterService) };
            container.BindMultiSingleton(typeArray2, typeof(RuntimeInvokerCollectorComponent));
            Type[] typeArray3 = new Type[] { typeof(IMetadataUsageCollectorReaderService), typeof(IMetadataUsageCollectorWriterService) };
            container.BindMultiSingleton(typeArray3, typeof(MetadataUsageCollectorComponent));
            Type[] typeArray4 = new Type[] { typeof(IIl2CppTypeCollectorReaderService), typeof(IIl2CppTypeCollectorWriterService) };
            container.BindMultiSingleton(typeArray4, typeof(Il2CppTypeCollectorComponent));
            Type[] typeArray5 = new Type[] { typeof(IIl2CppGenericInstCollectorReaderService), typeof(IIl2CppGenericInstCollectorWriterService) };
            container.BindMultiSingleton(typeArray5, typeof(Il2CppGenericInstCollectorComponent));
            Type[] typeArray6 = new Type[] { typeof(IIl2CppGenericMethodCollectorReaderService), typeof(IIl2CppGenericMethodCollectorWriterService) };
            container.BindMultiSingleton(typeArray6, typeof(Il2CppGenericMethodCollectorComponent));
            Type[] typeArray7 = new Type[] { typeof(IIl2CppMethodReferenceCollectorReaderService), typeof(IIl2CppMethodReferenceCollectorWriterService) };
            container.BindMultiSingleton(typeArray7, typeof(Il2CppMethodReferenceCollectorComponent));
            Type[] typeArray8 = new Type[] { typeof(IIl2CppFieldReferenceCollectorReaderService), typeof(IIl2CppFieldReferenceCollectorWriterService) };
            container.BindMultiSingleton(typeArray8, typeof(Il2CppFieldReferenceCollectorComponent));
            Type[] typeArray9 = new Type[] { typeof(IStringLiteralProvider), typeof(IStringLiteralCollection) };
            container.BindMultiSingleton(typeArray9, typeof(StringLiteralCollection));
            Type[] typeArray10 = new Type[] { typeof(IWindowsRuntimeProjectionsInitializer), typeof(IWindowsRuntimeProjections) };
            container.BindMultiSingleton(typeArray10, typeof(WindowsRuntimeProjectionsComponent));
        }
    }
}

