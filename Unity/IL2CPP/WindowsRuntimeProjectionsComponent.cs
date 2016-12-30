namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using Mono.Collections.Generic;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    internal class WindowsRuntimeProjectionsComponent : IWindowsRuntimeProjections, IWindowsRuntimeProjectionsInitializer
    {
        private readonly Dictionary<TypeDefinition, CCWWriterPair> _ccwWriterTypeMap = new Dictionary<TypeDefinition, CCWWriterPair>();
        private readonly Dictionary<TypeDefinition, TypeDefinition> _clrTypeToWindowsRuntimeTypeMap = new Dictionary<TypeDefinition, TypeDefinition>();
        private bool _hasIEnumerableCCW;
        private readonly Dictionary<MethodDefinition, WindowsRuntimeProjectedMethodBodyWriter> _methodBodyWriterTypeMap = new Dictionary<MethodDefinition, WindowsRuntimeProjectedMethodBodyWriter>();
        private ModuleDefinition _mscorlib;
        private readonly AssemblyNameReference _windowsAssemblyReference;
        private readonly Dictionary<TypeDefinition, TypeDefinition> _windowsRuntimeTypeToCLRTypeMap = new Dictionary<TypeDefinition, TypeDefinition>();
        [CompilerGenerated]
        private static Func<MethodDefinition, TypeDefinition> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeDefinition, CCWWriterPair>, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<TypeDefinition, CCWWriterPair>, KeyValuePair<TypeDefinition, WindowsRuntimeProjectedCCWWriter>> <>f__am$cache2;
        [CompilerGenerated]
        private static WindowsRuntimeProjectedMethodBodyWriter <>f__mg$cache0;
        [CompilerGenerated]
        private static WindowsRuntimeProjectedCCWWriter <>f__mg$cache1;
        [CompilerGenerated]
        private static WindowsRuntimeProjectedCCWWriter <>f__mg$cache2;
        [CompilerGenerated]
        private static WindowsRuntimeProjectedCCWWriter <>f__mg$cache3;
        [CompilerGenerated]
        private static WindowsRuntimeProjectedCCWWriter <>f__mg$cache4;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public WindowsRuntimeProjectionsComponent()
        {
            AssemblyNameReference reference = new AssemblyNameReference("Windows", new Version(0xff, 0xff, 0xff, 0xff)) {
                IsWindowsRuntime = true
            };
            this._windowsAssemblyReference = reference;
        }

        private void AddCCWWriter(TypeDefinition windowsRuntimeType, WindowsRuntimeProjectedCCWWriter typeDefinitionWriter, WindowsRuntimeProjectedCCWWriter methodDefinitionsWriter)
        {
            this._ccwWriterTypeMap.Add(windowsRuntimeType, new CCWWriterPair(typeDefinitionWriter, methodDefinitionsWriter));
        }

        private static void AddInterfaceToIl2CppComObject(TypeDefinition interfaceType)
        {
            TypeDefinition definition = TypeProvider.Il2CppComObjectTypeReference.Resolve();
            ModuleDefinition module = definition.Module;
            definition.Interfaces.Add(new InterfaceImplementation(module.ImportReference(interfaceType)));
            foreach (MethodDefinition definition3 in interfaceType.Methods)
            {
                MethodDefinition item = new MethodDefinition(definition3.Name, MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, module.ImportReference(definition3.ReturnType));
                definition.Methods.Add(item);
                item.Overrides.Add(definition3);
                foreach (ParameterDefinition definition5 in definition3.Parameters)
                {
                    item.Parameters.Add(new ParameterDefinition(definition5.Name, definition5.Attributes, module.ImportReference(definition5.ParameterType)));
                }
            }
            using (Collection<PropertyDefinition>.Enumerator enumerator3 = interfaceType.Properties.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    <AddInterfaceToIl2CppComObject>c__AnonStorey1 storey = new <AddInterfaceToIl2CppComObject>c__AnonStorey1 {
                        interfaceProperty = enumerator3.Current
                    };
                    PropertyDefinition definition6 = new PropertyDefinition(storey.interfaceProperty.Name, storey.interfaceProperty.Attributes, module.ImportReference(storey.interfaceProperty.PropertyType));
                    definition.Properties.Add(definition6);
                    if (storey.interfaceProperty.GetMethod != null)
                    {
                        definition6.GetMethod = interfaceType.Methods.First<MethodDefinition>(new Func<MethodDefinition, bool>(storey.<>m__0));
                    }
                    if (storey.interfaceProperty.SetMethod != null)
                    {
                        definition6.SetMethod = interfaceType.Methods.First<MethodDefinition>(new Func<MethodDefinition, bool>(storey.<>m__1));
                    }
                }
            }
        }

        private void AddMethodBodyWriter(TypeDefinition clrType, string clrMethodName, WindowsRuntimeProjectedMethodBodyWriter methodBodyWriter)
        {
            <AddMethodBodyWriter>c__AnonStorey0 storey = new <AddMethodBodyWriter>c__AnonStorey0 {
                clrMethodName = clrMethodName
            };
            MethodDefinition key = clrType.Methods.First<MethodDefinition>(new Func<MethodDefinition, bool>(storey.<>m__0)).Resolve();
            this._methodBodyWriterTypeMap.Add(key, methodBodyWriter);
        }

        private bool AddProjection(string clrAssembly, string clrNamespace, string clrName, string windowsRuntimeNamespace, string windowsRuntimeName, out TypeDefinition clrType, out TypeDefinition windowsRuntimeType)
        {
            TypeReference reference = new TypeReference(clrNamespace, clrName, this._mscorlib, new AssemblyNameReference(clrAssembly, new Version(4, 0, 0, 0)));
            TypeReference reference2 = new TypeReference(windowsRuntimeNamespace, windowsRuntimeName, this._mscorlib, this._windowsAssemblyReference);
            try
            {
                clrType = reference.Resolve();
                windowsRuntimeType = reference2.Resolve();
                if ((clrType != null) && (windowsRuntimeType != null))
                {
                    this._clrTypeToWindowsRuntimeTypeMap.Add(clrType, windowsRuntimeType);
                    this._windowsRuntimeTypeToCLRTypeMap.Add(windowsRuntimeType, clrType);
                    return true;
                }
            }
            catch (AssemblyResolutionException)
            {
            }
            clrType = null;
            windowsRuntimeType = null;
            return false;
        }

        public IEnumerable<KeyValuePair<TypeDefinition, WindowsRuntimeProjectedCCWWriter>> GetAllNonGenericCCWMethodDefinitionsWriters()
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = p => !p.Key.HasGenericParameters;
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = p => new KeyValuePair<TypeDefinition, WindowsRuntimeProjectedCCWWriter>(p.Key, p.Value.MethodDefinitionsWriter);
            }
            return this._ccwWriterTypeMap.Where<KeyValuePair<TypeDefinition, CCWWriterPair>>(<>f__am$cache1).Select<KeyValuePair<TypeDefinition, CCWWriterPair>, KeyValuePair<TypeDefinition, WindowsRuntimeProjectedCCWWriter>>(<>f__am$cache2);
        }

        public WindowsRuntimeProjectedCCWWriter GetCCWWriter(TypeDefinition type, bool typeDefinition)
        {
            CCWWriterPair pair;
            if (!this._ccwWriterTypeMap.TryGetValue(type, out pair))
            {
                return null;
            }
            return (!typeDefinition ? pair.MethodDefinitionsWriter : pair.TypeDefinitionWriter);
        }

        public WindowsRuntimeProjectedMethodBodyWriter GetMethodBodyWriter(MethodDefinition method)
        {
            WindowsRuntimeProjectedMethodBodyWriter writer2;
            if (!method.HasOverrides)
            {
                return null;
            }
            MethodDefinition key = method.Overrides.Single<MethodReference>().Resolve();
            return (!this._methodBodyWriterTypeMap.TryGetValue(key, out writer2) ? null : writer2);
        }

        public IEnumerable<TypeDefinition> GetSupportedProjectedInterfacesCLR()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => m.DeclaringType;
            }
            return this._methodBodyWriterTypeMap.Keys.Select<MethodDefinition, TypeDefinition>(<>f__am$cache0).Distinct<TypeDefinition>();
        }

        public void Initialize(ModuleDefinition mscorlib, DotNetProfile dotNetProfile)
        {
            this._mscorlib = mscorlib;
            if ((dotNetProfile != DotNetProfile.Net20) && (dotNetProfile != DotNetProfile.Unity))
            {
                TypeDefinition definition;
                TypeDefinition definition2;
                this.AddProjection("System.ObjectModel", "System.Collections.Specialized", "INotifyCollectionChanged", "Windows.UI.Xaml.Interop", "INotifyCollectionChanged", out definition, out definition2);
                this.AddProjection("System.ObjectModel", "System.Collections.Specialized", "NotifyCollectionChangedAction", "Windows.UI.Xaml.Interop", "NotifyCollectionChangedAction", out definition, out definition2);
                this.AddProjection("System.ObjectModel", "System.Collections.Specialized", "NotifyCollectionChangedEventArgs", "Windows.UI.Xaml.Interop", "NotifyCollectionChangedEventArgs", out definition, out definition2);
                this.AddProjection("System.ObjectModel", "System.Collections.Specialized", "NotifyCollectionChangedEventHandler", "Windows.UI.Xaml.Interop", "NotifyCollectionChangedEventHandler", out definition, out definition2);
                this.AddProjection("System.ObjectModel", "System.ComponentModel", "INotifyPropertyChanged", "Windows.UI.Xaml.Data", "INotifyPropertyChanged", out definition, out definition2);
                this.AddProjection("System.ObjectModel", "System.ComponentModel", "PropertyChangedEventArgs", "Windows.UI.Xaml.Data", "PropertyChangedEventArgs", out definition, out definition2);
                this.AddProjection("System.ObjectModel", "System.ComponentModel", "PropertyChangedEventHandler", "Windows.UI.Xaml.Data", "PropertyChangedEventHandler", out definition, out definition2);
                this.AddProjection("System.ObjectModel", "System.Windows.Input", "ICommand", "Windows.UI.Xaml.Input", "ICommand", out definition, out definition2);
                this.AddProjection("System.Runtime", "System", "AttributeTargets", "Windows.Foundation.Metadata", "AttributeTargets", out definition, out definition2);
                this.AddProjection("System.Runtime", "System", "AttributeUsageAttribute", "Windows.Foundation.Metadata", "AttributeUsageAttribute", out definition, out definition2);
                this.AddProjection("System.Runtime", "System", "DateTimeOffset", "Windows.Foundation", "DateTime", out definition, out definition2);
                this.AddProjection("System.Runtime", "System", "EventHandler`1", "Windows.Foundation", "EventHandler`1", out definition, out definition2);
                this.AddProjection("System.Runtime", "System", "Exception", "Windows.Foundation", "HResult", out definition, out definition2);
                this.AddProjection("System.Runtime", "System", "IDisposable", "Windows.Foundation", "IClosable", out definition, out definition2);
                this.AddProjection("System.Runtime", "System", "Nullable`1", "Windows.Foundation", "IReference`1", out definition, out definition2);
                this.AddProjection("System.Runtime", "System", "TimeSpan", "Windows.Foundation", "TimeSpan", out definition, out definition2);
                this.AddProjection("System.Runtime", "System", "Type", "Windows.UI.Xaml.Interop", "TypeName", out definition, out definition2);
                this.AddProjection("System.Runtime", "System", "Uri", "Windows.Foundation", "Uri", out definition, out definition2);
                if (this.AddProjection("System.Runtime", "System.Collections", "IEnumerable", "Windows.UI.Xaml.Interop", "IBindableIterable", out definition, out definition2))
                {
                    if (<>f__mg$cache0 == null)
                    {
                        <>f__mg$cache0 = new WindowsRuntimeProjectedMethodBodyWriter(IEnumerableMethodBodyWriter.WriteGetEnumerator);
                    }
                    this.AddMethodBodyWriter(definition, "GetEnumerator", <>f__mg$cache0);
                    if (<>f__mg$cache1 == null)
                    {
                        <>f__mg$cache1 = new WindowsRuntimeProjectedCCWWriter(EnumerableCCWWriter.WriteTypeDefinition);
                    }
                    if (<>f__mg$cache2 == null)
                    {
                        <>f__mg$cache2 = new WindowsRuntimeProjectedCCWWriter(EnumerableCCWWriter.WriteMethodDefinitions);
                    }
                    this.AddCCWWriter(definition2, <>f__mg$cache1, <>f__mg$cache2);
                }
                this.AddProjection("System.Runtime", "System.Collections", "IList", "Windows.UI.Xaml.Interop", "IBindableVector", out definition, out definition2);
                this.AddProjection("System.Runtime", "System.Collections.Generic", "IDictionary`2", "Windows.Foundation.Collections", "IMap`2", out definition, out definition2);
                if (this.AddProjection("System.Runtime", "System.Collections.Generic", "IEnumerable`1", "Windows.Foundation.Collections", "IIterable`1", out definition, out definition2))
                {
                    if (<>f__mg$cache3 == null)
                    {
                        <>f__mg$cache3 = new WindowsRuntimeProjectedCCWWriter(GenericEnumerableCCWWriter.WriteTypeDefinition);
                    }
                    if (<>f__mg$cache4 == null)
                    {
                        <>f__mg$cache4 = new WindowsRuntimeProjectedCCWWriter(GenericEnumerableCCWWriter.WriteMethodDefinitions);
                    }
                    this.AddCCWWriter(definition2, <>f__mg$cache3, <>f__mg$cache4);
                }
                this.AddProjection("System.Runtime", "System.Collections.Generic", "IList`1", "Windows.Foundation.Collections", "IVector`1", out definition, out definition2);
                this.AddProjection("System.Runtime", "System.Collections.Generic", "IReadOnlyDictionary`2", "Windows.Foundation.Collections", "IMapView`2", out definition, out definition2);
                this.AddProjection("System.Runtime", "System.Collections.Generic", "IReadOnlyList`1", "Windows.Foundation.Collections", "IVectorView`1", out definition, out definition2);
                this.AddProjection("System.Runtime", "System.Collections.Generic", "KeyValuePair`2", "Windows.Foundation.Collections", "IKeyValuePair`2", out definition, out definition2);
                this.AddProjection("System.Runtime.InteropServices.WindowsRuntime", "System.Runtime.InteropServices.WindowsRuntime", "EventRegistrationToken", "Windows.Foundation", "EventRegistrationToken", out definition, out definition2);
                this.AddProjection("System.Runtime.WindowsRuntime", "Windows.Foundation", "Point", "Windows.Foundation", "Point", out definition, out definition2);
                this.AddProjection("System.Runtime.WindowsRuntime", "Windows.Foundation", "Rect", "Windows.Foundation", "Rect", out definition, out definition2);
                this.AddProjection("System.Runtime.WindowsRuntime", "Windows.Foundation", "Size", "Windows.Foundation", "Size", out definition, out definition2);
                this.AddProjection("System.Runtime.WindowsRuntime", "Windows.UI", "Color", "Windows.UI", "Color", out definition, out definition2);
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml", "CornerRadius", "Windows.UI.Xaml", "CornerRadius", out definition, out definition2);
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml", "Duration", "Windows.UI.Xaml", "Duration", out definition, out definition2);
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml", "DurationType", "Windows.UI.Xaml", "DurationType", out definition, out definition2);
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml", "GridLength", "Windows.UI.Xaml", "GridLength", out definition, out definition2);
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml", "GridUnitType", "Windows.UI.Xaml", "GridUnitType", out definition, out definition2);
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml", "Thickness", "Windows.UI.Xaml", "Thickness", out definition, out definition2);
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml.Controls.Primitives", "GeneratorPosition", "Windows.UI.Xaml.Controls.Primitives", "GeneratorPosition", out definition, out definition2);
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml.Media", "Matrix", "Windows.UI.Xaml.Media", "Matrix", out definition, out definition2);
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml.Media.Animation", "RepeatBehavior", "Windows.UI.Xaml.Media.Animation", "RepeatBehavior", out definition, out definition2);
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml.Media.Animation", "RepeatBehaviorType", "Windows.UI.Xaml.Media.Animation", "RepeatBehaviorType", out definition, out definition2);
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml.Media.Animation", "KeyTime", "Windows.UI.Xaml.Media.Animation", "KeyTime", out definition, out definition2);
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml.Media.Media3D", "Matrix3D", "Windows.UI.Xaml.Media.Media3D", "Matrix3D", out definition, out definition2);
                this.AddProjection("System.Numerics.Vectors", "System.Numerics", "Matrix3x2", "Windows.Foundation.Numerics", "Matrix3x2", out definition, out definition2);
                this.AddProjection("System.Numerics.Vectors", "System.Numerics", "Matrix4x4", "Windows.Foundation.Numerics", "Matrix4x4", out definition, out definition2);
                this.AddProjection("System.Numerics.Vectors", "System.Numerics", "Plane", "Windows.Foundation.Numerics", "Plane", out definition, out definition2);
                this.AddProjection("System.Numerics.Vectors", "System.Numerics", "Quaternion", "Windows.Foundation.Numerics", "Quaternion", out definition, out definition2);
                this.AddProjection("System.Numerics.Vectors", "System.Numerics", "Vector2", "Windows.Foundation.Numerics", "Vector2", out definition, out definition2);
                this.AddProjection("System.Numerics.Vectors", "System.Numerics", "Vector3", "Windows.Foundation.Numerics", "Vector3", out definition, out definition2);
                this.AddProjection("System.Numerics.Vectors", "System.Numerics", "Vector4", "Windows.Foundation.Numerics", "Vector4", out definition, out definition2);
                this.InitializeIl2CppComObject();
            }
        }

        private void InitializeIl2CppComObject()
        {
            foreach (TypeDefinition definition in this.GetSupportedProjectedInterfacesCLR())
            {
                AddInterfaceToIl2CppComObject(definition);
            }
        }

        public bool IsSupportedProjectedInterfaceWindowsRuntime(TypeReference type) => 
            this._ccwWriterTypeMap.ContainsKey(type.Resolve());

        public TypeDefinition ProjectToCLR(TypeDefinition windowsRuntimeType)
        {
            TypeDefinition definition;
            if (this._windowsRuntimeTypeToCLRTypeMap.TryGetValue(windowsRuntimeType, out definition))
            {
                return definition;
            }
            return windowsRuntimeType;
        }

        public TypeReference ProjectToCLR(TypeReference windowsRuntimeType)
        {
            TypeDefinition definition;
            if (this._windowsRuntimeTypeToCLRTypeMap.TryGetValue(windowsRuntimeType.Resolve(), out definition))
            {
                return Unity.IL2CPP.ILPreProcessor.TypeResolver.For(windowsRuntimeType).Resolve(definition);
            }
            return windowsRuntimeType;
        }

        public TypeDefinition ProjectToWindowsRuntime(TypeDefinition clrType)
        {
            TypeDefinition definition;
            if (this._clrTypeToWindowsRuntimeTypeMap.TryGetValue(clrType, out definition))
            {
                return definition;
            }
            return clrType;
        }

        public TypeReference ProjectToWindowsRuntime(TypeReference clrType)
        {
            TypeDefinition definition;
            if (this._clrTypeToWindowsRuntimeTypeMap.TryGetValue(clrType.Resolve(), out definition))
            {
                return Unity.IL2CPP.ILPreProcessor.TypeResolver.For(clrType).Resolve(definition);
            }
            return clrType;
        }

        public bool HasIEnumerableCCW
        {
            get => 
                this._hasIEnumerableCCW;
            set
            {
                this._hasIEnumerableCCW = value;
            }
        }

        [CompilerGenerated]
        private sealed class <AddInterfaceToIl2CppComObject>c__AnonStorey1
        {
            internal PropertyDefinition interfaceProperty;

            internal bool <>m__0(MethodDefinition m) => 
                (m.Name == this.interfaceProperty.GetMethod.Name);

            internal bool <>m__1(MethodDefinition m) => 
                (m.Name == this.interfaceProperty.SetMethod.Name);
        }

        [CompilerGenerated]
        private sealed class <AddMethodBodyWriter>c__AnonStorey0
        {
            internal string clrMethodName;

            internal bool <>m__0(MethodDefinition m) => 
                (m.Name == this.clrMethodName);
        }

        private class CCWWriterPair
        {
            public readonly WindowsRuntimeProjectedCCWWriter MethodDefinitionsWriter;
            public readonly WindowsRuntimeProjectedCCWWriter TypeDefinitionWriter;

            public CCWWriterPair(WindowsRuntimeProjectedCCWWriter typeDefinitionWriter, WindowsRuntimeProjectedCCWWriter methodDefinitionsWriter)
            {
                this.TypeDefinitionWriter = typeDefinitionWriter;
                this.MethodDefinitionsWriter = methodDefinitionsWriter;
            }
        }
    }
}

