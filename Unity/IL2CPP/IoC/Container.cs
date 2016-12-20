namespace Unity.IL2CPP.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Unity.IL2CPP.Portability;

    public class Container : IDisposable
    {
        private readonly Dictionary<Type, SingletonInfo> _serviceMap = new Dictionary<Type, SingletonInfo>();

        public void BindMultiSingleton(Type[] serviceTypes, Type concreteType)
        {
            SingletonInfo info = new SingletonInfo(concreteType);
            foreach (Type type in serviceTypes)
            {
                this._serviceMap.Add(type, info);
            }
        }

        public void BindSingleton<TService, TConcrete>() where TConcrete: TService
        {
            this.BindSingleton(typeof(TService), typeof(TConcrete));
        }

        public void BindSingleton(Type serviceType, Type concreteType)
        {
            this._serviceMap.Add(serviceType, new SingletonInfo(concreteType));
        }

        public void Dispose()
        {
            HashSet<IDisposable> set = new HashSet<IDisposable>();
            foreach (KeyValuePair<Type, SingletonInfo> pair in this._serviceMap)
            {
                IDisposable instance = pair.Value.Instance as IDisposable;
                if (instance != null)
                {
                    set.Add(instance);
                }
                foreach (FieldInfo info in pair.Value.InstanceFields)
                {
                    info.SetValue(null, null);
                }
            }
            foreach (IDisposable disposable2 in set)
            {
                disposable2.Dispose();
            }
            this._serviceMap.Clear();
        }

        public void Install(Assembly assembly)
        {
            Assembly[] assemblies = new Assembly[] { assembly };
            this.Install(assemblies);
        }

        public void Install(Assembly[] assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                foreach (Module module in AssemblyExtensions.GetModulesPortable(assembly))
                {
                    this.Install(ModuleExtensions.GetTypesPortable(module));
                }
            }
        }

        public void Install(Type type)
        {
            Type[] types = new Type[] { type };
            this.Install(types);
        }

        public void Install(Type[] types)
        {
            foreach (Type type in types)
            {
                this.InstallOnType(type);
            }
        }

        private void InstallOnType(Type type)
        {
            foreach (Type type2 in TypeExtensions.GetNestedTypesPortable(type))
            {
                this.InstallOnType(type2);
            }
            foreach (FieldInfo info in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
            {
                foreach (object obj2 in info.GetCustomAttributes(true))
                {
                    if (obj2 is InjectAttribute)
                    {
                        SingletonInfo info2;
                        Type fieldType = info.FieldType;
                        if (!this._serviceMap.TryGetValue(fieldType, out info2))
                        {
                            throw new ArgumentException(string.Format("Field {0} cannot be injected because type is not bound", fieldType));
                        }
                        info.SetValue(null, info2.CreateInstanceFor(info));
                    }
                }
            }
        }

        private class SingletonInfo
        {
            private readonly Type _concreteType;
            private object _instance;
            private readonly List<FieldInfo> _instanceFields = new List<FieldInfo>();

            public SingletonInfo(Type concreteType)
            {
                this._concreteType = concreteType;
            }

            public object CreateInstanceFor(FieldInfo fieldInfo)
            {
                if (this._instance == null)
                {
                    this._instance = Activator.CreateInstance(this.ConcreteType);
                }
                this._instanceFields.Add(fieldInfo);
                return this._instance;
            }

            public Type ConcreteType
            {
                get
                {
                    return this._concreteType;
                }
            }

            public object Instance
            {
                get
                {
                    return this._instance;
                }
            }

            public List<FieldInfo> InstanceFields
            {
                get
                {
                    return this._instanceFields;
                }
            }
        }
    }
}

