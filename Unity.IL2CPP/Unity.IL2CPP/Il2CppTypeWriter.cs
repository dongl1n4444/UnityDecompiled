using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.IoC;
using Unity.IL2CPP.IoCServices;
using Unity.IL2CPP.Metadata;

namespace Unity.IL2CPP
{
	public class Il2CppTypeWriter : MetadataWriter
	{
		[Inject]
		public static IIl2CppTypeCollectorReaderService Il2CppTypeCollectorReader;

		public Il2CppTypeWriter(CppCodeWriter writer) : base(writer)
		{
		}

		public TableInfo WriteIl2CppTypeDefinitions(IMetadataCollection metadataCollection)
		{
			base.Writer.AddCodeGenIncludes();
			IDictionary<Il2CppTypeData, int> items = Il2CppTypeWriter.Il2CppTypeCollectorReader.Items;
			foreach (IGrouping<TypeReference, Il2CppTypeData> current in items.Keys.GroupBy((Il2CppTypeData entry) => entry.Type.GetNonPinnedAndNonByReferenceType(), new TypeReferenceEqualityComparer()))
			{
				base.Writer.WriteLine();
				TypeReference key = current.Key;
				GenericParameter genericParameter = key as GenericParameter;
				GenericInstanceType genericInstanceType = key as GenericInstanceType;
				ArrayType arrayType = key as ArrayType;
				PointerType pointerType = key as PointerType;
				string text;
				if (genericParameter != null)
				{
					text = "(void*)" + metadataCollection.GetGenericParameterIndex(genericParameter);
				}
				else if (genericInstanceType != null)
				{
					text = this.WriteGenericInstanceTypeDataValue(genericInstanceType, metadataCollection);
				}
				else if (arrayType != null)
				{
					text = this.WriteArrayDataValue(arrayType);
				}
				else if (pointerType != null)
				{
					text = this.WritePointerDataValue(pointerType);
				}
				else
				{
					text = "(void*)" + metadataCollection.GetTypeInfoIndex(key.Resolve()).ToString(CultureInfo.InvariantCulture);
				}
				foreach (Il2CppTypeData current2 in current)
				{
					base.Writer.WriteLine("extern const Il2CppType {0} = {{ {1}, {2}, {3}, {4}, {5}, {6} }};", new object[]
					{
						MetadataWriter.Naming.ForIl2CppType(current2.Type, current2.Attrs),
						text,
						current2.Attrs.ToString(CultureInfo.InvariantCulture),
						Il2CppTypeSupport.For(current2.Type),
						"0",
						(!current2.Type.IsByReference) ? "0" : "1",
						(!current2.Type.IsPinned) ? "0" : "1"
					});
				}
			}
			return MetadataWriter.WriteTable<KeyValuePair<Il2CppTypeData, int>>(base.Writer, "extern const Il2CppType* const ", "g_Il2CppTypeTable", (from kvp in items
			orderby kvp.Value
			select kvp).ToArray<KeyValuePair<Il2CppTypeData, int>>(), (KeyValuePair<Il2CppTypeData, int> kvp) => "&" + MetadataWriter.Naming.ForIl2CppType(kvp.Key.Type, kvp.Key.Attrs));
		}

		private string WritePointerDataValue(PointerType pointerType)
		{
			base.Writer.WriteExternForIl2CppType(pointerType.ElementType);
			return "(void*)&" + MetadataWriter.Naming.ForIl2CppType(pointerType.ElementType, 0);
		}

		private string WriteGenericInstanceTypeDataValue(GenericInstanceType genericInstanceType, IMetadataCollection metadataCollection)
		{
			new GenericClassWriter(base.Writer).WriteDefinition(genericInstanceType, metadataCollection);
			return "&" + MetadataWriter.Naming.ForGenericClass(genericInstanceType);
		}

		private string WriteArrayDataValue(ArrayType arrayType)
		{
			base.Writer.WriteExternForIl2CppType(arrayType.ElementType);
			string result;
			if (arrayType.Rank == 1)
			{
				result = "(void*)&" + MetadataWriter.Naming.ForIl2CppType(arrayType.ElementType, 0);
			}
			else
			{
				base.WriteLine("Il2CppArrayType {0} = ", new object[]
				{
					MetadataWriter.Naming.ForArrayType(arrayType)
				});
				base.WriteArrayInitializer(new string[]
				{
					string.Format("&{0}", MetadataWriter.Naming.ForIl2CppType(arrayType.ElementType, 0)),
					arrayType.Rank.ToString(),
					0.ToString(),
					0.ToString(),
					MetadataWriter.Naming.Null,
					MetadataWriter.Naming.Null
				}, MetadataWriter.ArrayTerminator.None);
				result = "&" + MetadataWriter.Naming.ForArrayType(arrayType);
			}
			return result;
		}
	}
}
