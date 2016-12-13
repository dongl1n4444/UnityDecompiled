using System;
using Unity.IL2CPP.FileNaming;
using Unity.IL2CPP.GenericSharing;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.Metadata;
using Unity.IL2CPP.StackAnalysis;
using Unity.IL2CPP.StringLiterals;

namespace Unity.IL2CPP.IoCServices
{
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
			container.BindMultiSingleton(new Type[]
			{
				typeof(ITypeProviderInitializerService),
				typeof(ITypeProviderService)
			}, typeof(TypeProviderComponent));
			container.BindMultiSingleton(new Type[]
			{
				typeof(IRuntimeInvokerCollectorAdderService),
				typeof(IRuntimeInvokerCollectorReaderService),
				typeof(IRuntimeInvokerCollectorWriterService)
			}, typeof(RuntimeInvokerCollectorComponent));
			container.BindMultiSingleton(new Type[]
			{
				typeof(IMetadataUsageCollectorReaderService),
				typeof(IMetadataUsageCollectorWriterService)
			}, typeof(MetadataUsageCollectorComponent));
			container.BindMultiSingleton(new Type[]
			{
				typeof(IIl2CppTypeCollectorReaderService),
				typeof(IIl2CppTypeCollectorWriterService)
			}, typeof(Il2CppTypeCollectorComponent));
			container.BindMultiSingleton(new Type[]
			{
				typeof(IIl2CppGenericInstCollectorReaderService),
				typeof(IIl2CppGenericInstCollectorWriterService)
			}, typeof(Il2CppGenericInstCollectorComponent));
			container.BindMultiSingleton(new Type[]
			{
				typeof(IIl2CppGenericMethodCollectorReaderService),
				typeof(IIl2CppGenericMethodCollectorWriterService)
			}, typeof(Il2CppGenericMethodCollectorComponent));
			container.BindMultiSingleton(new Type[]
			{
				typeof(IIl2CppMethodReferenceCollectorReaderService),
				typeof(IIl2CppMethodReferenceCollectorWriterService)
			}, typeof(Il2CppMethodReferenceCollectorComponent));
			container.BindMultiSingleton(new Type[]
			{
				typeof(IIl2CppFieldReferenceCollectorReaderService),
				typeof(IIl2CppFieldReferenceCollectorWriterService)
			}, typeof(Il2CppFieldReferenceCollectorComponent));
			container.BindMultiSingleton(new Type[]
			{
				typeof(IStringLiteralProvider),
				typeof(IStringLiteralCollection)
			}, typeof(StringLiteralCollection));
			container.BindMultiSingleton(new Type[]
			{
				typeof(IWindowsRuntimeProjectionsInitializer),
				typeof(IWindowsRuntimeProjections)
			}, typeof(WindowsRuntimeProjectionsComponent));
		}
	}
}
