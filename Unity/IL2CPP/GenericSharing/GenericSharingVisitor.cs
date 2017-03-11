namespace Unity.IL2CPP.GenericSharing
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Unity.Cecil.Visitor;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class GenericSharingVisitor : Unity.Cecil.Visitor.Visitor
    {
        private List<RuntimeGenericData> _methodList;
        private List<RuntimeGenericData> _typeList;
        [Inject]
        public static IGenericSharingAnalysisService GenericSharingAnalysis;
        [Inject]
        public static INamingService Naming;

        public void AddArrayUsage(TypeReference genericType)
        {
            this.AddData(new RuntimeGenericTypeData(RuntimeGenericContextInfo.Array, genericType));
        }

        public void AddClassUsage(TypeReference genericType)
        {
            this.AddData(new RuntimeGenericTypeData(RuntimeGenericContextInfo.Class, genericType));
        }

        public void AddData(RuntimeGenericTypeData data)
        {
            List<RuntimeGenericData> list;
            <AddData>c__AnonStorey0 storey = new <AddData>c__AnonStorey0 {
                data = data
            };
            switch (GenericUsageFor(storey.data.GenericType))
            {
                case GenericContextUsage.Type:
                    list = this._typeList;
                    break;

                case GenericContextUsage.Method:
                    list = this._methodList;
                    break;

                case GenericContextUsage.Both:
                    list = this._methodList;
                    break;

                case GenericContextUsage.None:
                    return;

                default:
                    throw new NotSupportedException("Invalid generic parameter usage");
            }
            if (list.FindIndex(new Predicate<RuntimeGenericData>(storey.<>m__0)) == -1)
            {
                list.Add(storey.data);
            }
        }

        public void AddMethodUsage(MethodReference genericMethod)
        {
            List<RuntimeGenericData> list;
            <AddMethodUsage>c__AnonStorey1 storey = new <AddMethodUsage>c__AnonStorey1();
            switch (GenericUsageFor(genericMethod))
            {
                case GenericContextUsage.Type:
                    list = this._typeList;
                    break;

                case GenericContextUsage.Method:
                    list = this._methodList;
                    break;

                case GenericContextUsage.Both:
                    list = this._methodList;
                    break;

                case GenericContextUsage.None:
                    return;

                default:
                    throw new NotSupportedException("Invalid generic parameter usage");
            }
            storey.data = new RuntimeGenericMethodData(RuntimeGenericContextInfo.Method, null, genericMethod);
            if (list.FindIndex(new Predicate<RuntimeGenericData>(storey.<>m__0)) == -1)
            {
                list.Add(storey.data);
            }
        }

        public void AddStaticUsage(TypeReference genericType)
        {
            this.AddStaticUsageRecursiveIfNeeded(genericType);
        }

        private void AddStaticUsageRecursiveIfNeeded(TypeReference genericType)
        {
            if (genericType.IsGenericInstance)
            {
                this.AddData(new RuntimeGenericTypeData(RuntimeGenericContextInfo.Static, genericType));
            }
            TypeReference baseType = genericType.GetBaseType();
            if (baseType != null)
            {
                this.AddStaticUsageRecursiveIfNeeded(baseType);
            }
        }

        public void AddTypeUsage(TypeReference genericType)
        {
            this.AddData(new RuntimeGenericTypeData(RuntimeGenericContextInfo.Type, genericType));
        }

        internal static GenericContextUsage GenericUsageFor(MethodReference method)
        {
            GenericContextUsage usage = GenericUsageFor(method.DeclaringType);
            GenericInstanceMethod method2 = method as GenericInstanceMethod;
            if (method2 != null)
            {
                foreach (TypeReference reference in method2.GenericArguments)
                {
                    usage |= GenericUsageFor(reference);
                }
                return usage;
            }
            return usage;
        }

        internal static GenericContextUsage GenericUsageFor(TypeReference type)
        {
            GenericParameter parameter = type as GenericParameter;
            if (parameter != null)
            {
                return ((parameter.Type != GenericParameterType.Type) ? GenericContextUsage.Method : GenericContextUsage.Type);
            }
            ArrayType type2 = type as ArrayType;
            if (type2 != null)
            {
                return GenericUsageFor(type2.ElementType);
            }
            GenericInstanceType type3 = type as GenericInstanceType;
            if (type3 != null)
            {
                GenericContextUsage none = GenericContextUsage.None;
                foreach (TypeReference reference in type3.GenericArguments)
                {
                    none |= GenericUsageFor(reference);
                }
                return none;
            }
            if (!(type is PointerType) && (type is TypeSpecification))
            {
                throw new NotSupportedException("TypeSpecification found which is not supported");
            }
            return GenericContextUsage.None;
        }

        protected override void Visit(ExceptionHandler exceptionHandler, Unity.Cecil.Visitor.Context context)
        {
            if (exceptionHandler.CatchType != null)
            {
                this.AddClassUsage(exceptionHandler.CatchType);
            }
            base.Visit(exceptionHandler, context);
        }

        protected override void Visit(Instruction instruction, Unity.Cecil.Visitor.Context context)
        {
            MethodReference reference12;
            Code code = instruction.OpCode.Code;
            switch (code)
            {
                case Code.Callvirt:
                    goto Label_0292;

                case Code.Ldobj:
                case Code.Ldfld:
                case Code.Ldflda:
                case Code.Stfld:
                case Code.Stobj:
                case Code.Ldelema:
                case Code.Unbox:
                    goto Label_03D2;

                case Code.Newobj:
                {
                    MethodReference genericMethod = (MethodReference) instruction.Operand;
                    if (genericMethod.DeclaringType.IsArray)
                    {
                        this.AddClassUsage(genericMethod.DeclaringType);
                    }
                    else
                    {
                        this.AddClassUsage(genericMethod.DeclaringType);
                        this.AddMethodUsage(genericMethod);
                    }
                    goto Label_03D2;
                }
                case Code.Castclass:
                case Code.Isinst:
                case Code.Box:
                    break;

                case Code.Ldsfld:
                case Code.Ldsflda:
                case Code.Stsfld:
                {
                    FieldReference reference3 = (FieldReference) instruction.Operand;
                    TypeReference declaringType = reference3.DeclaringType;
                    this.AddStaticUsage(declaringType);
                    goto Label_03D2;
                }
                case Code.Newarr:
                {
                    TypeReference genericType = (TypeReference) instruction.Operand;
                    this.AddArrayUsage(genericType);
                    goto Label_03D2;
                }
                default:
                    switch (code)
                    {
                        case Code.Ldelem_Any:
                        case Code.Stelem_Any:
                        case Code.Initobj:
                            goto Label_03D2;

                        case Code.Unbox_Any:
                        case Code.Constrained:
                            break;

                        case Code.Mkrefany:
                            throw new NotImplementedException();

                        case Code.Ldtoken:
                        {
                            TypeReference reference7 = instruction.Operand as TypeReference;
                            if (reference7 != null)
                            {
                                this.AddTypeUsage(reference7);
                            }
                            MethodReference reference8 = instruction.Operand as MethodReference;
                            if (reference8 != null)
                            {
                                this.AddMethodUsage(reference8);
                            }
                            FieldReference reference9 = instruction.Operand as FieldReference;
                            if (reference9 != null)
                            {
                                this.AddClassUsage(reference9.DeclaringType);
                            }
                            goto Label_03D2;
                        }
                        case Code.Ldftn:
                        {
                            MethodReference reference10 = (MethodReference) instruction.Operand;
                            this.AddMethodUsage(reference10);
                            goto Label_03D2;
                        }
                        case Code.Ldvirtftn:
                        {
                            MethodReference reference11 = (MethodReference) instruction.Operand;
                            this.AddMethodUsage(reference11);
                            if (reference11.DeclaringType.IsInterface())
                            {
                                this.AddClassUsage(reference11.DeclaringType);
                            }
                            goto Label_03D2;
                        }
                        case Code.Call:
                            goto Label_0292;

                        case Code.Sizeof:
                        {
                            TypeReference reference6 = (TypeReference) instruction.Operand;
                            this.AddClassUsage(reference6);
                            goto Label_03D2;
                        }
                        default:
                            if (instruction.Operand is MemberReference)
                            {
                                throw new NotImplementedException();
                            }
                            goto Label_03D2;
                    }
                    break;
            }
            TypeReference operand = (TypeReference) instruction.Operand;
            this.AddClassUsage(operand);
            goto Label_03D2;
        Label_0292:
            reference12 = (MethodReference) instruction.Operand;
            if (!Naming.IsSpecialArrayMethod(reference12) && (!reference12.DeclaringType.IsSystemArray() || ((reference12.Name != "GetGenericValueImpl") && (reference12.Name != "SetGenericValueImpl"))))
            {
                if (((instruction.OpCode.Code == Code.Callvirt) && reference12.DeclaringType.IsInterface()) && !reference12.IsGenericInstance)
                {
                    this.AddClassUsage(reference12.DeclaringType);
                    if ((instruction.Previous != null) && (instruction.Previous.OpCode.Code == Code.Constrained))
                    {
                        this.AddMethodUsage(reference12);
                    }
                }
                else
                {
                    this.AddMethodUsage(reference12);
                }
            }
            MethodReference data = (MethodReference) context.Data;
            if (GenericSharingAnalysis.ShouldTryToCallStaticConstructorBeforeMethodCall(reference12, data))
            {
                this.AddStaticUsage(reference12.DeclaringType);
            }
        Label_03D2:
            base.Visit(instruction, context);
        }

        protected override void Visit(MethodDefinition methodDefinition, Unity.Cecil.Visitor.Context context)
        {
            List<RuntimeGenericData> list = this._methodList;
            this._methodList = new List<RuntimeGenericData>();
            base.Visit(methodDefinition, context);
            if (this._methodList.Count > 0)
            {
                GenericSharingAnalysis.AddMethod(methodDefinition, this._methodList);
            }
            this._methodList = list;
        }

        protected override void Visit(PropertyDefinition propertyDefinition, Unity.Cecil.Visitor.Context context)
        {
        }

        protected override void Visit(TypeDefinition typeDefinition, Unity.Cecil.Visitor.Context context)
        {
            List<RuntimeGenericData> list = this._typeList;
            this._typeList = new List<RuntimeGenericData>();
            base.Visit(typeDefinition, context);
            if (this._typeList.Count > 0)
            {
                GenericSharingAnalysis.AddType(typeDefinition, this._typeList);
            }
            this._typeList = list;
        }

        [CompilerGenerated]
        private sealed class <AddData>c__AnonStorey0
        {
            internal RuntimeGenericTypeData data;

            internal bool <>m__0(RuntimeGenericData d) => 
                ((d.InfoType == this.data.InfoType) && Unity.IL2CPP.Common.TypeReferenceEqualityComparer.AreEqual(((RuntimeGenericTypeData) d).GenericType, this.data.GenericType, TypeComparisonMode.Exact));
        }

        [CompilerGenerated]
        private sealed class <AddMethodUsage>c__AnonStorey1
        {
            internal RuntimeGenericMethodData data;

            internal bool <>m__0(RuntimeGenericData d) => 
                ((d.InfoType == this.data.InfoType) && new Unity.IL2CPP.Common.MethodReferenceComparer().Equals(((RuntimeGenericMethodData) d).GenericMethod, this.data.GenericMethod));
        }
    }
}

