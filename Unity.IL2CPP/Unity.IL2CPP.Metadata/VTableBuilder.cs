using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP.Metadata
{
	public class VTableBuilder
	{
		private readonly Dictionary<MethodReference, int> _methodSlots = new Dictionary<MethodReference, int>(new MethodReferenceComparer());

		private readonly Dictionary<TypeReference, VTable> _vtables = new Dictionary<TypeReference, VTable>(new TypeReferenceEqualityComparer());

		[Inject]
		public static IGenericSharingAnalysisService GenericSharingAnalysis;

		public int IndexFor(MethodDefinition method)
		{
			int result;
			if (method.DeclaringType.IsInterface)
			{
				this.SetupMethodSlotsForInterface(method.DeclaringType);
				result = this.GetSlot(method);
			}
			else
			{
				this.VTableFor(method.DeclaringType, null);
				if (!method.IsVirtual)
				{
					result = 65535;
				}
				else
				{
					int num = this._methodSlots[method];
					result = num;
				}
			}
			return result;
		}

		private int GetSlot(MethodReference method)
		{
			return this._methodSlots[method];
		}

		private void SetSlot(MethodReference method, int slot)
		{
			this._methodSlots[method] = slot;
		}

		public VTable VTableFor(TypeReference typeReference, TypeResolver resolver = null)
		{
			VTable vTable;
			VTable result;
			if (this._vtables.TryGetValue(typeReference, out vTable))
			{
				result = vTable;
			}
			else
			{
				TypeDefinition typeDefinition = typeReference.Resolve();
				if (typeDefinition.IsInterface && !typeDefinition.IsComOrWindowsRuntimeType())
				{
					throw new Exception();
				}
				int currentSlot = (typeDefinition.BaseType != null) ? this.VTableFor(typeReference.GetBaseType(), null).Slots.Count : 0;
				Dictionary<TypeReference, int> dictionary = this.SetupInterfaceOffsets(typeReference, ref currentSlot);
				GenericInstanceType genericInstanceType = typeReference as GenericInstanceType;
				ErrorInformation.CurrentlyProcessing.Type = typeDefinition;
				result = ((genericInstanceType == null) ? this.VTableForType(typeDefinition, dictionary, currentSlot) : this.VTableForGenericInstance(genericInstanceType, dictionary));
			}
			return result;
		}

		private static int VirtualMethodCount(TypeReference type)
		{
			return type.Resolve().Methods.Count((MethodDefinition m) => m.IsVirtual);
		}

		private Dictionary<TypeReference, int> SetupInterfaceOffsets(TypeReference type, ref int currentSlot)
		{
			Dictionary<TypeReference, int> dictionary = new Dictionary<TypeReference, int>(new TypeReferenceEqualityComparer());
			for (TypeReference baseType = type.GetBaseType(); baseType != null; baseType = baseType.GetBaseType())
			{
				VTable vTable = this.VTableFor(baseType, null);
				foreach (TypeReference current in baseType.GetInterfaces())
				{
					this.SetupMethodSlotsForInterface(current);
					int value = vTable.InterfaceOffsets[current];
					dictionary[current] = value;
				}
			}
			foreach (TypeReference current2 in type.GetInterfaces())
			{
				if (!dictionary.ContainsKey(current2))
				{
					this.SetupMethodSlotsForInterface(current2);
					dictionary.Add(current2, currentSlot);
					currentSlot += VTableBuilder.VirtualMethodCount(current2);
				}
			}
			return dictionary;
		}

		private void SetupMethodSlotsForInterface(TypeReference typeReference)
		{
			if (!typeReference.Resolve().IsInterface)
			{
				throw new Exception();
			}
			int num = 0;
			TypeDefinition typeDefinition = typeReference.Resolve();
			foreach (MethodDefinition current in from m in typeDefinition.Methods
			where m.IsVirtual && !m.IsStatic
			select m)
			{
				this.SetSlot(TypeResolver.For(typeReference).Resolve(current), num++);
			}
		}

		private VTable VTableForGenericInstance(GenericInstanceType genericInstanceType, Dictionary<TypeReference, int> offsets)
		{
			TypeDefinition typeDefinition = genericInstanceType.Resolve();
			VTable vTable = this.VTableFor(typeDefinition, null);
			List<MethodReference> list = new List<MethodReference>(vTable.Slots);
			TypeResolver typeResolver = new TypeResolver(genericInstanceType);
			for (int i = 0; i < list.Count; i++)
			{
				MethodReference methodReference = list[i];
				if (methodReference != null)
				{
					MethodReference methodReference2 = typeResolver.Resolve(methodReference);
					list[i] = methodReference2;
					this.SetSlot(methodReference2, this.GetSlot(methodReference));
				}
			}
			for (int j = 0; j < typeDefinition.Methods.Count; j++)
			{
				MethodDefinition methodDefinition = typeDefinition.Methods[j];
				if (methodDefinition.IsVirtual)
				{
					MethodReference methodReference3 = typeResolver.Resolve(methodDefinition);
					if (!this._methodSlots.ContainsKey(methodReference3))
					{
						int slot = this.GetSlot(methodDefinition);
						this.SetSlot(methodReference3, slot);
					}
				}
			}
			VTable vTable2 = new VTable(list.AsReadOnly(), offsets);
			this._vtables[genericInstanceType] = vTable2;
			return vTable2;
		}

		private VTable VTableForType(TypeDefinition typeDefinition, Dictionary<TypeReference, int> interfaceOffsets, int currentSlot)
		{
			TypeReference baseType = typeDefinition.BaseType;
			List<MethodReference> list = (baseType == null) ? new List<MethodReference>() : new List<MethodReference>(this.VTableFor(baseType, null).Slots);
			if (currentSlot > list.Count)
			{
				list.AddRange(new MethodReference[currentSlot - list.Count]);
			}
			Dictionary<MethodReference, MethodDefinition> overrides = VTableBuilder.CollectOverrides(typeDefinition);
			Dictionary<MethodReference, MethodReference> overrideMap = new Dictionary<MethodReference, MethodReference>(new MethodReferenceComparer());
			this.OverrideInterfaceMethods(interfaceOffsets, list, overrides, overrideMap);
			this.SetupInterfaceMethods(typeDefinition, interfaceOffsets, overrideMap, list);
			this.ValidateInterfaceMethodSlots(typeDefinition, interfaceOffsets, list);
			this.SetupClassMethods(list, typeDefinition, overrideMap);
			this.OverrideNonInterfaceMethods(overrides, list, overrideMap);
			VTableBuilder.ReplaceOverridenMethods(overrideMap, list);
			VTableBuilder.ValidateAllMethodSlots(typeDefinition, list);
			VTable vTable = new VTable(list.AsReadOnly(), (!typeDefinition.IsInterface || typeDefinition.IsComOrWindowsRuntimeInterface()) ? interfaceOffsets : null);
			this._vtables[typeDefinition] = vTable;
			return vTable;
		}

		private static Dictionary<MethodReference, MethodDefinition> CollectOverrides(TypeDefinition typeDefinition)
		{
			Dictionary<MethodReference, MethodDefinition> dictionary = new Dictionary<MethodReference, MethodDefinition>(new MethodReferenceComparer());
			foreach (MethodDefinition current in from m in typeDefinition.Methods
			where m.HasOverrides
			select m)
			{
				foreach (MethodReference current2 in current.Overrides)
				{
					dictionary.Add(current2, current);
				}
			}
			return dictionary;
		}

		private void OverrideInterfaceMethods(Dictionary<TypeReference, int> interfaceOffsets, List<MethodReference> slots, Dictionary<MethodReference, MethodDefinition> overrides, Dictionary<MethodReference, MethodReference> overrideMap)
		{
			foreach (KeyValuePair<MethodReference, MethodDefinition> current in overrides)
			{
				MethodReference key = current.Key;
				if (key.DeclaringType.Resolve().IsInterface)
				{
					int num = this.GetSlot(key);
					num += interfaceOffsets[current.Key.DeclaringType];
					slots[num] = current.Value;
					this.SetSlot(current.Value, num);
					overrideMap.Add(current.Key, current.Value);
				}
			}
		}

		private void SetupInterfaceMethods(TypeDefinition typeDefinition, Dictionary<TypeReference, int> interfaceOffsets, Dictionary<MethodReference, MethodReference> overrideMap, List<MethodReference> slots)
		{
			foreach (KeyValuePair<TypeReference, int> current in interfaceOffsets)
			{
				TypeReference itf = current.Key;
				int value = current.Value;
				TypeDefinition typeDefinition2 = itf.Resolve();
				this.SetupMethodSlotsForInterface(itf);
				bool interfaceIsExplicitlyImplementedByClass = VTableBuilder.InterfaceIsExplicitlyImplementedByClass(typeDefinition, itf);
				foreach (MethodReference current2 in from m in typeDefinition2.Methods
				where !m.IsStatic
				select TypeResolver.For(itf).Resolve(m))
				{
					int num = value + this.GetSlot(current2);
					MethodReference methodReference;
					if (!overrideMap.TryGetValue(current2, out methodReference))
					{
						foreach (MethodReference current3 in typeDefinition.GetVirtualMethods())
						{
							if (VTableBuilder.CheckInterfaceMethodOverride(current2, current3, true, interfaceIsExplicitlyImplementedByClass, slots[num] == null))
							{
								slots[num] = current3;
								this.SetSlot(current3, num);
							}
						}
						if (slots[num] == null && typeDefinition.BaseType != null)
						{
							VTable vTable = this.VTableFor(typeDefinition.BaseType, null);
							for (int i = vTable.Slots.Count - 1; i >= 0; i--)
							{
								MethodReference methodReference2 = vTable.Slots[i];
								if (methodReference2 != null)
								{
									if (VTableBuilder.CheckInterfaceMethodOverride(current2, methodReference2, false, false, true))
									{
										slots[num] = methodReference2;
										if (!this._methodSlots.ContainsKey(methodReference2))
										{
											this.SetSlot(methodReference2, num);
										}
									}
								}
							}
						}
					}
					else if (slots[num] != methodReference)
					{
						throw new Exception();
					}
				}
			}
		}

		private void ValidateInterfaceMethodSlots(TypeDefinition typeDefinition, Dictionary<TypeReference, int> interfaceOffsets, List<MethodReference> slots)
		{
			if (!typeDefinition.IsAbstract)
			{
				foreach (KeyValuePair<TypeReference, int> current in interfaceOffsets)
				{
					TypeReference key = current.Key;
					int value = current.Value;
					TypeResolver @object = TypeResolver.For(key);
					foreach (MethodReference current2 in (from m in key.Resolve().Methods
					where !m.IsStatic
					select m).Select(new Func<MethodDefinition, MethodReference>(@object.Resolve)))
					{
						int index = value + this.GetSlot(current2);
						if (slots[index] == null)
						{
							throw new Exception(string.Format("Interface {0} method {1} not implemented on non-abstract class {2}", key.FullName, current2.FullName, typeDefinition.FullName));
						}
					}
				}
			}
		}

		private void SetupClassMethods(List<MethodReference> slots, TypeDefinition typeDefinition, Dictionary<MethodReference, MethodReference> overrideMap)
		{
			foreach (MethodDefinition current in from m in typeDefinition.Methods
			where m.IsVirtual
			select m)
			{
				if (!current.IsNewSlot)
				{
					int num = -1;
					for (TypeReference baseType = typeDefinition.GetBaseType(); baseType != null; baseType = baseType.GetBaseType())
					{
						foreach (MethodReference current2 in baseType.GetVirtualMethods())
						{
							if (!(current.Name != current2.Name) && VirtualMethodResolution.MethodSignaturesMatch(current, current2))
							{
								num = this.GetSlot(current2);
								overrideMap.Add(current2, current);
								break;
							}
						}
						if (num >= 0)
						{
							break;
						}
					}
					if (num >= 0)
					{
						this.SetSlot(current, num);
					}
				}
				if (current.IsNewSlot && !current.IsFinal && this._methodSlots.ContainsKey(current))
				{
					this._methodSlots.Remove(current);
				}
				if (!this._methodSlots.ContainsKey(current))
				{
					int count = slots.Count;
					slots.Add(null);
					this.SetSlot(current, count);
				}
				int slot = this.GetSlot(current);
				slots[slot] = ((!current.IsAbstract || typeDefinition.IsComOrWindowsRuntimeInterface()) ? current : null);
			}
		}

		private void OverrideNonInterfaceMethods(Dictionary<MethodReference, MethodDefinition> overrides, List<MethodReference> slots, Dictionary<MethodReference, MethodReference> overrideMap)
		{
			foreach (KeyValuePair<MethodReference, MethodDefinition> current in overrides)
			{
				MethodReference key = current.Key;
				MethodDefinition value = current.Value;
				TypeReference declaringType = key.DeclaringType;
				if (!declaringType.Resolve().IsInterface)
				{
					int slot = this.GetSlot(key);
					slots[slot] = value;
					this.SetSlot(value, slot);
					MethodReference methodReference;
					overrideMap.TryGetValue(key, out methodReference);
					if (methodReference != null)
					{
						if (!MethodReferenceComparer.AreEqual(methodReference, value))
						{
							throw new InvalidOperationException(string.Format("Error while creating VTable for {0}. The base method {1} is implemented both by {2} and {3}.", new object[]
							{
								declaringType,
								key,
								methodReference,
								value
							}));
						}
					}
					else
					{
						overrideMap.Add(key, value);
					}
				}
			}
		}

		private static void ReplaceOverridenMethods(Dictionary<MethodReference, MethodReference> overrideMap, List<MethodReference> slots)
		{
			if (overrideMap.Count > 0)
			{
				for (int i = 0; i < slots.Count; i++)
				{
					if (slots[i] != null)
					{
						MethodReference value;
						if (overrideMap.TryGetValue(slots[i], out value))
						{
							slots[i] = value;
						}
					}
				}
			}
		}

		private static void ValidateAllMethodSlots(TypeDefinition typeDefinition, IEnumerable<MethodReference> slots)
		{
			if (!typeDefinition.IsAbstract)
			{
				foreach (MethodReference current in slots)
				{
					if (current == null || (current.Resolve().IsAbstract && !current.Resolve().DeclaringType.IsComOrWindowsRuntimeInterface()) || current.Resolve().IsStatic)
					{
						throw new Exception(string.Format("Invalid method '{0}' found in vtable for '{1}'", (current != null) ? current.FullName : "null", typeDefinition.FullName));
					}
				}
			}
		}

		private static bool InterfaceIsExplicitlyImplementedByClass(TypeDefinition typeDefinition, TypeReference itf)
		{
			return typeDefinition.BaseType == null || typeDefinition.Interfaces.Any((TypeReference classItf) => TypeReferenceEqualityComparer.AreEqual(itf, classItf, TypeComparisonMode.Exact));
		}

		private static bool CheckInterfaceMethodOverride(MethodReference itfMethod, MethodReference virtualMethod, bool requireNewslot, bool interfaceIsExplicitlyImplementedByClass, bool slotIsEmpty)
		{
			bool result;
			if (itfMethod.Name == virtualMethod.Name)
			{
				if (!virtualMethod.Resolve().IsPublic)
				{
					result = false;
				}
				else
				{
					if (!slotIsEmpty)
					{
						if (requireNewslot)
						{
							if (!interfaceIsExplicitlyImplementedByClass)
							{
								result = false;
								return result;
							}
							if (!virtualMethod.Resolve().IsNewSlot)
							{
								result = false;
								return result;
							}
						}
					}
					result = VirtualMethodResolution.MethodSignaturesMatch(itfMethod, virtualMethod);
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		internal static MethodReference InflatedMethodFromMethodDefinition(GenericInstanceType genericInstanceType, MethodDefinition method)
		{
			if (!method.DeclaringType.HasGenericParameters)
			{
				throw new Exception("Declaring type should be generic");
			}
			if (method.DeclaringType.Resolve() != genericInstanceType.Resolve())
			{
				throw new NotSupportedException();
			}
			return VTableBuilder.CloneMethodReference(genericInstanceType, method);
		}

		internal static MethodReference CloneMethodReference(GenericInstanceType genericInstanceType, MethodReference method)
		{
			MethodReference methodReference = new MethodReference(method.Name, method.ReturnType, genericInstanceType)
			{
				HasThis = method.HasThis,
				ExplicitThis = method.ExplicitThis,
				CallingConvention = method.CallingConvention
			};
			foreach (ParameterDefinition current in method.Parameters)
			{
				methodReference.Parameters.Add(new ParameterDefinition(current.Name, current.Attributes, current.ParameterType));
			}
			foreach (GenericParameter current2 in method.GenericParameters)
			{
				methodReference.GenericParameters.Add(new GenericParameter(current2.Name, methodReference));
			}
			return methodReference;
		}

		internal MethodReference GetVirtualMethodTargetMethodForConstrainedCallOnValueType(TypeReference type, MethodReference method)
		{
			MethodDefinition methodDefinition = method.Resolve();
			MethodReference result;
			if (!methodDefinition.IsVirtual)
			{
				result = method;
			}
			else
			{
				int num = this.IndexFor(methodDefinition);
				VTable vTable = this.VTableFor(type, null);
				if (method.DeclaringType.IsInterface())
				{
					int num2;
					if (vTable.InterfaceOffsets.TryGetValue(method.DeclaringType, out num2))
					{
						result = vTable.Slots[num2 + num];
					}
					else
					{
						result = null;
					}
				}
				else
				{
					result = vTable.Slots[num];
				}
			}
			return result;
		}
	}
}
