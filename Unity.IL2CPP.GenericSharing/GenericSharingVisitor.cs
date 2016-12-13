using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Unity.Cecil.Visitor;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.GenericSharing
{
	public class GenericSharingVisitor : Visitor
	{
		private List<RuntimeGenericData> _typeList;

		private List<RuntimeGenericData> _methodList;

		[Inject]
		public static IGenericSharingAnalysisService GenericSharingAnalysis;

		[Inject]
		public static INamingService Naming;

		protected override void Visit(TypeDefinition typeDefinition, Context context)
		{
			List<RuntimeGenericData> typeList = this._typeList;
			this._typeList = new List<RuntimeGenericData>();
			base.Visit(typeDefinition, context);
			if (this._typeList.Count > 0)
			{
				GenericSharingVisitor.GenericSharingAnalysis.AddType(typeDefinition, this._typeList);
			}
			this._typeList = typeList;
		}

		protected override void Visit(MethodDefinition methodDefinition, Context context)
		{
			List<RuntimeGenericData> methodList = this._methodList;
			this._methodList = new List<RuntimeGenericData>();
			base.Visit(methodDefinition, context);
			if (this._methodList.Count > 0)
			{
				GenericSharingVisitor.GenericSharingAnalysis.AddMethod(methodDefinition, this._methodList);
			}
			this._methodList = methodList;
		}

		protected override void Visit(PropertyDefinition propertyDefinition, Context context)
		{
		}

		protected override void Visit(ExceptionHandler exceptionHandler, Context context)
		{
			if (exceptionHandler.CatchType != null)
			{
				this.AddClassUsage(exceptionHandler.CatchType);
			}
			base.Visit(exceptionHandler, context);
		}

		protected override void Visit(Instruction instruction, Context context)
		{
			Code code = instruction.OpCode.Code;
			switch (code)
			{
			case Code.Callvirt:
				goto IL_28A;
			case Code.Cpobj:
			case Code.Ldstr:
			case Code.Conv_R_Un:
			case Code.Throw:
			case Code.Conv_Ovf_I1_Un:
			case Code.Conv_Ovf_I2_Un:
			case Code.Conv_Ovf_I4_Un:
			case Code.Conv_Ovf_I8_Un:
			case Code.Conv_Ovf_U1_Un:
			case Code.Conv_Ovf_U2_Un:
			case Code.Conv_Ovf_U4_Un:
			case Code.Conv_Ovf_U8_Un:
			case Code.Conv_Ovf_I_Un:
			case Code.Conv_Ovf_U_Un:
			case Code.Ldlen:
				IL_95:
				switch (code)
				{
				case Code.Ldelem_Any:
				case Code.Stelem_Any:
					goto IL_10C;
				case Code.Unbox_Any:
					goto IL_19E;
				default:
				{
					if (code == Code.Mkrefany)
					{
						throw new NotImplementedException();
					}
					if (code == Code.Ldtoken)
					{
						TypeReference typeReference = instruction.Operand as TypeReference;
						if (typeReference != null)
						{
							this.AddTypeUsage(typeReference);
						}
						MethodReference methodReference = instruction.Operand as MethodReference;
						if (methodReference != null)
						{
							this.AddMethodUsage(methodReference);
						}
						FieldReference fieldReference = instruction.Operand as FieldReference;
						if (fieldReference != null)
						{
							this.AddClassUsage(fieldReference.DeclaringType);
						}
						goto IL_3CA;
					}
					if (code == Code.Ldftn)
					{
						MethodReference genericMethod = (MethodReference)instruction.Operand;
						this.AddMethodUsage(genericMethod);
						goto IL_3CA;
					}
					if (code == Code.Ldvirtftn)
					{
						MethodReference methodReference2 = (MethodReference)instruction.Operand;
						if (methodReference2.DeclaringType.IsInterface())
						{
							this.AddClassUsage(methodReference2.DeclaringType);
						}
						goto IL_3CA;
					}
					if (code == Code.Initobj)
					{
						goto IL_3CA;
					}
					if (code == Code.Constrained)
					{
						goto IL_19E;
					}
					if (code == Code.Call)
					{
						goto IL_28A;
					}
					if (code == Code.Sizeof)
					{
						TypeReference genericType = (TypeReference)instruction.Operand;
						this.AddClassUsage(genericType);
						goto IL_3CA;
					}
					MemberReference memberReference = instruction.Operand as MemberReference;
					if (memberReference != null)
					{
						throw new NotImplementedException();
					}
					goto IL_3CA;
				}
				}
				break;
			case Code.Ldobj:
			case Code.Ldfld:
			case Code.Ldflda:
			case Code.Stfld:
			case Code.Stobj:
			case Code.Ldelema:
				goto IL_10C;
			case Code.Newobj:
			{
				MethodReference methodReference3 = (MethodReference)instruction.Operand;
				if (methodReference3.DeclaringType.IsArray)
				{
					this.AddClassUsage(methodReference3.DeclaringType);
				}
				else
				{
					this.AddClassUsage(methodReference3.DeclaringType);
					this.AddMethodUsage(methodReference3);
				}
				goto IL_3CA;
			}
			case Code.Castclass:
			case Code.Isinst:
			case Code.Box:
				goto IL_19E;
			case Code.Unbox:
				goto IL_3CA;
			case Code.Ldsfld:
			case Code.Ldsflda:
			case Code.Stsfld:
			{
				FieldReference fieldReference2 = (FieldReference)instruction.Operand;
				TypeReference declaringType = fieldReference2.DeclaringType;
				this.AddStaticUsage(declaringType);
				goto IL_3CA;
			}
			case Code.Newarr:
			{
				TypeReference genericType2 = (TypeReference)instruction.Operand;
				this.AddArrayUsage(genericType2);
				goto IL_3CA;
			}
			}
			goto IL_95;
			IL_10C:
			goto IL_3CA;
			IL_19E:
			TypeReference genericType3 = (TypeReference)instruction.Operand;
			this.AddClassUsage(genericType3);
			goto IL_3CA;
			IL_28A:
			MethodReference methodReference4 = (MethodReference)instruction.Operand;
			if (!GenericSharingVisitor.Naming.IsSpecialArrayMethod(methodReference4))
			{
				if (!methodReference4.DeclaringType.IsSystemArray() || (!(methodReference4.Name == "GetGenericValueImpl") && !(methodReference4.Name == "SetGenericValueImpl")))
				{
					if (instruction.OpCode.Code == Code.Callvirt && methodReference4.DeclaringType.IsInterface() && !methodReference4.IsGenericInstance)
					{
						this.AddClassUsage(methodReference4.DeclaringType);
						if (instruction.Previous != null && instruction.Previous.OpCode.Code == Code.Constrained)
						{
							this.AddMethodUsage(methodReference4);
						}
					}
					else
					{
						this.AddMethodUsage(methodReference4);
					}
				}
			}
			MethodReference invokingMethod = (MethodReference)context.Data;
			if (GenericSharingVisitor.GenericSharingAnalysis.ShouldTryToCallStaticConstructorBeforeMethodCall(methodReference4, invokingMethod))
			{
				this.AddStaticUsage(methodReference4.DeclaringType);
			}
			IL_3CA:
			base.Visit(instruction, context);
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

		public void AddClassUsage(TypeReference genericType)
		{
			this.AddData(new RuntimeGenericTypeData(RuntimeGenericContextInfo.Class, genericType));
		}

		public void AddArrayUsage(TypeReference genericType)
		{
			this.AddData(new RuntimeGenericTypeData(RuntimeGenericContextInfo.Array, genericType));
		}

		public void AddTypeUsage(TypeReference genericType)
		{
			this.AddData(new RuntimeGenericTypeData(RuntimeGenericContextInfo.Type, genericType));
		}

		public void AddData(RuntimeGenericTypeData data)
		{
			GenericContextUsage genericContextUsage = GenericSharingVisitor.GenericUsageFor(data.GenericType);
			List<RuntimeGenericData> list;
			if (genericContextUsage == GenericContextUsage.Type)
			{
				list = this._typeList;
			}
			else if (genericContextUsage == GenericContextUsage.Method)
			{
				list = this._methodList;
			}
			else if (genericContextUsage == GenericContextUsage.Both)
			{
				list = this._methodList;
			}
			else
			{
				if (genericContextUsage == GenericContextUsage.None)
				{
					return;
				}
				throw new NotSupportedException("Invalid generic parameter usage");
			}
			if (list.FindIndex((RuntimeGenericData d) => d.InfoType == data.InfoType && TypeReferenceEqualityComparer.AreEqual(((RuntimeGenericTypeData)d).GenericType, data.GenericType, TypeComparisonMode.Exact)) == -1)
			{
				list.Add(data);
			}
		}

		public void AddMethodUsage(MethodReference genericMethod)
		{
			GenericContextUsage genericContextUsage = GenericSharingVisitor.GenericUsageFor(genericMethod);
			List<RuntimeGenericData> list;
			if (genericContextUsage == GenericContextUsage.Type)
			{
				list = this._typeList;
			}
			else if (genericContextUsage == GenericContextUsage.Method)
			{
				list = this._methodList;
			}
			else if (genericContextUsage == GenericContextUsage.Both)
			{
				list = this._methodList;
			}
			else
			{
				if (genericContextUsage == GenericContextUsage.None)
				{
					return;
				}
				throw new NotSupportedException("Invalid generic parameter usage");
			}
			RuntimeGenericMethodData data = new RuntimeGenericMethodData(RuntimeGenericContextInfo.Method, null, genericMethod);
			if (list.FindIndex((RuntimeGenericData d) => d.InfoType == data.InfoType && new MethodReferenceComparer().Equals(((RuntimeGenericMethodData)d).GenericMethod, data.GenericMethod)) == -1)
			{
				list.Add(data);
			}
		}

		internal static GenericContextUsage GenericUsageFor(MethodReference method)
		{
			GenericContextUsage genericContextUsage = GenericSharingVisitor.GenericUsageFor(method.DeclaringType);
			GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
			GenericContextUsage result;
			if (genericInstanceMethod != null)
			{
				foreach (TypeReference current in genericInstanceMethod.GenericArguments)
				{
					genericContextUsage |= GenericSharingVisitor.GenericUsageFor(current);
				}
				result = genericContextUsage;
			}
			else
			{
				result = genericContextUsage;
			}
			return result;
		}

		internal static GenericContextUsage GenericUsageFor(TypeReference type)
		{
			GenericParameter genericParameter = type as GenericParameter;
			GenericContextUsage result;
			if (genericParameter != null)
			{
				result = ((genericParameter.Type != GenericParameterType.Type) ? GenericContextUsage.Method : GenericContextUsage.Type);
			}
			else
			{
				ArrayType arrayType = type as ArrayType;
				if (arrayType != null)
				{
					result = GenericSharingVisitor.GenericUsageFor(arrayType.ElementType);
				}
				else
				{
					GenericInstanceType genericInstanceType = type as GenericInstanceType;
					if (genericInstanceType != null)
					{
						GenericContextUsage genericContextUsage = GenericContextUsage.None;
						foreach (TypeReference current in genericInstanceType.GenericArguments)
						{
							genericContextUsage |= GenericSharingVisitor.GenericUsageFor(current);
						}
						result = genericContextUsage;
					}
					else
					{
						PointerType pointerType = type as PointerType;
						if (pointerType != null)
						{
							result = GenericContextUsage.None;
						}
						else
						{
							TypeSpecification typeSpecification = type as TypeSpecification;
							if (typeSpecification != null)
							{
								throw new NotSupportedException("TypeSpecification found which is not supported");
							}
							result = GenericContextUsage.None;
						}
					}
				}
			}
			return result;
		}
	}
}
