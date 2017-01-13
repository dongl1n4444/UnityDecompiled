namespace Unity.Serialization.Weaver
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal class CStringStore
    {
        [CompilerGenerated]
        private static Func<TypeDefinition, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<TypeDefinition, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<TypeDefinition, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<MethodDefinition, bool> <>f__am$cache4;
        private readonly List<byte> fieldNames = new List<byte>();
        private readonly FieldDefinition fieldNamesField;
        private readonly TypeDefinition fieldNamesStorageType;
        private readonly TypeDefinition fieldNamesType;
        private static Dictionary<ModuleDefinition, CStringStore> instances = new Dictionary<ModuleDefinition, CStringStore>();
        private readonly ModuleDefinition module;
        private readonly Dictionary<string, int> stringOffsets = new Dictionary<string, int>();

        private CStringStore(ModuleDefinition module)
        {
            this.module = module;
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<TypeDefinition, bool>(CStringStore.<CStringStore>m__0);
            }
            TypeReference baseType = module.ImportReference(module.TypeSystem.Object.Resolve().Module.Types.First<TypeDefinition>(<>f__am$cache0));
            this.fieldNamesStorageType = new TypeDefinition("UnityEngine.Internal", "$FieldNamesStorage", TypeAttributes.Abstract | TypeAttributes.Sealed, module.TypeSystem.Object);
            this.fieldNamesType = new TypeDefinition("UnityEngine.Internal", "$FieldNames", TypeAttributes.AnsiClass | TypeAttributes.ExplicitLayout | TypeAttributes.NestedAssembly | TypeAttributes.Sealed, baseType);
            this.fieldNamesType.ClassSize = 1;
            this.fieldNamesType.PackingSize = 1;
            module.Types.Add(this.fieldNamesStorageType);
            this.fieldNamesStorageType.NestedTypes.Add(this.fieldNamesType);
            this.fieldNamesField = new FieldDefinition("$RuntimeNames", FieldAttributes.Assembly | FieldAttributes.Static, new ArrayType(module.TypeSystem.Byte));
            this.fieldNamesStorageType.Fields.Add(this.fieldNamesField);
        }

        [CompilerGenerated]
        private static bool <CStringStore>m__0(TypeDefinition t) => 
            (t.FullName == "System.ValueType");

        private int GetOffsetForString(string str)
        {
            int count;
            if (!this.stringOffsets.TryGetValue(str, out count))
            {
                count = this.fieldNames.Count;
                this.stringOffsets.Add(str, count);
                this.fieldNames.AddRange(Encoding.UTF8.GetBytes(str));
                this.fieldNames.Add(0);
            }
            return count;
        }

        private static FieldDefinition GetStorageField(ModuleDefinition module)
        {
            CStringStore store;
            if (!instances.TryGetValue(module, out store))
            {
                store = new CStringStore(module);
                instances.Add(module, store);
            }
            return store.fieldNamesField;
        }

        public static int GetStringOffset(ModuleDefinition module, string str)
        {
            CStringStore store;
            if (!instances.TryGetValue(module, out store))
            {
                store = new CStringStore(module);
                instances.Add(module, store);
            }
            return store.GetOffsetForString(str);
        }

        private void SaveCStringStore()
        {
            if (this.fieldNames.Count == 0)
            {
                this.fieldNames.Add(0);
            }
            this.fieldNamesType.ClassSize = this.fieldNames.Count;
            FieldDefinition item = new FieldDefinition("$RVAStaticNames", FieldAttributes.Assembly | FieldAttributes.HasFieldRVA | FieldAttributes.InitOnly | FieldAttributes.Static, this.fieldNamesType) {
                InitialValue = this.fieldNames.ToArray()
            };
            this.fieldNamesStorageType.Fields.Add(item);
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = t => t.FullName == "System.Runtime.CompilerServices.RuntimeHelpers";
            }
            TypeDefinition definition2 = this.module.TypeSystem.Object.Resolve().Module.Types.First<TypeDefinition>(<>f__am$cache1);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = m => (((m.Name == "InitializeArray") && (m.Parameters.Count == 2)) && (m.Parameters[0].ParameterType.FullName == "System.Array")) && (m.Parameters[1].ParameterType.FullName == "System.RuntimeFieldHandle");
            }
            MethodReference method = this.module.ImportReference(definition2.Methods.Single<MethodDefinition>(<>f__am$cache2));
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = t => t.FullName == "<Module>";
            }
            TypeDefinition definition3 = this.module.Types.First<TypeDefinition>(<>f__am$cache3);
            bool flag = true;
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = m => ((m.Name == ".cctor") && (((ushort) (m.Attributes & (MethodAttributes.CompilerControlled | MethodAttributes.Static))) != 0)) && (((ushort) (m.Attributes & (MethodAttributes.CompilerControlled | MethodAttributes.RTSpecialName))) != 0);
            }
            MethodDefinition definition4 = definition3.Methods.FirstOrDefault<MethodDefinition>(<>f__am$cache4);
            if (definition4 == null)
            {
                flag = false;
                definition4 = new MethodDefinition(".cctor", MethodAttributes.CompilerControlled | MethodAttributes.Private | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static, this.module.TypeSystem.Void);
                definition3.Methods.Add(definition4);
            }
            List<Instruction> list = new List<Instruction> {
                Instruction.Create(OpCodes.Ldc_I4, this.fieldNames.Count),
                Instruction.Create(OpCodes.Newarr, this.module.TypeSystem.Byte),
                Instruction.Create(OpCodes.Dup),
                Instruction.Create(OpCodes.Ldtoken, item),
                Instruction.Create(OpCodes.Call, method),
                Instruction.Create(OpCodes.Stsfld, this.fieldNamesField)
            };
            if (!flag)
            {
                list.Add(Instruction.Create(OpCodes.Ret));
            }
            for (int i = list.Count - 1; i > -1; i--)
            {
                definition4.Body.Instructions.Insert(0, list[i]);
            }
            definition4.Body.OptimizeMacros();
        }

        public static void SaveCStringStore(ModuleDefinition module)
        {
            CStringStore store;
            if (!instances.TryGetValue(module, out store))
            {
                store = new CStringStore(module);
                instances.Add(module, store);
            }
            store.SaveCStringStore();
        }

        internal static void StoreStoragePointerIntoLocalVariable(ILProcessor processor, LocalVariable fieldNamesVariable, ModuleDefinition module)
        {
            Instruction[] instructionArray = new Instruction[] { Instruction.Create(OpCodes.Ldsfld, GetStorageField(module)), Instruction.Create(OpCodes.Ldc_I4_0), Instruction.Create(OpCodes.Ldelema, module.TypeSystem.Byte), Instruction.Create(OpCodes.Stloc, processor.Body.Variables[fieldNamesVariable.Index]) };
            for (int i = instructionArray.Length - 1; i > -1; i--)
            {
                processor.Body.Instructions.Insert(0, instructionArray[i]);
            }
        }
    }
}

