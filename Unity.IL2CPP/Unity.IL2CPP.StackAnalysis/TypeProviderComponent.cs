using Mono.Cecil;
using System;
using System.Linq;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.StackAnalysis
{
	public class TypeProviderComponent : ITypeProviderService, ITypeProviderInitializerService, IDisposable
	{
		private AssemblyDefinition _mscorlib;

		private TypeReference _nativeIntType;

		private TypeReference _nativeUIntType;

		private TypeReference _runtimeTypeHandleType;

		private TypeReference _runtimeMethodHandleType;

		private TypeReference _runtimeFieldHandleType;

		private TypeDefinition _iActivationFactoryType;

		private TypeDefinition _il2cppComObjectType;

		public AssemblyDefinition Corlib
		{
			get
			{
				return this._mscorlib;
			}
		}

		public TypeDefinition SystemObject
		{
			get
			{
				return this._mscorlib.MainModule.GetType("System.Object");
			}
		}

		public TypeDefinition SystemString
		{
			get
			{
				return this._mscorlib.MainModule.GetType("System.String");
			}
		}

		public TypeDefinition SystemArray
		{
			get
			{
				return this._mscorlib.MainModule.GetType("System.Array");
			}
		}

		public TypeDefinition SystemException
		{
			get
			{
				return this._mscorlib.MainModule.GetType("System.Exception");
			}
		}

		public TypeDefinition SystemDelegate
		{
			get
			{
				return this._mscorlib.MainModule.GetType("System.Delegate");
			}
		}

		public TypeDefinition SystemMulticastDelegate
		{
			get
			{
				return this._mscorlib.MainModule.GetType("System.MulticastDelegate");
			}
		}

		public TypeDefinition SystemByte
		{
			get
			{
				return this._mscorlib.MainModule.GetType("System.Byte");
			}
		}

		public TypeDefinition SystemUInt16
		{
			get
			{
				return this._mscorlib.MainModule.GetType("System.UInt16");
			}
		}

		public TypeDefinition SystemIntPtr
		{
			get
			{
				return this._mscorlib.MainModule.GetType("System.IntPtr");
			}
		}

		public TypeDefinition SystemUIntPtr
		{
			get
			{
				return this._mscorlib.MainModule.GetType("System.UIntPtr");
			}
		}

		public TypeDefinition SystemNullable
		{
			get
			{
				return this._mscorlib.MainModule.GetType("System.Nullable`1");
			}
		}

		public TypeReference NativeIntTypeReference
		{
			get
			{
				return this._nativeIntType;
			}
		}

		public TypeReference NativeUIntTypeReference
		{
			get
			{
				return this._nativeUIntType;
			}
		}

		public TypeReference Int32TypeReference
		{
			get
			{
				return this._mscorlib.MainModule.TypeSystem.Int32;
			}
		}

		public TypeReference Int16TypeReference
		{
			get
			{
				return this._mscorlib.MainModule.TypeSystem.Int16;
			}
		}

		public TypeReference UInt16TypeReference
		{
			get
			{
				return this._mscorlib.MainModule.TypeSystem.UInt16;
			}
		}

		public TypeReference SByteTypeReference
		{
			get
			{
				return this._mscorlib.MainModule.TypeSystem.SByte;
			}
		}

		public TypeReference ByteTypeReference
		{
			get
			{
				return this._mscorlib.MainModule.TypeSystem.Byte;
			}
		}

		public TypeReference IntPtrTypeReference
		{
			get
			{
				return this._mscorlib.MainModule.TypeSystem.IntPtr;
			}
		}

		public TypeReference UIntPtrTypeReference
		{
			get
			{
				return this._mscorlib.MainModule.TypeSystem.UIntPtr;
			}
		}

		public TypeReference Int64TypeReference
		{
			get
			{
				return this._mscorlib.MainModule.TypeSystem.Int64;
			}
		}

		public TypeReference UInt32TypeReference
		{
			get
			{
				return this._mscorlib.MainModule.TypeSystem.UInt32;
			}
		}

		public TypeReference UInt64TypeReference
		{
			get
			{
				return this._mscorlib.MainModule.TypeSystem.UInt64;
			}
		}

		public TypeReference SingleTypeReference
		{
			get
			{
				return this._mscorlib.MainModule.TypeSystem.Single;
			}
		}

		public TypeReference DoubleTypeReference
		{
			get
			{
				return this._mscorlib.MainModule.TypeSystem.Double;
			}
		}

		public TypeReference ObjectTypeReference
		{
			get
			{
				return this._mscorlib.MainModule.TypeSystem.Object;
			}
		}

		public TypeReference StringTypeReference
		{
			get
			{
				return this._mscorlib.MainModule.TypeSystem.String;
			}
		}

		public TypeReference RuntimeTypeHandleTypeReference
		{
			get
			{
				return this._runtimeTypeHandleType;
			}
		}

		public TypeReference RuntimeMethodHandleTypeReference
		{
			get
			{
				return this._runtimeMethodHandleType;
			}
		}

		public TypeReference RuntimeFieldHandleTypeReference
		{
			get
			{
				return this._runtimeFieldHandleType;
			}
		}

		public TypeReference IActivationFactoryTypeReference
		{
			get
			{
				return this._iActivationFactoryType;
			}
		}

		public TypeReference Il2CppComObjectTypeReference
		{
			get
			{
				return this._il2cppComObjectType;
			}
		}

		public void Initialize(AssemblyDefinition mscorlib)
		{
			this._mscorlib = mscorlib;
			TypeDefinition baseType = mscorlib.MainModule.Types.Single((TypeDefinition t) => t.FullName == "System.ValueType");
			this._nativeIntType = new TypeDefinition(string.Empty, "intptr_t", TypeAttributes.NotPublic, baseType);
			this._nativeUIntType = new TypeDefinition(string.Empty, "uintptr_t", TypeAttributes.NotPublic, baseType);
			this._iActivationFactoryType = new TypeDefinition(string.Empty, "IActivationFactory", TypeAttributes.NotPublic, this.SystemObject);
			this._iActivationFactoryType.IsInterface = true;
			TypeAttributes attributes = TypeAttributes.BeforeFieldInit;
			this._il2cppComObjectType = new TypeDefinition("System", "__Il2CppComObject", attributes, this.SystemObject);
			mscorlib.MainModule.Types.Add(this._il2cppComObjectType);
			foreach (TypeDefinition current in mscorlib.MainModule.Types)
			{
				if (current.FullName == "System.RuntimeTypeHandle")
				{
					this._runtimeTypeHandleType = current;
				}
				if (current.FullName == "System.RuntimeMethodHandle")
				{
					this._runtimeMethodHandleType = current;
				}
				if (current.FullName == "System.RuntimeFieldHandle")
				{
					this._runtimeFieldHandleType = current;
				}
			}
		}

		public void Dispose()
		{
			this._mscorlib = null;
			this._nativeIntType = null;
			this._nativeUIntType = null;
			this._runtimeTypeHandleType = null;
			this._runtimeMethodHandleType = null;
			this._runtimeFieldHandleType = null;
		}
	}
}
