namespace Unity.IL2CPP.StackAnalysis
{
    using Mono.Cecil;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.IoCServices;

    public class TypeProviderComponent : ITypeProviderService, ITypeProviderInitializerService, IDisposable
    {
        private TypeDefinition _iActivationFactoryType;
        private TypeDefinition _il2cppComDelegateType;
        private TypeDefinition _il2cppComObjectType;
        private AssemblyDefinition _mscorlib;
        private TypeReference _nativeIntType;
        private TypeReference _nativeUIntType;
        private TypeReference _runtimeFieldHandleType;
        private TypeReference _runtimeMethodHandleType;
        private TypeReference _runtimeTypeHandleType;
        [CompilerGenerated]
        private static Func<TypeDefinition, bool> <>f__am$cache0;

        public void Dispose()
        {
            this._mscorlib = null;
            this._nativeIntType = null;
            this._nativeUIntType = null;
            this._runtimeTypeHandleType = null;
            this._runtimeMethodHandleType = null;
            this._runtimeFieldHandleType = null;
        }

        public void Initialize(AssemblyDefinition mscorlib)
        {
            this._mscorlib = mscorlib;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<TypeDefinition, bool>(null, (IntPtr) <Initialize>m__0);
            }
            TypeDefinition baseType = mscorlib.MainModule.Types.Single<TypeDefinition>(<>f__am$cache0);
            this._nativeIntType = new TypeDefinition(string.Empty, "intptr_t", TypeAttributes.AnsiClass, baseType);
            this._nativeUIntType = new TypeDefinition(string.Empty, "uintptr_t", TypeAttributes.AnsiClass, baseType);
            this._iActivationFactoryType = new TypeDefinition(string.Empty, "IActivationFactory", TypeAttributes.AnsiClass, this.SystemObject);
            this._iActivationFactoryType.IsInterface = true;
            TypeAttributes attributes = TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit;
            this._il2cppComObjectType = new TypeDefinition("System", "__Il2CppComObject", attributes, this.SystemObject);
            mscorlib.MainModule.Types.Add(this._il2cppComObjectType);
            TypeAttributes attributes2 = TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed;
            this._il2cppComDelegateType = new TypeDefinition("System", "__Il2CppComDelegate", attributes2, this._il2cppComObjectType);
            mscorlib.MainModule.Types.Add(this._il2cppComDelegateType);
            foreach (TypeDefinition definition2 in mscorlib.MainModule.Types)
            {
                if (definition2.FullName == "System.RuntimeTypeHandle")
                {
                    this._runtimeTypeHandleType = definition2;
                }
                if (definition2.FullName == "System.RuntimeMethodHandle")
                {
                    this._runtimeMethodHandleType = definition2;
                }
                if (definition2.FullName == "System.RuntimeFieldHandle")
                {
                    this._runtimeFieldHandleType = definition2;
                }
            }
        }

        public TypeReference BoolTypeReference =>
            this._mscorlib.MainModule.TypeSystem.Boolean;

        public TypeReference ByteTypeReference =>
            this._mscorlib.MainModule.TypeSystem.Byte;

        public TypeReference CharTypeReference =>
            this._mscorlib.MainModule.TypeSystem.Char;

        public AssemblyDefinition Corlib =>
            this._mscorlib;

        public TypeReference DoubleTypeReference =>
            this._mscorlib.MainModule.TypeSystem.Double;

        public TypeReference IActivationFactoryTypeReference =>
            this._iActivationFactoryType;

        public TypeReference Il2CppComDelegateTypeReference =>
            this._il2cppComDelegateType;

        public TypeReference Il2CppComObjectTypeReference =>
            this._il2cppComObjectType;

        public TypeReference Int16TypeReference =>
            this._mscorlib.MainModule.TypeSystem.Int16;

        public TypeReference Int32TypeReference =>
            this._mscorlib.MainModule.TypeSystem.Int32;

        public TypeReference Int64TypeReference =>
            this._mscorlib.MainModule.TypeSystem.Int64;

        public TypeReference IntPtrTypeReference =>
            this._mscorlib.MainModule.TypeSystem.IntPtr;

        public TypeReference NativeIntTypeReference =>
            this._nativeIntType;

        public TypeReference NativeUIntTypeReference =>
            this._nativeUIntType;

        public TypeReference ObjectTypeReference =>
            this._mscorlib.MainModule.TypeSystem.Object;

        public TypeReference RuntimeFieldHandleTypeReference =>
            this._runtimeFieldHandleType;

        public TypeReference RuntimeMethodHandleTypeReference =>
            this._runtimeMethodHandleType;

        public TypeReference RuntimeTypeHandleTypeReference =>
            this._runtimeTypeHandleType;

        public TypeReference SByteTypeReference =>
            this._mscorlib.MainModule.TypeSystem.SByte;

        public TypeReference SingleTypeReference =>
            this._mscorlib.MainModule.TypeSystem.Single;

        public TypeReference StringTypeReference =>
            this._mscorlib.MainModule.TypeSystem.String;

        public TypeDefinition SystemArray =>
            this._mscorlib.MainModule.GetType("System.Array");

        public TypeDefinition SystemByte =>
            this._mscorlib.MainModule.GetType("System.Byte");

        public TypeDefinition SystemDelegate =>
            this._mscorlib.MainModule.GetType("System.Delegate");

        public TypeDefinition SystemException =>
            this._mscorlib.MainModule.GetType("System.Exception");

        public TypeDefinition SystemIntPtr =>
            this._mscorlib.MainModule.GetType("System.IntPtr");

        public TypeDefinition SystemMulticastDelegate =>
            this._mscorlib.MainModule.GetType("System.MulticastDelegate");

        public TypeDefinition SystemNullable =>
            this._mscorlib.MainModule.GetType("System.Nullable`1");

        public TypeDefinition SystemObject =>
            this._mscorlib.MainModule.GetType("System.Object");

        public TypeDefinition SystemString =>
            this._mscorlib.MainModule.GetType("System.String");

        public TypeDefinition SystemUInt16 =>
            this._mscorlib.MainModule.GetType("System.UInt16");

        public TypeDefinition SystemUIntPtr =>
            this._mscorlib.MainModule.GetType("System.UIntPtr");

        public TypeReference UInt16TypeReference =>
            this._mscorlib.MainModule.TypeSystem.UInt16;

        public TypeReference UInt32TypeReference =>
            this._mscorlib.MainModule.TypeSystem.UInt32;

        public TypeReference UInt64TypeReference =>
            this._mscorlib.MainModule.TypeSystem.UInt64;

        public TypeReference UIntPtrTypeReference =>
            this._mscorlib.MainModule.TypeSystem.UIntPtr;
    }
}

