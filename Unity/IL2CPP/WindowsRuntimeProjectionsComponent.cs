namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;
    using Unity.IL2CPP.Marshaling.BodyWriters.ManagedToNative.WindowsRuntimeProjection;
    using Unity.IL2CPP.WindowsRuntime;

    internal class WindowsRuntimeProjectionsComponent : IWindowsRuntimeProjections, IWindowsRuntimeProjectionsInitializer
    {
        private readonly Dictionary<TypeDefinition, TypeDefinition> _clrTypeToWindowsRuntimeTypeMap = new Dictionary<TypeDefinition, TypeDefinition>();
        private ModuleDefinition _mscorlib;
        private Dictionary<TypeDefinition, TypeDefinition> _nativeToManagedInterfaceAdapterClasses = new Dictionary<TypeDefinition, TypeDefinition>();
        private readonly Dictionary<TypeDefinition, IProjectedComCallableWrapperMethodWriter> _projectedComCallableWrapperWriterMap = new Dictionary<TypeDefinition, IProjectedComCallableWrapperMethodWriter>();
        private readonly AssemblyNameReference _windowsAssemblyReference;
        private readonly Dictionary<TypeDefinition, TypeDefinition> _windowsRuntimeTypeToCLRTypeMap = new Dictionary<TypeDefinition, TypeDefinition>();
        [CompilerGenerated]
        private static Func<InterfaceImplementation, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<InterfaceImplementation, bool> <>f__am$cache1;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public WindowsRuntimeProjectionsComponent()
        {
            AssemblyNameReference reference = new AssemblyNameReference("Windows", new Version(0xff, 0xff, 0xff, 0xff)) {
                IsWindowsRuntime = true
            };
            this._windowsAssemblyReference = reference;
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

        public TypeDefinition GetNativeToManagedAdapterClassFor(TypeDefinition interfaceType)
        {
            TypeDefinition definition = null;
            this._nativeToManagedInterfaceAdapterClasses.TryGetValue(interfaceType, out definition);
            return definition;
        }

        public IProjectedComCallableWrapperMethodWriter GetProjectedComCallableWrapperMethodWriterFor(TypeDefinition type)
        {
            IProjectedComCallableWrapperMethodWriter writer;
            this._projectedComCallableWrapperWriterMap.TryGetValue(type, out writer);
            return writer;
        }

        private MethodDefinition GetSingleMethod(TypeDefinition type, string name)
        {
            <GetSingleMethod>c__AnonStorey0 storey = new <GetSingleMethod>c__AnonStorey0 {
                name = name
            };
            return type.Methods.Single<MethodDefinition>(new Func<MethodDefinition, bool>(storey.<>m__0));
        }

        public void Initialize(ModuleDefinition mscorlib, DotNetProfile dotNetProfile)
        {
            this._mscorlib = mscorlib;
            if ((dotNetProfile != DotNetProfile.Net20) && (dotNetProfile != DotNetProfile.Unity))
            {
                TypeDefinition definition;
                TypeDefinition definition2;
                Dictionary<MethodDefinition, InterfaceAdapterMethodBodyWriter> adapterMethodBodyWriters = new Dictionary<MethodDefinition, InterfaceAdapterMethodBodyWriter>();
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
                if (this.AddProjection("System.Runtime", "System", "IDisposable", "Windows.Foundation", "IClosable", out definition, out definition2))
                {
                    adapterMethodBodyWriters.Add(this.GetSingleMethod(definition, "Dispose"), new InterfaceAdapterMethodBodyWriter(new IDisposableDisposeMethodBodyWriter(this.GetSingleMethod(definition2, "Close")).WriteDispose));
                    this._projectedComCallableWrapperWriterMap.Add(definition2, new DisposableCCWWriter());
                }
                this.AddProjection("System.Runtime", "System", "Nullable`1", "Windows.Foundation", "IReference`1", out definition, out definition2);
                this.AddProjection("System.Runtime", "System", "TimeSpan", "Windows.Foundation", "TimeSpan", out definition, out definition2);
                this.AddProjection("System.Runtime", "System", "Type", "Windows.UI.Xaml.Interop", "TypeName", out definition, out definition2);
                this.AddProjection("System.Runtime", "System", "Uri", "Windows.Foundation", "Uri", out definition, out definition2);
                TypeDefinition ienumeratorType = TypeProvider.OptionalResolve("System.Collections", "IEnumerator", TypeProvider.Corlib.Name);
                TypeDefinition iteratorType = TypeProvider.OptionalResolve("Windows.UI.Xaml.Interop", "IBindableIterator", this._windowsAssemblyReference);
                if (((ienumeratorType != null) && (iteratorType != null)) && this.AddProjection("System.Runtime", "System.Collections", "IEnumerable", "Windows.UI.Xaml.Interop", "IBindableIterable", out definition, out definition2))
                {
                    TypeDefinition iteratorToEnumeratorAdapter = new IteratorToEnumeratorAdapterTypeGenerator(TypeProvider.Corlib.MainModule, iteratorType, ienumeratorType).Generate();
                    adapterMethodBodyWriters.Add(this.GetSingleMethod(definition, "GetEnumerator"), new InterfaceAdapterMethodBodyWriter(new IEnumerableMethodBodyWriter(iteratorToEnumeratorAdapter).WriteGetEnumerator));
                    this._projectedComCallableWrapperWriterMap.Add(definition2, new EnumerableCCWWriter());
                }
                this.AddProjection("System.Runtime", "System.Collections", "IList", "Windows.UI.Xaml.Interop", "IBindableVector", out definition, out definition2);
                this.AddProjection("System.Runtime", "System.Collections.Generic", "IDictionary`2", "Windows.Foundation.Collections", "IMap`2", out definition, out definition2);
                TypeDefinition definition6 = TypeProvider.OptionalResolve("System.Collections.Generic", "IEnumerator`1", TypeProvider.Corlib.Name);
                TypeDefinition definition7 = TypeProvider.OptionalResolve("Windows.Foundation.Collections", "IIterator`1", this._windowsAssemblyReference);
                if (((definition6 != null) && (definition7 != null)) && this.AddProjection("System.Runtime", "System.Collections.Generic", "IEnumerable`1", "Windows.Foundation.Collections", "IIterable`1", out definition, out definition2))
                {
                    TypeDefinition definition8 = new IteratorToEnumeratorAdapterTypeGenerator(TypeProvider.Corlib.MainModule, definition7, definition6).Generate();
                    adapterMethodBodyWriters.Add(this.GetSingleMethod(definition, "GetEnumerator"), new InterfaceAdapterMethodBodyWriter(new IEnumerableMethodBodyWriter(definition8).WriteGetEnumerator));
                    this._projectedComCallableWrapperWriterMap.Add(definition2, new EnumerableCCWWriter());
                }
                this.AddProjection("System.Runtime", "System.Collections.Generic", "IList`1", "Windows.Foundation.Collections", "IVector`1", out definition, out definition2);
                TypeDefinition definition9 = null;
                TypeDefinition type = null;
                TypeDefinition definition11 = null;
                TypeDefinition definition12 = null;
                if (this.AddProjection("System.Runtime", "System.Collections.Generic", "IReadOnlyDictionary`2", "Windows.Foundation.Collections", "IMapView`2", out definition, out definition2))
                {
                    IReadOnlyDictionaryProjectedMethodBodyWriter writer = new IReadOnlyDictionaryProjectedMethodBodyWriter(definition, definition2);
                    adapterMethodBodyWriters.Add(this.GetSingleMethod(definition, "get_Item"), new InterfaceAdapterMethodBodyWriter(writer.WriteGetItem));
                    adapterMethodBodyWriters.Add(this.GetSingleMethod(definition, "get_Keys"), new InterfaceAdapterMethodBodyWriter(writer.WriteGetKeys));
                    adapterMethodBodyWriters.Add(this.GetSingleMethod(definition, "get_Values"), new InterfaceAdapterMethodBodyWriter(writer.WriteGetValues));
                    adapterMethodBodyWriters.Add(this.GetSingleMethod(definition, "ContainsKey"), new InterfaceAdapterMethodBodyWriter(writer.WriteContainsKey));
                    adapterMethodBodyWriters.Add(this.GetSingleMethod(definition, "TryGetValue"), new InterfaceAdapterMethodBodyWriter(writer.WriteTryGetValue));
                    this._projectedComCallableWrapperWriterMap.Add(definition2, new ReadOnlyDictionaryCCWWriter(definition));
                    definition9 = definition;
                    type = definition2;
                }
                if (this.AddProjection("System.Runtime", "System.Collections.Generic", "IReadOnlyList`1", "Windows.Foundation.Collections", "IVectorView`1", out definition, out definition2))
                {
                    adapterMethodBodyWriters.Add(this.GetSingleMethod(definition, "get_Item"), new InterfaceAdapterMethodBodyWriter(new IReadOnlyListGetItemMethodBodyWriter(this.GetSingleMethod(definition2, "GetAt")).WriteGetItem));
                    this._projectedComCallableWrapperWriterMap.Add(definition2, new ReadOnlyListCCWWriter(definition));
                    definition11 = definition;
                    definition12 = definition2;
                }
                if ((definition12 != null) || (type != null))
                {
                    if (definition11 == null)
                    {
                    }
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = i => i.InterfaceType.Name == "IReadOnlyCollection`1";
                    }
                    TypeDefinition iReadOnlyCollectionType = (<>f__am$cache0 != null) ? definition9.Interfaces.Single<InterfaceImplementation>(<>f__am$cache1).InterfaceType.Resolve() : definition11.Interfaces.Single<InterfaceImplementation>(<>f__am$cache0).InterfaceType.Resolve();
                    MethodDefinition iMapViewGetSizeMethod = null;
                    MethodDefinition iVectorViewGetSizeMethod = null;
                    if (type != null)
                    {
                        iMapViewGetSizeMethod = this.GetSingleMethod(type, "get_Size");
                    }
                    if (definition12 != null)
                    {
                        iVectorViewGetSizeMethod = this.GetSingleMethod(definition12, "get_Size");
                    }
                    IReadOnlyCollectionGetCountMethodBodyWriter writer2 = new IReadOnlyCollectionGetCountMethodBodyWriter(iReadOnlyCollectionType, iMapViewGetSizeMethod, iVectorViewGetSizeMethod);
                    adapterMethodBodyWriters.Add(this.GetSingleMethod(iReadOnlyCollectionType, "get_Count"), new InterfaceAdapterMethodBodyWriter(writer2.WriteGetCount));
                }
                if (this.AddProjection("System.Runtime", "System.Collections.Generic", "KeyValuePair`2", "Windows.Foundation.Collections", "IKeyValuePair`2", out definition, out definition2))
                {
                    this._projectedComCallableWrapperWriterMap.Add(definition2, new KeyValuePairCCWWriter(definition));
                }
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
                this._nativeToManagedInterfaceAdapterClasses = InterfaceNativeToManagedAdapterGenerator.Generate(this._clrTypeToWindowsRuntimeTypeMap, adapterMethodBodyWriters);
            }
        }

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
            if (!(windowsRuntimeType is TypeSpecification) || windowsRuntimeType.IsGenericInstance)
            {
                TypeDefinition definition;
                if (windowsRuntimeType.IsGenericParameter)
                {
                    return windowsRuntimeType;
                }
                if (this._windowsRuntimeTypeToCLRTypeMap.TryGetValue(windowsRuntimeType.Resolve(), out definition))
                {
                    return Unity.IL2CPP.ILPreProcessor.TypeResolver.For(windowsRuntimeType).Resolve(definition);
                }
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
            if (!(clrType is TypeSpecification) || clrType.IsGenericInstance)
            {
                TypeDefinition definition;
                if (clrType.IsGenericParameter)
                {
                    return clrType;
                }
                if (this._clrTypeToWindowsRuntimeTypeMap.TryGetValue(clrType.Resolve(), out definition))
                {
                    return Unity.IL2CPP.ILPreProcessor.TypeResolver.For(clrType).Resolve(definition);
                }
            }
            return clrType;
        }

        [CompilerGenerated]
        private sealed class <GetSingleMethod>c__AnonStorey0
        {
            internal string name;

            internal bool <>m__0(MethodDefinition m) => 
                (m.Name == this.name);
        }
    }
}

