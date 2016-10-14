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
			public readonly MethodDefinition InterfaceMethod;

			public MethodDefinition ManagedMethod;

			public InterfaceMethodMapping(MethodDefinition interfaceMethod, MethodDefinition managedMethod)
			{
				this.InterfaceMethod = interfaceMethod;
				this.ManagedMethod = managedMethod;
			}
		}

		[Inject]
		public static INamingService Naming;

		private readonly TypeDefinition _type;

		private readonly string _typeName;

		private readonly string _functionName;

		private readonly string _functionDeclaration;

		[CompilerGenerated]
		private static Func<MethodDefinition, TypeDefinition[], IEnumerable<MethodDefinition>> <>f__mg$cache0;

		[CompilerGenerated]
		private static Func<MethodDefinition, TypeDefinition[], IEnumerable<MethodDefinition>> <>f__mg$cache1;

		public string FunctionName
		{
			get
			{
				return this._functionName;
			}
		}

		public string FunctionDeclaration
		{
			get
			{
				return this._functionDeclaration;
			}
		}

		public CCWWriter(TypeDefinition type)
		{
			this._type = type;
			this._typeName = CCWWriter.Naming.ForTypeNameOnly(type) + "CCW";
			this._functionName = CCWWriter.Naming.ForTypeNameOnly(type) + "_create_ccw";
			this._functionDeclaration = string.Format("extern \"C\" Il2CppIUnknown* {0}(Il2CppObject* obj, const Il2CppGuid& iid)", this._functionName);
		}

		public void WriteTypeDefinition(CppCodeWriter writer)
		{
			string text = "il2cpp::vm::ManagedObjectBase<" + this._typeName;
			TypeDefinition[] array = (from i in this._type.Interfaces
			where i.IsComOrWindowsRuntimeInterface()
			select i into ci
			select ci.Resolve()).ToArray<TypeDefinition>();
			TypeDefinition[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				TypeDefinition typeDefinition = array2[j];
				writer.AddIncludeForTypeDefinition(typeDefinition);
				text += ", ";
				text += CCWWriter.Naming.ForTypeNameOnly(typeDefinition);
			}
			text += '>';
			writer.WriteLine("struct NOVTABLE {0} : {1}", new object[]
			{
				this._typeName,
				text
			});
			using (new BlockWriter(writer, true))
			{
				writer.WriteLine("inline {0}(Il2CppObject* obj) : {1}(obj) {{}}", new object[]
				{
					this._typeName,
					text
				});
				foreach (CCWWriter.InterfaceMethodMapping current in this.GetComInterfaceMethodMappings())
				{
					string signature = ComInterfaceWriter.GetSignature(current.InterfaceMethod, TypeResolver.For(this._type), null);
					writer.WriteLine(signature + ';');
				}
			}
		}

		public void WriteMethodDefinitions(CppCodeWriter writer)
		{
			using (IEnumerator<CCWWriter.InterfaceMethodMapping> enumerator = this.GetComInterfaceMethodMappings().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CCWWriter.<WriteMethodDefinitions>c__AnonStorey0 <WriteMethodDefinitions>c__AnonStorey = new CCWWriter.<WriteMethodDefinitions>c__AnonStorey0();
					<WriteMethodDefinitions>c__AnonStorey.mapping = enumerator.Current;
					MarshalType marshalType = (!<WriteMethodDefinitions>c__AnonStorey.mapping.InterfaceMethod.DeclaringType.IsWindowsRuntime) ? MarshalType.COM : MarshalType.WindowsRuntime;
					MethodWriter.WriteMethodWithMetadataInitialization(writer, ComInterfaceWriter.GetSignature(<WriteMethodDefinitions>c__AnonStorey.mapping.InterfaceMethod, TypeResolver.For(this._type), this._typeName), <WriteMethodDefinitions>c__AnonStorey.mapping.ManagedMethod.FullName, delegate(CppCodeWriter bodyWriter, MetadataUsage metadataUsage, MethodUsage methodUsage)
					{
						IRuntimeMetadataAccess defaultRuntimeMetadataAccess = MethodWriter.GetDefaultRuntimeMetadataAccess(<WriteMethodDefinitions>c__AnonStorey.mapping.ManagedMethod, metadataUsage, methodUsage);
						ComCallableWrapperMethodBodyWriter comCallableWrapperMethodBodyWriter = new ComCallableWrapperMethodBodyWriter(<WriteMethodDefinitions>c__AnonStorey.mapping.ManagedMethod, <WriteMethodDefinitions>c__AnonStorey.mapping.InterfaceMethod, marshalType);
						comCallableWrapperMethodBodyWriter.WriteMethodBody(bodyWriter, defaultRuntimeMetadataAccess);
					}, CCWWriter.Naming.ForMethod(<WriteMethodDefinitions>c__AnonStorey.mapping.InterfaceMethod) + "_CCW_" + CCWWriter.Naming.ForMethod(<WriteMethodDefinitions>c__AnonStorey.mapping.ManagedMethod));
				}
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
				writer.WriteLine("if (obj == {0})", new object[]
				{
					CCWWriter.Naming.Null
				});
				using (new BlockWriter(writer, false))
				{
					writer.WriteLine("return {0};", new object[]
					{
						CCWWriter.Naming.Null
					});
				}
				writer.WriteLine("il2cpp::vm::ComObject<{0}>* instance = il2cpp::vm::ComObject<{0}>::__CreateInstance(obj);", new object[]
				{
					this._typeName
				});
				writer.WriteLine("Il2CppIUnknown* result;");
				writer.WriteLine("const il2cpp_hresult_t hr = instance->QueryInterface(iid, reinterpret_cast<void**>(&result));");
				writer.WriteLine("if (IL2CPP_HR_FAILED(hr))");
				using (new BlockWriter(writer, false))
				{
					writer.WriteLine("instance->__DestroyInstance();");
					writer.WriteLine("il2cpp_codegen_com_raise_exception(hr);");
				}
				writer.WriteLine("return result;");
			}
		}

		private IEnumerable<CCWWriter.InterfaceMethodMapping> GetComInterfaceMethodMappings()
		{
			List<CCWWriter.InterfaceMethodMapping> list = new List<CCWWriter.InterfaceMethodMapping>();
			TypeDefinition[] array = (from i in this._type.Interfaces
			select i.Resolve() into i
			where i.IsComOrWindowsRuntimeType()
			select i).ToArray<TypeDefinition>();
			TypeDefinition arg_81_0 = this._type;
			List<CCWWriter.InterfaceMethodMapping> arg_81_1 = list;
			TypeDefinition[] arg_81_2 = array;
			if (CCWWriter.<>f__mg$cache0 == null)
			{
				CCWWriter.<>f__mg$cache0 = new Func<MethodDefinition, TypeDefinition[], IEnumerable<MethodDefinition>>(CCWWriter.GetInterfaceMethods);
			}
			CCWWriter.GetComInterfaceMethodMappings(arg_81_0, arg_81_1, arg_81_2, CCWWriter.<>f__mg$cache0);
			TypeDefinition arg_AB_0 = this._type;
			List<CCWWriter.InterfaceMethodMapping> arg_AB_1 = list;
			TypeDefinition[] arg_AB_2 = array;
			if (CCWWriter.<>f__mg$cache1 == null)
			{
				CCWWriter.<>f__mg$cache1 = new Func<MethodDefinition, TypeDefinition[], IEnumerable<MethodDefinition>>(CCWWriter.GetOverridenInterfaceMethods);
			}
			CCWWriter.GetComInterfaceMethodMappings(arg_AB_0, arg_AB_1, arg_AB_2, CCWWriter.<>f__mg$cache1);
			return list;
		}

		private static void GetComInterfaceMethodMappings(TypeDefinition type, List<CCWWriter.InterfaceMethodMapping> mappings, TypeDefinition[] interfaces, Func<MethodDefinition, TypeDefinition[], IEnumerable<MethodDefinition>> getInterfaceMethods)
		{
			if (type.BaseType != null)
			{
				CCWWriter.GetComInterfaceMethodMappings(type.BaseType.Resolve(), mappings, interfaces, getInterfaceMethods);
			}
			foreach (MethodDefinition current in from m in type.Methods
			where m.IsVirtual && m.IsNewSlot
			select m)
			{
				using (IEnumerator<MethodDefinition> enumerator2 = getInterfaceMethods(current, interfaces).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						MethodDefinition interfaceMethod = enumerator2.Current;
						CCWWriter.InterfaceMethodMapping interfaceMethodMapping = mappings.FirstOrDefault((CCWWriter.InterfaceMethodMapping m) => TypeReferenceEqualityComparer.AreEqual(m.InterfaceMethod.DeclaringType, interfaceMethod.DeclaringType, TypeComparisonMode.Exact) && m.InterfaceMethod.Name == interfaceMethod.Name && VirtualMethodResolution.MethodSignaturesMatch(m.InterfaceMethod, interfaceMethod));
						if (interfaceMethodMapping != null)
						{
							interfaceMethodMapping.ManagedMethod = current;
						}
						else
						{
							mappings.Add(new CCWWriter.InterfaceMethodMapping(interfaceMethod, current));
						}
					}
				}
			}
		}

		private static IEnumerable<MethodDefinition> GetInterfaceMethods(MethodDefinition method, TypeDefinition[] interfaces)
		{
			return (!method.HasOverrides) ? interfaces.SelectMany((TypeDefinition i) => from m in i.Methods
			where m.Name == method.Name && VirtualMethodResolution.MethodSignaturesMatch(m, method)
			select m) : Enumerable.Empty<MethodDefinition>();
		}

		private static IEnumerable<MethodDefinition> GetOverridenInterfaceMethods(MethodDefinition method, TypeDefinition[] interfaces)
		{
			IEnumerable<MethodDefinition> arg_60_0;
			if (method.HasOverrides)
			{
				arg_60_0 = from m in method.Overrides
				where m.DeclaringType.IsComOrWindowsRuntimeInterface()
				select m.Resolve();
			}
			else
			{
				arg_60_0 = Enumerable.Empty<MethodDefinition>();
			}
			return arg_60_0;
		}
	}
}
