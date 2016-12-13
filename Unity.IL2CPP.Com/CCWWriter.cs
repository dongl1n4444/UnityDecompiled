using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.ILPreProcessor;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Marshaling;
using Unity.IL2CPP.Marshaling.BodyWriters.NativeToManaged;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP.Com
{
	public class CCWWriter
	{
		private sealed class InterfaceMethodMapping
		{
			public readonly MethodReference InterfaceMethod;

			public MethodReference ManagedMethod;

			public InterfaceMethodMapping(MethodReference interfaceMethod, MethodReference managedMethod)
			{
				this.InterfaceMethod = interfaceMethod;
				this.ManagedMethod = managedMethod;
			}
		}

		private struct InterfaceImplementationMapping
		{
			public readonly TypeDefinition DeclaringType;

			public readonly InterfaceImplementation InterfaceImplementation;

			public InterfaceImplementationMapping(TypeDefinition declaringType, InterfaceImplementation interfaceImplementation)
			{
				this.DeclaringType = declaringType;
				this.InterfaceImplementation = interfaceImplementation;
			}
		}

		[Inject]
		public static INamingService Naming;

		private readonly TypeDefinition _type;

		private readonly string _typeName;

		private readonly string _functionDeclaration;

		private readonly List<TypeReference> _interfacesToImplement;

		private readonly List<CCWWriter.InterfaceMethodMapping> _interfaceMethodMappings;

		private readonly TypeReference[] _allInteropInterfaces;

		private readonly TypeReference[] _interfacesToForwardToBaseClass;

		private readonly bool _canForwardMethodsToBaseClass;

		[CompilerGenerated]
		private static Func<CCWWriter.InterfaceImplementationMapping, bool> <>f__mg$cache0;

		public CCWWriter(TypeDefinition type)
		{
			this._type = type;
			this._typeName = CCWWriter.Naming.ForTypeNameOnly(type) + "CCW";
			this._functionDeclaration = string.Format("extern \"C\" Il2CppIManagedObjectHolder* {0}(Il2CppObject* obj)", CCWWriter.Naming.ForCreateComCallableWrapperFunction(type));
			this._interfaceMethodMappings = new List<CCWWriter.InterfaceMethodMapping>();
			this._interfacesToImplement = new List<TypeReference>();
			this._canForwardMethodsToBaseClass = type.GetTypeHierarchy().Any((TypeDefinition t) => t.IsComOrWindowsRuntimeType());
			IEnumerable<CCWWriter.InterfaceImplementationMapping> allImplementedInteropInterfacesInTypeHierarchy = CCWWriter.GetAllImplementedInteropInterfacesInTypeHierarchy(this._type);
			IEnumerable<CCWWriter.InterfaceImplementationMapping> arg_B2_0 = allImplementedInteropInterfacesInTypeHierarchy;
			if (CCWWriter.<>f__mg$cache0 == null)
			{
				CCWWriter.<>f__mg$cache0 = new Func<CCWWriter.InterfaceImplementationMapping, bool>(CCWWriter.CanImplementInCCW);
			}
			IEnumerable<TypeReference> enumerable = (from i in arg_B2_0.Where(CCWWriter.<>f__mg$cache0)
			select i.InterfaceImplementation.InterfaceType).Distinct(new TypeReferenceEqualityComparer());
			VTable vTable = new VTableBuilder().VTableFor(type, null);
			foreach (TypeReference current in enumerable)
			{
				int num = vTable.InterfaceOffsets[current];
				IEnumerable<MethodReference> enumerable2 = from m in current.GetMethods()
				where m.HasThis && m.Resolve().IsVirtual
				select m;
				bool flag = false;
				List<CCWWriter.InterfaceMethodMapping> list = new List<CCWWriter.InterfaceMethodMapping>();
				int num2 = 0;
				foreach (MethodReference current2 in enumerable2)
				{
					if (!current2.IsStripped())
					{
						MethodReference methodReference = vTable.Slots[num + num2];
						list.Add(new CCWWriter.InterfaceMethodMapping(current2, methodReference));
						num2++;
						if (!methodReference.DeclaringType.Resolve().IsComOrWindowsRuntimeType())
						{
							flag = true;
						}
					}
					else
					{
						list.Add(new CCWWriter.InterfaceMethodMapping(current2, null));
					}
				}
				if (!this._canForwardMethodsToBaseClass || flag)
				{
					this._interfacesToImplement.Add(current);
					this._interfaceMethodMappings.AddRange(list);
				}
			}
			this._allInteropInterfaces = (from i in allImplementedInteropInterfacesInTypeHierarchy
			select i.InterfaceImplementation.InterfaceType).ToArray<TypeReference>();
			this._interfacesToForwardToBaseClass = this._allInteropInterfaces.Except(this._interfacesToImplement, new TypeReferenceEqualityComparer()).ToArray<TypeReference>();
		}

		public void WriteTypeDefinition(CppCodeWriter writer)
		{
			foreach (TypeReference current in this._interfacesToImplement)
			{
				writer.AddIncludeForTypeDefinition(current);
			}
			string baseTypeName = this.GetBaseTypeName();
			writer.WriteLine(string.Format("struct {0} : {1}", this._typeName, baseTypeName));
			using (new BlockWriter(writer, true))
			{
				writer.WriteLine(string.Format("inline {0}(Il2CppObject* obj) : {1}(obj) {{}}", this._typeName, baseTypeName));
				if (this._interfacesToForwardToBaseClass.Length > 0)
				{
					writer.WriteLine("virtual il2cpp_hresult_t STDCALL QueryInterface(const Il2CppGuid& iid, void** object) IL2CPP_OVERRIDE;");
					writer.WriteLine("virtual il2cpp_hresult_t STDCALL GetIids(uint32_t* iidCount, Il2CppGuid** iids) IL2CPP_OVERRIDE;");
				}
				foreach (CCWWriter.InterfaceMethodMapping current2 in this._interfaceMethodMappings)
				{
					string signature = ComInterfaceWriter.GetSignature(current2.ManagedMethod ?? current2.InterfaceMethod, current2.InterfaceMethod, TypeResolver.For(this._type), null);
					writer.WriteLine(signature + " IL2CPP_OVERRIDE;");
				}
			}
		}

		public void WriteMethodDefinitions(CppCodeWriter writer)
		{
			if (this._interfacesToForwardToBaseClass.Length > 0)
			{
				this.WriteQueryInterfaceDefinition(writer);
				this.WriteGetIidsDefinition(writer);
			}
			using (List<CCWWriter.InterfaceMethodMapping>.Enumerator enumerator = this._interfaceMethodMappings.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CCWWriter.<WriteMethodDefinitions>c__AnonStorey0 <WriteMethodDefinitions>c__AnonStorey = new CCWWriter.<WriteMethodDefinitions>c__AnonStorey0();
					<WriteMethodDefinitions>c__AnonStorey.mapping = enumerator.Current;
					<WriteMethodDefinitions>c__AnonStorey.$this = this;
					MarshalType marshalType = (!<WriteMethodDefinitions>c__AnonStorey.mapping.InterfaceMethod.DeclaringType.Resolve().IsWindowsRuntime) ? MarshalType.COM : MarshalType.WindowsRuntime;
					string signature = ComInterfaceWriter.GetSignature(<WriteMethodDefinitions>c__AnonStorey.mapping.ManagedMethod ?? <WriteMethodDefinitions>c__AnonStorey.mapping.InterfaceMethod, <WriteMethodDefinitions>c__AnonStorey.mapping.InterfaceMethod, TypeResolver.For(this._type), this._typeName);
					MethodWriter.WriteMethodWithMetadataInitialization(writer, signature, (<WriteMethodDefinitions>c__AnonStorey.mapping.ManagedMethod ?? <WriteMethodDefinitions>c__AnonStorey.mapping.InterfaceMethod).FullName, delegate(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
					{
						if (<WriteMethodDefinitions>c__AnonStorey.mapping.InterfaceMethod.IsStripped())
						{
							bodyWriter.WriteCommentedLine("Managed method has been stripped");
							bodyWriter.WriteLine("return IL2CPP_E_ILLEGAL_METHOD_CALL;");
						}
						else if (!<WriteMethodDefinitions>c__AnonStorey.mapping.ManagedMethod.Resolve().DeclaringType.IsComOrWindowsRuntimeType())
						{
							IRuntimeMetadataAccess defaultRuntimeMetadataAccess = MethodWriter.GetDefaultRuntimeMetadataAccess(<WriteMethodDefinitions>c__AnonStorey.mapping.ManagedMethod, metadataUsage, methodUsage);
							ComCallableWrapperMethodBodyWriter comCallableWrapperMethodBodyWriter = new ComCallableWrapperMethodBodyWriter(<WriteMethodDefinitions>c__AnonStorey.mapping.ManagedMethod, <WriteMethodDefinitions>c__AnonStorey.mapping.InterfaceMethod, marshalType);
							comCallableWrapperMethodBodyWriter.WriteMethodBody(bodyWriter, defaultRuntimeMetadataAccess);
						}
						else
						{
							string text = CCWWriter.Naming.ForVariable(<WriteMethodDefinitions>c__AnonStorey.$this._type);
							string text2 = CCWWriter.Naming.ForComTypeInterfaceFieldGetter(<WriteMethodDefinitions>c__AnonStorey.mapping.InterfaceMethod.DeclaringType);
							string text3 = CCWWriter.Naming.ForMethod(<WriteMethodDefinitions>c__AnonStorey.mapping.InterfaceMethod);
							string text4 = ComInterfaceWriter.BuildMethodParameterList(<WriteMethodDefinitions>c__AnonStorey.mapping.ManagedMethod, <WriteMethodDefinitions>c__AnonStorey.mapping.InterfaceMethod, TypeResolver.For(<WriteMethodDefinitions>c__AnonStorey.$this._type), marshalType, false);
							bodyWriter.WriteLine(string.Format("return (({0})GetManagedObjectInline())->{1}()->{2}({3});", new object[]
							{
								text,
								text2,
								text3,
								text4
							}));
						}
					}, CCWWriter.Naming.ForMethod(<WriteMethodDefinitions>c__AnonStorey.mapping.InterfaceMethod) + "_CCW_" + ((<WriteMethodDefinitions>c__AnonStorey.mapping.ManagedMethod == null) ? this._typeName : CCWWriter.Naming.ForMethod(<WriteMethodDefinitions>c__AnonStorey.mapping.ManagedMethod)));
				}
			}
		}

		private void WriteQueryInterfaceDefinition(CppCodeWriter writer)
		{
			writer.WriteLine(string.Format("il2cpp_hresult_t STDCALL {0}::QueryInterface(const Il2CppGuid& iid, void** object)", this._typeName));
			using (new BlockWriter(writer, false))
			{
				string arg = (from i in this._interfacesToForwardToBaseClass
				select string.Format("!::memcmp(&iid, &{0}::IID, sizeof(Il2CppGuid))", CCWWriter.Naming.ForTypeNameOnly(i))).AggregateWith(" || ");
				writer.WriteLine(string.Format("if ({0})", arg));
				using (new BlockWriter(writer, false))
				{
					string arg2 = CCWWriter.Naming.ForVariable(this._type);
					string arg3 = CCWWriter.Naming.ForIl2CppComObjectIdentityField();
					writer.WriteLine(string.Format("return (({0})GetManagedObjectInline())->{1}->QueryInterface(iid, object);", arg2, arg3));
				}
				writer.WriteLine(string.Format("return {0}::QueryInterface(iid, object);", this.GetBaseTypeName()));
			}
		}

		private void WriteGetIidsDefinition(CppCodeWriter writer)
		{
			writer.WriteLine(string.Format("il2cpp_hresult_t STDCALL {0}::GetIids(uint32_t* iidCount, Il2CppGuid** iids)", this._typeName));
			using (new BlockWriter(writer, false))
			{
				int num = 0;
				TypeReference[] allInteropInterfaces = this._allInteropInterfaces;
				for (int i = 0; i < allInteropInterfaces.Length; i++)
				{
					TypeReference typeReference = allInteropInterfaces[i];
					if (typeReference.Resolve().IsWindowsRuntime)
					{
						num++;
					}
				}
				writer.WriteLine(string.Format("Il2CppGuid* interfaceIds = il2cpp_codegen_marshal_allocate_array<Il2CppGuid>({0});", num));
				int num2 = 0;
				TypeReference[] allInteropInterfaces2 = this._allInteropInterfaces;
				for (int j = 0; j < allInteropInterfaces2.Length; j++)
				{
					TypeReference typeReference2 = allInteropInterfaces2[j];
					if (typeReference2.Resolve().IsWindowsRuntime)
					{
						string arg = CCWWriter.Naming.ForTypeNameOnly(typeReference2);
						writer.WriteLine(string.Format("interfaceIds[{0}] = {1}::IID;", num2, arg));
						num2++;
					}
				}
				writer.WriteLine();
				writer.WriteLine(string.Format("*iidCount = {0};", num));
				writer.WriteLine("*iids = interfaceIds;");
				writer.WriteLine("return IL2CPP_S_OK;");
			}
		}

		public void WriteCreateCCWDeclaration(CppCodeWriter writer)
		{
			writer.WriteLine(this._functionDeclaration + ';');
		}

		public void WriteCreateCCWDefinition(CppCodeWriter writer)
		{
			writer.WriteLine(this._functionDeclaration);
			using (new BlockWriter(writer, false))
			{
				writer.WriteLine(string.Format("return {0}::__CreateInstance(obj);", this._typeName));
			}
		}

		private string GetBaseTypeName()
		{
			string text = "il2cpp::vm::ComObjectBase<" + this._typeName;
			bool flag = false;
			foreach (TypeReference current in this._interfacesToImplement)
			{
				text += ", ";
				text += CCWWriter.Naming.ForTypeNameOnly(current);
				if (current.Resolve().IsWindowsRuntime)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				text += ", Il2CppIInspectable";
			}
			text += '>';
			return text;
		}

		private static IEnumerable<CCWWriter.InterfaceImplementationMapping> GetAllImplementedInteropInterfacesInTypeHierarchy(TypeDefinition typeDefinition)
		{
			return typeDefinition.GetTypeHierarchy().SelectMany((TypeDefinition t) => from i in t.Interfaces
			where i.InterfaceType.IsComOrWindowsRuntimeInterface()
			select new CCWWriter.InterfaceImplementationMapping(t, i));
		}

		private static bool CanImplementInCCW(CCWWriter.InterfaceImplementationMapping mapping)
		{
			bool result;
			if (!mapping.DeclaringType.IsComOrWindowsRuntimeType())
			{
				result = true;
			}
			else
			{
				result = mapping.InterfaceImplementation.CustomAttributes.Any((CustomAttribute ca) => ca.AttributeType.FullName == "Windows.Foundation.Metadata.OverridableAttribute");
			}
			return result;
		}
	}
}
