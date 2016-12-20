namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoCServices;

    internal class WindowsRuntimeProjectionsComponent : IWindowsRuntimeProjections, IWindowsRuntimeProjectionsInitializer
    {
        private readonly Dictionary<TypeDefinition, TypeDefinition> _clrTypeToWindowsRuntimeTypeMap = new Dictionary<TypeDefinition, TypeDefinition>();
        private ModuleDefinition _mscorlib;
        private readonly AssemblyNameReference _windowsAssemblyReference;
        private readonly Dictionary<TypeDefinition, TypeDefinition> _windowsRuntimeTypeToCLRTypeMap = new Dictionary<TypeDefinition, TypeDefinition>();

        public WindowsRuntimeProjectionsComponent()
        {
            AssemblyNameReference reference = new AssemblyNameReference("Windows", new Version(0xff, 0xff, 0xff, 0xff)) {
                IsWindowsRuntime = true
            };
            this._windowsAssemblyReference = reference;
        }

        private void AddProjection(string clrAssembly, string clrNamespace, string clrName, string windowsRuntimeNamespace, string windowsRuntimeName)
        {
            TypeReference reference = new TypeReference(clrNamespace, clrName, this._mscorlib, new AssemblyNameReference(clrAssembly, new Version(4, 0, 0, 0)));
            TypeReference reference2 = new TypeReference(windowsRuntimeNamespace, windowsRuntimeName, this._mscorlib, this._windowsAssemblyReference);
            try
            {
                TypeDefinition key = reference.Resolve();
                TypeDefinition definition2 = reference2.Resolve();
                if ((key != null) && (definition2 != null))
                {
                    this._clrTypeToWindowsRuntimeTypeMap.Add(key, definition2);
                    this._windowsRuntimeTypeToCLRTypeMap.Add(definition2, key);
                }
            }
            catch (AssemblyResolutionException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void Initialize(ModuleDefinition mscorlib, DotNetProfile dotNetProfile)
        {
            this._mscorlib = mscorlib;
            if ((dotNetProfile != DotNetProfile.Net20) && (dotNetProfile != DotNetProfile.Unity))
            {
                this.AddProjection("System.ObjectModel", "System.Collections.Specialized", "INotifyCollectionChanged", "Windows.UI.Xaml.Interop", "INotifyCollectionChanged");
                this.AddProjection("System.ObjectModel", "System.Collections.Specialized", "NotifyCollectionChangedAction", "Windows.UI.Xaml.Interop", "NotifyCollectionChangedAction");
                this.AddProjection("System.ObjectModel", "System.Collections.Specialized", "NotifyCollectionChangedEventArgs", "Windows.UI.Xaml.Interop", "NotifyCollectionChangedEventArgs");
                this.AddProjection("System.ObjectModel", "System.Collections.Specialized", "NotifyCollectionChangedEventHandler", "Windows.UI.Xaml.Interop", "NotifyCollectionChangedEventHandler");
                this.AddProjection("System.ObjectModel", "System.ComponentModel", "INotifyPropertyChanged", "Windows.UI.Xaml.Data", "INotifyPropertyChanged");
                this.AddProjection("System.ObjectModel", "System.ComponentModel", "PropertyChangedEventArgs", "Windows.UI.Xaml.Data", "PropertyChangedEventArgs");
                this.AddProjection("System.ObjectModel", "System.ComponentModel", "PropertyChangedEventHandler", "Windows.UI.Xaml.Data", "PropertyChangedEventHandler");
                this.AddProjection("System.ObjectModel", "System.Windows.Input", "ICommand", "Windows.UI.Xaml.Input", "ICommand");
                this.AddProjection("System.Runtime", "System", "AttributeTargets", "Windows.Foundation.Metadata", "AttributeTargets");
                this.AddProjection("System.Runtime", "System", "AttributeUsageAttribute", "Windows.Foundation.Metadata", "AttributeUsageAttribute");
                this.AddProjection("System.Runtime", "System", "DateTimeOffset", "Windows.Foundation", "DateTime");
                this.AddProjection("System.Runtime", "System", "EventHandler`1", "Windows.Foundation", "EventHandler`1");
                this.AddProjection("System.Runtime", "System", "Exception", "Windows.Foundation", "HResult");
                this.AddProjection("System.Runtime", "System", "IDisposable", "Windows.Foundation", "IClosable");
                this.AddProjection("System.Runtime", "System", "Nullable`1", "Windows.Foundation", "IReference`1");
                this.AddProjection("System.Runtime", "System", "TimeSpan", "Windows.Foundation", "TimeSpan");
                this.AddProjection("System.Runtime", "System", "Type", "Windows.UI.Xaml.Interop", "TypeName");
                this.AddProjection("System.Runtime", "System", "Uri", "Windows.Foundation", "Uri");
                this.AddProjection("System.Runtime", "System.Collections", "IEnumerable", "Windows.UI.Xaml.Interop", "IBindableIterable");
                this.AddProjection("System.Runtime", "System.Collections", "IList", "Windows.UI.Xaml.Interop", "IBindableVector");
                this.AddProjection("System.Runtime", "System.Collections.Generic", "IDictionary`2", "Windows.Foundation.Collections", "IMap`2");
                this.AddProjection("System.Runtime", "System.Collections.Generic", "IEnumerable`1", "Windows.Foundation.Collections", "IIterable`1");
                this.AddProjection("System.Runtime", "System.Collections.Generic", "IList`1", "Windows.Foundation.Collections", "IVector`1");
                this.AddProjection("System.Runtime", "System.Collections.Generic", "IReadOnlyDictionary`2", "Windows.Foundation.Collections", "IMapView`2");
                this.AddProjection("System.Runtime", "System.Collections.Generic", "IReadOnlyList`1", "Windows.Foundation.Collections", "IVectorView`1");
                this.AddProjection("System.Runtime", "System.Collections.Generic", "KeyValuePair`2", "Windows.Foundation.Collections", "IKeyValuePair`2");
                this.AddProjection("System.Runtime.InteropServices.WindowsRuntime", "System.Runtime.InteropServices.WindowsRuntime", "EventRegistrationToken", "Windows.Foundation", "EventRegistrationToken");
                this.AddProjection("System.Runtime.WindowsRuntime", "Windows.Foundation", "Point", "Windows.Foundation", "Point");
                this.AddProjection("System.Runtime.WindowsRuntime", "Windows.Foundation", "Rect", "Windows.Foundation", "Rect");
                this.AddProjection("System.Runtime.WindowsRuntime", "Windows.Foundation", "Size", "Windows.Foundation", "Size");
                this.AddProjection("System.Runtime.WindowsRuntime", "Windows.UI", "Color", "Windows.UI", "Color");
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml", "CornerRadius", "Windows.UI.Xaml", "CornerRadius");
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml", "Duration", "Windows.UI.Xaml", "Duration");
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml", "DurationType", "Windows.UI.Xaml", "DurationType");
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml", "GridLength", "Windows.UI.Xaml", "GridLength");
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml", "GridUnitType", "Windows.UI.Xaml", "GridUnitType");
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml", "Thickness", "Windows.UI.Xaml", "Thickness");
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml.Controls.Primitives", "GeneratorPosition", "Windows.UI.Xaml.Controls.Primitives", "GeneratorPosition");
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml.Media", "Matrix", "Windows.UI.Xaml.Media", "Matrix");
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml.Media.Animation", "RepeatBehavior", "Windows.UI.Xaml.Media.Animation", "RepeatBehavior");
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml.Media.Animation", "RepeatBehaviorType", "Windows.UI.Xaml.Media.Animation", "RepeatBehaviorType");
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml.Media.Animation", "KeyTime", "Windows.UI.Xaml.Media.Animation", "KeyTime");
                this.AddProjection("System.Runtime.WindowsRuntime.UI.Xaml", "Windows.UI.Xaml.Media.Media3D", "Matrix3D", "Windows.UI.Xaml.Media.Media3D", "Matrix3D");
                this.AddProjection("System.Numerics.Vectors", "System.Numerics", "Matrix3x2", "Windows.Foundation.Numerics", "Matrix3x2");
                this.AddProjection("System.Numerics.Vectors", "System.Numerics", "Matrix4x4", "Windows.Foundation.Numerics", "Matrix4x4");
                this.AddProjection("System.Numerics.Vectors", "System.Numerics", "Plane", "Windows.Foundation.Numerics", "Plane");
                this.AddProjection("System.Numerics.Vectors", "System.Numerics", "Quaternion", "Windows.Foundation.Numerics", "Quaternion");
                this.AddProjection("System.Numerics.Vectors", "System.Numerics", "Vector2", "Windows.Foundation.Numerics", "Vector2");
                this.AddProjection("System.Numerics.Vectors", "System.Numerics", "Vector3", "Windows.Foundation.Numerics", "Vector3");
                this.AddProjection("System.Numerics.Vectors", "System.Numerics", "Vector4", "Windows.Foundation.Numerics", "Vector4");
            }
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

        public TypeReference ProjectToWindowsRuntime(TypeReference clrType)
        {
            TypeDefinition definition;
            if (this._clrTypeToWindowsRuntimeTypeMap.TryGetValue(clrType.Resolve(), out definition))
            {
                return Unity.IL2CPP.ILPreProcessor.TypeResolver.For(clrType).Resolve(definition);
            }
            return clrType;
        }
    }
}

