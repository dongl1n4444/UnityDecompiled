namespace Unity.IL2CPP.WindowsRuntime
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.ILPreProcessor;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    internal sealed class IteratorToEnumeratorAdapterTypeGenerator
    {
        private readonly TypeDefinition _adapterType;
        private readonly FieldDefinition _currentField;
        private readonly MethodReference _getCurrentMethod;
        private readonly MethodReference _getHasCurrentMethod;
        private readonly FieldDefinition _hadCurrentField;
        private readonly TypeReference _ienumeratorType;
        private readonly FieldDefinition _initializedField;
        private readonly MethodReference _invalidOperationExceptionConstructor;
        private readonly FieldDefinition _iteratorField;
        private readonly TypeReference _iteratorType;
        private readonly ModuleDefinition _module;
        private readonly MethodReference _moveNextMethod;
        private readonly Unity.IL2CPP.ILPreProcessor.TypeResolver _typeResolver;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache6;
        private const TypeAttributes AdapterTypeAttributes = (TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed);
        [Inject]
        public static INamingService Naming;
        [Inject]
        public static ITypeProviderService TypeProvider;

        public IteratorToEnumeratorAdapterTypeGenerator(ModuleDefinition module, TypeDefinition iteratorType, TypeDefinition ienumeratorType)
        {
            this._module = module;
            TypeReference fieldType = this._module.ImportReference(TypeProvider.ObjectTypeReference);
            this._iteratorType = module.ImportReference(iteratorType);
            this._ienumeratorType = module.ImportReference(ienumeratorType);
            string name = Naming.ForWindowsRuntimeAdapterTypeName(iteratorType, ienumeratorType);
            this._adapterType = new TypeDefinition("System.Runtime.InteropServices.WindowsRuntime", name, TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed, TypeProvider.ObjectTypeReference);
            if (ienumeratorType.HasGenericParameters)
            {
                GenericInstanceType type = new GenericInstanceType(this._iteratorType);
                GenericInstanceType type2 = new GenericInstanceType(this._ienumeratorType);
                foreach (GenericParameter parameter in ienumeratorType.GenericParameters)
                {
                    GenericParameter item = new GenericParameter(parameter.Name, this._adapterType);
                    this._adapterType.GenericParameters.Add(item);
                    type.GenericArguments.Add(item);
                    type2.GenericArguments.Add(item);
                }
                this._iteratorType = this._module.ImportReference(type, this._adapterType);
                this._ienumeratorType = this._module.ImportReference(type2, this._adapterType);
                fieldType = this._adapterType.GenericParameters[0];
            }
            this._iteratorField = new FieldDefinition("iterator", FieldAttributes.CompilerControlled | FieldAttributes.Private, this._iteratorType);
            this._initializedField = new FieldDefinition("initialized", FieldAttributes.CompilerControlled | FieldAttributes.Private, this._module.ImportReference(TypeProvider.BoolTypeReference));
            this._hadCurrentField = new FieldDefinition("hadCurrent", FieldAttributes.CompilerControlled | FieldAttributes.Private, this._module.ImportReference(TypeProvider.BoolTypeReference));
            this._currentField = new FieldDefinition("current", FieldAttributes.CompilerControlled | FieldAttributes.Private, fieldType);
            this._typeResolver = Unity.IL2CPP.ILPreProcessor.TypeResolver.For(this._iteratorType);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<MethodDefinition, bool>(IteratorToEnumeratorAdapterTypeGenerator.<IteratorToEnumeratorAdapterTypeGenerator>m__0);
            }
            this._getCurrentMethod = this._typeResolver.Resolve(iteratorType.Methods.First<MethodDefinition>(<>f__am$cache0));
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<MethodDefinition, bool>(IteratorToEnumeratorAdapterTypeGenerator.<IteratorToEnumeratorAdapterTypeGenerator>m__1);
            }
            this._getHasCurrentMethod = this._typeResolver.Resolve(iteratorType.Methods.First<MethodDefinition>(<>f__am$cache1));
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = new Func<MethodDefinition, bool>(IteratorToEnumeratorAdapterTypeGenerator.<IteratorToEnumeratorAdapterTypeGenerator>m__2);
            }
            this._moveNextMethod = this._typeResolver.Resolve(iteratorType.Methods.First<MethodDefinition>(<>f__am$cache2));
            TypeDefinition definition = TypeProvider.Corlib.MainModule.GetType("System", "InvalidOperationException");
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<MethodDefinition, bool>(IteratorToEnumeratorAdapterTypeGenerator.<IteratorToEnumeratorAdapterTypeGenerator>m__3);
            }
            this._invalidOperationExceptionConstructor = definition.Methods.First<MethodDefinition>(<>f__am$cache3);
        }

        [CompilerGenerated]
        private static bool <IteratorToEnumeratorAdapterTypeGenerator>m__0(MethodDefinition m) => 
            (m.Name == "get_Current");

        [CompilerGenerated]
        private static bool <IteratorToEnumeratorAdapterTypeGenerator>m__1(MethodDefinition m) => 
            (m.Name == "get_HasCurrent");

        [CompilerGenerated]
        private static bool <IteratorToEnumeratorAdapterTypeGenerator>m__2(MethodDefinition m) => 
            (m.Name == "MoveNext");

        [CompilerGenerated]
        private static bool <IteratorToEnumeratorAdapterTypeGenerator>m__3(MethodDefinition m) => 
            (((m.IsConstructor && !m.IsStatic) && (m.Parameters.Count == 1)) && (m.Parameters[0].ParameterType.MetadataType == MetadataType.String));

        private TypeDefinition CreateAdapterType(TypeDefinition iteratorType, TypeDefinition ienumeratorType)
        {
            throw new NotImplementedException();
        }

        public TypeDefinition Generate()
        {
            this._module.Types.Add(this._adapterType);
            InterfaceUtilities.MakeImplementInterface(this._adapterType, this._ienumeratorType);
            this._adapterType.Fields.Add(this._iteratorField);
            this._adapterType.Fields.Add(this._initializedField);
            this._adapterType.Fields.Add(this._hadCurrentField);
            this._adapterType.Fields.Add(this._currentField);
            foreach (MethodDefinition definition in this._adapterType.Methods)
            {
                definition.Attributes = (MethodAttributes) ((ushort) (definition.Attributes & (MethodAttributes.Assembly | MethodAttributes.CheckAccessOnOverride | MethodAttributes.Family | MethodAttributes.Final | MethodAttributes.HasSecurity | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.PInvokeImpl | MethodAttributes.RequireSecObject | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static | MethodAttributes.UnmanagedExport | MethodAttributes.Virtual)));
                switch (definition.Name)
                {
                    case "System.Collections.IEnumerator.MoveNext":
                    {
                        this.WriteMethodMoveNext(definition);
                        continue;
                    }
                    case "System.Collections.IEnumerator.get_Current":
                    case "System.Collections.Generic.IEnumerator`1.get_Current":
                    {
                        this.WriteMethodGetCurrent(definition);
                        continue;
                    }
                    case "System.Collections.IEnumerator.Reset":
                    {
                        this.WriteMethodReset(definition);
                        continue;
                    }
                    case "System.IDisposable.Dispose":
                    {
                        this.WriteDisposeMethod(definition);
                        continue;
                    }
                }
                throw new NotSupportedException($"Interface '{this._ienumeratorType.FullName}' contains unsupported method '{definition.Name}'.");
            }
            MethodDefinition item = new MethodDefinition(".ctor", MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName, this._module.ImportReference(TypeProvider.SystemVoid));
            this._adapterType.Methods.Add(item);
            item.Parameters.Add(new ParameterDefinition("iterator", ParameterAttributes.None, this._iteratorType));
            this.WriteConstructor(item);
            return this._adapterType;
        }

        private void WriteConstructor(MethodDefinition method)
        {
            MethodBody self = method.Body;
            ILProcessor iLProcessor = self.GetILProcessor();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = m => (m.IsConstructor && !m.IsStatic) && !m.HasParameters;
            }
            MethodDefinition definition = TypeProvider.SystemObject.Methods.Single<MethodDefinition>(<>f__am$cache4);
            iLProcessor.Emit(OpCodes.Call, this._module.ImportReference(definition));
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldarg_1);
            iLProcessor.Emit(OpCodes.Stfld, this._typeResolver.Resolve(this._iteratorField));
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldc_I4_1);
            iLProcessor.Emit(OpCodes.Stfld, this._hadCurrentField);
            iLProcessor.Emit(OpCodes.Ret);
            self.OptimizeMacros();
        }

        private void WriteDisposeMethod(MethodDefinition method)
        {
            MethodBody self = method.Body;
            self.GetILProcessor().Emit(OpCodes.Ret);
            self.OptimizeMacros();
        }

        private void WriteMethodGetCurrent(MethodDefinition method)
        {
            MethodBody self = method.Body;
            ILProcessor iLProcessor = self.GetILProcessor();
            Instruction target = Instruction.Create(OpCodes.Nop);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldfld, this._initializedField);
            iLProcessor.Emit(OpCodes.Brtrue_S, target);
            Instruction instruction2 = self.Instructions.Last<Instruction>();
            iLProcessor.Emit(OpCodes.Ldstr, "Enumeration has not started. Call MoveNext.");
            iLProcessor.Emit(OpCodes.Newobj, this._module.ImportReference(this._invalidOperationExceptionConstructor));
            iLProcessor.Emit(OpCodes.Throw);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            instruction2.Operand = self.Instructions.Last<Instruction>();
            iLProcessor.Emit(OpCodes.Ldfld, this._hadCurrentField);
            iLProcessor.Emit(OpCodes.Brtrue_S, target);
            Instruction instruction3 = self.Instructions.Last<Instruction>();
            iLProcessor.Emit(OpCodes.Ldstr, "Enumeration already finished.");
            iLProcessor.Emit(OpCodes.Newobj, this._module.ImportReference(this._invalidOperationExceptionConstructor));
            iLProcessor.Emit(OpCodes.Throw);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            instruction3.Operand = self.Instructions.Last<Instruction>();
            iLProcessor.Emit(OpCodes.Ldfld, this._typeResolver.Resolve(this._currentField));
            if (this._currentField.FieldType.IsGenericParameter && (method.ReturnType.MetadataType == MetadataType.Object))
            {
                iLProcessor.Emit(OpCodes.Box, this._currentField.FieldType);
            }
            iLProcessor.Emit(OpCodes.Ret);
            self.OptimizeMacros();
        }

        private void WriteMethodMoveNext(MethodDefinition method)
        {
            MethodBody self = method.Body;
            ILProcessor iLProcessor = self.GetILProcessor();
            Instruction target = Instruction.Create(OpCodes.Nop);
            ExceptionHandler item = new ExceptionHandler(ExceptionHandlerType.Catch) {
                CatchType = this._module.ImportReference(TypeProvider.Corlib.MainModule.GetType("System", "Exception"))
            };
            method.Body.ExceptionHandlers.Add(item);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldfld, this._hadCurrentField);
            iLProcessor.Emit(OpCodes.Brtrue_S, target);
            Instruction instruction2 = self.Instructions.Last<Instruction>();
            iLProcessor.Emit(OpCodes.Ldc_I4_0);
            iLProcessor.Emit(OpCodes.Ret);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            Instruction instruction3 = self.Instructions.Last<Instruction>();
            item.TryStart = instruction3;
            instruction2.Operand = instruction3;
            iLProcessor.Emit(OpCodes.Ldfld, this._initializedField);
            iLProcessor.Emit(OpCodes.Brtrue_S, target);
            Instruction instruction4 = self.Instructions.Last<Instruction>();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldfld, this._typeResolver.Resolve(this._iteratorField));
            iLProcessor.Emit(OpCodes.Callvirt, this._module.ImportReference(this._getHasCurrentMethod, this._adapterType));
            iLProcessor.Emit(OpCodes.Stfld, this._hadCurrentField);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldc_I4_1);
            iLProcessor.Emit(OpCodes.Stfld, this._initializedField);
            iLProcessor.Emit(OpCodes.Br_S, target);
            Instruction instruction5 = self.Instructions.Last<Instruction>();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            instruction4.Operand = self.Instructions.Last<Instruction>();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldfld, this._typeResolver.Resolve(this._iteratorField));
            iLProcessor.Emit(OpCodes.Callvirt, this._module.ImportReference(this._moveNextMethod, this._adapterType));
            iLProcessor.Emit(OpCodes.Stfld, this._hadCurrentField);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            instruction5.Operand = self.Instructions.Last<Instruction>();
            iLProcessor.Emit(OpCodes.Ldfld, this._hadCurrentField);
            iLProcessor.Emit(OpCodes.Brfalse_S, target);
            Instruction instruction6 = self.Instructions.Last<Instruction>();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldfld, this._typeResolver.Resolve(this._iteratorField));
            iLProcessor.Emit(OpCodes.Callvirt, this._module.ImportReference(this._getCurrentMethod, this._adapterType));
            iLProcessor.Emit(OpCodes.Stfld, this._typeResolver.Resolve(this._currentField));
            iLProcessor.Emit(OpCodes.Leave_S, target);
            Instruction instruction7 = self.Instructions.Last<Instruction>();
            instruction6.Operand = instruction7;
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = m => m.Name == "GetHRForException";
            }
            MethodDefinition definition2 = TypeProvider.Corlib.MainModule.GetType("System.Runtime.InteropServices", "Marshal").Methods.Single<MethodDefinition>(<>f__am$cache5);
            iLProcessor.Emit(OpCodes.Call, this._module.ImportReference(definition2));
            instruction3 = self.Instructions.Last<Instruction>();
            item.HandlerStart = instruction3;
            item.TryEnd = instruction3;
            iLProcessor.Emit(OpCodes.Ldc_I4, -2147483636);
            iLProcessor.Emit(OpCodes.Bne_Un_S, target);
            Instruction instruction8 = self.Instructions.Last<Instruction>();
            iLProcessor.Emit(OpCodes.Ldstr, "Collection was modified; enumeration operation may not execute.");
            iLProcessor.Emit(OpCodes.Newobj, this._invalidOperationExceptionConstructor);
            iLProcessor.Emit(OpCodes.Throw);
            iLProcessor.Emit(OpCodes.Rethrow);
            instruction8.Operand = self.Instructions.Last<Instruction>();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            instruction3 = self.Instructions.Last<Instruction>();
            item.HandlerEnd = instruction3;
            instruction7.Operand = instruction3;
            iLProcessor.Emit(OpCodes.Ldfld, this._hadCurrentField);
            iLProcessor.Emit(OpCodes.Ret);
            self.OptimizeMacros();
        }

        private void WriteMethodReset(MethodDefinition method)
        {
            MethodBody self = method.Body;
            ILProcessor iLProcessor = self.GetILProcessor();
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = m => (m.IsConstructor && !m.IsStatic) && !m.HasParameters;
            }
            MethodDefinition definition2 = TypeProvider.Corlib.MainModule.GetType("System", "NotSupportedException").Methods.First<MethodDefinition>(<>f__am$cache6);
            iLProcessor.Emit(OpCodes.Newobj, this._module.ImportReference(definition2));
            iLProcessor.Emit(OpCodes.Throw);
            self.OptimizeMacros();
        }
    }
}

