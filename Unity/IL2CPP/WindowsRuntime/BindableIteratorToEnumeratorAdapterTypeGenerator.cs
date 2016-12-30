namespace Unity.IL2CPP.WindowsRuntime
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using Mono.Collections.Generic;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    internal sealed class BindableIteratorToEnumeratorAdapterTypeGenerator
    {
        private FieldDefinition _currentField;
        private MethodReference _getCurrentMethod;
        private MethodReference _getHasCurrentMethod;
        private FieldDefinition _hadCurrentField;
        private FieldDefinition _initializedField;
        private MethodReference _invalidOperationExceptionConstructor;
        private FieldDefinition _iteratorField;
        private readonly ModuleDefinition _module;
        private MethodReference _moveNextMethod;
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
        [Inject]
        public static ITypeProviderService TypeProvider;

        private BindableIteratorToEnumeratorAdapterTypeGenerator(ModuleDefinition module)
        {
            this._module = module;
        }

        private TypeDefinition Generate()
        {
            TypeDefinition type = TypeProvider.IBindableIteratorTypeReference?.Resolve();
            if (type == null)
            {
                throw new InvalidOperationException("BindableIteratorToEnumeratorAdapterTypeGenerator.Generate() should't be called if IBindableIterator is not available!");
            }
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = m => m.Name == "get_Current";
            }
            this._getCurrentMethod = type.Methods.First<MethodDefinition>(<>f__am$cache0);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = m => m.Name == "get_HasCurrent";
            }
            this._getHasCurrentMethod = type.Methods.First<MethodDefinition>(<>f__am$cache1);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = m => m.Name == "MoveNext";
            }
            this._moveNextMethod = type.Methods.First<MethodDefinition>(<>f__am$cache2);
            TypeDefinition definition2 = TypeProvider.Corlib.MainModule.GetType("System", "InvalidOperationException");
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = m => ((m.IsConstructor && !m.IsStatic) && (m.Parameters.Count == 1)) && (m.Parameters[0].ParameterType.MetadataType == MetadataType.String);
            }
            this._invalidOperationExceptionConstructor = definition2.Methods.First<MethodDefinition>(<>f__am$cache3);
            TypeDefinition item = new TypeDefinition("System.Runtime.InteropServices.WindowsRuntime", "BindableIteratorToEnumeratorAdapter", TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit, this._module.ImportReference(TypeProvider.SystemObject));
            this._module.Types.Add(item);
            TypeDefinition definition4 = TypeProvider.Corlib.MainModule.GetType("System.Collections", "IEnumerator");
            item.Interfaces.Add(new InterfaceImplementation(this._module.ImportReference(definition4)));
            this._iteratorField = new FieldDefinition("iterator", FieldAttributes.CompilerControlled | FieldAttributes.Private, this._module.ImportReference(type));
            item.Fields.Add(this._iteratorField);
            this._initializedField = new FieldDefinition("initialized", FieldAttributes.CompilerControlled | FieldAttributes.Private, this._module.ImportReference(TypeProvider.BoolTypeReference));
            item.Fields.Add(this._initializedField);
            this._hadCurrentField = new FieldDefinition("hadCurrent", FieldAttributes.CompilerControlled | FieldAttributes.Private, this._module.ImportReference(TypeProvider.BoolTypeReference));
            item.Fields.Add(this._hadCurrentField);
            this._currentField = new FieldDefinition("current", FieldAttributes.CompilerControlled | FieldAttributes.Private, this._module.ImportReference(TypeProvider.ObjectTypeReference));
            item.Fields.Add(this._currentField);
            MethodDefinition definition5 = new MethodDefinition(".ctor", MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName, this._module.ImportReference(TypeProvider.SystemVoid));
            item.Methods.Add(definition5);
            definition5.Parameters.Add(new ParameterDefinition("iterator", ParameterAttributes.None, this._module.ImportReference(type)));
            this.WriteConstructor(definition5);
            foreach (MethodDefinition definition6 in definition4.Methods)
            {
                MethodDefinition definition7 = new MethodDefinition(definition6.Name, MethodAttributes.CompilerControlled | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, this._module.ImportReference(definition6.ReturnType));
                item.Methods.Add(definition7);
                definition7.Overrides.Add(this._module.ImportReference(definition6));
                foreach (ParameterDefinition definition8 in definition6.Parameters)
                {
                    definition7.Parameters.Add(new ParameterDefinition(definition8.Name, definition8.Attributes, this._module.ImportReference(definition8.ParameterType)));
                }
                switch (definition6.Name)
                {
                    case "MoveNext":
                    {
                        this.WriteMethodMoveNext(definition7);
                        continue;
                    }
                    case "get_Current":
                    {
                        this.WriteMethodGetCurrent(definition7);
                        continue;
                    }
                    case "Reset":
                    {
                        this.WriteMethodReset(definition7);
                        continue;
                    }
                }
                throw new NotSupportedException($"Interface '{definition4.FullName}' contains unsupported method '{definition6.Name}'.");
            }
            using (Collection<PropertyDefinition>.Enumerator enumerator3 = definition4.Properties.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    <Generate>c__AnonStorey0 storey = new <Generate>c__AnonStorey0 {
                        interfaceProperty = enumerator3.Current
                    };
                    PropertyDefinition definition9 = new PropertyDefinition(storey.interfaceProperty.Name, storey.interfaceProperty.Attributes, this._module.ImportReference(storey.interfaceProperty.PropertyType));
                    item.Properties.Add(definition9);
                    if (storey.interfaceProperty.GetMethod != null)
                    {
                        definition9.GetMethod = item.Methods.First<MethodDefinition>(new Func<MethodDefinition, bool>(storey.<>m__0));
                    }
                    if (storey.interfaceProperty.SetMethod != null)
                    {
                        definition9.SetMethod = item.Methods.First<MethodDefinition>(new Func<MethodDefinition, bool>(storey.<>m__1));
                    }
                }
            }
            return item;
        }

        public static TypeDefinition Generate(ModuleDefinition module) => 
            new BindableIteratorToEnumeratorAdapterTypeGenerator(module).Generate();

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
            iLProcessor.Emit(OpCodes.Stfld, this._iteratorField);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldc_I4_1);
            iLProcessor.Emit(OpCodes.Stfld, this._hadCurrentField);
            iLProcessor.Emit(OpCodes.Ret);
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
            iLProcessor.Emit(OpCodes.Ldfld, this._currentField);
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
            iLProcessor.Emit(OpCodes.Ldfld, this._iteratorField);
            iLProcessor.Emit(OpCodes.Callvirt, this._module.ImportReference(this._getHasCurrentMethod));
            iLProcessor.Emit(OpCodes.Stfld, this._hadCurrentField);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldc_I4_1);
            iLProcessor.Emit(OpCodes.Stfld, this._initializedField);
            iLProcessor.Emit(OpCodes.Br_S, target);
            Instruction instruction5 = self.Instructions.Last<Instruction>();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            instruction4.Operand = self.Instructions.Last<Instruction>();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldfld, this._iteratorField);
            iLProcessor.Emit(OpCodes.Callvirt, this._module.ImportReference(this._moveNextMethod));
            iLProcessor.Emit(OpCodes.Stfld, this._hadCurrentField);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            instruction5.Operand = self.Instructions.Last<Instruction>();
            iLProcessor.Emit(OpCodes.Ldfld, this._hadCurrentField);
            iLProcessor.Emit(OpCodes.Brfalse_S, target);
            Instruction instruction6 = self.Instructions.Last<Instruction>();
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldarg_0);
            iLProcessor.Emit(OpCodes.Ldfld, this._iteratorField);
            iLProcessor.Emit(OpCodes.Callvirt, this._module.ImportReference(this._getCurrentMethod));
            iLProcessor.Emit(OpCodes.Stfld, this._currentField);
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

        [CompilerGenerated]
        private sealed class <Generate>c__AnonStorey0
        {
            internal PropertyDefinition interfaceProperty;

            internal bool <>m__0(MethodDefinition m) => 
                (m.Name == this.interfaceProperty.GetMethod.Name);

            internal bool <>m__1(MethodDefinition m) => 
                (m.Name == this.interfaceProperty.SetMethod.Name);
        }
    }
}

