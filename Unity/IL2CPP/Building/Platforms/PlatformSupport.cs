namespace Unity.IL2CPP.Building.Platforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.Building;
    using Unity.IL2CPP.Building.BuildDescriptions;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.Portability;

    public abstract class PlatformSupport
    {
        [CompilerGenerated]
        private static Func<Type, bool> <>f__am$cache0;

        protected PlatformSupport()
        {
        }

        private static IEnumerable<Type> AllTypes()
        {
            IEnumerable<Type> first = new Type[0];
            foreach (Assembly assembly in AppDomainPortable.GetAllAssembliesInCurrentAppDomainPortable())
            {
                try
                {
                    first = first.Concat<Type>(assembly.GetTypesPortable());
                }
                catch (ReflectionTypeLoadException)
                {
                }
            }
            return first;
        }

        public static bool Available(RuntimePlatform runtimePlatform)
        {
            PlatformSupport support;
            return TryFor(runtimePlatform, out support);
        }

        public static PlatformSupport For(RuntimePlatform runtimePlatform)
        {
            PlatformSupport support;
            if (!TryFor(runtimePlatform, out support))
            {
                throw new InvalidOperationException($"Could not find platform support for {runtimePlatform.Name} runtime platform. Is platform plugin present?");
            }
            return support;
        }

        public virtual CppToolChain MakeCppToolChain(BuildingOptions buildingOptions) => 
            this.MakeCppToolChain(buildingOptions.Architecture, buildingOptions.Configuration, buildingOptions.TreatWarningsAsErrors);

        public abstract CppToolChain MakeCppToolChain(Unity.IL2CPP.Building.Architecture architecture, BuildConfiguration buildConfiguration, bool treatWarningsAsErrors);
        public virtual ProgramBuildDescription PostProcessProgramBuildDescription(ProgramBuildDescription programBuildDescription) => 
            programBuildDescription;

        public virtual void RegisterRunner()
        {
        }

        public abstract bool Supports(RuntimePlatform platform);
        public static bool TryFor(RuntimePlatform runtimePlatform, out PlatformSupport support)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<Type, bool>(null, (IntPtr) <TryFor>m__0);
            }
            IEnumerable<Type> enumerable = AllTypes().Where<Type>(<>f__am$cache0);
            foreach (Type type in enumerable)
            {
                PlatformSupport support2 = (PlatformSupport) Activator.CreateInstance(type);
                if (support2.Supports(runtimePlatform))
                {
                    support = support2;
                    return true;
                }
            }
            support = null;
            return false;
        }
    }
}

