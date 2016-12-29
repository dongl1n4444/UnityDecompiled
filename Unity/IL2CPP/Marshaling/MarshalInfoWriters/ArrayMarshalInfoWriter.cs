namespace Unity.IL2CPP.Marshaling.MarshalInfoWriters
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.IL2CPP;
    using Unity.IL2CPP.Common;
    using Unity.IL2CPP.Marshaling;

    public abstract class ArrayMarshalInfoWriter : MarshalableMarshalInfoWriter
    {
        protected readonly string _arrayMarshaledTypeName;
        protected readonly int _arraySize;
        protected readonly ArraySizeOptions _arraySizeSelection;
        protected readonly ArrayType _arrayType;
        protected readonly TypeReference _elementType;
        protected readonly DefaultMarshalInfoWriter _elementTypeMarshalInfoWriter;
        private readonly MarshaledType[] _marshaledTypes;
        protected readonly MarshalInfo _marshalInfo;
        protected readonly MarshalType _marshalType;
        protected readonly NativeType _nativeElementType;
        protected readonly int _sizeParameterIndex;
        [CompilerGenerated]
        private static Action<CppCodeWriter> <>f__am$cache0;

        protected ArrayMarshalInfoWriter(ArrayType type, MarshalType marshalType, MarshalInfo marshalInfo) : base(type)
        {
            this._marshalInfo = marshalInfo;
            this._marshalType = marshalType;
            this._arrayType = type;
            this._elementType = type.ElementType;
            MarshalInfo info = null;
            ArrayMarshalInfo info2 = marshalInfo as ArrayMarshalInfo;
            FixedArrayMarshalInfo info3 = marshalInfo as FixedArrayMarshalInfo;
            this._arraySize = 1;
            this._nativeElementType = NativeType.None;
            if (info2 != null)
            {
                this._arraySize = info2.Size;
                this._sizeParameterIndex = info2.SizeParameterIndex;
                if (CodeGenOptions.Dotnetprofile == DotNetProfile.Net45)
                {
                    if ((this._arraySize == 0) || ((this._arraySize == -1) && (this._sizeParameterIndex >= 0)))
                    {
                        this._arraySizeSelection = ArraySizeOptions.UseSizeParameterIndex;
                    }
                    else
                    {
                        this._arraySizeSelection = ArraySizeOptions.UseArraySize;
                    }
                }
                else
                {
                    this._arraySizeSelection = (this._arraySize != 0) ? ArraySizeOptions.UseArraySize : ArraySizeOptions.UseSizeParameterIndex;
                }
                this._nativeElementType = info2.ElementType;
                info = new MarshalInfo(this._nativeElementType);
            }
            else if (info3 != null)
            {
                this._arraySize = info3.Size;
                this._nativeElementType = info3.ElementType;
                info = new MarshalInfo(this._nativeElementType);
            }
            if (this._arraySize == -1)
            {
                this._arraySize = 1;
            }
            this._elementTypeMarshalInfoWriter = MarshalDataCollector.MarshalInfoWriterFor(this._elementType, marshalType, info, false, false, true, null);
            if (this._elementTypeMarshalInfoWriter.MarshaledTypes.Length > 1)
            {
                throw new InvalidOperationException($"ArrayMarshalInfoWriter cannot marshal arrays of {this._elementType.FullName}.");
            }
            this._arrayMarshaledTypeName = this._elementTypeMarshalInfoWriter.MarshaledTypes[0].DecoratedName + "*";
            if (marshalType == MarshalType.WindowsRuntime)
            {
                string name = DefaultMarshalInfoWriter.Naming.ForVariable(DefaultMarshalInfoWriter.TypeProvider.UInt32TypeReference);
                this._arraySizeSelection = ArraySizeOptions.UseFirstMarshaledType;
                this._marshaledTypes = new MarshaledType[] { new MarshaledType(name, name, "ArraySize"), new MarshaledType(this._arrayMarshaledTypeName, this._arrayMarshaledTypeName) };
            }
            else
            {
                this._marshaledTypes = new MarshaledType[] { new MarshaledType(this._arrayMarshaledTypeName, this._arrayMarshaledTypeName) };
            }
            StringMarshalInfoWriter writer = this._elementTypeMarshalInfoWriter as StringMarshalInfoWriter;
            if (writer != null)
            {
                this._nativeElementType = writer.NativeType;
            }
        }

        protected void AllocateAndStoreManagedArray(CppCodeWriter writer, ManagedMarshalValue destinationVariable, IRuntimeMetadataAccess metadataAccess, string arraySizeVariable)
        {
            object[] args = new object[] { DefaultMarshalInfoWriter.Naming.ForVariable(this._arrayType), metadataAccess.TypeInfoFor(this._arrayType), arraySizeVariable };
            writer.WriteLine(destinationVariable.Store("reinterpret_cast<{0}>(SZArrayNew({1}, {2}))", args));
        }

        protected void AllocateAndStoreNativeArray(CppCodeWriter writer, string destinationVariable, string arraySize)
        {
            if (this.NeedsTrailingNullElement)
            {
                object[] args = new object[] { destinationVariable, this._elementTypeMarshalInfoWriter.MarshaledTypes[0].DecoratedName, arraySize };
                writer.WriteLine("{0} = il2cpp_codegen_marshal_allocate_array<{1}>({2} + 1);", args);
                object[] objArray2 = new object[] { destinationVariable, arraySize, DefaultMarshalInfoWriter.Naming.Null };
                writer.WriteLine("({0})[{1}] = {2};", objArray2);
            }
            else
            {
                object[] objArray3 = new object[] { destinationVariable, this._elementTypeMarshalInfoWriter.MarshaledTypes[0].DecoratedName, arraySize };
                writer.WriteLine("{0} = il2cpp_codegen_marshal_allocate_array<{1}>({2});", objArray3);
            }
        }

        public override bool CanMarshalTypeFromNative() => 
            this._elementTypeMarshalInfoWriter.CanMarshalTypeFromNative();

        public override bool CanMarshalTypeToNative() => 
            this._elementTypeMarshalInfoWriter.CanMarshalTypeToNative();

        public override string GetMarshalingException() => 
            this._elementTypeMarshalInfoWriter.GetMarshalingException();

        protected string MarshaledArraySizeFor(string nativeArray, IList<MarshaledParameter> methodParameters)
        {
            switch (this._arraySizeSelection)
            {
                case ArraySizeOptions.UseArraySize:
                    return this._arraySize.ToString(CultureInfo.InvariantCulture);

                case ArraySizeOptions.UseSizeParameterIndex:
                {
                    MarshaledParameter parameter = methodParameters[this._sizeParameterIndex];
                    if (parameter.ParameterType.MetadataType == MetadataType.Int32)
                    {
                        return parameter.NameInGeneratedCode;
                    }
                    return $"static_cast<int32_t>({parameter.NameInGeneratedCode})";
                }
                case ArraySizeOptions.UseFirstMarshaledType:
                    return $"static_cast<int32_t>({nativeArray}{this.MarshaledTypes[0].VariableName})";
            }
            throw new InvalidOperationException($"Unknown ArraySizeOptions: {this._arraySizeSelection}");
        }

        protected string WriteArraySizeFromManagedArray(CppCodeWriter writer, ManagedMarshalValue managedArray, string nativeArray)
        {
            string str;
            if (this._arraySizeSelection != ArraySizeOptions.UseFirstMarshaledType)
            {
                str = $"_{managedArray.GetNiceName()}_Length";
                object[] objArray1 = new object[] { str, managedArray.Load() };
                writer.WriteLine("int32_t {0} = ({1})->max_length;", objArray1);
                return str;
            }
            str = nativeArray + this._marshaledTypes[0].VariableName;
            object[] args = new object[] { str, managedArray.Load() };
            writer.WriteLine("{0} = static_cast<uint32_t>(({1})->max_length);", args);
            return $"static_cast<int32_t>({str})";
        }

        protected void WriteAssignNullArray(CppCodeWriter writer, string destinationVariable)
        {
            if (this._arraySizeSelection == ArraySizeOptions.UseFirstMarshaledType)
            {
                object[] objArray1 = new object[] { destinationVariable, this._marshaledTypes[0].VariableName, DefaultMarshalInfoWriter.Naming.Null };
                writer.WriteLine("{0}{1} = {2};", objArray1);
            }
            object[] args = new object[] { destinationVariable, DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("{0} = {1};", args);
        }

        protected void WriteCleanupLoop(CppCodeWriter outerWriter, string variableName, IRuntimeMetadataAccess metadataAccess, Func<CppCodeWriter, string> writeLoopCountVariable)
        {
            <WriteCleanupLoop>c__AnonStorey6 storey = new <WriteCleanupLoop>c__AnonStorey6 {
                variableName = variableName,
                metadataAccess = metadataAccess,
                $this = this
            };
            this.WriteLoop(outerWriter, writeLoopCountVariable, new Action<CppCodeWriter>(storey.<>m__0));
        }

        public override void WriteIncludesForFieldDeclaration(CppCodeWriter writer)
        {
            this._elementTypeMarshalInfoWriter.WriteMarshaledTypeForwardDeclaration(writer);
        }

        public override void WriteIncludesForMarshaling(CppCodeWriter writer)
        {
            writer.AddIncludeForTypeDefinition(this._arrayType);
            this._elementTypeMarshalInfoWriter.WriteIncludesForMarshaling(writer);
            base.WriteIncludesForMarshaling(writer);
        }

        private void WriteLoop(CppCodeWriter outerWriter, Func<CppCodeWriter, string> writeLoopCountVariable, Action<CppCodeWriter> writeLoopBody)
        {
            <WriteLoop>c__AnonStorey3 storey = new <WriteLoop>c__AnonStorey3 {
                writeLoopCountVariable = writeLoopCountVariable
            };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = bodyWriter => bodyWriter.EndBlock(false);
            }
            outerWriter.WriteIfNotEmpty(new Action<CppCodeWriter>(storey.<>m__0), writeLoopBody, <>f__am$cache0);
        }

        public abstract override void WriteMarshalCleanupVariable(CppCodeWriter writer, string variableName, IRuntimeMetadataAccess metadataAccess, string managedVariableName = null);
        public override void WriteMarshaledTypeForwardDeclaration(CppCodeWriter writer)
        {
            this._elementTypeMarshalInfoWriter.WriteMarshaledTypeForwardDeclaration(writer);
        }

        public override string WriteMarshalEmptyVariableFromNative(CppCodeWriter writer, string variableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalEmptyVariableFromNative>c__AnonStorey2 storey = new <WriteMarshalEmptyVariableFromNative>c__AnonStorey2 {
                methodParameters = methodParameters,
                metadataAccess = metadataAccess,
                $this = this,
                emptyVariableName = $"_{DefaultMarshalInfoWriter.CleanVariableName(variableName)}_empty"
            };
            ManagedMarshalValue value2 = new ManagedMarshalValue(storey.emptyVariableName);
            writer.WriteVariable(base._typeRef, storey.emptyVariableName);
            object[] args = new object[] { variableName, DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("if ({0} != {1})", args);
            using (new BlockWriter(writer, false))
            {
                <WriteMarshalEmptyVariableFromNative>c__AnonStorey1 storey2 = new <WriteMarshalEmptyVariableFromNative>c__AnonStorey1 {
                    <>f__ref$2 = storey,
                    arraySize = this.MarshaledArraySizeFor(variableName, storey.methodParameters)
                };
                object[] objArray2 = new object[] { DefaultMarshalInfoWriter.Naming.ForVariable(this._arrayType), storey.metadataAccess.TypeInfoFor(this._arrayType), storey2.arraySize };
                writer.WriteLine(value2.Store("reinterpret_cast<{0}>(SZArrayNew({1}, {2}));", objArray2));
                this.WriteLoop(writer, new Func<CppCodeWriter, string>(storey2, (IntPtr) this.<>m__0), new Action<CppCodeWriter>(storey2.<>m__1));
            }
            return storey.emptyVariableName;
        }

        public override string WriteMarshalEmptyVariableToNative(CppCodeWriter writer, ManagedMarshalValue variableName, IList<MarshaledParameter> methodParameters)
        {
            string str = $"_{variableName.GetNiceName()}_marshaled";
            this.WriteNativeVariableDeclarationOfType(writer, str);
            object[] args = new object[] { variableName.Load(), DefaultMarshalInfoWriter.Naming.Null };
            writer.WriteLine("if ({0} != {1})", args);
            using (new BlockWriter(writer, false))
            {
                string str2 = this.WriteArraySizeFromManagedArray(writer, variableName, str);
                string str3 = !this.NeedsTrailingNullElement ? str2 : $"({str2} + 1)";
                object[] objArray2 = new object[] { str, this._elementTypeMarshalInfoWriter.MarshaledTypes[0].DecoratedName, str3 };
                writer.WriteLine("{0} = il2cpp_codegen_marshal_allocate_array<{1}>({2});", objArray2);
                object[] objArray3 = new object[] { str, str3, this._elementTypeMarshalInfoWriter.MarshaledTypes[0].DecoratedName };
                writer.WriteLine("memset({0}, 0, {1} * sizeof({2}));", objArray3);
            }
            return str;
        }

        protected void WriteMarshalFromNativeLoop(CppCodeWriter outerWriter, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess, Func<CppCodeWriter, string> writeLoopCountVariable)
        {
            <WriteMarshalFromNativeLoop>c__AnonStorey5 storey = new <WriteMarshalFromNativeLoop>c__AnonStorey5 {
                variableName = variableName,
                methodParameters = methodParameters,
                returnValue = returnValue,
                forNativeWrapperOfManagedMethod = forNativeWrapperOfManagedMethod,
                metadataAccess = metadataAccess,
                destinationVariable = destinationVariable,
                $this = this
            };
            this.WriteLoop(outerWriter, writeLoopCountVariable, new Action<CppCodeWriter>(storey.<>m__0));
        }

        public abstract override void WriteMarshalOutParameterFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess);
        public override void WriteMarshalOutParameterToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IList<MarshaledParameter> methodParameters, IRuntimeMetadataAccess metadataAccess)
        {
            <WriteMarshalOutParameterToNative>c__AnonStorey0 storey = new <WriteMarshalOutParameterToNative>c__AnonStorey0 {
                destinationVariable = destinationVariable,
                methodParameters = methodParameters,
                $this = this
            };
            if (this._marshalType == MarshalType.WindowsRuntime)
            {
                object[] args = new object[] { sourceVariable.Load(), DefaultMarshalInfoWriter.Naming.Null };
                writer.WriteLine("if ({0} != {1})", args);
                using (new BlockWriter(writer, false))
                {
                    this.WriteMarshalToNativeLoop(writer, sourceVariable, storey.destinationVariable, managedVariableName, metadataAccess, new Func<CppCodeWriter, string>(storey, (IntPtr) this.<>m__0));
                }
                writer.WriteLine("else");
                using (new BlockWriter(writer, false))
                {
                    this.WriteAssignNullArray(writer, storey.destinationVariable);
                }
            }
        }

        protected void WriteMarshalToNativeLoop(CppCodeWriter outerWriter, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess, Func<CppCodeWriter, string> writeLoopCountVariable)
        {
            <WriteMarshalToNativeLoop>c__AnonStorey4 storey = new <WriteMarshalToNativeLoop>c__AnonStorey4 {
                sourceVariable = sourceVariable,
                destinationVariable = destinationVariable,
                managedVariableName = managedVariableName,
                metadataAccess = metadataAccess,
                $this = this
            };
            this.WriteLoop(outerWriter, writeLoopCountVariable, new Action<CppCodeWriter>(storey.<>m__0));
        }

        public abstract override void WriteMarshalVariableFromNative(CppCodeWriter writer, string variableName, ManagedMarshalValue destinationVariable, IList<MarshaledParameter> methodParameters, bool returnValue, bool forNativeWrapperOfManagedMethod, IRuntimeMetadataAccess metadataAccess);
        public abstract override void WriteMarshalVariableToNative(CppCodeWriter writer, ManagedMarshalValue sourceVariable, string destinationVariable, string managedVariableName, IRuntimeMetadataAccess metadataAccess);

        public override MarshaledType[] MarshaledTypes =>
            this._marshaledTypes;

        public override string NativeSize =>
            "-1";

        protected bool NeedsTrailingNullElement =>
            ((this._elementTypeMarshalInfoWriter is StringMarshalInfoWriter) && (this._marshalType != MarshalType.WindowsRuntime));

        [CompilerGenerated]
        private sealed class <WriteCleanupLoop>c__AnonStorey6
        {
            internal ArrayMarshalInfoWriter $this;
            internal IRuntimeMetadataAccess metadataAccess;
            internal string variableName;

            internal void <>m__0(CppCodeWriter bodyWriter)
            {
                this.$this._elementTypeMarshalInfoWriter.WriteMarshalCleanupVariable(bodyWriter, this.$this._elementTypeMarshalInfoWriter.UndecorateVariable($"({this.variableName})[i]"), this.metadataAccess, null);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteLoop>c__AnonStorey3
        {
            internal Func<CppCodeWriter, string> writeLoopCountVariable;

            internal void <>m__0(CppCodeWriter bodyWriter)
            {
                object[] args = new object[] { this.writeLoopCountVariable.Invoke(bodyWriter) };
                bodyWriter.WriteLine("for (int32_t i = 0; i < {0}; i++)", args);
                bodyWriter.BeginBlock();
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalEmptyVariableFromNative>c__AnonStorey1
        {
            internal ArrayMarshalInfoWriter.<WriteMarshalEmptyVariableFromNative>c__AnonStorey2 <>f__ref$2;
            internal string arraySize;

            internal string <>m__0(CppCodeWriter bodyWriter) => 
                this.arraySize;

            internal void <>m__1(CppCodeWriter bodyWriter)
            {
                string str = this.<>f__ref$2.$this._elementTypeMarshalInfoWriter.WriteMarshalEmptyVariableFromNative(bodyWriter, "_item", this.<>f__ref$2.methodParameters, this.<>f__ref$2.metadataAccess);
                object[] args = new object[] { Emit.StoreArrayElement(this.<>f__ref$2.emptyVariableName, "i", str, false) };
                bodyWriter.WriteLine("{0};", args);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalEmptyVariableFromNative>c__AnonStorey2
        {
            internal ArrayMarshalInfoWriter $this;
            internal string emptyVariableName;
            internal IRuntimeMetadataAccess metadataAccess;
            internal IList<MarshaledParameter> methodParameters;
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalFromNativeLoop>c__AnonStorey5
        {
            internal ArrayMarshalInfoWriter $this;
            internal ManagedMarshalValue destinationVariable;
            internal bool forNativeWrapperOfManagedMethod;
            internal IRuntimeMetadataAccess metadataAccess;
            internal IList<MarshaledParameter> methodParameters;
            internal bool returnValue;
            internal string variableName;

            internal void <>m__0(CppCodeWriter bodyWriter)
            {
                string variableName = this.$this._elementTypeMarshalInfoWriter.UndecorateVariable($"({this.variableName})[i]");
                string str2 = this.$this._elementTypeMarshalInfoWriter.WriteMarshalVariableFromNative(bodyWriter, variableName, this.methodParameters, this.returnValue, this.forNativeWrapperOfManagedMethod, this.metadataAccess);
                object[] args = new object[] { Emit.StoreArrayElement(this.destinationVariable.Load(), "i", str2, false) };
                bodyWriter.WriteLine("{0};", args);
            }
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalOutParameterToNative>c__AnonStorey0
        {
            internal ArrayMarshalInfoWriter $this;
            internal string destinationVariable;
            internal IList<MarshaledParameter> methodParameters;

            internal string <>m__0(CppCodeWriter bodyWriter) => 
                this.$this.MarshaledArraySizeFor(this.destinationVariable, this.methodParameters);
        }

        [CompilerGenerated]
        private sealed class <WriteMarshalToNativeLoop>c__AnonStorey4
        {
            internal ArrayMarshalInfoWriter $this;
            internal string destinationVariable;
            internal string managedVariableName;
            internal IRuntimeMetadataAccess metadataAccess;
            internal ManagedMarshalValue sourceVariable;

            internal void <>m__0(CppCodeWriter bodyWriter)
            {
                this.$this._elementTypeMarshalInfoWriter.WriteMarshalVariableToNative(bodyWriter, new ManagedMarshalValue(this.sourceVariable, "i"), this.$this._elementTypeMarshalInfoWriter.UndecorateVariable($"({this.destinationVariable})[i]"), this.managedVariableName, this.metadataAccess);
            }
        }

        protected enum ArraySizeOptions
        {
            UseArraySize,
            UseSizeParameterIndex,
            UseFirstMarshaledType
        }
    }
}

