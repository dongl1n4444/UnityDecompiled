using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.IL2CPP.Portability;

namespace Unity.IL2CPP.IoC
{
	public class Container : IDisposable
	{
		private class SingletonInfo
		{
			private readonly Type _concreteType;

			private readonly List<FieldInfo> _instanceFields = new List<FieldInfo>();

			private object _instance;

			public object Instance
			{
				get
				{
					return this._instance;
				}
			}

			public Type ConcreteType
			{
				get
				{
					return this._concreteType;
				}
			}

			public List<FieldInfo> InstanceFields
			{
				get
				{
					return this._instanceFields;
				}
			}

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
		}

		private readonly Dictionary<Type, Container.SingletonInfo> _serviceMap = new Dictionary<Type, Container.SingletonInfo>();

		public void BindSingleton<TService, TConcrete>() where TConcrete : TService
		{
			this.BindSingleton(typeof(TService), typeof(TConcrete));
		}

		public void BindSingleton(Type serviceType, Type concreteType)
		{
			this._serviceMap.Add(serviceType, new Container.SingletonInfo(concreteType));
		}

		public void BindMultiSingleton(Type[] serviceTypes, Type concreteType)
		{
			Container.SingletonInfo value = new Container.SingletonInfo(concreteType);
			for (int i = 0; i < serviceTypes.Length; i++)
			{
				Type key = serviceTypes[i];
				this._serviceMap.Add(key, value);
			}
		}

		public void Install(Assembly assembly)
		{
			this.Install(new Assembly[]
			{
				assembly
			});
		}

		public void Install(Assembly[] assemblies)
		{
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				Module[] modulesPortable = assembly.GetModulesPortable();
				for (int j = 0; j < modulesPortable.Length; j++)
				{
					Module module = modulesPortable[j];
					this.Install(module.GetTypesPortable());
				}
			}
		}

		public void Install(Type type)
		{
			this.Install(new Type[]
			{
				type
			});
		}

		public void Install(Type[] types)
		{
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				this.InstallOnType(type);
			}
		}

		private void InstallOnType(Type type)
		{
			Type[] nestedTypesPortable = type.GetNestedTypesPortable();
			for (int i = 0; i < nestedTypesPortable.Length; i++)
			{
				Type type2 = nestedTypesPortable[i];
				this.InstallOnType(type2);
			}
			FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int j = 0; j < fields.Length; j++)
			{
				FieldInfo fieldInfo = fields[j];
				object[] customAttributes = fieldInfo.GetCustomAttributes(true);
				for (int k = 0; k < customAttributes.Length; k++)
				{
					object obj = customAttributes[k];
					if (obj is InjectAttribute)
					{
						Type fieldType = fieldInfo.FieldType;
						Container.SingletonInfo singletonInfo;
						if (!this._serviceMap.TryGetValue(fieldType, out singletonInfo))
						{
							throw new ArgumentException(string.Format("Field {0} cannot be injected because type is not bound", fieldType));
						}
						fieldInfo.SetValue(null, singletonInfo.CreateInstanceFor(fieldInfo));
					}
				}
			}
		}

		public void Dispose()
		{
			HashSet<IDisposable> hashSet = new HashSet<IDisposable>();
			foreach (KeyValuePair<Type, Container.SingletonInfo> current in this._serviceMap)
			{
				IDisposable disposable = current.Value.Instance as IDisposable;
				if (disposable != null)
				{
					hashSet.Add(disposable);
				}
				foreach (FieldInfo current2 in current.Value.InstanceFields)
				{
					current2.SetValue(null, null);
				}
			}
			foreach (IDisposable current3 in hashSet)
			{
				current3.Dispose();
			}
			this._serviceMap.Clear();
		}
	}
}
