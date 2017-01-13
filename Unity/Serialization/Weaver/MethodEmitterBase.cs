namespace Unity.Serialization.Weaver
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using Mono.Collections.Generic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Unity.CecilTools;
    using Unity.SerializationLogic;

    public abstract class MethodEmitterBase
    {
        private LocalVariable _itemsIndex;
        protected readonly SerializationBridgeProvider _serializationBridgeProvider;
        private readonly TypeDefinition _typeDef;
        private LocalVariablesGenerator _variablesGenerator;
        [CompilerGenerated]
        private static Func<FieldDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Mono.Cecil.MethodDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<Mono.Cecil.MethodDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<AssemblyNameReference, bool> <>f__am$cache3;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Mono.Cecil.MethodDefinition <MethodDefinition>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ILProcessor <Processor>k__BackingField;
        protected const string AlignMethodName = "Align";
        protected const string ConstructorMethodName = ".ctor";
        private const string CountPropertyName = "get_Count";
        private const int DepthLimit = 7;
        private const string GetTypeFromHandleMethodName = "GetTypeFromHandle";
        protected Unity.SerializationLogic.TypeResolver TypeResolver = new Unity.SerializationLogic.TypeResolver(null);

        protected MethodEmitterBase(TypeDefinition typeDef, SerializationBridgeProvider serializationBridgeProvider)
        {
            this._typeDef = typeDef;
            this._serializationBridgeProvider = serializationBridgeProvider;
        }

        protected void Add()
        {
            this.Processor.Emit(OpCodes.Add);
        }

        protected void Bgt(Instruction endLbl)
        {
            this.Processor.Emit(OpCodes.Bgt, endLbl);
        }

        protected void Box(TypeReference typeRef)
        {
            this.Processor.Emit(OpCodes.Box, typeRef);
        }

        protected void Br_S(Instruction endLbl)
        {
            this.Processor.Emit(OpCodes.Br, endLbl);
        }

        protected void Brfalse_S(Instruction endLbl)
        {
            this.Processor.Emit(OpCodes.Brfalse, endLbl);
        }

        protected void Brtrue_S(Instruction ifNotNullLbl)
        {
            this.Processor.Emit(OpCodes.Brtrue, ifNotNullLbl);
        }

        protected void Call(MethodReference methodReference)
        {
            this.Processor.Emit(OpCodes.Call, methodReference);
        }

        private void Call(System.Type type, string name)
        {
            this.Processor.Emit(OpCodes.Call, this.MethodRefFor(type, name));
        }

        protected void CallMethodOn(MethodReference methodReference, TypeReference thisType)
        {
            if (IsStruct(thisType))
            {
                this.Call(methodReference);
            }
            else
            {
                this.Callvirt(methodReference);
            }
        }

        protected void Callvirt(MethodReference methodReference)
        {
            this.Processor.Emit(OpCodes.Callvirt, methodReference);
        }

        protected void Callvirt(TypeDefinition typeDefinition, string name)
        {
            this.Processor.Emit(OpCodes.Callvirt, this.MethodRefFor(typeDefinition, name, false));
        }

        protected void Callvirt(System.Type type, string name)
        {
            this.Processor.Emit(OpCodes.Callvirt, this.MethodRefFor(type, name));
        }

        protected static bool CanInlineLoopOn(TypeReference typeReference) => 
            (UnitySerializationLogic.IsSupportedCollection(typeReference) && !IsSystemByte(CecilUtils.ElementTypeOfCollection(typeReference)));

        protected void Ceq()
        {
            this.Processor.Emit(OpCodes.Ceq);
        }

        private void Clt()
        {
            this.Processor.Emit(OpCodes.Clt);
        }

        private TypeReference ConcreteImplementationFor(TypeDefinition stateIntefrace) => 
            new TypeReference("UnityEngine.Serialization", stateIntefrace.Name.Substring(1), this.Module, this.UnityEngineScope());

        protected void ConstructAndStoreSerializableObject(FieldReference fieldRef)
        {
            this.Ldarg_0();
            this.ConstructSerializableObject(this.TypeOf(fieldRef));
            this.EmitStoreField(fieldRef);
        }

        protected void ConstructSerializableObject(TypeReference typeReference)
        {
            MethodReference ctor = this.DefaultConstructorFor(typeReference);
            if (ctor != null)
            {
                this.Newobj(ctor);
            }
            else
            {
                this.Ldnull();
                this.Newobj(this.SerializationWeaverInjectedConstructorFor(typeReference));
            }
        }

        protected void Conv_I()
        {
            this.Processor.Emit(OpCodes.Conv_I);
        }

        protected void Conv_I4()
        {
            this.Processor.Emit(OpCodes.Conv_I4);
        }

        private MethodReference CountMethodFor(TypeReference typeReference) => 
            new MethodReference("get_Count", this.Import(typeof(int)), TypeReferenceFor(typeReference)) { HasThis = true };

        protected void CreateMethodDef(string methodName)
        {
            this.MethodDefinition = new Mono.Cecil.MethodDefinition(methodName, Mono.Cecil.MethodAttributes.CompilerControlled | Mono.Cecil.MethodAttributes.FamANDAssem | Mono.Cecil.MethodAttributes.Family | Mono.Cecil.MethodAttributes.Virtual, this.VoidTypeRef());
            this.MethodDefinition.Parameters.Add(new ParameterDefinition("depth", Mono.Cecil.ParameterAttributes.None, this.Import(typeof(int))));
            this.MethodDefinition.Body = new Mono.Cecil.Cil.MethodBody(this.MethodDefinition);
            this.Processor = this.MethodDefinition.Body.GetILProcessor();
            this._variablesGenerator = new LocalVariablesGenerator(this.MethodDefinition, this.Processor);
            this.EmitMethodBody();
            this.MethodDefinition.Body.OptimizeMacros();
        }

        private MethodReference DefaultConstructorFor(TypeReference typeReference)
        {
            TypeDefinition definition = typeReference.Resolve();
            if (definition == null)
            {
                return null;
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = x => (x.Name == ".ctor") && !x.HasParameters;
            }
            if (definition.Methods.SingleOrDefault<Mono.Cecil.MethodDefinition>(<>f__am$cache2) == null)
            {
                return null;
            }
            return new MethodReference(".ctor", this.Module.TypeSystem.Void, typeReference) { HasThis = true };
        }

        protected Instruction DefineLabel() => 
            this.Processor.Create(OpCodes.Nop);

        protected LocalVariable DefineLocal(TypeReference type) => 
            this._variablesGenerator.Create(type);

        private void Emit(OpCode opcode)
        {
            this.Processor.Emit(opcode);
        }

        private void EmitIncrementItemIndex()
        {
            this.EmitLoadItemIndex();
            this.Ldc_I4(1);
            this.Add();
            this.EmitStoreItemIndex();
        }

        protected abstract void EmitInstructionsFor(FieldReference field);
        private void EmitItemsLoopCondition(FieldReference fieldDef, Action<FieldReference> maxValueGen)
        {
            this.EmitLoadItemIndex();
            maxValueGen(fieldDef);
            this.Conv_I4();
            this.Clt();
        }

        private void EmitItemsLoopInitializer()
        {
            this.Ldc_I4(0);
            this.EmitStoreItemIndex();
        }

        protected void EmitLengthOf(FieldReference fieldDef)
        {
            this.EmitLoadField(fieldDef);
            if (this.TypeOf(fieldDef).IsArray)
            {
                this.Ldlen();
            }
            else
            {
                this.Call(this.CountMethodFor(this.TypeOf(fieldDef)));
            }
        }

        public void EmitLoadField(FieldReference fieldDef)
        {
            TypeReference typeRef = this.TypeOf(fieldDef);
            this.Ldarg_0();
            if (IsStructByRef(typeRef))
            {
                this.Ldflda(fieldDef);
            }
            else
            {
                this.Ldfld(fieldDef);
            }
        }

        protected void EmitLoadFieldArrayItemInLoop(FieldReference fieldDef)
        {
            this.EmitLoadField(fieldDef);
            this.EmitLoadItemIndex();
            TypeReference typeRef = CecilUtils.ElementTypeOfCollection(this.TypeOf(fieldDef));
            if ((typeRef.IsPrimitive || IsEnum(typeRef)) || UnityEngineTypePredicates.IsColor32(typeRef))
            {
                this.Processor.Emit(OpCodes.Ldelem_Any, typeRef);
            }
            else if (IsStruct(typeRef))
            {
                this.Processor.Emit(OpCodes.Ldelema, typeRef);
            }
            else
            {
                this.Processor.Emit(OpCodes.Ldelem_Any, typeRef);
            }
        }

        protected void EmitLoadItemIndex()
        {
            this.ItemsIndexVar.EmitLoad();
        }

        protected void EmitLoopOnFieldElements(FieldReference fieldDef, Action<FieldReference, TypeReference> loopBody)
        {
            this.EmitLoopOnFieldElements(fieldDef, new Action<FieldReference>(this.EmitLengthOf), loopBody);
        }

        protected void EmitLoopOnFieldElements(FieldReference fieldDef, Action<FieldReference> maxValueGen, Action<FieldReference, TypeReference> loopBody)
        {
            Instruction ifNotNullLbl = this.DefineLabel();
            Instruction endLbl = this.DefineLabel();
            this.EmitItemsLoopInitializer();
            this.Br_S(endLbl);
            this.MarkLabel(ifNotNullLbl);
            loopBody(fieldDef, CecilUtils.ElementTypeOfCollection(this.TypeOf(fieldDef)));
            this.EmitIncrementItemIndex();
            this.MarkLabel(endLbl);
            this.EmitItemsLoopCondition(fieldDef, maxValueGen);
            this.Brtrue_S(ifNotNullLbl);
        }

        private void EmitMethodBody()
        {
            <EmitMethodBody>c__AnonStorey1 storey = new <EmitMethodBody>c__AnonStorey1 {
                $this = this,
                baseTypes = new Stack<TypeReference>()
            };
            TypeReference baseType = this.TypeDef.BaseType;
            this.TypeResolver = new Unity.SerializationLogic.TypeResolver(null);
            this.InjectBeforeSerialize();
            while (!UnitySerializationLogic.IsNonSerialized(baseType))
            {
                GenericInstanceType genericInstanceType = baseType as GenericInstanceType;
                if (genericInstanceType != null)
                {
                    this.TypeResolver.Add(genericInstanceType);
                }
                storey.baseTypes.Push(baseType);
                baseType = baseType.Resolve().BaseType;
            }
            while (storey.baseTypes.Count > 0)
            {
                TypeReference reference2 = storey.baseTypes.Pop();
                TypeDefinition definition = reference2.Resolve();
                foreach (FieldDefinition definition2 in definition.Fields.Where<FieldDefinition>(new Func<FieldDefinition, bool>(this.ShouldProcess)).Where<FieldDefinition>(new Func<FieldDefinition, bool>(storey.<>m__0)))
                {
                    this.EmitInstructionsFor(this.ResolveGenericFieldReference(definition2));
                    if (!definition2.IsPublic && !definition2.IsFamily)
                    {
                        definition2.IsPrivate = false;
                        definition2.IsFamilyOrAssembly = true;
                    }
                }
                GenericInstanceType type2 = reference2 as GenericInstanceType;
                if (type2 != null)
                {
                    this.TypeResolver.Remove(type2);
                }
            }
            foreach (FieldDefinition definition3 in this.FilteredFields())
            {
                this.EmitInstructionsFor(definition3);
                if (!definition3.IsPublic && !definition3.IsFamily)
                {
                    definition3.IsPrivate = false;
                    definition3.IsFamilyOrAssembly = true;
                }
            }
            this.InjectAfterDeserialize();
            this.MethodBodySuffix();
            this.MethodDefinition.Body.OptimizeMacros();
        }

        protected void EmitStoreField(FieldReference fieldDef)
        {
            this.Processor.Emit(OpCodes.Stfld, this.ResolveGenericFieldReference(fieldDef));
        }

        private void EmitStoreItemIndex()
        {
            this.ItemsIndexVar.EmitStore();
        }

        protected void EmitTypeOf(TypeReference typeRef)
        {
            this.Ldtoken(typeRef);
            this.Call(typeof(System.Type), "GetTypeFromHandle");
        }

        protected void EmitTypeOfIfNeeded(TypeReference typeRef)
        {
            if (InvocationRequiresTypeRef(typeRef))
            {
                if (!IsStruct(typeRef))
                {
                    this.EmitTypeOf(typeRef);
                }
                else
                {
                    this.Ldnull();
                }
            }
        }

        private void EmitWithDepthCheck(Action action)
        {
            Instruction endLbl = this.DefineLabel();
            this.Ldarg_1();
            this.Ldc_I4(7);
            this.Bgt(endLbl);
            action();
            this.MarkLabel(endLbl);
        }

        protected void EmitWithDepthCheck<TArg>(Action<TArg> action, TArg arg)
        {
            <EmitWithDepthCheck>c__AnonStorey6<TArg> storey = new <EmitWithDepthCheck>c__AnonStorey6<TArg> {
                action = action,
                arg = arg
            };
            this.EmitWithDepthCheck(new Action(storey.<>m__0));
        }

        protected void EmitWithDepthCheck<TArg1, TArg2>(Action<TArg1, TArg2> action, TArg1 arg1, TArg2 arg2)
        {
            <EmitWithDepthCheck>c__AnonStorey7<TArg1, TArg2> storey = new <EmitWithDepthCheck>c__AnonStorey7<TArg1, TArg2> {
                action = action,
                arg1 = arg1,
                arg2 = arg2
            };
            this.EmitWithDepthCheck(new Action(storey.<>m__0));
        }

        protected void EmitWithNotNullCheckOnField(FieldReference fieldDef, Action<FieldReference> ifNotNull)
        {
            Instruction endLbl = this.DefineLabel();
            this.Ldarg_0();
            this.Ldfld(fieldDef);
            this.Brfalse_S(endLbl);
            ifNotNull(fieldDef);
            this.MarkLabel(endLbl);
        }

        protected void EmitWithNullCheckOnField(FieldReference fieldRef, Action<FieldReference> ifNull)
        {
            Instruction ifNotNullLbl = this.DefineLabel();
            this.Ldarg_0();
            this.Ldfld(fieldRef);
            this.Brtrue_S(ifNotNullLbl);
            ifNull(fieldRef);
            this.MarkLabel(ifNotNullLbl);
        }

        protected void EmitWithNullCheckOnField(FieldReference fieldDef, Action<FieldReference> ifNull, Action<FieldReference> ifNotNull)
        {
            Instruction ifNotNullLbl = this.DefineLabel();
            Instruction endLbl = this.DefineLabel();
            this.EmitLoadField(fieldDef);
            this.Brtrue_S(ifNotNullLbl);
            ifNull(fieldDef);
            this.Br_S(endLbl);
            this.MarkLabel(ifNotNullLbl);
            ifNotNull(fieldDef);
            this.MarkLabel(endLbl);
        }

        private IEnumerable<FieldDefinition> FilteredFields()
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = f => (UnitySerializationLogic.IsSupportedCollection(f.FieldType) || !f.FieldType.IsGenericInstance) || UnitySerializationLogic.ShouldImplementIDeserializable(f.FieldType.Resolve());
            }
            return this.TypeDef.Fields.Where<FieldDefinition>(new Func<FieldDefinition, bool>(this.ShouldProcess)).Where<FieldDefinition>(<>f__am$cache0);
        }

        private static Mono.Cecil.MethodDefinition FindMethodReference(TypeDefinition typeDefinition, string name)
        {
            <FindMethodReference>c__AnonStorey5 storey = new <FindMethodReference>c__AnonStorey5 {
                name = name
            };
            Mono.Cecil.MethodDefinition definition = typeDefinition.Methods.FirstOrDefault<Mono.Cecil.MethodDefinition>(new Func<Mono.Cecil.MethodDefinition, bool>(storey.<>m__0));
            if (definition != null)
            {
                return definition;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = m => m.HasOverrides;
            }
            return typeDefinition.Methods.Where<Mono.Cecil.MethodDefinition>(<>f__am$cache1).FirstOrDefault<Mono.Cecil.MethodDefinition>(new Func<Mono.Cecil.MethodDefinition, bool>(storey.<>m__1));
        }

        protected MethodReference GetItemMethodRefFor(TypeReference typeReference)
        {
            GenericParameter returnType = ((GenericInstanceType) typeReference).ElementType.GenericParameters[0];
            MethodReference method = new MethodReference("get_Item", returnType, TypeReferenceFor(typeReference)) {
                Parameters = { new ParameterDefinition("index", Mono.Cecil.ParameterAttributes.None, this.Import(typeof(int))) },
                HasThis = true
            };
            return this.Module.ImportReference(method);
        }

        protected bool IfTypeImplementsInterface(string interfaceName)
        {
            <IfTypeImplementsInterface>c__AnonStorey3 storey = new <IfTypeImplementsInterface>c__AnonStorey3 {
                interfaceName = interfaceName
            };
            return (InterfacesOf(this._typeDef).FirstOrDefault<TypeReference>(new Func<TypeReference, bool>(storey.<>m__0)) != null);
        }

        protected FieldReference Import(FieldInfo fieldInfo) => 
            this.Module.ImportReference(fieldInfo);

        protected TypeReference Import(System.Type type) => 
            this.Module.ImportReference(type);

        protected abstract void InjectAfterDeserialize();
        protected abstract void InjectBeforeSerialize();
        [DebuggerHidden]
        protected static IEnumerable<TypeReference> InterfacesOf(TypeDefinition typeDefinition) => 
            new <InterfacesOf>c__Iterator0 { 
                typeDefinition = typeDefinition,
                $PC = -2
            };

        private static bool InvocationRequiresTypeRef(TypeReference typeRef) => 
            ((!IsUnityEngineObject(typeRef) && !UnityEngineTypePredicates.IsSerializableUnityStruct(typeRef)) && ShouldImplementIDeserializable(typeRef));

        protected void InvokeMethodIfTypeImplementsInterface(string interfaceName, string methodName)
        {
            <InvokeMethodIfTypeImplementsInterface>c__AnonStorey4 storey = new <InvokeMethodIfTypeImplementsInterface>c__AnonStorey4 {
                interfaceName = interfaceName
            };
            TypeReference type = InterfacesOf(this._typeDef).FirstOrDefault<TypeReference>(new Func<TypeReference, bool>(storey.<>m__0));
            if (type != null)
            {
                if (this._typeDef.IsValueType)
                {
                    this.Processor.Emit(OpCodes.Ldarg_0);
                    this.Call(this.MethodRefFor(this._typeDef, methodName, false));
                }
                else
                {
                    this.Processor.Emit(OpCodes.Ldarg_0);
                    this.Processor.Emit(OpCodes.Castclass, this.Module.ImportReference(type));
                    this.Callvirt(this.MethodRefFor(type.Resolve(), methodName, true));
                }
            }
        }

        protected static bool IsEnum(TypeReference typeRef) => 
            (!typeRef.IsArray && typeRef.Resolve().IsEnum);

        private static bool IsHiddenByParentClass(IEnumerable<TypeReference> parentTypes, FieldDefinition fieldDefinition, TypeDefinition processingType)
        {
            <IsHiddenByParentClass>c__AnonStorey2 storey = new <IsHiddenByParentClass>c__AnonStorey2 {
                fieldDefinition = fieldDefinition
            };
            return (processingType.Fields.Any<FieldDefinition>(new Func<FieldDefinition, bool>(storey.<>m__0)) || parentTypes.Any<TypeReference>(new Func<TypeReference, bool>(storey.<>m__1)));
        }

        protected void Isinst(TypeReference typeReference)
        {
            this.Processor.Emit(OpCodes.Isinst, typeReference);
        }

        protected static bool IsIUnitySerializable(TypeReference typeRef)
        {
            if (IsUnityEngineObject(typeRef) || UnityEngineTypePredicates.IsSerializableUnityStruct(typeRef))
            {
                return false;
            }
            return ShouldImplementIDeserializable(typeRef);
        }

        protected static bool IsStruct(TypeReference typeRef)
        {
            if (!typeRef.IsValueType)
            {
                return false;
            }
            if (IsEnum(typeRef))
            {
                return false;
            }
            return !typeRef.IsPrimitive;
        }

        protected static bool IsStructByRef(TypeReference typeRef)
        {
            if (!IsStruct(typeRef))
            {
                return false;
            }
            string fullName = typeRef.FullName;
            if ((fullName != null) && (fullName == "UnityEngine.Color32"))
            {
                return false;
            }
            return true;
        }

        protected static bool IsSystemByte(TypeReference typeRef) => 
            (typeRef.FullName == "System.Byte");

        private static bool IsSystemString(TypeReference typeRef) => 
            (typeRef.FullName == "System.String");

        protected static bool IsUnityEngineObject(TypeReference typeRef) => 
            UnityEngineTypePredicates.IsUnityEngineObject(typeRef);

        protected void Ldarg_0()
        {
            this.Emit(OpCodes.Ldarg_0);
        }

        protected void Ldarg_1()
        {
            this.Emit(OpCodes.Ldarg_1);
        }

        protected void Ldc_I4(int value)
        {
            switch ((value + 1))
            {
                case 0:
                    this.Processor.Emit(OpCodes.Ldc_I4_M1);
                    break;

                case 1:
                    this.Processor.Emit(OpCodes.Ldc_I4_0);
                    break;

                case 2:
                    this.Processor.Emit(OpCodes.Ldc_I4_1);
                    break;

                case 3:
                    this.Processor.Emit(OpCodes.Ldc_I4_2);
                    break;

                case 4:
                    this.Processor.Emit(OpCodes.Ldc_I4_3);
                    break;

                case 5:
                    this.Processor.Emit(OpCodes.Ldc_I4_4);
                    break;

                case 6:
                    this.Processor.Emit(OpCodes.Ldc_I4_5);
                    break;

                case 7:
                    this.Processor.Emit(OpCodes.Ldc_I4_6);
                    break;

                case 8:
                    this.Processor.Emit(OpCodes.Ldc_I4_7);
                    break;

                case 9:
                    this.Processor.Emit(OpCodes.Ldc_I4_8);
                    break;

                default:
                    if ((value >= -128) && (value <= 0x7f))
                    {
                        this.Processor.Emit(OpCodes.Ldc_I4_S, (sbyte) value);
                    }
                    else
                    {
                        this.Processor.Emit(OpCodes.Ldc_I4, value);
                    }
                    break;
            }
        }

        protected void Ldelem_Ref()
        {
            this.Processor.Emit(OpCodes.Ldelem_Ref);
        }

        protected void Ldelema(TypeReference type)
        {
            this.Processor.Emit(OpCodes.Ldelema, type);
        }

        protected void Ldfld(FieldReference fieldRef)
        {
            this.Processor.Emit(OpCodes.Ldfld, this.ResolveGenericFieldReference(fieldRef));
        }

        private void Ldflda(FieldReference fieldRef)
        {
            this.Processor.Emit(OpCodes.Ldflda, this.ResolveGenericFieldReference(fieldRef));
        }

        private void Ldlen()
        {
            this.Processor.Emit(OpCodes.Ldlen);
        }

        protected void Ldnull()
        {
            this.Processor.Emit(OpCodes.Ldnull);
        }

        protected void Ldsfld(FieldReference fieldRef)
        {
            this.Processor.Emit(OpCodes.Ldsfld, this.ResolveGenericFieldReference(fieldRef));
        }

        protected void Ldsflda(FieldReference fieldRef)
        {
            this.Processor.Emit(OpCodes.Ldsflda, this.ResolveGenericFieldReference(fieldRef));
        }

        private void Ldtoken(TypeReference typeRef)
        {
            this.Processor.Emit(OpCodes.Ldtoken, typeRef);
        }

        protected void LoadStateInstance(TypeDefinition stateInterface)
        {
            this.Processor.Emit(OpCodes.Ldsfld, new FieldReference("Instance", this.Module.ImportReference(stateInterface), this.ConcreteImplementationFor(stateInterface)));
        }

        protected void MarkLabel(Instruction ifNotNullLbl)
        {
            this.Processor.Append(ifNotNullLbl);
        }

        protected void MethodBodySuffix()
        {
            this.Ret();
        }

        protected MethodReference MethodRefFor(System.Type type, string name) => 
            this.Module.ImportReference(type.GetMethod(name));

        protected MethodReference MethodRefFor(TypeDefinition typeDefinition, string name, bool checkInBaseTypes = false)
        {
            Mono.Cecil.MethodDefinition method = FindMethodReference(typeDefinition, name);
            if ((method == null) && checkInBaseTypes)
            {
                for (TypeDefinition definition2 = typeDefinition.BaseType.Resolve(); definition2 != null; definition2 = definition2.BaseType?.Resolve())
                {
                    method = FindMethodReference(definition2, name);
                    if (method != null)
                    {
                        break;
                    }
                }
            }
            if (method == null)
            {
                throw new ArgumentException("Cannot find method " + name + " on type " + typeDefinition.FullName, "name");
            }
            return this.Module.ImportReference(method);
        }

        protected string MethodSuffixFor(TypeReference typeRef)
        {
            if (typeRef.IsPrimitive || IsSystemString(typeRef))
            {
                return ToPascalCase(typeRef.Name);
            }
            if (IsEnum(typeRef))
            {
                return "Int32";
            }
            if (CecilUtils.IsGenericList(typeRef))
            {
                return ("ListOf" + this.MethodSuffixFor(CecilUtils.ElementTypeOfCollection(typeRef)));
            }
            if (typeRef.IsArray)
            {
                return ("ArrayOf" + this.MethodSuffixFor(typeRef.GetElementType()));
            }
            if (IsUnityEngineObject(typeRef))
            {
                return "UnityEngineObject";
            }
            if (UnityEngineTypePredicates.IsSerializableUnityStruct(typeRef))
            {
                return typeRef.Name;
            }
            if (!ShouldImplementIDeserializable(typeRef))
            {
                throw new ArgumentException("typeRef", "Cannot serialize field of type " + typeRef.FullName);
            }
            return "IDeserializable";
        }

        protected bool NeedsDepthCheck(TypeReference type) => 
            ((type.MetadataType == MetadataType.Class) || ((type is ArrayType) || CecilUtils.IsGenericList(type)));

        protected void Newobj(MethodReference ctor)
        {
            this.Processor.Emit(OpCodes.Newobj, ctor);
        }

        protected ParameterDefinition ParamDef(string name, System.Type type) => 
            new ParameterDefinition(name, Mono.Cecil.ParameterAttributes.None, this.Import(type));

        protected static bool RequiresAlignment(TypeReference typeRef)
        {
            switch (typeRef.MetadataType)
            {
                case MetadataType.Boolean:
                case MetadataType.Char:
                case MetadataType.SByte:
                case MetadataType.Byte:
                case MetadataType.Int16:
                case MetadataType.UInt16:
                    return true;
            }
            return (UnitySerializationLogic.IsSupportedCollection(typeRef) && RequiresAlignment(CecilUtils.ElementTypeOfCollection(typeRef)));
        }

        protected static bool RequiresBoxing(TypeReference typeRef) => 
            IsStruct(typeRef);

        private TypeReference ResolveDeclaringType(TypeReference declaringType)
        {
            TypeDefinition definition = declaringType.Resolve();
            if ((definition == null) || !definition.HasGenericParameters)
            {
                return definition;
            }
            GenericInstanceType typeReference = new GenericInstanceType(definition);
            foreach (GenericParameter parameter in definition.GenericParameters)
            {
                typeReference.GenericArguments.Add(parameter);
            }
            return this.TypeResolver.Resolve(typeReference);
        }

        protected FieldReference ResolveGenericFieldReference(FieldReference fieldRef)
        {
            FieldReference field = new FieldReference(fieldRef.Name, fieldRef.FieldType, this.ResolveDeclaringType(fieldRef.DeclaringType));
            return this.Module.ImportReference(field);
        }

        protected void Ret()
        {
            this.Emit(OpCodes.Ret);
        }

        private ParameterDefinition SelfUnitySerializableParam() => 
            new ParameterDefinition("self", Mono.Cecil.ParameterAttributes.None, this.Module.ImportReference(this._serializationBridgeProvider.UnitySerializableInterface));

        private MethodReference SerializationWeaverInjectedConstructorFor(TypeReference typeReference)
        {
            MethodReference reference = new MethodReference(".ctor", this.Module.TypeSystem.Void, typeReference) {
                HasThis = true
            };
            reference.Parameters.Add(this.SelfUnitySerializableParam());
            return reference;
        }

        protected MethodReference SetItemMethodRefFor(TypeReference typeReference)
        {
            GenericParameter parameterType = ((GenericInstanceType) typeReference).ElementType.GenericParameters[0];
            MethodReference method = new MethodReference("set_Item", this.Import(typeof(void)), TypeReferenceFor(typeReference)) {
                Parameters = { 
                    new ParameterDefinition("index", Mono.Cecil.ParameterAttributes.None, this.Import(typeof(int))),
                    new ParameterDefinition("item", Mono.Cecil.ParameterAttributes.None, parameterType)
                },
                HasThis = true
            };
            return this.Module.ImportReference(method);
        }

        protected static bool ShouldImplementIDeserializable(TypeReference typeRef) => 
            UnitySerializationLogic.ShouldImplementIDeserializable(typeRef);

        protected abstract bool ShouldProcess(FieldDefinition fieldDefinition);
        protected void Starg(ushort value)
        {
            if (value <= 0xff)
            {
                this.Processor.Emit(OpCodes.Starg_S, (byte) value);
            }
            else
            {
                this.Processor.Emit(OpCodes.Starg, (int) value);
            }
        }

        protected void Stelem_Ref()
        {
            this.Processor.Emit(OpCodes.Stelem_Ref);
        }

        protected static string ToPascalCase(string name) => 
            (char.ToUpper(name[0]) + name.Substring(1));

        protected TypeReference TypeOf(FieldReference fieldDef) => 
            this.Module.ImportReference(this.TypeResolver.Resolve(fieldDef.FieldType));

        protected static TypeReference TypeReferenceFor(TypeReference typeReference)
        {
            TypeDefinition definition = typeReference as TypeDefinition;
            if (definition == null)
            {
                return typeReference;
            }
            return new TypeReference(definition.Namespace, definition.Name, definition.Module, definition.Scope, definition.IsValueType) { DeclaringType = TypeReferenceFor(definition.DeclaringType) };
        }

        private AssemblyNameReference UnityEngineScope()
        {
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = m => m.Name == "UnityEngine";
            }
            return this.Module.AssemblyReferences.First<AssemblyNameReference>(<>f__am$cache3);
        }

        protected TypeReference VoidTypeRef() => 
            this.Module.TypeSystem.Void;

        protected bool WillUnitySerialize(FieldDefinition fieldDefinition)
        {
            bool flag;
            try
            {
                TypeReference typeReference = this.TypeResolver.Resolve(fieldDefinition.FieldType);
                if (UnitySerializationLogic.ShouldNotTryToResolve(typeReference))
                {
                    return false;
                }
                if (!UnityEngineTypePredicates.IsUnityEngineObject(typeReference) && (typeReference.FullName == fieldDefinition.DeclaringType.FullName))
                {
                    return false;
                }
                flag = UnitySerializationLogic.WillUnitySerialize(fieldDefinition, this.TypeResolver);
            }
            catch (Exception exception)
            {
                throw new Exception($"Exception while processing {fieldDefinition.FieldType.FullName} {fieldDefinition.FullName}, error {exception.Message}");
            }
            return flag;
        }

        protected LocalVariable ItemsIndexVar
        {
            get
            {
                LocalVariable variable2;
                if (this._itemsIndex != null)
                {
                    variable2 = this._itemsIndex;
                }
                else
                {
                    variable2 = this._itemsIndex = this.DefineLocal(this.Import(typeof(int)));
                }
                return variable2;
            }
        }

        protected Mono.Cecil.MethodDefinition MethodDefinition { get; set; }

        protected ModuleDefinition Module =>
            this.TypeDef.Module;

        protected ILProcessor Processor { get; set; }

        protected TypeDefinition TypeDef =>
            this._typeDef;

        [CompilerGenerated]
        private sealed class <EmitMethodBody>c__AnonStorey1
        {
            internal MethodEmitterBase $this;
            internal Stack<TypeReference> baseTypes;

            internal bool <>m__0(FieldDefinition f) => 
                !MethodEmitterBase.IsHiddenByParentClass(this.baseTypes, f, this.$this.TypeDef);
        }

        [CompilerGenerated]
        private sealed class <EmitWithDepthCheck>c__AnonStorey6<TArg>
        {
            internal Action<TArg> action;
            internal TArg arg;

            internal void <>m__0()
            {
                this.action(this.arg);
            }
        }

        [CompilerGenerated]
        private sealed class <EmitWithDepthCheck>c__AnonStorey7<TArg1, TArg2>
        {
            internal Action<TArg1, TArg2> action;
            internal TArg1 arg1;
            internal TArg2 arg2;

            internal void <>m__0()
            {
                this.action(this.arg1, this.arg2);
            }
        }

        [CompilerGenerated]
        private sealed class <FindMethodReference>c__AnonStorey5
        {
            internal string name;

            internal bool <>m__0(MethodDefinition m) => 
                (m.Name == this.name);

            internal bool <>m__1(MethodDefinition m) => 
                m.Overrides.Any<MethodReference>(o => (o.Name == this.name));

            internal bool <>m__2(MethodReference o) => 
                (o.Name == this.name);
        }

        [CompilerGenerated]
        private sealed class <IfTypeImplementsInterface>c__AnonStorey3
        {
            internal string interfaceName;

            internal bool <>m__0(TypeReference i) => 
                (i.FullName == this.interfaceName);
        }

        [CompilerGenerated]
        private sealed class <InterfacesOf>c__Iterator0 : IEnumerable, IEnumerable<TypeReference>, IEnumerator, IDisposable, IEnumerator<TypeReference>
        {
            internal TypeReference $current;
            internal bool $disposing;
            internal Collection<InterfaceImplementation>.Enumerator $locvar0;
            internal int $PC;
            internal TypeDefinition <current>__0;
            internal InterfaceImplementation <interfaceImpl>__1;
            internal TypeDefinition typeDefinition;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$disposing = true;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
                        }
                        finally
                        {
                            this.$locvar0.Dispose();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        this.<current>__0 = this.typeDefinition;
                        goto Label_00F1;

                    case 1:
                        break;

                    default:
                        goto Label_0103;
                }
            Label_0050:
                try
                {
                    while (this.$locvar0.MoveNext())
                    {
                        this.<interfaceImpl>__1 = this.$locvar0.Current;
                        this.$current = this.<interfaceImpl>__1.InterfaceType;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        flag = true;
                        return true;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    this.$locvar0.Dispose();
                }
                this.<current>__0 = this.<current>__0.BaseType?.Resolve();
            Label_00F1:
                if (this.<current>__0 != null)
                {
                    this.$locvar0 = this.<current>__0.Interfaces.GetEnumerator();
                    num = 0xfffffffd;
                    goto Label_0050;
                }
                this.$PC = -1;
            Label_0103:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<TypeReference> IEnumerable<TypeReference>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new MethodEmitterBase.<InterfacesOf>c__Iterator0 { typeDefinition = this.typeDefinition };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<Mono.Cecil.TypeReference>.GetEnumerator();

            TypeReference IEnumerator<TypeReference>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <InvokeMethodIfTypeImplementsInterface>c__AnonStorey4
        {
            internal string interfaceName;

            internal bool <>m__0(TypeReference i) => 
                (i.FullName == this.interfaceName);
        }

        [CompilerGenerated]
        private sealed class <IsHiddenByParentClass>c__AnonStorey2
        {
            internal FieldDefinition fieldDefinition;

            internal bool <>m__0(FieldDefinition f) => 
                (f.Name == this.fieldDefinition.Name);

            internal bool <>m__1(TypeReference t) => 
                t.Resolve().Fields.Any<FieldDefinition>(f => (f.Name == this.fieldDefinition.Name));

            internal bool <>m__2(FieldDefinition f) => 
                (f.Name == this.fieldDefinition.Name);
        }
    }
}

