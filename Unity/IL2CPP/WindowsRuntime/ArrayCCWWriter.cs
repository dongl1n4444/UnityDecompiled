namespace Unity.IL2CPP.WindowsRuntime
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Unity.IL2CPP;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    internal sealed class ArrayCCWWriter : CCWWriterBase
    {
        private readonly string _baseTypeName;
        private readonly List<TypeReference> _implementedInterfaces;
        private readonly ArrayType _type;
        private readonly string _typeName;
        [Inject]
        public static IStatsService Stats;
        [Inject]
        public static IWindowsRuntimeProjections WindowsRuntimeProjections;

        public ArrayCCWWriter(ArrayType type) : base(type)
        {
            this._implementedInterfaces = new List<TypeReference>();
            this._type = type;
            this._typeName = CCWWriterBase.Naming.ForComCallableWrapperClass(type);
            StringBuilder builder = new StringBuilder();
            builder.Append($"il2cpp::vm::CachedCCWBase<{this._typeName}>");
            if (WindowsRuntimeProjections.HasIEnumerableCCW)
            {
                this._implementedInterfaces.Add(CCWWriterBase.TypeProvider.IBindableIterableTypeReference);
                builder.Append($", {CCWWriterBase.Naming.ForComCallableWrapperClass(CCWWriterBase.TypeProvider.IBindableIterableTypeReference)}<{this._typeName}>");
            }
            foreach (TypeReference reference in type.GetWindowsRuntimeCovariantTypes())
            {
                TypeReference item = WindowsRuntimeProjections.ProjectToWindowsRuntime(reference);
                this._implementedInterfaces.Add(item);
                builder.Append($", {CCWWriterBase.Naming.ForComCallableWrapperClass(item)}<{this._typeName}>");
            }
            this._baseTypeName = builder.ToString();
        }

        public static bool IsArrayCCWSupported() => 
            ((CCWWriterBase.TypeProvider.IIterableTypeReference != null) || (WindowsRuntimeProjections.HasIEnumerableCCW && (CCWWriterBase.TypeProvider.IBindableIterableTypeReference != null)));

        public void Write(CppCodeWriter writer)
        {
            writer.AddInclude("vm/CachedCCWBase.h");
            base.AddIncludes(writer);
            writer.WriteLine();
            writer.WriteCommentedLine("COM Callable Wrapper for " + this._type.FullName);
            writer.WriteLine($"struct {this._typeName} IL2CPP_FINAL : {this._baseTypeName}");
            using (new BlockWriter(writer, true))
            {
                writer.WriteLine($"inline {this._typeName}(Il2CppObject* obj) : il2cpp::vm::CachedCCWBase<{this._typeName}>(obj) {{}}");
                base.WriteCommonInterfaceMethods(writer);
            }
            Stats.RecordArrayComCallableWrapper();
        }

        protected override IEnumerable<TypeReference> AllImplementedInterfaces =>
            this._implementedInterfaces;

        protected override bool ImplementsAnyIInspectableInterfaces =>
            true;
    }
}

