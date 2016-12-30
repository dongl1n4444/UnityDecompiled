namespace Unity.IL2CPP.IoCServices
{
    using Mono.Cecil.Cil;
    using System;
    using Unity.IL2CPP;

    public interface ISourceAnnotationWriter
    {
        void EmitAnnotation(CppCodeWriter writer, SequencePoint sequencePoint);
    }
}

